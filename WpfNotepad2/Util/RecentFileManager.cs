using System.Windows.Controls;

namespace WpfNotepad2.Util;

public static class RecentFileManager
{
    const int maxRecentsToTrack = 10;
    public static List<string> RecentFiles { get; private set; } = new();

    public static void LoadRecentFilesFromSettings()
    {
        string recentFilesString = Properties.Settings.Default.RecentFiles;
        if(!string.IsNullOrEmpty(recentFilesString))
            RecentFiles = recentFilesString.Split(',').ToList();
    }

    public static void AddRecentFile(string filePath, MenuItem FileDropDown, Action SaveSettings)
    {
        if(!string.IsNullOrEmpty(filePath) && !RecentFiles.Contains(filePath))
        {
            RecentFiles.Insert(0, filePath);
            if(RecentFiles.Count > maxRecentsToTrack)
                RecentFiles.RemoveAt(RecentFiles.Count - 1);
            SaveSettings();
            PopulateRecentFilesMenu(FileDropDown);
        }
    }

    public static void PopulateRecentFilesMenu(MenuItem FileDropDown)
    {
        MenuItem openRecentMenuItem = (MenuItem)FileDropDown.FindName("MenuItem_OpenRecent");// .Items[3];
        openRecentMenuItem.Items.Clear();
        foreach(string file in RecentFiles)
        {
            MenuItem menuItem = new MenuItem();
            menuItem.Header = file;
            openRecentMenuItem.Items.Add(menuItem);
        }
    }
}

