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
    ScrollViewer scrollViewer;

    public void StartDrag(Rectangle rectangle, TextBox textBox, MouseButtonEventArgs e)
    {
        var scrollBar = FindParentScrollBar(rectangle);
        if(scrollBar == null) return;

        activeScrollBar = scrollBar;
        isDragging = true;
        lastMousePosition = e.GetPosition(scrollBar);
        this.textBox = textBox;
        this.scrollViewer = FindScrollViewer(textBox);

        if(scrollViewer != null)
            UpdateScrollPosition(lastMousePosition);

        rectangle.MouseMove += Rectangle_MouseMove;
        rectangle.MouseUp += Rectangle_MouseUp;
        rectangle.CaptureMouse();
    }

    void UpdateScrollPosition(Point mousePosition)
    {
        if(textBox == null || activeScrollBar == null || scrollViewer == null) return;

        double newValue;
        if(activeScrollBar.Orientation == Orientation.Vertical)
        {
            // Calculate the proportional position
            newValue = (mousePosition.Y / activeScrollBar.ActualHeight) *
                      (activeScrollBar.Maximum - activeScrollBar.Minimum) + activeScrollBar.Minimum;
            
            // Ensure bounds
            newValue = Math.Max(activeScrollBar.Minimum, Math.Min(newValue, activeScrollBar.Maximum));
            
            // Update scrollbar and scrollviewer
            activeScrollBar.Value = newValue;
            scrollViewer.ScrollToVerticalOffset(newValue);
        }
        else
        {
            newValue = (mousePosition.X / activeScrollBar.ActualWidth) *
                      (activeScrollBar.Maximum - activeScrollBar.Minimum) + activeScrollBar.Minimum;
            newValue = Math.Max(activeScrollBar.Minimum, Math.Min(newValue, activeScrollBar.Maximum));
            
            activeScrollBar.Value = newValue;
            scrollViewer.ScrollToHorizontalOffset(newValue);
        }
    }

    void Rectangle_MouseMove(object sender, MouseEventArgs e)
    {
        if(isDragging && activeScrollBar != null)
        {
            var currentPosition = e.GetPosition(activeScrollBar);

            if(activeScrollBar.Orientation == Orientation.Vertical)
            {
                // Calculate the amount to scroll
                var delta = (currentPosition.Y - lastMousePosition.Y) / activeScrollBar.ActualHeight *
                           (activeScrollBar.Maximum - activeScrollBar.Minimum);
                
                // Update position
                var newValue = activeScrollBar.Value + delta;
                newValue = Math.Max(activeScrollBar.Minimum, Math.Min(newValue, activeScrollBar.Maximum));
                
                // Apply changes
                activeScrollBar.Value = newValue;
                scrollViewer?.ScrollToVerticalOffset(newValue);
            }
            else
            {
                var delta = (currentPosition.X - lastMousePosition.X) / activeScrollBar.ActualWidth *
                           (activeScrollBar.Maximum - activeScrollBar.Minimum);
                var newValue = activeScrollBar.Value + delta;
                newValue = Math.Max(activeScrollBar.Minimum, Math.Min(newValue, activeScrollBar.Maximum));
                
                activeScrollBar.Value = newValue;
                scrollViewer?.ScrollToHorizontalOffset(newValue);
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
            scrollViewer = null;
        }
    }

    ScrollBar FindParentScrollBar(DependencyObject child)
    {
        var parent = VisualTreeHelper.GetParent(child);
        while(parent != null && !(parent is ScrollBar))
            parent = VisualTreeHelper.GetParent(parent);

        return parent as ScrollBar;
    }
    
    ScrollViewer FindScrollViewer(TextBox textBox)
    {
        if(textBox == null) return null;
        
        for(int i = 0; i < VisualTreeHelper.GetChildrenCount(textBox); i++)
        {
            var child = VisualTreeHelper.GetChild(textBox, i);
            
            if(child is ScrollViewer scrollViewer)
                return scrollViewer;
                
            var result = FindNestedScrollViewer(child);
            if(result != null)
                return result;
        }
        
        return null;
    }
    
    ScrollViewer FindNestedScrollViewer(DependencyObject parent)
    {
        if(parent == null) return null;
        
        for(int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);
            
            if(child is ScrollViewer scrollViewer)
                return scrollViewer;
                
            var result = FindNestedScrollViewer(child);
            if(result != null)
                return result;
        }
        
        return null;
    }
}