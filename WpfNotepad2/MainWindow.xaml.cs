using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using WpfNotepad2.Util;

namespace WpfNotepad2;

public partial class MainWindow : Window
{
    public MainWindow() => InitializeComponent();

    void btnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    void btnExit_Click(object sender, RoutedEventArgs e) => Close();

    void txtTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if(e.GetPosition(this).Y > UiLayoutConstants.ResizeBorderWidth)
            DragMove();
    }

    void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(e.ClickCount == 2)
            WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    void Border_MouseMove(object sender, MouseEventArgs e)
    {
        var position = e.GetPosition(this);
        WindowResizer.ResizeWindow(this, position, UiLayoutConstants.ResizeBorderWidth);
    }



    void MenuItemNew_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemNewWindow_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemOpen_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemOpenRecent_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemMultiOpen_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemSaveAs_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemSave_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemPrint_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemExit_Click(object sender, RoutedEventArgs e)
    {

    }
}