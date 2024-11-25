using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace NotepadEx.MVVM.Behaviors;

public class ScrollBarBehavior
{
    ScrollBar _activeScrollBar;
    bool _isDragging;
    Point _lastMousePosition;
    TextBox _textBox;

    public void StartDrag(Rectangle rectangle, TextBox textBox, MouseButtonEventArgs e)
    {
        //var scrollBar = FindParentScrollBar(rectangle);
        //if(scrollBar == null) return;

        //_activeScrollBar = scrollBar;
        //_isDragging = true;
        //_lastMousePosition = e.GetPosition(scrollBar);
        //_textBox = textBox;

        //UpdateScrollPosition();

        //rectangle.MouseMove += Rectangle_MouseMove;
        //rectangle.MouseUp += Rectangle_MouseUp;
        //rectangle.CaptureMouse();
    }

    void UpdateScrollPosition()
    {
        if(_textBox == null || _activeScrollBar == null) return;

        double newValue;
        if(_activeScrollBar.Orientation == Orientation.Vertical)
        {
            newValue = (_lastMousePosition.Y / _activeScrollBar.ActualHeight) *
                      (_activeScrollBar.Maximum - _activeScrollBar.Minimum) + _activeScrollBar.Minimum;
            newValue = Math.Max(_activeScrollBar.Minimum, Math.Min(newValue, _activeScrollBar.Maximum));
            _activeScrollBar.Value = newValue;
            _textBox.ScrollToVerticalOffset(newValue);
        }
        else
        {
            newValue = (_lastMousePosition.X / _activeScrollBar.ActualWidth) *
                      (_activeScrollBar.Maximum - _activeScrollBar.Minimum) + _activeScrollBar.Minimum;
            newValue = Math.Max(_activeScrollBar.Minimum, Math.Min(newValue, _activeScrollBar.Maximum));
            _activeScrollBar.Value = newValue;
            _textBox.ScrollToHorizontalOffset(newValue);
        }
    }

    void Rectangle_MouseMove(object sender, MouseEventArgs e)
    {
        if(_isDragging && _activeScrollBar != null)
        {
            var currentPosition = e.GetPosition(_activeScrollBar);

            if(_activeScrollBar.Orientation == Orientation.Vertical)
            {
                var delta = (currentPosition.Y - _lastMousePosition.Y) / _activeScrollBar.ActualHeight *
                           (_activeScrollBar.Maximum - _activeScrollBar.Minimum);
                var newValue = _activeScrollBar.Value + delta;
                _activeScrollBar.Value = Math.Max(_activeScrollBar.Minimum, Math.Min(newValue, _activeScrollBar.Maximum));
                _textBox?.ScrollToVerticalOffset(_activeScrollBar.Value);
            }
            else
            {
                var delta = (currentPosition.X - _lastMousePosition.X) / _activeScrollBar.ActualWidth *
                           (_activeScrollBar.Maximum - _activeScrollBar.Minimum);
                var newValue = _activeScrollBar.Value + delta;
                _activeScrollBar.Value = Math.Max(_activeScrollBar.Minimum, Math.Min(newValue, _activeScrollBar.Maximum));
                _textBox?.ScrollToHorizontalOffset(_activeScrollBar.Value);
            }

            _lastMousePosition = currentPosition;
        }
    }

    void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if(_isDragging && sender is Rectangle rectangle)
        {
            rectangle.MouseMove -= Rectangle_MouseMove;
            rectangle.MouseUp -= Rectangle_MouseUp;
            rectangle.ReleaseMouseCapture();

            _isDragging = false;
            _activeScrollBar = null;
            _textBox = null;
        }
    }

    ScrollBar FindParentScrollBar(DependencyObject child)
    {
        var parent = VisualTreeHelper.GetParent(child);
        while(parent != null && !(parent is ScrollBar))
            parent = VisualTreeHelper.GetParent(parent);
        return parent as ScrollBar;
    }
}
