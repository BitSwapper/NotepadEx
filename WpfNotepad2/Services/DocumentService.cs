using System.Drawing.Printing;
using System.IO;
using NotepadEx.MVVM.Models;
using NotepadEx.Services.Interfaces;

namespace NotepadEx.Services;

public class DocumentService : IDocumentService
{
    public void LoadDocument(string filePath, Document document)
    {
        if(!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }

        document.Content = File.ReadAllText(filePath);
        document.FilePath = filePath;
        document.IsModified = false;
    }

    public void SaveDocument(Document document)
    {
        File.WriteAllText(document.FilePath, document.Content);
        document.IsModified = false;
    }

    public void PrintDocument(Document document)
    {
        var printDialog = new System.Windows.Forms.PrintDialog();
        if(printDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            var printDoc = new PrintDocument();
            printDoc.PrintPage += (sender, e) =>
            {
                e.Graphics.DrawString(document.Content,
                    new System.Drawing.Font("Arial", 12),
                    System.Drawing.Brushes.Black,
                    new System.Drawing.RectangleF(100, 100, 700, 1000));
            };
            printDoc.Print();
        }
    }
}
