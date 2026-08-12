namespace EBVL.BackEnd.Infrastructure.Database;

public partial class DatabaseService : IDatabaseService
{
    public DbSet<ApiCall> ApiCalls => Set<ApiCall>();
    public DbSet<Audit> Audits => Set<Audit>();
    public DbSet<Configuration> Configurations => Set<Configuration>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<PublicHoliday> PublicHolidays => Set<PublicHoliday>();
    public DbSet<User> Users => Set<User>();
    public DbSet<VendorAccount> VendorAccounts => Set<VendorAccount>();
    public DbSet<VendorRegistration> VendorRegistrations => Set<VendorRegistration>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<VendorType> VendorTypes => Set<VendorType>();
    public DbSet<ContactType> ContactTypes => Set<ContactType>();
    public DbSet<VendorContact> VendorContacts => Set<VendorContact>();
    public DbSet<DocumentTemplate> DocumentTemplates => Set<DocumentTemplate>();
    public DbSet<VendorDocument> VendorDocuments => Set<VendorDocument>();
    public DbSet<VendorRegistrationDocument> VendorRegistrationDocuments => Set<VendorRegistrationDocument>();
    public DbSet<VendorMigrationRun> VendorMigrationRuns => Set<VendorMigrationRun>();
    public DbSet<VendorMigrationRow> VendorMigrationRows => Set<VendorMigrationRow>();
    public DbSet<VendorMigrationCrosswalk> VendorMigrationCrosswalks => Set<VendorMigrationCrosswalk>();
    public DbSet<VendorMigrationQuarantine> VendorMigrationQuarantines => Set<VendorMigrationQuarantine>();
}
