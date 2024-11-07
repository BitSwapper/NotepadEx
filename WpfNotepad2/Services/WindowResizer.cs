using System.Windows;
using System.Windows.Interop;
using NotepadEx.Util;
using Cursors = System.Windows.Input.Cursors;
using Point = System.Windows.Point;

namespace NotepadEx.Services;

public class WindowResizer
{
    WindowState _previousState;

    public void ToggleMaximizeState(Window window)
    {
        if(window.WindowState == WindowState.Maximized)
        {
            window.WindowState = _previousState;
        }
        else
        {
            _previousState = window.WindowState;
            window.WindowState = WindowState.Maximized;
        }
    }

    public void HandleMouseMove(Window window, Point position)
    {
        WindowResizerUtil.ResizeWindow(window, position, 8);
        return;
    }
}
