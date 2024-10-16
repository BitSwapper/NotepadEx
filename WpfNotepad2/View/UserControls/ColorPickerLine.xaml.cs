using System.Windows;
using System.Windows.Controls;
using NotepadEx.Util;
using NotepadEx.Windows;
using Color = System.Windows.Media.Color;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
namespace NotepadEx.View.UserControls;

public partial class ColorPickerLine : UserControl
{
    string path;
    Color themeColor;
    public Grid GridForImg => gridForImage;
    public ColorPickerLine() => InitializeComponent();

    public void SetText(string text) => txtThemeName.Text = text;
    public void SetPath(string path) => this.path = path;
    public void SetColorOrGradientType(bool isGradient)
    {
        rdBtnColor.IsChecked = !isGradient;
        rdBtnGradient.IsChecked = isGradient;
    }

    void ButtonRandomize_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var brush =  ColorUtil.GetRandomColorBrush(180);
            AppResourceUtil<SolidColorBrush>.TrySetResource(Application.Current, path, brush);
            gridForImage.Background = brush;
        }
        catch(Exception ex) { MessageBox.Show(ex.Message); }
    }


    private void ButtonEdit_Click(object sender, RoutedEventArgs e)
    {
        if(rdBtnColor.IsChecked == true)
        {
            ColorPickerWindow colorPickerWindow = new();
            colorPickerWindow.myColorPicker.SetInitialColor((gridForImage.Background as SolidColorBrush).Color);

            colorPickerWindow.myColorPicker.OnSelectedColorChanged += () =>
            {
                AppResourceUtil<SolidColorBrush>.TrySetResource(Application.Current, path, new SolidColorBrush(colorPickerWindow.SelectedColor));
            };

            themeColor = colorPickerWindow.SelectedColor = (gridForImage.Background as SolidColorBrush).Color;
            colorPickerWindow.ShowDialog();
            gridForImage.Background = new SolidColorBrush(colorPickerWindow.SelectedColor);
        }
        else
        {

        }
    }
}
