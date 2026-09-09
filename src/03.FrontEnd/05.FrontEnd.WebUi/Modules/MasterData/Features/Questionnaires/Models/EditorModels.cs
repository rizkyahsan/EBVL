namespace EBVL.FrontEnd.WebUi.Modules.MasterData.Features.Questionnaires.Models;

public sealed class SectionModel
{
    public Guid Id { get; set; }
    public string BusinessProcess { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public VendorCompanyStatusType? CompanyType { get; set; } = VendorCompanyStatusType.Manufacture;
    public bool IsActive { get; set; } = true;
    public List<QuestionModel> Questions { get; set; } = [];
}

public sealed class QuestionModel
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string? Hint { get; set; }
    public string? Placeholder { get; set; }
    public QuestionnaireQuestionType Type { get; set; } = QuestionnaireQuestionType.ShortText;
    public VendorCompanyStatusType? CompanyType { get; set; } = VendorCompanyStatusType.Manufacture;
    public bool IsRequired { get; set; }
    public bool IsVisible { get; set; } = true;
    public bool IsActive { get; set; } = true;
    public QuestionnaireAnswerRule AnswerRule { get; set; } = QuestionnaireAnswerRule.Optional;
    public int Order { get; set; } = 1;
    public List<OptionModel> Options { get; set; } = [];
}

public sealed class OptionModel { public string Code { get; set; } = string.Empty; public string Label { get; set; } = string.Empty; }
public sealed class RuleModel { public Guid SourceQuestionId { get; set; } public Guid TargetQuestionId { get; set; } public QuestionnaireRuleOperator Operator { get; set; } = QuestionnaireRuleOperator.Equals; public QuestionnaireRuleAction Action { get; set; } = QuestionnaireRuleAction.Show; public string Value { get; set; } = string.Empty; }
