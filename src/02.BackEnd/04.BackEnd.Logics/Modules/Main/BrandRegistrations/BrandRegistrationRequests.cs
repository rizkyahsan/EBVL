using EBVL.BackEnd.Services.CurrentUser;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.AssignBrandRegistration;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.Common;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.CreateBrandRegistration;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.GetBrandRegistrations;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.RespondBrandInvitation;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.SendBrandInvitation;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.TransitionBrandRegistration;
using EBVL.Shared.Statics;
using MainPermissions = EBVL.Shared.Dto.Modules.Main.Permissions;

namespace EBVL.BackEnd.Logics.Modules.Main.BrandRegistrations;

[AuthorizeRequest]
public sealed record CreateBrandRegistrationCommand(CreateBrandRegistrationRequest Request) : IRequest<BrandRegistrationItem>;
[AuthorizeRequest]
public sealed record GetBrandRegistrationQuery(Guid RegistrationId) : IRequest<BrandRegistrationItem>;
[AuthorizeRequest]
public sealed record GetBrandRegistrationsQuery : IRequest<GetBrandRegistrationsResponse>;
[AuthorizeRequest]
public sealed record TransitionBrandRegistrationCommand(Guid RegistrationId, TransitionBrandRegistrationRequest Request) : IRequest<BrandRegistrationItem>;
[AuthorizeRequest]
public sealed record AssignBrandRegistrationCommand(Guid RegistrationId, AssignBrandRegistrationRequest Request) : IRequest<BrandRegistrationItem>;
[AuthorizeRequest]
public sealed record SendBrandInvitationCommand(Guid RegistrationId, SendBrandInvitationRequest Request) : IRequest<BrandRegistrationItem>;
[AuthorizeRequest]
public sealed record RespondBrandInvitationCommand(Guid RegistrationId, Guid InvitationId, RespondBrandInvitationRequest Request) : IRequest<BrandRegistrationItem>;

public sealed class CreateBrandRegistrationCommandValidator : AbstractValidator<CreateBrandRegistrationCommand>
{
    public CreateBrandRegistrationCommandValidator()
    {
        _ = RuleFor(x => x.Request).SetValidator(new CreateBrandRegistrationRequestValidator());
    }
}

public sealed class TransitionBrandRegistrationCommandValidator : AbstractValidator<TransitionBrandRegistrationCommand>
{
    public TransitionBrandRegistrationCommandValidator()
    {
        _ = RuleFor(x => x.RegistrationId).NotEmpty();
        _ = RuleFor(x => x.Request).SetValidator(new TransitionBrandRegistrationRequestValidator());
    }
}

public sealed class AssignBrandRegistrationCommandValidator : AbstractValidator<AssignBrandRegistrationCommand>
{
    public AssignBrandRegistrationCommandValidator()
    {
        _ = RuleFor(x => x.RegistrationId).NotEmpty();
        _ = RuleFor(x => x.Request).SetValidator(new AssignBrandRegistrationRequestValidator());
    }
}

public sealed class SendBrandInvitationCommandValidator : AbstractValidator<SendBrandInvitationCommand>
{
    public SendBrandInvitationCommandValidator()
    {
        _ = RuleFor(x => x.RegistrationId).NotEmpty();
        _ = RuleFor(x => x.Request).SetValidator(new SendBrandInvitationRequestValidator());
    }
}

public sealed class RespondBrandInvitationCommandValidator : AbstractValidator<RespondBrandInvitationCommand>
{
    public RespondBrandInvitationCommandValidator()
    {
        _ = RuleFor(x => x.RegistrationId).NotEmpty();
        _ = RuleFor(x => x.InvitationId).NotEmpty();
        _ = RuleFor(x => x.Request).SetValidator(new RespondBrandInvitationRequestValidator());
    }
}

