namespace NotepadEx;
using System.Windows;
using System.Windows.Input;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using NotepadEx.Services;

public partial class MainWindow : Window
{
    private readonly WindowResizer _resizer;
    private readonly MainWindowViewModel _viewModel;
    private readonly ThemeService _themeService;

    public MainWindow()
    {
        var windowService = new WindowService(this);
        var settingsService = new SettingsService();
        var documentService = new DocumentService();
        var _themeService = new ThemeService(Application.Current);

        DataContext = _viewModel = new MainWindowViewModel(windowService, settingsService, documentService, _themeService);

        var titleBarViewModel = new CustomTitleBarViewModel(this);
        CustomTitleBar.InitializeTitleBar(ref titleBarViewModel, this, "NotepadEx");
        _viewModel.TitleBarViewModel = titleBarViewModel;

        InitializeComponent();
        _resizer = new WindowResizer();

        StateChanged += OnWindowStateChanged;
        MouseMove += OnWindowMouseMove;
    }

    private void OnWindowStateChanged(object sender, EventArgs e)
    {
        if(WindowState != WindowState.Minimized)
        {
            _viewModel.UpdateWindowState(WindowState);
        }
    }

    private void OnBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(e.ClickCount == 2)
            _resizer.ToggleMaximizeState(this);
        else if(e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    private void OnWindowMouseMove(object sender, MouseEventArgs e)
    {
        if(_viewModel.IsAutoHideMenuBarEnabled)
        {
            var position = e.GetPosition(this);
            _viewModel.HandleMouseMovement(position.Y);
        }

        if(WindowState == WindowState.Normal)
        {
            var position = e.GetPosition(this);
            _resizer.HandleMouseMove(this, position);
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        _resizer.Initialize(this);
    }

    void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {

    }

    void Window_StateChanged(object sender, EventArgs e)
    {

    }
}