using EBVL.Shared.Dto.Modules.MasterData.EmailTemplates.AddEmailTemplate;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.EmailTemplates.AddEmailTemplate;

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

public sealed class AddEmailTemplateCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<AddEmailTemplateCommand, AddEmailTemplateResponse>
{
    public async Task<AddEmailTemplateResponse> Handle(AddEmailTemplateCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(AddEmailTemplateRoute.ResourceUri, Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<AddEmailTemplateResponse>(restRequest, cancellationToken);
    }
}
