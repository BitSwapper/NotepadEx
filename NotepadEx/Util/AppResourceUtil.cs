using System.Windows;

namespace NotepadEx.Util;

public static class AppResourceUtil
{
    public static bool TrySetResource(Application app, string path, object value)
    {
        try
        {
            app.Resources[path] = value;
            return true;
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
            return false;
        }
    }
}

// Generic class for type-safe getting of resources
public static class AppResourceUtil<T>
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
            var resource = app.Resources[path];
            if(resource is T typedResource)
            {
                return typedResource;
            }
            return default;
        }
        catch(Exception ex)
        {
            MessageBox.Show(ex.Message);
            return default;
        }
    }
}
//public static class AppResourceUtil<T> where T : class
//{
//    public static bool TrySetResource<T>(Application app, string path, T value)
//    {
//        try
//        {
//            app.Resources[path] = value;
//            return true;
//        }
//        catch(Exception ex) { MessageBox.Show(ex.Message); }
//        return false;
//    }

//    public static T TryGetResource(Application app, string path)
//    {
//        try
//        {
//            return (app.Resources[path] as T);
//        }
//        catch(Exception ex) { MessageBox.Show(ex.Message); }
//        return null;
//    }
//}
