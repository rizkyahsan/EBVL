using Microsoft.EntityFrameworkCore;
using EBVL.BackEnd.Domain.Entities;

namespace EBVL.BackEnd.Services.Database;

public partial interface IDatabaseService
{
    public DbSet<ApiCall> ApiCalls { get; }
    public DbSet<Audit> Audits { get; }
    public DbSet<Configuration> Configurations { get; }
    public DbSet<Country> Countries { get; }
    public DbSet<Document> Documents { get; }
    public DbSet<PublicHoliday> PublicHolidays { get; }
    public DbSet<User> Users { get; }
    public DbSet<VendorAccount> VendorAccounts { get; }
    public DbSet<VendorRegistration> VendorRegistrations { get; }
    public DbSet<Vendor> Vendors { get; }
    public DbSet<VendorType> VendorTypes { get; }
    public DbSet<ContactType> ContactTypes { get; }
    public DbSet<VendorContact> VendorContacts { get; }
    public DbSet<DocumentTemplate> DocumentTemplates { get; }
    public DbSet<VendorDocument> VendorDocuments { get; }
    public DbSet<VendorRegistrationDocument> VendorRegistrationDocuments { get; }
    public DbSet<VendorMigrationRun> VendorMigrationRuns { get; }
    public DbSet<VendorMigrationRow> VendorMigrationRows { get; }
    public DbSet<VendorMigrationCrosswalk> VendorMigrationCrosswalks { get; }
    public DbSet<VendorMigrationQuarantine> VendorMigrationQuarantines { get; }
}
