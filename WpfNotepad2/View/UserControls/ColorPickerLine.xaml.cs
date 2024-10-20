using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NotepadEx.Theme;
using NotepadEx.Util;
using NotepadEx.ViewModels;
using NotepadEx.Windows;
using Application = System.Windows.Application;
using Color = System.Windows.Media.Color;
using LinearGradientBrush = System.Windows.Media.LinearGradientBrush;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
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
