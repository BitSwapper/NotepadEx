using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using NotepadEx.Util;
using Color = System.Windows.Media.Color;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using Point = System.Windows.Point;
using UserControl = System.Windows.Controls.UserControl;
namespace NotepadEx.View.UserControls;

public partial class ColorPicker : UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty SelectedColorProperty =
        DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker),
            new PropertyMetadata(Colors.Red, OnColorChanged));

    public event Action OnWindowConfirm;
    public event Action OnWindowCancel;

    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set
        {
            SetValue(SelectedColorProperty, value);
            txtHexColor.Text = ColorUtil.ColorToHexString(value);//why tf can't i just bind

        }
    }

    public byte Red
    {
        get => SelectedColor.R;
        set
        {
            Color newColor = Color.FromArgb(SelectedColor.A, value, SelectedColor.G, SelectedColor.B);
            SelectedColor = newColor;
            UpdateColorFromRgb(newColor);
        }
    }

    public byte Green
    {
        get => SelectedColor.G;
        set
        {
            Color newColor = Color.FromArgb(SelectedColor.A, SelectedColor.R, value, SelectedColor.B);
            SelectedColor = newColor;
            UpdateColorFromRgb(newColor);
        }
    }

    public byte Blue
    {
        get => SelectedColor.B;
        set
        {
            Color newColor = Color.FromArgb(SelectedColor.A, SelectedColor.R, SelectedColor.G, value);
            SelectedColor = newColor;
            UpdateColorFromRgb(newColor);
        }
    }

    public byte Alpha
    {
        get => SelectedColor.A;
        set
        {
            Color newColor = Color.FromArgb(value, SelectedColor.R, SelectedColor.G, SelectedColor.B);
            SelectedColor = newColor;
            UpdateColorFromRgb(newColor);
        }
    }


    Color ogColor;
    private bool _isUpdating = false;
    private bool _isColorPlaneDragging;
    private bool _isHueDragging;
    private double _currentHue;
    private double _currentSaturation;
    private double _currentValue;

    public ColorPicker()
    {
        InitializeComponent();

        if(SelectedColor != null)
            ogColor = SelectedColor;

        DataContext = this;
        _currentHue = 0;
        _currentSaturation = 1;
        _currentValue = 1;
        UpdateColorPlane();
        UpdateColorSelector();
        UpdateHueSelector();
    }








    private static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ColorPicker colorPicker = d as ColorPicker;
        if(colorPicker != null && !colorPicker._isUpdating)
        {
            colorPicker.UpdateColorFromSelectedColor();
        }
    }

    private void UpdateColorFromSelectedColor()
    {
        _isUpdating = true;
        var hsv = RgbToHsv(SelectedColor);
        _currentHue = hsv.H;
        _currentSaturation = hsv.S;
        _currentValue = hsv.V;

        UpdateColorPlane();
        UpdateColorSelector();
        UpdateHueSelector();

        // Update RGBA properties
        OnPropertyChanged(nameof(Red));
        OnPropertyChanged(nameof(Green));
        OnPropertyChanged(nameof(Blue));
        OnPropertyChanged(nameof(Alpha));
        _isUpdating = false;
    }

    private void UpdateColorFromRgb(Color color)
    {
        if(!_isUpdating)
        {
            _isUpdating = true;
            var hsv = RgbToHsv(color);
            _currentHue = hsv.H;
            _currentSaturation = hsv.S;
            _currentValue = hsv.V;

            UpdateColorPlane();
            UpdateColorSelector();
            UpdateHueSelector();
            //HexColor = ColorUtil.ColorToHexString(color);
            _isUpdating = false;
        }
    }

    private void UpdateColorFromColorPlane(Point position)
    {
        _currentSaturation = Math.Clamp(position.X / ColorPlane.ActualWidth, 0, 1);
        _currentValue = Math.Clamp(1 - (position.Y / ColorPlane.ActualHeight), 0, 1);

        UpdateColorSelector();
        UpdateSelectedColor();
    }

    private void UpdateColorFromHueSlider(Point position)
    {
        _currentHue = Math.Clamp(position.Y / HueSlider.ActualHeight, 0, 1);

        UpdateHueSelector();
        UpdateColorPlane();
        UpdateSelectedColor();
    }

    private void UpdateSelectedColor()
    {
        _isUpdating = true;
        SelectedColor = HsvToRgb(_currentHue, _currentSaturation, _currentValue, SelectedColor.A);
        OnPropertyChanged(nameof(Red));
        OnPropertyChanged(nameof(Green));
        OnPropertyChanged(nameof(Blue));
        OnPropertyChanged(nameof(Alpha));
        _isUpdating = false;
    }

    // Implement INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void UpdateColorPlane() => ColorPlane.Background = new SolidColorBrush(HsvToRgb(_currentHue, 1, 1));

    private void UpdateColorSelector()
    {
        Canvas.SetLeft(ColorSelector, _currentSaturation * ColorPlane.ActualWidth);
        Canvas.SetTop(ColorSelector, (1 - _currentValue) * ColorPlane.ActualHeight);
    }

    private void UpdateHueSelector() => Canvas.SetTop(HueSelector, _currentHue * HueSlider.ActualHeight);

    private void ColorPlane_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isColorPlaneDragging = true;
        UpdateColorFromColorPlane(e.GetPosition(ColorPlane));
    }
    private void ColorPlane_MouseLeave(object sender, MouseEventArgs e) => _isColorPlaneDragging = false;

    private void ColorPlane_MouseMove(object sender, MouseEventArgs e)
    {
        if(e.LeftButton == MouseButtonState.Pressed)
        {
            _isColorPlaneDragging = true;
        }
        if(_isColorPlaneDragging)
        {
            UpdateColorFromColorPlane(e.GetPosition(ColorPlane));
        }
    }

    private void ColorPlane_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => _isColorPlaneDragging = false;

    private void HueSlider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isHueDragging = true;
        UpdateColorFromHueSlider(e.GetPosition(HueSlider));
    }

    private void HueSlider_MouseMove(object sender, MouseEventArgs e)
    {
        if(e.LeftButton == MouseButtonState.Pressed)
        {
            _isHueDragging = true;
        }
        if(_isHueDragging)
        {
            UpdateColorFromHueSlider(e.GetPosition(HueSlider));
        }
    }

    private void HueSlider_MouseLeave(object sender, MouseEventArgs e) => _isHueDragging = false;

    private void HueSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => _isHueDragging = false;

    private (double H, double S, double V) RgbToHsv(Color color)
    {
        double r = color.R / 255.0;
        double g = color.G / 255.0;
        double b = color.B / 255.0;

        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        double delta = max - min;

        double h = 0;
        if(delta != 0)
        {
            if(max == r)
                h = (g - b) / delta % 6;
            else if(max == g)
                h = (b - r) / delta + 2;
            else
                h = (r - g) / delta + 4;
        }
        h /= 6;

        double s = max == 0 ? 0 : delta / max;
        double v = max;

        return (h, s, v);
    }

    private Color HsvToRgb(double h, double s, double v, byte a = 255)
    {
        double r, g, b;

        int hi = (int)(h * 6) % 6;
        double f = h * 6 - hi;
        double p = v * (1 - s);
        double q = v * (1 - f * s);
        double t = v * (1 - (1 - f) * s);

        switch(hi)
        {
            case 0: r = v; g = t; b = p; break;
            case 1: r = q; g = v; b = p; break;
            case 2: r = p; g = v; b = t; break;
            case 3: r = p; g = q; b = v; break;
            case 4: r = t; g = p; b = v; break;
            default: r = v; g = p; b = q; break;
        }

        return Color.FromArgb(a, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }

    void ButtonConfirm_Click(object sender, RoutedEventArgs e)
    {
        ogColor = SelectedColor;
        OnWindowConfirm.Invoke();
    }

    void ButtonCancel_Click(object sender, RoutedEventArgs e)
    {
        SelectedColor = ogColor;
        OnWindowCancel.Invoke();
    }
}


