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
        if(!_isSelecting || isScrollbarDragging || _scrollViewer == null) return;

        double speed = (double)_scrollTimer.Tag;

        double newOffset = _scrollViewer.VerticalOffset + (speed / 2400.0);
        newOffset = Math.Max(0, Math.Min(newOffset, _scrollViewer.ScrollableHeight));

        _scrollViewer.ScrollToVerticalOffset(newOffset);

        if(_verticalScrollBar != null)
            _verticalScrollBar.Value = newOffset;
    }

    void TextBox_MouseMove(object sender, MouseEventArgs e)
    {
        if(_isSelecting && _scrollViewer != null)
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

    void ScrollToCaretPosition(bool ensureVisible = false)
    {
        if(_scrollViewer == null || isScrollbarDragging || isAutoScrolling || _isSelecting) return;
        
        try
        {
            var rect = _textBox.GetRectFromCharacterIndex(_textBox.CaretIndex);
            
            // If caret is outside the visible area (either above or below), scroll to it
            if(rect.Top < _scrollViewer.VerticalOffset || 
               rect.Bottom > _scrollViewer.VerticalOffset + _scrollViewer.ViewportHeight)
            {
                double offset;
                
                if(rect.Top < _scrollViewer.VerticalOffset)
                    offset = rect.Top - PADDING; // Scroll up to show caret with padding
                else
                    offset = rect.Bottom - _scrollViewer.ViewportHeight + PADDING; // Scroll down
                    
                offset = Math.Max(0, Math.Min(offset, _scrollViewer.ScrollableHeight));
                _scrollViewer.ScrollToVerticalOffset(offset);
                
                if(_verticalScrollBar != null)
                    _verticalScrollBar.Value = offset;
            }
            
            // Also check horizontal scrolling
            if(rect.Left < _scrollViewer.HorizontalOffset || 
               rect.Right > _scrollViewer.HorizontalOffset + _scrollViewer.ViewportWidth)
            {
                double offset;
                
                if(rect.Left < _scrollViewer.HorizontalOffset)
                    offset = rect.Left - PADDING;
                else
                    offset = rect.Right - _scrollViewer.ViewportWidth + PADDING;
                    
                offset = Math.Max(0, Math.Min(offset, _scrollViewer.ScrollableWidth));
                _scrollViewer.ScrollToHorizontalOffset(offset);
                
                if(_horizontalScrollBar != null)
                    _horizontalScrollBar.Value = offset;
            }
        }
        catch(Exception)
        {
            // Caret index might be invalid, just ignore
        }
    }

    void TextBox_MouseDown(object sender, MouseButtonEventArgs e)
    {
        isMouseDown = true;
        isAutoScrolling = false;
    }

    void TextBox_MouseUp(object sender, MouseButtonEventArgs e)
    {
        isMouseDown = false;
        isAutoScrolling = false;
        StopScrolling();
    }

    void StartScrolling(double speed)
    {
        _scrollTimer.Tag = speed;
        if(!_scrollTimer.IsEnabled)
            _scrollTimer.Start();
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
        _scrollViewer = VisualTreeUtil.FindVisualChildren<ScrollViewer>(_textBox).FirstOrDefault();
        if(_scrollViewer == null) return;
        
        var grid = _scrollViewer.Template.FindName("PART_Root", _scrollViewer) as Grid;

        if(grid != null)
        {
            _verticalScrollBar = grid.FindName("PART_VerticalScrollBar") as ScrollBar;
            _horizontalScrollBar = grid.FindName("PART_HorizontalScrollBar") as ScrollBar;

            if(_verticalScrollBar != null)
            {
                _verticalScrollBar.PreviewMouseDown += (s, args) => isScrollbarDragging = true;
                _verticalScrollBar.PreviewMouseUp += (s, args) => isScrollbarDragging = false;
                _verticalScrollBar.ValueChanged += (s, args) => 
                {
                    if(isScrollbarDragging && _scrollViewer != null)
                        _scrollViewer.ScrollToVerticalOffset(args.NewValue);
                };
            }

            if(_horizontalScrollBar != null)
            {
                _horizontalScrollBar.PreviewMouseDown += (s, args) => isScrollbarDragging = true;
                _horizontalScrollBar.PreviewMouseUp += (s, args) => isScrollbarDragging = false;
                _horizontalScrollBar.ValueChanged += (s, args) => 
                {
                    if(isScrollbarDragging && _scrollViewer != null)
                        _scrollViewer.ScrollToHorizontalOffset(args.NewValue);
                };
            }
        }
    }

    public void HandleNavigationKey(Key key, ModifierKeys modifiers) => 
        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() =>
        {
            ScrollToCaretPosition((modifiers & ModifierKeys.Control) == ModifierKeys.Control);
        }));

    public void HandleTextChanged() => ScrollToCaretPosition(false);

    public void HandleSelectionChanged() => ScrollToCaretPosition(false);

    void UpdateScrollBars()
    {
        if(_verticalScrollBar != null)
            _verticalScrollBar.Value = _scrollViewer.VerticalOffset;
            
        if(_horizontalScrollBar != null)
            _horizontalScrollBar.Value = _scrollViewer.HorizontalOffset;
    }

    public void HandleMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if(_scrollViewer == null) return;
        
        if(sender is TextBox || sender is ScrollViewer)
        {
            // With these delta values, the scroll speed should be smoother
            double delta = e.Delta / 120.0;
            double newOffset = _scrollViewer.VerticalOffset - delta;
            
            // Ensure offset stays within bounds
            newOffset = Math.Max(0, Math.Min(newOffset, _scrollViewer.ScrollableHeight));
            
            // Apply scroll both to ScrollViewer and ScrollBar
            _scrollViewer.ScrollToVerticalOffset(newOffset);
            
            if(_verticalScrollBar != null)
                _verticalScrollBar.Value = newOffset;
                
            e.Handled = true;
        }
    }
}