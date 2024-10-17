using System.Windows.Media;
using Brush = System.Windows.Media.Brush;
using Color = System.Windows.Media.Color;
namespace NotepadEx.View;

public class ColorPickerViewModel : ViewModelBase
{
    private Brush _backgroundColor = new SolidColorBrush(Color.FromArgb(255, 0, 255, 255));
    public Brush BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            if(_backgroundColor != value)
            {
                _backgroundColor = value;
                OnPropertyChanged();
            }
        }
    }
}