using System.Text.Json.Serialization;
using NotepadEx.Util;
using Color = System.Windows.Media.Color;
using LinearGradientBrush = System.Windows.Media.LinearGradientBrush;

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
            Gradient = ColorUtil.SerializeGradient(themeObject.gradient);
    }

    public ThemeObject ToThemeObject()
    {
        if(IsGradient)
            return new ThemeObject(ColorUtil.DeserializeGradient(Gradient));

        else
            return new ThemeObject(ColorUtil.GetColorFromHex(Color).Value);
    }
}