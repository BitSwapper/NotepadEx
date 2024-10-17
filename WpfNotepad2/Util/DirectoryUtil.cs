namespace NotepadEx.Util;

public static class DirectoryUtil
{
    static readonly string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

    public static readonly string AppName = "NotepadEx";
    public static readonly string NotepadExFolderPath = appDataPath + "\\NotepadEx\\";
    public static readonly string NotepadExThemesPath = NotepadExFolderPath + "Themes\\";
    public static readonly string ImagePath_MainIcon = "Images/NotepadEx.png";
}
