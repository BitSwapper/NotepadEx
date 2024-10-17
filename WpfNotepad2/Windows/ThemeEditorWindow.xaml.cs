using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Win32;
using NotepadEx.Theme;
using NotepadEx.Util;
using NotepadEx.View.UserControls;
using Color = System.Windows.Media.Color;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
namespace NotepadEx.Windows;

public partial class ThemeEditorWindow : Window
{
    WindowState prevWindowState;
    WindowResizer resizer;
    string currentThemeName;
    int lineCt = 0;

    SolidColorBrush brushWhite = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
    SolidColorBrush brushGray = new SolidColorBrush(Color.FromArgb(255, 188, 188, 188));

    List<string> keysMain = new()
            {
                UIConstants.Color_TextEditorBg,
                UIConstants.Color_TextEditorFg,
                UIConstants.Color_TitleBarBg,
                UIConstants.Color_TitleBarFont,
                UIConstants.Color_SystemButtons,
                UIConstants.Color_BorderColor,
            };


    List<string> keysMenuBar = new()
            {
                UIConstants.Color_MenuBarBg,
                UIConstants.Color_MenuItemFg,
            };


    List<string> keysInfoBar = new()
            {
                UIConstants.Color_InfoBarBg,
                UIConstants.Color_InfoBarFg,
            };

    List<string> keysMenuItem = new()
            {
                UIConstants.Color_MenuBorder,
                UIConstants.Color_MenuBg,
                UIConstants.Color_MenuFg,
                UIConstants.Color_MenuSeperator,
                UIConstants.Color_MenuDisabledFg,
                UIConstants.Color_MenuItemSelectedBg,
                UIConstants.Color_MenuItemSelectedBorder,
                UIConstants.Color_MenuItemHighlightBg,
                UIConstants.Color_MenuItemHighlightBorder,
            };


    public ThemeEditorWindow()
    {
        InitializeComponent();
        resizer = new WindowResizer();
        ThemeEditorTitleBar.Init(this, "Theme Editor", Minimize_Click, null!, Close_Click);
        InitThemeData();
    }

    void InitThemeData()
    {
        AddNewColorLineSafe(UIConstants.Color_TextEditorBg, "Text Editor Background", ref ThemeManager.CurrentTheme.themeObj_TextEditorBg!);
        AddNewColorLineSafe(UIConstants.Color_TextEditorFg, "Text Editor Font", ref ThemeManager.CurrentTheme.themeObj_TextEditorFg!);

        AddNewColorLineSafe(UIConstants.Color_TitleBarBg, "Title Bar Background", ref ThemeManager.CurrentTheme.themeObj_TitleBarBg!);
        AddNewColorLineSafe(UIConstants.Color_TitleBarFont, "Title Bar Font", ref ThemeManager.CurrentTheme.themeObj_TitleBarFont!);

        AddNewColorLineSafe(UIConstants.Color_SystemButtons, "System Buttons", ref ThemeManager.CurrentTheme.themeObj_SystemButtons!);
        AddNewColorLineSafe(UIConstants.Color_BorderColor, "Border Color", ref ThemeManager.CurrentTheme.themeObj_BorderColor!);

        AddNewColorLineSafe(UIConstants.Color_MenuBarBg, "Menu Bar Background", ref ThemeManager.CurrentTheme.themeObj_MenuBarBg!);
        AddNewColorLineSafe(UIConstants.Color_MenuItemFg, "Menu Item Font", ref ThemeManager.CurrentTheme.themeObj_MenuItemFg!);

        AddNewColorLineSafe(UIConstants.Color_InfoBarBg, "Info Bar Background", ref ThemeManager.CurrentTheme.themeObj_InfoBarBg!);
        AddNewColorLineSafe(UIConstants.Color_InfoBarFg, "Info Bar Font", ref ThemeManager.CurrentTheme.themeObj_InfoBarFg!);


        //AddNewColorLineSafe("Color_MenuBgColor_MenuBg", "Menu BG", ref ThemeManager.CurrentTheme.themeObj_MenuBgThemeObj_MenuBg!);
        AddNewColorLineSafe(UIConstants.Color_MenuBg, "Menu Background", ref ThemeManager.CurrentTheme.themeObj_MenuBg!);
        AddNewColorLineSafe(UIConstants.Color_MenuBorder, "Menu Border", ref ThemeManager.CurrentTheme.themeObj_MenuBorder!);
        AddNewColorLineSafe(UIConstants.Color_MenuItemHighlightBg, "Menu Item Highlight Background", ref ThemeManager.CurrentTheme.themeObj_MenuItemHighlightBg!);
        AddNewColorLineSafe(UIConstants.Color_MenuItemHighlightBorder, "Selected Menu Item Border", ref ThemeManager.CurrentTheme.themeObj_MenuItemHighlightBorder!);
        AddNewColorLineSafe(UIConstants.Color_MenuSeperator, "Menu Seperator", ref ThemeManager.CurrentTheme.themeObj_MenuSeperator!);
        AddNewColorLineSafe(UIConstants.Color_MenuDisabledFg, "Menu Disabled Font", ref ThemeManager.CurrentTheme.themeObj_MenuDisabledFg!);

        AddNewColorLineSafe(UIConstants.Color_MenuItemSelectedBg, "Checkbox Background", ref ThemeManager.CurrentTheme.themeObj_MenuItemSelectedBg!);
        AddNewColorLineSafe(UIConstants.Color_MenuItemSelectedBorder, "Checkbox Border", ref ThemeManager.CurrentTheme.themeObj_MenuItemSelectedBorder!);
        AddNewColorLineSafe(UIConstants.Color_MenuFg, "Checkmark / Arrow", ref ThemeManager.CurrentTheme.themeObj_MenuFg!);

        //AddNewColorLineSafe("Color_MenuItemHighlightDisabledBg", "Menu Item Highlight Disabled Bg", ref ThemeManager.CurrentTheme.themeObj_MenuItemHighlightDisabledBg!);
        //AddNewColorLineSafe("Color_MenuItemHighlightDisabledBorder", "Menu Item Highlight Disabled Border", ref ThemeManager.CurrentTheme.themeObj_MenuItemHighlightDisabledBorder!);

    }

    void AddNewColorLineSafe(string resourceKey, string friendlyThemeName, ref ThemeObject themeObj)
    {
        if(themeObj == null)
            themeObj = new(AppResourceUtil<SolidColorBrush>.TryGetResource(Application.Current, resourceKey).Color);
        AddColorLine(resourceKey, friendlyThemeName, themeObj ?? new());

        void AddColorLine(string resourceKey, string friendlyThemeName, ThemeObject themeObj)
        {
            ColorPickerLine line = new();
            line.SetupThemeObj(themeObj, resourceKey, friendlyThemeName);

            line.ViewModel.BackgroundColor = ++lineCt % 2 == 0 ? brushWhite : brushGray;

            if(keysMain.Contains(resourceKey))
                StackMain.Children.Add(line);

            else if(keysMenuBar.Contains(resourceKey))
                StackMenuBar.Children.Add(line);

            else if(keysInfoBar.Contains(resourceKey))
                StackInfoBar.Children.Add(line);

            else if(keysMenuItem.Contains(resourceKey))
                StackMenuItem.Children.Add(line);
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
        WindowResizer.ResizeWindow(this, position, UIConstants.ResizeBorderWidth);
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
