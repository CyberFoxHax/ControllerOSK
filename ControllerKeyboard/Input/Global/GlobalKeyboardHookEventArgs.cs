namespace ControllerKeyboard.Input.Global{
	public class GlobalKeyboardHookEventArgs : System.ComponentModel.HandledEventArgs{
		public GlobalKeyboardHookEventArgs(LowLevelKeyboardInputEvent keyboardData, KeyboardState keyboardState){
			KeyboardData = keyboardData;
			KeyboardState = keyboardState;
		}

		public KeyboardState KeyboardState { get; private set; }
		public LowLevelKeyboardInputEvent KeyboardData { get; private set; }
	}
}