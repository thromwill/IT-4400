using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Web.WebView2.Wpf;

namespace M14
{
    public partial class MainWindow : Window, Search
    {
        public AppData appData = new();
        public SettingsPage settingsPage;

        public MainWindow()
        {
            // Initialize MainWindow
            InitializeComponent();
            DataContext = appData;
            this.Icon = new BitmapImage(new Uri("pack://application:,,,/assets/chromium.ico"));
            settingsPage = new SettingsPage(appData, this);
        }

        // BASIC FUNCTIONALITY
        public void ForwardButton_Click(object sender, RoutedEventArgs e)
        {

            // Go forward to recent webpage if possible
            ((WebView2)((DockPanel)((TabItem)tabControl.SelectedItem).Content).Children[0]).CoreWebView2.GoForward();

        }

        public void BackButton_Click(object sender, RoutedEventArgs e)
        {

            // Go backward to recent webopage if possible
            ((WebView2)((DockPanel)((TabItem)tabControl.SelectedItem).Content).Children[0]).CoreWebView2.GoBack();

        }

        public void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                // Start the loading bar
                appData.Visibility = "Visible";
            });

            // Refresh current webpage
            ((WebView2)((DockPanel)((TabItem)tabControl.SelectedItem).Content).Children[0]).CoreWebView2.Reload();
        }

        // MANAGE TABBED BROWSING
        public void AddNewTabButton_Click(object sender, RoutedEventArgs e)
        {
            // Only allow 10 tabs
            if (tabControl.Items.Count == 10)
            {
                MessageBox.Show("Max tabs reached!");
                return;
            }

            // Create a new TabItem and add it to the TabControl
            TabItem newTabItem = new TabItem();
            newTabItem.Header = "New Tab";
            tabControl.Items.Add(newTabItem);

            // Create a WebView2 control for the tab
            WebView2 webView = new WebView2();
            webView.Name = "webView";
            webView.Source = new Uri("https://www.google.com/");
            webView.NavigationCompleted += WebView_NavigationCompleted;

            // Add the web view to the tab
            DockPanel panel = new DockPanel();
            panel.Children.Add(webView);
            newTabItem.Content = panel;

            // Update the search bar text
            appData.Url = "https://www.google.com/";

            // Select the new tab
            tabControl.SelectedItem = newTabItem;

        }

        public void RemoveTabButton_Click(object sender, RoutedEventArgs e)
        {
            // Check for only 1 tab
            int count = 0;
            foreach(TabItem item in tabControl.Items) { count++; }
            if (count == 1)
            {
                return;
            }

            // Get the currently selected tab
            TabItem selectedTab = (TabItem)tabControl.SelectedItem;

            

            // Remove the tab
            if (selectedTab != null)
            {
                selectedTab.Template = null;
                tabControl.Items.Remove(selectedTab);
            }
        }

        public void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Avoid issues with null object
            var selectedItem = tabControl.SelectedItem as TabItem;
            if (selectedItem != null)
            {
                var dockPanel = selectedItem.Content as DockPanel;
                if (dockPanel != null && dockPanel.Children.Count > 0)
                {
                    var webView = dockPanel.Children[0] as WebView2;
                    if (webView != null && webView.CoreWebView2 != null)
                    {
                        // Update url when tab changes
                        appData.Url = webView.CoreWebView2.Source;
                    }
                }
            }
        }

        // MANAGE BOOKMARKS
        public void AddBookmark_Click(object sender, RoutedEventArgs e)
        {
            // Create popup window
            var popup = new Window
            {
                Title = "Add Bookmark",
                Width = 500,
                Height = 200,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            // Create input textbox
            var textBox = new TextBox
            {
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Center
            };


            // Create confirm button
            var confirmButton = new Button
            {
                Content = "Confirm",
                Width = 100,
                Height = 30,
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Bottom
            };

            // Wait for confirm click
            confirmButton.Click += (s, args) =>
            {
                // Add bookmark 
                string url = textBox.Text;
                appData.AddBookmark(url);

                // Close the window
                popup.Close();
            };

            // Create popup and set content with header, textbox, and button
            popup.Content = new StackPanel
            {
                Children =
        {
            new TextBlock
            {
                Text = "Enter the URL for the bookmark:",
                Margin = new Thickness(10),
                VerticalAlignment = VerticalAlignment.Center
            },
            textBox,
            confirmButton
        }
            };

            // Show the popup
            popup.ShowDialog();
        }

        public void RemoveBookmark_Click(object sender, RoutedEventArgs e)
        {
            // Undo the single-click action since function gets called with a doubleclick
            RemoveTabButton_Click(sender, e);

            // Get url from bookmarks list
            string url = (sender as Button).CommandParameter.ToString();

            // Remove bookmark
            appData.RemoveBookmark(url);

        }

        // MANAGE SETTINGS PAGE/BROWSER HISTORY
        public void Settings_Click(object sender, RoutedEventArgs e)
        {

            // Store current main page content
            var temp = this.Content;

            // Create new settings page instance with up to date AppData
            SettingsPage newSettingsPage = new SettingsPage(appData, this);

            // Set page content to the settings page
            this.Content = newSettingsPage;

            // When user returns from settings page...
            newSettingsPage.BackButtonPressed += (s, args) =>
            {
                // Update the current settings page
                settingsPage = newSettingsPage;

                // Reset the page content to the main page
                this.Content = temp;
            };
        }

        // SEARCH FUNCTIONS

        // Redirects current tab based on search bar input
        public void GoToUrl(object sender, RoutedEventArgs e)
        {

            Task.Run(() =>
            {
                // Start the loading bar
                appData.Visibility = "Visible";
            });

            // Remove focus from the textbox
            Keyboard.ClearFocus();

            // Get the content of the search bar
            string query = urlTextBox.Text;

            // If the user entered a .com type query but left off https://, add https://
            if (!(query.StartsWith("https://") || query.StartsWith("http://")) &&
                (query.Contains(".com") || query.Contains(".org") || query.Contains(".net") ||
                  query.Contains(".edu") || query.Contains(".gov") || query.Contains(".mil") ||
                  query.Contains(".biz") || query.Contains(".info") || query.Contains(".io") ||
                  query.Contains(".co")))
            {
                query = "https://" + query;
            }
            try
            {
                // If the query is a url, go to the url
                ((WebView2)((DockPanel)((TabItem)tabControl.SelectedItem).Content).Children[0]).CoreWebView2.Navigate(query);
                appData.Url = query;

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                // Otherwise, search for the query on Google
                string url = "https://www.google.com/search?q=" + Uri.EscapeDataString(query);
                ((WebView2)((DockPanel)((TabItem)tabControl.SelectedItem).Content).Children[0]).CoreWebView2.Navigate(url);
                appData.Url = url;
            }
        }

        // Opens link from browser history in new tab
        public void GoToUrlFromHistory(string displayString)
        {
            // Only allow 11 tabs
            if (tabControl.Items.Count > 10)
            {
                return;
            }

            Task.Run(() =>
            {
                // Start the loading bar
                appData.Visibility = "Visible";
            });

            // Strip url from display string in the form '01/01/2020 https://google.com 12:00:00 PM'
            string url = displayString.Split(' ')[1];

            // Create a new TabItem and add it to the TabControl
            TabItem newTabItem = new TabItem();
            newTabItem.Header = "New Tab";
            tabControl.Items.Add(newTabItem);

            // Create a WebView2 control for the tab
            WebView2 webView = new WebView2();
            webView.Name = "webView";
            webView.Source = new Uri(url);
            webView.NavigationCompleted += WebView_NavigationCompleted;

            // Add the web view to the tab
            DockPanel panel = new DockPanel();
            panel.Children.Add(webView);
            newTabItem.Content = panel;

            // Select the new tab
            tabControl.SelectedItem = newTabItem;

            // Put the current url in the search bar
            appData.Url = url;
        }

        // Redirects to url in new tab
        public void GoToUrl(string url)
        {
            // Only allow 11 tabs
            if (tabControl.Items.Count > 10)
            {
                return;
            }

            Task.Run(() =>
            {
                // Start the loading bar
                appData.Visibility = "Visible";
            });

            // Create a new TabItem and add it to the TabControl
            TabItem newTabItem = new TabItem();
            newTabItem.Header = "New Tab";
            tabControl.Items.Add(newTabItem);

            // Create a WebView2 control for the tab
            WebView2 webView = new WebView2();
            webView.Name = "webView";
            webView.Source = new Uri(url);
            webView.NavigationCompleted += WebView_NavigationCompleted;

            // Add the web view to the tab
            DockPanel panel = new DockPanel();
            panel.Children.Add(webView);
            newTabItem.Content = panel;

            // Select the new tab
            tabControl.SelectedItem = newTabItem;

            // Put the current url in the search bar
            appData.Url = url;
        }

        // Opens link from bookmark in new tab
        public void GoToBookMark(object sender, RoutedEventArgs e)
        {
            // Only allow 11 tabs
            if (tabControl.Items.Count > 10)
            {
                return;
            }

            Task.Run(() =>
            {
                // Start the loading bar
                appData.Visibility = "Visible";
            });

            string query = ""; // raw user input

            // Get input from button
            Button button = sender as Button;
            if (button != null)
            {
                query = button.CommandParameter as string;
            }

            // Create a new TabItem and add it to the TabControl
            TabItem newTabItem = new TabItem();
            newTabItem.Header = "New Tab";
            tabControl.Items.Add(newTabItem);

            // If the user entered a .com type query but left off https://, add https://
            if (!(query.StartsWith("https://") || query.StartsWith("http://")) &&
                (query.Contains(".com") || query.Contains(".org") || query.Contains(".net") ||
                  query.Contains(".edu") || query.Contains(".gov") || query.Contains(".mil") ||
                  query.Contains(".biz") || query.Contains(".info") || query.Contains(".io") ||
                  query.Contains(".co")))
            {
                query = "https://" + query;
            }
            try
            {
                // Try adding the web view to the tab
                WebView2 webView = new WebView2();
                webView.Name = "webView";
                webView.Source = new Uri(query);
                webView.NavigationCompleted += WebView_NavigationCompleted;

                DockPanel panel = new DockPanel();
                panel.Children.Add(webView);
                newTabItem.Content = panel;

                appData.Url = query;
                tabControl.SelectedItem = newTabItem;
            }
            catch (Exception ex)
            {
                // If url was invalid, search Google for the query in the new tab
                string url = "https://www.google.com/search?q=" + query;

                WebView2 webView = new WebView2();
                webView.Name = "webView";
                webView.Source = new Uri(url);
                webView.NavigationCompleted += WebView_NavigationCompleted;

                DockPanel panel = new DockPanel();
                panel.Children.Add(webView);
                newTabItem.Content = panel;

                appData.Url = url;
                tabControl.SelectedItem = newTabItem;
            }
        }

        // HELPER FUNCTIONS

        // Listens for browser to finish loading a page
        public void WebView_NavigationCompleted(object sender, Microsoft.Web.WebView2.Core.CoreWebView2NavigationCompletedEventArgs e)
        {
            Task.Run(() =>
            {
                // Stop the loading bar
                appData.Visibility = "Collapsed";
            });

            // Set the tabs header to the new page title
            ((TabItem)tabControl.SelectedItem).Header = ((WebView2)sender).CoreWebView2.DocumentTitle;

            // Get the url of the new page
            string url = ((WebView2)((DockPanel)((TabItem)tabControl.SelectedItem).Content).Children[0]).CoreWebView2.Source;

            // Update search bar
            if(appData.Url != url)
            {
                appData.Url = url;
            }

            // Add new url to browser history
            appData.AddUrl(url);
        }

        // Waits for user to click into the search bar
        public void UrlTextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            // Select entire url
            urlTextBox.SelectAll();
        }

        // Tells urlTextBox_GotKeyboardFocus that the user clicked into the search bar
        public void UrlTextBox_MouseUp(object sender, MouseButtonEventArgs e)
        {
            urlTextBox.Focus();
        }

        // Tells urlTextBox_GotKeyboardFocus that the user clicked into the search bar
        public void UrlTextBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!urlTextBox.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                urlTextBox.Focus();
            }
        }

        // KEYBOARD FUNCTIONALITY

        // Listens for enter while using the search bar
        public void UrlTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            // If enter is pressed, search for the query
            if (e.Key == Key.Enter)
            {
                if (sender == urlTextBox && urlTextBox.IsFocused)
                {
                    GoToUrl(sender, e);
                    e.Handled = true;
                    return;
                }
            }
        }

        // Listens for keypress
        public void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (CtrlKeyIsPressed())
            {
                HandleCtrlKeyPress(sender, e);
                return;
            }

            HandleStandardKeyPress(sender, e);
        }

        public void HandleCtrlKeyPress(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Right:
                    ForwardButton_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.Left:
                    BackButton_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.T:
                    AddNewTabButton_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.Delete:
                    RemoveTabButton_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.B:
                    AddBookmark_Click(sender, e);
                    e.Handled = true;
                    return;

            }
        }

        public void HandleStandardKeyPress(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.F8:
                    Settings_Click(sender, e);
                    e.Handled = true;
                    return;
            }
        }

        public bool CtrlKeyIsPressed()
        {
            return (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl));
        }
    }
  
    public class AppData : INotifyPropertyChanged
    {
        private string url; // Search bar text
        private string visibility; // Loading bar visibility
        private List<string> history; // List of websites visited
        public ObservableCollection<string> Bookmarks { get; set; }

        public AppData()
        {
            this.url = "https://google.com/";
            this.visibility = "Collapsed";
            this.history = new List<string>();
            Bookmarks = new ObservableCollection<string>();
        }

        public string Url
        {
            get { return url; }
            set
            {
                if (url != value)
                {
                    url = value;
                    OnPropertyChanged(nameof(Url));
                }
            }
        }

        public string Visibility
        {
            get { return visibility; }
            set
            {
                if (visibility != value)
                {
                    visibility = value;
                    OnPropertyChanged(nameof(Visibility));
                }
            }

        }
        
        public List<string> History
        {
            get { return history; }
            set
            {
                if (history != value)
                {
                    history = value;
                    OnPropertyChanged(nameof(History));
                }
            }
        }

        public void AddUrl(string url)
        {
            string currentDate = DateTime.Now.ToString("MM/dd/yyyy");
            string currentTime = DateTime.Now.ToString("h:mm:ss tt");
            string displayString = $"{currentDate} {url} {currentTime}";
            history.Add(displayString);
        }

        public void AddBookmark(string url)
        {
            
            if(Bookmarks.Count == 10)
            {
                MessageBox.Show("Maximum bookmarks reached!");
                return;
            }

            Bookmarks.Add(url);
        }

        public void RemoveBookmark(string url)
        {
            if (Bookmarks.Count == 0)
            {
                MessageBox.Show("No bookmarks to remove!");
                return;
            }

            Bookmarks.Remove(url);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
