namespace NotepadEx.Theme;

public static class ThemeManager
{
    static ColorTheme curTheme;
    public static ColorTheme CurrentTheme { get => curTheme; set => curTheme = value; }
}
