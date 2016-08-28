using System;

namespace ControllerKeyboard.Input.Global {
	public class Controller : IDisposable {
		private GlobalKeyboardHook _globalKeyboardHook;

		public void SetupKeyboardHooks() {
			_globalKeyboardHook = new GlobalKeyboardHook();
			_globalKeyboardHook.KeyboardPressed += OnKeyPressed;
		}

		private static void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e) {
			if (e.KeyboardData.VirtualCode != Key.Back)
				return;

			if (e.KeyboardState == KeyboardState.KeyDown) {
				System.Windows.MessageBox.Show("Print Screen");
				//e.Handled = true;
			}
		}

		public void Dispose(){
			if (_globalKeyboardHook != null) _globalKeyboardHook.Dispose();
		}
	}
}