using System.Linq;
using System.Windows.Controls;

namespace ControllerKeyboard.Controls {
	public partial class InputBlock {
		public InputBlock() {
			InitializeComponent();
			Background = null;
			_elms = Grid.Children.OfType<TextBlock>().ToArray();
		}

		private readonly TextBlock[] _elms;

		public void SetChars(string str){
			var controls = Grid.Children.OfType<TextBlock>().ToArray();
			for (var i = 0; i < str.Length; i++){
				controls[i].Text = str[i].ToString();
			}
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
