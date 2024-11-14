namespace NotepadEx;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using NotepadEx.Services;
using NotepadEx.Util;

public partial class MainWindow : Window
{
    readonly MainWindowViewModel viewModel;
    readonly ThemeService themeService;

    public MainWindow()
    {
        var windowService = new WindowService(this);
        var documentService = new DocumentService();
        themeService = new ThemeService(Application.Current);

        InitializeComponent();
        DataContext = viewModel = new MainWindowViewModel(windowService, documentService, themeService, MenuItemFileDropDown, SaveSettings);
        InitTitleBar();

        StateChanged += OnWindowStateChanged;
        MouseMove += OnWindowMouseMove;
        Closed += WindowClosed;
    }

    void InitTitleBar()
    {
        var titleBarViewModel = new CustomTitleBarViewModel(this);
        CustomTitleBar.InitializeTitleBar(ref titleBarViewModel, this, "NotepadEx");
        viewModel.TitleBarViewModel = titleBarViewModel;
    }

    void OnWindowStateChanged(object sender, EventArgs e)
    {
        if(WindowState != WindowState.Minimized)
            viewModel.UpdateWindowState(WindowState);
    }

    void OnBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(e.ClickCount == 2)
            WindowResizerUtil.ToggleMaximizeState(this);
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
            WindowResizerUtil.ResizeWindow(this, position);
        }
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

    void PART_Background_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if(e.LeftButton == MouseButtonState.Pressed)
        {
            var scrollBar = sender as FrameworkElement;
            if(scrollBar != null)
            {

                var position = e.GetPosition(scrollBar);
                var scrollBars = VisualTreeUtil.FindVisualChildren<ScrollBar>(txtEditor);
                foreach(var sb in scrollBars)
                {
                    var scrollBarPosition = sb.PointToScreen(new Point(0, 0));
                    var mousePosition = Mouse.GetPosition(txtEditor);
                    mousePosition.X += Width;
                    mousePosition.Y += Height;

                    if(mousePosition.X >= scrollBarPosition.X && mousePosition.X <= scrollBarPosition.X + sb.ActualWidth && mousePosition.Y >= scrollBarPosition.Y && mousePosition.Y <= scrollBarPosition.Y + sb.ActualHeight)
                    {
                        double newValue;
                        if(sb.Orientation == Orientation.Vertical)
                        {
                            newValue = mousePosition.Y / sb.ActualHeight * (sb.Maximum - sb.Minimum) + sb.Minimum;
                            txtEditor.ScrollToVerticalOffset(newValue);
                        }
                        else
                        {
                            newValue = mousePosition.X / sb.ActualWidth * (sb.Maximum - sb.Minimum) + sb.Minimum;
                            txtEditor.ScrollToHorizontalOffset(newValue);
                        }
                    }
                }
            }
        }
    }
}