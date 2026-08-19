using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.UpdateEmailTemplate;

namespace EBVL.BackEnd.Logics.Modules.MasterData.EmailTemplates.UpdateEmailTemplate;

public sealed record UpdateEmailTemplateCommand : UpdateEmailTemplateRequest, IRequest
{
}

public sealed class UpdateEmailTemplateCommandValidator : AbstractValidatorBase<UpdateEmailTemplateCommand>
{
    public UpdateEmailTemplateCommandValidator()
    {
        Include(new UpdateEmailTemplateRequestValidator());
    }
}

public sealed class UpdateEmailTemplateCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<UpdateEmailTemplateCommand>
{
    public async Task Handle(UpdateEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var emailTemplate = await databaseService.EmailTemplates
            .Where(x => !x.IsDeleted && x.Id == request.EmailTemplateId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(EmailTemplatesDisplayTextFor.EmailTemplate, CommonDisplayTextFor.Id, request.EmailTemplateId);

        emailTemplate.DefaultTo = request.DefaultTo;
        emailTemplate.DefaultCc = request.DefaultCc;
        emailTemplate.Subject = request.Subject;
        emailTemplate.Content = request.Content;

        _ = await databaseService.SaveAsync(nameof(UpdateEmailTemplate), cancellationToken);
    }
}

