using Calc.Model;
using Calc.Shell;
using Calc.ViewModels;
using DryIoc.ImTools;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Windows;
using System.Diagnostics;
using System.Numerics;

namespace Calc.ViewModel
{
    public class ShellViewModel : BindableBase
    {
        private readonly ShellModel _calculator;
        private readonly HistoryViewModel _historyViewModel;
        private string _expression = string.Empty;
        private string _result = string.Empty;
        private bool _isHistoryVisible;

        public event Action ScrollExpressionToEndRequested;

        private Brush _expressionColor;
        public Brush ExpressionColor
        {
            get => _expressionColor;
            set => SetProperty(ref _expressionColor, value);
        }

        private double _expressionOffsetX;
        public double ExpressionOffsetX
        {
            get => _expressionOffsetX;
            set => SetProperty(ref _expressionOffsetX, value);
        }

        private bool _canScrollLeft;
        public bool CanScrollLeft
        {
            get => _canScrollLeft;
            set => SetProperty(ref _canScrollLeft, value);
        }

        private bool _canScrollRight;
        public bool CanScrollRight
        {
            get => _canScrollRight;
            set => SetProperty(ref _canScrollRight, value);
        }

        public string Expression
        {
            get => _expression;
            set
            {
                SetProperty(ref _expression, value);
            }
        }

        public string Result
        {
            get => _result;
            set
            {
                if (value.Length > 24)
                    value = value.Substring(0, 24);

                SetProperty(ref _result, value);
            }
        }

        public bool IsHistoryVisible
        {
            get => _isHistoryVisible;
            set => SetProperty(ref _isHistoryVisible, value);
        }

        public HistoryViewModel HistoryViewModel => _historyViewModel;

        public ShellViewModel(HistoryViewModel historyViewModel, ShellModel calculator)
        {
            _calculator = calculator;
            _historyViewModel = historyViewModel;

            _historyViewModel.HistoryEntrySelected += FromHistoryToCalc;
            _historyViewModel.CloseHistoryCommand = new DelegateCommand(() => IsHistoryVisible = false);

            AppendToExpressionCommand = new DelegateCommand<string>(AppendToExpression);
            CalculateResultCommand = new DelegateCommand(CalculateResult);
            HandleKeyPressCommand = new DelegateCommand<Key?>(key => { if (key.HasValue) HandleKeyPress(key.Value); });

            ToggleHistoryCommand = new DelegateCommand(() => IsHistoryVisible = !IsHistoryVisible);
            PercentageCommand = new DelegateCommand(ApplyPercentage);
            ClearEntryCommand = new DelegateCommand(ClearEntry);
            ClearCommand = new DelegateCommand(Clear);
            BackspaceCommand = new DelegateCommand(Backspace);
            FactorialCommand = new DelegateCommand(ApplyFactorial);
            DegreeCommand = new DelegateCommand(ApplyDegree);
            SquareCommand = new DelegateCommand(ApplySquareRoot);
            ToggleSignCommand = new DelegateCommand(ToggleSign);

            var colorHex = App.Configuration["Appearance:ExpressionColor"] ?? "#FFFFFF";
            try { ExpressionColor = (SolidColorBrush)new BrushConverter().ConvertFromString(colorHex); }
            catch { ExpressionColor = Brushes.White; }

            var historyColorHex = App.Configuration["Appearance:HistoryTextColor"] ?? "#D3D3D3";
            try { _historyViewModel.HistoryTextColor = (SolidColorBrush)new BrushConverter().ConvertFromString(historyColorHex); }
            catch { _historyViewModel.HistoryTextColor = Brushes.LightGray; }
        }

        public ICommand AppendToExpressionCommand { get; }
        public ICommand CalculateResultCommand { get; }
        public ICommand HandleKeyPressCommand { get; }
        public ICommand ToggleHistoryCommand { get; }
        public ICommand PercentageCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand FactorialCommand { get; }
        public ICommand DegreeCommand { get; }
        public ICommand SquareCommand { get; }
        public ICommand ToggleSignCommand { get; }
        public ICommand ScrollExpressionCommand { get; }


