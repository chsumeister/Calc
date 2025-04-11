using System;
using System.Windows;
using System.Windows.Input;
using curc_c_.Model;
using Prism.Commands;
using curc_c_.Views;
using Prism.Mvvm;

namespace curc_c_.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private readonly Calculator _calculator = new Calculator();
        private string _expression = string.Empty;
        private string _result = string.Empty;
        private readonly HistoryViewModel _historyViewModel;

        public ICommand AppendToExpressionCommand { get; }
        public ICommand CalculateResultCommand { get; }
        public ICommand HandleKeyPressCommand { get; }
        public ICommand ShowHistoryWindowCommand { get; }
        public ICommand PercentageCommand { get; }
        public ICommand ClearEntryCommand { get; }
        public ICommand ClearCommand { get; }
        public ICommand BackspaceCommand { get; }
        public ICommand ReciprocalCommand { get; }
        public ICommand DegreeCommand { get; }
        public ICommand SquareCommand { get; }
        public ICommand ToggleSignCommand { get; }

        public ShellViewModel(HistoryViewModel historyViewModel)
        {
            _historyViewModel = historyViewModel;
            _historyViewModel.HistoryEntrySelected += FromHistoryToCalc;

            AppendToExpressionCommand = new DelegateCommand<string>(AppendToExpression);
            CalculateResultCommand = new DelegateCommand(CalculateResult);
            HandleKeyPressCommand = new DelegateCommand<object>(param => HandleKeyPress((Key)param));
            ShowHistoryWindowCommand = new DelegateCommand(ShowHistoryWindow);
            PercentageCommand = new DelegateCommand(Percentage);
            ClearEntryCommand = new DelegateCommand(ClearEntry);
            ClearCommand = new DelegateCommand(Clear);
            BackspaceCommand = new DelegateCommand(Backspace);
            ReciprocalCommand = new DelegateCommand(Reciprocal);
            DegreeCommand = new DelegateCommand(Degree);
            SquareCommand = new DelegateCommand(Square);
            ToggleSignCommand = new DelegateCommand(ToggleSign);

            
        }

        /// <summary>
        /// Переносит выражение из истории в калькулятор
        /// </summary>
        /// <param name="entry">Запись из истории в формате "выражение = результат"</param>
        private void FromHistoryToCalc(string entry)
        {
            var expression = entry.Split('=')[0].Trim();
            Expression = expression;
            CalculateIntermediateResult();
        }

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

        /// <summary>
        /// Добавляет символ к текущему выражению
        /// </summary>
        /// <param name="value">Добавляемый символ</param>
        public void AppendToExpression(string value)
        {
            Expression += value;
            CalculateIntermediateResult();
        }

        /// <summary>
        /// Вычисляет и отображает результат выражения
        /// </summary>
        public void CalculateResult()
        {
            try
            {
                var result = _calculator.EvaluateExpression(Expression);
                Result = result.ToString();
                _historyViewModel.AddEntry($"{Expression} = {Result}");
            }
            catch
            {
                Result = "Error";
            }
        }

        /// <summary>
        /// Вычисляет промежуточный результат выражения
        /// </summary>
        private void CalculateIntermediateResult()
        {
            try
            {
                var result = _calculator.EvaluateExpression(Expression);
                Result = result.ToString();
            }
            catch
            {
                Result = "Error";
            }
        }

        /// <summary>
        /// Обрабатывает нажатия клавиш клавиатуры
        /// </summary>
        /// <param name="key">Нажатая клавиша</param>
        public void HandleKeyPress(Key key)
        {
            bool isShiftPressed = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift;

            switch (key)
            {
                case Key.D0:
                    AppendToExpression("0");
                    break;
                case Key.D1:
                    AppendToExpression("1");
                    break;
                case Key.D2:
                    AppendToExpression("2");
                    break;
                case Key.D3:
                    AppendToExpression("3");
                    break;
                case Key.D4:
                    AppendToExpression("4");
                    break;
                case Key.D5:
                    AppendToExpression("5");
                    break;
                case Key.D6:
                    AppendToExpression("6");
                    break;
                case Key.D7:
                    AppendToExpression("7");
                    break;
                case Key.D8:
                    if (isShiftPressed)
                    {
                        AppendToExpression("*");
                        return;
                    }
                    AppendToExpression("8");
                    break;
                case Key.D9:
                case Key.NumPad9:
                    AppendToExpression("9");
                    break;
                case Key.Add:
                case Key.OemPlus:
                    AppendToExpression("+");
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    AppendToExpression("-");
                    break;
                case Key.Multiply:
                    AppendToExpression("*");
                    break;
                case Key.Divide:
                case Key.OemQuestion:
                    AppendToExpression("/");
                    break;
                case Key.Decimal:
                case Key.OemPeriod:
                    AppendToExpression(".");
                    break;
                case Key.Enter:
                    CalculateResult();
                    break;
                case Key.Back:
                    Backspace();
                    break;
                case Key.Escape:
                    Clear();
                    break;
            }
        }

        /// <summary>
        /// Преобразует текущее выражение в проценты
        /// </summary>
        public void Percentage()
        {
            if (double.TryParse(Expression, out double number))
            {
                Expression = (number / 100).ToString();
            }
        }

        /// <summary>
        /// Очищает текущее выражение
        /// </summary>
        public void ClearEntry()
        {
            Expression = string.Empty;
        }
        /// <summary>
        /// Полностью очищает калькулятор (выражение и результат)
        /// </summary>
        public void Clear()
        {
            Expression = string.Empty;
            Result = string.Empty;
        }

        /// <summary>
        /// Удаляет последний символ из выражения
        /// </summary>
        public void Backspace()
        {
            if (Expression.Length > 0)
            {
                Expression = Expression.Substring(0, Expression.Length - 1);
            }
        }

        /// <summary>
        /// Вычисляет обратную величину текущего числа
        /// </summary>
        public void Reciprocal()
        {
            if (double.TryParse(Expression, out double number))
            {
                Expression = (1 / number).ToString();
            }
        }

        /// <summary>
        /// Возводит текущее число в квадрат
        /// </summary>
        public void Degree()
        {
            if (double.TryParse(Expression, out double number))
            {
                Expression = (number * number).ToString();
            }
        }

        /// <summary>
        /// Вычисляет квадратный корень из текущего числа
        /// </summary>
        public void Square()
        {
            if (double.TryParse(Expression, out double number))
            {
                Expression = Math.Sqrt(number).ToString();
            }
        }

        /// <summary>
        /// Открывает окно истории вычислений
        /// </summary>
        public void ShowHistoryWindow()
        {
            var historyView = new HistoryView();
            historyView.DataContext = _historyViewModel;
            historyView.Owner = Application.Current.MainWindow;
            historyView.ShowDialog();
        }

        /// <summary>
        /// Меняет знак текущего числа
        /// </summary>
        public void ToggleSign()
        {
            if (double.TryParse(Expression, out double number))
            {
                Expression = (-number).ToString();
            }
        }
    }
}