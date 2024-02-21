using annulus.MVVM.ViewModel;
using annulus.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;


namespace annulus
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceCollection services = new ServiceCollection();
        private readonly IServiceProvider _serviceProvider;
        public App()
        {
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<SettingsViewModel>();

            services.AddSingleton<ViewModelLocator>();
            services.AddSingleton<WindowMapper>();
            services.AddSingleton<IWindowManager, WindowManager>();
            services.AddSingleton<IItemsService, ItemsService>();

            _serviceProvider = services.BuildServiceProvider();

        }

        protected override void OnStartup(StartupEventArgs startupArgs)
        {
            var windowManager = _serviceProvider.GetRequiredService<IWindowManager>();
            windowManager.ShowWindow(_serviceProvider.GetRequiredService<MainViewModel>());
            base.OnStartup(startupArgs);
        }
    }

}
