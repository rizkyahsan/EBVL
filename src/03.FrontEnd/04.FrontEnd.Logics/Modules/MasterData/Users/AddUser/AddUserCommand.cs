using EBVL.Shared.Dto.Modules.MasterData.Users.AddUser;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Users.AddUser;

public sealed record AddUserCommand : AddUserRequest, IRequest<AddUserResponse>
{
}

public sealed class AddUserCommandValidator : AbstractValidatorBase<AddUserCommand>
{
    public AddUserCommandValidator()
    {
        Include(new AddUserRequestValidator());
    }
}

public sealed class AddUserCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<AddUserCommand, AddUserResponse>
{
    public async Task<AddUserResponse> Handle(AddUserCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(AddUserRoute.ResourceUri, Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<AddUserResponse>(restRequest, cancellationToken);
    }
}
