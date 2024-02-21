using annulus.Core;
using annulus.Services;
using System;

namespace annulus.MVVM.ViewModel
{
    class MainViewModel : ObservableObject
    {

        public RelayCommand HomeViewCommand { get; set; }
        public RelayCommand ControlViewCommand { get; set; }
        public RelayCommand CameraViewCommand { get; set; }
        public RelayCommand DeviceViewCommand { get; set; }


        public HomeViewModel HomeVM { get; set; }
        public ControlViewModel ControlVM { get; set; }
        public CameraViewModel CameraVM { get; set; }
        public DeviceViewModel DeviceVM { get; set; }



        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        private readonly IWindowManager _windowManager;
        private readonly ViewModelLocator _viewModelLocator;
        public IItemsService ItemsService { get; set; }
        public RelayCommand OpenSettingsWindowCommand { get; set; }

        public MainViewModel(IItemsService itemsService, IWindowManager WindowManager, ViewModelLocator viewModelLocator)
        {
            _windowManager = WindowManager;
            _viewModelLocator = viewModelLocator;
            ItemsService = itemsService;
            OpenSettingsWindowCommand = new RelayCommand(o => { _windowManager.ShowWindow(_viewModelLocator.SettingsViewModel); }, o => true);

            HomeVM = new HomeViewModel();
            ControlVM = new ControlViewModel();
            CameraVM = new CameraViewModel();
            DeviceVM = new DeviceViewModel();


            CurrentView = HomeVM;

            HomeViewCommand = new RelayCommand(o =>
            {
                CurrentView = HomeVM;
            });

            ControlViewCommand = new RelayCommand(o =>
            {
                CurrentView = ControlVM;
            });

            CameraViewCommand = new RelayCommand(o =>
            {
                CurrentView = CameraVM;
            });

            DeviceViewCommand = new RelayCommand(o =>
            {
                CurrentView = DeviceVM;
            });

        }

    }
}