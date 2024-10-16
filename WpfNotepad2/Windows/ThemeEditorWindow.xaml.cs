using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using NotepadEx.Theme;
using NotepadEx.Util;
using NotepadEx.View.UserControls;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
namespace NotepadEx.Windows;

public partial class ThemeEditorWindow : Window
{
    WindowState prevWindowState;
    WindowResizer resizer;
    string currentThemeName;

    public ThemeEditorWindow()
    {
        InitializeComponent();
        resizer = new WindowResizer();
        ThemeEditorTitleBar.Init(this, "Theme Editor", Minimize_Click, null!, Close_Click);
        InitThemeData();
    }

    void InitThemeData()
    {
        AddNewColorLineSafe("Color_TextEditorBg", "Text Editor Background", ref ThemeManager.CurrentTheme.themeObj_TextEditorBg!);
        AddNewColorLineSafe("Color_TextEditorFg", "Text Editor Font", ref ThemeManager.CurrentTheme.themeObj_TextEditorFg!);

        AddNewColorLineSafe("Color_TitleBarBg", "Title Bar Background", ref ThemeManager.CurrentTheme.themeObj_TitleBarBg!);
        AddNewColorLineSafe("Color_TitleBarFont", "Title Bar Font", ref ThemeManager.CurrentTheme.themeObj_TitleBarFont!);

        AddNewColorLineSafe("Color_SystemButtons", "System Buttons", ref ThemeManager.CurrentTheme.themeObj_SystemButtons!);
        AddNewColorLineSafe("Color_BorderColor", "Border Color", ref ThemeManager.CurrentTheme.themeObj_BorderColor!);

        AddNewColorLineSafe("Color_MenuBarBg", "Menu Bar Background", ref ThemeManager.CurrentTheme.themeObj_MenuBarBg!);
        AddNewColorLineSafe("Color_MenuItemFg", "Menu Item Font", ref ThemeManager.CurrentTheme.themeObj_MenuItemFg!);

        AddNewColorLineSafe("Color_InfoBarBg", "Info Bar Background", ref ThemeManager.CurrentTheme.themeObj_InfoBarBg!);
        AddNewColorLineSafe("Color_InfoBarFg", "Info Bar Font", ref ThemeManager.CurrentTheme.themeObj_InfoBarFg!);


        //AddNewColorLineSafe("Color_MenuBgColor_MenuBg", "Menu BG", ref ThemeManager.CurrentTheme.themeObj_MenuBgThemeObj_MenuBg!);
        AddNewColorLineSafe("Color_MenuBorder", "Menu Border", ref ThemeManager.CurrentTheme.themeObj_MenuBorder!);
        AddNewColorLineSafe("Color_MenuBg", "Menu Bg2", ref ThemeManager.CurrentTheme.themeObj_MenuBg!);
        AddNewColorLineSafe("Color_MenuItemHighlightBg", "Menu Item Highlight Bg", ref ThemeManager.CurrentTheme.themeObj_MenuItemHighlightBg!);
        AddNewColorLineSafe("Color_MenuItemHighlightBorder", "Selected Menu Item Border", ref ThemeManager.CurrentTheme.themeObj_MenuItemHighlightBorder!);
        AddNewColorLineSafe("Color_MenuSeperator", "Menu Seperator", ref ThemeManager.CurrentTheme.themeObj_MenuSeperator!);
        AddNewColorLineSafe("Color_MenuDisabledFg", "Menu Disabled Font", ref ThemeManager.CurrentTheme.themeObj_MenuDisabledFg!);

        AddNewColorLineSafe("Color_MenuItemSelectedBg", "Checkbox Bg", ref ThemeManager.CurrentTheme.themeObj_MenuItemSelectedBg!);
        AddNewColorLineSafe("Color_MenuItemSelectedBorder", "Checkbox Border", ref ThemeManager.CurrentTheme.themeObj_MenuItemSelectedBorder!);
        AddNewColorLineSafe("Color_MenuFg", "Checkmark / Arrow", ref ThemeManager.CurrentTheme.themeObj_MenuFg!);

        //AddNewColorLineSafe("Color_MenuItemHighlightDisabledBg", "Menu Item Highlight Disabled Bg", ref ThemeManager.CurrentTheme.themeObj_MenuItemHighlightDisabledBg!);
        //AddNewColorLineSafe("Color_MenuItemHighlightDisabledBorder", "Menu Item Highlight Disabled Border", ref ThemeManager.CurrentTheme.themeObj_MenuItemHighlightDisabledBorder!);

    }

    void AddNewColorLineSafe(string resourceKey, string friendlyThemeName, ref ThemeObject themeObj)
    {
        if(themeObj == null)
            themeObj = new(AppResourceUtil<SolidColorBrush>.TryGetResource(Application.Current, resourceKey).Color);
        AddColorLine(resourceKey, friendlyThemeName, themeObj ?? new());

        void AddColorLine(string themePath, string friendlyThemeName, ThemeObject themeObj)
        {
            ColorPickerLine line = new();
            line.SetupThemeObj(themeObj, themePath, friendlyThemeName);
            stackPanelMain.Children.Add(line);
        }
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

    void MenuItemNew_Click(object sender, RoutedEventArgs e)
    {

    }

    void MenuItemSave_Click(object sender, RoutedEventArgs e) => SaveFile();

    bool SaveFile()
    {
        string fileName;

        SaveFileDialog saveFileDialog = new();
        saveFileDialog.InitialDirectory = DirectoryUtil.NotepadExThemesPath;
        saveFileDialog.Filter = "Theme Files (*.custom)|*.custom|All Files (*.*)|*.*";
        saveFileDialog.DefaultExt = ".custom";

        if(saveFileDialog.ShowDialog() == true)
            fileName = saveFileDialog.FileName;
        else
            return false;

        var theme = ThemeManager.CurrentTheme;
        var serializedTheme = theme.ToSerializable();

        var options = new JsonSerializerOptions
        {
            WriteIndented = true, // Optional: Makes JSON human-readable
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Ensures consistency with camelCase
        };

        File.WriteAllText(fileName, JsonSerializer.Serialize<ColorThemeSerializable>(serializedTheme, options));
        //UpdateTitleText(fileName);
        currentThemeName = fileName;
        //UpdateModifiedStateOfTitleBar();
        //AddRecentFile(fileName);
        return true;
    }
}


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