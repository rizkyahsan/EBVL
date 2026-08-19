using EBVL.Shared.Statics.Common.Extensions;

namespace EBVL.Shared.Statics.FileStorages;

public static class DisplayTextFor
{
    public static readonly string FileStorage = nameof(FileStorage).SplitWords();
    public static readonly string FileStorages = nameof(FileStorages).SplitWords();

    public static readonly string FileName = nameof(FileName).SplitWords();
    public static readonly string SortNo = nameof(SortNo).SplitWords();
    public static readonly string FileExtension = nameof(FileExtension).SplitWords();
    public static readonly string ContentType = nameof(ContentType).SplitWords();
    public static readonly string IsFileStorage = nameof(IsFileStorage).SplitWords();
    public static readonly string FileSize = nameof(FileSize).SplitWords();
    public static readonly string FileData = nameof(FileData).SplitWords();
    public static readonly string FileHash = nameof(FileHash).SplitWords();
}
