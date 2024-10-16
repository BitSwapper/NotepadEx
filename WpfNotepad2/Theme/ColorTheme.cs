using System.Windows.Media;
using NotepadEx.Util;
using Color = System.Windows.Media.Color;

namespace NotepadEx.Theme;

[Serializable]
public class ThemeObject
{
    public bool isGradient;
    public Color color;
    public LinearGradientBrush gradient;
}

public class ColorTheme
{
    public Color? Color_TextEditorBg;
    public Color? Color_TextEditorFg;
    public Color? Color_TitleBarBg;
    public Color? Color_TitleBarFont;
    public Color? Color_SystemButtons;
    public Color? Color_BorderColor;
    public Color? Color_MenuBarBg;
    public Color? Color_MenuItemFg;
    public Color? Color_InfoBarBg;
    public Color? Color_InfoBarFg;
    public Color? Color_MenuBgColor_MenuBg;
    public Color? Color_MenuBorder;
    public Color? Color_MenuFg;
    public Color? Color_MenuSeperator;
    public Color? Color_MenuDisabledFg;
    public Color? Color_MenuItemSelectedBg;
    public Color? Color_MenuItemSelectedBorder;
    public Color? Color_MenuItemHighlightBg;
    public Color? Color_MenuItemHighlightBorder;
    public Color? Color_MenuItemHighlightDisabledBg;
    public Color? Color_MenuItemHighlightDisabledBorder;
    public float GradientAngle;
    public float GradientAngleDif;
    public float GradientScale = 1.0f;

    public ColorThemeSerializable ToSerializable() => new ColorThemeSerializable(this);
}

[Serializable]
public class ColorThemeSerializable
{
    public string Color_TextEditorBg { get; set; }
    public string Color_TextEditorFg { get; set; }
    public string Color_TitleBarBg { get; set; }
    public string Color_TitleBarFont { get; set; }
    public string Color_SystemButtons { get; set; }
    public string Color_BorderColor { get; set; }
    public string Color_MenuBarBg { get; set; }
    public string Color_MenuItemFg { get; set; }
    public string Color_InfoBarBg { get; set; }
    public string Color_InfoBarFg { get; set; }
    public string Color_MenuBgColor_MenuBg { get; set; }
    public string Color_MenuBorder { get; set; }
    public string Color_MenuFg { get; set; }
    public string Color_MenuSeperator { get; set; }
    public string Color_MenuDisabledFg { get; set; }
    public string Color_MenuItemSelectedBg { get; set; }
    public string Color_MenuItemSelectedBorder { get; set; }
    public string Color_MenuItemHighlightBg { get; set; }
    public string Color_MenuItemHighlightBorder { get; set; }
    public string Color_MenuItemHighlightDisabledBg { get; set; }
    public string Color_MenuItemHighlightDisabledBorder { get; set; }
    public float GradientAngle { get; set; }
    public float GradientAngleDif { get; set; }
    public float GradientScale { get; set; } = 1.0f;

    public ColorThemeSerializable() { } //needs empty constructor