public sealed class CreateBrandRegistrationHandler(IDatabaseService databaseService, IRequestActor actor)
    : IRequestHandler<CreateBrandRegistrationCommand, BrandRegistrationItem>
{
    public async Task<BrandRegistrationItem> Handle(CreateBrandRegistrationCommand command, CancellationToken cancellationToken)
    {
        BrandRegistrationRuntime.RequireScope(actor, MainPermissions.BrandRegistrationCreate);
        if (command.Request.Submit)
        {
            BrandRegistrationRuntime.RequireScope(actor, MainPermissions.BrandRegistrationSubmit);
        }

        var now = DateTimeOffset.UtcNow;
        var workflowCase = new WorkflowCase { ProcessType = nameof(BrandRegistration), Status = BrandRegistrationStatus.Draft.ToString(), StartedAt = now };
        var registration = new BrandRegistration
        {
            WorkflowCase = workflowCase,
            OwnerUsername = actor.Username,
            BrandName = command.Request.BrandName.Trim(),
            ProductName = command.Request.ProductName.Trim(),
            Group = command.Request.Group.Trim(),
            FactoryCountry = command.Request.FactoryCountry.Trim(),
            Category = command.Request.Category.Trim(),
            ProductDescription = command.Request.ProductDescription.Trim()
        };
        if (command.Request.Submit)
        {
            BrandRegistrationRuntime.ValidateSubmission(registration);
            var rule = BrandRegistrationWorkflow.GetRule(BrandRegistrationStatus.Draft, BrandRegistrationAction.Submit);
            BrandRegistrationRuntime.ApplyTransition(registration, actor, BrandRegistrationAction.Submit, rule, null);
            registration.SubmittedAt = now;
            BrandRegistrationRuntime.MoveSubmittedToAssignment(registration, actor);
        }

        _ = await databaseService.BrandRegistrations.AddAsync(registration, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(CreateBrandRegistrationCommand), cancellationToken);
        return await BrandRegistrationRuntime.LoadResponse(databaseService, registration.Id, cancellationToken);
    }
}

public sealed class GetBrandRegistrationHandler(IDatabaseService databaseService, IRequestActor actor)
    : IRequestHandler<GetBrandRegistrationQuery, BrandRegistrationItem>
{
    public async Task<BrandRegistrationItem> Handle(GetBrandRegistrationQuery query, CancellationToken cancellationToken)
    {
        var registration = await BrandRegistrationRuntime.Load(databaseService, query.RegistrationId, false, cancellationToken);
        var hasSelfAccess = actor.Scopes.Contains(MainPermissions.BrandRegistrationSelf)
            && !actor.Scopes.Contains(MainPermissions.BrandRegistrationView);
        BrandRegistrationRuntime.RequireScope(actor, hasSelfAccess
            ? MainPermissions.BrandRegistrationSelf
            : MainPermissions.BrandRegistrationView);
        BrandRegistrationRuntime.RequireView(registration, actor);
        return BrandRegistrationRuntime.Map(registration);
    }
}

public sealed class GetBrandRegistrationsHandler(IDatabaseService databaseService, IRequestActor actor)
    : IRequestHandler<GetBrandRegistrationsQuery, GetBrandRegistrationsResponse>
{
    public async Task<GetBrandRegistrationsResponse> Handle(GetBrandRegistrationsQuery request, CancellationToken cancellationToken)
    {
        var hasSelfAccess = actor.Scopes.Contains(MainPermissions.BrandRegistrationSelf)
            && !actor.Scopes.Contains(MainPermissions.BrandRegistrationIndex);
        BrandRegistrationRuntime.RequireScope(actor, hasSelfAccess
            ? MainPermissions.BrandRegistrationSelf
            : MainPermissions.BrandRegistrationIndex);
        var query = databaseService.BrandRegistrations.AsNoTracking().Include(x => x.WorkflowCase).Where(x => !x.IsDeleted);
        if (hasSelfAccess)
        {
            query = query.Where(x => x.OwnerUsername == actor.Username);
        }

        var entities = await query.OrderByDescending(x => x.Modified ?? x.Created).ToListAsync(cancellationToken);
        return new(entities.Select(x => new BrandRegistrationListItem(x.Id, x.BrandName, x.ProductName, x.Group,
            Enum.Parse<BrandRegistrationStatus>(x.WorkflowCase.Status), x.SubmittedAt, x.Modified ?? x.Created)).ToList());
    }
}

