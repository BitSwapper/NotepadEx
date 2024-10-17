using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NotepadEx.Properties;
using NotepadEx.Util;

namespace NotepadEx.Theme;

public static class ThemeManager
{
    public static ColorTheme CurrentTheme { get; set; }

    public static void SetupThemes(MenuItem themeMenuItem)
    {
        AddAllCustomThemes(themeMenuItem);
        LoadCurrentThemeChoice();
    }

    public static void LoadCurrentThemeChoice()
    {
        var themeFiles = new DirectoryInfo(DirectoryUtil.NotepadExThemesPath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();
        var themeFile = themeFiles.Where(themeName => (themeName.Name) == Settings.Default.ThemeName).FirstOrDefault();
        ApplyTheme(Path.GetFileName(themeFile.Name), Application.Current);
    }

    public static void AddAllCustomThemes(MenuItem parentMenu)
    {
        var customThemes = new DirectoryInfo(DirectoryUtil.NotepadExThemesPath).GetFiles().OrderByDescending(f => f.LastWriteTime).ToList();
        foreach(var customTheme in customThemes)
            AddSingleThemeMenuItem(parentMenu, customTheme.Name);
    }

    public static void AddSingleThemeMenuItem(MenuItem parentMenu, string header)
    {
        MenuItem item = new MenuItem { Header = header };
        parentMenu.Items.Add(item);
    }

    public static void ApplyTheme(string themeName, Application currentApp)
    {
        var fileData = File.ReadAllText(DirectoryUtil.NotepadExThemesPath + themeName);
        var themeSerialized = JsonSerializer.Deserialize<ColorThemeSerializable>(fileData);
        var theme = themeSerialized.ToColorTheme();
        Settings.Default.ThemeName = themeName;
        CurrentTheme = theme;

        ApplyThemeObject(theme.themeObj_TextEditorBg, UIConstants.Color_TextEditorBg);
        ApplyThemeObject(theme.themeObj_TextEditorFg, UIConstants.Color_TextEditorFg);
        ApplyThemeObject(theme.themeObj_TitleBarBg, UIConstants.Color_TitleBarBg);
        ApplyThemeObject(theme.themeObj_TitleBarFont, UIConstants.Color_TitleBarFont);

        ApplyThemeObject(theme.themeObj_SystemButtons, UIConstants.Color_SystemButtons);
        ApplyThemeObject(theme.themeObj_BorderColor, UIConstants.Color_BorderColor);
        ApplyThemeObject(theme.themeObj_MenuBarBg, UIConstants.Color_MenuBarBg);
        ApplyThemeObject(theme.themeObj_MenuItemFg, UIConstants.Color_MenuItemFg);
        ApplyThemeObject(theme.themeObj_InfoBarBg, UIConstants.Color_InfoBarBg);
        ApplyThemeObject(theme.themeObj_InfoBarFg, UIConstants.Color_InfoBarFg);

        ApplyThemeObject(theme.themeObj_MenuBorder, UIConstants.Color_MenuBorder);
        ApplyThemeObject(theme.themeObj_MenuBg, UIConstants.Color_MenuBg);
        ApplyThemeObject(theme.themeObj_MenuFg, UIConstants.Color_MenuFg);
        ApplyThemeObject(theme.themeObj_MenuSeperator, UIConstants.Color_MenuSeperator);
        ApplyThemeObject(theme.themeObj_MenuDisabledFg, UIConstants.Color_MenuDisabledFg);
        ApplyThemeObject(theme.themeObj_MenuItemSelectedBg, UIConstants.Color_MenuItemSelectedBg);
        ApplyThemeObject(theme.themeObj_MenuItemSelectedBorder, UIConstants.Color_MenuItemSelectedBorder);
        ApplyThemeObject(theme.themeObj_MenuItemHighlightBg, UIConstants.Color_MenuItemHighlightBg);
        ApplyThemeObject(theme.themeObj_MenuItemHighlightBorder, UIConstants.Color_MenuItemHighlightBorder);


        void ApplyThemeObject(ThemeObject themeObj, string resourceKey)
        {
            if(themeObj != null)
            {
                if(themeObj.isGradient)
                    AppResourceUtil<LinearGradientBrush>.TrySetResource(currentApp, resourceKey, themeObj.gradient);
                else
                    AppResourceUtil<SolidColorBrush>.TrySetResource(currentApp, resourceKey, new SolidColorBrush(themeObj.color.GetValueOrDefault()));
            }
        }
    }
}
