using System.Windows;
using System.Windows.Interop;
using Cursors = System.Windows.Input.Cursors;
using Point = System.Windows.Point;

namespace NotepadEx.Services;

public class WindowResizer
{
    private HwndSource _hwndSource;
    private WindowState _previousState;

    public void Initialize(Window window)
    {
        _hwndSource = PresentationSource.FromVisual(window) as HwndSource;
        if(_hwndSource != null)
        {
            _hwndSource.AddHook(WndProc);
        }
    }

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
        if(window.WindowState != WindowState.Normal)
            return;

        const int resizeBorder = 5;
        var cursor = Cursors.Arrow;

        // Determine resize cursor based on mouse position
        if(position.X <= resizeBorder)
        {
            cursor = position.Y <= resizeBorder ? Cursors.SizeNWSE :
                    position.Y >= window.Height - resizeBorder ? Cursors.SizeNESW :
                    Cursors.SizeWE;
        }
        else if(position.X >= window.Width - resizeBorder)
        {
            cursor = position.Y <= resizeBorder ? Cursors.SizeNESW :
                    position.Y >= window.Height - resizeBorder ? Cursors.SizeNWSE :
                    Cursors.SizeWE;
        }
        else if(position.Y <= resizeBorder || position.Y >= window.Height - resizeBorder)
        {
            cursor = Cursors.SizeNS;
        }

        window.Cursor = cursor;
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        // Handle window messages for custom chrome behavior
        switch(msg)
        {
            case 0x0084: // WM_NCHITTEST
                // Handle window resize areas
                handled = true;
                return HandleNCHitTest(lParam);
        }

        return IntPtr.Zero;
    }

    private IntPtr HandleNCHitTest(IntPtr lParam) =>
        // Implementation for custom window resize behavior
        // Returns appropriate values for different resize areas
        // This would be actual implementation with proper hit testing
        new IntPtr(1); // HTCLIENT for now
}
