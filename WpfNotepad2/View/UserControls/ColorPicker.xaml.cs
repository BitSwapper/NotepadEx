using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
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
    public event Action OnSelectedColorChanged;

    public Color SelectedColor
    {
        get => (Color)GetValue(SelectedColorProperty);
        set
        {
            SetValue(SelectedColorProperty, value);
            txtHexColor.Text = ColorUtil.ColorToHexString(value);//why tf can't i just bind
            OnSelectedColorChanged?.Invoke();
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
    bool _isUpdating = false;
    bool _isColorPlaneDragging;
    bool _isHueDragging;
    double _currentHue;
    double _currentSaturation;
    double _currentValue;

    public ColorPicker()
    {
        InitializeComponent();

        DataContext = this;
        _currentHue = 0;
        _currentSaturation = 1;
        _currentValue = 1;
        UpdateColorFromSelectedColor();
        UpdateColorPlane();
        UpdateColorSelector();
        UpdateHueSelector();
    }








    static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ColorPicker colorPicker = d as ColorPicker;
        if(colorPicker != null && !colorPicker._isUpdating)
        {
            colorPicker.UpdateColorFromSelectedColor();
        }
    }

    void UpdateColorFromSelectedColor()
    {
        _isUpdating = true;
        var hsv = ColorUtil.RgbToHsv(SelectedColor);
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

    void UpdateColorFromRgb(Color color)
    {
        if(!_isUpdating)
        {
            _isUpdating = true;
            var hsv = ColorUtil.RgbToHsv(color);
            _currentHue = hsv.H;
            _currentSaturation = hsv.S;
            _currentValue = hsv.V;

            UpdateColorPlane();
            UpdateColorSelector();
            UpdateHueSelector();
            //HexColor = ColorUtil.ColorToHexString(color);
            _isUpdating = false;
            OnSelectedColorChanged?.Invoke();
        }
    }

    void UpdateColorFromColorPlane(Point position)
    {
        _currentSaturation = Math.Clamp(position.X / ColorPlane.ActualWidth, 0, 1);
        _currentValue = Math.Clamp(1 - (position.Y / ColorPlane.ActualHeight), 0, 1);

        UpdateColorSelector();
        UpdateSelectedColor();
    }

    void UpdateColorFromHueSlider(Point position)
    {
        _currentHue = Math.Clamp(position.Y / HueSlider.ActualHeight, 0, 1);

        UpdateHueSelector();
        UpdateColorPlane();
        UpdateSelectedColor();
    }

    void UpdateSelectedColor()
    {
        _isUpdating = true;
        SelectedColor = ColorUtil.HsvToRgb(_currentHue, _currentSaturation, _currentValue, SelectedColor.A);
        OnPropertyChanged(nameof(Red));
        OnPropertyChanged(nameof(Green));
        OnPropertyChanged(nameof(Blue));
        OnPropertyChanged(nameof(Alpha));
        _isUpdating = false;
    }

    // Implement INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    void UpdateColorPlane() => ColorPlane.Background = new SolidColorBrush(ColorUtil.HsvToRgb(_currentHue, 1, 1));

    void UpdateColorSelector()
    {
        Canvas.SetLeft(ColorSelector, _currentSaturation * ColorPlane.ActualWidth);
        Canvas.SetTop(ColorSelector, (1 - _currentValue) * ColorPlane.ActualHeight);
    }

    void UpdateHueSelector() => Canvas.SetTop(HueSelector, _currentHue * HueSlider.ActualHeight);

    void ColorPlane_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isColorPlaneDragging = true;
        UpdateColorFromColorPlane(e.GetPosition(ColorPlane));
    }
    void ColorPlane_MouseLeave(object sender, MouseEventArgs e) => _isColorPlaneDragging = false;

    void ColorPlane_MouseMove(object sender, MouseEventArgs e)
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

    void ColorPlane_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => _isColorPlaneDragging = false;

    void HueSlider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isHueDragging = true;
        UpdateColorFromHueSlider(e.GetPosition(HueSlider));
    }

    void HueSlider_MouseMove(object sender, MouseEventArgs e)
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

    void HueSlider_MouseLeave(object sender, MouseEventArgs e) => _isHueDragging = false;

    void HueSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => _isHueDragging = false;

    public void SetInitialColor(Color color) => ogColor = color;

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


