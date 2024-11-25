using System.Windows;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using NotepadEx.Properties;
using NotepadEx.Services;

namespace NotepadEx;

public partial class MainWindow : Window, IDisposable
{
    readonly MainWindowViewModel viewModel;

    public MainWindow()
    {
        InitializeComponent();

        var windowService = new WindowService(this);
        var documentService = new DocumentService();
        var themeService = new ThemeService(Application.Current);

        Settings.Default.MenuBarAutoHide = false;
        Settings.Default.TextWrapping = false;

        DataContext = viewModel = new MainWindowViewModel(windowService, documentService, themeService, MenuItemFileDropDown, txtEditor, () => SettingsManager.SaveSettings(this, txtEditor, themeService.CurrentThemeName));
        viewModel.TitleBarViewModel = CustomTitleBar.InitializeTitleBar(this, "NotepadEx", onClose: Application.Current.Shutdown);

        InitializeWindowEvents();
    }

    void InitializeWindowEvents()
    {
        StateChanged += (s, e) =>
        {
            if(WindowState != WindowState.Minimized)
                viewModel.UpdateWindowState(WindowState);
        };

        Closed += (s, e) => viewModel.PromptToSaveChanges();
    }

    public void Dispose() => viewModel?.Cleanup();
}