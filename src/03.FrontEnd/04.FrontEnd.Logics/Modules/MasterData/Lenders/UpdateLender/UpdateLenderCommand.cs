using EBVL.Shared.Dto.Modules.MasterData.Lenders.UpdateLender;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Lenders.UpdateLender;

public sealed record UpdateLenderCommand : UpdateLenderRequest, IRequest
{
}

public sealed class UpdateLenderCommandValidator : AbstractValidatorBase<UpdateLenderCommand>
{
    public UpdateLenderCommandValidator()
    {
        Include(new UpdateLenderRequestValidator());
    }
}

public sealed class UpdateLenderCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<UpdateLenderCommand>
{
    public async Task Handle(UpdateLenderCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(UpdateLenderRoute.ResourceUri(request.LenderId), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
