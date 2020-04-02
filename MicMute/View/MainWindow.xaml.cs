using Hardcodet.Wpf.TaskbarNotification;
using MicMute.Properties;
using MicMute.ViewModel;
using System.Windows;

namespace MicMute
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel ViewModel { get; }

        public MainWindow()
        {
            InitializeComponent();

            this.ViewModel = (MainViewModel)this.DataContext;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Window_StateChanged(sender, e);
        }

        private void Window_StateChanged(object sender, System.EventArgs e)
        {
            switch (this.WindowState)
            {
                case System.Windows.WindowState.Minimized:
                    this.ShowInTaskbar = false;
                    break;
                default:
                    this.ShowInTaskbar = true;
                    break;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (this.ViewModel.CanClose == false)
            {
                this.WindowState = System.Windows.WindowState.Minimized;
#if !DEBUG
                this.NotifyIcon.ShowBalloonTip(Properties.Resources.TooltipTitle,
                    Properties.Resources.TooltipMinimized, BalloonIcon.Info);

                e.Cancel = true;
#endif
            }

            Settings.Default.WindowLocation = new Point(this.Left, this.Top);
            Settings.Default.WindowSize = new Size(this.ActualWidth, this.ActualHeight);
            Settings.Default.WindowState = this.WindowState;
        }
    }
}
