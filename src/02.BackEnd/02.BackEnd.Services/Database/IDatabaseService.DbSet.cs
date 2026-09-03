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
    public DbSet<Lender> Lenders { get; }
    public DbSet<PublicHoliday> PublicHolidays { get; }
    public DbSet<Questionnaire> Questionnaires { get; }
    public DbSet<QuestionnaireVersion> QuestionnaireVersions { get; }
    public DbSet<QuestionnaireSection> QuestionnaireSections { get; }
    public DbSet<QuestionnaireQuestion> QuestionnaireQuestions { get; }
    public DbSet<QuestionnaireOption> QuestionnaireOptions { get; }
    public DbSet<QuestionnaireRule> QuestionnaireRules { get; }
    public DbSet<VendorRegistration> VendorRegistrations { get; }
    public DbSet<VendorRegistrationDocument> VendorRegistrationDocuments { get; }
    public DbSet<QuestionnaireSubmission> QuestionnaireSubmissions { get; }
    public DbSet<QuestionnaireAnswer> QuestionnaireAnswers { get; }
    public DbSet<QuestionnaireAnswerOption> QuestionnaireAnswerOptions { get; }
    public DbSet<QuestionnaireAnswerFile> QuestionnaireAnswerFiles { get; }
    public DbSet<User> Users { get; }
}
