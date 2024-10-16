using System.Windows;

namespace NotepadEx.Util;
public static class AppResourceUtil<T> where T : class
{
    public static bool TrySetResource<T>(Application app, string path, T value)
    {
        try
        {
            app.Resources[path] = value;
            return true;
        }
        catch(Exception ex) { MessageBox.Show(ex.Message); }
        return false;
    }

    public static T TryGetResource(Application app, string path)
    {
        try
        {
            return (app.Resources[path] as T);
        }
        catch(Exception ex) { MessageBox.Show(ex.Message); }
        return null;
    }
}
