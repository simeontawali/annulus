using System.Windows.Controls;
using System;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using XInputDotNetPure; // Make sure to reference XInputDotNetPure
using annulus.MVVM.ViewModel;
using annulus.MVVM.View;
using annulus.Core;


namespace annulus.MVVM.View
{
    /// <summary>
    /// Interaction logic for HomeView.xaml
    /// </summary>
    public partial class HomeView : UserControl
    {
        private DispatcherTimer _timer;

        public HomeView()
        {
            InitializeComponent();
            sourceSelector.Items.Add("Click refresh for cameras");
            //_timer = new DispatcherTimer();
            //_timer.Interval = TimeSpan.FromMilliseconds(100); // Update rate
            //_timer.Tick += Timer_Tick;
            //_timer.Start();

        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            GamePadState state = GamePad.GetState(PlayerIndex.One);
            if (state.IsConnected)
            {
                sb.AppendLine($"LeftThumb: {state.ThumbSticks.Left.X}");
                sb.AppendLine($"RightThumb: {state.ThumbSticks.Right.X}");
                sb.AppendLine($"LeftTrigger: {state.Triggers.Left}");
                sb.AppendLine($"RightTrigger: {state.Triggers.Right}");
                sb.AppendLine($"Buttons: {state.Buttons.A}");
            }
            else
            {
                sb.AppendLine("Controller not connected.");
            }

            txtControllerInput.Text = sb.ToString();
        }

        public void RefreshCameras()
        {
            MediaManager.Instance.RefreshCameras();
            MediaManager.Instance.IsPlaying = false;
            sourceSelector.Items.Clear();
            updateComboBox();
            sourceSelector.SelectedIndex = -1;
        }
        public void updateComboBox()
        {
            List<String> cameras = MediaManager.Instance.GetCameras();
            foreach (String cameraName in cameras)
            {
                sourceSelector.Items.Add(cameraName);
            }
        }
        private void RefreshCameras_Click(object sender, RoutedEventArgs e)
        {
            RefreshCameras();
            sourceSelector.SelectedIndex = -1;
            MediaManager.Instance.IsPlaying = false;
        }
        private void StopCameraFeed_Click(object sender, RoutedEventArgs e)
        {
            sourceSelector.SelectedIndex = -1;
            MediaManager.Instance.StopAllMediaPlayers();
            MediaManager.Instance.IsPlaying = false;
        }

        private void SourceSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedItem = sourceSelector.SelectedItem as string; // MRL is directly used
            if (selectedItem != null && selectedItem != "Click refresh for cameras")
            {
                MediaManager.Instance.selectedCamera = selectedItem;
                MediaManager.Instance.IsPlaying = true;
                //MediaManager.Instance.PlaySelectedCamera();
                //MediaManager.Instance.OpenNetworkStream();
            }
        }
        private void OpenFolderButton_Click(object sender, EventArgs e)
        {
            MediaManager.Instance.OpenSnapshotFolder();
        }

    }
}
