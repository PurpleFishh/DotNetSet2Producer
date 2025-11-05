namespace Producer.Infrastructure.Utils;

public static class FileStreamExtensions
{
    public static char GetLastChar(this FileStream fs)
    {
        if (fs.Length == 0) return '\0';

        var pos = fs.Length - 1;
        int b;
        do
        {
            fs.Seek(pos, SeekOrigin.Begin);
            b = fs.ReadByte();
            pos--;
        } while (pos >= 0 && b is 0x20 or 0x09 or 0x0A or 0x0D); // space, tab, LF, CR

        return (char)b;
    }
}