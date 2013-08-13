using System;
using System.Collections.Generic;
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

namespace SendDataOfKinect
{
	/// <summary>
	/// Interaction logic for PositionOfPlayer.xaml
	/// </summary>
	public partial class PositionOfPlayer : UserControl
	{
        
        public double LeftOfCanvas
        {
            get { return (double)GetValue(LeftOfCanvasProperty); }
            set { SetValue(LeftOfCanvasProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LeftOfCanvas.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LeftOfCanvasProperty =
            DependencyProperty.Register("LeftOfCanvas", typeof(double), typeof(PositionOfPlayer), new UIPropertyMetadata(null));






        public double TopOfCanvas
        {
            get { return (double)GetValue(TopOfCanvasProperty); }
            set { SetValue(TopOfCanvasProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TopOfCanvas.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TopOfCanvasProperty =
            DependencyProperty.Register("TopOfCanvas", typeof(double), typeof(PositionOfPlayer), new UIPropertyMetadata(null));


        public Binding Bind_Left, Bind_Top;

		public PositionOfPlayer()
		{
			this.InitializeComponent();
               
		}

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Bind_Left = new Binding();
            Bind_Top = new Binding();
            Bind_Left.Source = this;
            Bind_Top.Source = this;
            Bind_Left.Path = new PropertyPath(PositionOfPlayer.LeftOfCanvasProperty);
            Bind_Top.Path = new PropertyPath(PositionOfPlayer.TopOfCanvasProperty);
            Player.SetBinding(Canvas.LeftProperty, Bind_Left);
            Player.SetBinding(Canvas.TopProperty, Bind_Top);

        }
	}
}