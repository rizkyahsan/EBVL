using EBVL.BackEnd.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EBVL.BackEnd.Services.Database;

public partial interface IDatabaseService
{
    public DbSet<ApiCall> ApiCalls { get; }
    public DbSet<Audit> Audits { get; }
    public DbSet<Configuration> Configurations { get; }
    public DbSet<Country> Countries { get; }
    public DbSet<Document> Documents { get; }
    public DbSet<EmailTemplate> EmailTemplates { get; }
    public DbSet<ExternalLogin> ExternalLogins { get; }
    public DbSet<ExternalLoginLog> ExternalLoginLogs { get; }
    public DbSet<FileStorage> FileStorages { get; }
    public DbSet<LogEmail> LogEmails { get; }
    public DbSet<LogTransaction> LogTransactions { get; }
    public DbSet<Lender> Lenders { get; }
    public DbSet<Project> Projects { get; }
    public DbSet<ProjectAttachment> ProjectAttachments { get; }
    public DbSet<ProjectFile> ProjectFiles { get; }
    public DbSet<ProjectLender> ProjectLenders { get; }
    public DbSet<ProjectLenderHistory> ProjectLenderHistories { get; }
    public DbSet<ProjectLenderReq> ProjectLenderReqs { get; }
    public DbSet<ProjectLenderReqFile> ProjectLenderReqFiles { get; }
    public DbSet<ProjectReq> ProjectReqs { get; }
    public DbSet<ProjectStage> ProjectStages { get; }
    public DbSet<PublicHoliday> PublicHolidays { get; }
    public DbSet<Status> Statuses { get; }
    public DbSet<User> Users { get; }
}
