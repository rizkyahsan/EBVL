namespace EBVL.BackEnd.Services.Database;

public partial interface IDatabaseService
{
    public Task<int> SaveAsync(string actionName, CancellationToken cancellationToken = default);
}
