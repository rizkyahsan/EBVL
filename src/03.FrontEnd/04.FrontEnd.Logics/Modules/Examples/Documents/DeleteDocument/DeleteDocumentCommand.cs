using EBVL.Shared.Dto.Modules.Examples.Documents.DeleteDocument;

namespace EBVL.FrontEnd.Logics.Modules.Examples.Documents.DeleteDocument;

public sealed record DeleteDocumentCommand : DeleteDocumentRequest, IRequest
{
}

public sealed class DeleteDocumentCommandValidator : AbstractValidatorBase<DeleteDocumentCommand>
{
    public DeleteDocumentCommandValidator()
    {
        Include(new DeleteDocumentRequestValidator());
    }
}

public sealed class DeleteDocumentCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<DeleteDocumentCommand>
{
    public async Task Handle(DeleteDocumentCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(DeleteDocumentRoute.ResourceUri(request.DocumentId), Method.Delete);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
