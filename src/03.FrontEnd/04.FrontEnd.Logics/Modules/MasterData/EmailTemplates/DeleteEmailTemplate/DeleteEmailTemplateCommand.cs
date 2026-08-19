using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.DeleteEmailTemplate;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.EmailTemplates.DeleteEmailTemplate;

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

public sealed class DeleteEmailTemplateCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<DeleteEmailTemplateCommand>
{
    public async Task Handle(DeleteEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(DeleteEmailTemplateRoute.ResourceUri(request.EmailTemplateId), Method.Delete);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
