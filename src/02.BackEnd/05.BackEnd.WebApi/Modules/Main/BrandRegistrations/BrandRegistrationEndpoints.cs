using EBVL.BackEnd.Logics.Modules.Main.BrandRegistrations;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.AssignBrandRegistration;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.CreateBrandRegistration;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.GetBrandRegistration;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.GetBrandRegistrations;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.RespondBrandInvitation;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.SendBrandInvitation;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.TransitionBrandRegistration;
using MainPermissions = EBVL.Shared.Dto.Modules.Main.Permissions;

namespace EBVL.BackEnd.WebApi.Modules.Main.BrandRegistrations;

public sealed class BrandRegistrationEndpoints : IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app)
    {
        _ = app.MapGet(GetBrandRegistrationsRoute.Pattern, (ISender sender, CancellationToken token) =>
                sender.Send(new GetBrandRegistrationsQuery(), token))
            .RequireAuthorization(MainPermissions.BrandRegistrationListPolicy)
            .WithTags(RouteConfig.Tag).WithName(GetBrandRegistrationsRoute.Name).WithDescription(GetBrandRegistrationsRoute.Description);
        _ = app.MapPost(CreateBrandRegistrationRoute.Pattern, (CreateBrandRegistrationRequest request, ISender sender, CancellationToken token) =>
                sender.Send(new CreateBrandRegistrationCommand(request), token))
            .RequireAuthorization(MainPermissions.BrandRegistrationCreate)
            .WithTags(RouteConfig.Tag).WithName(CreateBrandRegistrationRoute.Name).WithDescription(CreateBrandRegistrationRoute.Description);
        _ = app.MapGet(GetBrandRegistrationRoute.Pattern, (Guid registrationId, ISender sender, CancellationToken token) =>
                sender.Send(new GetBrandRegistrationQuery(registrationId), token))
            .RequireAuthorization(MainPermissions.BrandRegistrationViewPolicy)
            .WithTags(RouteConfig.Tag).WithName(GetBrandRegistrationRoute.Name).WithDescription(GetBrandRegistrationRoute.Description);
        _ = app.MapPost(TransitionBrandRegistrationRoute.Pattern, (Guid registrationId, TransitionBrandRegistrationRequest request, ISender sender, CancellationToken token) =>
                sender.Send(new TransitionBrandRegistrationCommand(registrationId, request), token))
            .RequireAuthorization(MainPermissions.BrandRegistrationTransition)
            .WithTags(RouteConfig.Tag).WithName(TransitionBrandRegistrationRoute.Name).WithDescription(TransitionBrandRegistrationRoute.Description);
        _ = app.MapPost(AssignBrandRegistrationRoute.Pattern, (Guid registrationId, AssignBrandRegistrationRequest request, ISender sender, CancellationToken token) =>
                sender.Send(new AssignBrandRegistrationCommand(registrationId, request), token))
            .RequireAuthorization(MainPermissions.BrandRegistrationDisposition)
            .WithTags(RouteConfig.Tag).WithName(AssignBrandRegistrationRoute.Name).WithDescription(AssignBrandRegistrationRoute.Description);
        _ = app.MapPost(SendBrandInvitationRoute.Pattern, (Guid registrationId, SendBrandInvitationRequest request, ISender sender, CancellationToken token) =>
                sender.Send(new SendBrandInvitationCommand(registrationId, request), token))
            .RequireAuthorization(MainPermissions.BrandRegistrationInvite)
            .WithTags(RouteConfig.Tag).WithName(SendBrandInvitationRoute.Name).WithDescription(SendBrandInvitationRoute.Description);
        return app.MapPost(RespondBrandInvitationRoute.Pattern,
                (Guid registrationId, Guid invitationId, RespondBrandInvitationRequest request, ISender sender, CancellationToken token) =>
                    sender.Send(new RespondBrandInvitationCommand(registrationId, invitationId, request), token))
            .RequireAuthorization(MainPermissions.BrandRegistrationRespondInvitation)
            .WithTags(RouteConfig.Tag).WithName(RespondBrandInvitationRoute.Name).WithDescription(RespondBrandInvitationRoute.Description);
    }
}
