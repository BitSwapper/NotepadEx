using System.Text.Json.Serialization;

namespace NotepadEx.Theme;

public class ColorTheme
{
    public ThemeObject? themeObj_TextEditorBg;
    public ThemeObject? themeObj_TextEditorFg;
    public ThemeObject? themeObj_TitleBarBg;
    public ThemeObject? themeObj_TitleBarFont;

    public ThemeObject? themeObj_SystemButtons;
    public ThemeObject? themeObj_BorderColor;
    public ThemeObject? themeObj_MenuBarBg;
    public ThemeObject? themeObj_MenuItemFg;
    public ThemeObject? themeObj_InfoBarBg;
    public ThemeObject? themeObj_InfoBarFg;

    public ThemeObject? themeObj_MenuBgThemeObj_MenuBg;
    public ThemeObject? themeObj_MenuBorder;
    public ThemeObject? themeObj_MenuBg;
    public ThemeObject? themeObj_MenuFg;
    public ThemeObject? themeObj_MenuSeperator;
    public ThemeObject? themeObj_MenuDisabledFg;
    public ThemeObject? themeObj_MenuItemSelectedBg;
    public ThemeObject? themeObj_MenuItemSelectedBorder;
    public ThemeObject? themeObj_MenuItemHighlightBg;
    public ThemeObject? themeObj_MenuItemHighlightBorder;

    public ThemeObject? themeObj_ToolWindowBg;
    public ThemeObject? themeObj_ToolWindowFont;
    public ThemeObject? themeObj_ToolWindowButtonBg;
    public ThemeObject? themeObj_ToolWindowButtonBorder;

    public ColorThemeSerializable ToSerializable() => new ColorThemeSerializable(this);
}

[Serializable]
public class ColorThemeSerializable
{
    [JsonPropertyName("themeObj_TextEditorBg")] public ThemeObjectSerializable ThemeObj_TextEditorBg { get; set; }
    [JsonPropertyName("themeObj_TextEditorFg")] public ThemeObjectSerializable ThemeObj_TextEditorFg { get; set; }
    [JsonPropertyName("themeObj_TitleBarBg")] public ThemeObjectSerializable ThemeObj_TitleBarBg { get; set; }
    [JsonPropertyName("themeObj_TitleBarFont")] public ThemeObjectSerializable ThemeObj_TitleBarFont { get; set; }
    [JsonPropertyName("themeObj_SystemButtons")] public ThemeObjectSerializable ThemeObj_SystemButtons { get; set; }
    [JsonPropertyName("themeObj_BorderColor")] public ThemeObjectSerializable ThemeObj_BorderColor { get; set; }

    [JsonPropertyName("themeObj_MenuBarBg")] public ThemeObjectSerializable ThemeObj_MenuBarBg { get; set; }
    [JsonPropertyName("themeObj_MenuItemFg")] public ThemeObjectSerializable ThemeObj_MenuItemFg { get; set; }
    [JsonPropertyName("themeObj_InfoBarBg")] public ThemeObjectSerializable ThemeObj_InfoBarBg { get; set; }
    [JsonPropertyName("themeObj_InfoBarFg")] public ThemeObjectSerializable ThemeObj_InfoBarFg { get; set; }

    [JsonPropertyName("themeObj_MenuBorder")] public ThemeObjectSerializable ThemeObj_MenuBorder { get; set; }
    [JsonPropertyName("themeObj_MenuBg")] public ThemeObjectSerializable ThemeObj_MenuBg { get; set; }
    [JsonPropertyName("themeObj_MenuFg")] public ThemeObjectSerializable ThemeObj_MenuFg { get; set; }
    [JsonPropertyName("themeObj_MenuSeperator")] public ThemeObjectSerializable ThemeObj_MenuSeperator { get; set; }
    [JsonPropertyName("themeObj_MenuDisabledFg")] public ThemeObjectSerializable ThemeObj_MenuDisabledFg { get; set; }
    [JsonPropertyName("themeObj_MenuItemSelectedBg")] public ThemeObjectSerializable ThemeObj_MenuItemSelectedBg { get; set; }
    [JsonPropertyName("themeObj_MenuItemSelectedBorder")] public ThemeObjectSerializable ThemeObj_MenuItemSelectedBorder { get; set; }
    [JsonPropertyName("themeObj_MenuItemHighlightBg")] public ThemeObjectSerializable ThemeObj_MenuItemHighlightBg { get; set; }
    [JsonPropertyName("themeObj_MenuItemHighlightBorder")] public ThemeObjectSerializable ThemeObj_MenuItemHighlightBorder { get; set; }
    
