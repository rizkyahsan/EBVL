using Microsoft.EntityFrameworkCore.Storage;
using EBVL.BackEnd.Domain.Entities;

namespace EBVL.BackEnd.Services.Database;

public partial interface IDatabaseService
{
    public Task<int> SaveAsync(string actionName, CancellationToken cancellationToken = default);

    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    public void SetQuestionnaireOriginalRowVersion(Questionnaire questionnaire, byte[] rowVersion);
    public void SetVendorRegistrationOriginalRowVersion(VendorRegistration registration, byte[] rowVersion);
}
