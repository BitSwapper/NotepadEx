using System.IO;
using System.Windows;

namespace NotepadEx.MVVM.Models;
public class Document
{
    private string content = string.Empty;
    private string[] cachedLines = Array.Empty<string>();

    public int SelectionStart { get; set; }
    public int SelectionLength { get; set; }

    public string Content
    {
        get => content;
        set
        {
            content = value;
            UpdateCachedLines();
            IsModified = true;
        }
    }

    public string FilePath { get; set; } = string.Empty;
    public bool IsModified { get; set; }
    public string FileName => string.IsNullOrEmpty(FilePath) ? string.Empty : Path.GetFileName(FilePath);
    public string SelectedText => SelectionLength > 0 ? content.Substring(SelectionStart, SelectionLength) : string.Empty;

    // New properties for line and caret tracking
    public int CurrentLineNumber => GetLineNumberFromPosition(SelectionStart);
    public int CaretIndex => SelectionStart;
    public int CaretLineIndex => GetColumnIndexInLine(SelectionStart);
    public int TotalLines => cachedLines.Length;

    private void UpdateCachedLines()
    {
        cachedLines = content.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
    }

    public int GetLineNumberFromPosition(int position)
    {
        if(position < 0 || string.IsNullOrEmpty(content))
            return 1;

        int lineCount = 1;
        int currentPos = 0;

        foreach(string line in cachedLines)
        {
            if(position <= currentPos + line.Length)
                return lineCount;

            currentPos += line.Length + Environment.NewLine.Length;
            lineCount++;
        }

        return lineCount;
    }

    public int GetColumnIndexInLine(int position)
    {
        if(position <= 0)
            return 0;

        int currentPos = 0;
        foreach(string line in cachedLines)
        {
            int lineLength = line.Length + Environment.NewLine.Length;
            if(position <= currentPos + lineLength)
                return position - currentPos;

            currentPos += lineLength;
        }

        return 0;
    }

    public string GetCurrentLine()
    {
        int lineNumber = CurrentLineNumber - 1; // Convert to 0-based index
        return lineNumber >= 0 && lineNumber < cachedLines.Length
            ? cachedLines[lineNumber]
            : string.Empty;
    }

    public void DeleteSelected()
    {
        if(SelectionLength > 0)
        {
            content = content.Remove(SelectionStart, SelectionLength);
            UpdateCachedLines();
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
        content = content.Insert(SelectionStart, text);
        UpdateCachedLines();
        SelectionStart += text.Length;
        IsModified = true;
    }

    // Helper method to get position at specific line and column
    public int GetPosition(int lineNumber, int columnIndex)
    {
        if(lineNumber <= 0 || lineNumber > cachedLines.Length)
            return 0;

        int position = 0;
        for(int i = 0; i < lineNumber - 1; i++)
        {
            position += cachedLines[i].Length + Environment.NewLine.Length;
        }

        return position + Math.Min(columnIndex, cachedLines[lineNumber - 1].Length);
    }

    // New method to cut the current line
    public void CutLine()
    {
        // Get the current line number (0-based index)
        int lineIndex = CurrentLineNumber - 1;

        if(lineIndex >= 0 && lineIndex < cachedLines.Length)
        {
            // Calculate the start position of the current line
            int lineStartPosition = GetPosition(CurrentLineNumber, 0);

            // Calculate the length of the line including the line ending
            int lineLength = cachedLines[lineIndex].Length;

            // Add the length of the line ending if it's not the last line
            if(lineIndex < cachedLines.Length - 1)
            {
                lineLength += Environment.NewLine.Length;
            }

            // Get the text of the current line (without line ending)
            string lineText = cachedLines[lineIndex];

            // Copy to clipboard
            Clipboard.SetText(lineText);

            // Remove the line from the content
            content = content.Remove(lineStartPosition, lineLength);
            UpdateCachedLines();

            // Update selection and modification state
            SelectionStart = lineStartPosition;
            SelectionLength = 0;
            IsModified = true;
        }
    }
}