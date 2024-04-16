using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace annulus.MVVM.View
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl
    {
        private LibVLC _libVLC;
        private LibVLCSharp.Shared.MediaPlayer _mediaPlayer1;
        private LibVLCSharp.Shared.MediaPlayer _mediaPlayer2;
        public bool isFullscreen = false;
        public bool isPlaying = false;
        public Size oldVideoSize;
        public Size oldFormSize;
        public Point oldVideoLocation;
        public CameraView()
        {
            InitializeComponent();
            LibVLCSharp.Shared.Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer1 = new LibVLCSharp.Shared.MediaPlayer(_libVLC) { EnableHardwareDecoding = true };
            _mediaPlayer2 = new LibVLCSharp.Shared.MediaPlayer(_libVLC) { EnableHardwareDecoding = true };
            videoView1.MediaPlayer = _mediaPlayer1;
            videoView2.MediaPlayer = _mediaPlayer2;
            sourceSelector1.Items.Add("Open Network Stream");
            //sourceSelector2.Items.Add("Open Network Stream");
            GetAllConnectedCameras();
        }
        public void refreshCameras()
        {
            StopCameraFeed();
            sourceSelector1.Items.Clear();
            sourceSelector2.Items.Clear();
            sourceSelector1.Items.Add("Open Network Stream");
            GetAllConnectedCameras();
        }
        private void RefreshCameras_Click(object sender, RoutedEventArgs e)
        {
            refreshCameras();
        }
        private void SourceSelector1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_mediaPlayer1.IsPlaying)
            {
                _mediaPlayer1.Stop();
            }
            var selectedItem = sourceSelector1.SelectedItem as string; // MRL is directly used
            if (selectedItem == "Open Network Stream")
            {
                OpenNetworkStream();
            }
        }
        private void SourceSelector2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_mediaPlayer2.IsPlaying)
            {
                _mediaPlayer2.Stop();
            }
            var selectedItem = sourceSelector2.SelectedItem as string; // MRL is directly used
                OpenCaptureDevice(selectedItem);
        }

        private void GetAllConnectedCameras()
        {
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
            {
                foreach (var device in searcher.Get())
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        sourceSelector2.Items.Add(device["Caption"].ToString());
                    });
                }
            }
        }

        private void OpenCaptureDevice(string deviceName)
        {
            if (_mediaPlayer2.IsPlaying)
            {
                _mediaPlayer2.Stop();
            }
            var mediaOptions = new string[] { $"--dshow-vdev={deviceName}",
                "--live-caching=0" ,":dshow-size=1280x720",
                     ":dshow-aspect-ratio=16\\:9", ":dshow-adev=none",":dshow-fps=30"};
            var media = new Media(_libVLC, "dshow://", FromType.FromLocation, mediaOptions);
            _mediaPlayer2.Play(media);
        }


        private void OpenNetworkStream()
        {
            if (_mediaPlayer1.IsPlaying)
            {
                _mediaPlayer1.Stop();
            }
            // for testing
            //var media = new Media(_libVLC, "http://devimages.apple.com/iphone/samples/bipbop/bipbopall.m3u8", FromType.FromLocation);

            var hostname = "bpmi.local"; // mDNS hostname for the Raspberry Pi
            var port = "8000";
            var path = "/stream.mjpg";
            //var media = new Media(_libVLC, $"http://{hostname}:{port}{path}", FromType.FromLocation);
            //_mediaPlayer.Play(media);
            var mediaOptions = new string[]
            {
                $"http://{hostname}:{port}{path}",
                ":network-caching=200",
                ":live-caching=0",
                "--video-filter=transform",
                "--transform-type=rotate-180" // This will rotate the video by 180 degrees

                };
            var media = new Media(_libVLC, mediaOptions[0], FromType.FromLocation, mediaOptions.Skip(1).ToArray());
            _mediaPlayer1.Play(media);

        }
        private void StopCameraFeed()
        {
            if (_mediaPlayer1.IsPlaying)
            {
                _mediaPlayer1.Stop();
            }
            if (_mediaPlayer2.IsPlaying)
            {
                _mediaPlayer2.Stop();
            }
        }

    }
}
