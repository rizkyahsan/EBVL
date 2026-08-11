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
}
