namespace EBVL.BackEnd.Infrastructure.Endpoint;

public interface IEndpoint
{
    public RouteHandlerBuilder RegisterTo(WebApplication app);
}
