using EBVL.Shared.Dto.Modules.Examples.Documents.AddDocument;

namespace EBVL.FrontEnd.Logics.Modules.Examples.Documents.AddDocument;

public sealed record AddDocumentCommand : AddDocumentRequest, IRequest<AddDocumentResponse>
{
}

public sealed class AddDocumentCommandValidator : AbstractValidatorBase<AddDocumentCommand>
{
    public AddDocumentCommandValidator()
    {
        Include(new AddDocumentRequestValidator());
    }
}

public sealed class AddDocumentCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<AddDocumentCommand, AddDocumentResponse>
{
    public async Task<AddDocumentResponse> Handle(AddDocumentCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(AddDocumentRoute.ResourceUri, Method.Post);
        restRequest.AddFormParameter(request);

        return await backEndApiService.SendRequestAsync<AddDocumentResponse>(restRequest, cancellationToken);
    }
}
