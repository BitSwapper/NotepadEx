using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using NotepadEx.Util;

namespace NotepadEx.MVVM.ViewModels;

public class CustomTitleBarViewModel : ViewModelBase
{
    Window window;
    BitmapImage iconImage;
    string titleText;
    bool isResizeable;
    bool showMinimizeButton = true;
    bool showMaximizeButton = true;
    bool showCloseButton = true;

    public string TitleText
    {
        get => titleText;
        set => SetProperty(ref titleText, value);
    }

    public BitmapImage IconImage
    {
        get => iconImage;
        set => SetProperty(ref iconImage, value);
    }

   bool _isMaximized;

    public bool IsMaximized
    {
        get => _isMaximized;
        set => SetProperty(ref _isMaximized, value);
    }

    public bool ShowMinimizeButton
    {
        get => showMinimizeButton;
        set => SetProperty(ref showMinimizeButton, value);
    }

    public bool ShowMaximizeButton
    {
        get => showMaximizeButton;
        set => SetProperty(ref showMaximizeButton, value);
    }

    public bool ShowCloseButton
    {
        get => showCloseButton;
        set => SetProperty(ref showCloseButton, value);
    }

    public ICommand MinimizeCommand { get; }
    public ICommand MaximizeCommand { get; }
    public ICommand CloseCommand { get; }
    public ICommand TitleBarMouseDownCommand { get; }

    public CustomTitleBarViewModel(Window window, bool isResizeable = true)
    {
        this.window = window;
        isResizeable = isResizeable;

        MinimizeCommand = new RelayCommand(ExecuteMinimize);
        MaximizeCommand = new RelayCommand(ExecuteMaximize);
        CloseCommand = new RelayCommand(ExecuteClose);
        TitleBarMouseDownCommand = new RelayCommand<MouseButtonEventArgs>(ExecuteTitleBarMouseDown);
    }

    void ExecuteMinimize() => window.WindowState = WindowState.Minimized;

    void ExecuteMaximize()
    {
        IsMaximized = !IsMaximized;
        WindowResizerUtil.ToggleMaximizeState(window);
    }

    void ExecuteClose() => window.Close();

    void ExecuteTitleBarMouseDown(MouseButtonEventArgs e)
    {
        if(!isResizeable || (isResizeable && e.GetPosition(window).Y > UIConstants.ResizeBorderWidth / 2))
            window.DragMove();
    }

    public void Initialize(string titleText, bool showMinimize = true, bool showMaximize = true, bool showClose = true)
    {
        TitleText = titleText;
        ShowMinimizeButton = showMinimize;
        ShowMaximizeButton = showMaximize;
        ShowCloseButton = showClose;
    }
}