    [JsonPropertyName("themeObj_ToolWindowBg;")] public ThemeObjectSerializable ThemeObj_ToolWindowBg { get; set; }
    [JsonPropertyName("themeObj_ToolWindowFont;")] public ThemeObjectSerializable ThemeObj_ToolWindowFont { get; set; }
    [JsonPropertyName("themeObj_ToolWindowButtonBg;")] public ThemeObjectSerializable ThemeObj_ToolWindowButtonBg { get; set; }
    [JsonPropertyName("themeObj_ToolWindowButtonBorder;")] public ThemeObjectSerializable ThemeObj_ToolWindowButtonBorder { get; set; }

    public ColorThemeSerializable() { } //needs empty constructor

    public ColorThemeSerializable(ColorTheme colorTheme)
    {
        if(colorTheme.themeObj_TextEditorBg != null) ThemeObj_TextEditorBg = new ThemeObjectSerializable(colorTheme.themeObj_TextEditorBg);
        if(colorTheme.themeObj_TextEditorFg != null) ThemeObj_TextEditorFg = new ThemeObjectSerializable(colorTheme.themeObj_TextEditorFg);
        if(colorTheme.themeObj_TitleBarBg != null) ThemeObj_TitleBarBg = new ThemeObjectSerializable(colorTheme.themeObj_TitleBarBg);
        if(colorTheme.themeObj_TitleBarFont != null) ThemeObj_TitleBarFont = new ThemeObjectSerializable(colorTheme.themeObj_TitleBarFont);
        if(colorTheme.themeObj_SystemButtons != null) ThemeObj_SystemButtons = new ThemeObjectSerializable(colorTheme.themeObj_SystemButtons);
        if(colorTheme.themeObj_BorderColor != null) ThemeObj_BorderColor = new ThemeObjectSerializable(colorTheme.themeObj_BorderColor);

        if(colorTheme.themeObj_MenuBarBg != null) ThemeObj_MenuBarBg = new ThemeObjectSerializable(colorTheme.themeObj_MenuBarBg);
        if(colorTheme.themeObj_MenuItemFg != null) ThemeObj_MenuItemFg = new ThemeObjectSerializable(colorTheme.themeObj_MenuItemFg);
        if(colorTheme.themeObj_InfoBarBg != null) ThemeObj_InfoBarBg = new ThemeObjectSerializable(colorTheme.themeObj_InfoBarBg);
        if(colorTheme.themeObj_InfoBarFg != null) ThemeObj_InfoBarFg = new ThemeObjectSerializable(colorTheme.themeObj_InfoBarFg);

        if(colorTheme.themeObj_MenuBorder != null) ThemeObj_MenuBorder = new ThemeObjectSerializable(colorTheme.themeObj_MenuBorder);
        if(colorTheme.themeObj_MenuBg != null) ThemeObj_MenuBg = new ThemeObjectSerializable(colorTheme.themeObj_MenuBg);
        if(colorTheme.themeObj_MenuFg != null) ThemeObj_MenuFg = new ThemeObjectSerializable(colorTheme.themeObj_MenuFg);
        if(colorTheme.themeObj_MenuSeperator != null) ThemeObj_MenuSeperator = new ThemeObjectSerializable(colorTheme.themeObj_TextEditorBg);
        if(colorTheme.themeObj_MenuDisabledFg != null) ThemeObj_MenuDisabledFg = new ThemeObjectSerializable(colorTheme.themeObj_MenuDisabledFg);
        if(colorTheme.themeObj_MenuItemSelectedBg != null) ThemeObj_MenuItemSelectedBg = new ThemeObjectSerializable(colorTheme.themeObj_MenuItemSelectedBg);
        if(colorTheme.themeObj_MenuItemSelectedBorder != null) ThemeObj_MenuItemSelectedBorder = new ThemeObjectSerializable(colorTheme.themeObj_MenuItemSelectedBorder);
        if(colorTheme.themeObj_MenuItemHighlightBg != null) ThemeObj_MenuItemHighlightBg = new ThemeObjectSerializable(colorTheme.themeObj_MenuItemHighlightBg);
        if(colorTheme.themeObj_MenuItemHighlightBorder != null) ThemeObj_MenuItemHighlightBorder = new ThemeObjectSerializable(colorTheme.themeObj_MenuItemHighlightBorder);
       
        if(colorTheme.themeObj_ToolWindowBg != null) ThemeObj_ToolWindowBg = new ThemeObjectSerializable(colorTheme.themeObj_ToolWindowBg);
        if(colorTheme.themeObj_ToolWindowFont != null) ThemeObj_ToolWindowFont = new ThemeObjectSerializable(colorTheme.themeObj_ToolWindowFont);
        if(colorTheme.themeObj_ToolWindowButtonBg != null) ThemeObj_ToolWindowButtonBg = new ThemeObjectSerializable(colorTheme.themeObj_ToolWindowButtonBg);
        if(colorTheme.themeObj_ToolWindowButtonBorder != null) ThemeObj_ToolWindowButtonBorder = new ThemeObjectSerializable(colorTheme.themeObj_ToolWindowButtonBorder);
    }

