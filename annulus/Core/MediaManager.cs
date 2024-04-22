using LibVLCSharp.Shared;
using System;
using System.Collections.Generic;
using LibVLCSharp.WPF;
using System.Management;
using System.Windows;
using MediaPlayer = LibVLCSharp.Shared.MediaPlayer;
using System.Diagnostics;
using System.IO;

namespace annulus.Core
{
    internal class MediaManager
    {
        private static MediaManager _instance;
        private static readonly object _lock = new object();
        private LibVLC _libVLC;
        private MediaPlayer _mediaPlayer0;
        private MediaPlayer _mediaPlayer1;
        private List<String> cameras;
        public bool IsPlaying { get; set; }
        public String? selectedCamera;
        public bool switchCamera;
        public MediaManager()
        {
            LibVLCSharp.Shared.Core.Initialize();
            _libVLC = new LibVLC();
            _mediaPlayer0 = new MediaPlayer(_libVLC) { EnableHardwareDecoding = true };
            _mediaPlayer1 = new MediaPlayer(_libVLC) { EnableHardwareDecoding = true };
            cameras = new List<String>();
            IsPlaying = false;
            selectedCamera = null;
            switchCamera = false;
        }
        public static MediaManager Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new MediaManager();
                    }
                    return _instance;
                }
            }
        }

        public MediaPlayer GetMediaPlayer0()
        {
            return _mediaPlayer0;
        }
        public MediaPlayer GetMediaPlayer1()
        {
            return _mediaPlayer1;
        }

        public List<String> GetCameras()
        {
            return cameras;
        }

        public void RefreshCameras()
        {
            StopAllMediaPlayers();
            GetAllConnectedCameras();
        }

        public void PlaySelectedCamera()
        {
            if (selectedCamera != null && IsPlaying)
            {
                OpenCaptureDevice(selectedCamera);
            }
        }

        public void GetAllConnectedCameras()
        {
            cameras.Clear();
            using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE (PNPClass = 'Image' OR PNPClass = 'Camera')"))
            {
                foreach (var device in searcher.Get())
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        cameras.Add(device["Caption"].ToString() ?? "Error: Null Device");
                    });
                }
            }
        }


        public void OpenCaptureDevice(string deviceName)
        {
            StopMediaPlayer1();
            var mediaOptions = new string[] { $"--dshow-vdev={deviceName}",
                "--live-caching=0" ,":dshow-size=1280x720",
                     ":dshow-aspect-ratio=16\\:9", ":dshow-adev=none",":dshow-fps=30"};
            var mediaOptions2 = new string[] { $"--dshow-vdev={deviceName}",
                "--live-caching=0" ,":dshow-size=1920x1080",
                     ":dshow-aspect-ratio=16\\:9", ":dshow-adev=none",":dshow-fps=30"};

            var media = new Media(_libVLC, "dshow://", FromType.FromLocation, mediaOptions);
            _mediaPlayer1.Play(media);
        }

        public void OpenNetworkStream()
        {
            StopMediaPlayer0();
            var hostname = "bpmi.local"; // mDNS hostname for the Raspberry Pi
            var port = "8000";
            var path = "/stream.mjpg";
            var mediaOptions = new string[]
            {
                $"http://{hostname}:{port}{path}",
                ":network-caching=200",
                ":live-caching=0",
                "--video-filter=transform",
                "--transform-type=rotate-180" // This will rotate the video by 180 degrees
            };
            var mediaOptions2 = new string[] 
            {
                $"http://{hostname}:{port}{path}",
                ":network-caching=150",  // May increase if experiencing buffering, but increases latency
                ":live-caching=150",
                "--no-drop-late-frames",
                "--no-skip-frames",
                "--video-filter=deinterlace",  // If your stream needs deinterlacing
                "--deinterlace-mode=yadif",  // Choose a deinterlace method if needed
                "--transform-type=rotate-180" // This will rotate the video by 180 degrees
            };

            var media = new Media(_libVLC, mediaOptions[0], FromType.FromLocation, mediaOptions.Skip(1).ToArray());
            _mediaPlayer0.Play(media);
        }
        public void StopMediaPlayer0()
        {
            if (_mediaPlayer0.IsPlaying)
            {
                _mediaPlayer0.Stop();
            }
        }
        public void StopMediaPlayer1()
        {
            if (_mediaPlayer1.IsPlaying)
            {
                _mediaPlayer1.Stop();
            }
        }

        public void StopAllMediaPlayers()
        {
            StopMediaPlayer0();
            StopMediaPlayer1();
        }
        public void BindMediaPlayerToView(MediaPlayer mediaPlayer, VideoView videoView)
        {
            videoView.MediaPlayer = mediaPlayer;
        }

        public void UnbindMediaPlayerFromView(VideoView videoView)
        {
            videoView.MediaPlayer = null;
        }

        public bool TakeSnapshot(MediaPlayer mediaPlayer, String camera)
        {
            if (mediaPlayer == null)
            {
                return false;
            }

            try
            {
                string dateTimeFormat = "yyyyMMdd_HHmmss";
                string fileName = $"{camera}_snapshot_{DateTime.Now.ToString(dateTimeFormat)}.jpg";
                string snapshotsFolder = "Snapshots";
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string snapshotDirectory = Path.Combine(baseDirectory, snapshotsFolder);
                if (!Directory.Exists(snapshotDirectory))
                {
                    Directory.CreateDirectory(snapshotDirectory);
                }
                // Combine the directory and file name
                string filePath = Path.Combine(snapshotDirectory, fileName);

                // Taking a snapshot of the current video frame.
                return mediaPlayer.TakeSnapshot(0,filePath, 0, 0);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it accordingly.
                Debug.WriteLine("Failed to take snapshot: " + ex.Message);
                return false;
            }
        }

        public void OpenSnapshotFolder()
        {
            string snapshotsFolder = "Snapshots";
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string snapshotDirectory = Path.Combine(baseDirectory, snapshotsFolder);

            // Ensure the directory exists before trying to open it
            if (!Directory.Exists(snapshotDirectory))
            {
                Directory.CreateDirectory(snapshotDirectory);
            }

            try
            {
                // Open the folder in Windows Explorer
                Process.Start("explorer.exe", snapshotDirectory);
            }
            catch (Exception ex)
            {
                // Log the error or alert the user
                Console.WriteLine("Unable to open the folder: " + ex.Message);
            }
        }


        public void close()
        {
            StopAllMediaPlayers();
            _libVLC.Dispose();
            _mediaPlayer0.Dispose();
            _mediaPlayer1.Dispose();
        }


    }
}
