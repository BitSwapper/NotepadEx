using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WpfNotepad2.Properties;
using WpfNotepad2.Util;

namespace WpfNotepad2;
public static class SettingsManager
{
    public static void SaveSettings()
    {
        //Settings.Default.FontSize = txtEditor.FontSize;
        //Settings.Default.FontFamily = txtEditor.FontFamily.Source;
        //Settings.Default.FontWeight = txtEditor.FontWeight.ToString();
        //Settings.Default.FontStyle = txtEditor.FontStyle.ToString();
        //Settings.Default.Underline = txtEditor.TextDecorations.Any(td => td.Location == TextDecorationLocation.Underline);
        //Settings.Default.Strikethrough = txtEditor.TextDecorations.Any(td => td.Location == TextDecorationLocation.Strikethrough);
        //Settings.Default.TextWrapping = txtEditor.TextWrapping == TextWrapping.Wrap;
        //Settings.Default.Theme = curTheme.ToString();
        //Settings.Default.WindowSizeX = this.Width;
        //Settings.Default.WindowSizeY = this.Height;
        Settings.Default.RecentFiles = string.Join(",", RecentFileManager.RecentFiles);
        //Settings.Default.CustomThemeName = customThemeName;
        Settings.Default.Save();
    }
}
