using EBVL.Shared.Dto.Modules.Log.LogEmails.GetLogEmails;

namespace EBVL.FrontEnd.Logics.Modules.Log.LogEmails.GetLogEmails;

public sealed record GetLogEmailsQuery : GetLogEmailsRequest, IRequest<GetLogEmailsResponse> { }

public sealed class GetLogEmailsQueryHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<GetLogEmailsQuery, GetLogEmailsResponse>
{
    public async Task<GetLogEmailsResponse> Handle(GetLogEmailsQuery request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(GetLogEmailsRoute.ResourceUri, Method.Get);
        _ = restRequest.AddQueryParameter(nameof(request.StartDatetime), request.StartDatetime.ToString());
        _ = restRequest.AddQueryParameter(nameof(request.EndDatetime), request.EndDatetime.ToString());

        return await backEndApiService.SendRequestAsync<GetLogEmailsResponse>(restRequest, cancellationToken);
    }
}
