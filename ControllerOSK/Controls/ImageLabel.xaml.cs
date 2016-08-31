using System.Windows;
using System.Windows.Media;

namespace ControllerOSK.Controls {
	public partial class ImageLabel {
		public ImageLabel() {
			InitializeComponent();

			Loaded += OnLoaded;
		}

		private void OnLoaded(object sender, RoutedEventArgs routedEventArgs){
			//ImageSource = ImageSource;
		}

		public static readonly DependencyProperty ImageSourceProperty = DependencyProperty.Register(
			"ImageSource", typeof (ImageSource), typeof (ImageLabel), new PropertyMetadata(default(ImageSource)));

		public ImageSource ImageSource{
			get { return (ImageSource) GetValue(ImageSourceProperty); }
			set { SetValue(ImageSourceProperty, value); }
		}


		public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
			"Text", typeof (string), typeof (ImageLabel), new PropertyMetadata(default(string)));

		public string Text{
			get { return (string) GetValue(TextProperty); }
			set { SetValue(TextProperty, value); }
		}
	}
}
