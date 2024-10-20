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
    private string _themeName;
    private string _themePath;
    private ThemeObject _themeObj;
    private bool _isGradient;
    Brush previewImage;
    Brush backgroundColor;

    public string ThemeName
    {
        get => _themeName;
        set
        {
            _themeName = value;
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
        get => _isGradient;
        set
        {
            _isGradient = value;
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
        _themePath = themePath;
        _themeObj = obj;

        if(obj == null)
        {
            IsGradient = false;
            return;
        }

        IsGradient = obj.isGradient;
        UpdatePreviewColor();
    }

    private void UpdatePreviewColor()
    {
        if(_themeObj == null) return;

        if(IsGradient)
        {
            PreviewImage = _themeObj.gradient ?? CreateDefaultGradient();
        }
        else
        {
            PreviewImage = new SolidColorBrush(_themeObj.color ?? Colors.Gray);
        }

        UpdateResourceBrush();
    }

    private LinearGradientBrush CreateDefaultGradient() => new LinearGradientBrush
    {
        GradientStops = new GradientStopCollection
        {
            new GradientStop(Colors.White, 0),
            new GradientStop(Colors.Black, 1)
        }
    };

    private void UpdateResourceBrush()
    {
        if(IsGradient)
        {
            AppResourceUtil<LinearGradientBrush>.TrySetResource(Application.Current, _themePath, PreviewImage as LinearGradientBrush);
        }
        else
        {
            AppResourceUtil<SolidColorBrush>.TrySetResource(Application.Current, _themePath, PreviewImage as SolidColorBrush);
        }
    }

    private void ExecuteRandomize(object parameter)
    {
        if(IsGradient)
        {
            var brush = ColorUtil.GetRandomLinearGradientBrush(180);
            PreviewImage = brush;
            _themeObj.gradient = brush;
        }
        else
        {
            var brush = ColorUtil.GetRandomColorBrush(180);
            PreviewImage = brush;
            _themeObj.color = brush.Color;
        }
        UpdateResourceBrush();
    }

    private void ExecuteEdit(object parameter)
    {
        if(!IsGradient)
        {
            ColorPickerWindow colorPickerWindow = new();
            colorPickerWindow.myColorPicker.SetInitialColor(_themeObj.color ?? Colors.Gray);

            colorPickerWindow.myColorPicker.OnSelectedColorChanged += () =>
            {
                var newBrush = new SolidColorBrush(colorPickerWindow.SelectedColor);
                PreviewImage = newBrush;
                _themeObj.color = colorPickerWindow.SelectedColor;
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
                AppResourceUtil<LinearGradientBrush>.TrySetResource(Application.Current, _themePath, newBrush);



                //PreviewImage = gradientPickerWindow.GradientPreview;
                //_themeObj.gradient = gradientPickerWindow.GradientPreview;
                //UpdateResourceBrush();
            };

            if(PreviewImage is LinearGradientBrush gradientBrush)
            {
                gradientPickerWindow.SetGradient(gradientBrush);
            }
            else if(_themeObj?.gradient != null)
            {
                gradientPickerWindow.SetGradient(_themeObj.gradient);
            }

            if(gradientPickerWindow.ShowDialog() == true)
            {
                _themeObj.gradient = gradientPickerWindow.GradientPreview;
            }
        }
    }

    private void ExecuteCopy(object parameter)
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

    private void ExecutePaste(object parameter)
    {
        var clipboardText = Clipboard.GetText();
        var gradient = ColorUtil.DeserializeGradient(clipboardText);
        if(gradient != null)
        {
            PreviewImage = gradient;
            _themeObj.gradient = gradient;
            IsGradient = true;
        }
        else
        {
            var color = ColorUtil.GetColorFromHex(clipboardText);
            if(color.HasValue)
            {
                var brush = new SolidColorBrush(color.Value);
                PreviewImage = brush;
                _themeObj.color = color.Value;
                IsGradient = false;
            }
        }
        UpdateResourceBrush();
    }
}
