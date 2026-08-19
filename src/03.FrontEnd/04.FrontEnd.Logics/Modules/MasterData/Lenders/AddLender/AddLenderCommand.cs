using EBVL.Shared.Dto.Modules.MasterData.Lenders.AddLender;

namespace EBVL.FrontEnd.Logics.Modules.MasterData.Lenders.AddLender;

public sealed record AddLenderCommand : AddLenderRequest, IRequest<AddLenderResponse>
{
}

public sealed class AddLenderCommandValidator : AbstractValidatorBase<AddLenderCommand>
{
    public AddLenderCommandValidator()
    {
        Include(new AddLenderRequestValidator());
    }
}

public sealed class AddLenderCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<AddLenderCommand, AddLenderResponse>
{
    public async Task<AddLenderResponse> Handle(AddLenderCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(AddLenderRoute.ResourceUri, Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<AddLenderResponse>(restRequest, cancellationToken);
    }
}
