using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Controls;
using NotepadEx.Util;

namespace NotepadEx.View.UserControls;

public partial class ColorPickerLine : UserControl
{
    string path;
    Color themeColor;
    public ColorPickerLine() => InitializeComponent();

    public void SetText(string text) => txtThemeName.Text = text;
    public void SetPath(string path) => this.path = path;

    void ButtonRandomize_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Application.Current.Resources[path] = ColorUtil.GetRandomLinearGradientBrush(180);
        }
        catch(Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void ColorPicker_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<System.Windows.Media.Color?> e)
    {
        try
        {
            Application.Current.Resources[path] = new System.Windows.Media.SolidColorBrush(e.NewValue.Value);
        }
        catch(Exception ex) { MessageBox.Show(ex.Message); }
    }
}
