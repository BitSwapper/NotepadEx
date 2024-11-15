namespace NotepadEx;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using NotepadEx.Services;


public partial class MainWindow : Window
{
    readonly MainWindowViewModel viewModel;

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
            txtEditor,
            () => SettingsManager.SaveSettings(this, txtEditor, themeService.CurrentThemeName));

        InitTitleBar();
        InitializeEventHandlers();
    }

    void InitTitleBar()
    {
        var titleBarViewModel = new CustomTitleBarViewModel(this);
        CustomTitleBar.InitializeTitleBar(ref titleBarViewModel, this, "NotepadEx");
        viewModel.TitleBarViewModel = titleBarViewModel;
    }

    void InitializeEventHandlers()
    {
        StateChanged += (s, e) =>
        {
            if(WindowState != WindowState.Minimized)
                viewModel.UpdateWindowState(WindowState);
        };

        Closed += (s, e) => viewModel.PromptToSaveChanges();

        txtEditor.SelectionChanged += (s, e) =>
        {
            if(s is TextBox textBox)
                viewModel.UpdateSelection(textBox.SelectionStart, textBox.SelectionLength);
        };
    }

    void OnWindowMouseMove(object sender, MouseEventArgs e)
    {
        var position = e.GetPosition(this);
        viewModel.HandleMouseMovement(position.Y);

        if(WindowState == WindowState.Normal)
            viewModel.HandleWindowResize(this, position);
    }

    void MenuItem_OpenRecent_Click(object sender, RoutedEventArgs e)
    {
        if(e.OriginalSource is MenuItem menuItem && menuItem.Header is string path && path != "...")
            viewModel.OpenRecentFile(path);
    }

    void PART_ScrollbarRect_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if(sender is System.Windows.Shapes.Rectangle rectangle && e.LeftButton == MouseButtonState.Pressed)
            viewModel.HandleScrollBarDrag(rectangle, txtEditor, e);
    }

    void ScrollToCaretPosition()
    {
        //To Do
    }
}