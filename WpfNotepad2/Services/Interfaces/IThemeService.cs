using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotepadEx.MVVM.Models;
using NotepadEx.Theme;

namespace NotepadEx.Services.Interfaces
{
    public interface IThemeService
    {
        event EventHandler<ThemeChangedEventArgs> ThemeChanged;
        ColorTheme CurrentTheme { get; }
        ObservableCollection<ThemeInfo> AvailableThemes { get; }
        void LoadCurrentTheme();
        void ApplyTheme(string themeName);
        void OpenThemeEditor();
    }
}
