using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using NotepadEx.Extensions;
using NotepadEx.MVVM.ViewModels;
using NotepadEx.Util;

namespace NotepadEx.MVVM.View.UserControls;
public partial class CustomTitleBar : UserControl
{
    public CustomTitleBar() => InitializeComponent();

    public static CustomTitleBarViewModel InitializeTitleBar(Window window, string windowName, bool showMinimize = true, bool showMaximize = true, bool showExit = true, bool isResizable = true, Action onClose = null)
    {
        var titleBarViewModel = new CustomTitleBarViewModel(window, isResizable);
        titleBarViewModel.Initialize(windowName, showMinimize, showMaximize, showExit, onClose);
        titleBarViewModel.IconImage = new BitmapImage(new Uri(DirectoryUtil.ImagePath_MainIcon.ToUriPath()));

        return titleBarViewModel;
    }
}