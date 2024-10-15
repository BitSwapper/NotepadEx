﻿using System.Drawing.Drawing2D;
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

    void Button_Click(object sender, System.Windows.RoutedEventArgs e)
    {
        //begin editing
    }

    void ButtonRandomize_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            Application.Current.Resources[path] = ColorUtil.GetRandomLinearGradientBrush(180);
            
        }
        catch(Exception ex) { MessageBox.Show(ex.Message); }
    }

    private void ColorPicker_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
        e.Handled = true;
    }
}
