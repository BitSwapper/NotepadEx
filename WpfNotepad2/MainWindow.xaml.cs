namespace NotepadEx;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using NotepadEx.Services;

public partial class MainWindow : Window
{
    readonly WindowResizer resizer;
    readonly MainWindowViewModel viewModel;
    readonly ThemeService themeService;

    public MainWindow()
    {
        var windowService = new WindowService(this);
        var documentService = new DocumentService();
        themeService = new ThemeService(Application.Current);
        resizer = new WindowResizer();

        InitializeComponent();
        DataContext = viewModel = new MainWindowViewModel(windowService, documentService, themeService, MenuItemFileDropDown, SaveSettings);
        InitTitleBar();


        StateChanged += OnWindowStateChanged;
        MouseMove += OnWindowMouseMove;
        Closed += WindowClosed;

        void InitTitleBar()
        {
            var titleBarViewModel = new CustomTitleBarViewModel(this);
            CustomTitleBar.InitializeTitleBar(ref titleBarViewModel, this, "NotepadEx");
            viewModel.TitleBarViewModel = titleBarViewModel;
        }
    }

    void OnWindowStateChanged(object sender, EventArgs e)
    {
        if(WindowState != WindowState.Minimized)
            viewModel.UpdateWindowState(WindowState);
    }

    void OnBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(e.ClickCount == 2)
            resizer.ToggleMaximizeState(this);
        else if(e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    void OnWindowMouseMove(object sender, MouseEventArgs e)
    {
        if(viewModel.IsAutoHideMenuBarEnabled)
        {
            var position = e.GetPosition(this);
            viewModel.HandleMouseMovement(position.Y);
        }

        if(WindowState == WindowState.Normal)
        {
            var position = e.GetPosition(this);
            resizer.HandleMouseMove(this, position);
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
    }

   void MenuItem_OpenRecent_Click(object sender, RoutedEventArgs e) //**Refactor / Fix
    {
        if(!viewModel.PromptToSaveChanges()) return;

        MenuItem menuItem = (MenuItem)sender;
        MenuItem subMenuItem = (MenuItem)e.OriginalSource;

        var path = (string)subMenuItem.Header;
        if(path != "...")
            viewModel.LoadDocument(path);
    }

    void SaveSettings() => SettingsManager.SaveSettings(this, txtEditor, themeService.CurrentThemeName);

    void WindowClosed(object sender, EventArgs e)
    {
        viewModel.PromptToSaveChanges();
    }

    private void TxtEditor_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if(DataContext is MainWindowViewModel viewModel && sender is TextBox textBox)
        {
            viewModel.SelectionStart = textBox.SelectionStart;
            viewModel.SelectionLength = textBox.SelectionLength;
        }
    }
}