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
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TrackBar;

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

    void WindowClosed(object sender, EventArgs e) => viewModel.PromptToSaveChanges();

    void TxtEditor_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if(DataContext is MainWindowViewModel viewModel && sender is TextBox textBox)
        {
            viewModel.SelectionStart = textBox.SelectionStart;
            viewModel.SelectionLength = textBox.SelectionLength;
        }
    }
    private ScrollBar _activeScrollBar;
private bool _isDragging;
private Point _lastMousePosition;

private void Rectangle_MouseMove(object sender, MouseEventArgs e)
{
    if(_isDragging && _activeScrollBar != null)
    {
        var currentPosition = e.GetPosition(_activeScrollBar);
        
        if(_activeScrollBar.Orientation == Orientation.Vertical)
        {
            var delta = (currentPosition.Y - _lastMousePosition.Y) / _activeScrollBar.ActualHeight * 
                       (_activeScrollBar.Maximum - _activeScrollBar.Minimum);
            var newValue = _activeScrollBar.Value + delta;
            newValue = Math.Max(_activeScrollBar.Minimum, Math.Min(newValue, _activeScrollBar.Maximum));
            _activeScrollBar.Value = newValue;
            txtEditor.ScrollToVerticalOffset(newValue);
        }
        else
        {
            // For horizontal, calculate the absolute position instead of using delta
            var newValue = (currentPosition.X / _activeScrollBar.ActualWidth) * 
                          (_activeScrollBar.Maximum - _activeScrollBar.Minimum) + _activeScrollBar.Minimum;
            newValue = Math.Max(_activeScrollBar.Minimum, Math.Min(newValue, _activeScrollBar.Maximum));
            _activeScrollBar.Value = newValue;
            txtEditor.ScrollToHorizontalOffset(newValue);
        }

        _lastMousePosition = currentPosition;
    }
}

private void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
{
    if(_isDragging)
    {
        var rectangle = sender as System.Windows.Shapes.Rectangle;
        if(rectangle != null)
        {
            rectangle.MouseMove -= Rectangle_MouseMove;
            rectangle.MouseUp -= Rectangle_MouseUp;
            rectangle.ReleaseMouseCapture();
        }

        _isDragging = false;
        _activeScrollBar = null;
    }
}

    // Helper method to find parent ScrollBar
    private ScrollBar FindParentScrollBar(DependencyObject child)
    {
        var parent = VisualTreeHelper.GetParent(child);

        while(parent != null && !(parent is ScrollBar))
        {
            parent = VisualTreeHelper.GetParent(parent);
        }

        return parent as ScrollBar;
    }

    private void PART_ScrollbarRect_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if(e.LeftButton == MouseButtonState.Pressed)
        {
            var rectangle = sender as System.Windows.Shapes.Rectangle;
            if(rectangle == null) return;

            var scrollBar = FindParentScrollBar(rectangle);
            if(scrollBar == null) return;

            _activeScrollBar = scrollBar;
            _isDragging = true;
            _lastMousePosition = e.GetPosition(scrollBar);

            var textBox = txtEditor;
            if(textBox == null) return;

            // Initial snap
            double newValue;
            if(scrollBar.Orientation == Orientation.Vertical)
            {
                newValue = (_lastMousePosition.Y / scrollBar.ActualHeight) *
                          (scrollBar.Maximum - scrollBar.Minimum) + scrollBar.Minimum;
                newValue = Math.Max(scrollBar.Minimum, Math.Min(newValue, scrollBar.Maximum));
                scrollBar.Value = newValue;
                textBox.ScrollToVerticalOffset(newValue);
            }
            else
            {
                newValue = (_lastMousePosition.X / scrollBar.ActualWidth) *
                          (scrollBar.Maximum - scrollBar.Minimum) + scrollBar.Minimum;
                newValue = Math.Max(scrollBar.Minimum, Math.Min(newValue, scrollBar.Maximum));
                scrollBar.Value = newValue;
                textBox.ScrollToHorizontalOffset(newValue);
            }

            rectangle.MouseMove += Rectangle_MouseMove;
            rectangle.MouseUp += Rectangle_MouseUp;
            rectangle.CaptureMouse();

            e.Handled = true;
        }
    }
}