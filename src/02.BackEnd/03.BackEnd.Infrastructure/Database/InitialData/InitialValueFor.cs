namespace EBVL.BackEnd.Infrastructure.Database.InitialData;

public static class InitialValueFor
{
    public static readonly DateTimeOffset Created = new(2025, 7, 5, 1, 2, 3, TimeSpan.FromHours(7));
    public const string CreatedBy = "SYSTEM EBVL";
    public const string Notes = "Seeding";
}