    public ColorThemeSerializable(
        Color color_TextEditorBg,
        Color color_TextEditorFg,
        Color color_TitleBarBg,
        Color color_TitleBarFont,
        Color color_SystemButtons,
        Color color_BorderColor,
        Color color_MenuBarBg,
        Color color_MenuItemFg,
        Color color_InfoBarBg,
        Color color_InfoBarFg,
        Color color_MenuBgColor_MenuBg,
        Color color_MenuBorder,
        Color color_MenuFg,
        Color color_MenuSeperator,
        Color color_MenuDisabledFg,
        Color color_MenuItemSelectedBg,
        Color color_MenuItemSelectedBorder,
        Color color_MenuItemHighlightBg,
        Color color_MenuItemHighlightBorder,
        Color color_MenuItemHighlightDisabledBg,
        Color color_MenuItemHighlightDisabledBorder,
        float gradientAngle,
        float gradientAngleDif,
        float gradientScale)
    {

        Color_TextEditorBg = ColorUtil.ColorToHexString(color_TextEditorBg);
        Color_TextEditorFg = ColorUtil.ColorToHexString(color_TextEditorFg);
        Color_TitleBarBg = ColorUtil.ColorToHexString(color_TitleBarBg);
        //Color_TitleBarFont = ColorUtil.ColorToHexString(color_TitleBarFont);
        //Color_SystemButtons = ColorUtil.ColorToHexString(color_SystemButtons);
        //Color_BorderColor = ColorUtil.ColorToHexString(color_BorderColor);
        //Color_MenuBarBg = ColorUtil.ColorToHexString(color_MenuBarBg);
        //Color_MenuItemFg = ColorUtil.ColorToHexString(color_MenuItemFg);
        //Color_InfoBarBg = ColorUtil.ColorToHexString(color_InfoBarBg);
        //Color_InfoBarFg = ColorUtil.ColorToHexString(color_InfoBarFg);
        //Color_MenuBgColor_MenuBg = ColorUtil.ColorToHexString(color_MenuBgColor_MenuBg);
        //Color_MenuBorder = ColorUtil.ColorToHexString(color_MenuBorder);
        //Color_MenuFg = ColorUtil.ColorToHexString(color_MenuFg);
        //Color_MenuSeperator = ColorUtil.ColorToHexString(color_MenuSeperator);
        //Color_MenuDisabledFg = ColorUtil.ColorToHexString(color_MenuDisabledFg);
        //Color_MenuItemSelectedBg = ColorUtil.ColorToHexString(color_MenuItemSelectedBg);
        //Color_MenuItemSelectedBorder = ColorUtil.ColorToHexString(color_MenuItemSelectedBorder);
        //Color_MenuItemHighlightBg = ColorUtil.ColorToHexString(color_MenuItemHighlightBg);
        //Color_MenuItemHighlightBorder = ColorUtil.ColorToHexString(color_MenuItemHighlightBorder);
        //Color_MenuItemHighlightDisabledBg = ColorUtil.ColorToHexString(color_MenuItemHighlightDisabledBg);
        //Color_MenuItemHighlightDisabledBorder = ColorUtil.ColorToHexString(color_MenuItemHighlightDisabledBorder);
        GradientAngle = gradientAngle;
        GradientAngleDif = gradientAngleDif;
        GradientScale = gradientScale;
    }

