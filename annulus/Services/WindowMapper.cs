using System;
using System.Windows;
using annulus.MVVM.ViewModel;
using annulus.MVVM.View;
using annulus.Core;

namespace annulus.Services
{
    class WindowMapper
    {
        private readonly Dictionary<Type, Type> _mappings = new();
        public WindowMapper()
        {
            RegisterMapping<MainViewModel, MainWindow>();
            RegisterMapping<SettingsViewModel, SettingsWindow>();
        }

        public void RegisterMapping<TViewModel, TWindow>() where TViewModel : ObservableObject where TWindow : Window
        {
            _mappings[typeof(TViewModel)] = typeof(TWindow);
        }
        public Type GetWindowTypeForViewModel(Type viewModelType)
        {
            _mappings.TryGetValue(viewModelType, out var windowType);
            return windowType;
        }
    }
}
