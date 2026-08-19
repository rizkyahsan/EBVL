namespace EBVL.Shared.Dto.Modules.Main.Projects.RevisionProjectLenderReq;

public record RevisionProjectLenderReqRequest
{
    public required Guid Id { get; set; }

    public required string Remarks { get; set; }
}

public sealed class RevisionProjectLenderReqRequestValidator : AbstractValidatorBase<RevisionProjectLenderReqRequest>
{
    public RevisionProjectLenderReqRequestValidator()
    {
        _ = RuleFor(x => x.Id)
            .NotEmpty();

        _ = RuleFor(x => x.Remarks)
            .NotEmpty()
            .MinimumLength(CommonMinimumLengthFor.Notes);
    }
}