public sealed class TransitionBrandRegistrationHandler(IDatabaseService databaseService, IRequestActor actor)
    : IRequestHandler<TransitionBrandRegistrationCommand, BrandRegistrationItem>
{
    public async Task<BrandRegistrationItem> Handle(TransitionBrandRegistrationCommand command, CancellationToken cancellationToken)
    {
        var registration = await BrandRegistrationRuntime.Load(databaseService, command.RegistrationId, true, cancellationToken);
        var status = Enum.Parse<BrandRegistrationStatus>(registration.WorkflowCase.Status);
        var rule = BrandRegistrationWorkflow.GetRule(status, command.Request.Action);
        BrandRegistrationRuntime.RequireActionScope(actor, status, command.Request.Action);
        BrandRegistrationRuntime.AuthorizeTransition(registration, actor, rule);
        if (command.Request.Action == BrandRegistrationAction.Submit)
        {
            BrandRegistrationRuntime.ValidateSubmission(registration);
        }

        BrandRegistrationRuntime.ApplyRowVersion(databaseService, registration.WorkflowCase, command.Request.RowVersion);
        BrandRegistrationRuntime.ApplyTransition(registration, actor, command.Request.Action, rule, command.Request.Note);
        if (command.Request.Action == BrandRegistrationAction.Submit)
        {
            registration.SubmittedAt = DateTimeOffset.UtcNow;
            BrandRegistrationRuntime.MoveSubmittedToAssignment(registration, actor);
        }

        _ = await databaseService.SaveAsync(nameof(TransitionBrandRegistrationCommand), cancellationToken);
        return await BrandRegistrationRuntime.LoadResponse(databaseService, registration.Id, cancellationToken);
    }
}

public sealed class AssignBrandRegistrationHandler(IDatabaseService databaseService, IRequestActor actor)
    : IRequestHandler<AssignBrandRegistrationCommand, BrandRegistrationItem>
{
    public async Task<BrandRegistrationItem> Handle(AssignBrandRegistrationCommand command, CancellationToken cancellationToken)
    {
        BrandRegistrationRuntime.RequireScope(actor, MainPermissions.BrandRegistrationDisposition);
        var registration = await BrandRegistrationRuntime.Load(databaseService, command.RegistrationId, true, cancellationToken);
        var status = Enum.Parse<BrandRegistrationStatus>(registration.WorkflowCase.Status);
        var rule = BrandRegistrationWorkflow.GetRule(status, BrandRegistrationAction.Assign);
        BrandRegistrationRuntime.AuthorizeTransition(registration, actor, rule);
        BrandRegistrationRuntime.ApplyRowVersion(databaseService, registration.WorkflowCase, command.Request.RowVersion);
        BrandRegistrationRuntime.ApplyTransition(registration, actor, BrandRegistrationAction.Assign, rule, command.Request.Note, command.Request.AssigneeUsername);
        _ = await databaseService.SaveAsync(nameof(AssignBrandRegistrationCommand), cancellationToken);
        return await BrandRegistrationRuntime.LoadResponse(databaseService, registration.Id, cancellationToken);
    }
}

