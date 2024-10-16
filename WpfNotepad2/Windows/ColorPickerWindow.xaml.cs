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
        myColorPicker.OnWindowCancel += OnClose;
        myColorPicker.OnWindowConfirm += OnClose;
    }

    public void OnClose()
    {
        Close();
    }
}
