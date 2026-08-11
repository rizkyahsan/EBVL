using EBVL.Shared.Dto.Modules.Examples.Emails.SendEmailWithTemplate;

namespace EBVL.FrontEnd.Logics.Modules.Examples.Emails.SendEmailWithTemplate;

public sealed record SendEmailWithTemplateCommand : SendEmailWithTemplateRequest, IRequest<SendEmailWithTemplateResponse>
{
}

public sealed class SendEmailWithTemplateCommandValidator : AbstractValidatorBase<SendEmailWithTemplateCommand>
{
    public SendEmailWithTemplateCommandValidator()
    {
        Include(new SendEmailWithTemplateRequestValidator());
    }
}

public sealed class SendEmailWithTemplateCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<SendEmailWithTemplateCommand, SendEmailWithTemplateResponse>
{
    public async Task<SendEmailWithTemplateResponse> Handle(SendEmailWithTemplateCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(SendEmailWithTemplateRoute.ResourceUri, Method.Post)
        {
            AlwaysMultipartFormData = true
        };

        restRequest.AddFormParameter(request);

        return await backEndApiService.SendRequestAsync<SendEmailWithTemplateResponse>(restRequest, cancellationToken);
    }
}
