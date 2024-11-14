namespace NotepadEx;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using NotepadEx.Services;
using NotepadEx.Util;


public partial class MainWindow : Window
{
    private readonly MainWindowViewModel viewModel;

    public MainWindow()
    {
        InitializeComponent();

        var windowService = new WindowService(this);
        var documentService = new DocumentService();
        var themeService = new ThemeService(Application.Current);

        DataContext = viewModel = new MainWindowViewModel(
            windowService,
            documentService,
            themeService,
            MenuItemFileDropDown,
            () => SettingsManager.SaveSettings(this, txtEditor, themeService.CurrentThemeName)
        );

        InitTitleBar();
        InitializeEventHandlers();
    }

    private void InitTitleBar()
    {
        var titleBarViewModel = new CustomTitleBarViewModel(this);
        CustomTitleBar.InitializeTitleBar(ref titleBarViewModel, this, "NotepadEx");
        viewModel.TitleBarViewModel = titleBarViewModel;
    }

    private void InitializeEventHandlers()
    {
        // Window events
        StateChanged += (s, e) =>
        {
            if(WindowState != WindowState.Minimized)
                viewModel.UpdateWindowState(WindowState);
        };

        Closed += (s, e) => viewModel.PromptToSaveChanges();

        // TextBox events
        txtEditor.SelectionChanged += (s, e) =>
        {
            if(s is TextBox textBox)
                viewModel.UpdateSelection(textBox.SelectionStart, textBox.SelectionLength);
        };
    }

    private void OnBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(e.ClickCount == 2)
            viewModel.ToggleMaximizeState(this);
    }

    private void OnWindowMouseMove(object sender, MouseEventArgs e)
    {
        var position = e.GetPosition(this);
        viewModel.HandleMouseMovement(position.Y);

        if(WindowState == WindowState.Normal)
            viewModel.HandleWindowResize(this, position);
    }

    private void MenuItem_OpenRecent_Click(object sender, RoutedEventArgs e)
    {
        if(e.OriginalSource is MenuItem menuItem && menuItem.Header is string path && path != "...")
            viewModel.OpenRecentFile(path);
    }

    private void PART_ScrollbarRect_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if(sender is System.Windows.Shapes.Rectangle rectangle && e.LeftButton == MouseButtonState.Pressed)
            viewModel.HandleScrollBarDrag(rectangle, txtEditor, e);
    }
}