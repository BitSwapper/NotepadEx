using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace NotepadEx.MVVM.Behaviors;

public class WindowMouseMoveBehavior : Behavior<Window>
{
    public static readonly DependencyProperty MouseMoveCommandProperty =
        DependencyProperty.Register(nameof(MouseMoveCommand), typeof(ICommand),
            typeof(WindowMouseMoveBehavior));

    public static readonly DependencyProperty ResizeCommandProperty =
        DependencyProperty.Register(nameof(ResizeCommand), typeof(ICommand),
            typeof(WindowMouseMoveBehavior));

    public ICommand MouseMoveCommand
    {
        get => (ICommand)GetValue(MouseMoveCommandProperty);
        set => SetValue(MouseMoveCommandProperty, value);
    }

    public ICommand ResizeCommand
    {
        get => (ICommand)GetValue(ResizeCommandProperty);
        set => SetValue(ResizeCommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.MouseMove += Window_MouseMove;
    }

    protected void Cleanup()
    {
        if(AssociatedObject != null)
            AssociatedObject.MouseMove -= Window_MouseMove;
    }

    void Window_MouseMove(object sender, MouseEventArgs e)
    {
        var window = AssociatedObject;
        var position = e.GetPosition(window);

        if(MouseMoveCommand?.CanExecute(position.Y) == true)
            MouseMoveCommand.Execute(position.Y);

        if(window.WindowState == WindowState.Normal && ResizeCommand?.CanExecute(position) == true)
            ResizeCommand.Execute(position);
    }
}
