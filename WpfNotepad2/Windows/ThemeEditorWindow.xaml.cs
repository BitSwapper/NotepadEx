using System.Windows;
using System.Windows.Input;
using NotepadEx.Util;

namespace NotepadEx.Windows;

/// <summary>
/// Interaction logic for ThemeEditorWindow.xaml
/// </summary>
public partial class ThemeEditorWindow : Window
{
    WindowState prevWindowState;
    WindowResizer resizer;
    public ThemeEditorWindow()
    {
        InitializeComponent();
        resizer = new WindowResizer();
        ThemeEditorTitleBar.Init(this, "Theme Editor", Minimize_Click, null!, Close_Click);
    }

    void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(e.ClickCount == 2)
            resizer.DoWindowMaximizedStateChange(this, prevWindowState);
    }

    void Border_MouseMove(object sender, MouseEventArgs e)
    {
        var position = e.GetPosition(this);
        WindowResizer.ResizeWindow(this, position, Constants.ResizeBorderWidth);
    }

    void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    void Close_Click(object sender, RoutedEventArgs e) => Close();
}
