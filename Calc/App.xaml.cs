using System.Windows;
using Prism.DryIoc;
using Prism.Ioc;
using curc_c_.Views;
using curc_c_.ViewModels;

namespace curc_c_
{
    public partial class App : PrismApplication
    {
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ShellView, ShellViewModel>();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }
    }
}