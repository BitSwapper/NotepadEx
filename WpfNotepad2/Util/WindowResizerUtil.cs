using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Point = System.Windows.Point;

namespace NotepadEx.Util;
public class WindowResizerUtil
{
    bool isManuallyMaximized = false;
    double oldLeft;
    double oldTop;
    double oldWidth;
    double oldHeight;

    public void DoWindowMaximizedStateChange(Window window, WindowState prevWindowState)
    {
        if(prevWindowState == WindowState.Minimized) return;
        if(!isManuallyMaximized)
        {
            var screen = System.Windows.Forms.Screen.FromPoint(new System.Drawing.Point((int)window.Left, (int)window.Top)); //get the screen where the window is currently located
            var workingArea = screen.WorkingArea;

            oldLeft = window.Left;
            oldTop = window.Top;
            oldWidth = window.Width;
            oldHeight = window.Height;

            window.Left = workingArea.Left;
            window.Top = workingArea.Top;
            window.Width = workingArea.Width;
            window.Height = workingArea.Height;

            isManuallyMaximized = true;
        }
        else if(isManuallyMaximized)
        {
            window.Left = oldLeft;
            window.Top = oldTop;
            window.Width = oldWidth;
            window.Height = oldHeight;

            isManuallyMaximized = false;
        }
    }

    public static void ResizeWindow(Window window, Point position, int edgeThreshold)
    {
        double windowWidth = window.ActualWidth;
        double windowHeight = window.ActualHeight;

        if(position.X <= edgeThreshold && position.Y <= edgeThreshold)
            ResizeWindowInternal(ResizeDirection.TopLeft, Cursors.SizeNWSE);

        else if(position.X >= windowWidth - edgeThreshold && position.Y <= edgeThreshold)
            ResizeWindowInternal(ResizeDirection.TopRight, Cursors.SizeNESW);

        else if(position.X <= edgeThreshold && position.Y >= windowHeight - edgeThreshold)
            ResizeWindowInternal(ResizeDirection.BottomLeft, Cursors.SizeNESW);

        else if(position.X >= windowWidth - edgeThreshold && position.Y >= windowHeight - edgeThreshold)
            ResizeWindowInternal(ResizeDirection.BottomRight, Cursors.SizeNWSE);

        else if(position.X <= edgeThreshold)
            ResizeWindowInternal(ResizeDirection.Left, Cursors.SizeWE);

        else if(position.X >= windowWidth - edgeThreshold)
            ResizeWindowInternal(ResizeDirection.Right, Cursors.SizeWE);

        else if(position.Y <= edgeThreshold)
            ResizeWindowInternal(ResizeDirection.Top, Cursors.SizeNS);

        else if(position.Y >= windowHeight - edgeThreshold)
            ResizeWindowInternal(ResizeDirection.Bottom, Cursors.SizeNS);

        else
            window.Cursor = Cursors.Arrow;

        void ResizeWindowInternal(ResizeDirection direction, Cursor cursor)
        {
            window.Cursor = cursor;
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
