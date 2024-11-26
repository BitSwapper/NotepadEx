using System.Collections.ObjectModel;
using System.Windows;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;

namespace NotepadEx.Services.Interfaces;
public interface IFontService
{
    FontSettings CurrentFont { get; }
    ObservableCollection<FontFamily> AvailableFonts { get; }
    void LoadCurrentFont();
    void ApplyFont(FontSettings fontSettings);
    void OpenFontEditor();
}

public class FontSettings
{
    public string FontFamily { get; set; } = "Consolas";
    public double FontSize { get; set; } = 12;
    public FontStyle FontStyle { get; set; } = FontStyles.Normal;
    public FontWeight FontWeight { get; set; } = FontWeights.Normal;
}

public class FontInfo
{
    public string Name { get; set; }
    public string FilePath { get; set; }
    public DateTime LastModified { get; set; }
}