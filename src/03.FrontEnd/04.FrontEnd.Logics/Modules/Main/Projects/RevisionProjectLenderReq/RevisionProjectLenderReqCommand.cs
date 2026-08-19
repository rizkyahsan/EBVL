using EBVL.Shared.Dto.Modules.Main.Projects.RevisionProjectLenderReq;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.RevisionProjectLenderReq;

public sealed record RevisionProjectLenderReqCommand : RevisionProjectLenderReqRequest, IRequest { }

public sealed class RevisionProjectLenderReqCommandValidator : AbstractValidatorBase<RevisionProjectLenderReqCommand>
{
    public RevisionProjectLenderReqCommandValidator()
    {
        Include(new RevisionProjectLenderReqRequestValidator());
    }
}

public sealed class RevisionProjectLenderReqCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<RevisionProjectLenderReqCommand>
{
    public async Task Handle(RevisionProjectLenderReqCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(RevisionProjectLenderReqRoute.ResourceUri(request.Id), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
