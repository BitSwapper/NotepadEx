using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Xaml.Behaviors;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace NotepadEx.MVVM.Behaviors;

public class ScrollBarDragBehavior : Behavior<Rectangle>
{
    public static readonly DependencyProperty PreviewMouseDownCommandProperty = DependencyProperty.Register(nameof(PreviewMouseDownCommand), typeof(ICommand), typeof(ScrollBarDragBehavior));

    ScrollBar parentScrollBar;
    TextBox textBox;

    public ICommand PreviewMouseDownCommand
    {
        get => (ICommand)GetValue(PreviewMouseDownCommandProperty);
        set => SetValue(PreviewMouseDownCommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseDown += Rectangle_PreviewMouseDown;
        parentScrollBar = FindParentScrollBar();
        textBox = FindTextBox();
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseDown -= Rectangle_PreviewMouseDown;
        parentScrollBar = null;
        textBox = null;
        base.OnDetaching();
    }

    void Rectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if(PreviewMouseDownCommand?.CanExecute(e) == true)
        {
            if(e.LeftButton == MouseButtonState.Pressed && parentScrollBar != null)
            {
                Point clickPoint = e.GetPosition(parentScrollBar);

                double proportion;
                if(parentScrollBar.Orientation == Orientation.Vertical)
                    proportion = clickPoint.Y / parentScrollBar.ActualHeight;
                else
                    proportion = clickPoint.X / parentScrollBar.ActualWidth;

                double newValue = proportion * (parentScrollBar.Maximum - parentScrollBar.Minimum) + parentScrollBar.Minimum;
                newValue = Math.Max(parentScrollBar.Minimum, Math.Min(newValue, parentScrollBar.Maximum));
                
                parentScrollBar.Value = newValue;
                
                // Update the TextBox scroll position directly
                if(textBox != null)
                {
                    if(parentScrollBar.Orientation == Orientation.Vertical)
                        textBox.ScrollToVerticalOffset(newValue);
                    else
                        textBox.ScrollToHorizontalOffset(newValue);
                }

                PreviewMouseDownCommand.Execute(e);
                e.Handled = true;
            }
        }
    }

    ScrollBar FindParentScrollBar()
    {
        DependencyObject current = AssociatedObject;
        while(current != null && !(current is ScrollBar))
            current = VisualTreeHelper.GetParent(current);

        return current as ScrollBar;
    }
    
    TextBox FindTextBox()
    {
        // Traverse up to find ScrollViewer
        DependencyObject current = AssociatedObject;
        ScrollViewer scrollViewer = null;
        
        while(current != null)
        {
            current = VisualTreeHelper.GetParent(current);
            
            if(current is ScrollViewer sv)
            {
                scrollViewer = sv;
                break;
            }
        }
        
        if(scrollViewer == null) return null;
        
        // Find the TextBox that owns this ScrollViewer
        current = scrollViewer;
        while(current != null && !(current is TextBox))
        {
            current = VisualTreeHelper.GetParent(current);
        }
        
        return current as TextBox;
    }
}