public sealed class SendBrandInvitationHandler(IDatabaseService databaseService, IRequestActor actor)
    : IRequestHandler<SendBrandInvitationCommand, BrandRegistrationItem>
{
    public async Task<BrandRegistrationItem> Handle(SendBrandInvitationCommand command, CancellationToken cancellationToken)
    {
        BrandRegistrationRuntime.RequireScope(actor, MainPermissions.BrandRegistrationInvite);
        var registration = await BrandRegistrationRuntime.Load(databaseService, command.RegistrationId, true, cancellationToken);
        if (new[] { command.Request.Subject, command.Request.Recipient, command.Request.Cc, command.Request.Body }.Any(string.IsNullOrWhiteSpace))
        {
            throw new ValidationException("Subject, recipient, CC, and body are required.");
        }

        var status = Enum.Parse<BrandRegistrationStatus>(registration.WorkflowCase.Status);
        var rule = BrandRegistrationWorkflow.GetRule(status, BrandRegistrationAction.SendInvitation);
        BrandRegistrationRuntime.AuthorizeTransition(registration, actor, rule);
        BrandRegistrationRuntime.ApplyRowVersion(databaseService, registration.WorkflowCase, command.Request.RowVersion);
        var invitation = new BrandInvitation
        {
            BrandRegistration = registration,
            Subject = command.Request.Subject.Trim(),
            Recipient = command.Request.Recipient.Trim(),
            Cc = command.Request.Cc.Trim(),
            Body = command.Request.Body.Trim(),
            SentAt = DateTimeOffset.UtcNow,
            SentBy = actor.Username
        };
        registration.Invitations.Add(invitation);
        BrandRegistrationRuntime.ApplyTransition(registration, actor, BrandRegistrationAction.SendInvitation, rule, null, registration.OwnerUsername);
        _ = await databaseService.SaveAsync(nameof(SendBrandInvitationCommand), cancellationToken);
        return await BrandRegistrationRuntime.LoadResponse(databaseService, registration.Id, cancellationToken);
    }
}

public sealed class RespondBrandInvitationHandler(IDatabaseService databaseService, IRequestActor actor)
    : IRequestHandler<RespondBrandInvitationCommand, BrandRegistrationItem>
{
    public async Task<BrandRegistrationItem> Handle(RespondBrandInvitationCommand command, CancellationToken cancellationToken)
    {
        BrandRegistrationRuntime.RequireScope(actor, MainPermissions.BrandRegistrationRespondInvitation);
        var registration = await BrandRegistrationRuntime.Load(databaseService, command.RegistrationId, true, cancellationToken);
        var invitation = registration.Invitations.SingleOrDefault(x => x.Id == command.InvitationId)
            ?? throw new KeyNotFoundException("Brand invitation was not found.");
        if (invitation.Response.HasValue)
        {
            throw new ValidationException("The invitation has already been answered.");
        }

        var action = command.Request.Response == BrandInvitationResponse.Confirmed
            ? BrandRegistrationAction.ConfirmInvitation
            : BrandRegistrationAction.RejectInvitation;
        var status = Enum.Parse<BrandRegistrationStatus>(registration.WorkflowCase.Status);
        var rule = BrandRegistrationWorkflow.GetRule(status, action);
        BrandRegistrationRuntime.AuthorizeTransition(registration, actor, rule);
        BrandRegistrationRuntime.ApplyRowVersion(databaseService, registration.WorkflowCase, command.Request.RowVersion);
        invitation.Response = command.Request.Response;
        invitation.RespondedAt = DateTimeOffset.UtcNow;
        invitation.RespondedBy = actor.Username;
        BrandRegistrationRuntime.ApplyTransition(registration, actor, action, rule, null);
        _ = await databaseService.SaveAsync(nameof(RespondBrandInvitationCommand), cancellationToken);
        return await BrandRegistrationRuntime.LoadResponse(databaseService, registration.Id, cancellationToken);
    }
}

internal static class BrandRegistrationRuntime
{
    public static async Task<BrandRegistration> Load(IDatabaseService databaseService, Guid id, bool tracking, CancellationToken cancellationToken)
    {
        var query = databaseService.BrandRegistrations
            .Include(x => x.WorkflowCase).ThenInclude(x => x.Assignments)
            .Include(x => x.WorkflowCase).ThenInclude(x => x.Transitions)
            .Include(x => x.Invitations).Where(x => !x.IsDeleted);
        if (!tracking)
        {
            query = query.AsNoTracking();
        }

        return await query.SingleOrDefaultAsync(x => x.Id == id, cancellationToken)
            ?? throw new KeyNotFoundException("Brand registration was not found.");
    }

