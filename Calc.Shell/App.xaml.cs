using Calc.Shell.Views;
using Prism.DryIoc;
using Prism.Ioc;
using System.Windows;

namespace Calc.Shell
{
    public partial class App : PrismApplication
    {
        protected override Window CreateShell() => Container.Resolve<ShellView>();

        protected override void RegisterTypes(IContainerRegistry container)
        {
            container.RegisterForNavigation<ShellView>("Calculator");
            container.RegisterForNavigation<HistoryView>("History");
        }
    }
}