using EBVL.Shared.Dto.Modules.Examples.Documents.UpdateDocument;

namespace EBVL.FrontEnd.Logics.Modules.Examples.Documents.UpdateDocument;

public sealed record UpdateDocumentCommand : UpdateDocumentRequest, IRequest
{
}

public sealed class UpdateDocumentCommandValidator : AbstractValidatorBase<UpdateDocumentCommand>
{
    public UpdateDocumentCommandValidator()
    {
        Include(new UpdateDocumentRequestValidator());
    }
}

public sealed class UpdateDocumentCommandHandler(IBackEndApiService backEndApiService)
    : IRequestHandler<UpdateDocumentCommand>
{
    public async Task Handle(UpdateDocumentCommand request, CancellationToken cancellationToken)
    {
        var restRequest = new RestRequest(UpdateDocumentRoute.ResourceUri(request.DocumentId), Method.Patch);
        _ = restRequest.AddJsonBody(request);

        await backEndApiService.SendRequestAsync(restRequest, cancellationToken);
    }
}