    public static async Task<BrandRegistrationItem> LoadResponse(IDatabaseService databaseService, Guid id, CancellationToken cancellationToken)
    {
        return Map(await Load(databaseService, id, false, cancellationToken));
    }

    public static BrandRegistrationItem Map(BrandRegistration registration)
    {
        var invitation = registration.Invitations.OrderByDescending(x => x.SentAt).FirstOrDefault();
        return new BrandRegistrationItem(registration.Id, registration.BrandName, registration.ProductName, registration.Group,
            registration.FactoryCountry, registration.Category, registration.ProductDescription,
            Enum.Parse<BrandRegistrationStatus>(registration.WorkflowCase.Status), registration.SubmittedAt,
            registration.Modified ?? registration.Created, Convert.ToBase64String(registration.WorkflowCase.RowVersion),
            registration.WorkflowCase.Transitions.OrderBy(x => x.OccurredAt).Select(x => new WorkflowHistoryItem(x.Id,
                Enum.Parse<BrandRegistrationStatus>(x.FromStatus), Enum.Parse<BrandRegistrationStatus>(x.ToStatus),
                Enum.Parse<BrandRegistrationAction>(x.Action), x.ActorUsername, x.ActorRole, x.Note, x.OccurredAt)).ToList(),
            invitation is null ? null : new BrandInvitationItem(invitation.Id, invitation.Subject, invitation.Recipient,
                invitation.Cc, invitation.Body, invitation.SentAt, invitation.SentBy, invitation.Response,
                invitation.RespondedAt, invitation.RespondedBy));
    }

    public static void RequireView(BrandRegistration registration, IRequestActor actor)
    {
        if (actor.Scopes.Contains(MainPermissions.BrandRegistrationSelf)
            && !actor.Scopes.Contains(MainPermissions.BrandRegistrationView)
            && !registration.OwnerUsername.Equals(actor.Username, StringComparison.OrdinalIgnoreCase))
        {
            throw new UnauthorizedAccessException("The registration belongs to another vendor.");
        }
    }

    public static void RequireRole(IRequestActor actor, string role)
    {
        if (!actor.Roles.Contains(role))
        {
            throw new UnauthorizedAccessException($"Role '{role}' is required.");
        }
    }

    public static void RequireScope(IRequestActor actor, string scope)
    {
        if (!actor.Scopes.Contains(scope))
        {
            throw new UnauthorizedAccessException($"Scope '{scope}' is required.");
        }
    }

    public static void RequireActionScope(IRequestActor actor, BrandRegistrationStatus status, BrandRegistrationAction action)
    {
        var scope = action switch
        {
            BrandRegistrationAction.Submit => MainPermissions.BrandRegistrationSubmit,
            BrandRegistrationAction.StartReview when status is BrandRegistrationStatus.RequestEvaluationBySpecialistSection
                => MainPermissions.BrandRegistrationEvaluate,
            BrandRegistrationAction.StartReview when status is BrandRegistrationStatus.RequestPresentationEvaluationBySpecialist
                => MainPermissions.BrandRegistrationPresentationReview,
            BrandRegistrationAction.StartReview => MainPermissions.BrandRegistrationReview,
            BrandRegistrationAction.Approve => MainPermissions.BrandRegistrationApprove,
            BrandRegistrationAction.NeedRevision or BrandRegistrationAction.Resubmit => MainPermissions.BrandRegistrationRevise,
            BrandRegistrationAction.RaiseObjection or BrandRegistrationAction.ResolveObjection => MainPermissions.BrandRegistrationObject,
            BrandRegistrationAction.StartClosing or BrandRegistrationAction.Close => MainPermissions.BrandRegistrationClose,
            BrandRegistrationAction.Reject => MainPermissions.BrandRegistrationReject,
            BrandRegistrationAction.StartAssignment or BrandRegistrationAction.Assign
                or BrandRegistrationAction.SendInvitation or BrandRegistrationAction.ConfirmInvitation
                or BrandRegistrationAction.RejectInvitation
                => throw new ValidationException($"Action '{action}' must use its dedicated endpoint."),
            _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
        };
        RequireScope(actor, scope);
    }

