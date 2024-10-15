using System.Diagnostics;
using System.Windows;

namespace NotepadEx;
internal class AdditionalWindowManager
{
    public static void TryCreateNewNotepadWindow()
    {
        try
        {
            string appPath = System.Windows.Forms.Application.ExecutablePath;
            Process.Start(appPath);
        }
        catch(Exception ex)
        {
            MessageBox.Show("Error launching new instance: " + ex.Message);
        }
    }
}
