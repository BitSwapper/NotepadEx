using NotepadEx.Util;
using System.Text.Json.Serialization;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using LinearGradientBrush = System.Windows.Media.LinearGradientBrush;
using Point = System.Windows.Point;

namespace NotepadEx.Theme;

public class ThemeObject
{
    public bool isGradient;
    public Color? color;
    public LinearGradientBrush? gradient;

    public ThemeObject() { }

    public ThemeObject(Color Color)
    {
        color = Color;
        isGradient = false;
    }

    public ThemeObject(LinearGradientBrush Gradient)
    {
        gradient = Gradient;
        isGradient = true;
    }

    public ThemeObjectSerializable ToSerializable() => new ThemeObjectSerializable(this);
}


[Serializable]
public class ThemeObjectSerializable
{
    [JsonPropertyName("isGradient")]
    public bool IsGradient { get; set; }

    [JsonPropertyName("color")]
    public string Color { get; set; }

    [JsonPropertyName("gradient")]
    public string Gradient { get; set; }

    public ThemeObjectSerializable() { } // Needs empty constructor

    public ThemeObjectSerializable(ThemeObject themeObject)
    {
        IsGradient = themeObject.isGradient;

        if(themeObject.color.HasValue)
            Color = ColorUtil.ColorToHexString(themeObject.color.Value);

        if(themeObject.gradient != null)
            Gradient = SerializeGradient(themeObject.gradient);
    }

    public ThemeObject ToThemeObject()
    {
        if(IsGradient)
            return new ThemeObject(DeserializeGradient(Gradient));

        else
            return new ThemeObject(ColorUtil.GetColorFromHex(Color).Value);
    }

    string SerializeGradient(LinearGradientBrush gradient)
    {
        var serializedData = new List<string>
        {
            $"StartPoint:{gradient.StartPoint.X},{gradient.StartPoint.Y}",
            $"EndPoint:{gradient.EndPoint.X},{gradient.EndPoint.Y}",
            $"SpreadMethod:{gradient.SpreadMethod}",
            $"MappingMode:{gradient.MappingMode}",
            $"ColorInterpolationMode:{gradient.ColorInterpolationMode}"
        };

        var stops = gradient.GradientStops.Select(stop =>
            $"{ColorUtil.ColorToHexString(stop.Color)}:{stop.Offset}");
        serializedData.Add($"GradientStops:{string.Join("|", stops)}");

        return string.Join(";", serializedData);
    }

    LinearGradientBrush DeserializeGradient(string gradientString)
    {
        var parts = gradientString.Split(';');
        var brush = new LinearGradientBrush();

        foreach(var part in parts)
        {
            var keyValue = part.Split(':');
            if(keyValue.Length < 2) continue;

            switch(keyValue[0])
            {
                case "StartPoint":
                    var startPoint = keyValue[1].Split(',');
                    brush.StartPoint = new Point(double.Parse(startPoint[0]), double.Parse(startPoint[1]));
                    break;
                case "EndPoint":
                    var endPoint = keyValue[1].Split(',');
                    brush.EndPoint = new Point(double.Parse(endPoint[0]), double.Parse(endPoint[1]));
                    break;
                case "SpreadMethod":
                    brush.SpreadMethod = (GradientSpreadMethod)Enum.Parse(typeof(GradientSpreadMethod), keyValue[1]);
                    break;
                case "MappingMode":
                    brush.MappingMode = (BrushMappingMode)Enum.Parse(typeof(BrushMappingMode), keyValue[1]);
                    break;
                case "ColorInterpolationMode":
                    brush.ColorInterpolationMode = (ColorInterpolationMode)Enum.Parse(typeof(ColorInterpolationMode), keyValue[1]);
                    break;
                case "GradientStops":
                    var stopsData = string.Join(":", keyValue.Skip(1));  // Rejoin in case there were colons in the color data
                    var stops = stopsData.Split('|').Select(stop =>
                    {
                        var stopParts = stop.Split(':');
                        if (stopParts.Length != 2) throw new FormatException($"Invalid gradient stop format: {stop}");
                        return new GradientStop(ColorUtil.GetColorFromHex(stopParts[0]).Value, double.Parse(stopParts[1]));
                    });
                    brush.GradientStops = new GradientStopCollection(stops.ToList());  // Convert to List before creating GradientStopCollection
                    break;
            }
        }

        return brush;
    }
}


