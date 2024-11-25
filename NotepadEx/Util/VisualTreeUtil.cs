using System.Windows;
using System.Windows.Media;

namespace NotepadEx.Util;
public static class VisualTreeUtil
{

    public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
    {
        if(depObj != null)
        {
            for(int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                if(child != null && child is T)
                {
                    yield return (T)child;
                }

                foreach(T childOfChild in FindVisualChildren<T>(child))
                {
                    yield return childOfChild;
                }
            }
        }
    }
    public static T FindVisualChild<T>(DependencyObject parent, string childName) where T : DependencyObject
    {
        for(int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if(child is T typedChild && (string.IsNullOrEmpty(childName) || (child is FrameworkElement fe && fe.Name == childName)))
                return typedChild;

            var result = FindVisualChild<T>(child, childName);
            if(result != null)
                return result;
        }
        return null;
    }

    public static T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        DependencyObject parentObject = VisualTreeHelper.GetParent(child);

        if(parentObject == null) return null;

        T parent = parentObject as T;
        if(parent != null)
            return parent;
        else
            return FindParent<T>(parentObject);
    }
}
