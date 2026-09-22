using EBVL.BackEnd.Domain.Entities;
using EBVL.BackEnd.Logics.Modules.MasterData.Documents.Common;
using EBVL.Shared.Enums;
using FluentValidation;
using Xunit;

namespace EBVL.BackEnd.Logics.Tests;

public sealed class DocumentRequirementSetLifecycleTests
{
    [Fact]
    public void CloneCreatesNextDraftAndPreservesPublishedDefinition()
    {
        var source = PublishedSet();
        var original = source.Requirements.Single();
        var draft = DocumentRequirementSetGraph.Clone(source, 2);

        Assert.Equal(source.DocumentRequirementSetSeriesId, draft.DocumentRequirementSetSeriesId);
        Assert.Equal(source.Id, draft.PreviousVersionId);
        Assert.Equal(QuestionnaireStatus.Draft, draft.Status);
        Assert.NotEqual(original.Id, draft.Requirements.Single().Id);
        Assert.Equal(original.Code, draft.Requirements.Single().Code);
        draft.Requirements.Single().Name = "Changed";
        Assert.Equal("Tax document", original.Name);
    }

    [Theory]
    [InlineData(QuestionnaireStatus.Publish)]
    [InlineData(QuestionnaireStatus.Superseded)]
    public void HistoricalSetsAreImmutable(QuestionnaireStatus status)
    {
        var set = PublishedSet();
        set.Status = status;
        _ = Assert.Throws<ValidationException>(() => DocumentRequirementSetGraph.EnsureDraft(set));
    }

    [Fact]
    public void MappingReturnsConcreteSetAndRequirementIds()
    {
        var set = PublishedSet();
        var item = DocumentRequirementSetGraph.Map(set);
        Assert.Equal(set.Id, item.Id);
        Assert.Equal(set.Requirements.Single().Id, item.Requirements.Single().Id);
        Assert.Equal(set.Id, item.Requirements.Single().DocumentRequirementSetId);
    }

    private static DocumentRequirementSet PublishedSet()
    {
        var id = Guid.NewGuid();
        var set = new DocumentRequirementSet { Id = id, DocumentRequirementSetSeriesId = Guid.NewGuid(), BusinessProcess = "Vendor Registration", Version = 1, Status = QuestionnaireStatus.Publish, RowVersion = new byte[8] };
        set.Requirements.Add(new DocumentDefinition { Id = Guid.NewGuid(), DocumentRequirementSetId = id, Code = "TAX", BusinessProcess = set.BusinessProcess, Name = "Tax document", Order = 1, MaxSizeMb = 50, IsMandatory = true, IsActive = true });
        return set;
    }
}
