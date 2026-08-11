using Pertamina.Common.Dto.Attributes;

namespace EBVL.Shared.Dto.Modules.Examples.Dummies.PostDummy;

public record PostDummyRequest
{
    public int Angka1 { get; set; }

    [Sensitive]
    public int Angka2 { get; set; }

    public DateOnly SuatuDateOnly { get; set; }
    public DateTime SuatuDateTime { get; set; }
    public DateTimeOffset SuatuDateTimeOffset { get; set; }
}

public sealed class PostDummyRequestValidator : AbstractValidatorBase<PostDummyRequest>
{
    public PostDummyRequestValidator()
    {
        _ = RuleFor(x => x.Angka1)
            .GreaterThan(100)
            .WithMessage("Angka1 harus lebih besar dari 100.");

        _ = RuleFor(x => x.Angka1)
            .GreaterThan(x => x.Angka2)
            .WithMessage("Angka1 harus lebih besar dari Angka2.");

        _ = RuleFor(x => x.Angka2)
            .GreaterThan(3)
            .WithMessage("Angka2 harus lebih besar dari 3.");
    }
}
