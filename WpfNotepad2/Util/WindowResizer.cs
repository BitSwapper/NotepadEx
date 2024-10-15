using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace WpfNotepad2.Util;
internal class WindowResizer
{
    public static void ResizeWindow(Window window, Point position, int edgeThreshold)
    {
        double windowWidth = window.ActualWidth;
        double windowHeight = window.ActualHeight;

        if(position.X <= edgeThreshold && position.Y <= edgeThreshold)
        {
            window.Cursor = Cursors.SizeNWSE;
            ResizeWindowInternal(ResizeDirection.TopLeft);
        }
        else if(position.X >= windowWidth - edgeThreshold && position.Y <= edgeThreshold)
        {
            window.Cursor = Cursors.SizeNESW;
            ResizeWindowInternal(ResizeDirection.TopRight);
        }
        else if(position.X <= edgeThreshold && position.Y >= windowHeight - edgeThreshold)
        {
            window.Cursor = Cursors.SizeNESW;
            ResizeWindowInternal(ResizeDirection.BottomLeft);
        }
        else if(position.X >= windowWidth - edgeThreshold && position.Y >= windowHeight - edgeThreshold)
        {
            window.Cursor = Cursors.SizeNWSE;
            ResizeWindowInternal(ResizeDirection.BottomRight);
        }
        else if(position.X <= edgeThreshold)
        {
            window.Cursor = Cursors.SizeWE;
            ResizeWindowInternal(ResizeDirection.Left);
        }
        else if(position.X >= windowWidth - edgeThreshold)
        {
            window.Cursor = Cursors.SizeWE;
            ResizeWindowInternal(ResizeDirection.Right);
        }
        else if(position.Y <= edgeThreshold)
        {
            window.Cursor = Cursors.SizeNS;
            ResizeWindowInternal(ResizeDirection.Top);
        }
        else if(position.Y >= windowHeight - edgeThreshold)
        {
            window.Cursor = Cursors.SizeNS;
            ResizeWindowInternal(ResizeDirection.Bottom);
        }
        else
        {
            window.Cursor = Cursors.Arrow;
        }

        void ResizeWindowInternal(ResizeDirection direction)
        {
            if(Mouse.LeftButton == MouseButtonState.Pressed)
            {
                HwndSource hwndSource = HwndSource.FromHwnd(new WindowInteropHelper(window).Handle);
                SendMessage(hwndSource.Handle, WM_SYSCOMMAND, (IntPtr)(SC_SIZE + (int)direction), IntPtr.Zero);
            }
        }
    }

    enum ResizeDirection
    {
        Left = 1,
        Right = 2,
        Top = 3,
        Bottom = 6,
        TopLeft = 4,
        TopRight = 5,
        BottomLeft = 7,
        BottomRight = 8
    }

    const int WM_SYSCOMMAND = 0x112;
    const int SC_SIZE = 0xF000;

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);
}
