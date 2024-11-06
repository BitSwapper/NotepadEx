using System.Windows;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using Color = System.Windows.Media.Color;

namespace NotepadEx.Windows;

public partial class ColorPickerWindow : Window
{
    CustomTitleBarViewModel _titleBarViewModel;
    public CustomTitleBarViewModel TitleBarViewModel => _titleBarViewModel;

    public Color SelectedColor
    {
        get => myColorPicker.SelectedColor; set => myColorPicker.SelectedColor = value;
    }

    public ColorPickerWindow()
    {
        InitializeComponent();
        DataContext = this;
        CustomTitleBar.InitializeTitleBar(ref _titleBarViewModel, this, "Color Picker");
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
