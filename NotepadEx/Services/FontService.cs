using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using NotepadEx.MVVM.View;
using NotepadEx.Properties;
using NotepadEx.Services.Interfaces;
using NotepadEx.Util;
using FontFamily = System.Windows.Media.FontFamily;
using FontStyle = System.Windows.FontStyle;

namespace NotepadEx.Services; public class FontService : IFontService
{
    public FontSettings CurrentFont { get; private set; }
    public ObservableCollection<FontFamily> AvailableFonts { get; private set; }

    readonly Application application;
    FontEditorWindow fontEditorWindow;

    public FontService(Application application)
    {
        this.application = application;
        AvailableFonts = new ObservableCollection<FontFamily>();
        LoadAvailableFonts();
        CurrentFont = new FontSettings();
    }

    private void LoadAvailableFonts()
    {
        AvailableFonts.Clear();
        foreach(var font in Fonts.SystemFontFamilies.OrderBy(f => f.Source))
        {
            AvailableFonts.Add(font);
        }
    }

    public void LoadCurrentFont()
    {
        // Load from settings
        var fontSettings = new FontSettings
        {
            FontFamily = Settings.Default.FontFamily ?? "Consolas",
            FontSize = Settings.Default.FontSize != 0 ? Settings.Default.FontSize : 12,
            FontStyle = ParseFontStyle(Settings.Default.FontStyle),
            FontWeight = ParseFontWeight(Settings.Default.FontWeight),
        };

        ApplyFont(fontSettings);
    }

    public void ApplyFont(FontSettings fontSettings)
    {
        try
        {
            CurrentFont = fontSettings;

            // Save to settings
            Settings.Default.FontFamily = fontSettings.FontFamily;
            Settings.Default.FontSize = fontSettings.FontSize;
            Settings.Default.FontStyle = fontSettings.FontStyle.ToString();
            Settings.Default.FontWeight = fontSettings.FontWeight.ToString();
            Settings.Default.Save();

            // Apply font settings to application resources
            AppResourceUtil<FontFamily>.TrySetResource(application, UIConstants.Font_Family, new FontFamily(fontSettings.FontFamily));
            AppResourceUtil<double>.TrySetResource(application, UIConstants.Font_Size, fontSettings.FontSize);
            AppResourceUtil<FontStyle>.TrySetResource(application, UIConstants.Font_Style, fontSettings.FontStyle);
            AppResourceUtil<FontWeight>.TrySetResource(application, UIConstants.Font_Weight, fontSettings.FontWeight);
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Error applying font settings. Reverting to default.\r\nException Message: {ex.Message}");
            ApplyFont(new FontSettings()); // Apply defaults
        }
    }


    private FontStyle ParseFontStyle(string fontStyle)
    {
        if(string.IsNullOrEmpty(fontStyle)) return FontStyles.Normal;

        return fontStyle switch
        {
            "Italic" => FontStyles.Italic,
            "Oblique" => FontStyles.Oblique,
            _ => FontStyles.Normal
        };
    }

    private FontWeight ParseFontWeight(string fontWeight)
    {
        if(string.IsNullOrEmpty(fontWeight)) return FontWeights.Normal;

        return fontWeight switch
        {
            "Thin" => FontWeights.Thin,
            "ExtraLight" => FontWeights.ExtraLight,
            "Light" => FontWeights.Light,
            "Regular" => FontWeights.Regular,
            "Medium" => FontWeights.Medium,
            "SemiBold" => FontWeights.SemiBold,
            "Bold" => FontWeights.Bold,
            "ExtraBold" => FontWeights.ExtraBold,
            "Black" => FontWeights.Black,
            _ => FontWeights.Normal
        };
    }

    public void OpenFontEditor()
    {
        if(fontEditorWindow == null || !fontEditorWindow.IsLoaded)
        {
            fontEditorWindow = new FontEditorWindow(this);
        }
        fontEditorWindow.Show();
        fontEditorWindow.Activate();
    }
}