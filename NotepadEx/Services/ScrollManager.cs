using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using NotepadEx.Util;
using Point = System.Windows.Point;
namespace NotepadEx.MVVM.ViewModels;

public class ScrollManager
{
    ScrollViewer _scrollViewer;
    ScrollBar _verticalScrollBar;
    ScrollBar _horizontalScrollBar;
    bool isScrollbarDragging;
    readonly TextBox _textBox;
    const double PADDING = 20;
    const double SCROLL_ZONE_PERCENTAGE  = 0.20;
    bool _isInitialized;
    bool isMouseDown;
    DispatcherTimer _scrollTimer;
    double _scrollSpeed = 25;
    bool isAutoScrolling;

    //Probably check out this class + TextBoxSelectionBehavior if you want to fix scrolling highlighted text on mouse drag

    bool _isSelecting => _textBox.SelectionLength > 0 && isMouseDown;

    public ScrollManager(TextBox textBox)
    {
        _textBox = textBox;
        _textBox.Loaded += TextBox_Loaded;
        InitializeTimer();
        InitializeMouseEvents();
    }

    void ScrollTimer_Tick(object sender, EventArgs e)
    {
        if(!_isSelecting || isScrollbarDragging) return;

        double speed = (double)_scrollTimer.Tag;

        double newOffset = _scrollViewer.VerticalOffset + (speed / 2400.0);
        newOffset = Math.Max(0, Math.Min(newOffset, _scrollViewer.ScrollableHeight));

        _scrollViewer.ScrollToVerticalOffset(newOffset);

        if(_verticalScrollBar != null)
        {
            _verticalScrollBar.Value = newOffset;
        }
    }

    void TextBox_MouseMove(object sender, MouseEventArgs e)
    {
        if(_isSelecting)
        {
            Point mousePosition = e.GetPosition(_scrollViewer);
            double zoneHeight = _scrollViewer.ActualHeight * SCROLL_ZONE_PERCENTAGE;

            // Top scroll zone check
            if(mousePosition.Y >= 0 && mousePosition.Y <= zoneHeight)
            {
                double scrollFactor = 1 - (mousePosition.Y / zoneHeight);
                isAutoScrolling = true;
                StartScrolling(-_scrollSpeed * Math.Pow(scrollFactor, 3));
            }
            // Bottom scroll zone check
            else if(mousePosition.Y >= _scrollViewer.ActualHeight - zoneHeight &&
                    mousePosition.Y <= _scrollViewer.ActualHeight)
            {
                double scrollFactor = (mousePosition.Y - (_scrollViewer.ActualHeight - zoneHeight)) / zoneHeight;
                isAutoScrolling = true;
                StartScrolling(_scrollSpeed * Math.Pow(scrollFactor, 3));
            }
            else
            {
                isAutoScrolling = false;
                StopScrolling();
            }
        }
    }


    void ScrollToCaretPosition(bool ensureVisible = false) { }
    //{
    //    if(_scrollViewer == null || isScrollbarDragging || isAutoScrolling || _isSelecting) return;

    //}

    void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if(e.LeftButton != MouseButtonState.Pressed) return;
        isMouseDown = true;
        isAutoScrolling = false;
    }

    void TextBox_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if(e.LeftButton != MouseButtonState.Pressed) return;

        isMouseDown = false;
        isAutoScrolling = false;
        StopScrolling();
    }

    void StartScrolling(double speed)
    {
        _scrollTimer.Tag = speed;
        if(!_scrollTimer.IsEnabled)
        {
            _scrollTimer.Start();
        }
    }

    void StopScrolling() => _scrollTimer.Stop();


    void InitializeTimer()
    {
        _scrollTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(20)
        };
        _scrollTimer.Tick += ScrollTimer_Tick;
    }

    void InitializeMouseEvents()
    {
        _textBox.PreviewMouseDown += TextBox_MouseDown;
        _textBox.PreviewMouseUp += TextBox_MouseUp;
        _textBox.PreviewMouseMove += TextBox_MouseMove;
    }


    void TextBox_Loaded(object sender, RoutedEventArgs e)
    {
        if(!_isInitialized)
        {
            InitializeScrollViewer();
            _isInitialized = true;
        }
    }

    void InitializeScrollViewer()
    {
        _scrollViewer = VisualTreeUtil.FindVisualChildren<ScrollViewer>(_textBox).First();
        var grid = _scrollViewer.Template.FindName("PART_Root", _scrollViewer) as Grid;

        if(grid != null)
        {
            _verticalScrollBar = grid.FindName("PART_VerticalScrollBar") as ScrollBar;
            _horizontalScrollBar = grid.FindName("PART_HorizontalScrollBar") as ScrollBar;

            if(_verticalScrollBar != null)
            {
                _verticalScrollBar.PreviewMouseDown += (s, args) => isScrollbarDragging = true;
                _verticalScrollBar.PreviewMouseUp += (s, args) => isScrollbarDragging = false;
            }

            if(_horizontalScrollBar != null)
            {
                _horizontalScrollBar.PreviewMouseDown += (s, args) => isScrollbarDragging = true;
                _horizontalScrollBar.PreviewMouseUp += (s, args) => isScrollbarDragging = false;
            }
        }
    }

    public void HandleNavigationKey(Key key, ModifierKeys modifiers) => Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
                                                                             {
                                                                                 ScrollToCaretPosition((modifiers & ModifierKeys.Control) == ModifierKeys.Control);
                                                                             }));

    public void HandleTextChanged() => ScrollToCaretPosition(false);

    public void HandleSelectionChanged() => ScrollToCaretPosition(false);

    bool IsAtStartOfLine()
    {
        if(_textBox.CaretIndex == 0) return true;
        string text = _textBox.Text;
        int index = _textBox.CaretIndex;
        return index > 0 && text[index - 1] == '\n';
    }

    public void HandleMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if(sender is TextBox textBox)
        {
            var scrollViewer = FindScrollViewer(textBox);
            if(scrollViewer != null)
            {
                var grid = scrollViewer.Template.FindName("PART_Root", scrollViewer) as Grid;
                if(grid != null)
                {
                    var verticalScrollBar = grid.FindName("PART_VerticalScrollBar") as ScrollBar;
                    var newOffset = scrollViewer.VerticalOffset - (e.Delta / 3.0);
                    newOffset = Math.Max(0, Math.Min(newOffset, scrollViewer.ScrollableHeight));
                    scrollViewer.ScrollToVerticalOffset(newOffset);
                    if(verticalScrollBar != null)
                        verticalScrollBar.Value = newOffset;
                }
                e.Handled = true;
            }
        }
    }

    ScrollViewer FindScrollViewer(TextBox textBox)
    {
        if(textBox == null) return null;
        return VisualTreeUtil.FindVisualChildren<ScrollViewer>(textBox).FirstOrDefault();
    }
}
