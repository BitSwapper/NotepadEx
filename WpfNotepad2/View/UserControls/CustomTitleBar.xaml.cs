using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfNotepad2.Util;

namespace WpfNotepad2.View.UserControls;

public partial class CustomTitleBar : UserControl
{
    Window WindowRef;
    Action<object, RoutedEventArgs> Minimize;
    Action<object, RoutedEventArgs> Maximize;
    Action<object, RoutedEventArgs> Close;

    public CustomTitleBar() => InitializeComponent();

    public void Init(Window window, Action<object, RoutedEventArgs> Minimize, Action<object, RoutedEventArgs> Maximize, Action<object, RoutedEventArgs> Close)
    {
        WindowRef = window;
        this.Minimize = Minimize;
        this.Maximize = Maximize;
        this.Close = Close;
    }

    void txtTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if(e.GetPosition(this).Y > UiLayoutConstants.ResizeBorderWidth)
            WindowRef?.DragMove();
    }

    void btnMinimize_Click(object sender, RoutedEventArgs e) => Minimize.Invoke(sender, e);

    void btnMaximize_Click(object sender, RoutedEventArgs e) => Maximize.Invoke(sender, e);

    void btnExit_Click(object sender, RoutedEventArgs e) => Close.Invoke(sender, e);
}
