using System.IO;

namespace NotepadEx.Util;

public static class DirectoryUtil
{
    static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    public static string NotepadExFolderPath = Path.Combine(appDataPath, "NotepadEx/");
    public static string NotepadExThemesPath = Path.Combine(NotepadExFolderPath, "Themes/");
}
