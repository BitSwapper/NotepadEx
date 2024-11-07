using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Windows;
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
        try
        {
            if(IsGradient)
                return new ThemeObject(ColorUtil.DeserializeGradient(Gradient));

            else
                return new ThemeObject(ColorUtil.GetColorFromHex(Color).Value);
        }
        catch
        {
            MessageBox.Show("Failed to deserialize theme object. Substituting Blue. (Your theme contains corrupt data - Try picking a different one and restarting application for best results)");
            return new ThemeObject(ColorUtil.GetColorFromHex("#FF0000FF").Value);
        }
    }
}