using System.IO;
using System.Text.Json;
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
        MainWindowTitleBar.Init(this, Constants.AppName, Minimize_Click, Maximize_Click, Exit_Click);
        string imagePath = Constants.ImagePath_MainIcon.ToUriPath();
        var v = new BitmapImage(new Uri(imagePath));
        MainWindowTitleBar.ImageSource = v;

        txtEditor.TextWrapping = Settings.Default.TextWrapping ? TextWrapping.Wrap : TextWrapping.NoWrap;
        MenuItem_ToggleWrapping.IsChecked = Settings.Default.TextWrapping;

        SetupMainMenuBar(!Settings.Default.MenuBarAutoHide);
        MenuItem_AutoHideMenuBar.IsChecked = Settings.Default.MenuBarAutoHide;

        SetupInfoBar();
        SetupThemes();
    }

    void SetupThemes()
    {
        MenuItem themeMenuItem = MenuItem_Theme;
        //themeMenuItem.Items.Clear();

        AddCustomThemes(themeMenuItem);
    }

    void AddCustomThemes(MenuItem themeMenuItem)
    {
        var customThemes = Directory.GetFiles(DirectoryUtil.NotepadExThemesPath);
        foreach(var customTheme in customThemes)
            AddThemeMenuItem(themeMenuItem, Path.GetFileName(customTheme));

        void AddThemeMenuItem(MenuItem menuItem, string header)
        {
            MenuItem item = new MenuItem { Header = header };
            item.Click += ThemeItem_Click;
            menuItem.Items.Add(item);
        }
    }

    void ThemeItem_Click(object sender, RoutedEventArgs e) { }

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

    void MenuItemNewWindow_Click(object sender, RoutedEventArgs e) => AdditionalWindowManager.TryCreateNewNotepadWindow();

    void MenuItemOpen_Click(object sender, RoutedEventArgs e) => DocumentUtil.OpenDocument(SaveDocument, LoadDocument, hasTextChangedSinceSave, Constants.AppName);

    void MenuItemOpenRecent_Click(object sender, RoutedEventArgs e)
    {
        MenuItem menuItem = (MenuItem)sender;
        MenuItem subMenuItem = (MenuItem)e.OriginalSource;
        DocumentUtil.OpenDocument(SaveDocument, LoadDocument, hasTextChangedSinceSave, Constants.AppName, (string)subMenuItem.Header);
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

    void UpdateTitleText(string fileName) => MainWindowTitleBar.txtTitleBar.Text = fileName == string.Empty ? Constants.AppName : $"{Constants.AppName}  |  " + Path.GetFileName(fileName);

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
            Row_MainMenuBar.Height = new(Constants.InfoBarSize, GridUnitType.Pixel);
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
            Row_InfoBar.Height = new(Constants.InfoBarSize, GridUnitType.Pixel);
            Row_InfoBar.IsEnabled = true;
        }

        MenuItem_ToggleInfoBar.IsChecked = Settings.Default.InfoBarVisible;
    }

    void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    void Maximize_Click(object sender, RoutedEventArgs e) => resizer.DoWindowMaximizedStateChange(this, prevWindowState);

    void Exit_Click(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

    void Window_StateChanged(object sender, EventArgs e)
    {
        if(WindowState != WindowState.Minimized)
            resizer.DoWindowMaximizedStateChange(this, prevWindowState);
        prevWindowState = WindowState;
    }

    void MenuItem_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemTestTheme_Click(object sender, RoutedEventArgs e) =>
        //Application.Current.Resources["Color_TextEditorBg"] = GetRandomLinearGradientBrush(180);
        Application.Current.Resources["Color_TextEditorFg"] = ColorUtil.GetRandomLinearGradientBrush(180);//Application.Current.Resources["Color_TitleBarFont"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_TitleBarBg"] = GetRandomLinearGradientBrush(180);//Application.Current.Resources["Color_SystemButtons"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_BorderColor"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_InfoBarBg"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_InfoBarFg"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_MenuItemFg"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_MenuBarBg"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_MenuBg"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_MenuBorder"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_MenuFg"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_MenuSeperator"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_MenuDisabledFg"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_MenuItemSelectedBg"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_MenuItemSelectedBorder"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_MenuItemHighlightBg"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_MenuItemHighlightBorder"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_MenuItemHighlightDisabledBg"] = GetRandomColorBrush(180);//Application.Current.Resources["Color_MenuItemHighlightDisabledBorder"] = GetRandomColorBrush(180);

    void MenuItemTheme_Click(object sender, RoutedEventArgs e)
    {
        var menuItem = e.OriginalSource as MenuItem;
        var themeName = menuItem.Header.ToString();

        var fileData = File.ReadAllText(DirectoryUtil.NotepadExThemesPath + themeName);
        var themeSerialized = JsonSerializer.Deserialize<ColorThemeSerializable>(fileData);
        var theme = themeSerialized.ToColorTheme();
        ApplyTheme(theme);
    }

    void ApplyTheme(ColorTheme theme)
    {
        Application.Current.Resources["Color_TextEditorBg"] = new System.Windows.Media.SolidColorBrush(theme.Color_TextEditorBg);
        Application.Current.Resources["Color_TextEditorFg"] = new System.Windows.Media.SolidColorBrush(theme.Color_TextEditorFg);
    }

    void MenuItemThemeEditor_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        ThemeEditorWindow themeEditorWindow = new();
        themeEditorWindow.ShowDialog();
    }

    void MenuItemColorEditor_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        ColorPickerWindow colorPickerWindow = new();
        colorPickerWindow.ShowDialog();
        MessageBox.Show(colorPickerWindow.SelectedColor.ToString());
    }
}