using EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.Common;
using EBVL.Shared.Dto.Modules.MasterData.Questionnaires;

namespace EBVL.BackEnd.Logics.Modules.MasterData.Questionnaires.UpdateQuestionnaireSection;

[AuthorizeRequestByPermission(QuestionnairePermissions.Manage)]
public sealed record UpdateQuestionnaireSectionCommand(Guid QuestionnaireId, Guid SectionId, UpdateQuestionnaireSectionRequest Section) : IRequest<GetQuestionnaireResponse>;

public sealed class UpdateQuestionnaireSectionCommandValidator : AbstractValidatorBase<UpdateQuestionnaireSectionCommand>
{
    public UpdateQuestionnaireSectionCommandValidator()
    {
        _ = RuleFor(x => x.Section).SetValidator(new UpdateQuestionnaireSectionRequestValidator());
    }
}

public sealed class UpdateQuestionnaireSectionHandler(IDatabaseService db) : IRequestHandler<UpdateQuestionnaireSectionCommand, GetQuestionnaireResponse>
{
    public async Task<GetQuestionnaireResponse> Handle(UpdateQuestionnaireSectionCommand r, CancellationToken cancellationToken)
    {
        await using var tx = await db.BeginTransactionAsync(cancellationToken);
        var questionnaire = await QuestionnaireGraph.Load(db, r.QuestionnaireId, true, cancellationToken);
        QuestionnaireGraph.EnsureDraft(questionnaire);
        await QuestionnaireGraph.EnsureNotReferenced(db, questionnaire.Id, cancellationToken);
        db.SetQuestionnaireOriginalRowVersion(questionnaire, QuestionnaireGraph.ParseRowVersion(r.Section.RowVersion));
        questionnaire.Modified = DateTimeOffset.UtcNow;
        var section = questionnaire.Sections.SingleOrDefault(item => item.Id == r.SectionId && !item.IsDeleted) ?? throw new KeyNotFoundException("Questionnaire section was not found.");
        var code = r.Section.Code?.Trim() ?? section.Code;
        if (questionnaire.Sections.Any(item => item.Id != section.Id && !item.IsDeleted && item.Code.Equals(code, StringComparison.OrdinalIgnoreCase)))
        {
            throw new ValidationException("A section with the same code already exists in this questionnaire version.");
        }

        questionnaire.BusinessProcess = r.Section.BusinessProcess.Trim();
        section.Code = code;
        section.Title = r.Section.Section.Trim();
        section.CompanyType = r.Section.VendorType;
        section.IsActive = r.Section.IsActive;
        if (r.Section.Order is { } requestedOrder)
        {
            section.Order = Math.Clamp(requestedOrder, 1, questionnaire.Sections.Count);
            var nextOrder = 1;
            foreach (var item in questionnaire.Sections.Where(item => item.Id != section.Id && !item.IsDeleted).OrderBy(item => item.Order))
            {
                if (nextOrder == section.Order)
                {
                    nextOrder++;
                }

                item.Order = nextOrder++;
            }
        }

        await QuestionnaireGraph.SaveMutation(db, nameof(UpdateQuestionnaireSectionCommand), cancellationToken);
        await tx.CommitAsync(cancellationToken);
        return new() { Item = await QuestionnaireGraph.MapWithState(db, await QuestionnaireGraph.Load(db, questionnaire.Id, false, cancellationToken), cancellationToken) };
    }
}
