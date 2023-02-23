using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace M6
{
    public partial class MainWindow : Window
    {
        private readonly MainWindowData mainWindowData = new();
        private readonly Dictionary<Key, Action<object, RoutedEventArgs>> keyToFunctionCall;
        private readonly Dictionary<string, char> operationToSymbol;

        public MainWindow()
        {
            InitializeComponent();
            DataContext = mainWindowData;

            keyToFunctionCall = new Dictionary<Key, Action<object, RoutedEventArgs>>
            {
                { Key.D0, (s, e) => NumPad_ButtonPress(0) },
                { Key.D1, (s, e) => NumPad_ButtonPress(1) },
                { Key.D2, (s, e) => NumPad_ButtonPress(2) },
                { Key.D3, (s, e) => NumPad_ButtonPress(3) },
                { Key.D4, (s, e) => NumPad_ButtonPress(4) },
                { Key.D5, (s, e) => NumPad_ButtonPress(5) },
                { Key.D6, (s, e) => NumPad_ButtonPress(6) },
                { Key.D7, (s, e) => NumPad_ButtonPress(7) },
                { Key.D8, (s, e) => NumPad_ButtonPress(8) },
                { Key.D9, (s, e) => NumPad_ButtonPress(9) },
                { Key.Enter, (s, e) => Equals_ButtonPress(s, e) },
                { Key.OemPlus, (s, e) => Equals_ButtonPress(s, e) },
                { Key.OemMinus, (s, e) => Subtract_ButtonPress(s, e) },
                { Key.OemPeriod, (s, e) => DecimalPoint_ButtonPress(s, e) },
                { Key.OemQuestion, (s, e) => Divide_ButtonPress(s, e) },
                { Key.Back, (s, e) => Delete_ButtonPress(s, e) },
                { Key.Delete, (s, e) => Clear_ButtonPress(s, e) },
            };

            operationToSymbol = new Dictionary<string, char>
            {
                { "add", '+' },
                { "subtract", '-' },
                { "multiply", '×' },
                { "divide", '÷' }
            };
        }

        public void Theme_ButtonPress(object sender, RoutedEventArgs e)
        {
            SetTheme(mainWindowData.ThemeName == "Standard" ? "Dark" : "Standard");
        }

        public void NumPad_ButtonPress(object sender, RoutedEventArgs e)
        {
            RemoveLeadingZero();
            mainWindowData.CurrentValue = $"{mainWindowData.CurrentValue}{((Button)sender).CommandParameter}";
        }

        public void NumPad_ButtonPress(int n)
        {
            RemoveLeadingZero();
            mainWindowData.CurrentValue += $"{n}";
        }

        public void Add_ButtonPress(object sender, RoutedEventArgs e)
        {
            if (NoCurrentValue()) { return; }
            if (OperationInProgress()) { CompletePreviousOperation("add"); return; }

            SetNewOperation(mainWindowData.CurrentValue, "add");
        }

        public void Subtract_ButtonPress(object sender, RoutedEventArgs e)
        {
            if (NoCurrentValue()) { return; }
            if (OperationInProgress()) { CompletePreviousOperation("subtract"); return; }

            SetNewOperation(mainWindowData.CurrentValue, "subtract");
        }

        public void Multiply_ButtonPress(object sender, RoutedEventArgs e)
        {
            if (NoCurrentValue()) { return; }
            if (OperationInProgress()) { CompletePreviousOperation("multiply"); return; }

            SetNewOperation(mainWindowData.CurrentValue, "multiply");
        }

        public void Divide_ButtonPress(object sender, RoutedEventArgs e)
        {
            if (NoCurrentValue()) { return; }
            if (OperationInProgress()) { CompletePreviousOperation("divide"); return; }

            SetNewOperation(mainWindowData.CurrentValue, "divide");
        }

        public void ClearEntry_ButtonPress(object sender, RoutedEventArgs e)
        {
            mainWindowData.CurrentValue = "";
        }

        public void Clear_ButtonPress(object sender, RoutedEventArgs e)
        {
            mainWindowData.PreviousValue = "";
            mainWindowData.CurrentValue = "";
        }

        public void Delete_ButtonPress(object sender, RoutedEventArgs e)
        {
            if (InvalidButtonPress()) { return; }

            int rightmostIndex = mainWindowData.CurrentValue.Length - 1;
            mainWindowData.CurrentValue = mainWindowData.CurrentValue.Remove(rightmostIndex);
        }

        public void Percent_ButtonPress(object sender, RoutedEventArgs e)
        {
            if (InvalidButtonPress() || NoCurrentValue()) { return; }

            double result = Calculator.Percent(double.Parse(mainWindowData.CurrentValue));
            mainWindowData.CurrentValue = $"{result}";
        }

        public void Reciprocal_ButtonPress(object sender, RoutedEventArgs e)
        {
            if (InvalidButtonPress() || NoCurrentValue()) { return; }

            double result = Calculator.Reciprocal(double.Parse(mainWindowData.CurrentValue));
            mainWindowData.CurrentValue = $"{result}";
        }

        public void Square_ButtonPress(object sender, RoutedEventArgs e)
        {
            if (InvalidButtonPress() || NoCurrentValue()) { return; }

            double result = Calculator.Square(double.Parse(mainWindowData.CurrentValue));
            mainWindowData.CurrentValue = $"{result}";
        }

        public void SquareRoot_ButtonPress(object sender, RoutedEventArgs e)
        {
            if (InvalidButtonPress() || NoCurrentValue()) { return; }

            double currentValue = double.Parse(mainWindowData.CurrentValue);
            if (currentValue < 0) return;

            double result = Calculator.SquareRoot(currentValue);
            mainWindowData.CurrentValue = $"{result}";
        }

        public void SignChange_ButtonPress(object sender, RoutedEventArgs e)
        {
            if (InvalidButtonPress() || NoCurrentValue()) {  return; }

            double currentValue = double.Parse(mainWindowData.CurrentValue);
            if (currentValue == 0) return;

            double result = Calculator.SignChange(currentValue);
            mainWindowData.CurrentValue = $"{result}";
        }

        public void DecimalPoint_ButtonPress(object sender, RoutedEventArgs e)
        {
            if (DecimalPointAlreadyUsed()) { return; }

            mainWindowData.CurrentValue += ".";
        }

        public void Equals_ButtonPress(object sender, RoutedEventArgs e)
        {
            if (CannotCompleteOperation() || NoCurrentValue()) { return; }

            double result = CalculateResult();

            mainWindowData.PreviousValue += $" {mainWindowData.CurrentValue} = ";
            mainWindowData.CurrentValue = $"{result}";
        }

        // KEYBOARD FUNCTIONALITY
        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (ShiftKeyIsPressed())
            {
                HandleShiftKeyPress(sender, e);
                return;
            }

            HandleStandardKeyPress(sender, e);
        }

        // HELPER FUNCTIONS
        private void CompletePreviousOperation(string new_operation)
        {
            if (CannotCompleteOperation() || NoCurrentValue()) { return; }

            double result = CalculateResult();

            SetNewOperation(result, new_operation);
        }

        private double CalculateResult()
        {
            string previousValue = mainWindowData.PreviousValue;
            double a = double.Parse(mainWindowData.PreviousValue.TrimEnd(' ', '+', '-', '×', '÷'));
            double b = double.Parse(mainWindowData.CurrentValue);
            double result = 0;

            switch (mainWindowData.CurrentOperation)
            {
                case "add":
                    return result = Calculator.Add(a, b);
                case "subtract":
                    return result = Calculator.Subtract(a, b);
                case "multiply":
                    return result = Calculator.Multiply(a, b);
                case "divide":
                    return result = Calculator.Divide(a, b);
            }

            return result;
        }

        private void SetNewOperation(double operand, string new_operation)
        {
            mainWindowData.PreviousValue = $"{operand} {operationToSymbol[new_operation]}";
            mainWindowData.CurrentValue = $"";
            mainWindowData.CurrentOperation = new_operation;
        }

        private void SetNewOperation(string operand, string new_operation)
        {
            mainWindowData.PreviousValue = $"{operand} {operationToSymbol[new_operation]}";
            mainWindowData.CurrentValue = $"";
            mainWindowData.CurrentOperation = new_operation;
        }

        private void RemoveLeadingZero()
        {
            if (mainWindowData.CurrentValue == "0")
            {
                mainWindowData.CurrentValue = "";
            }
        }

        private bool ShiftKeyIsPressed()
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                return true;
            }

            return false;
        }

        private void HandleShiftKeyPress(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D8:
                    Multiply_ButtonPress(sender, e);
                    e.Handled = true;
                    return;
                case Key.D5:
                    Percent_ButtonPress(sender, e);
                    e.Handled = true;
                    return;
                case Key.OemPlus:
                    Add_ButtonPress(sender, e);
                    e.Handled = true;
                    return;
            }
        }

        private void HandleStandardKeyPress(object sender, KeyEventArgs e)
        {
            if (keyToFunctionCall.TryGetValue(e.Key, out var function_call))
            {
                function_call(sender, e);
                e.Handled = true;
            }
        }

        private bool InvalidButtonPress()
        {
            if (string.IsNullOrEmpty(mainWindowData.CurrentValue))
            {
                return true;
            }

            return false;
        }

        private bool OperationInProgress()
        {
            return new char[] { '+', '-', '÷', '×' }.Any(c => mainWindowData.PreviousValue.EndsWith(c));
        }

        private bool CannotCompleteOperation()
        {
            if (mainWindowData.PreviousValue.Contains('=') || string.IsNullOrEmpty(mainWindowData.PreviousValue) || string.IsNullOrEmpty(mainWindowData.CurrentValue))
            {
                return true;
            }

            return false;
        }
        
        private bool NoCurrentValue()
        {
            if (string.IsNullOrEmpty(mainWindowData.CurrentValue) || mainWindowData.CurrentValue == ".")
            {
                return true;
            }

            return false;
        }

        private bool DecimalPointAlreadyUsed()
        {
            if (mainWindowData.CurrentValue.Contains('.'))
            {
                return true;
            }

            return false;
        }

        private void SetTheme(string theme)
        {
            if (theme == "Dark")
            {
                mainWindowData.ThemeName = "Dark";
                mainWindowData.BackgroundColor = "#1a212c";
                mainWindowData.MainTextColor = "White";
                mainWindowData.LightTextColor = "White";
                mainWindowData.EqualsTextColor = "#1b1b1b";
                mainWindowData.NumPadButtonColor = "#373c49";
                mainWindowData.OperationButtonColor = "#2d3240";
                mainWindowData.EqualsButtonColor = "#a1b5bb";
                return;
            }

            mainWindowData.ThemeName = "Standard";
            mainWindowData.BackgroundColor = "#eff4f9";
            mainWindowData.MainTextColor = "#1b1b1b";
            mainWindowData.LightTextColor = "#656565";
            mainWindowData.EqualsTextColor = "#cccccc";
            mainWindowData.NumPadButtonColor = "#f6f6f6";
            mainWindowData.OperationButtonColor = "#f6f6f6";
            mainWindowData.EqualsButtonColor = "#485c68";
        }
    }

    public class MainWindowData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string currentValue;
        private string previousValue;
        private string currentOperation;

        private string themeName;
        private string backgroundColor;
        private string mainTextColor;
        private string lightTextColor;
        private string equalsTextColor;
        private string numPadButtonColor;
        private string operationButtonColor;
        private string equalsButtonColor;

        public MainWindowData()
        {
            currentValue = "0";
            previousValue = "";
            currentOperation = "";

            themeName = "Standard";
            backgroundColor = "#eff4f9";
            mainTextColor = "#1b1b1b";
            lightTextColor = "#656565";
            equalsTextColor = "#cccccc";
            numPadButtonColor = "#f6f6f6";
            operationButtonColor = "#f6f6f6";
            equalsButtonColor = "#485c68";
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string ThemeName
        {
            get => themeName;
            set
            {
                themeName = value;
                OnPropertyChanged(nameof(themeName));
            }
        }

        public string BackgroundColor
        {
            get => backgroundColor;
            set
            {
                backgroundColor = value;
                OnPropertyChanged(nameof(backgroundColor));
            }
        }

        public string MainTextColor
        {
            get => mainTextColor;
            set
            {
                mainTextColor = value;
                OnPropertyChanged(nameof(mainTextColor));
            }
        }

        public string LightTextColor
        {
            get => lightTextColor;
            set
            {
                lightTextColor = value;
                OnPropertyChanged(nameof(lightTextColor));
            }
        }

        public string EqualsTextColor
        {
            get => equalsTextColor;
            set
            {
                equalsTextColor = value;
                OnPropertyChanged(nameof(equalsTextColor));
            }
        }

        public string NumPadButtonColor
        {
            get => numPadButtonColor;
            set
            {
                numPadButtonColor = value;
                OnPropertyChanged(nameof(numPadButtonColor));
            }
        }

        public string OperationButtonColor
        {
            get => operationButtonColor;
            set
            {
                operationButtonColor = value;
                OnPropertyChanged(nameof(operationButtonColor));
            }
        }

        public string EqualsButtonColor
        {
            get => equalsButtonColor;
            set
            {
                equalsButtonColor = value;
                OnPropertyChanged(nameof(equalsButtonColor));
            }
        }

        public string CurrentValue
        {
            get => currentValue;
            set
            {
                currentValue = value;
                OnPropertyChanged(nameof(currentValue));
            }
        }

        public string PreviousValue
        {
            get => previousValue;
            set
            {
                previousValue = value;
                OnPropertyChanged(nameof(previousValue));
            }
        }

        public string CurrentOperation
        {
            get => currentOperation;
            set
            {
                currentOperation = value;
                OnPropertyChanged(nameof(currentOperation));
            }
        }
    }

    public class Calculator
    {
        public Calculator()
        {

        }

        public static double Add(double a, double b)
        {
            return a + b;
        }

        public static double Subtract(double a, double b)
        {
            return a - b;
        }

        public static double Multiply(double a, double b)
        {
            return a * b;
        }

        public static double Divide(double a, double b)
        {
            return a / b;
        }

        public static double Percent(double a)
        {
            return a / 100;
        }

        public static double Reciprocal(double a)
        {
            return 1 / a;
        }

        public static double Square(double a)
        {
            return Math.Pow(a, 2);
        }

        public static double SquareRoot(double a)
        {
            return Math.Sqrt(a);
        }

        public static double SignChange(double a)
        {
            return -a;
        }
    }
}
