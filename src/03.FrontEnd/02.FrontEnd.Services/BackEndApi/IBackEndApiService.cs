using RestSharp;

namespace EBVL.FrontEnd.Services.BackEndApi;

public interface IBackEndApiService
{
    public Task SendRequestAsync(RestRequest restRequest, CancellationToken cancellationToken = default);
    public Task<T> SendRequestAsync<T>(RestRequest restRequest, CancellationToken cancellationToken = default);
}
