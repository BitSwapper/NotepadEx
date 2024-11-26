using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace NotepadEx.MVVM.Behaviors;

public class ScrollViewerBehavior : Behavior<Grid>
{
    public static readonly DependencyProperty MouseWheelCommandProperty = DependencyProperty.Register(nameof(MouseWheelCommand), typeof(ICommand), typeof(ScrollViewerBehavior));

    ScrollViewer scrollViewer;
    ScrollBar verticalScrollBar;

    public ICommand MouseWheelCommand
    {
        get => (ICommand)GetValue(MouseWheelCommandProperty);
        set => SetValue(MouseWheelCommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseWheel += Grid_PreviewMouseWheel;
        InitializeScrollComponents();
    }

    protected override void OnDetaching()
    {
        if(AssociatedObject != null)
        {
            AssociatedObject.PreviewMouseWheel -= Grid_PreviewMouseWheel;
        }
        scrollViewer = null;
        verticalScrollBar = null;
        base.OnDetaching();
    }

    void InitializeScrollComponents()
    {
        if(AssociatedObject.TemplatedParent is ScrollViewer scrollViewer)
        {
            this.scrollViewer = scrollViewer;
            verticalScrollBar = AssociatedObject.FindName("PART_VerticalScrollBar") as ScrollBar;
        }
    }

    void Grid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if(MouseWheelCommand?.CanExecute(e) == true)
        {
            MouseWheelCommand.Execute(e);

            if(scrollViewer != null && verticalScrollBar != null)
            {
                double newOffset = scrollViewer.VerticalOffset - (e.Delta / 3.0);
                newOffset = Math.Max(0, Math.Min(newOffset, scrollViewer.ScrollableHeight));

                scrollViewer.ScrollToVerticalOffset(newOffset);
                verticalScrollBar.Value = newOffset;
            }

            e.Handled = true;
        }
    }
}
