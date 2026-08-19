using EBVL.Shared.Dto.Modules.MasterData.Lenders.DeleteLender;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Lenders.DeleteLender;

public sealed record DeleteLenderCommand : DeleteLenderRequest, IRequest
{
}

public sealed class DeleteLenderCommandValidator : AbstractValidatorBase<DeleteLenderCommand>
{
    public DeleteLenderCommandValidator()
    {
        Include(new DeleteLenderRequestValidator());
    }
}

public sealed class DeleteLenderCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<DeleteLenderCommand>
{
    public async Task Handle(DeleteLenderCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(DeleteLenderRoute.ResourceUri(request.LenderId), Method.Delete);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
