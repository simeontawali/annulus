using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using AForge.Video.DirectShow;


namespace annulus.MVVM.View
{
    /// <summary>
    /// Interaction logic for USBCameraView.xaml
    /// </summary>
    public partial class USBCameraView : UserControl
    {
        private FilterInfoCollection videoDevices;
        public USBCameraView()
        {
            InitializeComponent();
            Loaded += ControlView_Loaded;
        }
        private void ControlView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            // Enumerate video devices
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);
            //cameraSelectionComboBox.ItemsSource = videoDevices;
            if (videoDevices.Count > 0)
            {
                //cameraSelectionComboBox.SelectedIndex = 0; // Automatically select the first camera
            }
        }

        private void CameraSelectionComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (true)   //cameraSelectionComboBox.SelectedItem is FilterInfo selectedCamera)
            {
                StopVideoFeed();
                //StartVideoFeed(selectedCamera.MonikerString);
            }
            else
            {
                StopVideoFeed();
            }
        }



        public void StartVideoFeed(string cameraMonikerString)
        {
            videoFeedControl.StartVideoFeed(cameraMonikerString);
        }

        public void StopVideoFeed()
        {
            videoFeedControl.StopVideoFeed();
            VideoFeedControl.StopAllVideoFeeds();
        }

        private void VideoStreamButton_Click(object sender, RoutedEventArgs e)
        {
            //string ip = ipAddressTextBox.Text;
            //string port = portTextBox.Text;
            //string url = $"http://{ip}:{port}/index.html";

            //videoFeedBrowser.Navigate(new Uri(url));

            // <WebBrowser x:Name="videoFeedBrowser" Margin="10"/>
        }
    }
}
