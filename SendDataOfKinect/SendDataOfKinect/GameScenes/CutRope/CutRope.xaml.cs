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
using System.Diagnostics;

namespace SendDataOfKinect.GameScenes.CutRope
{
    public partial class CutRope : UserControl
    {
        private KinectProcess KinectSensor;
        private MainWindow MainFrame;
        private double MaxAngle;
        private double MinAngle;
        private AnimationStatus m_PreviousAnimationStatus;
        enum AnimationStatus:int
        {
            Status0 = 0,
            Stage1,
            Stage2,
            Stage3,

            InvalidStage,
        }
        public CutRope()
        {
            InitializeComponent();
        }

        public void InitGame(KinectProcess Kinect, MainWindow Main)
        {
            Binding binding_TrainingGoal = new Binding();
            binding_TrainingGoal.Source = Main.kinect;
            binding_TrainingGoal.Path = new PropertyPath(KinectProcess.AngleOfHandElbow_RightSide_HorizonProperty);
            Main.TrainingGoal.SetBinding(TextBlock.TextProperty, binding_TrainingGoal);

            this.KinectSensor = Kinect;
            this.MainFrame = Main;
            this.KinectSensor.LoopDoneSomething += new TimeRoutedEventHandler(KinectSensor_LoopDoneSomething);
            MainFrame.TrainingName.Text = "手臂平举前后";

            MaxAngle = 90;
            MinAngle = 0;

            m_PreviousAnimationStatus = AnimationStatus.Status0;
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
                            CheckToResetAnimation();
                        }
                        if (this.KinectSensor.HandBodyState == ActiveStates.Yellow && MainFrame.tb_level.Text != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Visible;
                            MainFrame.LightGreen.Visibility = Visibility.Hidden;
                            AnimationStatus status = CheckAnimationStatus();
                            PlayAnimation(status);
                            GamePlay();
                        }
                        if (this.KinectSensor.HandBodyState == ActiveStates.Green && MainFrame.tb_level.Text != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Visible;
                            MainFrame.LightGreen.Visibility = Visibility.Visible;
                            AnimationStatus status = CheckAnimationStatus();
                            PlayAnimation(status);
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
                            CheckToResetAnimation();
                        }
                        if (this.KinectSensor.HandBodyState == ActiveStates.Yellow && MainFrame.tb_level.Text != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Visible;
                            MainFrame.LightGreen.Visibility = Visibility.Hidden;
                            AnimationStatus status = CheckAnimationStatus();
                            PlayAnimation(status);
                            GamePlay();
                        }
                        if (this.KinectSensor.HandBodyState == ActiveStates.Green && MainFrame.tb_level.Text != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Visible;
                            MainFrame.LightGreen.Visibility = Visibility.Visible;
                            AnimationStatus status = CheckAnimationStatus();
                            PlayAnimation(status);
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
                            CheckToResetAnimation();
                        }
                        if (this.KinectSensor.HandBodyState == ActiveStates.Yellow && MainFrame.tb_level.Text != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Visible;
                            MainFrame.LightGreen.Visibility = Visibility.Hidden;
                            AnimationStatus status = CheckAnimationStatus();
                            PlayAnimation(status);
                            GamePlay();
                        }
                        if (this.KinectSensor.HandBodyState == ActiveStates.Green && MainFrame.tb_level.Text != null)
                        {
                            MainFrame.red.Visibility = Visibility.Visible;
                            MainFrame.Yellow.Visibility = Visibility.Visible;
                            MainFrame.LightGreen.Visibility = Visibility.Visible;
                            AnimationStatus status = CheckAnimationStatus();
                            PlayAnimation(status);
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
            double AngleOfHandElbow_RightSide_Horizon = double.Parse(this.KinectSensor.AngleOfHandElbow_RightSide_Horizon);
            if (AngleOfHandElbow_RightSide_Horizon < 0)
            {
                MainFrame.CurrentTrainingIsLessMin = true;
            }
            if (MainFrame.CompleteOneTraining_Time > 4 && double.Parse(this.KinectSensor.AngleOfHandElbow_RightSide_Horizon) > MainFrame.level && MainFrame.CurrentTrainingIsLessMin == true)
            {
                MainFrame.WholeTrainingTimes += 1;
                MainFrame.CompleteOneTraining_Time = 0;
                MainFrame.CurrentTrainingIsLessMin = false;
            }
        }

