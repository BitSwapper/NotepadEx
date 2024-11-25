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
    private ScrollViewer _scrollViewer;
    private ScrollBar _verticalScrollBar;
    private ScrollBar _horizontalScrollBar;
    private bool isScrollbarDragging;
    private readonly TextBox _textBox;
    private const double PADDING = 20;
    private bool _isInitialized;

    public ScrollManager(TextBox textBox)
    {
        _textBox = textBox;
        _textBox.Loaded += TextBox_Loaded;
    }

    private void TextBox_Loaded(object sender, RoutedEventArgs e)
    {
        if(!_isInitialized)
        {
            InitializeScrollViewer();
            _isInitialized = true;
        }
    }

    private void InitializeScrollViewer()
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

    private bool IsAtStartOfLine()
    {
        if(_textBox.CaretIndex == 0) return true;
        string text = _textBox.Text;
        int index = _textBox.CaretIndex;
        return index > 0 && text[index - 1] == '\n';
    }

    private void ScrollToCaretPosition(bool ensureVisible = false)
    {
        if(_scrollViewer == null || isScrollbarDragging) return;

        try
        {
            Rect caretRect = _textBox.GetRectFromCharacterIndex(_textBox.CaretIndex);
            Point caretPosition = _textBox.TranslatePoint(new Point(caretRect.Right, caretRect.Bottom), _scrollViewer);

            double viewportHeight = _scrollViewer.ViewportHeight;
            double viewportWidth = _scrollViewer.ViewportWidth;

            // Handle horizontal scrolling
            if(caretPosition.X > viewportWidth)
            {
                double newOffset = _scrollViewer.HorizontalOffset + (caretPosition.X - viewportWidth) + PADDING;
                _scrollViewer.ScrollToHorizontalOffset(Math.Min(newOffset, _scrollViewer.ScrollableWidth));
            }
            else if(caretPosition.X < PADDING)
            {
                double newOffset = _scrollViewer.HorizontalOffset + caretPosition.X - PADDING;
                _scrollViewer.ScrollToHorizontalOffset(Math.Max(0, newOffset));
            }
            else if(IsAtStartOfLine())
            {
                _scrollViewer.ScrollToHorizontalOffset(0);
            }

            // Handle vertical scrolling
            if(ensureVisible || caretPosition.Y > viewportHeight)
            {
                double targetOffset;
                if(caretPosition.Y > viewportHeight)
                {
                    targetOffset = _scrollViewer.VerticalOffset + (caretPosition.Y - (viewportHeight * 0.75));
                }
                else
                {
                    targetOffset = _scrollViewer.VerticalOffset + caretPosition.Y;
                }
                _scrollViewer.ScrollToVerticalOffset(Math.Min(targetOffset, _scrollViewer.ScrollableHeight));
            }
            else if(caretPosition.Y < PADDING)
            {
                double newOffset = _scrollViewer.VerticalOffset + caretPosition.Y - PADDING;
                _scrollViewer.ScrollToVerticalOffset(Math.Max(0, newOffset));
            }

            // Special case for Ctrl+End
            if(_textBox.CaretIndex == _textBox.Text.Length &&
                (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                _scrollViewer.ScrollToVerticalOffset(_scrollViewer.ScrollableHeight);
            }

            UpdateScrollBars();
        }
        catch
        {
            // Handle invalid caret positions
        }
    }

    private void UpdateScrollBars()
    {
        if(_verticalScrollBar != null)
        {
            _verticalScrollBar.Value = _scrollViewer.VerticalOffset;
        }
        if(_horizontalScrollBar != null)
        {
            _horizontalScrollBar.Value = _scrollViewer.HorizontalOffset;
        }
    }

    public void HandleMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var grid = (Grid)sender;
        var scrollViewer = grid.TemplatedParent as ScrollViewer;
        if(scrollViewer != null)
        {
            var verticalScrollBar = grid.FindName("PART_VerticalScrollBar") as ScrollBar;
            var newOffset = scrollViewer.VerticalOffset - (e.Delta / 3.0);
            newOffset = Math.Max(0, Math.Min(newOffset, scrollViewer.ScrollableHeight));
            scrollViewer.ScrollToVerticalOffset(newOffset);
            if(verticalScrollBar != null)
                verticalScrollBar.Value = newOffset;
            e.Handled = true;
        }
    }
}
