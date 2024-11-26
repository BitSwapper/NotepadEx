using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace NotepadEx.MVVM.Behaviors;

public class MenuItemBehavior : Behavior<MenuItem>
{
    public static readonly DependencyProperty ClickCommandProperty = DependencyProperty.Register(nameof(ClickCommand), typeof(ICommand), typeof(MenuItemBehavior));

    public ICommand ClickCommand
    {
        get => (ICommand)GetValue(ClickCommandProperty);
        set => SetValue(ClickCommandProperty, value);
    }

    protected override void OnAttached()
    {
        base.OnAttached();
        AssociatedObject.Click += MenuItem_Click;
    }

    protected override void OnDetaching()
    {
        if(AssociatedObject != null)
            AssociatedObject.Click -= MenuItem_Click;

        base.OnDetaching();
    }

    void MenuItem_Click(object sender, RoutedEventArgs e)
    {
        if(e.OriginalSource is MenuItem menuItem && menuItem != AssociatedObject && ClickCommand?.CanExecute(e) == true)
            ClickCommand.Execute(e);
    }
}
