using System.Windows.Controls;
using System.Windows;

namespace ControllerOSK.Controls {
	public partial class InputBlock {
		public InputBlock() {
			InitializeComponent();
			_elms = new []{ Txt1, Txt2, Txt3, Txt4 };
		}

		public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register(
			"IsActive",
			typeof (bool),
			typeof (InputBlock),
			new PropertyMetadata(default(bool))
		);

		public bool IsActive {
			get { return (bool) GetValue(IsActiveProperty); }
			set { SetValue(IsActiveProperty, value); }
		}

		private readonly TextBlock[] _elms;

		public void SetChars(string str){
			var controls = _elms;
			for (var i = 0; i < str.Length; i++)
				controls[i].Text = str[i].ToString();
		}

		public char GetStr(int x, int y){
			if (y > 0) return _elms[3].Text[0];
			if (y < 0) return _elms[0].Text[0];
			if (x > 0) return _elms[1].Text[0];
			if (x < 0) return _elms[2].Text[0];
			return '\0';
		}
	}
}
