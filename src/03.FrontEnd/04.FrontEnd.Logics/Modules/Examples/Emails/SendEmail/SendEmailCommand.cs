using EBVL.Shared.Dto.Modules.Examples.Emails.SendEmail;

namespace EBVL.FrontEnd.Logics.Modules.Examples.Emails.SendEmail;

public sealed record SendEmailCommand : SendEmailRequest, IRequest<SendEmailResponse>
{
}

public sealed class SendEmailCommandValidator : AbstractValidatorBase<SendEmailCommand>
{
    public SendEmailCommandValidator()
    {
        Include(new SendEmailRequestValidator());
    }
}

public sealed class SendEmailCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<SendEmailCommand, SendEmailResponse>
{
    public async Task<SendEmailResponse> Handle(SendEmailCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(SendEmailRoute.ResourceUri, Method.Post)
        {
            AlwaysMultipartFormData = true
        };

        restRequest.AddFormParameter(request);

        return await backEndApiService.SendRequestAsync<SendEmailResponse>(restRequest, cancellationToken);
    }
}
