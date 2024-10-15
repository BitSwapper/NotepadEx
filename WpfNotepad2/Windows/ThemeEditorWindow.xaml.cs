using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfNotepad2.Util;

namespace WpfNotepad2.Windows
{
    /// <summary>
    /// Interaction logic for ThemeEditorWindow.xaml
    /// </summary>
    public partial class ThemeEditorWindow : Window
    {
        WindowState prevWindowState;
        WindowResizer resizer;
        public ThemeEditorWindow()
        {
            InitializeComponent();
            resizer = new WindowResizer();
            ThemeEditorTitleBar.Init(this, "Theme Editor", Minimize_Click, null!, Close_Click);
        }

        void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
                resizer.DoWindowMaximizedStateChange(this, prevWindowState);
        }

        void Border_MouseMove(object sender, MouseEventArgs e)
        {
            var position = e.GetPosition(this);
            WindowResizer.ResizeWindow(this, position, Constants.ResizeBorderWidth);
        }

        void Minimize_Click(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;

        void Close_Click(object sender, RoutedEventArgs e) => Close();   
    }
}
