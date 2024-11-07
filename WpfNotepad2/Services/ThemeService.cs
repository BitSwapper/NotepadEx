using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;
using NotepadEx.MVVM.Models;
using NotepadEx.MVVM.View;
using NotepadEx.Properties;
using NotepadEx.Services.Interfaces;
using NotepadEx.Theme;
using NotepadEx.Util;

namespace NotepadEx.Services;

public class ThemeService : IThemeService
{
    public event EventHandler<ThemeChangedEventArgs> ThemeChanged;
    public ColorTheme CurrentTheme { get; private set; }
    public string CurrentThemeName { get; private set; }
    public ObservableCollection<ThemeInfo> AvailableThemes { get; private set; }

    readonly Application _application;

    public ThemeService(Application application)
    {
        _application = application;
        AvailableThemes = new ObservableCollection<ThemeInfo>();
        LoadAvailableThemes();
    }

    public void LoadCurrentTheme()
    {
        var themeFiles = new DirectoryInfo(DirectoryUtil.NotepadExThemesPath)
        .GetFiles()
        .OrderByDescending(f => f.LastWriteTime);

        var themeFile = themeFiles.FirstOrDefault(t => t.Name == Settings.Default.ThemeName);
        if(themeFile != null)
        {
            ApplyTheme(themeFile.Name);
        }
    }

    public void ApplyTheme(string themeName)
    {
        var fileData = File.ReadAllText(Path.Combine(DirectoryUtil.NotepadExThemesPath, themeName));
        var themeSerialized = JsonSerializer.Deserialize<ColorThemeSerializable>(fileData);
        var theme = themeSerialized.ToColorTheme();
        Settings.Default.ThemeName = themeName;
        Settings.Default.Save();
        CurrentTheme = theme;
        CurrentThemeName = themeName;

        // Apply all theme objects
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

        // Raise theme changed event if needed
        ThemeChanged?.Invoke(this, new ThemeChangedEventArgs(themeName, theme));
    }


    public void OpenThemeEditor()
    {
        var themeEditorWindow = new ThemeEditorWindow(this);
        themeEditorWindow.ShowDialog();
        LoadAvailableThemes(); // Refresh themes after editor closes
    }

    void LoadAvailableThemes()
    {
        AvailableThemes.Clear();
        var themeFiles = new DirectoryInfo(DirectoryUtil.NotepadExThemesPath)
        .GetFiles()
        .OrderByDescending(f => f.LastWriteTime);

        foreach(var file in themeFiles)
        {
            AvailableThemes.Add(new ThemeInfo
            {
                Name = file.Name,
                FilePath = file.FullName,
                LastModified = file.LastWriteTime
            });
        }
    }

    void ApplyThemeObject(ThemeObject themeObj, string resourceKey)
    {
        if(themeObj == null) return;

        if(themeObj.isGradient)
        {
            AppResourceUtil<LinearGradientBrush>.TrySetResource(_application, resourceKey, themeObj.gradient);
        }
        else
        {
            AppResourceUtil<SolidColorBrush>.TrySetResource(
                _application,
                resourceKey,
                new SolidColorBrush(themeObj.color.GetValueOrDefault())
            );
        }
    }
}
public class ThemeChangedEventArgs : EventArgs
{
    public string ThemeName { get; }
    public ColorTheme Theme { get; }

    public ThemeChangedEventArgs(string themeName, ColorTheme theme)
    {
        ThemeName = themeName;
        Theme = theme;
    }
}
