using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace NotepadEx.Windows;

public partial class GradientPickerWindow : Window
{
    public ObservableCollection<GradientStop> GradientStops { get; set; }

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
}