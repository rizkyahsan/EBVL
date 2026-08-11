namespace EBVL.Shared.Dto.Modules.MasterData;

public static class Permissions
{
    public const string MasterData = "SolTem2.MasterData";  // Digunakan untuk memunculkan menu Master Data
    public const string MasterDataCountriesRead = "SolTem2.MasterData.Countries.Read"; // Digunakan juga untuk memunculkan submenu Countries di Master Data
    public const string MasterDataCountriesWrite = "SolTem2.MasterData.Countries.Write";

    public static readonly string[] All =
    [
        MasterData,
        MasterDataCountriesRead,
        MasterDataCountriesWrite
    ];
}
