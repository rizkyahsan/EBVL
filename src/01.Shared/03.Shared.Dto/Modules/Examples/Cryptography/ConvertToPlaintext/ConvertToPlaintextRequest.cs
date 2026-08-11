namespace EBVL.Shared.Dto.Modules.Examples.Cryptography.ConvertToPlaintext;

public record ConvertToPlaintextRequest
{
    public required string Chipertext { get; init; }
}

public sealed class ConvertToPlaintextRequestValidator : AbstractValidatorBase<ConvertToPlaintextRequest>
{
    public ConvertToPlaintextRequestValidator()
    {
        _ = RuleFor(x => x.Chipertext)
            .NotEmpty();
    }
}