    public ColorThemeSerializable(ColorTheme colorTheme)
    {
        if(colorTheme.Color_TextEditorBg.HasValue) Color_TextEditorBg = ColorUtil.ColorToHexString(colorTheme.Color_TextEditorBg.Value);
        if(colorTheme.Color_TextEditorFg.HasValue) Color_TextEditorFg = ColorUtil.ColorToHexString(colorTheme.Color_TextEditorFg.Value);
        if(colorTheme.Color_TitleBarBg.HasValue) Color_TitleBarBg = ColorUtil.ColorToHexString(colorTheme.Color_TitleBarBg.Value);
        //Color_TitleBarFont = ColorUtil.ColorToHexString(colorTheme.Color_TitleBarFont);
        //Color_SystemButtons = ColorUtil.ColorToHexString(colorTheme.Color_SystemButtons);
        //Color_BorderColor = ColorUtil.ColorToHexString(colorTheme.Color_BorderColor);
        //Color_MenuBarBg = ColorUtil.ColorToHexString(colorTheme.Color_MenuBarBg);
        //Color_MenuItemFg = ColorUtil.ColorToHexString(colorTheme.Color_MenuItemFg);
        //Color_InfoBarBg = ColorUtil.ColorToHexString(colorTheme.Color_InfoBarBg);
        //Color_InfoBarFg = ColorUtil.ColorToHexString(colorTheme.Color_InfoBarFg);
        //Color_MenuBgColor_MenuBg = ColorUtil.ColorToHexString(colorTheme.Color_MenuBgColor_MenuBg);
        //Color_MenuBorder = ColorUtil.ColorToHexString(colorTheme.Color_MenuBorder);
        //Color_MenuFg = ColorUtil.ColorToHexString(colorTheme.Color_MenuFg);
        //Color_MenuSeperator = ColorUtil.ColorToHexString(colorTheme.Color_MenuSeperator);
        //Color_MenuDisabledFg = ColorUtil.ColorToHexString(colorTheme.Color_MenuDisabledFg);
        //Color_MenuItemSelectedBg = ColorUtil.ColorToHexString(colorTheme.Color_MenuItemSelectedBg);
        //Color_MenuItemSelectedBorder = ColorUtil.ColorToHexString(colorTheme.Color_MenuItemSelectedBorder);
        //Color_MenuItemHighlightBg = ColorUtil.ColorToHexString(colorTheme.Color_MenuItemHighlightBg);
        //Color_MenuItemHighlightBorder = ColorUtil.ColorToHexString(colorTheme.Color_MenuItemHighlightBorder);
        //Color_MenuItemHighlightDisabledBg = ColorUtil.ColorToHexString(colorTheme.Color_MenuItemHighlightDisabledBg);
        //Color_MenuItemHighlightDisabledBorder = ColorUtil.ColorToHexString(colorTheme.Color_MenuItemHighlightDisabledBorder);
        GradientAngle = colorTheme.GradientAngle;
        GradientAngleDif = colorTheme.GradientAngleDif;
        GradientScale = colorTheme.GradientScale;
    }

    public ColorTheme ToColorTheme() => new ColorTheme
    {
        Color_TextEditorBg = ColorUtil.GetColorFromHex(Color_TextEditorBg),
        Color_TextEditorFg = ColorUtil.GetColorFromHex(Color_TextEditorFg),
        Color_TitleBarBg = ColorUtil.GetColorFromHex(Color_TitleBarBg),
        //Color_TitleBarFont = GetColorFromHex(Color_TitleBarFont),
        //Color_SystemButtons = GetColorFromHex(Color_SystemButtons),
        //Color_BorderColor = GetColorFromHex(Color_BorderColor),
        //Color_MenuBarBg = GetColorFromHex(Color_MenuBarBg),
        //Color_MenuItemFg = GetColorFromHex(Color_MenuItemFg),
        //Color_InfoBarBg = GetColorFromHex(Color_InfoBarBg),
        //Color_InfoBarFg = GetColorFromHex(Color_InfoBarFg),
        //Color_MenuBgColor_MenuBg = GetColorFromHex(Color_MenuBgColor_MenuBg),
        //Color_MenuBorder = GetColorFromHex(Color_MenuBorder),
        //Color_MenuFg = GetColorFromHex(Color_MenuFg),
        //Color_MenuSeperator = GetColorFromHex(Color_MenuSeperator),
        //Color_MenuDisabledFg = GetColorFromHex(Color_MenuDisabledFg),
        //Color_MenuItemSelectedBg = GetColorFromHex(Color_MenuItemSelectedBg),
        //Color_MenuItemSelectedBorder = GetColorFromHex(Color_MenuItemSelectedBorder),
        //Color_MenuItemHighlightBg = GetColorFromHex(Color_MenuItemHighlightBg),
        //Color_MenuItemHighlightBorder = GetColorFromHex(Color_MenuItemHighlightBorder),
        //Color_MenuItemHighlightDisabledBg = GetColorFromHex(Color_MenuItemHighlightDisabledBg),
        //Color_MenuItemHighlightDisabledBorder = GetColorFromHex(Color_MenuItemHighlightDisabledBorder),
        GradientAngle = GradientAngle,
        GradientAngleDif = GradientAngleDif,
        GradientScale = GradientScale
    };
}
