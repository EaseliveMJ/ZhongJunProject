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
    /// SettingInMain.xaml 的交互逻辑
    /// </summary>
    public partial class SettingInMain : Window
    {
        public SettingInMain()
        {
            InitializeComponent();
        }


        public string  TrainingNum
        {
            get { return (string )GetValue(TrainingNumProperty); }
            set { SetValue(TrainingNumProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrainingNum.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrainingNumProperty =
            DependencyProperty.Register("TrainingNum", typeof(string ), typeof(SettingInMain), new UIPropertyMetadata(null));


        public string TrainTimes
        {
            get { return (string)GetValue(TrainTimesProperty); }
            set { SetValue(TrainTimesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TrainTimes.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TrainTimesProperty =
            DependencyProperty.Register("TrainTimes", typeof(string), typeof(SettingInMain), new UIPropertyMetadata(null));



            
        private void Ensure_Click(object sender, RoutedEventArgs e)
        {
            TrainingNum=this.level_tb.Text;
            TrainTimes = this.times_tb.Text;
            this.Close();

        }

      
    }
}
