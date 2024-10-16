using System.Windows;
using Color = System.Windows.Media.Color;

namespace NotepadEx.Windows;

public partial class ColorPickerWindow : Window
{
    public Color SelectedColor => myColorPicker.SelectedColor;
    public ColorPickerWindow()
    {
        InitializeComponent();
        myColorPicker.OnWindowCancel += OnClose;
        myColorPicker.OnWindowConfirm += OnClose;
    }

    public void OnClose()
    {
        Close();
    }
}
