using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using NotepadEx.Services;
using NotepadEx.Services.Interfaces;
using NotepadEx.Theme;
using NotepadEx.Util;
using Color = System.Windows.Media.Color;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;

namespace NotepadEx.MVVM.View;

public partial class ThemeEditorWindow : Window
{
    readonly IThemeService themeService;
    readonly IWindowService windowService;
    CustomTitleBarViewModel titleBarViewModel;
    public CustomTitleBarViewModel TitleBarViewModel => titleBarViewModel;

    int colorLineCt = 0;

    SolidColorBrush brushA = new SolidColorBrush(Color.FromArgb(255, 64, 64, 64));
    SolidColorBrush brushB = new SolidColorBrush(Color.FromArgb(255, 44, 44, 44));

    public ThemeEditorWindow(IThemeService themeService)
    {
        this.themeService = themeService;
        windowService = new WindowService(this);
        InitializeComponent();
        DataContext = this;
        CustomTitleBar.InitializeTitleBar(ref titleBarViewModel, this, "Theme Editor", showMinimize: false, showMaximize: false);
        this.themeService.LoadCurrentTheme();
        AddEditableColorLinesToWindow();
    }

    void AddEditableColorLinesToWindow()
    {
        AddNewColorLineSafe(UIConstants.Color_TextEditorBg, "Text Editor Background", ref themeService.CurrentTheme.themeObj_TextEditorBg!);
        AddNewColorLineSafe(UIConstants.Color_TextEditorFg, "Text Editor Font", ref themeService.CurrentTheme.themeObj_TextEditorFg!);
        AddNewColorLineSafe(UIConstants.Color_TextEditorCaret, "Text Editor Caret", ref themeService.CurrentTheme.themeObj_TextEditorCaret!);
        AddNewColorLineSafe(UIConstants.Color_TextEditorScrollBar, "Text Editor ScrollBar", ref themeService.CurrentTheme.themeObj_TextEditorScrollBar!);
        AddNewColorLineSafe(UIConstants.Color_TextEditorTextHighlight, "Highlighted Text", ref themeService.CurrentTheme.themeObj_TextEditorTextHighlight!);
        AddNewColorLineSafe(UIConstants.Color_TitleBarBg, "Title Bar Background", ref themeService.CurrentTheme.themeObj_TitleBarBg!);
        AddNewColorLineSafe(UIConstants.Color_TitleBarFont, "Title Bar Font", ref themeService.CurrentTheme.themeObj_TitleBarFont!);
        AddNewColorLineSafe(UIConstants.Color_SystemButtons, "System Buttons", ref themeService.CurrentTheme.themeObj_SystemButtons!);
        AddNewColorLineSafe(UIConstants.Color_BorderColor, "Border Color", ref themeService.CurrentTheme.themeObj_BorderColor!);

        AddNewColorLineSafe(UIConstants.Color_MenuBarBg, "Menu Bar Background", ref themeService.CurrentTheme.themeObj_MenuBarBg!);
        AddNewColorLineSafe(UIConstants.Color_MenuItemFg, "Menu Item Font", ref themeService.CurrentTheme.themeObj_MenuItemFg!);
        AddNewColorLineSafe(UIConstants.Color_InfoBarBg, "Info Bar Background", ref themeService.CurrentTheme.themeObj_InfoBarBg!);
        AddNewColorLineSafe(UIConstants.Color_InfoBarFg, "Info Bar Font", ref themeService.CurrentTheme.themeObj_InfoBarFg!);


        AddNewColorLineSafe(UIConstants.Color_MenuBg, "Menu Background", ref themeService.CurrentTheme.themeObj_MenuBg!);
        AddNewColorLineSafe(UIConstants.Color_MenuBorder, "Menu Border", ref themeService.CurrentTheme.themeObj_MenuBorder!);
        AddNewColorLineSafe(UIConstants.Color_MenuItemHighlightBg, "Menu Item Highlight Background", ref themeService.CurrentTheme.themeObj_MenuItemHighlightBg!);
        AddNewColorLineSafe(UIConstants.Color_MenuItemHighlightBorder, "Selected Menu Item Border", ref themeService.CurrentTheme.themeObj_MenuItemHighlightBorder!);
        AddNewColorLineSafe(UIConstants.Color_MenuSeperator, "Menu Seperator", ref themeService.CurrentTheme.themeObj_MenuSeperator!);
        AddNewColorLineSafe(UIConstants.Color_MenuDisabledFg, "Menu Disabled Font", ref themeService.CurrentTheme.themeObj_MenuDisabledFg!);
        AddNewColorLineSafe(UIConstants.Color_MenuItemSelectedBg, "Checkbox Background", ref themeService.CurrentTheme.themeObj_MenuItemSelectedBg!);
        AddNewColorLineSafe(UIConstants.Color_MenuItemSelectedBorder, "Checkbox Border", ref themeService.CurrentTheme.themeObj_MenuItemSelectedBorder!);
        AddNewColorLineSafe(UIConstants.Color_MenuFg, "Checkmark / Arrow", ref themeService.CurrentTheme.themeObj_MenuFg!);

        AddNewColorLineSafe(UIConstants.Color_ToolWindowBg, "Tool Window Background", ref themeService.CurrentTheme.themeObj_ToolWindowBg!);
        AddNewColorLineSafe(UIConstants.Color_ToolWindowFont, "Tool Window Font", ref themeService.CurrentTheme.themeObj_ToolWindowFont!);
        AddNewColorLineSafe(UIConstants.Color_ToolWindowButtonBg, "Tool Window Buttons", ref themeService.CurrentTheme.themeObj_ToolWindowButtonBg!);
        AddNewColorLineSafe(UIConstants.Color_ToolWindowButtonBorder, "Tool Window Button Border", ref themeService.CurrentTheme.themeObj_ToolWindowButtonBorder!);
    }

