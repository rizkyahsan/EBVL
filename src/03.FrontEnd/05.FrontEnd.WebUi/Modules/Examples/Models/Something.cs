namespace EBVL.FrontEnd.WebUi.Modules.Examples.Models;

public sealed record Something
{
    public required string Name { get; set; }
}

public sealed class SomethingValidator : AbstractValidatorBase<Something>
{
    public SomethingValidator()
    {
        _ = RuleFor(x => x.Name)
            .MinimumLength(3)
            .MaximumLength(100);
    }
}
