using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using NotepadEx.Theme;
using NotepadEx.Util;
using NotepadEx.View;
using NotepadEx.Windows;
using Brush = System.Windows.Media.Brush;

namespace NotepadEx.ViewModels;

public class ColorPickerLineViewModel : ViewModelBase
{
    private string themeName;
    private string themePath;
    private ThemeObject themeObj;
    private bool isGradient;
    Brush previewImage;
    Brush backgroundColor;

    public string ThemeName
    {
        get => themeName;
        set
        {
            themeName = value;
            OnPropertyChanged();
        }
    }

    public Brush BackgroundColor
    {
        get => backgroundColor;
        set
        {
            backgroundColor = value;
            OnPropertyChanged();
        }
    }

    public Brush PreviewImage
    {
        get => previewImage;
        set
        {
            previewImage = value;
            OnPropertyChanged();
        }
    }

    public bool IsGradient
    {
        get => isGradient;
        set
        {
            isGradient = value;
            OnPropertyChanged();
            UpdatePreviewColor();
        }
    }

    public ICommand RandomizeCommand { get; }
    public ICommand EditCommand { get; }
    public ICommand CopyCommand { get; }
    public ICommand PasteCommand { get; }

    public ColorPickerLineViewModel()
    {
        RandomizeCommand = new RelayCommand(ExecuteRandomize);
        EditCommand = new RelayCommand(ExecuteEdit);
        CopyCommand = new RelayCommand(ExecuteCopy);
        PasteCommand = new RelayCommand(ExecutePaste);
    }

    public void SetupThemeObj(ThemeObject obj, string themePath, string friendlyThemeName)
    {
        ThemeName = friendlyThemeName;
        this.themePath = themePath;
        themeObj = obj;

        if(obj == null)
        {
            IsGradient = false;
            return;
        }

        IsGradient = obj.isGradient;
        UpdatePreviewColor();
    }

    void UpdatePreviewColor()
    {
        if(themeObj == null) return;

        if(IsGradient)
        {
            PreviewImage = themeObj.gradient ?? CreateDefaultGradient();
        }
        else
        {
            PreviewImage = new SolidColorBrush(themeObj.color ?? Colors.Gray);
        }

        UpdateResourceBrush();
    }

    LinearGradientBrush CreateDefaultGradient() => new LinearGradientBrush
    {
        GradientStops = new GradientStopCollection
        {
            new GradientStop(Colors.White, 0),
            new GradientStop(Colors.Black, 1)
        }
    };

    void UpdateResourceBrush()
    {
        if(IsGradient)
        {
            AppResourceUtil<LinearGradientBrush>.TrySetResource(Application.Current, themePath, PreviewImage as LinearGradientBrush);
        }
        else
        {
            AppResourceUtil<SolidColorBrush>.TrySetResource(Application.Current, themePath, PreviewImage as SolidColorBrush);
        }
    }

    void ExecuteRandomize(object parameter)
    {
        if(IsGradient)
        {
            var brush = ColorUtil.GetRandomLinearGradientBrush(180);
            PreviewImage = brush;
            themeObj.gradient = brush;
        }
        else
        {
            var brush = ColorUtil.GetRandomColorBrush(180);
            PreviewImage = brush;
            themeObj.color = brush.Color;
        }
        UpdateResourceBrush();
    }

    void ExecuteEdit(object parameter)
    {
        if(!IsGradient)
        {
            ColorPickerWindow colorPickerWindow = new();
            colorPickerWindow.myColorPicker.SetInitialColor(themeObj.color ?? Colors.Gray);

            colorPickerWindow.myColorPicker.OnSelectedColorChanged += () =>
            {
                var newBrush = new SolidColorBrush(colorPickerWindow.SelectedColor);
                PreviewImage = newBrush;
                themeObj.color = colorPickerWindow.SelectedColor;
                UpdateResourceBrush();
            };

            colorPickerWindow.ShowDialog();
        }
        else
        {
            GradientPickerWindow gradientPickerWindow = new();

            gradientPickerWindow.OnSelectedColorChanged += () =>
            {
                PreviewImage = gradientPickerWindow.GradientPreview;
                var newBrush = new LinearGradientBrush();
                newBrush.StartPoint = gradientPickerWindow.GradientPreview.StartPoint;
                newBrush.EndPoint = gradientPickerWindow.GradientPreview.EndPoint;
                foreach(var stop in gradientPickerWindow.GradientPreview.GradientStops)
                {
                    newBrush.GradientStops.Add(new GradientStop(stop.Color, stop.Offset));
                }
                AppResourceUtil<LinearGradientBrush>.TrySetResource(Application.Current, themePath, newBrush);
            };

            if(PreviewImage is LinearGradientBrush gradientBrush)
            {
                gradientPickerWindow.SetGradient(gradientBrush);
            }
            else if(themeObj?.gradient != null)
            {
                gradientPickerWindow.SetGradient(themeObj.gradient);
            }

            if(gradientPickerWindow.ShowDialog() == true)
            {
                themeObj.gradient = gradientPickerWindow.GradientPreview;
            }
        }
    }

    void ExecuteCopy(object parameter)
    {
        if(!IsGradient)
        {
            var color = (PreviewImage as SolidColorBrush)?.Color;
            if(color.HasValue)
                Clipboard.SetText(ColorUtil.ColorToHexString(color.Value));
        }
        else
        {
            var gradient = PreviewImage as LinearGradientBrush;
            if(gradient != null)
            {
                var serializedGradient = ColorUtil.SerializeGradient(gradient);
                Clipboard.SetText(serializedGradient);
            }
        }
    }

    void ExecutePaste(object parameter)
    {
        var clipboardText = Clipboard.GetText();
        var gradient = ColorUtil.DeserializeGradient(clipboardText);
        if(gradient != null)
        {
            PreviewImage = gradient;
            themeObj.gradient = gradient;
            IsGradient = true;
        }
        else
        {
            var color = ColorUtil.GetColorFromHex(clipboardText);
            if(color.HasValue)
            {
                var brush = new SolidColorBrush(color.Value);
                PreviewImage = brush;
                themeObj.color = color.Value;
                IsGradient = false;
            }
        }
        UpdateResourceBrush();
    }
}
