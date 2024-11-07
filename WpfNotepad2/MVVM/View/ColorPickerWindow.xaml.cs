using System.Windows;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using Color = System.Windows.Media.Color;

namespace NotepadEx.MVVM.View;

public partial class ColorPickerWindow : Window
{
    CustomTitleBarViewModel titleBarViewModel;
    public CustomTitleBarViewModel TitleBarViewModel => titleBarViewModel;

    public Color SelectedColor
    {
        get => myColorPicker.SelectedColor; set => myColorPicker.SelectedColor = value;
    }

    public ColorPickerWindow()
    {
        InitializeComponent();
        DataContext = this;
        CustomTitleBar.InitializeTitleBar(ref titleBarViewModel, this, "Color Picker", showMinimize: false, showMaximize: false);
        myColorPicker.OnWindowCancel += OnCancel;
        myColorPicker.OnWindowConfirm += OnConfirm;
    }

    void OnCancel()
    {
        DialogResult = false;
        Close();
    }

    void OnConfirm()
    {
        DialogResult = true;
        Close();
    }
}
