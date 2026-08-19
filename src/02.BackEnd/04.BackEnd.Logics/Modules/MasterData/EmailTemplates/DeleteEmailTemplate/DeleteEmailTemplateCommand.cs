using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.DeleteEmailTemplate;

namespace EBVL.BackEnd.Logics.Modules.MasterData.EmailTemplates.DeleteEmailTemplate;

public sealed record DeleteEmailTemplateCommand : DeleteEmailTemplateRequest, IRequest
{
}

public sealed class DeleteEmailTemplateCommandValidator : AbstractValidatorBase<DeleteEmailTemplateCommand>
{
    public DeleteEmailTemplateCommandValidator()
    {
        Include(new DeleteEmailTemplateRequestValidator());
    }
}

public sealed class DeleteEmailTemplateCommandHandler(IDatabaseService databaseService)
    : IRequestHandler<DeleteEmailTemplateCommand>
{
    public async Task Handle(DeleteEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var emailTemplate = await databaseService.EmailTemplates
            .Where(x => !x.IsDeleted && x.Id == request.EmailTemplateId)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw ExceptionFor.EntityNotFound(EmailTemplatesDisplayTextFor.EmailTemplate, CommonDisplayTextFor.Id, request.EmailTemplateId);

        emailTemplate.IsDeleted = true;

        _ = await databaseService.SaveAsync(nameof(DeleteEmailTemplate), cancellationToken);
    }
}
