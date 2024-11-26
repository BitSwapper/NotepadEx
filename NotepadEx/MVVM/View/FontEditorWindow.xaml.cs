using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using NotepadEx.Services.Interfaces;
using FontFamily = System.Windows.Media.FontFamily;

namespace NotepadEx.MVVM.View;

public partial class FontEditorWindow : Window
{
    private readonly IFontService _fontService;
    private FontSettings _workingCopy;
    CustomTitleBarViewModel titleBarViewModel;
    public CustomTitleBarViewModel TitleBarViewModel => titleBarViewModel;
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
        DataContext = this;
        titleBarViewModel = CustomTitleBar.InitializeTitleBar(this, "Font Settings", showMaximize: false);

        _fontService = fontService;

        // Create a working copy of current font settings
        _workingCopy = new FontSettings
        {
            FontFamily = _fontService.CurrentFont.FontFamily,
            FontSize = _fontService.CurrentFont.FontSize,
            FontStyle = _fontService.CurrentFont.FontStyle,
            FontWeight = _fontService.CurrentFont.FontWeight,
        };

        AvailableFonts = _fontService.AvailableFonts;


        FontFamilyComboBox.Loaded += FontFamilyComboBox_Loaded;

        FontSizeTextBox.Text = _workingCopy.FontSize.ToString();
        FontStyleComboBox.SelectedValue = _workingCopy.FontStyle;
        FontWeightComboBox.SelectedValue = _workingCopy.FontWeight;
    }

    private void FontFamilyComboBox_Loaded(object sender, RoutedEventArgs e)
    {
        // Find the matching FontFamily object from AvailableFonts based on the Source property
        var matchingFontFamily = AvailableFonts.FirstOrDefault(f => f.Source == _workingCopy.FontFamily);
        if(matchingFontFamily != null)
        {
            FontFamilyComboBox.SelectedItem = matchingFontFamily;
        }
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
