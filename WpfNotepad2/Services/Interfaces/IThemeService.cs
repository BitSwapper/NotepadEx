using System.Collections.ObjectModel;
using NotepadEx.MVVM.Models;
using NotepadEx.Theme;

namespace NotepadEx.Services.Interfaces;

public interface IThemeService
{
    ColorTheme CurrentTheme { get; }
    string CurrentThemeName { get; }
    ObservableCollection<ThemeInfo> AvailableThemes { get; }
    void LoadCurrentTheme();
    void ApplyTheme(string themeName);
    void OpenThemeEditor();
    void LoadAvailableThemes();
}
