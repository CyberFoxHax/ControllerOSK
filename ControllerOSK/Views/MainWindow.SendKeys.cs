using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ControllerOSK.Views {
	public partial class MainWindow {

		private bool _isEnabled = true;

		protected override void OnClosed(System.EventArgs e) {
			InputControl.InputSystem.Dispose();
			System.Windows.Application.Current.Shutdown();
			base.OnClosed(e);
		}

		private static void SendKey(string key) {
			if (key.Length == 1)
				foreach (var c in "+^%~(){}".Where(c => key.Contains(c))) {
					key = key.Replace(c.ToString(), "{" + c + "}");
					break;
				}
			System.Windows.Forms.SendKeys.SendWait(key);
		}

		private void InputSystemOnKeyChange(Input.IInput input) {
			if (input.OpenClose && _isEnabled == false) {
				input.Enable();
				Show();
				_isEnabled = true;
			}
			else if (_isEnabled == false)
				return;

			else if (input.OpenClose && _isEnabled) {
				input.Disable();
				Hide();
				ResetText();
				InvalidateVisual();
				_isEnabled = false;
				return;
			}

			if (input.Delete) {
				if (_caretIndex > 0 && TextBox.Text.Length > 0) {
					CaretIndex--;
					RemoveText();
				}
				SendKey("{BS}");
			}

			if (input.Space)
				InsertText(" ");

			if (input.Enter) {
				InsertText(" ", false);
				SendKey("{ENTER}");
			}

			if (input.MoveLeft) {
				CaretIndex--;
				SendKey("{LEFT}");
			}
			else if (input.MoveRight) {
				CaretIndex++;
				SendKey("{RIGHT}");
			}


			if (input.ChangeSymbols) InputControl.SwitchSymbols();
			else if (input.ChangeCase) InputControl.SwitchUppercase();
			else InputControl.SwitchLowercase();
		}

		public void ResetText() {
			TextBox.Text = "";
			CaretIndex = 0;
		}

		public void InsertText(string input, bool send = true) {
			TextBox.Text = TextBox.Text.Insert(_caretIndex, input);
			CaretIndex++;
			if (send)
				SendKey(input);
		}

		public void RemoveText() {
			if (_caretIndex > -1)
				TextBox.Text = TextBox.Text.Remove(_caretIndex, 1);
		}

		private int _caretIndex;
		public int CaretIndex {
			get { return _caretIndex; }
			set {
				if (value > -1 && value < TextBox.Text.Length + 1) {
					_caretIndex = value;
					TextBox.CaretIndex = value;
					var rect = TextBox.GetRectFromCharacterIndex(value);
					System.Windows.Controls.Canvas.SetLeft(Caret, rect.X);
				}
			}
		}

		private void InputControlOnOnKey(char c) {
			InsertText(c.ToString());
		}
	}
}
