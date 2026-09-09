namespace EBVL.Shared.Enums;

#pragma warning disable CA1720

public enum QuestionnaireQuestionType { ShortText = 100, LongText = 200, Boolean = 300, Integer = 400, Decimal = 500, Date = 600, SingleChoice = 700, MultipleChoice = 800, Address = 900, File = 1000 }
public enum QuestionnaireAnswerRule { Mandatory = 100, Optional = 200, AddedValue = 300 }
public enum QuestionnaireRuleOperator { Equals = 100, NotEquals = 200, Contains = 300, GreaterThan = 400, LessThan = 500 }
public enum QuestionnaireRuleAction { Show = 100, Hide = 200, Require = 300 }
public enum VendorRegistrationStatus { Draft = 100, Submitted = 200 }
#pragma warning restore CA1720
