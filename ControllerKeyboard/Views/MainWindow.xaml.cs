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

namespace ControllerKeyboard.Views {
	public partial class MainWindow  {
		public MainWindow() {
			InitializeComponent();
			InputControl.OnKey += InputControlOnOnKey;
		}

		private void InputControlOnOnKey(char c){
			TextBox.Text += c;
		}

		protected override void OnKeyDown(KeyEventArgs e) {
			InputControl.OnKeyDown(e);
			if (e.Key == Key.Back && TextBox.Text.Length > 0)
				TextBox.Text = TextBox.Text.Remove(TextBox.Text.Length - 1, 1);
			base.OnKeyDown(e);
		}

		protected override void OnKeyUp(KeyEventArgs e) {
			InputControl.OnKeyUp(e);
			base.OnKeyUp(e);
		}
	}
}
