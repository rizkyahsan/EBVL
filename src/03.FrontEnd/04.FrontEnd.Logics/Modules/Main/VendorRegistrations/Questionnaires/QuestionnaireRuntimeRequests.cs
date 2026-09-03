using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.PreRegistration;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.FrontEnd.Logics.Modules.Main.VendorRegistrations.Questionnaires;

public sealed record StartQuestionnaireCommand(PreRegistrationRequest Request) : IRequest<QuestionnaireRuntimeResponse>;
public sealed record GetQuestionnaireRuntimeQuery(Guid RegistrationId, string ResumeToken) : IRequest<QuestionnaireRuntimeResponse>;
public sealed record UpdateVendorRegistrationProfileCommand(Guid RegistrationId, UpdateVendorRegistrationProfileRequest Request) : IRequest<QuestionnaireRuntimeResponse>;
public sealed record SaveQuestionnaireAnswersCommand(Guid RegistrationId, SaveAnswersRequest Request) : IRequest<QuestionnaireRuntimeResponse>;
public sealed record SubmitQuestionnaireCommand(Guid RegistrationId, SubmitQuestionnaireRequest Request) : IRequest<QuestionnaireRuntimeResponse>;
public sealed record UploadQuestionnaireFileCommand(Guid RegistrationId, Guid QuestionId, string ResumeToken, string FileName, byte[] Content) : IRequest<UploadQuestionnaireFileResponse>;
public sealed record DeleteQuestionnaireFileCommand(Guid RegistrationId, Guid FileId, string ResumeToken) : IRequest;
public sealed record UploadVendorRegistrationDocumentCommand(Guid RegistrationId, string DefinitionKey, string ResumeToken, string FileName, byte[] Content) : IRequest<UploadVendorRegistrationDocumentResponse>;
public sealed record DeleteVendorRegistrationDocumentCommand(Guid RegistrationId, Guid DocumentId, string ResumeToken) : IRequest;

internal static class RuntimeUri
{
    public static string For(string route, Guid registrationId, Guid? itemId = null)
    {
        return route.Replace("{registrationId:guid}", registrationId.ToString()).Replace("{questionId:guid}", itemId?.ToString()).Replace("{fileId:guid}", itemId?.ToString());
    }
}
public sealed class StartQuestionnaireHandler(IBackEndApiService api) : IRequestHandler<StartQuestionnaireCommand, QuestionnaireRuntimeResponse>
{
    public Task<QuestionnaireRuntimeResponse> Handle(StartQuestionnaireCommand c, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<QuestionnaireRuntimeResponse>(new RestRequest(QuestionnaireRuntimeRoutes.Start, Method.Post).AddJsonBody(c.Request), cancellationToken);
    }
}
public sealed class GetQuestionnaireRuntimeHandler(IBackEndApiService api) : IRequestHandler<GetQuestionnaireRuntimeQuery, QuestionnaireRuntimeResponse>
{
    public Task<QuestionnaireRuntimeResponse> Handle(GetQuestionnaireRuntimeQuery q, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<QuestionnaireRuntimeResponse>(new RestRequest(RuntimeUri.For(QuestionnaireRuntimeRoutes.Get, q.RegistrationId), Method.Post).AddJsonBody(new ResumeQuestionnaireRequest(q.RegistrationId, q.ResumeToken)), cancellationToken);
    }
}
public sealed class SaveQuestionnaireAnswersHandler(IBackEndApiService api) : IRequestHandler<SaveQuestionnaireAnswersCommand, QuestionnaireRuntimeResponse>
{
    public Task<QuestionnaireRuntimeResponse> Handle(SaveQuestionnaireAnswersCommand c, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<QuestionnaireRuntimeResponse>(new RestRequest(RuntimeUri.For(QuestionnaireRuntimeRoutes.Answers, c.RegistrationId), Method.Put).AddJsonBody(c.Request), cancellationToken);
    }
}
public sealed class UpdateVendorRegistrationProfileHandler(IBackEndApiService api) : IRequestHandler<UpdateVendorRegistrationProfileCommand, QuestionnaireRuntimeResponse>
{
    public Task<QuestionnaireRuntimeResponse> Handle(UpdateVendorRegistrationProfileCommand command, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<QuestionnaireRuntimeResponse>(new RestRequest(RuntimeUri.For(QuestionnaireRuntimeRoutes.Profile, command.RegistrationId), Method.Put).AddJsonBody(command.Request), cancellationToken);
    }
}
public sealed class SubmitQuestionnaireHandler(IBackEndApiService api) : IRequestHandler<SubmitQuestionnaireCommand, QuestionnaireRuntimeResponse>
{
    public Task<QuestionnaireRuntimeResponse> Handle(SubmitQuestionnaireCommand c, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<QuestionnaireRuntimeResponse>(new RestRequest(RuntimeUri.For(QuestionnaireRuntimeRoutes.Submit, c.RegistrationId), Method.Post).AddJsonBody(c.Request), cancellationToken);
    }
}
public sealed class UploadQuestionnaireFileHandler(IBackEndApiService api) : IRequestHandler<UploadQuestionnaireFileCommand, UploadQuestionnaireFileResponse>
{
    public Task<UploadQuestionnaireFileResponse> Handle(UploadQuestionnaireFileCommand c, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync<UploadQuestionnaireFileResponse>(new RestRequest(RuntimeUri.For(QuestionnaireRuntimeRoutes.Files, c.RegistrationId, c.QuestionId), Method.Post).AddParameter("resumeToken", c.ResumeToken).AddFile("file", c.Content, c.FileName, "application/pdf"), cancellationToken);
    }
}
public sealed class DeleteQuestionnaireFileHandler(IBackEndApiService api) : IRequestHandler<DeleteQuestionnaireFileCommand>
{
    public Task Handle(DeleteQuestionnaireFileCommand c, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync(new RestRequest(RuntimeUri.For(QuestionnaireRuntimeRoutes.File, c.RegistrationId, c.FileId), Method.Delete).AddQueryParameter("resumeToken", c.ResumeToken), cancellationToken);
    }
}
public sealed class UploadVendorRegistrationDocumentHandler(IBackEndApiService api) : IRequestHandler<UploadVendorRegistrationDocumentCommand, UploadVendorRegistrationDocumentResponse>
{
    public Task<UploadVendorRegistrationDocumentResponse> Handle(UploadVendorRegistrationDocumentCommand command, CancellationToken cancellationToken)
    {
        var route = QuestionnaireRuntimeRoutes.Documents.Replace("{registrationId:guid}", command.RegistrationId.ToString()).Replace("{definitionKey}", Uri.EscapeDataString(command.DefinitionKey));
        return api.SendRequestAsync<UploadVendorRegistrationDocumentResponse>(new RestRequest(route, Method.Post).AddParameter("resumeToken", command.ResumeToken).AddFile("file", command.Content, command.FileName, "application/pdf"), cancellationToken);
    }
}
public sealed class DeleteVendorRegistrationDocumentHandler(IBackEndApiService api) : IRequestHandler<DeleteVendorRegistrationDocumentCommand>
{
    public Task Handle(DeleteVendorRegistrationDocumentCommand command, CancellationToken cancellationToken)
    {
        return api.SendRequestAsync(new RestRequest(QuestionnaireRuntimeRoutes.Document.Replace("{registrationId:guid}", command.RegistrationId.ToString()).Replace("{documentId:guid}", command.DocumentId.ToString()), Method.Delete).AddQueryParameter("resumeToken", command.ResumeToken), cancellationToken);
    }
}
