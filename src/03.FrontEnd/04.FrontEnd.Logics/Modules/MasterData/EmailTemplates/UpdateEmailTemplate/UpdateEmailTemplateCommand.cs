using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.UpdateEmailTemplate;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.EmailTemplates.UpdateEmailTemplate;

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

public sealed class UpdateEmailTemplateCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<UpdateEmailTemplateCommand>
{
    public async Task Handle(UpdateEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(UpdateEmailTemplateRoute.ResourceUri(request.EmailTemplateId), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
