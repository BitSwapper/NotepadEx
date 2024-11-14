using System.Collections.ObjectModel;
using NotepadEx.MVVM.Models;
using NotepadEx.Theme;

namespace NotepadEx.Services.Interfaces;

public interface IThemeService
{
    event EventHandler<ThemeChangedEventArgs> ThemeChanged;
    ColorTheme CurrentTheme { get; }
    string CurrentThemeName { get; }
    ObservableCollection<ThemeInfo> AvailableThemes { get; }
    void LoadCurrentTheme();
    void ApplyTheme(string themeName);
    void OpenThemeEditor();
}
