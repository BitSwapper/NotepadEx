﻿using System.Windows;
using System.Windows.Controls;
using NotepadEx.Theme;
using NotepadEx.Util;
using NotepadEx.Windows;
using Application = System.Windows.Application;
using LinearGradientBrush = System.Windows.Media.LinearGradientBrush;
using SolidColorBrush = System.Windows.Media.SolidColorBrush;
namespace NotepadEx.View.UserControls;

public partial class ColorPickerLine : UserControl
{
    string path;
    ThemeObject themeObj;
    public Grid GridForImg => gridForImage;
    public ColorPickerViewModel ViewModel {  get; set; }
    public ColorPickerLine()
    {
        DataContext = ViewModel = new ColorPickerViewModel();
        InitializeComponent();
    }

    public void SetupThemeObj(ThemeObject obj, string themePath, string friendlyThemeName)
    {
        txtThemeName.Text = friendlyThemeName;
        path = themePath;
        if(obj == null)
        {
            rdBtnColor.IsChecked = true;
            return;
        }
        themeObj = obj;
        rdBtnColor.IsChecked = !obj.isGradient;
        rdBtnGradient.IsChecked = obj.isGradient;
    }

    void ButtonRandomize_Click(object sender, RoutedEventArgs e)
    {
        if(themeObj?.isGradient ?? false)
        {
            var brush =  ColorUtil.GetRandomLinearGradientBrush(180);
            AppResourceUtil<LinearGradientBrush>.TrySetResource(Application.Current, path, brush);
            gridForImage.Background = brush;
            themeObj.gradient = brush;
        }
        else
        {
            var brush =  ColorUtil.GetRandomColorBrush(180);
            AppResourceUtil<SolidColorBrush>.TrySetResource(Application.Current, path, brush);
            gridForImage.Background = brush;
            themeObj.color = brush.Color;
        }
    }


    void ButtonEdit_Click(object sender, RoutedEventArgs e)
    {
        if(!themeObj.isGradient)
        {
            ColorPickerWindow colorPickerWindow = new();
            colorPickerWindow.myColorPicker.SetInitialColor(themeObj.color.GetValueOrDefault());

            colorPickerWindow.myColorPicker.OnSelectedColorChanged += () =>
            {
                AppResourceUtil<SolidColorBrush>.TrySetResource(Application.Current, path, new SolidColorBrush(colorPickerWindow.SelectedColor));
                themeObj.color = colorPickerWindow.SelectedColor;
            };

            if(colorPickerWindow.ShowDialog() == true)
                gridForImage.Background = new SolidColorBrush(colorPickerWindow.SelectedColor);
        }
        else
        {
            GradientPickerWindow gradientPickerWindow = new();

            //      **This code shows our current gradient but breaks our sliders afterward..

            //if(gridForImage.Background is LinearGradientBrush gradientBrush)
            //{
            //    gradientPickerWindow.GradientStops.Clear();

            //    foreach(var stop in  gradientBrush.GradientStops)
            //        gradientPickerWindow.GradientStops.Add(stop);
            //}

            if(gradientPickerWindow.ShowDialog() == true)
            {
                gridForImage.Background = gradientPickerWindow.GradientBrush;
                AppResourceUtil<LinearGradientBrush>.TrySetResource(Application.Current, path, gradientPickerWindow.GradientBrush);
                themeObj.gradient = gradientPickerWindow.GradientBrush;
            }
        }
    }

    private void rdBtnColor_Checked(object sender, RoutedEventArgs e)
    {
        if(path != null)
            GridForImg.Background = AppResourceUtil<SolidColorBrush>.TryGetResource(Application.Current, path);

        if(themeObj != null)
            themeObj.isGradient = false;
    }

    private void rdBtnGradient_Checked(object sender, RoutedEventArgs e)
    {
        if(path != null)
            GridForImg.Background = AppResourceUtil<LinearGradientBrush>.TryGetResource(Application.Current, path);

        if(themeObj != null)
            themeObj.isGradient = true;
    }
}
