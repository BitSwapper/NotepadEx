using System.Windows;
using System.Windows.Input;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using NotepadEx.Util;
using Color = System.Windows.Media.Color;

namespace NotepadEx.MVVM.View;

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

    void OnWindowMouseMove(object sender, MouseEventArgs e)
    {
        if(WindowState == WindowState.Normal)
        {
            var position = e.GetPosition(this);
            WindowResizerUtil.ResizeWindow(this, position, 6, () => myColorPicker.UpdateColorFromSelectedColor());
            //Need Improvement / **Refactor
        }
    }
}
