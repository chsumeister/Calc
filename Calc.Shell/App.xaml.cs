using Calc.Model;
using Calc.View;
using Calc.ViewModel;
using Calc.ViewModels;
using HistoryV.View;
using Microsoft.Extensions.Configuration;
using Prism.DryIoc;
using Prism.Ioc;
using System.IO;
using System.Windows;

namespace Calc.Shell
{
    public partial class App : PrismApplication
    {
        public static IConfiguration Configuration { get; private set; }

        public App()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            Configuration = builder.Build();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<ShellModel>();
            containerRegistry.RegisterSingleton<HistoryViewModel>();
            containerRegistry.Register<ShellViewModel>();

            containerRegistry.RegisterForNavigation<ShellView>();
            containerRegistry.RegisterForNavigation<HistoryView, HistoryViewModel>();
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<ShellView>();
        }

        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();

            Prism.Mvvm.ViewModelLocationProvider.Register<ShellView, ShellViewModel>();
        }
    }
}