        /// <summary>
        /// Обрабатывает выбор элемента из истории (переносит его в выражение).
        /// </summary>
        /// <param name="entry">Выбранная запись истории.</param>
        private void FromHistoryToCalc(string entry)
        {
            var expression = entry.Split('=')[0].Trim();
            Expression = expression;
            CalculateIntermediateResult();
        }
        /// <summary>
        /// Добавляет значение к текущему выражению.
        /// </summary>
        /// <param name="value">Значение для добавления.</param>
        private void AppendToExpression(string value)
        {

            if (string.IsNullOrEmpty(value))
                return;

            string[] binaryOperators = { "+", "*", "/", "%" };
            string[] operatorsForCheck = { "+", "-", "*", "/", "!" };

            if (value == "!")
            {
                string pattern = @"(\d+)$";
                var match = Regex.Match(Expression, pattern);
                if (!match.Success)
                    return;

                Expression = Expression.Insert(match.Index + match.Length, "!");
                CalculateIntermediateResult();
                return;
            }

            if (Expression.EndsWith("!") && (char.IsDigit(value[0]) || value == "."))
                return;

            if (Expression.EndsWith("!") && "+-*/%^".Contains(value))
            {
                Expression += value;
                ScrollExpressionToEndRequested?.Invoke();
                return;
            }

            if (char.IsDigit(value[0]) && Expression.EndsWith(")"))
                return;

            if (string.IsNullOrEmpty(Expression) && binaryOperators.Contains(value))
                return;

            if ("+*/%.".Contains(value) && Expression.EndsWith("^"))
                return;

            if (operatorsForCheck.Contains(value) && Expression.Length > 0)
            {
                var lastChar = Expression.Last().ToString();
                if (operatorsForCheck.Contains(lastChar))
                {
                    Expression = Expression.Substring(0, Expression.Length - 1) + value;
                    ScrollExpressionToEndRequested?.Invoke();
                    return;
                }
            }

            if (Expression.Length == 0 && value == "-")
            {
                Expression += value;
                return;
            }

            if (Expression.EndsWith("%") && (char.IsDigit(value[0]) || value == "."))
                return;

            if (value == "%" && Expression.Length > 0)
            {
                var lastChar = Expression.Last().ToString();
                if (operatorsForCheck.Contains(lastChar) || lastChar == "(")
                    return;
            }

            if (value == ".")
            {
                if (Expression.Length == 0 || "+-*/".Contains(Expression.Last()))
                {
                    Expression += "0.";
                    CalculateIntermediateResult();
                    ScrollExpressionToEndRequested?.Invoke();
                    return;
                }

                var match = Regex.Match(Expression, @"(\d+\.\d*|\d*\.\d+|\d+)$");
                if (match.Success && match.Value.Contains("."))
                    return;

                Expression += ".";
                CalculateIntermediateResult();
                ScrollExpressionToEndRequested?.Invoke();
                return;
            }

            if ("+-*/%!".Contains(value) && Expression.EndsWith("."))
                return;

            if (value == "^")
            {
                if (string.IsNullOrEmpty(Expression))
                    return;

                if (operatorsForCheck.Contains(Expression.Last().ToString()))
                    return;

                if (Expression.Contains("^") && !Expression.Split('^').Last().Contains(")"))
                    return;
            }

            if (value == "%" && Expression.EndsWith("%"))
                return;

            if (char.IsDigit(value[0]) && value != ".")
            {
                string[] operators = { "+", "-", "*", "/", "%", "!"};
                int lastOperatorIndex = -1;
                for (int i = Expression.Length - 1; i >= 0; i--)
                {
                    if (operators.Contains(Expression[i].ToString()))
                    {
                        lastOperatorIndex = i;
                        break;
                    }
                }

                string beforeLastNumber = lastOperatorIndex == -1 ? "" : Expression.Substring(0, lastOperatorIndex + 1);
                string lastNumber = lastOperatorIndex == -1 ? Expression : Expression.Substring(lastOperatorIndex + 1);

                if (lastNumber.Length > 0 && Regex.IsMatch(lastNumber, @"^0+$") && value == "0")
                {
                    Expression = beforeLastNumber + "0";
                    ScrollExpressionToEndRequested?.Invoke();
                    return;
                }

                if (lastNumber.Length > 1 && lastNumber.StartsWith("0") && !lastNumber.StartsWith("0."))
                {
                    Expression = beforeLastNumber + value;
                    CalculateIntermediateResult();
                    ScrollExpressionToEndRequested?.Invoke();
                    return;
                }
            }

            Expression += value;
            CalculateIntermediateResult();
            ScrollExpressionToEndRequested?.Invoke();
        }


