﻿using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using NotepadEx.MVVM.Models;
using NotepadEx.Services.Interfaces;
using NotepadEx.Util;

namespace NotepadEx.MVVM.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly Document _document;
    private readonly AppSettings _settings;
    private readonly IWindowService _windowService;
    private readonly ISettingsService _settingsService;
    private readonly IDocumentService _documentService;
    private readonly WindowState _prevWindowState;
    private readonly IThemeService _themeService;
    private string _statusText;
    private double _menuBarHeight;
    private double _infoBarHeight;
    private bool _isMenuBarEnabled;



    public ICommand NewCommand { get; private set; }
    public ICommand OpenCommand { get; private set; }
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

    public ObservableCollection<ThemeInfo> AvailableThemes => _themeService.AvailableThemes;

    public string DocumentContent
    {
        get => _document.Content;
        set
        {
            if(_document.Content != value)
            {
                _document.Content = value;
                _document.IsModified = true;
                OnPropertyChanged();
                UpdateTitle();
            }
        }
    }

    public string StatusText
    {
        get => _statusText;
        set => SetProperty(ref _statusText, value);
    }

    public double MenuBarHeight
    {
        get => _menuBarHeight;
        set => SetProperty(ref _menuBarHeight, value);
    }

    public double InfoBarHeight
    {
        get => _infoBarHeight;
        set => SetProperty(ref _infoBarHeight, value);
    }

    public bool IsMenuBarEnabled
    {
        get => _isMenuBarEnabled;
        set => SetProperty(ref _isMenuBarEnabled, value);
    }

    public TextWrapping TextWrappingMode => _settings.TextWrapping ? TextWrapping.Wrap : TextWrapping.NoWrap;

    public bool IsAutoHideMenuBarEnabled => _settings.MenuBarAutoHide;


    CustomTitleBarViewModel _titleBarViewModel;
    public CustomTitleBarViewModel TitleBarViewModel { get => _titleBarViewModel; set => _titleBarViewModel = value; }

    public MainWindowViewModel(IWindowService windowService, ISettingsService settingsService, IDocumentService documentService, IThemeService themeService)
    {
        _windowService = windowService;
        _settingsService = settingsService;
        _documentService = documentService;
        _themeService = themeService;

        _document = new Document();
        _settings = _settingsService.LoadSettings();

        InitializeCommands();
        UpdateMenuBarVisibility(_settings.MenuBarAutoHide);
        UpdateInfoBarVisibility(_settings.InfoBarVisible);
        _themeService.LoadCurrentTheme();
    }

    private void OnThemeChange(ThemeInfo theme)
    {
        if(theme != null)
        {
            _themeService.ApplyTheme(theme.Name);
        }
    }

    private void OnOpenThemeEditor() => _themeService.OpenThemeEditor();

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

    void NewDocument()
    {
        if(PromptToSaveChanges())
        {
            _document.Content = string.Empty;
            _document.FilePath = string.Empty;
            _document.IsModified = false;
            UpdateTitle();
        }
    }

    private void OpenDocument()
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


    private void SaveDocumentAs()
    {
        var dialog = new System.Windows.Forms.SaveFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*",
            DefaultExt = ".txt"
        };

        if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            _document.FilePath = dialog.FileName;
            SaveDocument();
        }
    }

    private void ToggleWordWrap()
    {
        _settings.TextWrapping = !_settings.TextWrapping;
        SaveSettings();
        OnPropertyChanged(nameof(TextWrappingMode));
    }

    private void UpdateMenuBarVisibility(bool autoHide)
    {
        MenuBarHeight = autoHide ? 0 : UIConstants.MenuBarHeight;
        IsMenuBarEnabled = !autoHide;
    }

    private void UpdateInfoBarVisibility(bool visible) => InfoBarHeight = visible ? UIConstants.InfoBarHeight : 0;

    private void UpdateTitle()
    {
        var title = string.IsNullOrEmpty(_document.FileName) ?
                "NotepadEx" :
                $"NotepadEx | {_document.FileName}{(_document.IsModified ? "*" : "")}";
        // Notify title bar via event or service
    }

    private bool PromptToSaveChanges()
    {
        if(!_document.IsModified) return true;

        var result = _windowService.ShowConfirmDialog(
                "Do you want to save changes?",
                "Save Changes");

        if(result)
        {
            SaveDocument();
        }

        return true;
    }

    private void LoadDocument(string filePath)
    {
        try
        {
            _documentService.LoadDocument(filePath, _document);
            UpdateTitle();
            UpdateStatusBar();
            RecentFileManager.AddRecentFile(filePath, null, null);
        }
        catch(Exception ex)
        {
            _windowService.ShowDialog(
                $"Error loading file: {ex.Message}",
                "Error");
        }
    }

    private void SaveDocument()
    {
        if(string.IsNullOrEmpty(_document.FilePath))
        {
            SaveDocumentAs();
            return;
        }

        try
        {
            _documentService.SaveDocument(_document);
            UpdateTitle();
            UpdateStatusBar();
            //return true;
        }
        catch(Exception ex)
        {
            _windowService.ShowDialog(
                $"Error saving file: {ex.Message}",
                "Error");
            //return false;
        }
    }

    private void PrintDocument()
    {
        try
        {
            _documentService.PrintDocument(_document);
        }
        catch(Exception ex)
        {
            _windowService.ShowDialog(
                $"Error printing document: {ex.Message}",
                "Error");
        }
    }

    private void ToggleMenuBar()
    {
        _settings.MenuBarAutoHide = !_settings.MenuBarAutoHide;
        UpdateMenuBarVisibility(_settings.MenuBarAutoHide);
        SaveSettings();
    }

    private void ToggleInfoBar()
    {
        _settings.InfoBarVisible = !_settings.InfoBarVisible;
        UpdateInfoBarVisibility(_settings.InfoBarVisible);
        SaveSettings();
    }

    private void SaveSettings() => _settingsService.SaveSettings(_settings);

    private void UpdateStatusBar()
    {
        var lineCount = _document.Content.Split('\n').Length;
        var charCount = _document.Content.Length;
        StatusText = $"Lines: {lineCount} | Characters: {charCount}";
    }

    public void HandleMouseMovement(double mouseY)
    {
        if(_settings.MenuBarAutoHide && mouseY < 2)
        {
            UpdateMenuBarVisibility(false);
        }
        else if(_settings.MenuBarAutoHide && mouseY > UIConstants.MenuBarHeight)
        {
            UpdateMenuBarVisibility(true);
        }
    }

    public void UpdateWindowState(WindowState newState)
    {
        // Handle window state changes
        if(newState != WindowState.Minimized)
        {
            // Update UI elements based on window state
            UpdateMenuBarVisibility(_settings.MenuBarAutoHide);
            UpdateInfoBarVisibility(_settings.InfoBarVisible);
        }
    }

    // Clipboard Operations
    private void Copy()
    {
        if(!string.IsNullOrEmpty(_document.SelectedText))
        {
            Clipboard.SetText(_document.SelectedText);
        }
    }

    private void Cut()
    {
        if(!string.IsNullOrEmpty(_document.SelectedText))
        {
            Clipboard.SetText(_document.SelectedText);
            _document.DeleteSelected();
            OnPropertyChanged(nameof(DocumentContent));
        }
    }

    private void Paste()
    {
        if(Clipboard.ContainsText())
        {
            var text = Clipboard.GetText();
            _document.InsertText(text);
            OnPropertyChanged(nameof(DocumentContent));
        }
    }

    // Event handlers for document changes
    private void OnDocumentChanged()
    {
        UpdateTitle();
        UpdateStatusBar();
    }
}