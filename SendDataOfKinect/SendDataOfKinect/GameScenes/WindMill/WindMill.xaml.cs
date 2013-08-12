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
using KinectSystem;
using System.Windows.Media.Animation;

namespace SendDataOfKinect.GameScenes.WindMill
{
    /// <summary>
    /// WindMill.xaml 的交互逻辑
    /// </summary>
    public partial class WindMill : UserControl
    {

        private KinectProcess KinectSensor;
        private MainWindow MainFrame;
        private bool IsOpen;
        public WindMill()
        {
            InitializeComponent();
        }
        public void InitGame(KinectProcess Kinect, MainWindow Main)
        {
            Binding binding_TrainingGoal = new Binding();
            binding_TrainingGoal.Source = Main.kinect;
            binding_TrainingGoal.Path = new PropertyPath(KinectProcess.AngleOfHandElbow_RightSide_HorizonProperty);
            Main.TrainingGoal.SetBinding(TextBlock.TextProperty, binding_TrainingGoal);

            IsOpen = true;
            this.KinectSensor = Kinect;
            this.MainFrame = Main;
            this.KinectSensor.LoopDoneSomething += new TimeRoutedEventHandler(KinectSensor_LoopDoneSomething);
            MainFrame.TrainingName.Text = "手臂平举前后";
        }

        void KinectSensor_LoopDoneSomething(object sensor, TimerRoutedEventArgs e)
        {

            switch (MainFrame.CurrentGameState)
            {
                case GameState.PriviewStart:
                    {

                    }
                    break;
                case GameState.StartGame:
                    {

                        this.MainFrame.TrainingTimes.Text = MainFrame.WholeTrainingTimes.ToString();
                        if (this.KinectSensor.HandBodyState == ActiveStates.Red && MainFrame.tb_level != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Hidden;
                            MainFrame.LightGreen.Visibility = Visibility.Hidden;
                        }
                        if (this.KinectSensor.HandBodyState == ActiveStates.Yellow && MainFrame.tb_level.Text != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Visible;
                            MainFrame.LightGreen.Visibility = Visibility.Hidden;
                            GamePlay();
                        }
                        if (this.KinectSensor.HandBodyState == ActiveStates.Green && MainFrame.tb_level.Text != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Visible;
                            MainFrame.LightGreen.Visibility = Visibility.Visible;
                            GamePlay();
                        }
                    }
                    break;
                case GameState.PauseGame:
                    break;
                case GameState.ResumeGame:
                    {


                        MainFrame.TrainingTimes.Text = MainFrame.WholeTrainingTimes.ToString();
                        if (this.KinectSensor.HandBodyState == ActiveStates.Red && MainFrame.tb_level != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Hidden;
                            MainFrame.LightGreen.Visibility = Visibility.Hidden;
                        }
                        if (this.KinectSensor.HandBodyState == ActiveStates.Yellow && MainFrame.tb_level.Text != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Visible;
                            MainFrame.LightGreen.Visibility = Visibility.Hidden;
                            GamePlay();
                        }
                        if (this.KinectSensor.HandBodyState == ActiveStates.Green && MainFrame.tb_level.Text != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Visible;
                            MainFrame.LightGreen.Visibility = Visibility.Visible;
                            GamePlay();
                        }

                    }
                    break;
                case GameState.AgainGame:
                    {

                        if (this.KinectSensor.HandBodyState == ActiveStates.Red && MainFrame.tb_level != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Hidden;
                            MainFrame.LightGreen.Visibility = Visibility.Hidden;
                        }
                        if (this.KinectSensor.HandBodyState == ActiveStates.Yellow && MainFrame.tb_level.Text != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Visible;
                            MainFrame.LightGreen.Visibility = Visibility.Hidden;
                            GamePlay();
                        }
                        if (this.KinectSensor.HandBodyState == ActiveStates.Green && MainFrame.tb_level.Text != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Visible;
                            MainFrame.LightGreen.Visibility = Visibility.Visible;
                            GamePlay();
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
        private void GamePlay()
        {
            MainFrame.level = double.Parse(MainFrame.tb_level.Text);
            if (double.Parse(this.KinectSensor.AngleOfHandElbow_RightSide_Horizon) < 0)
            {
                MainFrame.CurrentTrainingIsLessMin = true;
            }
            if (MainFrame.CompleteOneTraining_Time > 4 && double.Parse(this.KinectSensor.AngleOfHandElbow_RightSide_Horizon) > MainFrame.level && MainFrame.CurrentTrainingIsLessMin == true)
            {

                MainFrame.WholeTrainingTimes += 1;
                MainFrame.CompleteOneTraining_Time = 0;
                MainFrame.CurrentTrainingIsLessMin = false;
                Storyboard st = this.FindResource("WindMill") as Storyboard;
                lock (st)
                {
                    BeginStoryboard(st);
                }



            }

        }

        private void Calibration_bt(object sender, RoutedEventArgs e)
        {

            TestWindow testwindow = new TestWindow();
            testwindow.InitKinect(KinectSensor);
            testwindow.ShowDialog();

            if (testwindow.IsUse == true)
            {
                MainFrame.tb_level.Text = testwindow.MaxAngle.ToString();
            }
        }

        private void GameSetting_Click(object sender, RoutedEventArgs e)
        {
            if (IsOpen)
            {
                IsOpen = false;
                this.Menu.Visibility = Visibility.Visible;
                return;

            }
            if (IsOpen == false)
            {
                this.Menu.Visibility = Visibility.Hidden;
                IsOpen = true;
                return;
            }
        }


    }
}

