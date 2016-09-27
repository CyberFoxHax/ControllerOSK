using System.Windows.Controls;
using System.Windows;

namespace ControllerOSK.Controls {
	public partial class InputBlock {
		public InputBlock() {
			InitializeComponent();
			_elms = new []{ Txt1, Txt2, Txt3, Txt4 };
			Background = null;
		}

		public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
			"IsActive",
			typeof (bool),
			typeof (InputBlock),
			new PropertyMetadata(default(bool))
		);

		public bool IsActive {
			get { return (bool) GetValue(IsActiveProperty); }
			set {
				SetValue(IsActiveProperty, value);
				if (IsActiveChanged != null) IsActiveChanged(this);
			}
		}

		public static readonly DependencyProperty DistanceFromCenterProperty = DependencyProperty.Register(
			"DistanceFromCenter", typeof (double), typeof (InputBlock), new PropertyMetadata(default(double)));

		public double DistanceFromCenter {
			get { return (double) GetValue(DistanceFromCenterProperty); }
			set { SetValue(DistanceFromCenterProperty, value); }
		}

		protected override void OnRender(System.Windows.Media.DrawingContext dc){
			var halfHeight = ActualHeight/2;
			var halfWidth  = ActualWidth /2;

			var shift = _elms[0].Width/2;

			Canvas.SetTop(_elms[0], halfHeight - DistanceFromCenter - shift);
			Canvas.SetTop(_elms[1], halfHeight - shift);
			Canvas.SetTop(_elms[2], halfHeight - shift);
			Canvas.SetTop(_elms[3], halfHeight + DistanceFromCenter - shift);

			Canvas.SetLeft(_elms[0], halfWidth - shift);
			Canvas.SetLeft(_elms[1], halfWidth - DistanceFromCenter - shift);
			Canvas.SetLeft(_elms[2], halfWidth + DistanceFromCenter - shift);
			Canvas.SetLeft(_elms[3], halfWidth - shift);
		}

		public System.Action<InputBlock> IsActiveChanged;

		private readonly Label[] _elms;

		public void SetChars(string str){
			var controls = _elms;
			for (var i = 0; i < str.Length; i++)
				controls[i].Content = str[i].ToString();
		}

		public char GetStr(int x, int y){
			if (y > 0) return ((string)_elms[3].Content)[0];
			if (y < 0) return ((string)_elms[0].Content)[0];
			if (x > 0) return ((string)_elms[1].Content)[0];
			if (x < 0) return ((string)_elms[2].Content)[0];
			return '\0';
		}
	}
}
