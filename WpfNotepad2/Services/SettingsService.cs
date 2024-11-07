using NotepadEx.MVVM.Models;
using NotepadEx.Services.Interfaces;

namespace NotepadEx.Services
{
    public class SettingsService : ISettingsService
    {
        public AppSettings LoadSettings()
        {
            return new AppSettings
            {
                TextWrapping = Properties.Settings.Default.TextWrapping,
                MenuBarAutoHide = Properties.Settings.Default.MenuBarAutoHide,
                InfoBarVisible = Properties.Settings.Default.InfoBarVisible
            };
        }

        public void SaveSettings(AppSettings settings)
        {
            Properties.Settings.Default.TextWrapping = settings.TextWrapping;
            Properties.Settings.Default.MenuBarAutoHide = settings.MenuBarAutoHide;
            Properties.Settings.Default.InfoBarVisible = settings.InfoBarVisible;
            Properties.Settings.Default.Save();
        }
    }
}
