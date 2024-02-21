using annulus.Core;
using annulus.Services;
using System;

namespace annulus.MVVM.ViewModel
{
    class ControlViewModel : ObservableObject
    {

        public RelayCommand USBCameraViewCommand { get; set; }
        public RelayCommand WebpageCameraViewCommand { get; set; }
        public USBCameraViewModel USBVM { get; set; }
        public WebpageCameraViewModel WebpageVM { get; set; }
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
        public ControlViewModel()
        {
            
            WebpageVM = new WebpageCameraViewModel();
            USBVM = new USBCameraViewModel();

            CurrentView = WebpageVM;

            USBCameraViewCommand = new RelayCommand(o =>
            {
                CurrentView = USBVM;
            });

            WebpageCameraViewCommand = new RelayCommand(o =>
            {
                CurrentView = WebpageVM;
            });

        }

    }
}
