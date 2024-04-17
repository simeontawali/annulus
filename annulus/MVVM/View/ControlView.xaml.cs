using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System.Management;
using System.Windows.Media;

namespace annulus.MVVM.View
{
    public partial class ControlView : UserControl
    {
        private LibVLC _libVLC;
        private LibVLCSharp.Shared.MediaPlayer _mediaPlayer; 

        //public bool isFullscreen = false;
        //public bool isPlaying = false;
        //public Size oldVideoSize;
        //public Size oldFormSize;
        //public Point oldVideoLocation;
        public ControlView()
        {
            InitializeComponent();
            LibVLCSharp.Shared.Core.Initialize();

            //this.KeyPreview = true;
            //this.KeyDown += new KeyEventHandler(ShortcutEvent);
            //oldVideoSize = videoView.Size;
            //oldFormSize = this.Size;
            //oldVideoLocation = videoView.Location;

            _libVLC = new LibVLC();
            //_mediaPlayer = new MediaPlayer(_libVLC) { EnableHardwareDecoding = true };
            videoView.MediaPlayer = _mediaPlayer;
            sourceSelector.Items.Add("Open Network Stream");
            sourceSelector.Items.Add("Click refresh for cameras");
            //GetAllConnectedCameras();        
        }
        private LibVLCSharp.Shared.MediaPlayer MediaPlayer0
        {
            get
            {
                if (_mediaPlayer == null)
                {
                    _mediaPlayer = new LibVLCSharp.Shared.MediaPlayer(_libVLC) { EnableHardwareDecoding = true };
                    videoView.MediaPlayer = _mediaPlayer;
                }
                return _mediaPlayer;
            }
        }

        public void refreshCameras()
        {
            StopCameraFeed();
            sourceSelector.Items.Clear();
            sourceSelector.Items.Add("Open Network Stream");
            GetAllConnectedCameras();
        }
        private void RefreshCameras_Click(object sender, RoutedEventArgs e)
        {
            refreshCameras();
            sourceSelector.SelectedIndex = -1;
        }
        private void StopCameraFeed_Click(object sender, RoutedEventArgs e)
        {
            sourceSelector.SelectedIndex = -1;
            StopCameraFeed();
        }

        private void SourceSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MediaPlayer0.IsPlaying)
            {
                MediaPlayer0.Stop();
            }
            var selectedItem = sourceSelector.SelectedItem as string; // MRL is directly used
            if (selectedItem == "Open Network Stream")
            {
                OpenNetworkStream();
            }
            else if (selectedItem != null && selectedItem != "Click refresh for cameras")
            {
                OpenCaptureDevice(selectedItem);
            }

            //if (!string.IsNullOrEmpty(selectedItem) && selectedItem.StartsWith("dshow://"))
            //{
            //    OpenCaptureDevice(selectedItem);
            //}
            //else if (selectedItem == "Open Network Stream")
            //{
            //    OpenNetworkStream();
            //}
        }

        //private void GetAllConnectedCameras()
        //{
        //    using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
        //    {
        //        foreach (var device in searcher.Get())
        //        {
        //            Application.Current.Dispatcher.Invoke(() =>
        //            {
        //                sourceSelector.Items.Add(device["Caption"].ToString());
        //            });
        //        }
        //    }
        //}
        private async void GetAllConnectedCameras()
        {
            await Task.Run(() =>
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
                {
                    foreach (var device in searcher.Get())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            sourceSelector.Items.Add(device["Caption"].ToString());
                        });
                    }
                }
            });
        }


        private void OpenCaptureDevice(string deviceName)
        {
            if (MediaPlayer0.IsPlaying)
            {
                MediaPlayer0.Stop();
            }
            var mediaOptions = new string[] { $"--dshow-vdev={deviceName}", 
                "--live-caching=0" ,":dshow-size=1280x720",
                     ":dshow-aspect-ratio=16\\:9", ":dshow-adev=none",":dshow-fps=30"};
            var media = new Media(_libVLC, "dshow://", FromType.FromLocation, mediaOptions);
            MediaPlayer0.Play(media);
        }


        private void OpenNetworkStream()
        {
            if (MediaPlayer0.IsPlaying)
            {
                MediaPlayer0.Stop();
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
            MediaPlayer0.Play(media);

        }
        private void StopCameraFeed()
        {
            if (_mediaPlayer?.IsPlaying == true)
            {
                _mediaPlayer.Stop();
            }
            if (MediaPlayer0.IsPlaying)
            {
                MediaPlayer0.Stop();
            }

        }
    }
}
