using EBVL.Shared.Dto.Modules.Main.Projects.CreateProject;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.CreateProject;

public sealed record CreateProjectCommand : CreateProjectRequest, IRequest<CreateProjectResponse> { }

public sealed class CreateProjectCommandValidator : AbstractValidatorBase<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        Include(new CreateProjectRequestValidator());
    }
}

public sealed class CreateProjectCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<CreateProjectCommand, CreateProjectResponse>
{
    public async Task<CreateProjectResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(CreateProjectRoute.ResourceUri, Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<CreateProjectResponse>(restRequest, cancellationToken);
    }
}
