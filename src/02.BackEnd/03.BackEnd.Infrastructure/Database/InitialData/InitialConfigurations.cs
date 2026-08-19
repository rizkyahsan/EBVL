using EBVL.Shared.Statics.Configurations;

namespace EBVL.BackEnd.Infrastructure.Database.InitialData;

public static class InitialConfigurations
{
    public static readonly Configuration DefaultPass = new()
    {
        Id = new Guid("8ac5b6a3-34c2-435c-80fb-750f91a3ac0b"),
        Key = KeyFor.DefaultPass,
        Value = "EBVL@123"
    };

    public static readonly Configuration TokenExpiredHour = new()
    {
        Id = new Guid("a6d71541-ad08-4423-8ccf-59ac7c296bfe"),
        Key = KeyFor.TokenExpiredHour,
        Value = "48"
    };

    public static readonly Configuration DefaultFrom = new()
    {
        Id = new Guid("dba2ce02-f18e-4403-b78f-478cf910afbc"),
        Key = KeyFor.DefaultFrom,
        Value = "noreply@pertamina.com"
    };

    public static readonly Configuration DefaultAdminEmail = new()
    {
        Id = new Guid("0d94f581-debf-4c69-a457-58481ed4b6bd"),
        Key = KeyFor.DefaultAdminEmail,
        Value = "noreply@pertamina.com"
    };

    public static readonly Configuration CompanyName = new()
    {
        Id = new Guid("54016299-d398-4a1b-9157-fc3e22d31fe2"),
        Key = KeyFor.CompanyName,
        Value = "PT. PERTAMINA PATRA NIAGA"
    };

    public static readonly Configuration[] All =
    [
        DefaultAdminEmail,
        DefaultFrom,
        DefaultPass,
        TokenExpiredHour,
        CompanyName
    ];
}
