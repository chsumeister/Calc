using Calc.Shell;
using System.Windows;

namespace Calc.View
{
    public partial class ShellView : Window
    {
        public ShellView()
        {
            InitializeComponent();

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
    }
}