    public static void AuthorizeTransition(BrandRegistration registration, IRequestActor actor, BrandWorkflowRule rule)
    {
        if (rule.Role == RoleNameFor.Vendor)
        {
            if (!registration.OwnerUsername.Equals(actor.Username, StringComparison.OrdinalIgnoreCase))
            {
                throw new UnauthorizedAccessException("Only the registration owner can perform this action.");
            }
        }
        else
        {
            RequireRole(actor, rule.Role);
        }

        if (rule.RequiresAssignment)
        {
            var assignment = registration.WorkflowCase.Assignments.SingleOrDefault(x => x.EndedAt == null);
            if (assignment is null || assignment.Role != rule.Role
                || (assignment.AssigneeUsername is not null && !assignment.AssigneeUsername.Equals(actor.Username, StringComparison.OrdinalIgnoreCase)))
            {
                throw new UnauthorizedAccessException("The workflow is not assigned to this actor.");
            }
        }
    }

    public static void ApplyRowVersion(IDatabaseService databaseService, WorkflowCase workflowCase, string rowVersion)
    {
        try
        {
            databaseService.SetWorkflowCaseOriginalRowVersion(workflowCase, Convert.FromBase64String(rowVersion));
        }
        catch (FormatException)
        {
            throw new ValidationException("The row version is invalid.");
        }
    }

    public static void ValidateSubmission(BrandRegistration registration)
    {
        if (new[] { registration.BrandName, registration.ProductName, registration.Group, registration.FactoryCountry,
            registration.Category, registration.ProductDescription }.Any(string.IsNullOrWhiteSpace))
        {
            throw new ValidationException("Complete all required Brand Registration fields before submitting.");
        }
    }

    public static void MoveSubmittedToAssignment(BrandRegistration registration, IRequestActor actor)
    {
        var now = DateTimeOffset.UtcNow;
        registration.WorkflowCase.Status = BrandRegistrationStatus.NeedAssignmentBySeniorManagerMsai.ToString();
        registration.WorkflowCase.Transitions.Add(new WorkflowTransition
        {
            FromStatus = BrandRegistrationStatus.Submitted.ToString(),
            ToStatus = BrandRegistrationStatus.NeedAssignmentBySeniorManagerMsai.ToString(),
            Action = BrandRegistrationAction.StartAssignment.ToString(),
            ActorUsername = actor.Username,
            ActorRole = RoleNameFor.Vendor,
            OccurredAt = now
        });
    }

    public static void ApplyTransition(BrandRegistration registration, IRequestActor actor, BrandRegistrationAction action,
        BrandWorkflowRule rule, string? note, string? nextAssignee = null)
    {
        var now = DateTimeOffset.UtcNow;
        var workflowCase = registration.WorkflowCase;
        var from = workflowCase.Status;
        var activeAssignment = workflowCase.Assignments.SingleOrDefault(x => x.EndedAt == null);
        if (activeAssignment is not null && (rule.NextRole is not null || rule.Complete))
        {
            activeAssignment.EndedAt = now;
            activeAssignment.EndReason = action.ToString();
        }

        workflowCase.Status = rule.To.ToString();
        workflowCase.CompletedAt = rule.Complete ? now : null;
        workflowCase.Transitions.Add(new WorkflowTransition
        {
            FromStatus = from,
            ToStatus = workflowCase.Status,
            Action = action.ToString(),
            ActorUsername = actor.Username,
            ActorRole = rule.Role,
            Note = string.IsNullOrWhiteSpace(note) ? null : note.Trim(),
            OccurredAt = now
        });
        if (rule.NextRole is not null)
        {
            workflowCase.Assignments.Add(new WorkflowAssignment
            {
                Role = rule.NextRole,
                AssigneeUsername = nextAssignee,
                AssignedAt = now,
                AssignedBy = actor.Username
            });
        }
    }
}
