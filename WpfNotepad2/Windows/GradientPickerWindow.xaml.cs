using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NotepadEx.Util;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;
namespace NotepadEx.Windows;

public partial class GradientPickerWindow : Window
{
    public ObservableCollection<GradientStop> GradientStops { get; set; } = new();
    public LinearGradientBrush GradientBrush => GradientPreview;
    bool updatingFromAngle = false;
    bool updatingFromOffset = false;
    bool updatingFromScale = false;
    double ScaleX { get; set; } = 1.0;
    double ScaleY { get; set; } = 1.0;

    public GradientPickerWindow()
    {
        InitializeComponent();
        GradientStops = new ObservableCollection<GradientStop>();
        StopsListBox.ItemsSource = GradientStops;
        GradientStops.Add(new GradientStop(Colors.White, 0));
        GradientStops.Add(new GradientStop(Colors.Black, 1));
        UpdateGradientPreview();
    }

    public void SetGradient(GradientStopCollection stops)
    {
        GradientStops.Clear();
        foreach(var stop in stops)
            GradientStops.Add(stop);
        UpdateGradientPreview();
    }

    void UpdateGradientPreview()
    {
        if(GradientPreview == null) return;

        GradientPreview.GradientStops.Clear();
        foreach(var stop in GradientStops)
            GradientPreview.GradientStops.Add(stop);

        if(StartXSlider != null && StartYSlider != null && EndXSlider != null && EndYSlider != null)
        {
            double offsetX = SliderOffsetX.Value;
            double offsetY = SliderOffsetY.Value;

            // Original start and end points
            double startXOriginal = StartXSlider.Value;
            double startYOriginal = StartYSlider.Value;
            double endXOriginal = EndXSlider.Value;
            double endYOriginal = EndYSlider.Value;

            // Calculate the center point of the gradient
            double centerX = (startXOriginal + endXOriginal) / 2;
            double centerY = (startYOriginal + endYOriginal) / 2;

            // Calculate the angle and length of the original gradient
            double dx = endXOriginal - startXOriginal;
            double dy = endYOriginal - startYOriginal;
            double angle = Math.Atan2(dy, dx); // Angle in radians
            double originalLength = Math.Sqrt(dx * dx + dy * dy);

            // Apply scaling to the length while keeping the angle intact
            double scaledLengthX = originalLength * ScaleX;
            double scaledLengthY = originalLength * ScaleY;

            // Calculate new half-lengths based on scaled lengths
            double newHalfLengthX = (scaledLengthX / 2) * Math.Cos(angle);
            double newHalfLengthY = (scaledLengthY / 2) * Math.Sin(angle);

            // Recalculate the start and end points based on the scaled length and fixed angle
            double startX = centerX - newHalfLengthX + offsetX;
            double startY = centerY - newHalfLengthY + offsetY;
            double endX = centerX + newHalfLengthX + offsetX;
            double endY = centerY + newHalfLengthY + offsetY;

            GradientPreview.StartPoint = new Point(startX, startY);
            GradientPreview.EndPoint = new Point(endX, endY);
        }
    }

    void SliderAngle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if(updatingFromAngle) return;

        updatingFromAngle = true;

        double angle = SliderAngle.Value * Math.PI / 180;
        double length = Math.Sqrt(Math.Pow(EndXSlider.Value - StartXSlider.Value, 2) + Math.Pow(EndYSlider.Value - StartYSlider.Value, 2));

        StartXSlider.Value = 0.5 - (length / 2) * Math.Cos(angle);
        StartYSlider.Value = 0.5 - (length / 2) * Math.Sin(angle);
        EndXSlider.Value = 0.5 + (length / 2) * Math.Cos(angle);
        EndYSlider.Value = 0.5 + (length / 2) * Math.Sin(angle);

        UpdateGradientPreview();
        updatingFromAngle = false;
    }

    void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if(updatingFromAngle || updatingFromOffset) return;

        UpdateGradientPreview();

        if(StartXSlider != null && StartYSlider != null && EndXSlider != null && EndYSlider != null)
        {
            double dx = EndXSlider.Value - StartXSlider.Value;
            double dy = EndYSlider.Value - StartYSlider.Value;
            double angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            angle = (angle + 360) % 360;

            SliderAngle.Value = angle;
        }
    }

    void SliderOffsetX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if(updatingFromOffset) return;
        updatingFromOffset = true;
        UpdateGradientPreview();
        updatingFromOffset = false;
    }

    void SliderOffsetY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if(updatingFromOffset) return;
        updatingFromOffset = true;
        UpdateGradientPreview();
        updatingFromOffset = false;
    }

    void AddStop_Click(object sender, RoutedEventArgs e)
    {
        GradientStops.Add(new GradientStop(Colors.Gray, 0.5));
        UpdateGradientPreview();
    }

    void RemoveStop_Click(object sender, RoutedEventArgs e)
    {
        if(StopsListBox.SelectedItem is GradientStop selectedStop)
        {
            GradientStops.Remove(selectedStop);
            UpdateGradientPreview();
        }
    }

    void EditStop_Click(object sender, RoutedEventArgs e)
    {
        if((sender as FrameworkElement)?.DataContext is GradientStop selectedStop)
        {
            var colorPicker = new ColorPickerWindow
            {
                SelectedColor = selectedStop.Color
            };

            if(colorPicker.ShowDialog() == true)
            {
                SetStopColor(new SolidColorBrush(colorPicker.SelectedColor), selectedStop);
                UpdateGradientPreview();
            }
        }
    }

    void CopyStop_Click(object sender, RoutedEventArgs e)
    {
        if(sender is not Button button || button.Tag is not System.Windows.Shapes.Rectangle rectangle ||
                   rectangle.Fill is not SolidColorBrush brush) return;

        Clipboard.SetText(ColorUtil.ColorToHexString(brush.Color));
    }

    void PasteStop_Click(object sender, RoutedEventArgs e)
    {
        if(sender is not Button button || button.Tag is not System.Windows.Shapes.Rectangle rectangle ||
            rectangle.Fill is not SolidColorBrush brush) return;

        var color = ColorUtil.GetColorFromHex(Clipboard.GetText());
        if(color.HasValue)
        {
            brush.Color = color.Value;
            if((button.DataContext as GradientStop) is GradientStop selectedStop)
                SetStopColor(brush, selectedStop);

            UpdateGradientPreview();
        }
    }

    void SetStopColor(SolidColorBrush brush, GradientStop selectedStop)
    {
        int index = GradientStops.IndexOf(selectedStop);
        if(index != -1)
            GradientStops[index] = new GradientStop(brush.Color, selectedStop.Offset);
    }

    void ConfirmButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    void StopSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) => UpdateGradientPreview();

    void SliderScaleX_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if(updatingFromScale) return;
        updatingFromScale = true;
        ScaleX = e.NewValue;
        UpdateGradientPreview();
        updatingFromScale = false;
    }

    void SliderScaleY_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if(updatingFromScale) return;
        updatingFromScale = true;
        ScaleY = e.NewValue;
        UpdateGradientPreview();
        updatingFromScale = false;
    }
}