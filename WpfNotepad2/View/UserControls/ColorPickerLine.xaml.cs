using System.Windows;
using System.Windows.Controls;
using NotepadEx.Theme;
using NotepadEx.Util;
using NotepadEx.Windows;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using LinearGradientBrush = System.Windows.Media.LinearGradientBrush;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
namespace NotepadEx.View.UserControls;

public partial class ColorPickerLine : UserControl
{
    string path;
    ThemeObject themeObj;
    public Grid GridForImg => gridForImage;
    public ColorPickerViewModel ViewModel { get; set; }
    public ColorPickerLine()
    {
        DataContext = ViewModel = new ColorPickerViewModel();
        InitializeComponent();
    }

    public void SetupThemeObj(ThemeObject obj, string themePath, string friendlyThemeName)
    {
        txtThemeName.Text = friendlyThemeName;
        path = themePath;
        if(obj == null)
        {
            rdBtnColor.IsChecked = true;
            return;
        }
        themeObj = obj;
        rdBtnColor.IsChecked = !obj.isGradient;
        rdBtnGradient.IsChecked = obj.isGradient;
    }

    void ButtonRandomize_Click(object sender, RoutedEventArgs e)
    {
        if(themeObj?.isGradient ?? false)
        {
            var brush =  ColorUtil.GetRandomLinearGradientBrush(180);
            AppResourceUtil<LinearGradientBrush>.TrySetResource(Application.Current, path, brush);
            gridForImage.Background = brush;
            themeObj.gradient = brush;
        }
        else
        {
            var brush =  ColorUtil.GetRandomColorBrush(180);
            AppResourceUtil<SolidColorBrush>.TrySetResource(Application.Current, path, brush);
            gridForImage.Background = brush;
            themeObj.color = brush.Color;
        }
    }


    void ButtonEdit_Click(object sender, RoutedEventArgs e)
    {
        if(!themeObj.isGradient)
        {
            ColorPickerWindow colorPickerWindow = new();
            colorPickerWindow.myColorPicker.SetInitialColor(themeObj.color.GetValueOrDefault());

            colorPickerWindow.myColorPicker.OnSelectedColorChanged += () =>
            {
                AppResourceUtil<SolidColorBrush>.TrySetResource(Application.Current, path, new SolidColorBrush(colorPickerWindow.SelectedColor));
                themeObj.color = colorPickerWindow.SelectedColor;
            };

            if(colorPickerWindow.ShowDialog() == true)
                gridForImage.Background = new SolidColorBrush(colorPickerWindow.SelectedColor);
        }
        else
        {
            GradientPickerWindow gradientPickerWindow = new();

            //      **This code shows our current gradient but breaks our sliders afterward..
            if(gridForImage.Background is LinearGradientBrush gradientBrush)
            {
                gradientPickerWindow.GradientStops.Clear();
                gradientPickerWindow.SetGradient(gradientBrush.GradientStops);
            }


            if(gradientPickerWindow.ShowDialog() == true)
            {
                gridForImage.Background = gradientPickerWindow.GradientBrush;
                AppResourceUtil<LinearGradientBrush>.TrySetResource(Application.Current, path, gradientPickerWindow.GradientBrush);
                themeObj.gradient = gradientPickerWindow.GradientBrush;
            }
        }
    }

    void rdBtnColor_Checked(object sender, RoutedEventArgs e)
    {
        if(path != null)
            GridForImg.Background = AppResourceUtil<SolidColorBrush>.TryGetResource(Application.Current, path);

        if(themeObj != null)
            themeObj.isGradient = false;
    }

    void rdBtnGradient_Checked(object sender, RoutedEventArgs e)
    {
        if(path != null)
            GridForImg.Background = AppResourceUtil<LinearGradientBrush>.TryGetResource(Application.Current, path);

        if(themeObj != null)
            themeObj.isGradient = true;
    }

    private void ButtonCopy_Click(object sender, RoutedEventArgs e)
    {
        if(rdBtnColor.IsChecked == true)
        {
            var color = AppResourceUtil<SolidColorBrush>.TryGetResource(Application.Current, path).Color;
            Clipboard.SetText(ColorUtil.ColorToHexString(color));
        }
        else
        {
            var gradient = AppResourceUtil<LinearGradientBrush>.TryGetResource(Application.Current, path);
            var serializedGradient = ColorUtil.SerializeGradient(gradient);
            Clipboard.SetText(serializedGradient);
        }
    }

    private void ButtonPaste_Click(object sender, RoutedEventArgs e)
    {
        Color? color = ColorUtil.GetColorFromHex(Clipboard.GetText());
        if(color.HasValue)
        {
            var brush = new SolidColorBrush(color.Value);
            AppResourceUtil<SolidColorBrush>.TrySetResource(Application.Current, path, brush);
            gridForImage.Background = brush;
        }
        else
        {
            var gradient = ColorUtil.DeserializeGradient(Clipboard.GetText());
            AppResourceUtil<LinearGradientBrush>.TrySetResource(Application.Current, path, gradient);
            gridForImage.Background = gradient;
        }
    }
}
