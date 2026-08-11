namespace EBVL.BackEnd.Infrastructure.Database.InitialData;

public static class InitialCountries
{
    public static readonly Country Indonesia = new()
    {
        Id = new Guid("01980c88-acfb-7363-9473-64917b3534d4"),
        Name = "Indonesia",
        Code = "IDN",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Country Japan = new()
    {
        Id = new Guid("01980c88-acfb-754a-8b18-a55e47270ed4"),
        Name = "Japan",
        Code = "JPN",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Country SouthKorea = new()
    {
        Id = new Guid("01980c88-acfb-7277-9a9d-aecd0ddf4481"),
        Name = "South Korea",
        Code = "KOR",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Country UnitedKingdom = new()
    {
        Id = new Guid("01980c88-acfb-79b0-9fa7-c817586f97d5"),
        Name = "United Kingdom",
        Code = "GBR",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Country UnitedStates = new()
    {
        Id = new Guid("01980c88-acfb-7445-9b14-7d3caa3cdd9d"),
        Name = "United States",
        Code = "USA",
        Created = InitialValueFor.Created,
        CreatedBy = InitialValueFor.CreatedBy
    };

    public static readonly Country[] All =
    [
        Indonesia,
        Japan,
        SouthKorea,
        UnitedKingdom,
        UnitedStates
    ];
}
