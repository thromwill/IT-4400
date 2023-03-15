using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
namespace M8
{
    public partial class SettingsPage : Page
    {
        private AppData appData;
        public event EventHandler BackButtonPressed;
        public event PropertyChangedEventHandler? PropertyChanged;

        public SettingsPage(AppData appData)
        {
            InitializeComponent();
            this.appData = appData;
            DataContext = appData;
            FontMenu.SelectedIndex = 3;
            FontStyleMenu.SelectedIndex = 0;
            FontWeightMenu.SelectedIndex = 2;
        }

        private void FontMenu_SelectionChanged(object sender, EventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)FontMenu.SelectedItem;
            appData.UpdateTextBoxFont(selectedItem.Content.ToString());
        }

        private void FontStyleMenu_SelectionChanged(object sender, EventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)FontStyleMenu.SelectedItem;
            appData.UpdateTextBoxFontStyle(selectedItem.Content.ToString());
        }

        private void FontWeightMenu_SelectionChanged(object sender, EventArgs e)
        {
            ComboBoxItem selectedItem = (ComboBoxItem)FontWeightMenu.SelectedItem;
            appData.UpdateTextBoxFontWeight(selectedItem.Content.ToString());
        }

        public void SettingsBack_Click(object sender, RoutedEventArgs e)
        {
            BackButtonPressed?.Invoke(this, EventArgs.Empty);
        }

        private void SettingsHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This is a text editor. You can use it like Notepad.", "\nAbout", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SettingsGithub_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start https://github.com/thromwill/INFOTC-4400",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening URL: {ex.Message}");
            }
        }

        private void SettingsLinkedIn_Click(object sender, EventArgs e)
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo
                {
                    FileName = "cmd",
                    Arguments = $"/c start https://www.linkedin.com/in/willthrom/",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                };
                Process.Start(processStartInfo);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening URL: {ex.Message}");
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
