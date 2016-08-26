using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ControllerKeyboard.Controls {
	public partial class InputBlock {
		public InputBlock() {
			InitializeComponent();
			Background = null;
			_elms = Grid.Children.OfType<TextBlock>().ToArray();
		}

		private TextBlock[] _elms;

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