        /// <summary>
        /// Подготавливает выражение для вычисления (корни, степени, проценты).
        /// </summary>
        /// <param name="expr">Выражение в текстовом виде.</param>
        /// <returns>Обработанное выражение для вычислений.</returns>
        private string PrepareExpression(string expr)
        {
            expr = expr.Replace(" ", "");

            expr = Regex.Replace(expr, @"(\d+(\.\d+)?)!", m =>
            {
                string number = m.Groups[1].Value;
                return $"factorial({number})";
            });

            expr = Regex.Replace(expr, @"√(\([^()]*\)|factorial\(\d+(\.\d+)?\)|\d+(\.\d+)?)(\^(-?\d+(\.\d+)?|\([^()]*\)))?", m =>
            {
                string baseExpr = m.Groups[1].Value;
                string exponent = m.Groups[4].Success ? m.Groups[4].Value : "";
                string sqrtExpr = $"sqrt({baseExpr})";
                return string.IsNullOrEmpty(exponent) ? sqrtExpr : $"{sqrtExpr}{exponent}";
            });

            expr = Regex.Replace(expr, @"(\d+(\.\d+)?|\([^()]*\)|sqrt\([^()]*\))%", "($1*0.01)");

            string powerPattern = @"(\d+(\.\d+)?|factorial\(\d+(\.\d+)?\)|\([^()]*\)(!)?|sqrt\([^()]*\)(!)?)(\^(-?\d+(\.\d+)?|factorial\(\d+(\.\d+)?\)|\([^()]*\)(!)?|sqrt\([^()]*\)(!)?)){1,}";

            expr = Regex.Replace(expr, powerPattern, m => TransformPowers(m.Value));

            expr = Regex.Replace(expr, @"(?<![\w.])(\d+)(?![\w.])", "$1.0");

            return expr;
        }



        /// <summary>
        /// Преобразует выражение с операцией возведения в степень
        /// в эквивалентный формат с использованием функции pow(base, exponent).
        /// Например: "2^3^2" → "pow(2,pow(3,2))"
        /// </summary>
        private string TransformPowers(string expr)
        {
            var parts = expr.Split('^').Reverse().ToArray();

            string result = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                result = $"pow({parts[i]},{result})";
            }

            return result;
        }

        /// <summary>
        /// Вычисляет итоговый результат выражения и сохраняет в историю.
        /// </summary>
        private void CalculateResult()
        {
            try
            {
                var prepared = PrepareExpression(Expression);
                var result = _calculator.EvaluateExpression(prepared);

                if ((double.IsInfinity(result) || double.IsNaN(result)) && prepared.Contains("/0"))
                {
                    Result = "Делить на 0 нельзя";
                    return;
                }

                Result = Math.Abs(result) >= 1e15 || Math.Abs(result) <= 1e-15
            ? result.ToString("G6", CultureInfo.InvariantCulture) : Math.Round(result, 6).ToString("0.######", CultureInfo.InvariantCulture);
                _historyViewModel.AddEntry($"{Expression} = {Result}");
            }
            catch
            {
                // чтобы выводило прошлый результат
            }
        }

        /// <summary>
        /// Вычисляет промежуточный результат при вводе.
        /// </summary>
        private void CalculateIntermediateResult()
        {
            if (string.IsNullOrEmpty(Expression))
            {
                Result = string.Empty;
                return;
            }

            try
            {
                var prepared = PrepareExpression(Expression);
                var result = _calculator.EvaluateExpression(prepared);

                if ((double.IsInfinity(result) || double.IsNaN(result)) && prepared.Contains("/0"))
                {
                    Result = "Делить на 0 нельзя";
                    return;
                }

                result = Math.Round(result, 6);

                Result = Math.Abs(result) >= 1e15 || Math.Abs(result) <= 1e-15
            ? result.ToString("G6", CultureInfo.InvariantCulture) : Math.Round(result, 6).ToString("0.######", CultureInfo.InvariantCulture);
            }
            catch
            {
                if (string.IsNullOrEmpty(Expression))
                {
                    Result = string.Empty;
                }
            }
        }

