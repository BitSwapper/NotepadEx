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
    public static readonly DependencyProperty PreviewMouseDownCommandProperty =
            DependencyProperty.Register(
                nameof(PreviewMouseDownCommand),
                typeof(ICommand),
                typeof(ScrollBarDragBehavior));

    public ICommand PreviewMouseDownCommand
    {
        get => (ICommand)GetValue(PreviewMouseDownCommandProperty);
        set => SetValue(PreviewMouseDownCommandProperty, value);
    }

    ScrollBar _parentScrollBar;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseDown += Rectangle_PreviewMouseDown;

        // Find parent ScrollBar
        _parentScrollBar = FindParentScrollBar();
    }

    protected override void OnDetaching()
    {
        AssociatedObject.PreviewMouseDown -= Rectangle_PreviewMouseDown;
        _parentScrollBar = null;
        base.OnDetaching();
    }

    void Rectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if(PreviewMouseDownCommand?.CanExecute(e) == true)
        {
            if(e.LeftButton == MouseButtonState.Pressed && _parentScrollBar != null)
            {
                // Get click position relative to the scrollbar
                Point clickPoint = e.GetPosition(_parentScrollBar);

                // Calculate new scroll position based on click location
                double proportion;
                if(_parentScrollBar.Orientation == Orientation.Vertical)
                {
                    proportion = clickPoint.Y / _parentScrollBar.ActualHeight;
                }
                else
                {
                    proportion = clickPoint.X / _parentScrollBar.ActualWidth;
                }

                // Update scrollbar value
                double newValue = proportion * (_parentScrollBar.Maximum - _parentScrollBar.Minimum) + _parentScrollBar.Minimum;
                _parentScrollBar.Value = newValue;

                PreviewMouseDownCommand.Execute(e);
                e.Handled = true;
            }
        }
    }

    ScrollBar FindParentScrollBar()
    {
        DependencyObject current = AssociatedObject;
        while(current != null && !(current is ScrollBar))
        {
            current = VisualTreeHelper.GetParent(current);
        }
        return current as ScrollBar;
    }
}
