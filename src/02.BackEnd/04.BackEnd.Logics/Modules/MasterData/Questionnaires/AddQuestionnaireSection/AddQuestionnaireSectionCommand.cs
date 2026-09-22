using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.AddQuestionnaireSection;

[AuthorizeRequestByPermission(QuestionnairePermissions.Manage)]
public sealed record AddQuestionnaireSectionCommand(Guid QuestionnaireId, AddQuestionnaireSectionRequest Section) : IRequest<GetQuestionnaireResponse>;

public sealed class AddQuestionnaireSectionCommandValidator : AbstractValidatorBase<AddQuestionnaireSectionCommand>
{
    public AddQuestionnaireSectionCommandValidator()
    {
        _ = RuleFor(x => x.Section).SetValidator(new AddQuestionnaireSectionRequestValidator());
    }
}

public sealed class AddQuestionnaireSectionHandler(IDatabaseService db) : IRequestHandler<AddQuestionnaireSectionCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(AddQuestionnaireSectionCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var q = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(q);
        await QuestionnaireGraph.EnsureNotReferenced(db, q.Id, cancellationToken);
        QuestionnaireGraph.EnsureRowVersion(q, r.Section.RowVersion);
        var code = r.Section.Code.Trim();
        if (q.Sections.Any(x => !x.IsDeleted && x.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException("A section with the same code already exists in this questionnaire version.");
        }

        var lastOrder = q.Sections.Where(x => !x.IsDeleted).Select(x => x.Order).DefaultIfEmpty().Max();
        var order = r.Section.Order <= 0 || r.Section.Order > lastOrder ? lastOrder + 1 : r.Section.Order;
        foreach (var item in q.Sections.Where(x => !x.IsDeleted && x.Order >= order))
        {
            item.Order++;
        }

        q.BusinessProcess = r.Section.BusinessProcess.Trim();
        q.Modified = DateTimeOffset.UtcNow;
        var section = new QuestionnaireSection { Id = Guid.CreateVersion7(), QuestionnaireId = q.Id, Code = code, Title = r.Section.Title.Trim(), CompanyType = r.Section.VendorType, Order = order, IsActive = r.Section.IsActive };
        _ = db.QuestionnaireSections.Add(section);
        await QuestionnaireGraph.SaveMutation(db, nameof(AddQuestionnaireSectionCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, q, cancellationToken) };
    }
}
