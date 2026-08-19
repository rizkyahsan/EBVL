using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.AddEmailTemplate;

namespace EBVL.BackEnd.Logics.Modules.MasterData.EmailTemplates.AddEmailTemplate;

public sealed record AddEmailTemplateCommand : AddEmailTemplateRequest, IRequest<AddEmailTemplateResponse>
{
}

public sealed class AddEmailTemplateCommandValidator : AbstractValidatorBase<AddEmailTemplateCommand>
{
    public AddEmailTemplateCommandValidator()
    {
        Include(new AddEmailTemplateRequestValidator());
    }
}

public sealed class AddEmailTemplateCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<AddEmailTemplateCommand, AddEmailTemplateResponse>
{
    public async Task<AddEmailTemplateResponse> Handle(AddEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var checkDuplicate = await databaseService.EmailTemplates
            .Where(x => !x.IsDeleted && x.Module == request.Module && x.Action == request.Action)
            .AnyAsync(cancellationToken);

        if (checkDuplicate)
        {
            throw ExceptionFor.EntityAlreadyExists(EmailTemplatesDisplayTextFor.EmailTemplate, $"{EmailTemplatesDisplayTextFor.Module} and {EmailTemplatesDisplayTextFor.Action}", $"{request.Module} and {request.Action}");
        }

        var emailTemplate = new EmailTemplate
        {
            Module = request.Module,
            Action = request.Action,
            DefaultTo = request.DefaultTo,
            DefaultCc = request.DefaultCc,
            Subject = request.Subject,
            Content = request.Content
        };

        _ = await databaseService.EmailTemplates.AddAsync(emailTemplate, cancellationToken);
        _ = await databaseService.SaveAsync(nameof(AddEmailTemplate), cancellationToken);

        return new AddEmailTemplateResponse
        {
            Item = new EmailTemplateItem
            {
                Id = emailTemplate.Id
            }
        };
    }
}
