using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Windows;

namespace WpfNotepad2;
internal class DocumentHelper
{
    //void OpenDocument(string fileName = "")
    //{
    //    if(isTextChanged)
    //    {
    //        MessageBoxResult result = MessageBox.Show("Do you want to save changes before opening a new document?", appName, MessageBoxButton.YesNoCancel);
    //        if(result == MessageBoxResult.Yes)
    //            SaveDocument();
    //        else if(result == MessageBoxResult.Cancel)
    //            return;
    //    }

    //    if(string.IsNullOrEmpty(fileName))
    //    {
    //        OpenFileDialog openFileDialog = new OpenFileDialog();
    //        if(openFileDialog.ShowDialog() == true)
    //            fileName = openFileDialog.FileName;
    //        else
    //            return;
    //    }

    //    LoadDocument(fileName);
    //}

    //void LoadDocument(string fileName)
    //{
    //    txtEditor.Text = File.ReadAllText(fileName);
    //    currentFileName = fileName;
    //    isTextChanged = false;
    //    UpdateStatusBarInfo();
    //    Title = $"{appName} | " + Path.GetFileName(fileName);
    //    AddRecentFile(fileName);
    //}
}
