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
    ScrollBar horizontalScrollBar;

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
            AssociatedObject.PreviewMouseWheel -= Grid_PreviewMouseWheel;
        
        scrollViewer = null;
        verticalScrollBar = null;
        horizontalScrollBar = null;
        base.OnDetaching();
    }

    void InitializeScrollComponents()
    {
        // This grid should be the PART_Root from the ScrollViewer template
        if(AssociatedObject.TemplatedParent is ScrollViewer sv)
        {
            scrollViewer = sv;
            
            // Find the scrollbars within the template
            verticalScrollBar = AssociatedObject.FindName("PART_VerticalScrollBar") as ScrollBar;
            horizontalScrollBar = AssociatedObject.FindName("PART_HorizontalScrollBar") as ScrollBar;
        }
    }

    void Grid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if(MouseWheelCommand?.CanExecute(e) == true)
        {
            MouseWheelCommand.Execute(e);

            if(scrollViewer != null && verticalScrollBar != null)
            {
                // Calculate new offset with smoother scrolling (delta divided by smaller value for more precision)
                double newOffset = scrollViewer.VerticalOffset - (e.Delta / 120.0);
                
                // Ensure offset stays within bounds
                newOffset = Math.Max(0, Math.Min(newOffset, scrollViewer.ScrollableHeight));

                // Update both the scrollviewer and scrollbar to keep them in sync
                scrollViewer.ScrollToVerticalOffset(newOffset);
                verticalScrollBar.Value = newOffset;
            }

            e.Handled = true;
        }
    }
}