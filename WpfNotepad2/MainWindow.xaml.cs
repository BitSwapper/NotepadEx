using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using WpfNotepad2.Util;

namespace WpfNotepad2;

public partial class MainWindow : Window
{
    const string appName = "NotepadEx";

    string currentFileName = "";
    bool hasTextChangedSinceSave = false;

    public MainWindow()
    {
        InitializeComponent();

        RecentFileManager.LoadRecentFilesFromSettings();
        RecentFileManager.PopulateRecentFilesMenu(DropDown_File);
    }
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

    void MenuItemOpen_Click(object sender, RoutedEventArgs e) => DocumentHelper.OpenDocument(SaveDocument, LoadDocument, hasTextChangedSinceSave, appName);

    void MenuItemOpenRecent_Click(object sender, RoutedEventArgs e)
    {
        MenuItem menuItem = (MenuItem)sender;
        MenuItem subMenuItem = (MenuItem)e.OriginalSource;
        DocumentHelper.OpenDocument(SaveDocument, LoadDocument, hasTextChangedSinceSave, appName, (string)subMenuItem.Header);
    }

    void MenuItemMultiOpen_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemSave_Click(object sender, RoutedEventArgs e) => SaveDocument();

    void MenuItemSaveAs_Click(object sender, RoutedEventArgs e) => SaveDocumentAs();

    void MenuItemPrint_Click(object sender, RoutedEventArgs e) => DocumentHelper.PrintDocument(txtEditor);

    void MenuItemExit_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemFont_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemWordWrap_Click(object sender, RoutedEventArgs e)
    {
        var menuItem = sender as MenuItem;
        menuItem.IsChecked = !menuItem.IsChecked;
    }

    void MenuItemAutohideMenuBar_Click(object sender, RoutedEventArgs e)
    {
        var menuItem = sender as MenuItem;
        menuItem.IsChecked = !menuItem.IsChecked;
    }

    void MenuItemToggleInfoBar_Click(object sender, RoutedEventArgs e)
    {
        var menuItem = sender as MenuItem;
        menuItem.IsChecked = !menuItem.IsChecked;
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

    void UpdateTitleText(string fileName) => Title = $"{appName} | " + Path.GetFileName(fileName);

    void AddRecentFile(string filePath) => RecentFileManager.AddRecentFile(filePath, DropDown_File, SettingsManager.SaveSettings);

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        SettingsManager.SaveSettings();
    }
}