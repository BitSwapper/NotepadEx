using System.IO;

namespace NotepadEx.MVVM.Models;

public class Document
{
    string _content = string.Empty;
    
    public int SelectionStart { get; set; }
    public int SelectionLength { get; set; }

    public string Content
    {
        get => _content;
        set
        {
            _content = value;
            IsModified = true;
        }
    }

    public string FilePath { get; set; } = string.Empty;
    public bool IsModified { get; set; }
    public string FileName => string.IsNullOrEmpty(FilePath) ? string.Empty : Path.GetFileName(FilePath);

    public string SelectedText => SelectionLength > 0 ? _content.Substring(SelectionStart, SelectionLength) : string.Empty;

    public void DeleteSelected()
    {
        if(SelectionLength > 0)
        {
            _content = _content.Remove(SelectionStart, SelectionLength);
            SelectionLength = 0;
            IsModified = true;
        }
    }

    public void InsertText(string text)
    {
        if(SelectionLength > 0)
        {
            DeleteSelected();
        }
        _content = _content.Insert(SelectionStart, text);
        SelectionStart += text.Length;
        IsModified = true;
    }
}

