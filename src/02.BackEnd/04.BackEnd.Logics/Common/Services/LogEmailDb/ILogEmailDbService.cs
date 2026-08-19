using EBVL.BackEnd.Services.EmailBlast2.Model;

namespace EBVL.BackEnd.Logics.Common.Services.LogEmailDb;

public interface ILogEmailDbService
{
    public Task LogAsync(SendEmailInput2 input, string provider, bool success, string? message = null,
        string? externalMessageId = null, CancellationToken cancellationToken = default);
}
