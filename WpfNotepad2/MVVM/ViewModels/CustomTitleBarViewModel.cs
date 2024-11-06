using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotepadEx.MVVM.View;
using NotepadEx.Util;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows;

namespace NotepadEx.MVVM.ViewModels
{
    public class CustomTitleBarViewModel : ViewModelBase
    {
        private Window _window;
        private string _titleText;
        private BitmapImage _imageSource;
        private bool _isResizeable;
        private bool _showMinimizeButton = true;
        private bool _showMaximizeButton = true;
        private bool _showCloseButton = true;

        public string TitleText
        {
            get => _titleText;
            set => SetProperty(ref _titleText, value);
        }

        public BitmapImage ImageSource
        {
            get => _imageSource;
            set => SetProperty(ref _imageSource, value);
        }

        public bool ShowMinimizeButton
        {
            get => _showMinimizeButton;
            set => SetProperty(ref _showMinimizeButton, value);
        }

        public bool ShowMaximizeButton
        {
            get => _showMaximizeButton;
            set => SetProperty(ref _showMaximizeButton, value);
        }

        public bool ShowCloseButton
        {
            get => _showCloseButton;
            set => SetProperty(ref _showCloseButton, value);
        }

        public ICommand MinimizeCommand { get; }
        public ICommand MaximizeCommand { get; }
        public ICommand CloseCommand { get; }
        public ICommand TitleBarMouseDownCommand { get; }

        public CustomTitleBarViewModel(Window window, bool isResizeable = true)
        {
            _window = window;
            _isResizeable = isResizeable;

            MinimizeCommand = new RelayCommand(ExecuteMinimize);
            MaximizeCommand = new RelayCommand(ExecuteMaximize);
            CloseCommand = new RelayCommand(ExecuteClose);
            TitleBarMouseDownCommand = new RelayCommand<MouseButtonEventArgs>(ExecuteTitleBarMouseDown);
        }

        private void ExecuteMinimize()
        {
            _window.WindowState = WindowState.Minimized;
        }

        private void ExecuteMaximize()
        {
            _window.WindowState = _window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        }

        private void ExecuteClose()
        {
            _window.Close();
        }

        private void ExecuteTitleBarMouseDown(MouseButtonEventArgs e)
        {
            if(!_isResizeable || (_isResizeable && e.GetPosition(_window).Y > UIConstants.ResizeBorderWidth / 2))
                _window.DragMove();
        }

        public void Initialize(string titleText, bool showMinimize = true, bool showMaximize = true, bool showClose = true)
        {
            TitleText = titleText;
            ShowMinimizeButton = showMinimize;
            ShowMaximizeButton = showMaximize;
            ShowCloseButton = showClose;
        }
    }
}
