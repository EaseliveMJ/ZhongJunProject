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
	/// Interaction logic for UserControl1.xaml
	/// </summary>
	public partial class UserInfo : UserControl
	{

        private bool IsDepthImage;
        public ImageSource ColorImageSource
        {
            get { return (ImageSource)GetValue(ColorImageSourceProperty); }
            set { SetValue(ColorImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ColorImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColorImageSourceProperty =
            DependencyProperty.Register("ColorImageSource", typeof(ImageSource), typeof(UserInfo), new UIPropertyMetadata(null));


        public ImageSource DepthImageSource
        {
            get { return (ImageSource)GetValue(DepthImageSourceProperty); }
            set { SetValue(DepthImageSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DepthImageSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DepthImageSourceProperty =
            DependencyProperty.Register("DepthImageSource", typeof(ImageSource), typeof(UserInfo), new UIPropertyMetadata(null));


        public int AngleOfKinect
        {
            get { return (int)GetValue(AngleOfKinectProperty); }
            set { SetValue(AngleOfKinectProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AngleOfKinect.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AngleOfKinectProperty =
            DependencyProperty.Register("AngleOfKinect", typeof(int), typeof(UserInfo), new UIPropertyMetadata(null));

        


        public UserInfo()
		{
           
			this.InitializeComponent();
            this.IsDepthImage = true;
		}

        private void color_Click(object sender, RoutedEventArgs e)
        {
            if (this.DepthImageSource != null&&this.IsDepthImage)
            {
                this.MainImage.Source = this.DepthImageSource;
                this.ChangeImage_bt.Content = "彩  色";
                this.IsDepthImage = false;
                return;
            }
            if (this.ColorImageSource!=null&&(this.IsDepthImage==false))
            {
                this.MainImage.Source = this.ColorImageSource;
                this.ChangeImage_bt.Content = "深  度";
                this.IsDepthImage = true;
                return;
            }
            {

            }
        }

        //private void depth_Click(object sender, RoutedEventArgs e)
        //{
        //    if (DepthImageSource != null)
        //    {
        //        this.MainImage.Source = this.DepthImageSource;
        //    }
        //}

        private void kinectAngle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            //this.AngleOfKinect = Convert.ToInt16(slider.Value);
        }
        public void InitImage()
        {


            if (ColorImageSource != null)
            {
                this.MainImage.Source = this.ColorImageSource;
            }
        
        }
     
     
	}
}