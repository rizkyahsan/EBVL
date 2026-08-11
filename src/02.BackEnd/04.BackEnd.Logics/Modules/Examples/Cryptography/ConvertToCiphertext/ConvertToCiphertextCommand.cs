using Pertamina.Services.Cryptography;
using EBVL.Shared.Dto.Modules.Examples.Cryptography.ConvertToCiphertext;

namespace EBVL.BackEnd.Logics.Modules.Examples.Cryptography.ConvertToCiphertext;

public sealed record ConvertToCiphertextCommand : ConvertToCiphertextRequest, IRequest<ConvertToCiphertextResponse>
{
}

public sealed class ConvertToCiphertextCommandValidator : AbstractValidatorBase<ConvertToCiphertextCommand>
{
    public ConvertToCiphertextCommandValidator()
    {
        Include(new ConvertToCiphertextRequestValidator());
    }
}

public sealed class ConvertToCiphertextCommandHandler(ICryptographyService cryptographyService)
    : IRequestHandler<ConvertToCiphertextCommand, ConvertToCiphertextResponse>
{
    public Task<ConvertToCiphertextResponse> Handle(ConvertToCiphertextCommand request, CancellationToken cancellationToken)
    {
        var response = new ConvertToCiphertextResponse
        {
            Item = new ConvertToCiphertextResult
            {
                Ciphertext = cryptographyService.Encrypt(request.Plaintext)
            }
        };

        return Task.FromResult(response);
    }
}
