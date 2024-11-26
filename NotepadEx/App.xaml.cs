using System.IO;
using System.Windows;
using NotepadEx.MVVM.ViewModels;
using Application = System.Windows.Application;

namespace NotepadEx;

public partial class App : Application
{
    private string[] startupArgs;
    private bool initialized = false;
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        startupArgs = e.Args;

        this.Activated += App_Activated;
    }

    private void App_Activated(object sender, EventArgs e)
    {
        if(initialized) return;
        initialized = true;

        if(startupArgs.Length == 1)  // Single file - use existing window
        {
            string filePath = startupArgs[0];
            string fileContent = File.ReadAllText(filePath);
            string fileName = Path.GetFileName(filePath);
 
            if(Current.MainWindow?.DataContext is MainWindowViewModel viewModel)
            {
                viewModel.DocumentContent = fileContent;
                viewModel.DocumentFilePath = filePath;
                viewModel.TitleBarViewModel.TitleText = $"NotepadEx  |  {fileName}";
            }
        }
        else if(startupArgs.Length > 1)  //Multi files
        {
            Current.MainWindow?.Close();

            foreach(string filePath in startupArgs)
            {
                try
                {
                    string fileContent = File.ReadAllText(filePath);
                    var newWindow = new MainWindow();
                    if(newWindow.DataContext is MainWindowViewModel viewModel)
                    {
                        viewModel.TitleBarViewModel.TitleText = $"NotepadEx  |  {Path.GetFileName(filePath)}";
                        viewModel.DocumentContent = fileContent;
                        viewModel.DocumentFilePath = filePath;
                    }
                    newWindow.Show();
                }
                catch(Exception ex)
                {
                    MessageBox.Show($"Error opening file '{filePath}': {ex.Message}",
                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        this.Activated -= App_Activated;
    }

    public void OpenFileInNewWindow(string filePath)
    {
        try
        {
            string fileContent = File.ReadAllText(filePath);
            var window = new MainWindow();

            if(window.DataContext is MainWindowViewModel viewModel)
            {
                viewModel.TitleBarViewModel.TitleText = $"NotepadEx  |  {Path.GetFileName(filePath)}";
                viewModel.DocumentContent = fileContent;
                viewModel.DocumentFilePath = filePath;
            }

            OffsetWindowPosFromLast(window);
            window.Show();
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error opening file '{filePath}': {ex.Message}",
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    static void OffsetWindowPosFromLast(MainWindow window)
    {
        if(Current.Windows.Count > 0)
        {
            var lastWindow = Current.Windows[Current.Windows.Count - 1];
            window.Left = lastWindow.Left + 20;
            window.Top = lastWindow.Top + 20;

            //Reset position if off screen
            var screen = SystemParameters.WorkArea;
            if(window.Left + window.Width > screen.Right)
                window.Left = 0;
            if(window.Top + window.Height > screen.Bottom)
                window.Top = 0;
        }
    }
}


