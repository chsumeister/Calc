using Calc.Shell;
using Calc.ViewModel;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Calc.View
{
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();
            var vm = (ShellViewModel)DataContext;
            vm.ScrollExpressionToEndRequested += ScrollExpressionToEnd;

            try
            {
                double width = double.Parse(App.Configuration["Window:Width"]);
                double height = double.Parse(App.Configuration["Window:Height"]);

                Width = width;
                Height = height;
            }
            catch
            {
                Width = 400;
                Height = 600;
            }
        }

        private void ExpressionScrollViewer_OnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            const double scrollStep = 20;
            var scrollViewer = (ScrollViewer)sender;

            if (e.Delta > 0)
                scrollViewer.ScrollToHorizontalOffset(Math.Max(scrollViewer.HorizontalOffset - scrollStep, 0));
            else
                scrollViewer.ScrollToHorizontalOffset(Math.Min(scrollViewer.HorizontalOffset + scrollStep, scrollViewer.ScrollableWidth));

            e.Handled = true;
        }

        private void ScrollExpressionToEnd()
        {
            Dispatcher.InvokeAsync(() =>
            {
                double maxOffset = ExpressionScrollViewer.ExtentWidth - ExpressionScrollViewer.ViewportWidth;
                if (maxOffset < 0) maxOffset = 0;

                ExpressionScrollViewer.ScrollToHorizontalOffset(maxOffset);
            }, System.Windows.Threading.DispatcherPriority.Background);
        }

    }
}