using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;
using Point = System.Windows.Point;
namespace NotepadEx.Windows;

public partial class GradientPickerWindow : Window
{
    public ObservableCollection<GradientStop> GradientStops { get; set; } = new();
    public LinearGradientBrush GradientBrush => GradientPreview;

    public GradientPickerWindow()
    {
        InitializeComponent();
        GradientStops = new ObservableCollection<GradientStop>();
        StopsListBox.ItemsSource = GradientStops;
        GradientStops.Add(new GradientStop(Colors.White, 0));
        GradientStops.Add(new GradientStop(Colors.Black, 1));
        UpdateGradientPreview();
    }

    void UpdateGradientPreview()
    {
        GradientPreview.GradientStops.Clear();

        foreach(var stop in GradientStops)
        {
            GradientPreview.GradientStops.Add(stop);
        }

        if(StartXSlider != null && StartYSlider != null && EndXSlider != null && EndYSlider != null)
        {
            GradientPreview.StartPoint = new Point(StartXSlider.Value, StartYSlider.Value);
            GradientPreview.EndPoint = new Point(EndXSlider.Value, EndYSlider.Value);
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("One or more sliders are null");
            System.Diagnostics.Debug.WriteLine($"StartXSlider: {StartXSlider?.Value}, StartYSlider: {StartYSlider?.Value}");
            System.Diagnostics.Debug.WriteLine($"EndXSlider: {EndXSlider?.Value}, EndYSlider: {EndYSlider?.Value}");
        }
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
                selectedStop.Color = colorPicker.SelectedColor;
                UpdateGradientPreview();
            }
        }
    }

    void PositionSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        UpdateGradientPreview();
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

    public string GetGradientString()
    {
        var startPoint = GradientPreview.StartPoint;
        var endPoint = GradientPreview.EndPoint;
        return $"StartPoint:{startPoint.X:F2},{startPoint.Y:F2};EndPoint:{endPoint.X:F2},{endPoint.Y:F2};";
    }
}