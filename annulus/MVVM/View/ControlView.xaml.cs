using System;
using System.Windows;
using System.Windows.Controls;
using LibVLCSharp.Shared;
using LibVLCSharp.WPF;
using annulus.Core;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;

namespace annulus.MVVM.View
{
    public partial class ControlView : UserControl
    {
        public ControlView()
        {
            InitializeComponent();
            Loaded += (s, e) => {
                MediaManager.Instance.OpenNetworkStream();
                MediaManager.Instance.PlaySelectedCamera();
                MediaManager.Instance.BindMediaPlayerToView(MediaManager.Instance.GetMediaPlayer0(), videoView0);
                MediaManager.Instance.BindMediaPlayerToView(MediaManager.Instance.GetMediaPlayer1(), videoView1);
            };
            Unloaded += (s, e) => {
                MediaManager.Instance.StopAllMediaPlayers();
                MediaManager.Instance.UnbindMediaPlayerFromView(videoView0);
                MediaManager.Instance.UnbindMediaPlayerFromView(videoView1);
            };
            //DisplayFeed();
        }

        private void DisplayFeed()
        {
            videoView0.MediaPlayer = MediaManager.Instance.GetMediaPlayer0();
            if (MediaManager.Instance.IsPlaying) 
            {
                videoView1.MediaPlayer = MediaManager.Instance.GetMediaPlayer1();
            }
        }

        private void CaptureSnapshot0_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer mediaPlayer = MediaManager.Instance.GetMediaPlayer0();
            bool result = MediaManager.Instance.TakeSnapshot(mediaPlayer, "main");
            if (result)
            {
                //MessageBox.Show("Snapshot taken successfully!");
            }
            else
            {
                MessageBox.Show("Failed to take snapshot.");
            }
        }
        private void CaptureSnapshot1_Click(object sender, RoutedEventArgs e)
        {
            MediaPlayer mediaPlayer = MediaManager.Instance.GetMediaPlayer1();
            bool result = MediaManager.Instance.TakeSnapshot(mediaPlayer, "debris");
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
