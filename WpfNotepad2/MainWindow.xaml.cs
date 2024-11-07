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
        resizer.Initialize(this);
    }

   void MenuItem_OpenRecent_Click(object sender, RoutedEventArgs e) //**Refactor / Fix
    {
        MenuItem menuItem = (MenuItem)sender;
        MenuItem subMenuItem = (MenuItem)e.OriginalSource;
        bool hasTextChangedSinceSave = false; //**Refactor / Fix

        var path = (string)subMenuItem.Header;
        if(path != "...")
            viewModel.LoadDocument(path);
    }

    void SaveSettings() => SettingsManager.SaveSettings(this, txtEditor, themeService.CurrentThemeName);

}