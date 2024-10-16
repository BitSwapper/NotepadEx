using System.Drawing.Printing;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Microsoft.Win32;

namespace NotepadEx.Util;
internal class DocumentUtil
{
    public static void OpenDocument(Func<bool> SaveDocument, Action<string> LoadDocument, bool isTextChanged, string appName, string fileName = "")
    {
        if(isTextChanged)
        {
            MessageBoxResult result = MessageBox.Show("Do you want to save changes before opening a new document?", appName, MessageBoxButton.YesNoCancel);
            if(result == MessageBoxResult.Yes)
                SaveDocument();
            else if(result == MessageBoxResult.Cancel)
                return;
        }

        if(string.IsNullOrEmpty(fileName))
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(openFileDialog.ShowDialog() == true)
                fileName = openFileDialog.FileName;
            else
                return;
        }

        LoadDocument(fileName);
    }

    public static void PrintDocument(TextBox txtEditor)
    {
        PrintDialog printDialog = new PrintDialog();
        if(printDialog.ShowDialog() == true)
        {
            PrintDocument printDoc = new PrintDocument();
            System.Windows.Forms.PageSetupDialog pageSetupDialog = new System.Windows.Forms.PageSetupDialog();
            pageSetupDialog.Document = printDoc;

            pageSetupDialog.ShowDialog();
            PrintTicket printTicket = new PrintTicket();

            if(pageSetupDialog.PageSettings == null)
                return;

            printTicket.PageMediaSize = new PageMediaSize(pageSetupDialog.PageSettings.PaperSize.Width, pageSetupDialog.PageSettings.PaperSize.Height);
            printTicket.PageOrientation = pageSetupDialog.PageSettings.Landscape ? PageOrientation.Landscape : PageOrientation.Portrait;
            printDialog.PrintTicket = printTicket;

            FlowDocument document = new FlowDocument(new Paragraph(new Run(txtEditor.Text)));
            printDialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, "My Document");
            MessageBox.Show("Print Initiated " + DateTime.Now.ToShortTimeString());
        }
    }

    public static bool PromptToSaveChanges(bool isTextChanged, Func<bool> SaveDocument)
    {
        if(isTextChanged)
        {
            MessageBoxResult result = MessageBox.Show("You have unsaved changes. Would you like to save them before proceeding?", "Unsaved Changes", MessageBoxButton.YesNoCancel, MessageBoxImage.Warning);
            switch(result)
            {
                case MessageBoxResult.Yes:
                    return SaveDocument();
                case MessageBoxResult.No:
                    return true;
                case MessageBoxResult.Cancel:
                    return false;
            }
        }
        return true;
    }
}
