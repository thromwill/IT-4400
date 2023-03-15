using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace M8
{
    public partial class MainWindow : Window
    {
        private AppData appData = new AppData();
        private SettingsPage settingsPage;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = appData;
            settingsPage = new SettingsPage(appData);
            this.Icon = new BitmapImage(new Uri("pack://application:,,,/M8;component/Images/notepad.ico"));

            Closing += MainWindow_Closing;
        }

        // MAINWINDOW CLOSE
        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            // If the file has been modified, ask the user to save changes
            if (IsFilePathSet() ? FileHasChanged() : !IsTextBoxBlank())
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes to the file before closing?", "Save changes?", MessageBoxButton.YesNoCancel);
                switch (result)
                {
                    case MessageBoxResult.Yes:
                        FileSave_Click(sender, null);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        e.Cancel = true; // Cancel closing
                        break;
                }
            }
        }

        // FILE
        private void FileNew_Click(object sender, RoutedEventArgs e)
        {
            // If the file is already blank and unsaved, do nothing
            if (!IsFilePathSet() && IsTextBoxBlank()) return;
            if(IsFilePathSet() ? FileHasChanged() : !IsTextBoxBlank())
            {
                // Ask to save current file
                MessageBoxResult result = MessageBox.Show("Do you want to save changes to the current file before creating a new file?", "Save changes?", MessageBoxButton.YesNoCancel);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        FileSave_Click(sender, e);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }

            // Reset current file data
            MainTextBox.Text = string.Empty;
            appData.FilePath = "";
            appData.FileName = "Untitled";
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            // Ask to save file if necessary
            if (IsFilePathSet() ? FileHasChanged() : !IsTextBoxBlank())
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes to the current file before opening a new file?", "Save changes?", MessageBoxButton.YesNoCancel);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        FileSave_Click(sender, e);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }

            // Open file via OpenFileDialog
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                InitialDirectory = System.IO.Path.GetDirectoryName(appData.FilePath),
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Open Text File",
                CheckFileExists = true,
                CheckPathExists = true,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string fileName = openFileDialog.FileName;
                    string fileContent = TextDocument.Open(fileName);

                    MainTextBox.Text = fileContent;
                    appData.FilePath = fileName;
                    appData.FileName = System.IO.Path.GetFileNameWithoutExtension(fileName);

                    appData.IsSaveEnabled = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void FileSave_Click(object sender, RoutedEventArgs e)
        {
            // If the file is unsaved used Save As instead
            if (!IsFilePathSet())
            {
                FileSaveAs_Click(sender, e);
                return;
            }

            // Save the file
            try
            {
                TextDocument.Save(appData.FilePath, MainTextBox.Text);
                appData.IsSaveEnabled=false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving file: " + ex.Message);
            }

        }

        private void FileSaveAs_Click(object sender, RoutedEventArgs e)
        {
            // Save As via SaveFileDialog
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Save Text File As...",
                CheckPathExists = true,
                OverwritePrompt = true,
                InitialDirectory = System.IO.Path.GetDirectoryName(appData.FilePath)
            };
            
            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    string fileName = saveFileDialog.FileName;
                    TextDocument.Save(fileName, MainTextBox.Text);

                    appData.IsSaveEnabled = false;
                    appData.FilePath = fileName;
                    appData.FileName = System.IO.Path.GetFileName(fileName);

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            // Ask to save file if necessary
            if (IsFilePathSet() ? FileHasChanged() : !IsTextBoxBlank())
            {
                MessageBoxResult result = MessageBox.Show("Do you want to save changes to the current file before exiting?", "Save changes?", MessageBoxButton.YesNoCancel);

                switch (result)
                {
                    case MessageBoxResult.Yes:
                        FileSave_Click(sender, e);
                        break;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Cancel:
                        return;
                }
            }

            // Exit program
            Application.Current.Shutdown();
        }

        private bool FileHasChanged()
        {
            if (!IsFilePathSet()) return false;
            return MainTextBox.Text != File.ReadAllText(appData.FilePath);
        }

        private bool IsSaveEnabled()
        {
            if (!IsFilePathSet()) return !IsTextBoxBlank();
            return MainTextBox.Text != File.ReadAllText(appData.FilePath);
        }

        private bool IsTextBoxBlank()
        {
            return string.IsNullOrEmpty(MainTextBox.Text);
        }
        private bool IsFilePathSet()
        {
            return !string.IsNullOrEmpty(appData.FilePath);
        }

        // CHECK FOR FILE CHANGES
        private void File_TextChanged(object sender, TextChangedEventArgs e)
        {
            appData.IsSaveEnabled = IsSaveEnabled();
            appData.IsUndoEnabled = IsUndoEnabled();
            appData.IsRedoEnabled = IsRedoEnabled();
            appData.IsSelectAllEnabled = IsSelectAllEnabled();
           
        }

        private void File_ZoomChanged(object sender, EventArgs e)
        {
            appData.IsZoomInEnabled = IsZoomInEnabled();
            appData.IsZoomOutEnabled = IsZoomOutEnabled();
            appData.IsDefaultEnabled = IsDefaultEnabled();
        }

        private void File_SelectionChanged(object sender, RoutedEventArgs e)
        {
            appData.IsCutEnabled = IsCutEnabled();
            appData.IsCopyEnabled = IsCopyEnabled();
            appData.IsPasteEnabled = IsPasteEnabled();

            UpdateCaretPosition();
        }

        private void UpdateCaretPosition()
        {
            int caretIndex = MainTextBox.CaretIndex;
            int lineNumber = MainTextBox.GetLineIndexFromCharacterIndex(caretIndex);
            int columnIndex = caretIndex - MainTextBox.GetCharacterIndexFromLineIndex(lineNumber);

            appData.LineNumber = lineNumber + 1;
            appData.ColumnNumber = columnIndex + 1;
            appData.CaretPositionText = $"Ln {appData.LineNumber}, Col {appData.ColumnNumber}";
        }

        // EDIT
        private void EditUndo_Click(object sender, RoutedEventArgs e)
        {
            if (MainTextBox.CanUndo)
            {
                MainTextBox.Undo();
            }
        }

        private bool IsUndoEnabled()
        {
            return (MainTextBox != null && MainTextBox.CanUndo);
        }

        private void EditRedo_Click(object sender, RoutedEventArgs e)
        {
            if (MainTextBox.CanRedo)
            {
                MainTextBox.Redo();
            }
        }

        private bool IsRedoEnabled()
        {
            return MainTextBox.CanRedo;
        }

        private void EditCut_Click(object sender, RoutedEventArgs e)
        {
            MainTextBox.Cut();
        }

        private bool IsCutEnabled()
        {
            return MainTextBox.SelectionLength > 0;
        }

        private void EditCopy_Click(object sender, RoutedEventArgs e)
        {
            MainTextBox.Copy();
        }

        private bool IsCopyEnabled()
        {
            return MainTextBox.SelectionLength > 0;
        }

        private void EditPaste_Click(object sender, RoutedEventArgs e)
        {
            MainTextBox.Paste();
        }

        private bool IsPasteEnabled()
        {
            return Clipboard.ContainsText();
        }

        private void EditSelectAll_Click(object sender, RoutedEventArgs e)
        {
            MainTextBox.SelectAll();
        }

        private bool IsSelectAllEnabled()
        {
            return !string.IsNullOrEmpty(MainTextBox.Text);
        }

        // VIEW
        private void ViewZoomIn_Click(object sender, RoutedEventArgs e)
        {
            // Increase font size by 10%
            appData.ViewScale += (appData.ViewScale >= 300 ? 0 : 10);
            appData.FontSize = $"{18 * appData.ViewScale / 100}";

            // Update UI
            File_ZoomChanged(sender, e);
        }

        private bool IsZoomInEnabled()
        {
            return !(appData.ViewScale >= 300);
        }

        private void ViewZoomOut_Click(object sender, RoutedEventArgs e)
        {
            // Decrease font size by 1%
            appData.ViewScale -= (appData.ViewScale <= 10 ? 0 : 10);
            appData.FontSize = $"{18 * appData.ViewScale / 100}";

            // Update UI
            File_ZoomChanged(sender, e);
        }

        private bool IsZoomOutEnabled()
        {
            return !(appData.ViewScale <= 10);
        }

        private void ViewDefault_Click(object sender, RoutedEventArgs e)
        {
            // Reset font size to 18
            appData.ViewScale = 100;
            appData.FontSize = "18";

            // Update UI
            File_ZoomChanged(sender, e);
        }

        private bool IsDefaultEnabled()
        {
            return !(appData.ViewScale == 100);
        }

        private void ViewFullscreen_Click(object sender, RoutedEventArgs e)
        {
            if (appData.IsFullScreen)
            {
                RestoreWindow();
            }
            else
            {
                MaximizeWindow();
            }

            UpdateFullScreenStatus();
        }

        private void MaximizeWindow()
        {
            this.WindowState = WindowState.Maximized;
            this.WindowStyle = WindowStyle.None;

            appData.IsFullScreen = true;
        }

        private void RestoreWindow()
        {
            this.WindowState = WindowState.Normal;
            this.WindowStyle = WindowStyle.SingleBorderWindow;

            appData.IsFullScreen = false;
        }

        private void UpdateFullScreenStatus()
        {
            appData.FullScreenText = appData.IsFullScreen ? "Full Screen ✓" : "Full Screen";
        }

        private void ViewWordWrap_Click(object sender, RoutedEventArgs e)
        {
            appData.WordWrap = (appData.WordWrap == "Wrap" ? "NoWrap" : "Wrap");
            appData.WordWrapText = (appData.WordWrap == "Wrap" ? "Word Wrap  ✓" : "Word Wrap");
        }

        // HELP
        private void HelpAbout_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("This is a text editor. You can use it like notepad.", "\nAbout", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // SETTINGS
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (this.Content == settingsPage)
            {
                settingsPage.SettingsBack_Click(sender, e);
                return;
            }

            // Display settings page
            var temp = this.Content;
            this.Content = settingsPage;
            settingsPage.BackButtonPressed += (s, args) =>
            {
                this.Content = temp;
            };
        }

        // KEYBOARD FUNCTIONALITY
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (CtrlKeyIsPressed())
            {
                HandleCtrlKeyPress(sender, e);
                return;
            }

            HandleStandardKeyPress(sender, e);
        }

        private bool CtrlKeyIsPressed()
        {
            return (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) ;
        }

        private void HandleCtrlKeyPress(object sender, KeyEventArgs e)
        {
            if (ShiftKeyIsPressed())
            {
                HandleShiftKeyPress(sender, e);
                return;
            }

            switch (e.Key)
            {
                case Key.O:
                    FileOpen_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.S:
                    FileSave_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.Z:
                    EditUndo_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.X:
                    EditCut_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.C:
                    EditCopy_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.V:
                    EditPaste_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.A:
                    EditSelectAll_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.OemMinus:
                    ViewZoomOut_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.D0:
                    ViewDefault_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.OemPlus:
                    ViewZoomIn_Click(sender, e);
                    e.Handled = true;
                    return;
            }
        }

        private void HandleStandardKeyPress(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Escape:
                    if (appData.IsFullScreen)
                    {
                        ViewFullscreen_Click(sender, e);
                        e.Handled = true;
                        return;
                    }

                    if (SettingsPageIsOpen())
                    {
                        settingsPage.SettingsBack_Click(sender, e);
                        e.Handled = true;
                        return;
                    }

                    break;
                case Key.F9:
                    ViewWordWrap_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.F3:
                    Settings_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.F11:
                    ViewFullscreen_Click(sender, e);
                    e.Handled = true;
                    return;
            }
        }

        private void HandleShiftKeyPress(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.S:
                    FileSaveAs_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.Z:
                    EditRedo_Click(sender, e);
                    e.Handled = true;
                    return;
                case Key.W:
                    ViewWordWrap_Click(sender, e);
                    e.Handled = true;
                    return;
            }
        }

        private bool ShiftKeyIsPressed()
        {
            return (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift));
        }

        private bool SettingsPageIsOpen()
        {
            //return settingsPage.Visibility == Visibility.Visible;
            return this.Content == settingsPage;
        }
    }

    public class TextDocument : INotifyPropertyChanged
    {
        private string filePath;
        private string fileName;

        public TextDocument()
        {
            filePath = "";
            fileName = "Untitled";
        }

        public static string Open(string fileName)
        {
            return File.ReadAllText(fileName);
        }

        public static void Save(string filePath, string text)
        {
            File.WriteAllText(filePath, text);
        }

        public string FilePath
        {
            get => filePath;
            set
            {
                filePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class AppData : INotifyPropertyChanged
    {
        private TextDocument textDocument;

        private int viewScale;
        private string fontSize;

        private bool isFullScreen;
        private string fullScreenText;

        private string wordWrap;
        private string wordWrapText;

        private string textBoxFont;
        private string textBoxFontStyle;
        private string textBoxFontWeight;

        private bool isSaveEnabled;
        private bool isRedoEnabled;
        private bool isUndoEnabled;
        private bool isCutEnabled;
        private bool isCopyEnabled;
        private bool isPasteEnabled;
        private bool isSelectAllEnabled;
        private bool isZoomInEnabled;
        private bool isZoomOutEnabled;
        private bool isDefaultEnabled;

        private int lineNumber;
        private int columnNumber;
        private string caretPositionText;

        public AppData()
        {
            textDocument = new TextDocument();

            viewScale = 100;
            fontSize = "18";
            isFullScreen = false;
            fullScreenText = "Full Screen";
            wordWrap = "Wrap";
            wordWrapText = "Word Wrap ✓";
            textBoxFont = "Consolas";
            textBoxFontStyle = "Normal";
            textBoxFontWeight = "Normal";

            isSaveEnabled = false;
            isUndoEnabled = false;
            isRedoEnabled = false;
            isCutEnabled = false;
            isCopyEnabled = false;
            isPasteEnabled = true;
            isSelectAllEnabled = true;
            isZoomInEnabled = true;
            isZoomOutEnabled = true;
            isDefaultEnabled = false;

            lineNumber = 1;
            columnNumber = 1;
            caretPositionText = "Ln 1, Col 1";
    }

        public TextDocument TextDocument
        {
            get => textDocument;
            set
            {
                textDocument = value;
                OnPropertyChanged(nameof(TextDocument));
            }
        }

        public string FileName
        {
            get => TextDocument.FileName;
            set
            {
                TextDocument.FileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        public string FilePath
        {
            get => TextDocument.FilePath;
            set
            {
                TextDocument.FilePath = value;
                OnPropertyChanged(nameof(FilePath));
            }
        }

        public int ViewScale
        {
            get => viewScale;
            set
            {
                viewScale = value;
                OnPropertyChanged(nameof(ViewScale));
            }
        }

        public string FontSize
        {
            get => fontSize;
            set
            {
                fontSize = value;
                OnPropertyChanged(nameof(FontSize));
            }
        }

        public bool IsFullScreen
        {
            get => isFullScreen;
            set
            {
                isFullScreen = value;
                OnPropertyChanged(nameof(IsFullScreen));
            }
        }

        public string FullScreenText
        {
            get => fullScreenText;
            set
            {
                fullScreenText = value;
                OnPropertyChanged(nameof(FullScreenText));
            }

        }

        public string WordWrap
        {
            get => wordWrap;
            set
            {
                wordWrap = value;
                OnPropertyChanged(nameof(WordWrap));
            }
        }

        public string WordWrapText
        {
            get => wordWrapText;
            set
            {
                wordWrapText = value;
                OnPropertyChanged(nameof(WordWrapText));
            }
        }

        public string TextBoxFont
        {
            get => textBoxFont;
            set
            {
                textBoxFont = value;
                OnPropertyChanged(nameof(TextBoxFont));
            }
        }

        public void UpdateTextBoxFont(string newFont)
        {
            textBoxFont = newFont;
        }

        public string TextBoxFontStyle
        {
            get => textBoxFontStyle;
            set
            {
                textBoxFontStyle = value;
                OnPropertyChanged(nameof(TextBoxFontStyle));
            }
        }

        public void UpdateTextBoxFontStyle(string newStyle)
        {
            textBoxFontStyle = newStyle;
        }

        public string TextBoxFontWeight
        {
            get => textBoxFontWeight;
            set
            {
                textBoxFontWeight = value;
                OnPropertyChanged(nameof(TextBoxFontWeight));
            }
        }

        public void UpdateTextBoxFontWeight(string newWeight)
        {
            textBoxFontWeight = newWeight;
        }

        

        public string CaretPositionText
        {
            get => caretPositionText;
            set
            {
                caretPositionText = value;
                OnPropertyChanged(nameof(CaretPositionText));
            }
        }

        public int LineNumber
        {
            get => lineNumber;
            set
            {
                lineNumber = value;
                OnPropertyChanged(nameof(LineNumber));
            }
        }

        public int ColumnNumber
        {
            get => columnNumber;
            set
            {
                columnNumber = value;
                OnPropertyChanged(nameof(ColumnNumber));
            }
        }

        public bool IsUndoEnabled
        {
            get => isUndoEnabled;
            set
            {
                isUndoEnabled = value;
                OnPropertyChanged(nameof(IsUndoEnabled));
            }
        }

        public bool IsRedoEnabled
        {
            get => isRedoEnabled;
            set
            {
                isRedoEnabled = value;
                OnPropertyChanged(nameof(IsRedoEnabled));
            }
        }

        public bool IsCutEnabled
        {
            get => isCutEnabled;
            set
            {
                isCutEnabled = value;
                OnPropertyChanged(nameof(IsCutEnabled));
            }
        }

        public bool IsCopyEnabled
        {
            get => isCopyEnabled;
            set
            {
                isCopyEnabled = value;
                OnPropertyChanged(nameof(IsCopyEnabled));
            }
        }

        public bool IsPasteEnabled
        {
            get => isPasteEnabled;
            set
            {
                isPasteEnabled = value;
                OnPropertyChanged(nameof(IsPasteEnabled));
            }
        }

        public bool IsSelectAllEnabled
        {
            get => isSelectAllEnabled;
            set
            {
                isSelectAllEnabled = value;
                OnPropertyChanged(nameof(IsSelectAllEnabled));
            }
        }

        public bool IsZoomInEnabled
        {
            get => isZoomInEnabled;
            set
            {
                isZoomInEnabled = value;
                OnPropertyChanged(nameof(IsZoomInEnabled));
            }
        }

        public bool IsZoomOutEnabled
        {
            get => isZoomOutEnabled;
            set
            {
                isZoomOutEnabled = value;
                OnPropertyChanged(nameof(IsZoomOutEnabled));
            }
        }

        public bool IsDefaultEnabled
        {
            get => isDefaultEnabled;
            set
            {
                isDefaultEnabled = value;
                OnPropertyChanged(nameof(IsDefaultEnabled));
            }
        }


        public bool IsSaveEnabled
        {
            get => isSaveEnabled;
            set
            {
                isSaveEnabled = value;
                OnPropertyChanged(nameof(IsSaveEnabled));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
