using System.Windows.Controls;
using System;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using XInputDotNetPure; // Make sure to reference XInputDotNetPure


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
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100); // Update rate
            _timer.Tick += Timer_Tick;
            _timer.Start();

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

    }
}
