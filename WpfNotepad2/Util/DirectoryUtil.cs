namespace NotepadEx.Util;

public static class DirectoryUtil
{
    static string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    public static string NotepadExFolderPath = appDataPath + "\\NotepadEx\\";
    public static string NotepadExThemesPath = NotepadExFolderPath + "Themes\\";
}
