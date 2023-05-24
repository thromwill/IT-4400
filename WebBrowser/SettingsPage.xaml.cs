using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace M14
{
    public partial class SettingsPage : Page
    {
        private AppData appData;
        private MainWindow mainWindow;
        public event EventHandler BackButtonPressed;
        public event PropertyChangedEventHandler? PropertyChanged;

        public SettingsPage(AppData appData, MainWindow mainWindow)
        {
            // Initialize settings page
            InitializeComponent();
            DataContext = appData;
            this.appData = appData;
            this.mainWindow = mainWindow;
        }

        // Takes user to url from history element
        public void SettingsHistory_Click(object sender, RoutedEventArgs e)
        {
            // Get url from button
            Button? button = sender as Button;
            if (button != null)
            {
                string? url = button.CommandParameter as string;
                SettingsBack_Click(sender, e);

                if (url != null)
                {
                    // Open url in new tab
                    mainWindow.GoToUrlFromHistory(url);
                }
            }
        }

        // Takes user back to main page
        private void SettingsBack_Click(object sender, RoutedEventArgs e)
        {
            BackButtonPressed?.Invoke(this, EventArgs.Empty);
        }

        // Displays help message
        private void SettingsHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Search is a simple web browser.\r\n\r\nFeatures include:\r\n\r\n- Clean UI\r\n- Basic functionality like back, forward, and refresh buttons\r\n- Tabbed browsing\r\n- Browser history and sorting\r\n- Bookmarks\r\n- Loading bar\r\n- Keyboard controls\r\n- All features included with WebView2 like printing/downloading\r\n\r\nHelpful controls:\r\n\r\nButtons:\r\n\t◁\t: previous page\r\n\t▷\t: next page\r\n\t↻\t: refresh\r\n\t+\t: new tab\r\n\t-\t: close current tab\r\n\t∴\t: settings\r\n\r\nKeyboard:\r\n\tCtrl + left\t: previous page\r\n\tCtrl + right\t: next page\r\n\tCtrl + R\t\t: refresh\r\n\tCtrl + P\t\t: print\r\n\tCtrl + T\t\t: new tab\r\n\tCtrl + Del\t: close current tab\r\n\tCtrl + B\t\t: new bookmark\r\n\tF8\t\t: settings", "\nAbout", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Takes user to Github page in new tab
        private void SettingsGithub_Click(object sender, RoutedEventArgs e)
        {
            // Go back to main page
            SettingsBack_Click(sender, e);

            // Open link in new tab
            mainWindow.GoToUrl("https://github.com/thromwill/INFOTC-4400");
        }

        // Takes user to Linkedin page in new tab
        private void SettingsLinkedIn_Click(object sender, RoutedEventArgs e)
        {
            SettingsBack_Click(sender, e);
            mainWindow.GoToUrl("https://www.linkedin.com/in/willthrom/");
        }

        // HISTORY SORTING FUNCTIONS
        public void SettingsSortAlphabetical_Click(object sender, RoutedEventArgs e)
        {
            // Sort the list
            appData.History.Sort((str1, str2) =>
            {
                return str1.Split(' ')[1].CompareTo(str2.Split(' ')[1]);
            });

            // Reload settings page with updated AppData
            SettingsBack_Click(sender, e);
            mainWindow.Settings_Click(sender, e);
        }

        public void SettingsSortReverseAlphabetical_Click(object sender, RoutedEventArgs e)
        {
            appData.History.Sort((str1, str2) =>
            {
                return -(str1.Split(' ')[1].CompareTo(str2.Split(' ')[1]));
            });

            SettingsBack_Click(sender, e);
            mainWindow.Settings_Click(sender, e);

        }
        public void SettingsSortDateTimeAscending_Click(object sender, RoutedEventArgs e)
        {
            appData.History.Sort((str1, str2) =>
            {
                string[] substrings1 = str1.Split(' ');
                string[] substrings2 = str2.Split(' ');

                // Compare the dates first
                int dateComparison = substrings1[0].CompareTo(substrings2[0]);
                if (dateComparison != 0)
                {
                    return dateComparison;
                }

                // If the dates are equal, compare the times
                return substrings1[2].CompareTo(substrings2[2]);
            });

            SettingsBack_Click(sender, e);
            mainWindow.Settings_Click(sender, e);
        }

        public void SettingsSortDateTimeDescending_Click(object sender, RoutedEventArgs e)
        {
            appData.History.Sort((str1, str2) =>
            {
                string[] substrings1 = str1.Split(' ');
                string[] substrings2 = str2.Split(' ');

                int dateComparison = substrings1[0].CompareTo(substrings2[0]);
                if (dateComparison != 0)
                {
                    return -dateComparison;
                }

                return -(substrings1[2].CompareTo(substrings2[2]));
            });

            SettingsBack_Click(sender, e);
            mainWindow.Settings_Click(sender, e);
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}