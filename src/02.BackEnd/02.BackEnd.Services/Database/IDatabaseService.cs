using EBVL.BackEnd.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace EBVL.BackEnd.Services.Database;

public partial interface IDatabaseService
{
    public Task<int> SaveAsync(string actionName, CancellationToken cancellationToken = default);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    public void SetQuestionnaireOriginalRowVersion(Questionnaire questionnaire, byte[] rowVersion);
    public void SetDocumentDefinitionOriginalRowVersion(DocumentDefinition documentDefinition, byte[] rowVersion);
    public void SetDocumentRequirementSetOriginalRowVersion(DocumentRequirementSet documentRequirementSet, byte[] rowVersion);
    public void SetVendorRegistrationOriginalRowVersion(VendorRegistration registration, byte[] rowVersion);
    public void SetVendorRegistrationUnchanged(VendorRegistration registration);
    public void SetQuestionnaireRuntimeGraphUnchanged();
    public void SetWorkflowCaseOriginalRowVersion(WorkflowCase workflowCase, byte[] rowVersion);
}
