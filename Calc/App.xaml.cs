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
            // создание связи для вью и вьюмодел
            containerRegistry.RegisterForNavigation<ShellView, ShellViewModel>();
        }

        protected override Window CreateShell()
        {
            // создание главного окна
            return Container.Resolve<ShellView>();
        }
    }
}