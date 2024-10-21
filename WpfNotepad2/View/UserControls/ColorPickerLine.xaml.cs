using System.Windows.Controls;
using NotepadEx.ViewModels;
namespace NotepadEx.View.UserControls;

public partial class ColorPickerLine : UserControl
{
    public ColorPickerLineViewModel ViewModel { get; set; }
    public ColorPickerLine()
    {
        DataContext = ViewModel = new ColorPickerLineViewModel();
        InitializeComponent();
    }
}
