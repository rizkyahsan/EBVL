using EBVL.Shared.Dto.Modules.Main.Projects.UploadProjectFile;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.UploadProjectFile;

public sealed record UploadProjectFileCommand : UploadProjectFileRequest, IRequest { }

public sealed class UploadProjectFileCommandValidator : AbstractValidatorBase<UploadProjectFileCommand>
{
    public UploadProjectFileCommandValidator()
    {
        Include(new UploadProjectFileRequestValidator());
    }
}

public sealed class UploadProjectFileCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<UploadProjectFileCommand>
{
    public async Task Handle(UploadProjectFileCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(UploadProjectFileRoute.ResourceUri(request.Id), Method.Post);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
