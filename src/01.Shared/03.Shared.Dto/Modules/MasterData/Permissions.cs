namespace EBVL.Shared.Dto.Modules.MasterData;

public static class Permissions
{
    public const string MasterData = "fino.md";
    public const string MasterDataCountriesRead = "fino.md.co.read";
    public const string MasterDataCountriesWrite = "fino.md.co.write";
    public const string MasterDataLendersRead = "fino.md.le.read";
    public const string MasterDataLendersWrite = "fino.md.le.write";
    public const string MasterDataUsersRead = "fino.md.us.read";
    public const string MasterDataUsersWrite = "fino.md.us.write";

    public static readonly string[] All =
    [
        MasterData,
        MasterDataCountriesRead,
        MasterDataCountriesWrite,
        MasterDataLendersRead,
        MasterDataLendersWrite,
        MasterDataUsersRead,
        MasterDataUsersWrite
    ];
}
