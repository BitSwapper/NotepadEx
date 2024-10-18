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
    public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), new PropertyMetadata(Colors.Red, OnColorChanged));

    public event Action OnWindowConfirm;
    public event Action OnWindowCancel;
    public event Action OnSelectedColorChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    Color ogColor;
    bool isUpdating = false;
    bool isColorPlaneDragging;
    bool isHueDragging;
    double currentHue;
    double currentSaturation;
    double currentValue;

    public ColorPicker()
    {
        InitializeComponent();
        DataContext = this;
        currentHue = 0;
        currentSaturation = 1;
        currentValue = 1;
        UpdateColorFromSelectedColor();
        UpdateColorPlane();
        UpdateColorSelector();
        UpdateHueSelector();
    }

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

    static void OnColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        ColorPicker colorPicker = d as ColorPicker;
        if(colorPicker != null && !colorPicker.isUpdating)
            colorPicker.UpdateColorFromSelectedColor();
    }

    void ColorPlane_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        isColorPlaneDragging = true;
        UpdateColorFromColorPlane(e.GetPosition(ColorPlane));
    }

    void ColorPlane_MouseLeave(object sender, MouseEventArgs e) => isColorPlaneDragging = false;

    void ColorPlane_MouseMove(object sender, MouseEventArgs e)
    {
        if(e.LeftButton == MouseButtonState.Pressed)
            isColorPlaneDragging = true;

        if(isColorPlaneDragging)
            UpdateColorFromColorPlane(e.GetPosition(ColorPlane));
    }

    void ColorPlane_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => isColorPlaneDragging = false;

    void HueSlider_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        isHueDragging = true;
        UpdateColorFromHueSlider(e.GetPosition(HueSlider));
    }

    void HueSlider_MouseMove(object sender, MouseEventArgs e)
    {
        if(e.LeftButton == MouseButtonState.Pressed)
            isHueDragging = true;

        if(isHueDragging)
            UpdateColorFromHueSlider(e.GetPosition(HueSlider));
    }

    void HueSlider_MouseLeave(object sender, MouseEventArgs e) => isHueDragging = false;

    void HueSlider_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) => isHueDragging = false;

    public void SetInitialColor(Color color) => SelectedColor = ogColor = color;

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

    void txtHexColor_TextChanged(object sender, TextChangedEventArgs e)
    {
        var text = txtHexColor.Text.Replace("#",string.Empty);
        if(text.Length == 8)
            SelectedColor = ColorUtil.GetColorFromHex(txtHexColor.Text).GetValueOrDefault();
    }

    void UpdateColorFromSelectedColor()
    {
        isUpdating = true;
        var hsv = ColorUtil.RgbToHsv(SelectedColor);
        currentHue = hsv.H;
        currentSaturation = hsv.S;
        currentValue = hsv.V;

        UpdateColorPlane();
        UpdateColorSelector();
        UpdateHueSelector();

        OnPropertyChanged(nameof(Red));
        OnPropertyChanged(nameof(Green));
        OnPropertyChanged(nameof(Blue));
        OnPropertyChanged(nameof(Alpha));
        isUpdating = false;
    }

    void UpdateColorFromRgb(Color color)
    {
        if(!isUpdating)
        {
            isUpdating = true;
            var hsv = ColorUtil.RgbToHsv(color);
            currentHue = hsv.H;
            currentSaturation = hsv.S;
            currentValue = hsv.V;

            UpdateColorPlane();
            UpdateColorSelector();
            UpdateHueSelector();
            isUpdating = false;
            OnSelectedColorChanged?.Invoke();
        }
    }

    void UpdateColorFromColorPlane(Point position)
    {
        currentSaturation = Math.Clamp(position.X / ColorPlane.ActualWidth, 0, 1);
        currentValue = Math.Clamp(1 - (position.Y / ColorPlane.ActualHeight), 0, 1);

        UpdateColorSelector();
        UpdateSelectedColor();
    }

    void UpdateColorFromHueSlider(Point position)
    {
        currentHue = Math.Clamp(position.Y / HueSlider.ActualHeight, 0, 1);

        UpdateHueSelector();
        UpdateColorPlane();
        UpdateSelectedColor();
    }

    void UpdateSelectedColor()
    {
        isUpdating = true;
        SelectedColor = ColorUtil.HsvToRgb(currentHue, currentSaturation, currentValue, SelectedColor.A);
        OnPropertyChanged(nameof(Red));
        OnPropertyChanged(nameof(Green));
        OnPropertyChanged(nameof(Blue));
        OnPropertyChanged(nameof(Alpha));
        isUpdating = false;
    }

    void UpdateColorPlane() => ColorPlane.Background = new SolidColorBrush(ColorUtil.HsvToRgb(currentHue, 1, 1));

    void UpdateColorSelector()
    {
        Canvas.SetLeft(ColorSelector, currentSaturation * ColorPlane.ActualWidth - ColorSelector.Width / 2);
        Canvas.SetTop(ColorSelector, (1 - currentValue) * ColorPlane.ActualHeight - ColorSelector.Height / 2);
    }

    void UpdateHueSelector() => Canvas.SetTop(HueSelector, currentHue * HueSlider.ActualHeight);

    void UserControl_Loaded(object sender, RoutedEventArgs e) => UpdateColorFromSelectedColor();
}


