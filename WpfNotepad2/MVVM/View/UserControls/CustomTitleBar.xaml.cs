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


//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
//using System.Windows.Media.Imaging;
//using NotepadEx.Util;

//namespace NotepadEx.MVVM.View.UserControls;

//public partial class CustomTitleBar : UserControl
//{
//    bool IsResizeable { get; set; }
//    Window WindowRef;
//    Action<object, RoutedEventArgs> Minimize;
//    Action<object, RoutedEventArgs> Maximize;
//    Action<object, RoutedEventArgs> Close;


//    public static readonly DependencyProperty ImageSourceProperty =
//        DependencyProperty.Register("ImageSource", typeof(BitmapImage), typeof(CustomTitleBar), new PropertyMetadata(default(BitmapImage)));

//    public BitmapImage ImageSource
//    {
//        get => (BitmapImage)GetValue(ImageSourceProperty);
//        set => SetValue(ImageSourceProperty, value);
//    }

//    public CustomTitleBar()
//    {
//        InitializeComponent();
//        DataContext = this;
//    }

//    public void Init(Window window, string windowTitleText, bool isResizable, Action<object, RoutedEventArgs> Minimize = null, Action<object, RoutedEventArgs> Maximize = null, Action<object, RoutedEventArgs> Close = null)
//    {
//        WindowRef = window;
//        IsResizeable = isResizable;
//        SetText(windowTitleText);
//        this.Minimize = Minimize;
//        this.Maximize = Maximize;
//        this.Close = Close;

//        if(Minimize == null) btnMinimize.Visibility = Visibility.Collapsed;
//        if(Maximize == null) btnMaximize.Visibility = Visibility.Collapsed;
//        if(Close == null) btnExit.Visibility = Visibility.Collapsed;
//    }

//    void txtTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
//    {
//        if(!IsResizeable || (IsResizeable && e.GetPosition(this).Y > UIConstants.ResizeBorderWidth / 2))
//            WindowRef?.DragMove();
//    }

//    void btnMinimize_Click(object sender, RoutedEventArgs e) => Minimize.Invoke(sender, e);

//    void btnMaximize_Click(object sender, RoutedEventArgs e) => Maximize.Invoke(sender, e);

//    void btnExit_Click(object sender, RoutedEventArgs e) => Close.Invoke(sender, e);

//    public void SetText(string text) => txtTitleBar.Text = text;
//}
