using EBVL.Shared.Dto.Modules.Main.Projects.CreateProjectStage;

namespace EBVL.FrontEnd.Logics.Modules.Main.Projects.CreateProjectStage;

public sealed record CreateProjectStageCommand : CreateProjectStageRequest, IRequest<CreateProjectStageResponse> { }

public sealed class CreateProjectStageCommandValidator : AbstractValidatorBase<CreateProjectStageCommand>
{
    public CreateProjectStageCommandValidator()
    {
        Include(new CreateProjectStageRequestValidator());
    }
}

public sealed class CreateProjectStageCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<CreateProjectStageCommand, CreateProjectStageResponse>
{
    public async Task<CreateProjectStageResponse> Handle(CreateProjectStageCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(CreateProjectStageRoute.ResourceUri(request.ProjectId), Method.Post);
        _ = restRequest.AddJsonBody(request);

        return await backEndApiService.SendRequestAsync<CreateProjectStageResponse>(restRequest, cancellationToken);
    }
}
