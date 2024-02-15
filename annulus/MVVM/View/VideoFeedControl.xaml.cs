using System;
using System.Windows.Controls;
using AForge.Video;
using AForge.Video.DirectShow;
using System.Drawing; // Ensure System.Drawing.Common package is referenced
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using System.Windows;

namespace annulus.MVVM.View
{
    /// <summary>
    /// Interaction logic for VideoFeedControl.xaml
    /// </summary>
    public partial class VideoFeedControl : UserControl
    {
        private static List<VideoFeedControl> instances = new List<VideoFeedControl>();
        private VideoCaptureDevice videoSource;

        public VideoFeedControl()
        {
            InitializeComponent();
            instances.Add(this);
        }

        public void StartVideoFeed(string cameraMonikerString)
        {
            videoSource = new VideoCaptureDevice(cameraMonikerString);
            videoSource.NewFrame += VideoSource_NewFrame;
            videoSource.Start();
        }

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr hObject);

        private void VideoSource_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            if (Application.Current != null && !Application.Current.Dispatcher.HasShutdownStarted)
            {
                Application.Current.Dispatcher.Invoke(() => {
                    if (videoSourcePlayer != null && eventArgs.Frame != null)
                    {
                        var bmp = (System.Drawing.Bitmap)eventArgs.Frame.Clone();
                        var handle = bmp.GetHbitmap();
                        try
                        {
                            var bitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                                handle,
                                IntPtr.Zero,
                                System.Windows.Int32Rect.Empty,
                                System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

                            videoSourcePlayer.Source = bitmapSource;
                        }
                        finally
                        {
                            DeleteObject(handle); // Properly dispose of the HBitmap
                            bmp.Dispose();
                        }
                    }
                }, System.Windows.Threading.DispatcherPriority.Normal);
            }
        }

        public static void StopAllVideoFeeds()
        {
            foreach (var instance in instances)
            {
                instance.StopVideoFeed();
            }
        }

        public void StopVideoFeed()
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource.NewFrame -= VideoSource_NewFrame;
            }
        }

        // Destructor to remove the instance from the list
        ~VideoFeedControl()
        {
            instances.Remove(this);
        }
    }
}
