namespace NotepadEx.Util;

public static class UIConstants
{
    public static readonly int ResizeBorderWidth = 12;
    public static readonly int InfoBarSize = 18;

    public static readonly string Color_TextEditorBg = "Color_TextEditorBg";
    public static readonly string Color_TextEditorFg = "Color_TextEditorFg";
    public static readonly string Color_TitleBarBg = "Color_TitleBarBg";
    public static readonly string Color_TitleBarFont = "Color_TitleBarFont";
    public static readonly string Color_SystemButtons = "Color_SystemButtons";
    public static readonly string Color_BorderColor = "Color_BorderColor";
    public static readonly string Color_MenuBarBg = "Color_MenuBarBg";
    public static readonly string Color_MenuItemFg = "Color_MenuItemFg";
    public static readonly string Color_InfoBarBg = "Color_InfoBarBg";
    public static readonly string Color_InfoBarFg = "Color_InfoBarFg";
    public static readonly string Color_MenuBorder = "Color_MenuBorder";
    public static readonly string Color_MenuBg = "Color_MenuBg";
    public static readonly string Color_MenuFg = "Color_MenuFg";
    public static readonly string Color_MenuSeperator = "Color_MenuSeperator";
    public static readonly string Color_MenuDisabledFg = "Color_MenuDisabledFg";
    public static readonly string Color_MenuItemSelectedBg = "Color_MenuItemSelectedBg";
    public static readonly string Color_MenuItemSelectedBorder = "Color_MenuItemSelectedBorder";
    public static readonly string Color_MenuItemHighlightBg = "Color_MenuItemHighlightBg";
    public static readonly string Color_MenuItemHighlightBorder = "Color_MenuItemHighlightBorder";

    public static readonly List<string> UIColorKeysMain = new()
    {
        Color_TextEditorBg,
        Color_TextEditorFg,
        Color_TitleBarBg,
        Color_TitleBarFont,
        Color_SystemButtons,
        Color_BorderColor,
    };

    public static readonly List<string> UIColorKeysMenuBar = new()
    {
        Color_MenuBarBg,
        Color_MenuItemFg,
    };

    public static readonly List<string> UIColorKeysInfoBar = new()
    {
        Color_InfoBarBg,
        Color_InfoBarFg,
    };

    public static readonly List<string> UIColorKeysMenuItem = new()
    {
        Color_MenuBorder,
        Color_MenuBg,
        Color_MenuFg,
        Color_MenuSeperator,
        Color_MenuDisabledFg,
        Color_MenuItemSelectedBg,
        Color_MenuItemSelectedBorder,
        Color_MenuItemHighlightBg,
        Color_MenuItemHighlightBorder,
    };
}
