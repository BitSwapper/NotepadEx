using System.Windows;
using System.Windows.Input;

namespace NotepadEx.MVVM.Behaviors;

public static class MouseBehavior
{
    public static readonly DependencyProperty MouseDownCommandProperty = DependencyProperty.RegisterAttached("MouseDownCommand", typeof(ICommand), typeof(MouseBehavior), new PropertyMetadata(null, OnMouseDownCommandChanged));

    public static ICommand GetMouseDownCommand(DependencyObject obj) => (ICommand)obj.GetValue(MouseDownCommandProperty);

    public static void SetMouseDownCommand(DependencyObject obj, ICommand value) => obj.SetValue(MouseDownCommandProperty, value);

    static void OnMouseDownCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if(d is UIElement element)
        {
            if(e.OldValue != null)
                element.MouseLeftButtonDown -= Element_MouseLeftButtonDown;
            if(e.NewValue != null)
                element.MouseLeftButtonDown += Element_MouseLeftButtonDown;
        }
    }

    static void Element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        var element = sender as UIElement;
        var command = GetMouseDownCommand(element);

        if(command?.CanExecute(e) == true)
            command.Execute(e);
    }
}
