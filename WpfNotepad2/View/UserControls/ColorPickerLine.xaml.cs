﻿using System.Drawing.Drawing2D;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using NotepadEx.Util;
using NotepadEx.Windows;
using Color = System.Windows.Media.Color;
namespace NotepadEx.View.UserControls;

public partial class ColorPickerLine : UserControl
{
    string path;
    Color themeColor;
    public Grid GridForImg => gridForImage;
    public ColorPickerLine() => InitializeComponent();

    public void SetText(string text) => txtThemeName.Text = text;
    public void SetPath(string path) => this.path = path;

    void ButtonRandomize_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var brush =  ColorUtil.GetRandomColorBrush(180);
            Application.Current.Resources[path] = brush;
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
                Application.Current.Resources[path] = new System.Windows.Media.SolidColorBrush(colorPickerWindow.SelectedColor);
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
