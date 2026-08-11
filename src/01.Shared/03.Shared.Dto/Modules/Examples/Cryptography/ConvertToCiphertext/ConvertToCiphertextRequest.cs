namespace EBVL.Shared.Dto.Modules.Examples.Cryptography.ConvertToCiphertext;

public record ConvertToCiphertextRequest
{
    public required string Plaintext { get; init; }
}

public sealed class ConvertToCiphertextRequestValidator : AbstractValidatorBase<ConvertToCiphertextRequest>
{
    public ConvertToCiphertextRequestValidator()
    {
        _ = RuleFor(x => x.Plaintext)
            .NotEmpty();
    }
}