        private void CheckToResetAnimation()
        {
            if (m_PreviousAnimationStatus != AnimationStatus.Status0)
            {
                double AngleOfHandElbow_RightSide_Horizon = double.Parse(this.KinectSensor.AngleOfHandElbow_RightSide_Horizon);
                if (AngleOfHandElbow_RightSide_Horizon < 0)
                {
                    Storyboard st = this.FindResource("CutRope") as Storyboard;
                    lock (st)
                    {
                        st.BeginTime = TimeSpan.FromSeconds(0);
                        st.Duration = TimeSpan.FromSeconds(0);
                        st.Begin(this, true);
                    }
                    m_PreviousAnimationStatus = AnimationStatus.Status0;
                }
            }
        }

        private AnimationStatus CheckAnimationStatus()
        {
            double maxAngle = double.Parse(MainFrame.tb_level.Text);
            double Angle_Status1 = maxAngle / 3;
            double Angle_Status2 = maxAngle / 3 * 2;
            double Angle_Status3 = maxAngle;

            AnimationStatus stage;
            stage = AnimationStatus.InvalidStage;
            double AngleOfHandElbow_RightSide_Horizon = double.Parse(this.KinectSensor.AngleOfHandElbow_RightSide_Horizon);

            switch (m_PreviousAnimationStatus)
            {
                case AnimationStatus.Status0:
                    if (AngleOfHandElbow_RightSide_Horizon >= Angle_Status1 && AngleOfHandElbow_RightSide_Horizon < Angle_Status2)
                    {
                        stage = m_PreviousAnimationStatus + 1;
                    }
                    break;
                case AnimationStatus.Stage1:
                    if (AngleOfHandElbow_RightSide_Horizon >= Angle_Status2 && AngleOfHandElbow_RightSide_Horizon < Angle_Status3)
                    {
                        stage = m_PreviousAnimationStatus + 1;
                    }
                    break;
                case AnimationStatus.Stage2:
                    if (MainFrame.CompleteOneTraining_Time > 4 && double.Parse(this.KinectSensor.AngleOfHandElbow_RightSide_Horizon) > MainFrame.level && MainFrame.CurrentTrainingIsLessMin == true)
                    {
                        stage = m_PreviousAnimationStatus + 1;
                    }
                    break;
                case AnimationStatus.Stage3:
                    if (AngleOfHandElbow_RightSide_Horizon < 0)
                    {
                        stage = AnimationStatus.Status0;
                    }
                    break;
            }

            return stage;
        }

        private void PlayAnimation(AnimationStatus stage)
        {
//             textBlock1.Text = m_PreviousAnimationStage.ToString();
//             textBlock2.Text = stage.ToString();

            Storyboard st = this.FindResource("CutRope") as Storyboard;
            lock (st)
            {
                if (stage != m_PreviousAnimationStatus)
                {
                    switch (stage)
                    {
                        case AnimationStatus.Status0:
                            //初始状态,
                            if (m_PreviousAnimationStatus == AnimationStatus.Stage3)
                            {
                                st.BeginTime = TimeSpan.FromSeconds(0);
                                st.Duration = TimeSpan.FromSeconds(0);
                                st.Begin(this, true);
                                m_PreviousAnimationStatus = stage;
                            }
                            break;
                        case AnimationStatus.Stage1:
                            //播放第一段动画,
                            if (m_PreviousAnimationStatus == AnimationStatus.Status0)
                            {
                                st.BeginTime = TimeSpan.FromSeconds(0);
                                st.Duration = TimeSpan.FromSeconds(1);
                                st.Begin(this, true);
                                m_PreviousAnimationStatus = stage;
                            }                        
                             break;
                        case AnimationStatus.Stage2:
                            //播放第二段动画
                            if (m_PreviousAnimationStatus == AnimationStatus.Stage1)
                            {
                                st.BeginTime = TimeSpan.FromSeconds(-1);
                                st.Duration = TimeSpan.FromSeconds(2);
                                st.Begin(this, true);
                                m_PreviousAnimationStatus = stage;
                            }                         
                            break;
                        case AnimationStatus.Stage3:
                            //播放第三段动画
                            if (m_PreviousAnimationStatus == AnimationStatus.Stage2)
                            {
                                st.BeginTime = TimeSpan.FromSeconds(-2);
                                st.Duration = TimeSpan.FromSeconds(3);
                                st.Begin(this, true);
                                m_PreviousAnimationStatus = stage;
                            }    
                            break;
                    }                    
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

                MaxAngle = double.Parse(testwindow.MaxAngle.ToString());
                MinAngle = double.Parse(testwindow.MinAngle.ToString());
            }
        }
    }
}