    void AddNewColorLineSafe(string resourceKey, string friendlyThemeName, ref ThemeObject themeObj)
    {
        if(themeObj == null)
        {
            var brush = AppResourceUtil<SolidColorBrush>.TryGetResource(Application.Current, resourceKey);
            if(brush == null)
                brush = new SolidColorBrush();
            themeObj = new(brush.Color);
        }
        AddColorLine(resourceKey, friendlyThemeName, themeObj ?? new());

        void AddColorLine(string resourceKey, string friendlyThemeName, ThemeObject themeObj)
        {
            ColorPickerLine line = new();
            line.ViewModel.SetupThemeObj(themeObj, resourceKey, friendlyThemeName);

            line.ViewModel.BackgroundColor = ++colorLineCt % 2 == 0 ? brushA : brushB;

            if(UIConstants.UIColorKeysMain.Contains(resourceKey))
                StackPanelMain.Children.Add(line);

            else if(UIConstants.UIColorKeysMenuBar.Contains(resourceKey))
                StackPanelMenuBar.Children.Add(line);

            else if(UIConstants.UIColorKeysMenuItem.Contains(resourceKey))
                StackPanelMenuItem.Children.Add(line);

            else if(UIConstants.UIColorKeysInfoBar.Contains(resourceKey))
                StackPanelInfoBar.Children.Add(line);

            else if(UIConstants.UIColorKeysToolWindow.Contains(resourceKey))
                StackPanelToolWindow.Children.Add(line);
        }
    }

    void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

    void Close_Click(object sender, RoutedEventArgs e) => Close();

    void MenuItemSave_Click(object sender, RoutedEventArgs e) => SaveThemeFile();

    bool SaveThemeFile()
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

        var theme = themeService.CurrentTheme;
        var serializedTheme = theme.ToSerializable();

        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        File.WriteAllText(fileName, JsonSerializer.Serialize<ColorThemeSerializable>(serializedTheme, options));


        //UpdateTitleText(fileName);
        //UpdateModifiedStateOfTitleBar();
        //AddRecentFile(fileName);
        return true;
    }

    void OnWindowMouseMove(object sender, MouseEventArgs e)
    {
        if(WindowState == WindowState.Normal)
        {
            var position = e.GetPosition(this);
            WindowResizerUtil.ResizeWindow(this, position);
        }
    }
}
