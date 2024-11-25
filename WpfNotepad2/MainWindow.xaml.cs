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
        Settings.Default.TextWrapping = false;
        DataContext = viewModel = new MainWindowViewModel(windowService, documentService, themeService, MenuItemFileDropDown, txtEditor, () => SettingsManager.SaveSettings(this, txtEditor, themeService.CurrentThemeName));
        viewModel.TitleBarViewModel = CustomTitleBar.InitializeTitleBar(this, "NotepadEx", onClose : Application.Current.Shutdown);
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
        //if(_isSelecting) return;
        if(sender is System.Windows.Shapes.Rectangle rectangle && e.LeftButton == MouseButtonState.Pressed)
            viewModel.HandleScrollBarDrag(rectangle, txtEditor, e);
    }



    private void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if(sender is TextBox textBox)
        {
            // Check for navigation key combinations
            bool isNavigationKey = e.Key == Key.Left || e.Key == Key.Right ||
                                 e.Key == Key.Up || e.Key == Key.Down ||
                                 e.Key == Key.Home || e.Key == Key.End ||
                                 e.Key == Key.PageUp || e.Key == Key.PageDown;

            if(isNavigationKey)
            {
                // For Ctrl+Down/Up/End/Home, use a slightly delayed scroll to ensure caret is updated
                if((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                    (e.Key == Key.Down || e.Key == Key.Up || e.Key == Key.Home || e.Key == Key.End))
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                    {
                        ScrollToCaretPosition(textBox, true);
                    }));
                }
                else
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                    {
                        ScrollToCaretPosition(textBox);
                    }));
                }
            }
        }
    }

    private void ScrollToCaretPosition(TextBox textBox, bool ensureVisible = false)
    {
        if(_scrollViewer == null || isScrollbarDragging) return;

        try
        {
            // Get the rectangle for the current caret position
            Rect caretRect = textBox.GetRectFromCharacterIndex(textBox.CaretIndex);

            // Transform the rect to scroll viewer coordinates
            Point caretPosition = textBox.TranslatePoint(new Point(caretRect.Right, caretRect.Bottom), _scrollViewer);

            // Calculate the visible viewport
            double viewportHeight = _scrollViewer.ViewportHeight;
            double viewportWidth = _scrollViewer.ViewportWidth;
            double padding = 20; // Padding to keep caret away from edges

            // Handle horizontal scrolling
            if(caretPosition.X > viewportWidth)
            {
                double newOffset = _scrollViewer.HorizontalOffset + (caretPosition.X - viewportWidth) + padding;
                _scrollViewer.ScrollToHorizontalOffset(Math.Min(newOffset, _scrollViewer.ScrollableWidth));
            }
            else if(caretPosition.X < padding)
            {
                double newOffset = _scrollViewer.HorizontalOffset + caretPosition.X - padding;
                _scrollViewer.ScrollToHorizontalOffset(Math.Max(0, newOffset));
            }
            // Special case: if we're at the start of a line, scroll all the way left
            else if(IsAtStartOfLine(textBox))
            {
                _scrollViewer.ScrollToHorizontalOffset(0);
            }

            // Handle vertical scrolling
            if(ensureVisible || caretPosition.Y > viewportHeight)
            {
                // For Ctrl+Down/Up or when caret is below viewport, ensure the caret is visible
                double targetOffset;

                if(caretPosition.Y > viewportHeight)
                {
                    // Scroll to show caret at 3/4 of the viewport height for better context
                    targetOffset = _scrollViewer.VerticalOffset + (caretPosition.Y - (viewportHeight * 0.75));
                }
                else
                {
                    // Direct scroll to caret position
                    targetOffset = _scrollViewer.VerticalOffset + caretPosition.Y;
                }

                _scrollViewer.ScrollToVerticalOffset(Math.Min(targetOffset, _scrollViewer.ScrollableHeight));
            }
            else if(caretPosition.Y < padding)
            {
                double newOffset = _scrollViewer.VerticalOffset + caretPosition.Y - padding;
                _scrollViewer.ScrollToVerticalOffset(Math.Max(0, newOffset));
            }

            // Special case for Ctrl+End: ensure we're at the bottom if we're at the end of the document
            if(textBox.CaretIndex == textBox.Text.Length &&
                (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                _scrollViewer.ScrollToVerticalOffset(_scrollViewer.ScrollableHeight);
            }

            // Update scrollbar values
            if(_verticalScrollBar != null)
            {
                _verticalScrollBar.Value = _scrollViewer.VerticalOffset;
            }
            if(_horizontalScrollBar != null)
            {
                _horizontalScrollBar.Value = _scrollViewer.HorizontalOffset;
            }
        }
        catch
        {
            // Handle any potential exceptions from invalid caret positions
        }
    }

    private bool IsAtStartOfLine(TextBox textBox)
    {
        if(textBox.CaretIndex == 0) return true;

        string text = textBox.Text;
        int index = textBox.CaretIndex;

        // Check if the previous character is a newline
        return index > 0 && text[index - 1] == '\n';
    }


    void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e) => viewModel.HandleMouseScroll(sender, e);



    ScrollViewer _scrollViewer;
    ScrollBar _verticalScrollBar;
    ScrollBar _horizontalScrollBar;
    bool isMouseDown = false;
    bool isScrollbarDragging = false; // Tracks if the scrollbar itself is being dragged
    bool _isSelecting => txtEditor.SelectionLength > 0 && isMouseDown;
    DispatcherTimer _scrollTimer;
    double _scrollSpeed = 75;

    void TextBox_Loaded(object sender, RoutedEventArgs e)
    {
        if(sender is TextBox textBox)
        {
            _scrollViewer = VisualTreeUtil.FindVisualChildren<ScrollViewer>(textBox).First();

            var grid = _scrollViewer.Template.FindName("PART_Root", _scrollViewer) as Grid;
            if(grid != null)
            {
                _verticalScrollBar = grid.FindName("PART_VerticalScrollBar") as ScrollBar;
                _horizontalScrollBar = grid.FindName("PART_HorizontalScrollBar") as ScrollBar;

                if(_verticalScrollBar != null)
                {
                    _verticalScrollBar.PreviewMouseDown += (s, args) => isScrollbarDragging = true;
                    _verticalScrollBar.PreviewMouseUp += (s, args) => isScrollbarDragging = false;
                    _verticalScrollBar.ValueChanged += VerticalScrollBar_ValueChanged;
                }

                if(_horizontalScrollBar != null)
                {
                    _horizontalScrollBar.PreviewMouseDown += (s, args) => isScrollbarDragging = true;
                    _horizontalScrollBar.PreviewMouseUp += (s, args) => isScrollbarDragging = false;
                }
            }

            // Add TextChanged event handler
            textBox.TextChanged += TextBox_TextChanged;

            // Add CaretIndex changed event handler
            textBox.SelectionChanged += TextBox_SelectionChanged;

            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
        }

        _scrollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(16)
        };
        _scrollTimer.Tick += ScrollTimer_Tick;
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if(sender is TextBox textBox)
        {
            ScrollToCaretPosition(textBox);
        }
    }

    private void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        if(sender is TextBox textBox)
        {
            ScrollToCaretPosition(textBox);

            // Update selection info in ViewModel
            viewModel.UpdateSelection(textBox.SelectionStart, textBox.SelectionLength);
        }
    }

    void TextBox_MouseMove(object sender, MouseEventArgs e)
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

    void TextBox_MouseUp(object sender, MouseButtonEventArgs e)
    {
        isMouseDown = false;
        StopScrolling();
    }

    void StartScrolling(double speed)
    {
        if(!_scrollTimer.IsEnabled)
        {
            _scrollTimer.Tag = speed;
            _scrollTimer.Start();
        }
    }

    void StopScrolling()
    {
        _scrollTimer.Stop();
    }

    void ScrollTimer_Tick(object sender, EventArgs e)
    {
        if(!_isSelecting || isScrollbarDragging) return;

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

    void VerticalScrollBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if(isScrollbarDragging)
        {
            _scrollViewer.ScrollToVerticalOffset(e.NewValue);
        }
    }

    void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
        isMouseDown = true;
        StartScrolling(_scrollSpeed);
    }

    private void txtEditor_Loaded(object sender, RoutedEventArgs e)
    {

    }
}