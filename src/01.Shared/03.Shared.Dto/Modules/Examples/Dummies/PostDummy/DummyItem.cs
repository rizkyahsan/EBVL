namespace EBVL.Shared.Dto.Modules.Examples.Dummies.PostDummy;

public sealed record DummyItem
{
    public required int Iterasi { get; init; }
    public required int Angka1 { get; init; }
    public required int Angka2 { get; init; }
    public required int Hasil { get; init; }
    public required string Keterangan { get; init; }
    public required DateOnly SuatuDateOnly { get; init; }
    public required DateTime SuatuDateTime { get; init; }
    public required DateTimeOffset SuatuDateTimeOffset { get; init; }
}
