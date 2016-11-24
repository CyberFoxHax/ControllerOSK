
using System.Linq;
using XInputDotNetPure;

namespace ControllerOSK.Input {
	public class JoystickInput : IInput {
		public JoystickInput(){
			Enable();

			_isRunning = true;
			new System.Threading.Thread(Poll).Start();
		}

		~JoystickInput(){
			_isRunning = false;
		}

		private bool _isActive;

		public void Enable(){
			_isActive = true;
		}

		public void Disable(){
			_isActive = false;
		}

		public void Dispose() {
			_isActive = false;
			_isRunning = false;
		}

		private static readonly PlayerIndex[] PlayerIndices = { PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };

		readonly GamePadState[] _gamePadStates = new GamePadState[4];
		readonly uint[] _lastPacketNumbers = new uint[4];
		public int LastActivePlayerIndex;

		public GamePadDeadZone DeadZone { get; set; }
		public int LastActiveIndex { get { return LastActivePlayerIndex; } }
		public GamePadState LastActiveState { get { return _gamePadStates[LastActivePlayerIndex]; } }
		public bool LinkTriggersToVibration { get; set; }

		public static bool GamePadStateEquals(GamePadState a, GamePadState b){
			const float tolerance = 0.01f;
			return
				a.Buttons.LeftShoulder	== b.Buttons.LeftShoulder	 && 
				a.Buttons.A				== b.Buttons.A				 &&
				a.Buttons.B				== b.Buttons.B				 &&
				a.Buttons.Back			== b.Buttons.Back			 &&
				a.Buttons.Guide			== b.Buttons.Guide			 &&
				a.Buttons.LeftShoulder	== b.Buttons.LeftShoulder	 &&
				a.Buttons.LeftStick		== b.Buttons.LeftStick		 &&
				a.Buttons.RightShoulder	== b.Buttons.RightShoulder	 &&
				a.Buttons.RightStick	== b.Buttons.RightStick		 &&
				a.Buttons.Start			== b.Buttons.Start			 &&
				a.Buttons.X				== b.Buttons.X				 &&
				a.Buttons.Y				== b.Buttons.Y				 &&
				a.DPad.Left				== b.DPad.Left				 &&
				a.DPad.Right			== b.DPad.Right				 &&
				a.DPad.Up				== b.DPad.Up				 &&
				a.DPad.Down				== b.DPad.Down				 &&
				System.Math.Abs(a.ThumbSticks.Left.X	- b.ThumbSticks.Left.X	) < tolerance &&
				System.Math.Abs(a.ThumbSticks.Left.Y	- b.ThumbSticks.Left.Y	) < tolerance &&
				System.Math.Abs(a.ThumbSticks.Right.X	- b.ThumbSticks.Right.X	) < tolerance &&
				System.Math.Abs(a.ThumbSticks.Right.Y	- b.ThumbSticks.Right.Y	) < tolerance &&
				System.Math.Abs(a.Triggers.Left			- b.Triggers.Left		) < tolerance &&
				System.Math.Abs(a.Triggers.Right		- b.Triggers.Right		) < tolerance
			;
		}f

		private void Poll(){
			var oldGamePadStates = new GamePadState[4];
			while (_isRunning){
				var hasChanged = false;

				for (var i = 0; i < 4; i++){
					oldGamePadStates[i] = _gamePadStates[i];
					_gamePadStates[i] = GamePad.GetState(PlayerIndices[i], GamePadDeadZone.Circular);

					if (GamePadStateEquals(oldGamePadStates[i], _gamePadStates[i]) == false)
						hasChanged = true;
				}

				var activePlayerIndex = LastActivePlayerIndex;
				for (var i = 0; i < 4; i++){
					if (_gamePadStates[i].PacketNumber == _lastPacketNumbers[i])
						continue;
					activePlayerIndex = i;
					_lastPacketNumbers[i] = _gamePadStates[i].PacketNumber;

					LastActivePlayerIndex = activePlayerIndex;
					if (GamePadStateEquals(oldGamePadStates[i], _gamePadStates[i]) == false){

						if (_isActive == false) {
							if (hasChanged && _gamePadStates[i].Buttons.Back == ButtonState.Pressed)
								GamepadOnStateChanged();
						}
						else if (hasChanged)
							GamepadOnStateChanged();
					}
				}

				

				System.Threading.Thread.Sleep(30);
			}
		}

		private InputState _lastState;

