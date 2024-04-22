using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using System;
using System.Management;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using annulus;
using annulus.Core;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;


namespace annulus.MVVM.View
{
    /// <summary>
    /// Interaction logic for CameraView.xaml
    /// </summary>
    public partial class CameraView : UserControl
    {
        public CameraView()
        {
            InitializeComponent();
            Loaded += (s, e) => {
                if (MediaManager.Instance.switchCamera)
                {
                    MediaManager.Instance.OpenNetworkStream();
                }
                else
                {
                    MediaManager.Instance.PlaySelectedCamera();
                }
                MediaManager.Instance.BindMediaPlayerToView(MediaManager.Instance.switchCamera ? MediaManager.Instance.GetMediaPlayer0() : MediaManager.Instance.GetMediaPlayer1(), videoView);
                };
            Unloaded += (s, e) => {
                MediaManager.Instance.StopAllMediaPlayers();
                MediaManager.Instance.UnbindMediaPlayerFromView(videoView);
            };
        }

        private void SwitchCameras_Click(object sender, RoutedEventArgs e)
        {
            MediaManager.Instance.switchCamera = !MediaManager.Instance.switchCamera;
            MediaManager.Instance.StopAllMediaPlayers();
            MediaManager.Instance.UnbindMediaPlayerFromView(videoView);
            if (MediaManager.Instance.switchCamera)
            {
                MediaManager.Instance.OpenNetworkStream();
            }
            else
            {
                MediaManager.Instance.PlaySelectedCamera();
            }
            MediaManager.Instance.BindMediaPlayerToView(MediaManager.Instance.switchCamera ? MediaManager.Instance.GetMediaPlayer0() : MediaManager.Instance.GetMediaPlayer1(), videoView);
            //var current = videoView.MediaPlayer == MediaManager.Instance.GetMediaPlayer0() ? MediaManager.Instance.GetMediaPlayer1() : MediaManager.Instance.GetMediaPlayer0();
            //MediaManager.Instance.BindMediaPlayerToView(current, videoView);
            //play();
        }
        private void play()
        {
            if (MediaManager.Instance.switchCamera && MediaManager.Instance.IsPlaying)
            {
                videoView.MediaPlayer = MediaManager.Instance.GetMediaPlayer0();
            }
            else
            {
                videoView.MediaPlayer = MediaManager.Instance.GetMediaPlayer1();
            }
        }
        private void Screenshot_Click(object sender, RoutedEventArgs e)
        {
            String fileName = MediaManager.Instance.switchCamera ? "main" : "debris";
            MediaPlayer mediaPlayer = MediaManager.Instance.switchCamera ? MediaManager.Instance.GetMediaPlayer0() : MediaManager.Instance.GetMediaPlayer1();
            bool result = MediaManager.Instance.TakeSnapshot(mediaPlayer, fileName);
            if (result)
            {
                //MessageBox.Show("Snapshot taken successfully!");
            }
            else
            {
                MessageBox.Show("Failed to take snapshot.");
            }
        }

    }
}
