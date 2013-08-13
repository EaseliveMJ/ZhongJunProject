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

namespace SendDataOfKinect
{
    /// <summary>
    /// VideosOfTraining.xaml 的交互逻辑
    /// </summary>
    public partial class VideosOfTraining : Window
    {
        public VideosOfTraining()
        {
            InitializeComponent();
        }

        private void show_media_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.show_media.Stop();
            this.show_media.Play();
        }

        private void show_media_Loaded(object sender, RoutedEventArgs e)
        {
            this.show_media.Play();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            this.show_media.Stop();
            MainWindow mainwindow = new MainWindow();
            mainwindow.Show();
            this.Close();
        }
    }
}
