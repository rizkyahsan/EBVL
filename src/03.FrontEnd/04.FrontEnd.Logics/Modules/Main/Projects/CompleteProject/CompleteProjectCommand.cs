using EBVL.Shared.Dto.Modules.Main.Projects.CompleteProject;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.CompleteProject;

public sealed record CompleteProjectCommand : CompleteProjectRequest, IRequest { }

public sealed class CompleteProjectCommandValidator : AbstractValidatorBase<CompleteProjectCommand>
{
    public CompleteProjectCommandValidator()
    {
        Include(new CompleteProjectRequestValidator());
    }
}

public sealed class CompleteProjectCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<CompleteProjectCommand>
{
    public async Task Handle(CompleteProjectCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(CompleteProjectRoute.ResourceUri(request.Id), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
