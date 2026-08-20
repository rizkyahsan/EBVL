namespace EBVL.Shared.Enums;

public enum VendorCompanyStatusType
{
    [DisplayText("Manufacture")]
    Manufacture = 100,

    [DisplayText("Agen Distributor Tunggal")]
    SoleDistributorAgent = 200,

    [DisplayText("Agen Resmi")]
    AuthorizedAgent = 300
}