		private void GamepadOnStateChanged(){
			var hasChanged = false;

			ResetButtons();
			var startState = new InputState(this);

			_charPos = new Vector2();
			if (new[]{
				LastActiveState.Buttons.A == ButtonState.Pressed,
				LastActiveState.Buttons.B == ButtonState.Pressed,
				LastActiveState.Buttons.X == ButtonState.Pressed,
				LastActiveState.Buttons.Y == ButtonState.Pressed
			}.Any(p => p)) {
				hasChanged = true;
				if (LastActiveState.Buttons.Y == ButtonState.Pressed) _charPos.Y = 1;
				if (LastActiveState.Buttons.X == ButtonState.Pressed) _charPos.X = 1;
				if (LastActiveState.Buttons.A == ButtonState.Pressed) _charPos.Y = -1;
				if (LastActiveState.Buttons.B == ButtonState.Pressed) _charPos.X = -1;
			}
			
			if (LastActiveState.Buttons.LeftShoulder	== ButtonState.Pressed) { Delete	= true; hasChanged = true; }
			if (LastActiveState.Buttons.RightShoulder	== ButtonState.Pressed) { Space		= true; hasChanged = true; }
			if (LastActiveState.DPad.Left				== ButtonState.Pressed) { MoveLeft	= true; hasChanged = true; } // todo analogue move
			if (LastActiveState.DPad.Right				== ButtonState.Pressed) { MoveRight	= true; hasChanged = true; } // todo analogue move
			if (LastActiveState.Buttons.Back			== ButtonState.Pressed) { OpenClose	= true; hasChanged = true; }
			if (LastActiveState.DPad.Down				== ButtonState.Pressed) { Enter	= true; hasChanged = true; }
			
			const float triggerThreshhold = 0.01f;
			if (LastActiveState.Triggers.Left  > triggerThreshhold) { ChangeCase	= true; hasChanged = true; }
			else { ChangeCase = false; hasChanged = true; }
			if (LastActiveState.Triggers.Right > triggerThreshhold) { ChangeSymbols	= true; hasChanged = true; }
			else { ChangeSymbols = false; hasChanged = true; }
			
			if (HandleBlocks())
				hasChanged = true;
			
			var endState = new InputState(this);
			if (_lastState.Delete		== Delete		) Delete		= false; else if(LastActiveState.Buttons.LeftShoulder	== ButtonState.Released) _lastState.Delete		= false;
			if (_lastState.Space		== Space		) Space			= false; else if(LastActiveState.Buttons.RightShoulder	== ButtonState.Released) _lastState.Space		= false;
			if (_lastState.MoveLeft		== MoveLeft		) MoveLeft		= false; else if(LastActiveState.DPad.Left				== ButtonState.Released) _lastState.MoveLeft	= false;
			if (_lastState.MoveRight	== MoveRight	) MoveRight		= false; else if(LastActiveState.DPad.Right				== ButtonState.Released) _lastState.MoveRight	= false;
		//	if (_lastState.OpenClose	== OpenClose	) OpenClose		= false; else if(LastActiveState.Buttons.Back			== ButtonState.Released) _lastState.OpenClose	= false;
			if (_lastState.Return		== Enter		) Enter		= false; else if(LastActiveState.DPad.Down				== ButtonState.Released) _lastState.Return		= false;
			if (_lastState.CharPos.Y	== CharPos.Y	) _charPos.Y	= 0;
			if (_lastState.CharPos.X	== CharPos.X	) _charPos.X	= 0;
			//	if (_lastState.ChangeCase   == ChangeCase	) ChangeCase	= false;
			//	if (_lastState.ChangeSymbols== ChangeSymbols) ChangeSymbols	= false;

			if (hasChanged == false || startState == endState || KeyChange == null) return;
			_lastState = endState;
			KeyChange(this);
		}

		private bool HandleBlocks(){
			var lstick = LastActiveState.ThumbSticks.Left;
			const double tolerance = 0.0;
			if (System.Math.Abs(lstick.X) < tolerance && System.Math.Abs(lstick.Y) < tolerance)
				return false;

			const float mult = 5f;
			var newPos = new Vector2(
				-lstick.X * mult,
				 lstick.Y * mult
			);

			if (newPos.X > 1) newPos.X = 1;
			else if (newPos.X < -1) newPos.X = -1;

			if (newPos.Y > 1) newPos.Y = 1;
			else if (newPos.Y < -1) newPos.Y = -1;

			_blockPos = newPos;

			return true;
		}

		public void ResetButtons() {
			//ChangeCase = false;
			//ChangeSymbols = false;
			MoveLeft = false;
			MoveRight = false;
			Delete = false;
			Enter = false;
			Space = false;
			OpenClose = false;
		}

		private Vector2 _blockPos;
		private Vector2 _charPos;
		private bool _isRunning;
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
		public bool Enter { get; set; }
		public bool OpenClose { get; set; }
	}
}
