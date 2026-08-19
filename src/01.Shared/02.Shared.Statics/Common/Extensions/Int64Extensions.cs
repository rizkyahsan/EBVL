namespace EBVL.Shared.Statics.Common.Extensions;

public static class Int64Extensions
{
    public static string ToReadableFileSize(this long fileSize)
    {
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        var length = Convert.ToDouble(fileSize);
        var order = 0;

        while (length >= 1024 && order < sizes.Length - 1)
        {
            order++;
            length /= 1024;
        }

        return string.Format("{0:0.##} {1}", length, sizes[order]);
    }
}
