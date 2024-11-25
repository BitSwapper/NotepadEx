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
using NotepadEx.Services.Interfaces;

namespace NotepadEx.MVVM.View;
/// <summary>
/// Interaction logic for FontEditorWindow.xaml
/// </summary>
public partial class FontEditorWindow : Window
{
    public FontEditorWindow(IFontService fontService)
    {
        InitializeComponent();
    }
}
