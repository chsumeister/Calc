using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using curc_c_.ViewModels;

namespace curc_c_.Views
{
    public partial class HistoryView : Window
    {
        public HistoryView()
        {
            InitializeComponent();
        }
        // передает кнопки в историю для бизнес-логики
        private void HistoryItem_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            var listBoxItem = sender as ListBoxItem;
            if (listBoxItem != null)
            {
                this.Close();
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void ClearHistory_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as HistoryViewModel;
            viewModel?.ClearHistory();
        }
    }
}