    public ColorTheme ToColorTheme() => new ColorTheme
    {
        themeObj_TextEditorBg = ThemeObj_TextEditorBg?.ToThemeObject(),
        themeObj_TextEditorFg = ThemeObj_TextEditorFg?.ToThemeObject(),

        themeObj_TitleBarBg = ThemeObj_TitleBarBg?.ToThemeObject(),
        themeObj_TitleBarFont = ThemeObj_TitleBarFont?.ToThemeObject(),

        themeObj_SystemButtons = ThemeObj_SystemButtons?.ToThemeObject(),
        themeObj_BorderColor = ThemeObj_BorderColor?.ToThemeObject(),
        themeObj_MenuBarBg = ThemeObj_MenuBarBg?.ToThemeObject(),
        themeObj_MenuItemFg = ThemeObj_MenuItemFg?.ToThemeObject(),
        themeObj_InfoBarBg = ThemeObj_InfoBarBg?.ToThemeObject(),
        themeObj_InfoBarFg = ThemeObj_InfoBarFg?.ToThemeObject(),

        themeObj_MenuBorder = ThemeObj_MenuBorder?.ToThemeObject(),
        themeObj_MenuBg = ThemeObj_MenuBg?.ToThemeObject(),
        themeObj_MenuFg = ThemeObj_MenuFg?.ToThemeObject(),
        themeObj_MenuSeperator = ThemeObj_MenuSeperator?.ToThemeObject(),
        themeObj_MenuDisabledFg = ThemeObj_MenuDisabledFg?.ToThemeObject(),
        themeObj_MenuItemSelectedBg = ThemeObj_MenuItemSelectedBg?.ToThemeObject(),
        themeObj_MenuItemSelectedBorder = ThemeObj_MenuItemSelectedBorder?.ToThemeObject(),
        themeObj_MenuItemHighlightBg = ThemeObj_MenuItemHighlightBg?.ToThemeObject(),
        themeObj_MenuItemHighlightBorder = ThemeObj_MenuItemHighlightBorder?.ToThemeObject(),

        themeObj_ToolWindowBg = ThemeObj_ToolWindowBg?.ToThemeObject(),
        themeObj_ToolWindowFont = ThemeObj_ToolWindowFont?.ToThemeObject(),
        themeObj_ToolWindowButtonBg = ThemeObj_ToolWindowButtonBg?.ToThemeObject(),
        themeObj_ToolWindowButtonBorder = ThemeObj_ToolWindowButtonBorder?.ToThemeObject(),
    };
}
