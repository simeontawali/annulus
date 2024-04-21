using annulus.MVVM.View;
using annulus.MVVM.ViewModel;
using System.Windows;
using System.Windows.Input;
using annulus.Network;
namespace annulus
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        CommandSocket commandSource = new CommandSocket();
        public MainWindow()
        {
            InitializeComponent(); 
             
            commandSource.StartGamepadProcess();
            //IItemsService itemsService = new ItemsService(); // Create or retrieve an instance
            //IWindowManager windowManager = new WindowManager(); // Create or retrieve an instance
            //ViewModelLocator viewModelLocator = new ViewModelLocator(); // Create or retrieve an instance

            //// Now, you can pass the required parameters to the constructor
            //var mainViewModel = new MainViewModel(itemsService, windowManager, viewModelLocator);

            //// Set the DataContext for this window
            //this.DataContext = mainViewModel;
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void ButtonMinimize_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }
        private void ControlRefresh_Click(object sender, RoutedEventArgs e)
        {
            commandSource.RefreshConnection();
        }
        private void WindowStateButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
            {
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            }
            else
            {
                Application.Current.MainWindow.WindowState = WindowState.Normal;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            commandSource.StopGamepadProcess();
            //VideoFeedControl.StopAllVideoFeeds();
            //ControlView.StopCameraFeed();
            Application.Current.Shutdown();
        }
     
    }
}