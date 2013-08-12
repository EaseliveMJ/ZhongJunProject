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
using KinectSystem;
using System.Threading;
using System.Windows.Threading;

namespace SendDataOfKinect
{
    /// <summary>
    /// TestWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TestWindow : Window
    {
        public double MaxAngle, MinAngle;
        private Thread thread;
        private DispatcherTimer thread_timer;
        private KinectProcess Kinect;
        public bool IsUse;
        public TestWindow()
        {
            InitializeComponent();
            MaxAngle = 0;
            MinAngle = 0;
            IsUse = false;
            thread = new Thread(thread_CheckMinAngleAndMaxAngle);
            thread_timer = new DispatcherTimer();
          
           
        }
        public void  InitKinect(KinectProcess k)
        {
            Kinect = k;
            Binding binding = new Binding();
            binding.Source = k;
            binding.Path = new PropertyPath(KinectProcess.ColorImageProperty);
            this.player_image.SetBinding(Image.SourceProperty, binding);
        
        }
        private void StartCalibration_Click(object sender, RoutedEventArgs e)
        {
            
            this.thread.Start();
            this.StartCalibration.IsEnabled = false;
        }
        private void thread_CheckMinAngleAndMaxAngle()
        {
            thread_timer.Interval = TimeSpan.FromMilliseconds(100);
            thread_timer.Tick += new EventHandler(CheckMinAngleAndMaxAngle);
            thread_timer.Start();
        }

        private  void CheckMinAngleAndMaxAngle(object sender, EventArgs e)
        {
            if (this.Kinect.HandBodyState == ActiveStates.Red)
            {
                this.red.Visibility = Visibility.Visible;
                this.Yellow.Visibility = Visibility.Hidden;
                this.LightGreen.Visibility = Visibility.Hidden;
            }
            if (this.Kinect.HandBodyState == ActiveStates.Yellow)
            {
                this.red.Visibility = Visibility.Visible;
                this.Yellow.Visibility = Visibility.Visible;
                this.LightGreen.Visibility = Visibility.Hidden;
               CheckAngleValue();
            }
            if (this.Kinect.HandBodyState == ActiveStates.Green)
            {
                this.red.Visibility = Visibility.Visible;
                this.Yellow.Visibility = Visibility.Visible;
                this.LightGreen.Visibility = Visibility.Visible;
                CheckAngleValue();
            }

        }
        private void CheckAngleValue()
        {

            if (Convert.ToDouble(this.Kinect.AngleOfHandElbow_RightSide_Horizon) > MaxAngle)
            {
                this.MaxAngle = Convert.ToDouble(this.Kinect.AngleOfHandElbow_RightSide_Horizon);
            }
            if (Convert.ToDouble(this.Kinect.AngleOfHandElbow_RightSide_Horizon) < MinAngle)
            {
                this.MinAngle = Convert.ToDouble(this.Kinect.AngleOfHandElbow_RightSide_Horizon);
            }

            this.minangle_tbx.Text = MinAngle.ToString();
            this.maxangle_tbx.Text = MaxAngle.ToString();
        
        }
        private void EndCalibration_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void use_bt_Click(object sender, RoutedEventArgs e)
        {
            IsUse = true;
        }

        private void again_bt_Click(object sender, RoutedEventArgs e)
        {
            this.MaxAngle = 0;
            this.MinAngle = 0;
            this.minangle_tbx.Text = MinAngle.ToString();
            this.maxangle_tbx.Text = MaxAngle.ToString();            
        }        
    }
}
