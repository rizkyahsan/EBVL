using Pertamina.Services.Cryptography;
using EBVL.Shared.Dto.Modules.Examples.Cryptography.ConvertToPlaintext;

namespace EBVL.BackEnd.Logics.Modules.Examples.Cryptography.ConvertToPlaintext;

public sealed record ConvertToPlaintextCommand : ConvertToPlaintextRequest, IRequest<ConvertToPlaintextResponse>
{
}

public sealed class ConvertToPlaintextCommandValidator : AbstractValidatorBase<ConvertToPlaintextCommand>
{
    public ConvertToPlaintextCommandValidator()
    {
        Include(new ConvertToPlaintextRequestValidator());
    }
}

public sealed class ConvertToPlaintextCommandHandler(ICryptographyService cryptographyService)
    : IRequestHandler<ConvertToPlaintextCommand, ConvertToPlaintextResponse>
{
    public Task<ConvertToPlaintextResponse> Handle(ConvertToPlaintextCommand request, CancellationToken cancellationToken)
    {
        var response = new ConvertToPlaintextResponse
        {
            Item = new ConvertToPlaintextResult
            {
                Plaintext = cryptographyService.Decrypt(request.Chipertext)
            }
        };

        return Task.FromResult(response);
    }
}
