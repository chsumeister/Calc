using curc_c_.Model;
using curc_c_.Views;
using System.Windows;
using System.Windows.Input;
namespace curc_c_.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private readonly Calculator _calculator = new Calculator();
        private string _expression = string.Empty;
        private string _result = string.Empty;
        private readonly HistoryViewModel _historyViewModel;

        // конструктор с подпиской на события для истории
        public ShellViewModel(HistoryViewModel historyViewModel)
        {
            _historyViewModel = historyViewModel;
            _historyViewModel.HistoryEntrySelected += FromHistoryToCalc;
        }
        // переносит из истории обратно в калькулятор
        private void FromHistoryToCalc(string entry)
        {
            var expression = entry.Split('=')[0].Trim();
            Expression = expression;
            CalculateIntermediateResult();
        }
        // выражение
        public string Expression
        {
            get => _expression;
            set => SetProperty(ref _expression, value);
        }
        // результат
        public string Result
        {
            get => _result;
            set => SetProperty(ref _result, value);
        }

        //добавляет символ в выражение (expression) для того, чтобы пересчитывать сразу по типу 1+1+1+1
        public void AppendToExpression(string value)
        {
            Expression += value;
            CalculateIntermediateResult();
        }

        //подсчитывает результат
        public void CalculateResult()
        {
            try
            {
                var result = _calculator.EvaluateExpression(Expression);
                Result = result.ToString();

                // переносит выраж и результат в историю
                _historyViewModel.AddEntry($"{Expression} = {Result}");
            }
            catch
            {
                Result = "Error";
            }
        }
        //подсчитывает промежуточный результат 
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
        // это чтобы с клавиатуры вводилось в калькулятор
        public void HandleKeyPress(Key key)
        {
            // Проверяем состояние Shift
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
                        AppendToExpression("*"); // тут shift+8 для умножения, думаю понятно
                    else
                        AppendToExpression("8");
                    break;
                case Key.D9:
                    AppendToExpression("9");
                    break;
                case Key.OemPlus:
                    AppendToExpression("+");
                    break;
                case Key.OemMinus:
                    AppendToExpression("-");
                    break;
                case Key.OemQuestion:
                    AppendToExpression("/");
                    break;
                case Key.OemPeriod:
                    AppendToExpression(".");
                    break;
                //это управление типо энтер это равно бекспейс это 1 символ удалить а эскейп все выражение
                case Key.Enter:
                    CalculateResult();
                    break;
                case Key.Back:
                    if (Expression.Length > 0)
                    {
                        Expression = Expression.Substring(0, Expression.Length - 1);
                    }
                    break;
                case Key.Escape:
                    Expression = string.Empty;
                    Result = string.Empty;
                    break;
            }
        }
        // в проценты
        public void Percentage()
        {
            if (double.TryParse(Expression, out double number))
            {
                Expression = (number / 100).ToString();
            }
        }
        // очистка выражения
        public void ClearEntry()
        {
            Expression = string.Empty;
        }

        public void Clear()
        {
            Expression = string.Empty;
            Result = string.Empty;
        }
        // последний введеный символ удаляет
        public void Backspace()
        {
            if (Expression.Length > 0)
            {
                Expression = Expression.Substring(0, Expression.Length - 1);
            }
        }
        // это 1/x
        public void Reciprocal()
        {
            if (double.TryParse(Expression, out double number))
            {
                Expression = (1 / number).ToString();
            }
        }
        // это квадрат
        public void Degree()
        {
            if (double.TryParse(Expression, out double number))
            {
                Expression = (number * number).ToString();
            }
        }
        // это корень
        public void Square()
        {
            if (double.TryParse(Expression, out double number))
            {
                Expression = Math.Sqrt(number).ToString();
            }
        }
        // открывает историю
        public void ShowHistoryWindow()
        {
            var historyView = new HistoryView();
            historyView.DataContext = _historyViewModel;
            historyView.Owner = Application.Current.MainWindow;
            historyView.ShowDialog();
        }
        // x = -x
        public void ToggleSign()
        {
            if (double.TryParse(Expression, out double number))
            {
                Expression = (-number).ToString();
            }
        }
    }
}