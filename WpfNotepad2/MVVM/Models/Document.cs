using System.IO;

namespace NotepadEx.MVVM.Models;

public class Document
{
    private string _content = string.Empty;
    private int _selectionStart;
    private int _selectionLength;

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

    public string SelectedText
    {
        get => _selectionLength > 0 ? _content.Substring(_selectionStart, _selectionLength) : string.Empty;
    }

    public void DeleteSelected()
    {
        if(_selectionLength > 0)
        {
            _content = _content.Remove(_selectionStart, _selectionLength);
            _selectionLength = 0;
            IsModified = true;
        }
    }

    public void InsertText(string text)
    {
        if(_selectionLength > 0)
        {
            DeleteSelected();
        }
        _content = _content.Insert(_selectionStart, text);
        _selectionStart += text.Length;
        IsModified = true;
    }

    public void UpdateSelection(int start, int length)
    {
        _selectionStart = start;
        _selectionLength = length;
    }
}

