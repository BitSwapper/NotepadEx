﻿namespace NotepadEx;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;
using NotepadEx.Services;

public partial class MainWindow : Window
{
    readonly WindowResizer _resizer;
    readonly MainWindowViewModel _viewModel;
    readonly ThemeService _themeService;

    public MainWindow()
    {
        var windowService = new WindowService(this);
        var settingsService = new SettingsService();
        var documentService = new DocumentService();
        var _themeService = new ThemeService(Application.Current);

        InitializeComponent();
        DataContext = _viewModel = new MainWindowViewModel(windowService, settingsService, documentService, _themeService, MenuItemFileDropDown);

        var titleBarViewModel = new CustomTitleBarViewModel(this);
        CustomTitleBar.InitializeTitleBar(ref titleBarViewModel, this, "NotepadEx");
        _viewModel.TitleBarViewModel = titleBarViewModel;

        _resizer = new WindowResizer();

        StateChanged += OnWindowStateChanged;
        MouseMove += OnWindowMouseMove;
    }

    void OnWindowStateChanged(object sender, EventArgs e)
    {
        if(WindowState != WindowState.Minimized)
        {
            _viewModel.UpdateWindowState(WindowState);
        }
    }

    void OnBorderMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if(e.ClickCount == 2)
            _resizer.ToggleMaximizeState(this);
        else if(e.LeftButton == MouseButtonState.Pressed)
            DragMove();
    }

    void OnWindowMouseMove(object sender, MouseEventArgs e)
    {
        if(_viewModel.IsAutoHideMenuBarEnabled)
        {
            var position = e.GetPosition(this);
            _viewModel.HandleMouseMovement(position.Y);
        }

        if(WindowState == WindowState.Normal)
        {
            var position = e.GetPosition(this);
            _resizer.HandleMouseMove(this, position);
        }
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        _resizer.Initialize(this);
    }

    void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {

    }

    void Window_StateChanged(object sender, EventArgs e)
    {

    }

    private void MenuItem_OpenRecent_Click(object sender, RoutedEventArgs e)
    {

        //Old Code from OnClick Event before we made commands
        MenuItem menuItem = (MenuItem)sender;
        MenuItem subMenuItem = (MenuItem)e.OriginalSource;
        bool hasTextChangedSinceSave = false; //**Refactor / Fix

        var path = (string)subMenuItem.Header;
        if(path != "...")
            _viewModel.LoadDocument(path);
    }
}