using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using NotepadEx.Extensions;
using NotepadEx.Properties;
using NotepadEx.Theme;
using NotepadEx.Util;
using NotepadEx.Windows;
using LinearGradientBrush = System.Windows.Media.LinearGradientBrush;
using Point = System.Windows.Point;
namespace NotepadEx;

public partial class MainWindow : Window
{
    string currentFileName = string.Empty;
    bool hasTextChangedSinceSave = false;
    WindowState prevWindowState;
    WindowResizer resizer;

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
        resizer = new();
        MainWindowTitleBar.Init(this, DirectoryUtil.AppName, true, Minimize_Click, Maximize_Click, Exit_Click);
        MainWindowTitleBar.ImageSource = new BitmapImage(new Uri(DirectoryUtil.ImagePath_MainIcon.ToUriPath()));

        txtEditor.TextWrapping = Settings.Default.TextWrapping ? TextWrapping.Wrap : TextWrapping.NoWrap;
        MenuItem_ToggleWrapping.IsChecked = Settings.Default.TextWrapping;

        SetupMainMenuBar(!Settings.Default.MenuBarAutoHide);

        SetupInfoBar();
        ThemeManager.SetupThemes(MenuItem_Theme);
    }

    void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(e.ClickCount == 2)
            resizer.DoWindowMaximizedStateChange(this, prevWindowState);
    }

    void Border_MouseMove(object sender, MouseEventArgs e)
    {
        var position = e.GetPosition(this);
        WindowResizer.ResizeWindow(this, position, UIConstants.ResizeBorderWidth);
    }

    void MenuItemNew_Click(object sender, RoutedEventArgs e)
    {
        if(DocumentUtil.PromptToSaveChanges(hasTextChangedSinceSave, SaveDocument))
        {
            txtEditor.Text = string.Empty;
            currentFileName = string.Empty;
            UpdateTitleText(currentFileName);
            //isTextChanged = false;
            //UpdateStatusBarInfo();
        }
    }

    void MenuItemNewWindow_Click(object sender, RoutedEventArgs e) => AdditionalWindowUtil.TryCreateNewNotepadWindow();

    void MenuItemOpen_Click(object sender, RoutedEventArgs e) => DocumentUtil.OpenDocument(SaveDocument, LoadDocument, hasTextChangedSinceSave, DirectoryUtil.AppName);

    void MenuItemOpenRecent_Click(object sender, RoutedEventArgs e)
    {
        MenuItem menuItem = (MenuItem)sender;
        MenuItem subMenuItem = (MenuItem)e.OriginalSource;
        DocumentUtil.OpenDocument(SaveDocument, LoadDocument, hasTextChangedSinceSave, DirectoryUtil.AppName, (string)subMenuItem.Header);
    }

    void MenuItemMultiOpen_Click(object sender, RoutedEventArgs e) { }

    void MenuItemSave_Click(object sender, RoutedEventArgs e) => SaveDocument();

    void MenuItemSaveAs_Click(object sender, RoutedEventArgs e) => SaveDocumentAs();

    void MenuItemPrint_Click(object sender, RoutedEventArgs e) => DocumentUtil.PrintDocument(txtEditor);

    void MenuItemExit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    void MenuItemFont_Click(object sender, RoutedEventArgs e) { }

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

    void MenuItemFindReplace_Click(object sender, RoutedEventArgs e) { }

    void MenuItemCopy_Click(object sender, RoutedEventArgs e) => txtEditor.Copy();

    void MenuItemCut_Click(object sender, RoutedEventArgs e) => txtEditor.Cut();

    void MenuItemPaste_Click(object sender, RoutedEventArgs e) => txtEditor.Paste();

    void MenuItemUndo_Click(object sender, RoutedEventArgs e) { }

    void MenuItemRedo_Click(object sender, RoutedEventArgs e) { }

    void MenuItemDelete_Click(object sender, RoutedEventArgs e) { }

    void MenuItemSelectAll_Click(object sender, RoutedEventArgs e) { }

    void MenuItemTimeDate_Click(object sender, RoutedEventArgs e) { }

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

    void UpdateTitleText(string fileName) => MainWindowTitleBar.txtTitleBar.Text = fileName == string.Empty ? DirectoryUtil.AppName : $"{DirectoryUtil.AppName}  |  " + Path.GetFileName(fileName);

    void AddRecentFile(string filePath) => RecentFileManager.AddRecentFile(filePath, DropDown_File, SaveSettings);

    void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) => SaveSettings();

    void SaveSettings() => SettingsManager.SaveSettings(txtEditor);

    void txtEditor_MouseMove(object sender, MouseEventArgs e) => ShowMenuBarIfMouseInRange(e);

    void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    void Maximize_Click(object sender, RoutedEventArgs e) => resizer.DoWindowMaximizedStateChange(this, prevWindowState);

    void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    void Window_StateChanged(object sender, EventArgs e)
    {
        if(WindowState != WindowState.Minimized)
            resizer.DoWindowMaximizedStateChange(this, prevWindowState);
        prevWindowState = WindowState;
    }

    void MenuItemTestTheme_Click(object sender, RoutedEventArgs e) => AppResourceUtil<LinearGradientBrush>.TrySetResource(Application.Current, UIConstants.Color_TextEditorFg, ColorUtil.GetRandomLinearGradientBrush(180));

    void MenuItemThemeEditor_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        ThemeEditorWindow themeEditorWindow = new();
        themeEditorWindow.ShowDialog();
    }

    void MenuItemTheme_Click(object sender, RoutedEventArgs e)
    {
        var menuItem = e.OriginalSource as MenuItem;
        var themeName = menuItem.Header.ToString();
        ThemeManager.ApplyTheme(themeName, Application.Current);
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
            Row_MainMenuBar.Height = new(UIConstants.InfoBarSize, GridUnitType.Pixel);
            Row_MainMenuBar.IsEnabled = true;
            MainMenuBar.IsEnabled = true;
        }
        MenuItem_AutoHideMenuBar.IsChecked = Settings.Default.MenuBarAutoHide;
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
            Row_InfoBar.Height = new(UIConstants.InfoBarSize, GridUnitType.Pixel);
            Row_InfoBar.IsEnabled = true;
        }

        MenuItem_ToggleInfoBar.IsChecked = Settings.Default.InfoBarVisible;
    }

    void ShowMenuBarIfMouseInRange(MouseEventArgs e)
    {
        if(Settings.Default.MenuBarAutoHide == false && MainMenuBar.IsEnabled) return;

        Point mousePosition = e.GetPosition(txtEditor);
        if(mousePosition.Y < 2)
        {
            if(!MainMenuBar.IsEnabled)
                SetupMainMenuBar(true);
        }
        else
        {
            if(MainMenuBar.IsEnabled)
            {
                Point menuPosition = e.GetPosition(MainMenuBar);
                if(menuPosition.X < 0 || menuPosition.X > MainMenuBar.ActualWidth || menuPosition.Y < 0 || menuPosition.Y > MainMenuBar.ActualHeight)
                    SetupMainMenuBar(false);
            }
        }
    }
}