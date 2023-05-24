using System.Windows;
using System.Windows.Input;

namespace M14
{
    interface Search
    {
        void ForwardButton_Click(object sender, RoutedEventArgs e);
        void BackButton_Click(object sender, RoutedEventArgs e);
        void RefreshButton_Click(object sender, RoutedEventArgs e);
        void AddNewTabButton_Click(object sender, RoutedEventArgs e);
        void RemoveTabButton_Click(object sender, RoutedEventArgs e);
        void AddBookmark_Click(object sender, RoutedEventArgs e);
        void RemoveBookmark_Click(object sender, RoutedEventArgs e);
        void Settings_Click(object sender, RoutedEventArgs e);
        void GoToUrl(object sender, RoutedEventArgs e);
        void GoToUrlFromHistory(string displayString);
        void GoToUrl(string url);
        void GoToBookMark(object sender, RoutedEventArgs e);
        void HandleCtrlKeyPress(object sender, KeyEventArgs e);
        void HandleStandardKeyPress(object sender, KeyEventArgs e);
    }
}
