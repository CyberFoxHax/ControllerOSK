using System.Windows.Input;

namespace ControllerKeyboard.Views {
	public partial class MainWindow  {
		public MainWindow() {
			InitializeComponent();
			InputControl.OnKey += InputControlOnOnKey;
			InputControl.InputSystem.KeyChange += InputSystemOnKeyChange;
			TextBox.Text = "";
		}

		private void InputSystemOnKeyChange(Input.IInput input) {
			if (input.Delete && TextBox.Text.Length > 0)
				TextBox.Text = TextBox.Text.Remove(TextBox.Text.Length - 1, 1);

			if (input.Space)
				TextBox.Text += " ";
		}

		private void InputControlOnOnKey(char c){
			TextBox.Text += c;
		}
	}
}
