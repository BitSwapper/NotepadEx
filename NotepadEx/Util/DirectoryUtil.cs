using System.IO;

namespace NotepadEx.Util;

public static class DirectoryUtil
{
    static readonly string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    public static readonly string ExecutablePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

    public static readonly string AppName = "NotepadEx";
    // Get the directory where the executable is located

    // Define paths relative to the executable location
    public static readonly string NotepadExFolderPath = ExecutablePath + Path.DirectorySeparatorChar;
    public static readonly string NotepadExThemesPath = NotepadExFolderPath + "Themes\\";

    public static readonly string ImagePath_MainIcon = "Images/NotepadEx.png";
}
