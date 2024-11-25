namespace NotepadEx.Util;

public static class UIConstants
{
    public static readonly int ResizeBorderWidth = 12;
    //public static readonly int InfoBarSize = 18;
    public static readonly int InfoBarHeight = 18;
    public static readonly int MenuBarHeight = 18;

    public static readonly string Color_TextEditorBg            = "Color_TextEditorBg";
    public static readonly string Color_TextEditorFg            = "Color_TextEditorFg";
    public static readonly string Color_TextEditorCaret         = "Color_TextEditorCaret";
    public static readonly string Color_TextEditorScrollBar     = "Color_TextEditorScrollBar";
    public static readonly string Color_TextEditorTextHighlight = "Color_TextEditorTextHighlight";
    public static readonly string Color_TitleBarBg              = "Color_TitleBarBg";
    public static readonly string Color_TitleBarFont            = "Color_TitleBarFont";
    public static readonly string Color_SystemButtons           = "Color_SystemButtons";
    public static readonly string Color_BorderColor             = "Color_BorderColor";
    public static readonly string Color_MenuBarBg               = "Color_MenuBarBg";
    public static readonly string Color_MenuItemFg              = "Color_MenuItemFg";
    public static readonly string Color_InfoBarBg               = "Color_InfoBarBg";
    public static readonly string Color_InfoBarFg               = "Color_InfoBarFg";
    public static readonly string Color_MenuBorder              = "Color_MenuBorder";
    public static readonly string Color_MenuBg                  = "Color_MenuBg";
    public static readonly string Color_MenuFg                  = "Color_MenuFg";
    public static readonly string Color_MenuSeperator           = "Color_MenuSeperator";
    public static readonly string Color_MenuDisabledFg          = "Color_MenuDisabledFg";
    public static readonly string Color_MenuItemSelectedBg      = "Color_MenuItemSelectedBg";
    public static readonly string Color_MenuItemSelectedBorder  = "Color_MenuItemSelectedBorder";
    public static readonly string Color_MenuItemHighlightBg     = "Color_MenuItemHighlightBg";
    public static readonly string Color_MenuItemHighlightBorder = "Color_MenuItemHighlightBorder";
    public static readonly string Color_ToolWindowBg            = "Color_ToolWindowBg";
    public static readonly string Color_ToolWindowFont          = "Color_ToolWindowFont";
    public static readonly string Color_ToolWindowButtonBg      = "Color_ToolWindowButtonBg";
    public static readonly string Color_ToolWindowButtonBorder  = "Color_ToolWindowButtonBorder";

    public static readonly string Font_Family  = "Font_Family";
    public static readonly string Font_Size  = "Font_Size";
    public static readonly string Font_Style  = "Font_Style";
    public static readonly string Font_Weight  = "Font_Weight";
    public static readonly string Font_IsUnderline  = "Font_IsUnderline";
    public static readonly string Font_IsStrikethrough  = "Font_IsStrikethrough";


    public static readonly HashSet<string> UIColorKeysMain = new()
    {
        Color_TextEditorBg,
        Color_TextEditorFg,
        Color_TextEditorCaret,
        Color_TextEditorScrollBar,
        Color_TextEditorTextHighlight,
        Color_TitleBarBg,
        Color_TitleBarFont,
        Color_SystemButtons,
        Color_BorderColor,
    };

    public static readonly HashSet<string> UIColorKeysMenuBar = new()
    {
        Color_MenuBarBg,
        Color_MenuItemFg,
    };

    public static readonly HashSet<string> UIColorKeysInfoBar = new()
    {
        Color_InfoBarBg,
        Color_InfoBarFg,
    };

    public static readonly HashSet<string> UIColorKeysMenuItem = new()
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

    public static readonly List<string> UIColorKeysToolWindow = new()
    {
        Color_ToolWindowBg,
        Color_ToolWindowFont,
        Color_ToolWindowButtonBg,
        Color_ToolWindowButtonBorder,
    };
}
