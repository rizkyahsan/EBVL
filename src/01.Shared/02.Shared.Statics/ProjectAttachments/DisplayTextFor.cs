using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.ProjectAttachments;

public static class DisplayTextFor
{
    public static readonly string ProjectAttachment = nameof(ProjectAttachment).SplitWords();
    public static readonly string ProjectAttachments = nameof(ProjectAttachments).SplitWords();
    public static readonly string ProjectAttachmentLabel = "Document Template";

    public const string Attachment = nameof(Attachment);
    public const string Name = nameof(Name);
    public const string Desc = nameof(Desc);
    public const string SortNo = "View Order";
    public const string File = nameof(File);
}
