using System.Windows;
using System.Windows.Controls;
using NotepadEx.MVVM.View.UserControls;
using NotepadEx.MVVM.ViewModels;

namespace NotepadEx.MVVM.View;

public partial class FindAndReplaceWindow : Window
{
    private TextBox targetTextBox;
    private int currentPosition = 0;

    CustomTitleBarViewModel titleBarViewModel;
    public CustomTitleBarViewModel TitleBarViewModel => titleBarViewModel;

    public FindAndReplaceWindow(TextBox textBox)
    {
        InitializeComponent();
        DataContext = this;
        titleBarViewModel = CustomTitleBar.InitializeTitleBar(this, "Find and Replace", showMinimize: true, showMaximize: false, isResizable: false);

        targetTextBox = textBox;
    }

    private void FindNextButton_Click(object sender, RoutedEventArgs e) => Find(true);

    private void FindPreviousButton_Click(object sender, RoutedEventArgs e) => Find(false);

    private void Find(bool forward)
    {
        string searchText = FindTextBox.Text;
        string content = targetTextBox.Text;

        if(string.IsNullOrEmpty(searchText) || string.IsNullOrEmpty(content))
            return;

        StringComparison comparison = MatchCaseCheckBox.IsChecked == true ?
                StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        int startIndex = forward ?
                currentPosition + (currentPosition < content.Length ? 1 : 0) :
                currentPosition - 1;

        if(startIndex < 0) startIndex = content.Length - 1;
        if(startIndex >= content.Length) startIndex = 0;

        int foundIndex = forward ?
                content.IndexOf(searchText, startIndex, comparison) :
                content.LastIndexOf(searchText, startIndex, comparison);

        // Wrap around if not found
        if(foundIndex == -1)
        {
            foundIndex = forward ?
                content.IndexOf(searchText, 0, comparison) :
                content.LastIndexOf(searchText, content.Length - 1, comparison);
        }

        if(foundIndex != -1)
        {
            targetTextBox.Focus();
            targetTextBox.Select(foundIndex, searchText.Length);
            targetTextBox.ScrollToLine(GetLineIndexFromPosition(targetTextBox, foundIndex));
            currentPosition = foundIndex;
        }
        else
        {
            MessageBox.Show($"Cannot find \"{searchText}\"", "Find and Replace",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private void ReplaceButton_Click(object sender, RoutedEventArgs e)
    {
        if(targetTextBox.SelectionLength > 0 &&
            targetTextBox.SelectedText.Equals(FindTextBox.Text,
                MatchCaseCheckBox.IsChecked == true ?
                    StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase))
        {
            int currentPos = targetTextBox.SelectionStart;
            targetTextBox.SelectedText = ReplaceTextBox.Text;
            currentPosition = currentPos;
            Find(true);
        }
        else
        {
            Find(true);
        }
    }

    private void ReplaceAllButton_Click(object sender, RoutedEventArgs e)
    {
        string searchText = FindTextBox.Text;
        string replaceText = ReplaceTextBox.Text;
        StringComparison comparison = MatchCaseCheckBox.IsChecked == true ?
                StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        if(string.IsNullOrEmpty(searchText))
            return;

        string content = targetTextBox.Text;
        string newContent = content;
        int replacements = 0;

        if(MatchCaseCheckBox.IsChecked == true)
        {
            while(newContent.IndexOf(searchText, comparison) != -1)
            {
                newContent = newContent.Replace(searchText, replaceText);
                replacements++;
            }
        }
        else
        {
            int index = 0;
            while((index = newContent.IndexOf(searchText, index, comparison)) != -1)
            {
                newContent = newContent.Remove(index, searchText.Length)
                                     .Insert(index, replaceText);
                index += replaceText.Length;
                replacements++;
            }
        }

        if(replacements > 0)
        {
            targetTextBox.Text = newContent;
            MessageBox.Show($"Replaced {replacements} occurrence(s)", "Find and Replace",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    private int GetLineIndexFromPosition(TextBox textBox, int position)
    {
        int lineIndex = 0;
        int currentPos = 0;

        string[] lines = textBox.Text.Split(new[] { Environment.NewLine },
                StringSplitOptions.None);

        foreach(string line in lines)
        {
            if(currentPos + line.Length >= position)
                return lineIndex;

            currentPos += line.Length + Environment.NewLine.Length;
            lineIndex++;
        }

        return lineIndex;
    }
}
