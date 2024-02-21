
using annulus.MVVM.ViewModel;
using Microsoft.Extensions.DependencyInjection;

namespace annulus.Services
{
    class ViewModelLocator
    {
        private readonly IServiceProvider _provider;
        public ViewModelLocator(IServiceProvider provider)
        {
            _provider = provider;
        }
        public MainViewModel MainViewModel => _provider.GetRequiredService<MainViewModel>();
        public SettingsViewModel SettingsViewModel => _provider.GetRequiredService<SettingsViewModel>();

    }
}
