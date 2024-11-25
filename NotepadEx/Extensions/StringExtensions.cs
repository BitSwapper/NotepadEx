namespace NotepadEx.Extensions;

public static class StringExtensions
{
    public static string ToUriPath(this string simplePath)
    {
        return "pack://application:,,,/" + simplePath; ;
    }
}
