using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotepadEx.MVVM.Models;

namespace NotepadEx.Services.Interfaces
{
    public interface IDocumentService
    {
        void LoadDocument(string filePath, Document document);
        void SaveDocument(Document document);
        void PrintDocument(Document document);
    }
}
