using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NotepadEx.MVVM.Behaviors;
using NotepadEx.MVVM.Models;
using NotepadEx.Properties;
using NotepadEx.Services.Interfaces;
using NotepadEx.Util;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;
namespace NotepadEx.MVVM.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    readonly Document document;
    readonly IWindowService windowService;
    readonly IDocumentService documentService;
    readonly WindowState prevWindowState;
    readonly IThemeService themeService;
    readonly TextBox textBox;
    readonly MenuItem menuItemFileDropdown;
    readonly Action SaveSettings;
    string statusText;
    double menuBarHeight;
    double infoBarHeight;
    bool isMenuBarEnabled;
    readonly ScrollManager _scrollManager;

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
    public ICommand InsertTabCommand { get; private set; }
    public ICommand OpenFileLocationCommand { get; private set; }

    public ICommand MouseMoveCommand { get; private set; }
    public ICommand ResizeCommand { get; private set; }
    public ICommand SelectionChangedCommand { get; private set; }
    public ICommand TextChangedCommand { get; private set; }
    public ICommand PreviewKeyDownCommand { get; private set; }
    public ICommand ScrollCommand { get; private set; }
    public ICommand ScrollBarDragCommand { get; private set; }
    public ICommand OpenRecentCommand { get; private set; }
    public ICommand MouseWheelCommand { get; private set; }
    public ObservableCollection<ThemeInfo> AvailableThemes => themeService.AvailableThemes;
    readonly ScrollBarBehavior scrollBarBehavior = new();

    public string CurrentThemeName
    {
        get => Settings.Default.ThemeName;
        set
        {
            Settings.Default.ThemeName = value;
            OnPropertyChanged();
        }
    }
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
                UpdateStatusBar();
            }
        }
    }

    public int SelectionStart
    {
        get => document.SelectionStart;
        set => document.SelectionStart = value;
    }

    public int SelectionLength
    {
        get => document.SelectionLength;
        set => document.SelectionLength = value;
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
    public CustomTitleBarViewModel TitleBarViewModel { get; set; }

    public MainWindowViewModel(IWindowService windowService, IDocumentService documentService, IThemeService themeService, MenuItem menuItemFileDropdown, TextBox textBox, Action SaveSettings)
    {
        this.windowService = windowService;
        this.documentService = documentService;
        this.themeService = themeService;
        this.menuItemFileDropdown = menuItemFileDropdown;
        this.textBox = textBox;
        this.SaveSettings = SaveSettings;
        document = new Document();

        _scrollManager = new ScrollManager(textBox);
        InitializeTextBoxEvents(textBox);

        InitializeCommands();
        UpdateMenuBarVisibility(Settings.Default.MenuBarAutoHide);
        UpdateInfoBarVisibility(Settings.Default.InfoBarVisible);
        this.themeService.LoadCurrentTheme();
        LoadRecentFiles();
        UpdateStatusBar();

        OnPropertyChanged("AvailableThemes");
    }

    void LoadRecentFiles()
    {
        RecentFileManager.LoadRecentFilesFromSettings();
        RecentFileManager.PopulateRecentFilesMenu(menuItemFileDropdown);
    }

    void AddRecentFile(string filePath) => RecentFileManager.AddRecentFile(filePath, menuItemFileDropdown, SaveSettings);

    void OnOpenThemeEditor() => themeService.OpenThemeEditor();

    void InitializeCommands()
    {
        NewCommand = new RelayCommand(NewDocument);
        OpenCommand = new RelayCommand(OpenDocument);
        SaveCommand = new RelayCommand(SaveDocument);
        SaveAsCommand = new RelayCommand(SaveDocumentAs);
        PrintCommand = new RelayCommand(PrintDocument);
        //ExitCommand = new RelayCommand(ExitApp);
        ToggleWordWrapCommand = new RelayCommand(ToggleWordWrap);
        ToggleMenuBarCommand = new RelayCommand(ToggleMenuBar);
        ToggleInfoBarCommand = new RelayCommand(ToggleInfoBar);
        CopyCommand = new RelayCommand(Copy);
        CutCommand = new RelayCommand(Cut);
        PasteCommand = new RelayCommand(Paste);
        ChangeThemeCommand = new RelayCommand<ThemeInfo>(OnThemeChange);
        OpenThemeEditorCommand = new RelayCommand(OnOpenThemeEditor);
        InsertTabCommand = new RelayCommand(InsertTab);
        OpenFileLocationCommand = new RelayCommand(OpenFileLocation);

        MouseMoveCommand = new RelayCommand<double>(HandleMouseMovement);
        ResizeCommand = new RelayCommand<Point>(p => HandleWindowResize(Application.Current.MainWindow, p));
        SelectionChangedCommand = new RelayCommand<RoutedEventArgs>(HandleSelectionChanged);
        TextChangedCommand = new RelayCommand<TextChangedEventArgs>(HandleTextChanged);
        PreviewKeyDownCommand = new RelayCommand<KeyEventArgs>(HandlePreviewKeyDown);
        ScrollCommand = new RelayCommand<MouseWheelEventArgs>(HandleScroll);
        ScrollBarDragCommand = new RelayCommand<MouseButtonEventArgs>(HandleScrollBarDrag);
        OpenRecentCommand = new RelayCommand<RoutedEventArgs>(HandleOpenRecent);
        MouseWheelCommand = new RelayCommand<MouseWheelEventArgs>(HandleMouseWheel);
    }

    void OnThemeChange(ThemeInfo theme)
    {
        if(theme != null)
        {
            themeService.ApplyTheme(theme.Name);
            CurrentThemeName = theme.Name;
        }
    }

    void OpenDocument()
    {
        if(!PromptToSaveChanges()) return;

        var dialog = new System.Windows.Forms.OpenFileDialog
        {
            Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
        };

        if(dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            LoadDocument(dialog.FileName);
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
        TitleBarViewModel.TitleText = title;
    }

    public bool PromptToSaveChanges()
    {
        if(!document.IsModified)
            return true;

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

    //void ExitApp()
    //{
    //    Application.Current.Shutdown();
    //}

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

    public void UpdateSelection(int selectionStart, int selectionLength)
    {
        SelectionStart = selectionStart;
        SelectionLength = selectionLength;
    }

    public void ToggleMaximizeState(Window window) => WindowResizerUtil.ToggleMaximizeState(window);

    public void HandleWindowResize(Window window, Point position) => WindowResizerUtil.ResizeWindow(window, position);

    public void OpenRecentFile(string path)
    {
        if(!PromptToSaveChanges())
            return;
        LoadDocument(path);
    }



    void Copy()
    {
        if(!string.IsNullOrEmpty(document.SelectedText))
            Clipboard.SetText(document.SelectedText);
        else
            Clipboard.SetText(document.GetCurrentLine());
    }

    void CutLine() => document.CutLine(textBox);

    void Paste()
    {
        if(Clipboard.ContainsText())
        {
            var text = Clipboard.GetText();
            var caretPosition = textBox.SelectionStart + text.Length;
            textBox.SelectedText = text;
            textBox.SelectionStart = caretPosition;
        }
    }

    void InsertTab()
    {
        var caretPosition = textBox.SelectionStart + 4;
        textBox.SelectedText = "    ";
        textBox.SelectionStart = caretPosition;
    }

    void Cut()
    {
        if(!string.IsNullOrEmpty(document.SelectedText))
        {
            Clipboard.SetText(document.SelectedText);
            textBox.SelectedText = "";
        }
        else
            CutLine();
    }

    void LoadDocument(string filePath)
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

    void NewDocument()
    {
        if(!PromptToSaveChanges())
            return;

        document.Content = string.Empty;
        document.FilePath = string.Empty;
        document.IsModified = false;
        UpdateTitle();
        OnPropertyChanged("DocumentContent");
    }

    void OpenFileLocation()
    {
        var path = document.FilePath;
        if(File.Exists(path))
            Process.Start("explorer.exe", $"/select,\"{path}\"");
    }

    void InitializeTextBoxEvents(TextBox textBox)
    {
        textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
        textBox.TextChanged += TextBox_TextChanged;
        textBox.SelectionChanged += TextBox_SelectionChanged;
    }

    void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        bool isNavigationKey = e.Key == Key.Left || e.Key == Key.Right ||
                             e.Key == Key.Up || e.Key == Key.Down ||
                             e.Key == Key.Home || e.Key == Key.End ||
                             e.Key == Key.PageUp || e.Key == Key.PageDown;

        if(isNavigationKey)
        {
            _scrollManager.HandleNavigationKey(e.Key, Keyboard.Modifiers);
        }
    }

    void TextBox_TextChanged(object sender, TextChangedEventArgs e) => _scrollManager.HandleTextChanged();

    void TextBox_SelectionChanged(object sender, RoutedEventArgs e)
    {
        _scrollManager.HandleSelectionChanged();

        if(sender is TextBox textBox)
        {
            UpdateSelection(textBox.SelectionStart, textBox.SelectionLength);
        }
    }

    public void HandleScrollBarDrag(Rectangle rectangle, TextBox textBox, MouseButtonEventArgs e) => scrollBarBehavior.StartDrag(rectangle, textBox, e);

    //public void HandleMouseScroll(object sender, MouseWheelEventArgs e)
    //{
    //    var grid = (Grid)sender;
    //    var scrollViewer = grid.TemplatedParent as ScrollViewer;
    //    if(scrollViewer != null)
    //    {
    //        var verticalScrollBar = grid.FindName("PART_VerticalScrollBar") as ScrollBar;
    //        var newOffset = scrollViewer.VerticalOffset - (e.Delta / 3.0);

    //        newOffset = Math.Max(0, Math.Min(newOffset, scrollViewer.ScrollableHeight));

    //        scrollViewer.ScrollToVerticalOffset(newOffset);
    //        if(verticalScrollBar != null)
    //            verticalScrollBar.Value = newOffset;

    //        e.Handled = true;
    //    }
    //}

    public void HandleMouseScroll(object sender, MouseWheelEventArgs e) => _scrollManager.HandleMouseWheel(sender, e);

    void HandleOpenRecent(RoutedEventArgs e)
    {
        if(e.OriginalSource is MenuItem menuItem && menuItem.Header is string path && path != "...")
        {
            OpenRecentFile(path);
        }
    }

    void HandleSelectionChanged(RoutedEventArgs e)
    {
        if(e.Source is TextBox textBox)
        {
            UpdateSelection(textBox.SelectionStart, textBox.SelectionLength);
        }
    }

    void HandleTextChanged(TextChangedEventArgs e) => _scrollManager.HandleTextChanged();

    void HandlePreviewKeyDown(KeyEventArgs e)
    {
        bool isNavigationKey = e.Key == Key.Left || e.Key == Key.Right ||
                             e.Key == Key.Up || e.Key == Key.Down ||
                             e.Key == Key.Home || e.Key == Key.End ||
                             e.Key == Key.PageUp || e.Key == Key.PageDown;

        if(isNavigationKey)
        {
            _scrollManager.HandleNavigationKey(e.Key, Keyboard.Modifiers);
        }
    }

    void HandleScroll(MouseWheelEventArgs e) => _scrollManager.HandleMouseWheel(e.Source, e);

    void HandleScrollBarDrag(MouseButtonEventArgs e)
    {
        if(e.Source is Rectangle rectangle && e.LeftButton == MouseButtonState.Pressed)
        {
            scrollBarBehavior.StartDrag(rectangle, textBox, e);
        }
    }

    void HandleMouseWheel(MouseWheelEventArgs e)
    {
        if(e.Source is Grid grid && grid.TemplatedParent is ScrollViewer scrollViewer)
        {
            _scrollManager?.HandleMouseWheel(scrollViewer, e);
        }
    }

    public void Cleanup()
    {
        // Cleanup commands
        (MouseMoveCommand as IDisposable)?.Dispose();
        (ResizeCommand as IDisposable)?.Dispose();
        (SelectionChangedCommand as IDisposable)?.Dispose();
        (TextChangedCommand as IDisposable)?.Dispose();
        (PreviewKeyDownCommand as IDisposable)?.Dispose();
        (ScrollCommand as IDisposable)?.Dispose();
        (ScrollBarDragCommand as IDisposable)?.Dispose();
        (NewCommand as IDisposable)?.Dispose();
        (OpenCommand as IDisposable)?.Dispose();
        (SaveCommand as IDisposable)?.Dispose();
        (SaveAsCommand as IDisposable)?.Dispose();
        (PrintCommand as IDisposable)?.Dispose();
        (ExitCommand as IDisposable)?.Dispose();
        (ToggleWordWrapCommand as IDisposable)?.Dispose();
        (ToggleMenuBarCommand as IDisposable)?.Dispose();
        (ToggleInfoBarCommand as IDisposable)?.Dispose();
        (CopyCommand as IDisposable)?.Dispose();
        (CutCommand as IDisposable)?.Dispose();
        (PasteCommand as IDisposable)?.Dispose();
        (ChangeThemeCommand as IDisposable)?.Dispose();
        (OpenThemeEditorCommand as IDisposable)?.Dispose();
        (InsertTabCommand as IDisposable)?.Dispose();
        (OpenFileLocationCommand as IDisposable)?.Dispose();
        (OpenRecentCommand as IDisposable)?.Dispose();

        // Cleanup event handlers if any were directly subscribed
        if(textBox != null)
        {
            textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            textBox.TextChanged -= TextBox_TextChanged;
            textBox.SelectionChanged -= TextBox_SelectionChanged;
        }

        // Cleanup services if they implement IDisposable
        (windowService as IDisposable)?.Dispose();
        (documentService as IDisposable)?.Dispose();
        (themeService as IDisposable)?.Dispose();

        // Cleanup ScrollManager
        (_scrollManager as IDisposable)?.Dispose();

        // Clear collections if any
        AvailableThemes?.Clear();

        // Cleanup document if needed
        (document as IDisposable)?.Dispose();

        // Save settings one last time
        SaveSettings?.Invoke();
    }
}