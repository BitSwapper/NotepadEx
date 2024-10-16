using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using NotepadEx.Theme;
using NotepadEx.Util;
using NotepadEx.View.UserControls;
using Color = System.Windows.Media.Color;

namespace NotepadEx.Windows;

public partial class ThemeEditorWindow : Window
{
    WindowState prevWindowState;
    WindowResizer resizer;
    string currentThemeName;
    //public Dictionary<string, Color> pathToColors = new();

    public ThemeEditorWindow()
    {
        InitializeComponent();
        resizer = new WindowResizer();
        ThemeEditorTitleBar.Init(this, "Theme Editor", Minimize_Click, null!, Close_Click);
        InitThemeData();
    }

    void InitThemeData()
    {
        //pathToColors.Clear();
        AddColorLine("Color_TextEditorBg", "Main Background");
        AddColorLine("Color_TextEditorFg", "Font Foreground");

        //Application.Current.Resources["Color_TextEditorBg"]
        //Application.Current.Resources["Color_TextEditorFg"] = ColorUtil.GetRandomLinearGradientBrush(180);

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
    }

    private void AddColorLine(string path, string themeName)
    {
        ColorPickerLine line = new();
        line.SetPath(path);
        line.SetText(themeName);
        stackPanelMain.Children.Add(line);
        //pathToColors.Add(path, (Application.Current.Resources[path] as System.Windows.Media.SolidColorBrush).Color);
        line.GridForImg.Background = (Application.Current.Resources[path] as System.Windows.Media.SolidColorBrush);
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

    private void MenuItemNew_Click(object sender, RoutedEventArgs e)
    {

    }

    private void MenuItemSave_Click(object sender, RoutedEventArgs e)
    {
        SaveFile();
    }

    bool SaveFile()
    {
        string fileName = currentThemeName;

        if(string.IsNullOrEmpty(fileName))
        {
            SaveFileDialog saveFileDialog = new();
            saveFileDialog.InitialDirectory = DirectoryUtil.NotepadExThemesPath;
            saveFileDialog.Filter = "Theme Files (*.custom)|*.custom|All Files (*.*)|*.*";
            saveFileDialog.DefaultExt = ".custom";

            if(saveFileDialog.ShowDialog() == true)
                fileName = saveFileDialog.FileName;
            else
                return false;
        }

        var theme = new ColorTheme();
        theme.Color_TextEditorBg = pathToColors["Color_TextEditorBg"];
        theme.Color_TextEditorFg = pathToColors["Color_TextEditorFg"];

        var serializedTheme = theme.ToSerializable();

        File.WriteAllText(fileName, JsonSerializer.Serialize<ColorThemeSerializable>(serializedTheme));
        //UpdateTitleText(fileName);
        currentThemeName = fileName;
        //UpdateModifiedStateOfTitleBar();
        //AddRecentFile(fileName);
        return true;
    }
}
