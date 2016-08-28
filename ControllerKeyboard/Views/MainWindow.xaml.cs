using System.Windows.Input;

namespace ControllerKeyboard.Views {
	public partial class MainWindow  {
		public MainWindow(){
			InitializeComponent();
			Background = System.Windows.Media.Brushes.Transparent;
			InputControl.OnKey += InputControlOnOnKey;
			InputControl.InputSystem.KeyChange += InputSystemOnKeyChange;
			TextBox.Text = "";
			System.Windows.Controls.Canvas.SetLeft(Caret, 0);
		}

		protected override void OnClosed(System.EventArgs e){
			System.Windows.Application.Current.Shutdown();
			base.OnClosed(e);
		}

		private void SendKey(string key){
			System.Windows.Forms.SendKeys.SendWait(key);
		}

		private void InputSystemOnKeyChange(Input.IInput input) {
			if (input.Delete){
				if (_caretIndex > 0 && TextBox.Text.Length > 0){
					CaretIndex--;
					RemoveText();
				}
				SendKey("{BS}");
			}

			if (input.Space)
				InsertText(" ");

			if (input.MoveLeft){
				CaretIndex--;
				SendKey("{LEFT}");
			}
			else if (input.MoveRight){
				CaretIndex++;
				SendKey("{RIGHT}");
			}

			if		(input.ChangeSymbols) InputControl.SwitchSymbols();
			else if (input.ChangeCase	) InputControl.SwitchUppercase();
			else InputControl.SwitchLowercase();
		}

		public void InsertText(string input){
			TextBox.Text = TextBox.Text.Insert(_caretIndex, input);
			CaretIndex++;
			SendKey(input);
		}

		public void RemoveText(){
			if(_caretIndex > -1)
				TextBox.Text = TextBox.Text.Remove(_caretIndex, 1);
		}

		private int _caretIndex;
		public int CaretIndex {
			get { return _caretIndex; }
			set {
				if(value > -1 && value < TextBox.Text.Length+1){
					_caretIndex = value;
					TextBox.CaretIndex = value;
					var rect = TextBox.GetRectFromCharacterIndex(value);
					System.Windows.Controls.Canvas.SetLeft(Caret, rect.X);
				};
			}
		}

		private void InputControlOnOnKey(char c){
			InsertText(c.ToString());
		}
	}
}