/*
 * 
[Serializable]
public class ThemeObject
{
    public bool isGradient;
    public Color? color;
    public LinearGradientBrush? gradient;
}

public class ColorTheme
{
    public Color? ThemeObj_TextEditorBg;
    public Color? ThemeObj_TextEditorFg;
    public ThemeObject? ThemeObj_TitleBarBg;
    public Color? ThemeObj_TitleBarFont;
    public Color? ThemeObj_SystemButtons;
    public Color? ThemeObj_BorderColor;
    public Color? ThemeObj_MenuBarBg;
    public Color? ThemeObj_MenuItemFg;
    public Color? ThemeObj_InfoBarBg;
    public Color? ThemeObj_InfoBarFg;
    public Color? ThemeObj_MenuBgThemeObj_MenuBg;
    public Color? ThemeObj_MenuBorder;
    public Color? ThemeObj_MenuFg;
    public Color? ThemeObj_MenuSeperator;
    public Color? ThemeObj_MenuDisabledFg;
    public Color? ThemeObj_MenuItemSelectedBg;
    public Color? ThemeObj_MenuItemSelectedBorder;
    public Color? ThemeObj_MenuItemHighlightBg;
    public Color? ThemeObj_MenuItemHighlightBorder;
    public Color? ThemeObj_MenuItemHighlightDisabledBg;
    public Color? ThemeObj_MenuItemHighlightDisabledBorder;
    public float GradientAngle;
    public float GradientAngleDif;
    public float GradientScale = 1.0f;

    public ColorThemeSerializable ToSerializable() => new ColorThemeSerializable(this);
}

[Serializable]
public class ColorThemeSerializable
{
    public string ThemeObj_TextEditorBg { get; set; }
    public string ThemeObj_TextEditorFg { get; set; }
    public string ThemeObj_TitleBarBg { get; set; }
    public string ThemeObj_TitleBarFont { get; set; }
    public string ThemeObj_SystemButtons { get; set; }
    public string ThemeObj_BorderColor { get; set; }
    public string ThemeObj_MenuBarBg { get; set; }
    public string ThemeObj_MenuItemFg { get; set; }
    public string ThemeObj_InfoBarBg { get; set; }
    public string ThemeObj_InfoBarFg { get; set; }
    public string ThemeObj_MenuBgThemeObj_MenuBg { get; set; }
    public string ThemeObj_MenuBorder { get; set; }
    public string ThemeObj_MenuFg { get; set; }
    public string ThemeObj_MenuSeperator { get; set; }
    public string ThemeObj_MenuDisabledFg { get; set; }
    public string ThemeObj_MenuItemSelectedBg { get; set; }
    public string ThemeObj_MenuItemSelectedBorder { get; set; }
    public string ThemeObj_MenuItemHighlightBg { get; set; }
    public string ThemeObj_MenuItemHighlightBorder { get; set; }
    public string ThemeObj_MenuItemHighlightDisabledBg { get; set; }
    public string ThemeObj_MenuItemHighlightDisabledBorder { get; set; }
    public float GradientAngle { get; set; }
    public float GradientAngleDif { get; set; }
    public float GradientScale { get; set; } = 1.0f;

    public ColorThemeSerializable() { } //needs empty constructor

    public ColorThemeSerializable(
        Color themeObj_TextEditorBg,
        Color themeObj_TextEditorFg,
        ThemeObject themeObj_TitleBarBg,
        Color themeObj_TitleBarFont,
        Color themeObj_SystemButtons,
        Color themeObj_BorderColor,
        Color themeObj_MenuBarBg,
        Color themeObj_MenuItemFg,
        Color themeObj_InfoBarBg,
        Color themeObj_InfoBarFg,
        Color themeObj_MenuBgThemeObj_MenuBg,
        Color themeObj_MenuBorder,
        Color themeObj_MenuFg,
        Color themeObj_MenuSeperator,
        Color themeObj_MenuDisabledFg,
        Color themeObj_MenuItemSelectedBg,
        Color themeObj_MenuItemSelectedBorder,
        Color themeObj_MenuItemHighlightBg,
        Color themeObj_MenuItemHighlightBorder,
        Color themeObj_MenuItemHighlightDisabledBg,
        Color themeObj_MenuItemHighlightDisabledBorder,
        float gradientAngle,
        float gradientAngleDif,
        float gradientScale)
    {

        ThemeObj_TextEditorBg = ColorUtil.ColorToHexString(themeObj_TextEditorBg);
        ThemeObj_TextEditorFg = ColorUtil.ColorToHexString(themeObj_TextEditorFg);
        ThemeObj_TitleBarBg = themeObj_TitleBarBg;
        //ThemeObj_TitleBarFont = ColorUtil.ColorToHexString(themeObj_TitleBarFont);
        //ThemeObj_SystemButtons = ColorUtil.ColorToHexString(themeObj_SystemButtons);
        //ThemeObj_BorderColor = ColorUtil.ColorToHexString(themeObj_BorderColor);
        //ThemeObj_MenuBarBg = ColorUtil.ColorToHexString(themeObj_MenuBarBg);
        //ThemeObj_MenuItemFg = ColorUtil.ColorToHexString(themeObj_MenuItemFg);
        //ThemeObj_InfoBarBg = ColorUtil.ColorToHexString(themeObj_InfoBarBg);
        //ThemeObj_InfoBarFg = ColorUtil.ColorToHexString(themeObj_InfoBarFg);
        //ThemeObj_MenuBgThemeObj_MenuBg = ColorUtil.ColorToHexString(themeObj_MenuBgThemeObj_MenuBg);
        //ThemeObj_MenuBorder = ColorUtil.ColorToHexString(themeObj_MenuBorder);
        //ThemeObj_MenuFg = ColorUtil.ColorToHexString(themeObj_MenuFg);
        //ThemeObj_MenuSeperator = ColorUtil.ColorToHexString(themeObj_MenuSeperator);
        //ThemeObj_MenuDisabledFg = ColorUtil.ColorToHexString(themeObj_MenuDisabledFg);
        //ThemeObj_MenuItemSelectedBg = ColorUtil.ColorToHexString(themeObj_MenuItemSelectedBg);
        //ThemeObj_MenuItemSelectedBorder = ColorUtil.ColorToHexString(themeObj_MenuItemSelectedBorder);
        //ThemeObj_MenuItemHighlightBg = ColorUtil.ColorToHexString(themeObj_MenuItemHighlightBg);
        //ThemeObj_MenuItemHighlightBorder = ColorUtil.ColorToHexString(themeObj_MenuItemHighlightBorder);
        //ThemeObj_MenuItemHighlightDisabledBg = ColorUtil.ColorToHexString(themeObj_MenuItemHighlightDisabledBg);
        //ThemeObj_MenuItemHighlightDisabledBorder = ColorUtil.ColorToHexString(themeObj_MenuItemHighlightDisabledBorder);
        GradientAngle = gradientAngle;
        GradientAngleDif = gradientAngleDif;
        GradientScale = gradientScale;
    }

    public ColorThemeSerializable(ColorTheme colorTheme)
    {
        if(colorTheme.ThemeObj_TextEditorBg.HasValue) ThemeObj_TextEditorBg = ColorUtil.ColorToHexString(colorTheme.ThemeObj_TextEditorBg.Value);
        if(colorTheme.ThemeObj_TextEditorFg.HasValue) ThemeObj_TextEditorFg = ColorUtil.ColorToHexString(colorTheme.ThemeObj_TextEditorFg.Value);
        if(colorTheme.ThemeObj_TitleBarBg.HasValue) ThemeObj_TitleBarBg = ColorUtil.ColorToHexString(colorTheme.ThemeObj_TitleBarBg.Value);
        //ThemeObj_TitleBarFont = ColorUtil.ColorToHexString(colorTheme.ThemeObj_TitleBarFont);
        //ThemeObj_SystemButtons = ColorUtil.ColorToHexString(colorTheme.ThemeObj_SystemButtons);
        //ThemeObj_BorderColor = ColorUtil.ColorToHexString(colorTheme.ThemeObj_BorderColor);
        //ThemeObj_MenuBarBg = ColorUtil.ColorToHexString(colorTheme.ThemeObj_MenuBarBg);
        //ThemeObj_MenuItemFg = ColorUtil.ColorToHexString(colorTheme.ThemeObj_MenuItemFg);
        //ThemeObj_InfoBarBg = ColorUtil.ColorToHexString(colorTheme.ThemeObj_InfoBarBg);
        //ThemeObj_InfoBarFg = ColorUtil.ColorToHexString(colorTheme.ThemeObj_InfoBarFg);
        //ThemeObj_MenuBgThemeObj_MenuBg = ColorUtil.ColorToHexString(colorTheme.ThemeObj_MenuBgThemeObj_MenuBg);
        //ThemeObj_MenuBorder = ColorUtil.ColorToHexString(colorTheme.ThemeObj_MenuBorder);
        //ThemeObj_MenuFg = ColorUtil.ColorToHexString(colorTheme.ThemeObj_MenuFg);
        //ThemeObj_MenuSeperator = ColorUtil.ColorToHexString(colorTheme.ThemeObj_MenuSeperator);
        //ThemeObj_MenuDisabledFg = ColorUtil.ColorToHexString(colorTheme.ThemeObj_MenuDisabledFg);
        //ThemeObj_MenuItemSelectedBg = ColorUtil.ColorToHexString(colorTheme.ThemeObj_MenuItemSelectedBg);
        //ThemeObj_MenuItemSelectedBorder = ColorUtil.ColorToHexString(colorTheme.ThemeObj_MenuItemSelectedBorder);
        //ThemeObj_MenuItemHighlightBg = ColorUtil.ColorToHexString(colorTheme.ThemeObj_MenuItemHighlightBg);
        //ThemeObj_MenuItemHighlightBorder = ColorUtil.ColorToHexString(colorTheme.ThemeObj_MenuItemHighlightBorder);
        //ThemeObj_MenuItemHighlightDisabledBg = ColorUtil.ColorToHexString(colorTheme.ThemeObj_MenuItemHighlightDisabledBg);
        //ThemeObj_MenuItemHighlightDisabledBorder = ColorUtil.ColorToHexString(colorTheme.ThemeObj_MenuItemHighlightDisabledBorder);
        GradientAngle = colorTheme.GradientAngle;
        GradientAngleDif = colorTheme.GradientAngleDif;
        GradientScale = colorTheme.GradientScale;
    }

    public ColorTheme ToColorTheme() => new ColorTheme
    {
        ThemeObj_TextEditorBg = ColorUtil.GetColorFromHex(ThemeObj_TextEditorBg),
        ThemeObj_TextEditorFg = ColorUtil.GetColorFromHex(ThemeObj_TextEditorFg),
        ThemeObj_TitleBarBg = ColorUtil.GetColorFromHex(ThemeObj_TitleBarBg),
        //ThemeObj_TitleBarFont = GetColorFromHex(ThemeObj_TitleBarFont),
        //ThemeObj_SystemButtons = GetColorFromHex(ThemeObj_SystemButtons),
        //ThemeObj_BorderColor = GetColorFromHex(ThemeObj_BorderColor),
        //ThemeObj_MenuBarBg = GetColorFromHex(ThemeObj_MenuBarBg),
        //ThemeObj_MenuItemFg = GetColorFromHex(ThemeObj_MenuItemFg),
        //ThemeObj_InfoBarBg = GetColorFromHex(ThemeObj_InfoBarBg),
        //ThemeObj_InfoBarFg = GetColorFromHex(ThemeObj_InfoBarFg),
        //ThemeObj_MenuBgThemeObj_MenuBg = GetColorFromHex(ThemeObj_MenuBgThemeObj_MenuBg),
        //ThemeObj_MenuBorder = GetColorFromHex(ThemeObj_MenuBorder),
        //ThemeObj_MenuFg = GetColorFromHex(ThemeObj_MenuFg),
        //ThemeObj_MenuSeperator = GetColorFromHex(ThemeObj_MenuSeperator),
        //ThemeObj_MenuDisabledFg = GetColorFromHex(ThemeObj_MenuDisabledFg),
        //ThemeObj_MenuItemSelectedBg = GetColorFromHex(ThemeObj_MenuItemSelectedBg),
        //ThemeObj_MenuItemSelectedBorder = GetColorFromHex(ThemeObj_MenuItemSelectedBorder),
        //ThemeObj_MenuItemHighlightBg = GetColorFromHex(ThemeObj_MenuItemHighlightBg),
        //ThemeObj_MenuItemHighlightBorder = GetColorFromHex(ThemeObj_MenuItemHighlightBorder),
        //ThemeObj_MenuItemHighlightDisabledBg = GetColorFromHex(ThemeObj_MenuItemHighlightDisabledBg),
        //ThemeObj_MenuItemHighlightDisabledBorder = GetColorFromHex(ThemeObj_MenuItemHighlightDisabledBorder),
        GradientAngle = GradientAngle,
        GradientAngleDif = GradientAngleDif,
        GradientScale = GradientScale
    };
}*/