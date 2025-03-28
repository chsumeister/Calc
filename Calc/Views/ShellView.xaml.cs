using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using curc_c_.ViewModels;

namespace curc_c_.Views
{
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
        }

        // передает нажатые кнопки в шеллвьюмодел для бизнес-логики(обработка данных и тп)
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.HandleKeyPress(e.Key);
        }

        private void ShowHistory_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.ShowHistoryWindow();
        }

        private void Percentage_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.Percentage();
        }

        private void ClearEntry_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.ClearEntry();
        }

        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.Clear();
        }

        private void Backspace_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.Backspace();
        }

        private void Reciprocal_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.Reciprocal();
        }

        private void Degree_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.Degree();
        }

        private void Square_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.Square();
        }

        private void Divide_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.AppendToExpression("/");
        }

        private void Number_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var viewModel = DataContext as ShellViewModel;
            viewModel?.AppendToExpression(button.Content.ToString());
        }

        private void Multiply_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.AppendToExpression("*");
        }

        private void Subtract_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.AppendToExpression("-");
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.AppendToExpression("+");
        }

        private void ToggleSign_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.ToggleSign();
        }

        private void Decimal_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.AppendToExpression(".");
        }

        private void Equals_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as ShellViewModel;
            viewModel?.CalculateResult();
        }
    }
}