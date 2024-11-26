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

    private async void App_Activated(object sender, EventArgs e)
    {
        if(initialized) return;
        initialized = true;

        if(startupArgs.Length == 1)  //Single file - use existing window
        {
            string filePath = startupArgs[0];
            string fileContent = File.ReadAllText(filePath);
            string fileName = Path.GetFileName(filePath);

            if(Current.MainWindow?.DataContext is MainWindowViewModel viewModel)
            {
                viewModel.DocumentContent = fileContent;
                viewModel.Document.FilePath = filePath;
                viewModel.TitleBarViewModel.TitleText = $"NotepadEx  |  {fileName}";
                viewModel.Document.IsModified = false;
            }
        }
        //** TO DO
        //Multi Files only partially works as it bugs out about accessing the config file while another proccess is doing that. not clear how to fix 
        //even adding delay did not help whatsoever
        //** Also it's not even going to be a multiline argument but rather, when we multiselect and press enter, it starts x amount of proccesses with a single arg each (thus increasing difficulty in solving this)

        this.Activated -= App_Activated;
    }

    private async Task OpenFileWithDelay(string filePath)
    {
        string fileContent = File.ReadAllText(filePath);
        var newWindow = new MainWindow();
        if(newWindow.DataContext is MainWindowViewModel viewModel)
        {
            viewModel.DocumentContent = fileContent;
            viewModel.Document.FilePath = filePath;
            viewModel.TitleBarViewModel.TitleText = $"NotepadEx  |  {Path.GetFileName(filePath)}";
        }
        OffsetWindowPosFromLast(newWindow);
        newWindow.Show();
        await Task.Delay(500);
    }

    public async Task OpenFileInNewWindow(string filePath)
    {
        try
        {
            await OpenFileWithDelay(filePath);
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