using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using NotepadEx.Services.Interfaces;
using FontFamily = System.Windows.Media.FontFamily;

namespace NotepadEx.MVVM.View;

public partial class FontEditorWindow : Window
{
    private readonly IFontService _fontService;
    private FontSettings _workingCopy;
    CustomTitleBarViewModel _titleBarViewModel;
    public FontSettings CurrentFont
    {
        get => _workingCopy;
        set
        {
            _workingCopy = value;
            OnPropertyChanged(nameof(CurrentFont));
        }
    }

    public ObservableCollection<FontFamily> AvailableFonts { get; }

    public FontEditorWindow(IFontService fontService)
    {
        InitializeComponent();
        _fontService = fontService;

        // Create a working copy of current font settings
        _workingCopy = new FontSettings
        {
            FontFamily = _fontService.CurrentFont.FontFamily,
            FontSize = _fontService.CurrentFont.FontSize,
            FontStyle = _fontService.CurrentFont.FontStyle,
            FontWeight = _fontService.CurrentFont.FontWeight,
            IsUnderline = _fontService.CurrentFont.IsUnderline,
            IsStrikethrough = _fontService.CurrentFont.IsStrikethrough
        };

        AvailableFonts = _fontService.AvailableFonts;

        DataContext = this;
        _titleBarViewModel = CustomTitleBar.InitializeTitleBar(this, "Font Settings");
    }

    private void ApplyButton_Click(object sender, RoutedEventArgs e)
    {
        _fontService.ApplyFont(CurrentFont);
        Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e) => Close();

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
