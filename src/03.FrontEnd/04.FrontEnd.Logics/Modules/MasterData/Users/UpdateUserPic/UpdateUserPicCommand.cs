using EBVL.Shared.Dto.Modules.MasterData.Users.UpdateUserPic;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Users.UpdateUserPic;

public sealed record UpdateUserPicCommand : UpdateUserPicRequest, IRequest { }

public sealed class UpdateUserPicCommandValidator : AbstractValidatorBase<UpdateUserPicCommand>
{
    public UpdateUserPicCommandValidator()
    {
        Include(new UpdateUserPicRequestValidator());
    }
}

public sealed class UpdateUserPicCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<UpdateUserPicCommand>
{
    public async Task Handle(UpdateUserPicCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(UpdateUserPicRoute.ResourceUri(request.UserId), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
