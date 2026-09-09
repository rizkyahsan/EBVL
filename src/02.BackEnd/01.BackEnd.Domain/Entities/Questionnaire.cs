namespace EBVL.BackEnd.Domain.Entities;

public sealed class Questionnaire : ModifiableEntity
{
    public required string Code { get; set; }
    public required string BusinessProcess { get; set; }
    public bool IsActive { get; set; }
    public byte[] RowVersion { get; set; } = [];
    public ICollection<QuestionnaireSection> Sections { get; set; } = new HashSet<QuestionnaireSection>();
    public ICollection<QuestionnaireRule> Rules { get; set; } = new HashSet<QuestionnaireRule>();
}

public sealed class QuestionnaireSection : ModifiableEntity
{
    public Guid QuestionnaireId { get; set; }
    public required string Code { get; set; }
    public required string Title { get; set; }
    public int Order { get; set; }
    public VendorCompanyStatusType? CompanyType { get; set; }
    public bool IsActive { get; set; } = true;
    public Questionnaire Questionnaire { get; set; } = default!;
    public ICollection<QuestionnaireQuestion> Questions { get; set; } = new HashSet<QuestionnaireQuestion>();
}

public sealed class QuestionnaireQuestion : ModifiableEntity
{
    public Guid QuestionnaireSectionId { get; set; }
    public required string Code { get; set; }
    public required string Label { get; set; }
    public string? Hint { get; set; }
    public string? Placeholder { get; set; }
    public QuestionnaireQuestionType Type { get; set; }
    public VendorCompanyStatusType? CompanyType { get; set; }
    public int Order { get; set; }
    public bool IsRequired { get; set; }
    public bool IsVisible { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public QuestionnaireAnswerRule AnswerRule { get; set; }
    public QuestionnaireSection QuestionnaireSection { get; set; } = default!;
    public ICollection<QuestionnaireOption> Options { get; set; } = new HashSet<QuestionnaireOption>();
}

public sealed class QuestionnaireOption : ModifiableEntity
{
    public Guid QuestionnaireQuestionId { get; set; }
    public required string Code { get; set; }
    public required string Label { get; set; }
    public int Order { get; set; }
    public QuestionnaireQuestion QuestionnaireQuestion { get; set; } = default!;
}

public sealed class QuestionnaireRule : ModifiableEntity
{
    public Guid QuestionnaireId { get; set; }
    public Guid SourceQuestionId { get; set; }
    public Guid TargetQuestionId { get; set; }
    public QuestionnaireRuleOperator Operator { get; set; }
    public QuestionnaireRuleAction Action { get; set; }
    public required string Value { get; set; }
    public Questionnaire Questionnaire { get; set; } = default!;
}
