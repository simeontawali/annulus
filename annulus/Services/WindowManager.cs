using annulus.Core;
using System;
using System.Windows;

namespace annulus.Services
{

    public interface IWindowManager
    {
        void ShowWindow(ObservableObject viewModel);
        void CloseWindow();
    }
    class WindowManager : IWindowManager
    {
        private readonly WindowMapper _windowMapper;
        public WindowManager(WindowMapper windowMapper)
        {
            _windowMapper = windowMapper;
        }
        public void ShowWindow(ObservableObject viewModel)
        {
            var windowType = _windowMapper.GetWindowTypeForViewModel(viewModel.GetType());
            if (windowType != null)
            {
                var window = Activator.CreateInstance(windowType) as Window;
                window.DataContext = viewModel;
                window.Show();
                window.Closed += (sender, args) => CloseWindow();
            }
        }
        public void CloseWindow()
        {
            MessageBox.Show(messageBoxText: "Window Closed");
        }
    }
}
