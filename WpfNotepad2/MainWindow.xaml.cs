namespace NotepadEx;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using NotepadEx.Properties;
using NotepadEx.Services;
using NotepadEx.Util;

public partial class MainWindow : Window
{
    readonly MainWindowViewModel viewModel;

    public MainWindow()
    {
        InitializeComponent();

        var windowService = new WindowService(this);
        var documentService = new DocumentService();
        var themeService = new ThemeService(Application.Current);

        Settings.Default.MenuBarAutoHide = false;
        Settings.Default.TextWrapping = true;
        DataContext = viewModel = new MainWindowViewModel(windowService, documentService, themeService, MenuItemFileDropDown, txtEditor, () => SettingsManager.SaveSettings(this, txtEditor, themeService.CurrentThemeName));
        viewModel.TitleBarViewModel = CustomTitleBar.InitializeTitleBar(this, "NotepadEx");
        InitializeEventHandlers();
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

    void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e) => viewModel.HandleMouseScroll(sender, e);



    private ScrollViewer _scrollViewer;
    private ScrollBar _verticalScrollBar;
    private bool _isSelecting = false;
    private DispatcherTimer _scrollTimer;
    private double _scrollSpeed = 75; // Adjust the scrolling speed

    private void TextBox_Loaded(object sender, RoutedEventArgs e)
    {
        // Access the ScrollViewer inside the TextBox
        _scrollViewer = VisualTreeUtil.FindVisualChildren<ScrollViewer>(sender as TextBox).First();


        var grid = _scrollViewer.Template.FindName("PART_Root", _scrollViewer) as Grid;
        if(grid != null)
        {
            _verticalScrollBar = grid.FindName("PART_VerticalScrollBar") as ScrollBar;
            //if(_verticalScrollBar != null)
            //{
            //    _verticalScrollBar.Maximum = _scrollViewer.ScrollableHeight;
            //    _verticalScrollBar.Value = _scrollViewer.VerticalOffset;
            //}
        }


        _scrollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16) // ~60 FPS
        };
        _scrollTimer.Tick += ScrollTimer_Tick;
    }

    private void TextBox_MouseMove(object sender, MouseEventArgs e)
    {
        if(_isSelecting)
        {
            Point mousePosition = e.GetPosition(_scrollViewer);
            if(mousePosition.Y < 0)
            {
                // Scroll up
                StartScrolling(-_scrollSpeed);
            }
            else if(mousePosition.Y > _scrollViewer.ActualHeight)
            {
                // Scroll down
                StartScrolling(_scrollSpeed);
            }
            else
            {
                StopScrolling();
            }
        }
    }

    private void TextBox_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _isSelecting = false;
        StopScrolling();
    }

    private void StartScrolling(double speed)
    {
        if(!_scrollTimer.IsEnabled)
        {
            _scrollTimer.Tag = speed;
            _scrollTimer.Start();
        }
    }

    private void StopScrolling()
    {
        _scrollTimer.Stop();
    }

    private void ScrollTimer_Tick(object sender, EventArgs e)
    {
        double speed = (double)_scrollTimer.Tag;
        double newOffset = _scrollViewer.VerticalOffset + speed;

        // Clamp the new offset
        newOffset = Math.Max(0, Math.Min(newOffset, _scrollViewer.ScrollableHeight));

        // Update the ScrollViewer
        _scrollViewer.ScrollToVerticalOffset(newOffset);

        // Update the vertical scrollbar
        if(_verticalScrollBar != null)
            _verticalScrollBar.Value = newOffset;
    }

    private void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
        _isSelecting = true;
        StartScrolling(_scrollSpeed);
    }
}