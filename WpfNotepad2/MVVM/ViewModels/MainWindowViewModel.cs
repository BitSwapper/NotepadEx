using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NotepadEx.MVVM.Models;
using NotepadEx.Properties;
using NotepadEx.Services.Interfaces;
using NotepadEx.Util;

namespace NotepadEx.MVVM.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    readonly Document document;
    readonly IWindowService windowService;
    readonly IDocumentService documentService;
    readonly WindowState prevWindowState;
    readonly IThemeService themeService;
    string statusText;
    double menuBarHeight;
    double infoBarHeight;
    bool isMenuBarEnabled;

    public ICommand NewCommand { get; private set; }
    public ICommand OpenCommand { get; private set; }
    public ICommand OpenRecentCommand { get; private set; }
    public ICommand SaveCommand { get; private set; }
    public ICommand SaveAsCommand { get; private set; }
    public ICommand PrintCommand { get; private set; }
    public ICommand ExitCommand { get; private set; }
    public ICommand ToggleWordWrapCommand { get; private set; }
    public ICommand ToggleMenuBarCommand { get; private set; }
    public ICommand ToggleInfoBarCommand { get; private set; }
    public ICommand CopyCommand { get; private set; }
    public ICommand CutCommand { get; private set; }
    public ICommand PasteCommand { get; private set; }
    public ICommand ChangeThemeCommand { get; private set; }
    public ICommand OpenThemeEditorCommand { get; private set; }

    public ObservableCollection<ThemeInfo> AvailableThemes => themeService.AvailableThemes;

    public string DocumentContent
    {
        get => document.Content;
        set
        {
            if(document.Content != value)
            {
                document.Content = value;
                document.IsModified = true;
                OnPropertyChanged();
                UpdateTitle();
            }
        }
    }

    public string StatusText
    {
        get => statusText;
        set => SetProperty(ref statusText, value);
    }

    public double MenuBarHeight
    {
        get => menuBarHeight;
        set => SetProperty(ref menuBarHeight, value);
    }

    public double InfoBarHeight
    {
        get => infoBarHeight;
        set => SetProperty(ref infoBarHeight, value);
    }

    public bool IsMenuBarEnabled
    {
        get => isMenuBarEnabled;
        set => SetProperty(ref isMenuBarEnabled, value);
    }

    public TextWrapping TextWrappingMode => Settings.Default.TextWrapping ? TextWrapping.Wrap : TextWrapping.NoWrap;

    public bool IsAutoHideMenuBarEnabled => Settings.Default.MenuBarAutoHide;


    CustomTitleBarViewModel _titleBarViewModel;
    public CustomTitleBarViewModel TitleBarViewModel { get => _titleBarViewModel; set => _titleBarViewModel = value; }

    MenuItem menuItemFileDropdown;
    Action SaveSettings;

    public MainWindowViewModel(IWindowService windowService, IDocumentService documentService, IThemeService themeService, MenuItem menuItemFileDropdown, Action SaveSettings)
    {
        this.windowService = windowService;
        this.documentService = documentService;
        this.themeService = themeService;
        this.menuItemFileDropdown = menuItemFileDropdown;
        this.SaveSettings = SaveSettings;
        document = new Document();

        //_Settings.Default.MenuBarAutoHide = false;
        //SaveSettings();

        InitializeCommands();
        UpdateMenuBarVisibility(Settings.Default.MenuBarAutoHide);
        UpdateInfoBarVisibility(Settings.Default.InfoBarVisible);
        this.themeService.LoadCurrentTheme();
        LoadRecentFiles();

        OnPropertyChanged("AvailableThemes");
    }

    void LoadRecentFiles()
    {
        RecentFileManager.LoadRecentFilesFromSettings();
        RecentFileManager.PopulateRecentFilesMenu(menuItemFileDropdown);
    }


    void AddRecentFile(string filePath) => RecentFileManager.AddRecentFile(filePath, menuItemFileDropdown, SaveSettings);

    //void OpenRecent(object parameter)
    //{
    //    if(parameter is string filePath)
    //    {
    //        if(!PromptToSaveChanges()) return;
    //        LoadDocument(filePath);
    //    }
    //}

    public void LoadDocument(string filePath)
    {
        try
        {
            documentService.LoadDocument(filePath, document);
            UpdateTitle();
            UpdateStatusBar();
            AddRecentFile(filePath);
            OnPropertyChanged("DocumentContent");
        }
        catch(Exception ex)
        {
            windowService.ShowDialog(
                $"Error loading file: {ex.Message}",
                "Error");
        }
    }

    void OnOpenThemeEditor() => themeService.OpenThemeEditor();

    void InitializeCommands()
    {
        NewCommand = new RelayCommand(NewDocument);
        OpenCommand = new RelayCommand(OpenDocument);
        SaveCommand = new RelayCommand(SaveDocument);
        SaveAsCommand = new RelayCommand(SaveDocumentAs);
        PrintCommand = new RelayCommand(PrintDocument);
        ExitCommand = new RelayCommand(() => Application.Current.Shutdown());
        ToggleWordWrapCommand = new RelayCommand(ToggleWordWrap);
        ToggleMenuBarCommand = new RelayCommand(ToggleMenuBar);
        ToggleInfoBarCommand = new RelayCommand(ToggleInfoBar);
        CopyCommand = new RelayCommand(Copy);
        CutCommand = new RelayCommand(Cut);
        PasteCommand = new RelayCommand(Paste);
        ChangeThemeCommand = new RelayCommand<ThemeInfo>(OnThemeChange);
        OpenThemeEditorCommand = new RelayCommand(OnOpenThemeEditor);
    }

    void OnThemeChange(ThemeInfo theme)
    {
        if(theme != null)
            themeService.ApplyTheme(theme.Name);
    }


    void NewDocument()
    {
        if(!PromptToSaveChanges()) return;

        document.Content = string.Empty;
        document.FilePath = string.Empty;
        document.IsModified = false;
        UpdateTitle();
        OnPropertyChanged("DocumentContent");
    }

    void OpenDocument()
    {
        if(!PromptToSaveChanges()) return;

        var dialog = new System.Windows.Forms.OpenFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
        };

        if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            LoadDocument(dialog.FileName);
        }
    }

    void SaveDocumentAs()
    {
        var dialog = new System.Windows.Forms.SaveFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            DefaultExt = ".txt"
        };

        if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            document.FilePath = dialog.FileName;
            SaveDocument();
        }
    }

    void ToggleWordWrap()
    {
        Settings.Default.TextWrapping = !Settings.Default.TextWrapping;
        SaveSettings();
        OnPropertyChanged(nameof(TextWrappingMode));
    }

    void UpdateMenuBarVisibility(bool autoHide)
    {
        MenuBarHeight = autoHide ? 0 : UIConstants.MenuBarHeight;
        IsMenuBarEnabled = !autoHide;
    }

    void UpdateInfoBarVisibility(bool visible) => InfoBarHeight = visible ? UIConstants.InfoBarHeight : 0;

    void UpdateTitle()
    {
        var title = string.IsNullOrEmpty(document.FileName) ? "NotepadEx" : $"NotepadEx | {document.FileName}{(document.IsModified ? "*" : "")}";
        _titleBarViewModel.TitleText = title;
    }

    public bool PromptToSaveChanges()
    {
        if(!document.IsModified) return true;

        var bResult = windowService.ShowConfirmDialog("Do you want to save changes?", "Save Changes");

        if(bResult)
            SaveDocument();

        return true;
    }

    void SaveDocument()
    {
        if(string.IsNullOrEmpty(document.FilePath))
        {
            SaveDocumentAs();
            return;
        }

        try
        {
            documentService.SaveDocument(document);
            UpdateTitle();
            UpdateStatusBar();
        }
        catch(Exception ex)
        {
            windowService.ShowDialog($"Error saving file: {ex.Message}", "Error");
        }
    }

    void PrintDocument()
    {
        try
        {
            documentService.PrintDocument(document);
        }
        catch(Exception ex)
        {
            windowService.ShowDialog($"Error printing document: {ex.Message}", "Error");
        }
    }

    void ToggleMenuBar()
    {
        Settings.Default.MenuBarAutoHide = !Settings.Default.MenuBarAutoHide;
        UpdateMenuBarVisibility(Settings.Default.MenuBarAutoHide);
        SaveSettings();
    }

    void ToggleInfoBar()
    {
        Settings.Default.InfoBarVisible = !Settings.Default.InfoBarVisible;
        UpdateInfoBarVisibility(Settings.Default.InfoBarVisible);
        SaveSettings();
    }

    void UpdateStatusBar()
    {
        var lineCount = document.Content.Split('\n').Length;
        var charCount = document.Content.Length;
        StatusText = $"Lines: {lineCount} | Characters: {charCount}";
    }

    public void HandleMouseMovement(double mouseY)
    {
        if(Settings.Default.MenuBarAutoHide && mouseY < 2)
            UpdateMenuBarVisibility(false);
        else if(Settings.Default.MenuBarAutoHide && mouseY > UIConstants.MenuBarHeight)
            UpdateMenuBarVisibility(true);
    }

    public void UpdateWindowState(WindowState newState)
    {
        if(newState != WindowState.Minimized)
        {
            UpdateMenuBarVisibility(Settings.Default.MenuBarAutoHide);
            UpdateInfoBarVisibility(Settings.Default.InfoBarVisible);
        }
    }

    void Copy()
    {
        if(!string.IsNullOrEmpty(document.SelectedText))
            Clipboard.SetText(document.SelectedText);
    }

    void Cut()
    {
        if(!string.IsNullOrEmpty(document.SelectedText))
        {
            Clipboard.SetText(document.SelectedText);
            document.DeleteSelected();
            OnPropertyChanged(nameof(DocumentContent));
        }
    }

    void Paste()
    {
        if(Clipboard.ContainsText())
        {
            var text = Clipboard.GetText();
            document.InsertText(text);
            OnPropertyChanged(nameof(DocumentContent));
        }
    }
}