using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace NotepadEx.MVVM.Behaviors;

public class ScrollViewerBehavior : Behavior<Grid>
{
    public static readonly DependencyProperty MouseWheelCommandProperty =
        DependencyProperty.Register(
            nameof(MouseWheelCommand),
            typeof(ICommand),
            typeof(ScrollViewerBehavior));

    public ICommand MouseWheelCommand
    {
        get => (ICommand)GetValue(MouseWheelCommandProperty);
        set => SetValue(MouseWheelCommandProperty, value);
    }

    private ScrollViewer _scrollViewer;
    private ScrollBar _verticalScrollBar;

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.PreviewMouseWheel += Grid_PreviewMouseWheel;

        // Get reference to parent ScrollViewer and ScrollBar
        InitializeScrollComponents();
    }

    protected override void OnDetaching()
    {
        if(AssociatedObject != null)
        {
            AssociatedObject.PreviewMouseWheel -= Grid_PreviewMouseWheel;
        }
        _scrollViewer = null;
        _verticalScrollBar = null;
        base.OnDetaching();
    }

    private void InitializeScrollComponents()
    {
        if(AssociatedObject.TemplatedParent is ScrollViewer scrollViewer)
        {
            _scrollViewer = scrollViewer;
            _verticalScrollBar = AssociatedObject.FindName("PART_VerticalScrollBar") as ScrollBar;
        }
    }

    private void Grid_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        if(MouseWheelCommand?.CanExecute(e) == true)
        {
            // Execute the command
            MouseWheelCommand.Execute(e);

            // Update scrollbar and offset directly for smoother scrolling
            if(_scrollViewer != null && _verticalScrollBar != null)
            {
                double newOffset = _scrollViewer.VerticalOffset - (e.Delta / 3.0);
                newOffset = Math.Max(0, Math.Min(newOffset, _scrollViewer.ScrollableHeight));

                _scrollViewer.ScrollToVerticalOffset(newOffset);
                _verticalScrollBar.Value = newOffset;
            }

            e.Handled = true;
        }
    }
}