        /// <summary>
        /// Обрабатывает нажатие клавиши клавиатуры.
        /// </summary>
        /// <param name="key">Нажатая клавиша.</param>
        private void HandleKeyPress(Key key)
        {
            bool isShiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

            switch (key)
            {

                case Key.D1: AppendToExpression(isShiftPressed ? "!" : "1"); break;
                case Key.D2: AppendToExpression("2"); break;
                case Key.D3: AppendToExpression("3"); break;
                case Key.D4: AppendToExpression("4"); break;
                case Key.D5: AppendToExpression("5"); break;
                case Key.D6: AppendToExpression("6"); break;
                case Key.D7: AppendToExpression("7"); break;
                case Key.D8: AppendToExpression(isShiftPressed ? "*" : "8"); break;
                case Key.D9: AppendToExpression("9"); break;
                case Key.D0: AppendToExpression("0"); break;
                case Key.OemPlus: AppendToExpression("+"); break;
                case Key.OemMinus: AppendToExpression("-"); break;
                case Key.Multiply: AppendToExpression("*"); break;
                case Key.OemQuestion: AppendToExpression("/"); break;
                case Key.OemPeriod: AppendToExpression("."); break;
                case Key.Enter: CalculateResult(); break;
                case Key.Back: Backspace(); break;
                case Key.Escape: Clear(); break;
            }
        }

        /// <summary>
        /// Применяет процент к текущему выражению.
        /// </summary>
        private void ApplyPercentage() => AppendToExpression("%");

        /// <summary>
        /// Вычисляет факториал числа.
        /// </summary>
        private void ApplyFactorial()
        {
            if (string.IsNullOrWhiteSpace(Expression))
                return;

            var match = Regex.Match(Expression, @"(\d+)$");
            if (!match.Success)
                return;

            Expression = Expression.Insert(match.Index + match.Length, "!");
            CalculateIntermediateResult();
        }

        /// <summary>
        /// Возводит последнее число в выражении в степень.
        /// </summary>
        private void ApplyDegree()
        {
            if (string.IsNullOrWhiteSpace(Expression))
                return;

            string pattern = @"(\d+(\.\d+)?|\([^()]*\))$";
            var match = Regex.Match(Expression, pattern);

            if (match.Success)
            {
                var lastNumber = match.Value;
                Expression = Expression.Substring(0, match.Index) + $"{lastNumber}^";
            }
        }

        /// <summary>
        /// Применяет корень к последнему числу в выражении.
        /// </summary>
        private void ApplySquareRoot()
        {
            if (string.IsNullOrWhiteSpace(Expression))
                return;

            string pattern = @"(\d+(\.\d+)?|\([^()]*\))$";
            var match = Regex.Match(Expression, pattern);

            if (match.Success)
            {
                var lastNumber = match.Value;


                if (lastNumber.StartsWith("√") || Expression.EndsWith($"√{lastNumber}"))
                    return;

                Expression = Expression.Substring(0, match.Index) + $"√{lastNumber}";
                CalculateIntermediateResult();
            }
        }

        /// <summary>
        /// Переключает знак текущего числа (плюс или минус).
        /// </summary>
        private void ToggleSign()
        {
            if (string.IsNullOrEmpty(Expression))
            {
                if (!string.IsNullOrEmpty(Result))
                {
                    if (double.TryParse(Result, out double number))
                    {
                        Result = (-number).ToString(CultureInfo.InvariantCulture);
                        Expression = Result;
                    }
                }
                return;
            }

            try
            {
                var prepared = PrepareExpression(Expression);
                var currentValue = _calculator.EvaluateExpression(prepared);

                var invertedValue = -currentValue;
                Expression = invertedValue.ToString(CultureInfo.InvariantCulture);
                Result = Expression;
            }
            catch
            {
                if (!Expression.StartsWith("-"))
                {
                    Expression = "-" + Expression;
                }
                else
                {
                    Expression = Expression.Substring(1);
                }
                CalculateIntermediateResult();
            }
        }

        /// <summary>
        /// Очищает только текущее выражение.
        /// </summary>
        private void ClearEntry() => Expression = string.Empty;

        /// <summary>
        /// Полностью очищает и выражение, и результат.
        /// </summary>
        private void Clear()
        {
            Expression = string.Empty;
            Result = string.Empty;
        }

        /// <summary>
        /// Удаляет последний символ из текущего выражения.
        /// </summary>
        private void Backspace()
        {
            if (Expression.Length > 0)
            {
                Expression = Expression.Substring(0, Expression.Length - 1);
                CalculateIntermediateResult();
            }
        }
    }
}