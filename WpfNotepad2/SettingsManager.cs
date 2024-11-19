using System.Windows;
using System.Windows.Controls;
using NotepadEx.Properties;
using NotepadEx.Util;

namespace NotepadEx;
public static class SettingsManager
{
    public static void SaveSettings(Window window, TextBox txtEditor, string themeName)
    {
        Settings.Default.RecentFiles = string.Join(",", RecentFileManager.RecentFiles);
        Settings.Default.WindowSizeX = window.Width;
        Settings.Default.WindowSizeY = window.Height;
        Settings.Default.TextWrapping = txtEditor.TextWrapping == TextWrapping.Wrap;
        //Settings.Default.MenuBarAutoHide;
        //Settings.Default.InfoBarAutoHide;
        //Settings.Default.ThemeName = themeName;
        Settings.Default.FontSize = txtEditor.FontSize;
        Settings.Default.FontFamily = txtEditor.FontFamily.Source;
        Settings.Default.FontWeight = txtEditor.FontWeight.ToString();
        Settings.Default.FontStyle = txtEditor.FontStyle.ToString();
        Settings.Default.Underline = txtEditor.TextDecorations.Any(td => td.Location == TextDecorationLocation.Underline);
        Settings.Default.Strikethrough = txtEditor.TextDecorations.Any(td => td.Location == TextDecorationLocation.Strikethrough);

        Settings.Default.Save();
    }
}
