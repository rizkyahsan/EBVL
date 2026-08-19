namespace EBVL.Shared.Statics.Common.Extensions;

public static class StreamExtensions
{
    public static void CopyFromStream(this MemoryStream memoryStream, Stream stream)
    {
        int read;
        var buffer = new byte[16 * 1024];

        while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
        {
            memoryStream.Write(buffer, 0, read);
        }
    }
}
