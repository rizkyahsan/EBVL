using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.Common;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.CreateBrandRegistration;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.GetBrandRegistration;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.GetBrandRegistrations;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.RespondBrandInvitation;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.SendBrandInvitation;
using EBVL.Shared.Dto.Modules.Main.BrandRegistrations.TransitionBrandRegistration;

namespace EBVL.FrontEnd.Logics.Modules.Main.BrandRegistrations;

public sealed record GetBrandRegistrationsQuery : IRequest<GetBrandRegistrationsResponse>;
public sealed record GetBrandRegistrationQuery(Guid RegistrationId) : IRequest<BrandRegistrationItem>;
public sealed record CreateBrandRegistrationCommand(CreateBrandRegistrationRequest Request) : IRequest<BrandRegistrationItem>;
public sealed record TransitionBrandRegistrationCommand(Guid RegistrationId, TransitionBrandRegistrationRequest Request) : IRequest<BrandRegistrationItem>;
public sealed record SendBrandInvitationCommand(Guid RegistrationId, SendBrandInvitationRequest Request) : IRequest<BrandRegistrationItem>;
public sealed record RespondBrandInvitationCommand(Guid RegistrationId, Guid InvitationId, RespondBrandInvitationRequest Request) : IRequest<BrandRegistrationItem>;

public sealed class GetBrandRegistrationsHandler(IBackEndApiService api) : IRequestHandler<GetBrandRegistrationsQuery, GetBrandRegistrationsResponse>
{
    public Task<GetBrandRegistrationsResponse> Handle(GetBrandRegistrationsQuery request, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<GetBrandRegistrationsResponse>(new RestRequest(GetBrandRegistrationsRoute.Pattern), cancellationToken);
    }
}

public sealed class GetBrandRegistrationHandler(IBackEndApiService api) : IRequestHandler<GetBrandRegistrationQuery, BrandRegistrationItem>
{
    public Task<BrandRegistrationItem> Handle(GetBrandRegistrationQuery request, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<BrandRegistrationItem>(new RestRequest(GetBrandRegistrationRoute.ResourceUri(request.RegistrationId)), cancellationToken);
    }
}

public sealed class CreateBrandRegistrationHandler(IBackEndApiService api) : IRequestHandler<CreateBrandRegistrationCommand, BrandRegistrationItem>
{
    public Task<BrandRegistrationItem> Handle(CreateBrandRegistrationCommand command, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<BrandRegistrationItem>(new RestRequest(CreateBrandRegistrationRoute.Pattern, Method.Post).AddJsonBody(command.Request), cancellationToken);
    }
}

public sealed class TransitionBrandRegistrationHandler(IBackEndApiService api) : IRequestHandler<TransitionBrandRegistrationCommand, BrandRegistrationItem>
{
    public Task<BrandRegistrationItem> Handle(TransitionBrandRegistrationCommand command, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<BrandRegistrationItem>(new RestRequest(Route(TransitionBrandRegistrationRoute.Pattern, command.RegistrationId), Method.Post).AddJsonBody(command.Request), cancellationToken);
    }

    private static string Route(string route, Guid registrationId)
    {
        return route.Replace("{registrationId:guid}", registrationId.ToString());
    }
}

public sealed class SendBrandInvitationHandler(IBackEndApiService api) : IRequestHandler<SendBrandInvitationCommand, BrandRegistrationItem>
{
    public Task<BrandRegistrationItem> Handle(SendBrandInvitationCommand command, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<BrandRegistrationItem>(new RestRequest(Route(SendBrandInvitationRoute.Pattern, command.RegistrationId), Method.Post).AddJsonBody(command.Request), cancellationToken);
    }

    private static string Route(string route, Guid registrationId)
    {
        return route.Replace("{registrationId:guid}", registrationId.ToString());
    }
}

public sealed class RespondBrandInvitationHandler(IBackEndApiService api) : IRequestHandler<RespondBrandInvitationCommand, BrandRegistrationItem>
{
    public Task<BrandRegistrationItem> Handle(RespondBrandInvitationCommand command, CancellationToken cancellationToken)
    {
        var route = RespondBrandInvitationRoute.Pattern
            .Replace("{registrationId:guid}", command.RegistrationId.ToString())
            .Replace("{invitationId:guid}", command.InvitationId.ToString());
        return api.SendRequestAsync<BrandRegistrationItem>(new RestRequest(route, Method.Post).AddJsonBody(command.Request), cancellationToken);
    }
}
