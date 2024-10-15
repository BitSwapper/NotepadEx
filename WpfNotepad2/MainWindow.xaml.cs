using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using WpfNotepad2.Properties;
using WpfNotepad2.Util;
using Point = System.Windows.Point;

namespace WpfNotepad2;

public partial class MainWindow : Window
{
    const string appName = "NotepadEx";
    string currentFileName = string.Empty;
    bool hasTextChangedSinceSave = false;
    public int InfoBarSize { get; init; } = 18;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = this;
        InitUI();

        RecentFileManager.LoadRecentFilesFromSettings();
        RecentFileManager.PopulateRecentFilesMenu(DropDown_File);
    }

    void InitUI()
    {
       

        txtEditor.TextWrapping = Settings.Default.TextWrapping ? TextWrapping.Wrap : TextWrapping.NoWrap;
        MenuItem_ToggleWrapping.IsChecked = Settings.Default.TextWrapping;

        SetupMainMenuBar(!Settings.Default.MenuBarAutoHide);
        MenuItem_AutoHideMenuBar.IsChecked = Settings.Default.MenuBarAutoHide;

        SetupInfoBar();
    }

    void btnMinimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    void btnExit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    void txtTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        if(e.GetPosition(this).Y > UiLayoutConstants.ResizeBorderWidth)
            DragMove();
    }

    void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        //if(e.ClickCount == 2)
        //    WindowState = WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
    }

    void Border_MouseMove(object sender, MouseEventArgs e)
    {
        var position = e.GetPosition(this);
        WindowResizer.ResizeWindow(this, position, UiLayoutConstants.ResizeBorderWidth);
    }

    void MenuItemNew_Click(object sender, RoutedEventArgs e)
    {
        if(DocumentHelper.PromptToSaveChanges(hasTextChangedSinceSave, SaveDocument))
        {
            txtEditor.Text = string.Empty;
            currentFileName = string.Empty;
            UpdateTitleText(currentFileName);
            //isTextChanged = false;
            //UpdateStatusBarInfo();
        }
    }

    void MenuItemNewWindow_Click(object sender, RoutedEventArgs e) => AdditionalWindowManager.TryCreateNewNotepadWindow();

    void MenuItemOpen_Click(object sender, RoutedEventArgs e) => DocumentHelper.OpenDocument(SaveDocument, LoadDocument, hasTextChangedSinceSave, appName);

    void MenuItemOpenRecent_Click(object sender, RoutedEventArgs e)
    {
        MenuItem menuItem = (MenuItem)sender;
        MenuItem subMenuItem = (MenuItem)e.OriginalSource;
        DocumentHelper.OpenDocument(SaveDocument, LoadDocument, hasTextChangedSinceSave, appName, (string)subMenuItem.Header);
    }

    void MenuItemMultiOpen_Click(object sender, RoutedEventArgs e)
    {
        //
    }

    void MenuItemSave_Click(object sender, RoutedEventArgs e) => SaveDocument();

    void MenuItemSaveAs_Click(object sender, RoutedEventArgs e) => SaveDocumentAs();

    void MenuItemPrint_Click(object sender, RoutedEventArgs e) => DocumentHelper.PrintDocument(txtEditor);

    void MenuItemExit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    void MenuItemFont_Click(object sender, RoutedEventArgs e)
    {
        //
    }

    void MenuItemWordWrap_Click(object sender, RoutedEventArgs e)
    {
        var menuItem = sender as MenuItem;
        txtEditor.TextWrapping = txtEditor.TextWrapping == TextWrapping.NoWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
        SaveSettings();
        menuItem.IsChecked = Settings.Default.TextWrapping;
    }

    void MenuItemAutohideMenuBar_Click(object sender, RoutedEventArgs e)
    {
        Settings.Default.MenuBarAutoHide = !Settings.Default.MenuBarAutoHide;
        Settings.Default.Save();
        SetupMainMenuBar(!Settings.Default.MenuBarAutoHide);
        MenuItem_AutoHideMenuBar.IsChecked = Settings.Default.MenuBarAutoHide;
    }

    void MenuItemToggleInfoBar_Click(object sender, RoutedEventArgs e)
    {
        Settings.Default.InfoBarVisible = !Settings.Default.InfoBarVisible;
        Settings.Default.Save();
        SetupInfoBar();
    }

    void MenuItemFindReplace_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemCopy_Click(object sender, RoutedEventArgs e) => txtEditor.Copy();

    void MenuItemCut_Click(object sender, RoutedEventArgs e) => txtEditor.Cut();

    void MenuItemPaste_Click(object sender, RoutedEventArgs e) => txtEditor.Paste();

    void MenuItemUndo_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemRedo_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemDelete_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemSelectAll_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemTimeDate_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemTheme_Click(object sender, RoutedEventArgs e)
    {

    }

    bool SaveDocumentAs() => SaveFile(false);

    bool SaveDocument() => SaveFile(true);

    bool SaveFile(bool useCurrentFileName)
    {
        string fileName = useCurrentFileName ? currentFileName : string.Empty;

        if(string.IsNullOrEmpty(fileName))
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            saveFileDialog.DefaultExt = ".txt";

            if(saveFileDialog.ShowDialog() == true)
                fileName = saveFileDialog.FileName;
            else
                return false;
        }

        File.WriteAllText(fileName, txtEditor.Text);
        UpdateTitleText(fileName);
        currentFileName = fileName;
        //UpdateModifiedStateOfTitleBar();
        AddRecentFile(fileName);
        return true;
    }

    void LoadDocument(string fileName)
    {
        if(!File.Exists(fileName))
        {
            MessageBox.Show($"Trouble finding file '{fileName}'");
            return;
        }
        txtEditor.Text = File.ReadAllText(fileName);
        currentFileName = fileName;
        //isTextChanged = false;
        //UpdateStatusBarInfo();
        UpdateTitleText(fileName);
        AddRecentFile(fileName);
    }

    void UpdateTitleText(string fileName) => Title = txtTitleBar.Text = fileName == string.Empty ? appName : $"{appName}  |  " + Path.GetFileName(fileName);

    void AddRecentFile(string filePath) => RecentFileManager.AddRecentFile(filePath, DropDown_File, SaveSettings);

    void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => SaveSettings();

    void SaveSettings() => SettingsManager.SaveSettings(txtEditor);

    void txtEditor_MouseMove(object sender, MouseEventArgs e) => ShowMenuBarIfMouseInRange(e);

    void ShowMenuBarIfMouseInRange(MouseEventArgs e)
    {
        if(Settings.Default.MenuBarAutoHide == false && MainMenuBar.IsEnabled) return;

        Point mousePosition = e.GetPosition(txtEditor);
        if(mousePosition.Y < 2)
        {
            if(!MainMenuBar.IsEnabled)
            {
                SetupMainMenuBar(true);
            }
        }
        else
        {
            if(MainMenuBar.IsEnabled)
            {
                Point menuPosition = e.GetPosition(MainMenuBar);
                if(menuPosition.X < 0 || menuPosition.X > MainMenuBar.ActualWidth || menuPosition.Y < 0 || menuPosition.Y > MainMenuBar.ActualHeight)
                {
                    SetupMainMenuBar(false);
                }
            }
        }
    }

    void SetupMainMenuBar(bool showMenuBar)
    {
        if(!showMenuBar)
        {
            Row_MainMenuBar.Height = new(0, GridUnitType.Pixel);
            Row_MainMenuBar.IsEnabled = false;
            MainMenuBar.IsEnabled = false;
        }
        else
        {
            Row_MainMenuBar.Height = new(InfoBarSize, GridUnitType.Pixel);
            Row_MainMenuBar.IsEnabled = true;
            MainMenuBar.IsEnabled = true;
        }
    }

    void SetupInfoBar()
    {
        if(!Settings.Default.InfoBarVisible)
        {
            Row_InfoBar.Height = new(0, GridUnitType.Pixel);
            Row_InfoBar.IsEnabled = false;
        }
        else
        {
            Row_InfoBar.Height = new(InfoBarSize, GridUnitType.Pixel);
            Row_InfoBar.IsEnabled = true;
        }

        MenuItem_ToggleInfoBar.IsChecked = Settings.Default.InfoBarVisible;
    }

    void btnMaximize_Click(object sender, RoutedEventArgs e)
    {
        DoWindowStateChange();
    }


    bool isManuallyMaximized = false;
    double oldLeft;
    double oldTop;
    double oldWidth;
    double oldHeight;
    private void Window_StateChanged(object sender, EventArgs e) => DoWindowStateChange();

    void DoWindowStateChange()
    {
        if(!isManuallyMaximized)
        {
            isManuallyMaximized = true;

            // Get the screen where the window is currently located
            var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)this.Left, (int)this.Top));
            var workingArea = screen.WorkingArea;


            oldLeft = Left;
            oldTop = Top;
            oldWidth = Width;
            oldHeight = Height;

            this.Left = workingArea.Left;
            this.Top = workingArea.Top;
            this.Width = workingArea.Width;
            this.Height = workingArea.Height;
        }
        else if(isManuallyMaximized)
        {
            Left = oldLeft;
            Top = oldTop;
            Width = oldWidth;
            Height = oldHeight;
            isManuallyMaximized = false;
        }
    }





    //private const int WM_GETMINMAXINFO = 0x0024;

    //[StructLayout(LayoutKind.Sequential)]
    //public struct MINMAXINFO
    //{
    //    public POINT ptReserved;
    //    public POINT ptMaxSize;
    //    public POINT ptMaxPosition;
    //    public POINT ptMinTrackSize;
    //    public POINT ptMaxTrackSize;
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct POINT
    //{
    //    public int x;
    //    public int y;
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct MONITORINFO
    //{
    //    public int cbSize;
    //    public RECT rcMonitor;
    //    public RECT rcWork;
    //    public uint dwFlags;
    //}

    //[StructLayout(LayoutKind.Sequential)]
    //public struct RECT
    //{
    //    public int left;
    //    public int top;
    //    public int right;
    //    public int bottom;
    //}

    //[DllImport("user32.dll")]
    //static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

    //[DllImport("user32.dll")]
    //static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

    //[DllImport("user32.dll")]
    //static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumDelegate lpfnEnum, IntPtr dwData);

    //[DllImport("user32.dll", SetLastError = true)]
    //static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

    //public delegate bool MonitorEnumDelegate(IntPtr hMonitor, IntPtr hdcMonitor, ref RECT lprcMonitor, IntPtr dwData);

    //protected override void OnSourceInitialized(EventArgs e)
    //{
    //    base.OnSourceInitialized(e);
    //    ((HwndSource)PresentationSource.FromVisual(this)).AddHook(WndProc);
    //}

    //private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    //{
    //    if(msg == WM_GETMINMAXINFO)
    //    {
    //        RECT windowRect;
    //        GetWindowRect(hwnd, out windowRect);

    //        IntPtr currentMonitor = GetMonitorWithMostOverlap(windowRect);

    //        if(currentMonitor != IntPtr.Zero)
    //        {
    //            MONITORINFO monitorInfo = new MONITORINFO();
    //            monitorInfo.cbSize = Marshal.SizeOf(typeof(MONITORINFO));

    //            if(GetMonitorInfo(currentMonitor, ref monitorInfo))
    //            {
    //                RECT workingArea = monitorInfo.rcWork;

    //                MINMAXINFO mmi = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));
    //                mmi.ptMaxSize.x = workingArea.right - workingArea.left;
    //                mmi.ptMaxSize.y = workingArea.bottom - workingArea.top;
    //                mmi.ptMaxPosition.x = workingArea.left;
    //                mmi.ptMaxPosition.y = workingArea.top;

    //                mmi.ptMaxTrackSize.x = mmi.ptMaxSize.x;
    //                mmi.ptMaxTrackSize.y = mmi.ptMaxSize.y;

    //                Marshal.StructureToPtr(mmi, lParam, true);
    //                handled = true;
    //            }
    //        }
    //    }
    //    return IntPtr.Zero;
    //}

    //private IntPtr GetMonitorWithMostOverlap(RECT windowRect)
    //{
    //    IntPtr bestMonitor = IntPtr.Zero;
    //    int maxArea = 0;

    //    EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, (IntPtr hMonitor, IntPtr hdcMonitor, ref RECT monitorRect, IntPtr lParam) =>
    //    {
    //        RECT intersection = IntersectRects(windowRect, monitorRect);
    //        int area = (intersection.right - intersection.left) * (intersection.bottom - intersection.top);

    //        if(area > maxArea)
    //        {
    //            maxArea = area;
    //            bestMonitor = hMonitor;
    //        }

    //        return true;
    //    }, IntPtr.Zero);

    //    return bestMonitor;
    //}

    //private RECT IntersectRects(RECT rect1, RECT rect2)
    //{
    //    RECT intersection = new RECT
    //    {
    //        left = Math.Max(rect1.left, rect2.left),
    //        top = Math.Max(rect1.top, rect2.top),
    //        right = Math.Min(rect1.right, rect2.right),
    //        bottom = Math.Min(rect1.bottom, rect2.bottom)
    //    };

    //    // If the rectangles don't intersect, return an empty rectangle
    //    if(intersection.right < intersection.left || intersection.bottom < intersection.top)
    //    {
    //        intersection = new RECT();
    //    }

    //    return intersection;
    //}
}