using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Media;
using Calc.Model;
using Calc.Shell;
using Calc.ViewModels;
using Prism.Commands;
using Prism.Mvvm;

namespace Calc.ViewModel
{
    public class ShellViewModel : BindableBase
    {
        private readonly ShellModel _calculator;
        private readonly HistoryViewModel _historyViewModel;
        private string _expression = string.Empty;
        private string _result = string.Empty;
        private bool _isHistoryVisible;

        private Brush _expressionColor;
        public Brush ExpressionColor
        {
            get => _expressionColor;
            set => SetProperty(ref _expressionColor, value);
        }

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
            ReciprocalCommand = new DelegateCommand(ApplyReciprocal);
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
        public ICommand ReciprocalCommand { get; }
        public ICommand DegreeCommand { get; }
        public ICommand SquareCommand { get; }
        public ICommand ToggleSignCommand { get; }

        public string Expression
        {
            get => _expression;
            set => SetProperty(ref _expression, value);
        }

        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        public bool IsHistoryVisible
        {
            get => _isHistoryVisible;
            set => SetProperty(ref _isHistoryVisible, value);
        }

        public HistoryViewModel HistoryViewModel => _historyViewModel;

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
            if (string.IsNullOrEmpty(value)) return;

            string[] operators = { "+", "-", "*", "/" };

            if (Expression.Length > 0 &&
                operators.Contains(Expression.Last().ToString()) &&
                operators.Contains(value))
                return;

            if (Expression.Length == 0 && value == "-")
            {
                Expression += value;
                return;
            }

            Expression += value;
            CalculateIntermediateResult();
        }

        /// <summary>
        /// Подготавливает выражение для вычисления (корни, степени, проценты).
        /// </summary>
        /// <param name="expr">Выражение в текстовом виде.</param>
        /// <returns>Обработанное выражение для вычислений.</returns>
        private string PrepareExpression(string expr)
        {
            expr = expr.Replace(" ", "");

            expr = Regex.Replace(expr, @"√(\d+(\.\d+)?|\([^()]*\))", "sqrt($1)");

            expr = Regex.Replace(expr, @"(\d+(\.\d+)?|\([^()]*\))\^(\d+(\.\d+)?)?", m =>
            {
                var baseVal = m.Groups[1].Value;
                var exponent = m.Groups[3].Success ? m.Groups[3].Value : "";
                return exponent == "" ? $"pow({baseVal}," : $"pow({baseVal},{exponent})";
            });

            expr = Regex.Replace(expr, @"(\d+(\.\d+)?|\([^()]*\))%", "($1*0.01)");

            return expr;
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

                if (double.IsInfinity(result) || double.IsNaN(result))
                {
                    Result = "Делить на 0 нельзя";
                    return;
                }

                Result = result.ToString();
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
            try
            {
                var prepared = PrepareExpression(Expression);
                var result = _calculator.EvaluateExpression(prepared);

                if (double.IsInfinity(result) || double.IsNaN(result))
                {
                    Result = "Делить на 0 нельзя";
                    return;
                }

                Result = result.ToString();
            }
            catch
            {
                // чтобы выводило прошлый результат
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

                case Key.D1: AppendToExpression("1"); break;
                case Key.D2: AppendToExpression("2"); break;
                case Key.D3: AppendToExpression("3"); break;
                case Key.D4: AppendToExpression("4"); break;
                case Key.D5: AppendToExpression("5"); break;
                case Key.D6: AppendToExpression("6"); break;
                case Key.D7: AppendToExpression("7"); break;
                case Key.D8: AppendToExpression(isShiftPressed ? "*" : "8"); break;
                case Key.D9: AppendToExpression(isShiftPressed ? "(" : "9"); break;
                case Key.D0: AppendToExpression(isShiftPressed ? ")" : "0"); break;
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
        /// Применяет операцию обратного значения (1/x).
        /// </summary>
        private void ApplyReciprocal()
        {
            if (!string.IsNullOrEmpty(Expression))
            {
                Expression = $"1/({Expression})";
                CalculateIntermediateResult();
            }
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
                Expression = Expression.Substring(0, match.Index) + $"√{lastNumber}";
                CalculateIntermediateResult();
            }
        }

        /// <summary>
        /// Переключает знак текущего числа (плюс или минус).
        /// </summary>
        private void ToggleSign()
        {
            if (double.TryParse(Expression, out double number))
            {
                Expression = (-number).ToString();
                Result = Expression;
                _historyViewModel.AddEntry($"-({number}) = {Expression}");
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
                Expression = Expression.Substring(0, Expression.Length - 1);
        }
    }
}
