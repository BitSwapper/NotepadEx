using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using WpfNotepad2.Properties;
using WpfNotepad2.Util;
using WpfNotepad2.Windows;
using Point = System.Windows.Point;

namespace WpfNotepad2;

public partial class MainWindow : Window
{
    const string appName = "NotepadEx";

    string currentFileName = string.Empty;
    bool hasTextChangedSinceSave = false;
    WindowState prevWindowState;

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
        MainWindowTitleBar.Init(this, Minimize_Click, Maximize_Click, Exit_Click);

        txtEditor.TextWrapping = Settings.Default.TextWrapping ? TextWrapping.Wrap : TextWrapping.NoWrap;
        MenuItem_ToggleWrapping.IsChecked = Settings.Default.TextWrapping;

        SetupMainMenuBar(!Settings.Default.MenuBarAutoHide);
        MenuItem_AutoHideMenuBar.IsChecked = Settings.Default.MenuBarAutoHide;

        SetupInfoBar();
    }

    void txtTitleBar_MouseDown(object sender, MouseButtonEventArgs e)
    {
        //if(e.GetPosition(this).Y > UiLayoutConstants.ResizeBorderWidth)
        //    DragMove();
    }

    void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(e.ClickCount == 2)
            WindowResizer.DoWindowMaximizedStateChange(this, prevWindowState);
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

    void UpdateTitleText(string fileName)  {}// Title = txtTitleBar.Text = fileName == string.Empty ? appName : $"{appName}  |  " + Path.GetFileName(fileName);

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

    void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    void Maximize_Click(object sender, RoutedEventArgs e) => WindowResizer.DoWindowMaximizedStateChange(this, prevWindowState);

    void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    void Window_StateChanged(object sender, EventArgs e)
    {
        if(WindowState != WindowState.Minimized)
            WindowResizer.DoWindowMaximizedStateChange(this, prevWindowState);
        prevWindowState = WindowState;
    }

    void MenuItem_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemTestTheme_Click(object sender, RoutedEventArgs e)
    {
        //Application.Current.Resources["Color_TextEditorBg"] = GetRandomLinearGradientBrush(180);
        Application.Current.Resources["Color_TextEditorFg"] = GetRandomLinearGradientBrush(180);

        //Application.Current.Resources["Color_TitleBarFont"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_TitleBarBg"] = GetRandomLinearGradientBrush(180);
        //Application.Current.Resources["Color_SystemButtons"] = GetRandomColorBrush(180);

        //Application.Current.Resources["Color_BorderColor"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_InfoBarBg"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_InfoBarFg"] = GetRandomColorBrush(180);

        //Application.Current.Resources["Color_MenuItemFg"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_MenuBarBg"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_MenuBg"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_MenuBorder"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_MenuFg"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_MenuSeperator"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_MenuDisabledFg"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_MenuItemSelectedBg"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_MenuItemSelectedBorder"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_MenuItemHighlightBg"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_MenuItemHighlightBorder"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_MenuItemHighlightDisabledBg"] = GetRandomColorBrush(180);
        //Application.Current.Resources["Color_MenuItemHighlightDisabledBorder"] = GetRandomColorBrush(180);

        SolidColorBrush GetRandomColorBrush(byte minAlpha = 0, byte maxAlpha = 255) => new SolidColorBrush(System.Windows.Media.Color.FromArgb((byte)Random.Shared.Next(minAlpha, maxAlpha + 1), (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256)));

        LinearGradientBrush GetRandomLinearGradientBrush(byte minAlpha = 0, byte maxAlpha = 255)
        {
            LinearGradientBrush linearGradientBrush = new ();

            linearGradientBrush.StartPoint = new Point(Random.Shared.NextDouble(), Random.Shared.NextDouble());
            linearGradientBrush.EndPoint = new Point(Random.Shared.NextDouble(), Random.Shared.NextDouble());

            int gradientStopCount = Random.Shared.Next(2, 5);
            var gradientStops = new List<GradientStop>();
            for(int i = 0; i < gradientStopCount; i++)
            {
                double offset = i == 0 ? 0.0 : (i == gradientStopCount - 1 ? 1.0 : Random.Shared.NextDouble());

                System.Windows.Media.Color randomColor = System.Windows.Media.Color.FromArgb((byte)Random.Shared.Next(minAlpha, maxAlpha + 1), (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256));

                gradientStops.Add(new GradientStop(randomColor, offset));
            }
        
            foreach(var stop in gradientStops.OrderBy(gs => gs.Offset))    // Sort the gradient stops by offset and add them to the brush
                linearGradientBrush.GradientStops.Add(stop);

            return linearGradientBrush;
        }
    }

    void MenuItemTheme_Click(object sender, RoutedEventArgs e)
    {
        MessageBox.Show("Theme General");
    }

    void MenuItemThemeEditor_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;

        ThemeEditorWindow themeEditorWindow = new();
        themeEditorWindow.ShowDialog();
    }
}