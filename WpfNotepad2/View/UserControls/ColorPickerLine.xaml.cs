using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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

            gradientPickerWindow.OnSelectedColorChanged += () =>
            {
                gridForImage.Background = gradientPickerWindow.GradientPreview;
                var newBrush = new LinearGradientBrush();
                newBrush.StartPoint = gradientPickerWindow.GradientPreview.StartPoint;
                newBrush.EndPoint = gradientPickerWindow.GradientPreview.EndPoint;
                foreach(var stop in gradientPickerWindow.GradientPreview.GradientStops)
                {
                    newBrush.GradientStops.Add(new GradientStop(stop.Color, stop.Offset));
                }
                AppResourceUtil<LinearGradientBrush>.TrySetResource(Application.Current, path, newBrush);
            };

            if(gridForImage.Background is LinearGradientBrush gradientBrush)
            {
                gradientPickerWindow.GradientStops.Clear();
                gradientPickerWindow.SetGradient(gradientBrush);
            }

            else if(themeObj?.gradient != null)
            {
                gradientPickerWindow.GradientStops.Clear();
                gradientPickerWindow.SetGradient(themeObj.gradient);
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
        if(path is null) return;
        if(themeObj is not null)
        {
            themeObj.isGradient = false;
            if(themeObj.color.HasValue)
                gridForImage.Background = new SolidColorBrush(themeObj.color.Value);
            else
                gridForImage.Background = new SolidColorBrush(Colors.Gray);
        }
        else
        {
            var brush = AppResourceUtil<SolidColorBrush>.TryGetResource(Application.Current, path);
            if(brush is not null)
                gridForImage.Background = brush;
        }

        AppResourceUtil<SolidColorBrush>.TrySetResource(Application.Current, path, gridForImage.Background);
    }

    void rdBtnGradient_Checked(object sender, RoutedEventArgs e)
    {
        if(path is null) return;
        if(themeObj is not null)
        {
            themeObj.isGradient = true;
            if(themeObj.gradient is not null)
                gridForImage.Background = themeObj.gradient;
            else
            {
                GradientStopCollection GradientStops = new();
                GradientStops.Add(new GradientStop(Colors.White, 0));
                GradientStops.Add(new GradientStop(Colors.Black, 1));
                gridForImage.Background = new LinearGradientBrush(GradientStops);
            }
        }
        else
        {
            var brush = AppResourceUtil<LinearGradientBrush>.TryGetResource(Application.Current, path);
            if(brush != null)
                gridForImage.Background = brush;
        }
        AppResourceUtil<LinearGradientBrush>.TrySetResource(Application.Current, path, gridForImage.Background);
    }

    void ButtonCopy_Click(object sender, RoutedEventArgs e)
    {
        if(rdBtnColor.IsChecked == true)
        {
            var color = AppResourceUtil<SolidColorBrush>.TryGetResource(Application.Current, path)?.Color ?? null;
            if(color.HasValue)
                Clipboard.SetText(ColorUtil.ColorToHexString(color.Value));
        }
        else
        {
            var gradient = AppResourceUtil<LinearGradientBrush>.TryGetResource(Application.Current, path);
            if(gradient == null) return;
            var serializedGradient = ColorUtil.SerializeGradient(gradient);
            Clipboard.SetText(serializedGradient);
        }
    }

    void ButtonPaste_Click(object sender, RoutedEventArgs e)
    {
        var gradient = ColorUtil.DeserializeGradient(Clipboard.GetText());
        if(gradient is not null)
        {
            AppResourceUtil<LinearGradientBrush>.TrySetResource(Application.Current, path, gradient);
            gridForImage.Background = gradient;
            themeObj.gradient = gradient;
            themeObj.isGradient = true;
            rdBtnGradient.IsChecked = true;
        }
        else
        {
            Color? color = ColorUtil.GetColorFromHex(Clipboard.GetText());
            if(color.HasValue)
            {
                var brush = new SolidColorBrush(color.Value);
                AppResourceUtil<SolidColorBrush>.TrySetResource(Application.Current, path, brush);
                gridForImage.Background = brush;
                themeObj.color = brush.Color;
                themeObj.isGradient = false;
                rdBtnColor.IsChecked = true;
            }
        }
    }
}
