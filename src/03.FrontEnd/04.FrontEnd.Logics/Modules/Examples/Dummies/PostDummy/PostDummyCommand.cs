using EBVL.Shared.Dto.Modules.Examples.Dummies.PostDummy;

namespace EBVL.FrontEnd.Logics.Modules.Examples.Dummies.PostDummy;

public sealed record PostDummyCommand : PostDummyRequest, IRequest<PostDummyResponse>
{
}

public sealed class PostDummyCommandValidator : AbstractValidatorBase<PostDummyCommand>
{
    public PostDummyCommandValidator()
    {
        Include(new PostDummyRequestValidator());
    }
}

public sealed class PostDummyCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<PostDummyCommand, PostDummyResponse>
{
    public async Task<PostDummyResponse> Handle(PostDummyCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(PostDummyRoute.ResourceUri, Method.Post);
        _ = restRequest.AddBody(request);

        return await backEndApiService.SendRequestAsync<PostDummyResponse>(restRequest, cancellationToken);
    }
}
