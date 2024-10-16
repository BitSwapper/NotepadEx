using System.Windows;
using Color = System.Windows.Media.Color;

namespace NotepadEx.Windows;

public partial class ColorPickerWindow : Window
{
    public Color SelectedColor
    {
        get => myColorPicker.SelectedColor; set => myColorPicker.SelectedColor = value;
    }

    public ColorPickerWindow()
    {
        InitializeComponent();
        TitleBar.Init(this, "RGBA Color Picker");
        myColorPicker.OnWindowCancel += OnCancel;
        myColorPicker.OnWindowConfirm += OnConfirm;
    }

    private void OnCancel()
    {
        DialogResult = false;
        Close();
    }

    private void OnConfirm()
    {
        DialogResult = true;
        Close();
    }
}
