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
    public DbSet<Lender> Lenders => Set<Lender>();
    public DbSet<PublicHoliday> PublicHolidays => Set<PublicHoliday>();
    public DbSet<Questionnaire> Questionnaires => Set<Questionnaire>();
    public DbSet<QuestionnaireVersion> QuestionnaireVersions => Set<QuestionnaireVersion>();
    public DbSet<QuestionnaireSection> QuestionnaireSections => Set<QuestionnaireSection>();
    public DbSet<QuestionnaireQuestion> QuestionnaireQuestions => Set<QuestionnaireQuestion>();
    public DbSet<QuestionnaireOption> QuestionnaireOptions => Set<QuestionnaireOption>();
    public DbSet<QuestionnaireRule> QuestionnaireRules => Set<QuestionnaireRule>();
    public DbSet<VendorRegistration> VendorRegistrations => Set<VendorRegistration>();
    public DbSet<VendorRegistrationDocument> VendorRegistrationDocuments => Set<VendorRegistrationDocument>();
    public DbSet<QuestionnaireSubmission> QuestionnaireSubmissions => Set<QuestionnaireSubmission>();
    public DbSet<QuestionnaireAnswer> QuestionnaireAnswers => Set<QuestionnaireAnswer>();
    public DbSet<QuestionnaireAnswerOption> QuestionnaireAnswerOptions => Set<QuestionnaireAnswerOption>();
    public DbSet<QuestionnaireAnswerFile> QuestionnaireAnswerFiles => Set<QuestionnaireAnswerFile>();
    public DbSet<User> Users => Set<User>();
}
