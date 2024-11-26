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
    ScrollBar activeScrollBar;
    bool isDragging;
    Point lastMousePosition;
    TextBox textBox;

    public void StartDrag(Rectangle rectangle, TextBox textBox, MouseButtonEventArgs e)
    {
        var scrollBar = FindParentScrollBar(rectangle);
        if(scrollBar == null) return;

        activeScrollBar = scrollBar;
        isDragging = true;
        lastMousePosition = e.GetPosition(scrollBar);
        this.textBox = textBox;

        UpdateScrollPosition();

        rectangle.MouseMove += Rectangle_MouseMove;
        rectangle.MouseUp += Rectangle_MouseUp;
        rectangle.CaptureMouse();
    }

    void UpdateScrollPosition()
    {
        if(textBox == null || activeScrollBar == null) return;

        double newValue;
        if(activeScrollBar.Orientation == Orientation.Vertical)
        {
            newValue = (lastMousePosition.Y / activeScrollBar.ActualHeight) *
                      (activeScrollBar.Maximum - activeScrollBar.Minimum) + activeScrollBar.Minimum;
            newValue = Math.Max(activeScrollBar.Minimum, Math.Min(newValue, activeScrollBar.Maximum));
            activeScrollBar.Value = newValue;
            textBox.ScrollToVerticalOffset(newValue);
        }
        else
        {
            newValue = (lastMousePosition.X / activeScrollBar.ActualWidth) *
                      (activeScrollBar.Maximum - activeScrollBar.Minimum) + activeScrollBar.Minimum;
            newValue = Math.Max(activeScrollBar.Minimum, Math.Min(newValue, activeScrollBar.Maximum));
            activeScrollBar.Value = newValue;
            textBox.ScrollToHorizontalOffset(newValue);
        }
    }

    void Rectangle_MouseMove(object sender, MouseEventArgs e)
    {
        if(isDragging && activeScrollBar != null)
        {
            var currentPosition = e.GetPosition(activeScrollBar);

            if(activeScrollBar.Orientation == Orientation.Vertical)
            {
                var delta = (currentPosition.Y - lastMousePosition.Y) / activeScrollBar.ActualHeight *
                           (activeScrollBar.Maximum - activeScrollBar.Minimum);
                var newValue = activeScrollBar.Value + delta;
                activeScrollBar.Value = Math.Max(activeScrollBar.Minimum, Math.Min(newValue, activeScrollBar.Maximum));
                textBox?.ScrollToVerticalOffset(activeScrollBar.Value);
            }
            else
            {
                var delta = (currentPosition.X - lastMousePosition.X) / activeScrollBar.ActualWidth *
                           (activeScrollBar.Maximum - activeScrollBar.Minimum);
                var newValue = activeScrollBar.Value + delta;
                activeScrollBar.Value = Math.Max(activeScrollBar.Minimum, Math.Min(newValue, activeScrollBar.Maximum));
                textBox?.ScrollToHorizontalOffset(activeScrollBar.Value);
            }

            lastMousePosition = currentPosition;
        }
    }

    void Rectangle_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if(isDragging && sender is Rectangle rectangle)
        {
            rectangle.MouseMove -= Rectangle_MouseMove;
            rectangle.MouseUp -= Rectangle_MouseUp;
            rectangle.ReleaseMouseCapture();

            isDragging = false;
            activeScrollBar = null;
            textBox = null;
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
