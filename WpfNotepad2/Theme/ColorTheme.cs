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

    public ColorThemeSerializable() { } //needs empty constructor

    public ColorThemeSerializable(ColorTheme colorTheme)
    {
        if(colorTheme.themeObj_TextEditorBg is not null) ThemeObj_TextEditorBg = colorTheme.themeObj_TextEditorBg.ToSerializable();
        if(colorTheme.themeObj_TextEditorFg is not null) ThemeObj_TextEditorFg = colorTheme.themeObj_TextEditorFg.ToSerializable();
        if(colorTheme.themeObj_TitleBarBg is not null) ThemeObj_TitleBarBg = colorTheme.themeObj_TitleBarBg.ToSerializable();
        if(colorTheme.themeObj_TitleBarFont is not null) ThemeObj_TitleBarFont = colorTheme.themeObj_TitleBarFont.ToSerializable();
        if(colorTheme.themeObj_SystemButtons is not null) ThemeObj_SystemButtons = colorTheme.themeObj_SystemButtons.ToSerializable();
        if(colorTheme.themeObj_BorderColor is not null) ThemeObj_BorderColor = colorTheme.themeObj_BorderColor.ToSerializable();

        if(colorTheme.themeObj_MenuBarBg is not null) ThemeObj_MenuBarBg = colorTheme.themeObj_MenuBarBg.ToSerializable();
        if(colorTheme.themeObj_MenuItemFg is not null) ThemeObj_MenuItemFg = colorTheme.themeObj_MenuItemFg.ToSerializable();
        if(colorTheme.themeObj_InfoBarBg is not null) ThemeObj_InfoBarBg = colorTheme.themeObj_InfoBarBg.ToSerializable();
        if(colorTheme.themeObj_InfoBarFg is not null) ThemeObj_InfoBarFg = colorTheme.themeObj_InfoBarFg.ToSerializable();

        if(colorTheme.themeObj_MenuBorder is not null) ThemeObj_MenuBorder = colorTheme.themeObj_MenuBorder.ToSerializable();
        if(colorTheme.themeObj_MenuBg is not null) ThemeObj_MenuBg = colorTheme.themeObj_MenuBg.ToSerializable();
        if(colorTheme.themeObj_MenuFg is not null) ThemeObj_MenuFg = colorTheme.themeObj_MenuFg.ToSerializable();
        if(colorTheme.themeObj_MenuSeperator is not null) ThemeObj_MenuSeperator = colorTheme.themeObj_TextEditorBg.ToSerializable();
        if(colorTheme.themeObj_MenuDisabledFg is not null) ThemeObj_MenuDisabledFg = colorTheme.themeObj_MenuDisabledFg.ToSerializable();
        if(colorTheme.themeObj_MenuItemSelectedBg is not null) ThemeObj_MenuItemSelectedBg = colorTheme.themeObj_MenuItemSelectedBg.ToSerializable();
        if(colorTheme.themeObj_MenuItemSelectedBorder is not null) ThemeObj_MenuItemSelectedBorder = colorTheme.themeObj_MenuItemSelectedBorder.ToSerializable();
        if(colorTheme.themeObj_MenuItemHighlightBg is not null) ThemeObj_MenuItemHighlightBg = colorTheme.themeObj_MenuItemHighlightBg.ToSerializable();
        if(colorTheme.themeObj_MenuItemHighlightBorder is not null) ThemeObj_MenuItemHighlightBorder = colorTheme.themeObj_MenuItemHighlightBorder.ToSerializable();
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
    };
}