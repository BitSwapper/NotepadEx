using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace NotepadEx.MVVM.Behaviors;

public abstract class BehaviorBase<T> : Behavior<T> where T : FrameworkElement
{
    protected override void OnDetaching()
    {
        base.OnDetaching();
        Cleanup();
    }

    protected virtual void Cleanup()
    {
        // Override this method to cleanup any subscriptions or resources
    }
}
