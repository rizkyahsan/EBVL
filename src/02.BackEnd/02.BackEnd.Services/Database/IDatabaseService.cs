using Microsoft.EntityFrameworkCore.Storage;

namespace EBVL.BackEnd.Services.Database;

public partial interface IDatabaseService
{
    public Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    public Task<int> SaveAsync(string actionName, CancellationToken cancellationToken = default);
}
