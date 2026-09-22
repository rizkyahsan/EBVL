using EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.Common;
using EBVL.Shared.Dto.Modules.Main.VendorRegistrations.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.Main.VendorRegistrations.GetQuestionnaireRuntime;

public sealed record GetQuestionnaireRuntimeQuery(Guid RegistrationId, string ResumeToken) : IRequest<QuestionnaireRuntimeResponse>;

public sealed class GetQuestionnaireRuntimeHandler(IDatabaseService databaseService) : IRequestHandler<GetQuestionnaireRuntimeQuery, QuestionnaireRuntimeResponse>
{
    public Task<QuestionnaireRuntimeResponse> Handle(GetQuestionnaireRuntimeQuery request, CancellationToken cancellationToken)
    {
        return QuestionnaireRuntime.LoadResponse(databaseService, request.RegistrationId, request.ResumeToken, null, cancellationToken);
    }
}
