namespace EBVL.Shared.Dto.Modules.Log.LogTransactions.GetLogTransaction;

public record GetLogTransactionRequest
{
    public required Guid Id { get; init; }
}

public sealed class GetLogTransactionRequestValidator : AbstractValidatorBase<GetLogTransactionRequest>
{
    public GetLogTransactionRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();
    }
}
