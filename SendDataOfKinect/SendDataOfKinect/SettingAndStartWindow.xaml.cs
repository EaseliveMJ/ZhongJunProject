using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace SendDataOfKinect
{
    /// <summary>
    /// SettingAndStartWindow.xaml 的交互逻辑
    /// </summary>
    public partial class SettingAndStartWindow : Window
    {
        private DispatcherTimer timer;
        private Thread thread;
        public SettingAndStartWindow()
        {

            InitializeComponent();
            thread = new Thread(ThreadOne);
            timer = new DispatcherTimer();
            thread.Start();

        }

        private void ThreadOne()
        {
     
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        
        }
        void timer_Tick(object sender, EventArgs e)
        {
            this.progressbar.Value = this.Media.Position.TotalSeconds;
        }
        private void progressbar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
           
            if (e.NewValue == 8)
            {
                VideosOfTraining ShowVideo = new VideosOfTraining();
                ShowVideo.Show();
                this.WindowState = WindowState.Minimized;
                this.timer.Stop();
                this.thread.Abort();
                this.Close();
            }
        }
    }
}
