using System.Linq;
using XInput.Wrapper;

namespace ControllerKeyboard.Input {
	public class JoystickInput : IInput {
		public JoystickInput(){
			//if (X.IsAvailable == false)
			//	throw new System.Exception("XInput is unavailable");

			_gamepad = X.Gamepad_1;
			_gamepad.StateChanged += GamepadOnStateChanged;
			X.StartPolling(_gamepad);
		}

		~JoystickInput(){
			X.StopPolling();
		}

		private void GamepadOnStateChanged(object sender, System.EventArgs eventArgs){
			var hasChanged = false;
			ResetButtons();

			_charPos = new Vector2();
			if (new[]{
				_gamepad.A_down,
				_gamepad.B_down,
				_gamepad.X_down,
				_gamepad.Y_down
			}.Any(p => p)) {
				hasChanged = true;
				if (_gamepad.Y_down) _charPos.Y = 1;
				if (_gamepad.B_down) _charPos.X = 1;
				if (_gamepad.A_down) _charPos.Y = -1;
				if (_gamepad.X_down) _charPos.X = -1;
			}

			if (_gamepad.LBumper_down	) { Delete		= true; hasChanged = true; }
			if (_gamepad.RBumper_down	) { Space		= true; hasChanged = true; }
			if (_gamepad.Dpad_Left_down ) { MoveLeft	= true; hasChanged = true; } // todo analogue move
			if (_gamepad.Dpad_Right_down) { MoveRight	= true; hasChanged = true; } // todo analogue move
			if (_gamepad.Back_down		) { OpenClose	= true; hasChanged = true; }
			if (_gamepad.Dpad_Down_down	) { Return		= true; hasChanged = true; }

			const float triggerThreshhold = 0.1f;
			if (_gamepad.RTrigger_N > triggerThreshhold) { ChangeCase	 = true; hasChanged = true; }
			if (_gamepad.RTrigger_N > triggerThreshhold) { ChangeSymbols = true; hasChanged = true; }

			if (HandleBlocks())
				hasChanged = true;

			if (hasChanged && KeyChange != null)
				KeyChange(this);
		}

		private bool HandleBlocks(){
			var lstick = _gamepad.LStick_N;
			const double tolerance = 0.01;
			if (System.Math.Abs(lstick.X) < tolerance && System.Math.Abs(lstick.Y) < tolerance)
				return false;

			_blockPos = new Vector2(lstick.X, lstick.Y);
			return true;
		}

		public void ResetButtons() {
			//ChangeCase = false;
			//ChangeSymbols = false;
			MoveLeft = false;
			MoveRight = false;
			Delete = false;
			Space = false;
			OpenClose = false;
		}

		private Vector2 _blockPos;
		private Vector2 _charPos;
		private readonly X.Gamepad _gamepad;
		public event System.Action<IInput> KeyChange;

		public Vector2 BlockPos {
			get { return _blockPos; }
			set { _blockPos = value; }
		}

		public Vector2 CharPos {
			get { return _charPos; }
			set { _charPos = value; }
		}

		public bool ChangeCase { get; set; }
		public bool ChangeSymbols { get; set; }
		public bool MoveLeft { get; set; }
		public bool MoveRight { get; set; }
		public bool Delete { get; set; }
		public bool Space { get; set; }
		public bool Return { get; set; }
		public bool OpenClose { get; set; }
	}
}
