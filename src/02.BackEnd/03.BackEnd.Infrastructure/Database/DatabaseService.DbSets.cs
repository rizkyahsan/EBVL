namespace EBVL.BackEnd.Infrastructure.Database;

public partial class DatabaseService : IDatabaseService
{
    public DbSet<ApiCall> ApiCalls => Set<ApiCall>();
    public DbSet<Audit> Audits => Set<Audit>();
    public DbSet<Configuration> Configurations => Set<Configuration>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();
    public DbSet<ExternalLogin> ExternalLogins => Set<ExternalLogin>();
    public DbSet<ExternalLoginLog> ExternalLoginLogs => Set<ExternalLoginLog>();
    public DbSet<Domain.Entities.FileStorage> FileStorages => Set<Domain.Entities.FileStorage>();
    public DbSet<LogEmail> LogEmails => Set<LogEmail>();
    public DbSet<LogTransaction> LogTransactions => Set<LogTransaction>();
    public DbSet<Lender> Lenders => Set<Lender>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectAttachment> ProjectAttachments => Set<ProjectAttachment>();
    public DbSet<ProjectFile> ProjectFiles => Set<ProjectFile>();
    public DbSet<ProjectLender> ProjectLenders => Set<ProjectLender>();
    public DbSet<ProjectLenderHistory> ProjectLenderHistories => Set<ProjectLenderHistory>();
    public DbSet<ProjectLenderReq> ProjectLenderReqs => Set<ProjectLenderReq>();
    public DbSet<ProjectLenderReqFile> ProjectLenderReqFiles => Set<ProjectLenderReqFile>();
    public DbSet<ProjectReq> ProjectReqs => Set<ProjectReq>();
    public DbSet<ProjectStage> ProjectStages => Set<ProjectStage>();
    public DbSet<PublicHoliday> PublicHolidays => Set<PublicHoliday>();
    public DbSet<Status> Statuses => Set<Status>();
    public DbSet<User> Users => Set<User>();
}
