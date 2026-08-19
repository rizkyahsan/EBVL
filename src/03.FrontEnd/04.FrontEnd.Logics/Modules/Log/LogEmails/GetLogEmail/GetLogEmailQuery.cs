using EBVL.Shared.Dto.Modules.Log.LogEmails.GetLogEmail;

namespace EBVL.FrontEnd.Logics.Modules.Log.LogEmails.GetLogEmail;

public sealed record GetLogEmailQuery : GetLogEmailRequest, IRequest<GetLogEmailResponse> { }

public sealed class GetLogEmailQueryValidator : AbstractValidatorBase<GetLogEmailQuery>
{
    public GetLogEmailQueryValidator()
    {
        Include(new GetLogEmailRequestValidator());
    }
}

public sealed class GetLogEmailQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetLogEmailQuery, GetLogEmailResponse>
{
    public async Task<GetLogEmailResponse> Handle(GetLogEmailQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetLogEmailRoute.ResourceUri(request.Id), Method.Get);

        return await backEndApiService.SendRequestAsync<GetLogEmailResponse>(restRequest, cancellationToken);
    }
}
