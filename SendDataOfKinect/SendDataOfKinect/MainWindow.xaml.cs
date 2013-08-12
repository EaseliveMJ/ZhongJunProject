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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media.Animation;
using KinectSystem;
using System.Diagnostics;
using SendDataOfKinect.GameScenes.WindMill;
using SendDataOfKinect.GameScenes.CutRope;


namespace SendDataOfKinect
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public GameState CurrentGameState;
        public double CompleteOneTraining_Time, ChangeAngleOfKinect_Time;
        public int WholeTrainingTimes, OldAngleOfKinect;
        public double level;
        public bool CurrentTrainingIsLessMin, IsPause;
        public int CheckTimes;
        public DateTime oldtime, newtime, old_temp, new_temp;
     

        public MainWindow()
        {
            InitializeComponent();
            IsPause = true;
            WholeTrainingTimes = 0;
            CompleteOneTraining_Time = 0;
            ChangeAngleOfKinect_Time = 0;
            CurrentTrainingIsLessMin = false;
            CurrentGameState = GameState.PriviewStart;
            OldAngleOfKinect = this.userInfo.AngleOfKinect;
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.kinect.InitKinect();
            this.userInfo.InitImage();
            this.againtraing_bt.IsEnabled = false;
            this.endtraining_bt.IsEnabled = false;
            this.pausetrainin_bt.IsEnabled = false;
            this.TrainingGoal.Visibility = Visibility.Hidden;

        }

        /// <summary>
        /// 开始训练按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void training_bt_Click(object sender, RoutedEventArgs e)
        {

            this.test_bt.IsEnabled = false;
            this.setting_bt.IsEnabled = false;
            this.training_bt.IsEnabled = false;
            this.againtraing_bt.IsEnabled = true;
            this.endtraining_bt.IsEnabled = true;
            this.pausetrainin_bt.IsEnabled = true;
            this.CurrentGameState = GameState.StartGame;
            this.TrainingGoal.Visibility = Visibility.Visible;
            this.oldtime = DateTime.Now;


        }

        /// <summary>
        /// 返回上一步按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void back_bt_Click(object sender, RoutedEventArgs e)
        {
            SettingAndStartWindow setting = new SettingAndStartWindow();
            setting.Content = new Setting();
            setting.Show();
            this.Close();
        }

        /// <summary>
        /// 设置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void setting_bt_Click(object sender, RoutedEventArgs e)
        {
            SettingInMain InMain = new SettingInMain();
            InMain.ShowDialog();
            Binding binding = new Binding();
            Binding binding_traintimes = new Binding();
            binding.Source = InMain;
            binding_traintimes.Source = InMain;
            binding.Path = new PropertyPath(SettingInMain.TrainingNumProperty);
            binding_traintimes.Path = new PropertyPath(SettingInMain.TrainTimesProperty);
            tb_level.SetBinding(TextBlock.TextProperty, binding);
            this.label2.SetBinding(Label.ContentProperty, binding_traintimes);


        }

        /// <summary>
        /// 校准按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void test_bt_Click(object sender, RoutedEventArgs e)
        {
           
          

        }

        /// <summary>
        /// 重新训练按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void againtraing_bt_Click(object sender, RoutedEventArgs e)
        {

            if (IsPause == false)
            {
                this.pausetrainin_bt.Content = "暂停训练";
                IsPause = true;
            }
            this.TrainingGoal.Visibility = Visibility.Visible;
            this.oldtime = DateTime.Now;
            this.CurrentGameState = GameState.AgainGame;
            WholeTrainingTimes = 0;
            CompleteOneTraining_Time = 0;
            ChangeAngleOfKinect_Time = 0;

        }

        /// <summary>
        /// 结束训练按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void endtraining_bt_Click(object sender, RoutedEventArgs e)
        {
            if (IsPause == false)
            {
                this.pausetrainin_bt.Content = "暂停训练";
                IsPause = true;
            }
            this.CompleteOneTraining_Time = 0;
            this.ChangeAngleOfKinect_Time = 0;
            this.WholeTrainingTimes = 0;
            this.test_bt.IsEnabled = true;
            this.setting_bt.IsEnabled = true;
            this.training_bt.IsEnabled = true;
            this.againtraing_bt.IsEnabled = false;
            this.pausetrainin_bt.IsEnabled = false;
            this.endtraining_bt.IsEnabled = false;
            this.TrainingGoal.Visibility = Visibility.Hidden;
            this.CurrentGameState = GameState.EndGame;

        }

        /// <summary>
        /// 暂停按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pausetrainin_bt_Click(object sender, RoutedEventArgs e)
        {

            if (IsPause)
            {
                IsPause = false;
                old_temp = DateTime.Now;
                this.TrainingGoal.Visibility = Visibility.Hidden;
                this.pausetrainin_bt.Content = "继续训练";
                this.CurrentGameState = GameState.PauseGame;
                return;
            }
            else
            {
                IsPause = true;
                new_temp = DateTime.Now;
                this.TrainingGoal.Visibility = Visibility.Visible;
                this.pausetrainin_bt.Content = "暂停训练";
                oldtime = oldtime + (new_temp - old_temp);
                this.CurrentGameState = GameState.ResumeGame;
                return;
            }

        }

        /// <summary>
        /// 退出程序按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            //
            this.Close();
            Application.Current.Shutdown();
            this.kinect.StopKinect();
        }

    

        private void kinect_LoopDoneSomething(object sensor, TimerRoutedEventArgs e)
        {
            //监测Kinect角度
            MainFrame.ChangeAngleOfKinect_Time += 0.1f;
            if (MainFrame.ChangeAngleOfKinect_Time > 1.2 && MainFrame.OldAngleOfKinect != MainFrame.userInfo.AngleOfKinect)
            {

                this.kinect.kinectsensor.ElevationAngle = MainFrame.userInfo.AngleOfKinect;
                MainFrame.OldAngleOfKinect = MainFrame.userInfo.AngleOfKinect;
                MainFrame.ChangeAngleOfKinect_Time = 0;
            }

            switch (MainFrame.CurrentGameState)
            {
                case GameState.PriviewStart:
                    {

                    }
                    break;
                case GameState.StartGame:
                    {
                        MainFrame.CompleteOneTraining_Time += 0.1f;
                        MainFrame.CheckTimes++;
                        if (MainFrame.CheckTimes == 9)
                        {
                            MainFrame.newtime = DateTime.Now;
                            MainFrame.tbx_wholetime.Text = String.Format("{0:00}:{1:00}:{2:00}", (MainFrame.newtime - MainFrame.oldtime).Hours, (MainFrame.newtime - MainFrame.oldtime).Minutes, (MainFrame.newtime - MainFrame.oldtime).Seconds);
                            MainFrame.CheckTimes = 0;
                        }
                    }
                    break;
                case GameState.PauseGame:
                    break;
                case GameState.ResumeGame:
                    {

                        MainFrame.CompleteOneTraining_Time += 0.1f;
                        MainFrame.CheckTimes++;
                        if (MainFrame.CheckTimes == 9)
                        {
                            MainFrame.newtime = DateTime.Now;
                            MainFrame.tbx_wholetime.Text = String.Format("{0:00}:{1:00}:{2:00}", (MainFrame.newtime - MainFrame.oldtime).Hours, (MainFrame.newtime - MainFrame.oldtime).Minutes, (MainFrame.newtime - MainFrame.oldtime).Seconds);
                            MainFrame.CheckTimes = 0;
                        }
                      
           
                    }
                    break;
                case GameState.AgainGame:
                    {

                        MainFrame.CompleteOneTraining_Time += 0.1f;
                        MainFrame.CheckTimes++;
                        if (MainFrame.CheckTimes == 9)
                        {
                            MainFrame.newtime = DateTime.Now;
                            MainFrame.tbx_wholetime.Text = String.Format("{0:00}:{1:00}:{2:00}", (MainFrame.newtime - MainFrame.oldtime).Hours, (MainFrame.newtime - MainFrame.oldtime).Minutes, (MainFrame.newtime - MainFrame.oldtime).Seconds);
                            MainFrame.CheckTimes = 0;
                        }

                    }
                    break;
                case GameState.EndGame:
                    break;
                case GameState.CalibrationMode:
                    break;
                case GameState.SettingMode:
                    break;
                default:
                    break;
            }
        }

        private void DoubleAnimationUsingKeyFrames_Completed(object sender, EventArgs e)
        {
            this.Process.Visibility = Visibility.Collapsed;
            //WindMill game = new WindMill();
            CutRope game = new CutRope();
            game.InitGame(kinect, MainFrame);
            this.GameArea.Content = game;

            this.GameArea.Width = 1920;
            this.GameArea.Height = 1080;
        }
    }
}
