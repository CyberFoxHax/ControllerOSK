using System.Collections.Generic;
using System.Linq;
using Key = ControllerOSK.Input.Global.Key;

namespace ControllerOSK.Input {
	public class GlobalKeyboardInput : IInput {
		public GlobalKeyboardInput() {
			_globalHandle = new Global.GlobalKeyboardHook();
			_globalHandle.KeyboardPressed += OnKeyEvent;
		}

		public void Enable() {
			_cancelEvent = true;
		}

		public void Disable() {
			_cancelEvent = false;
		}

		public void Dispose() {
			_cancelEvent = false;
			_globalHandle.KeyboardPressed -= OnKeyEvent;
		}

		private bool _cancelEvent = true;

		~GlobalKeyboardInput(){
			_globalHandle.KeyboardPressed -= OnKeyEvent;
		}

		private readonly Global.GlobalKeyboardHook _globalHandle;

		private void OnKeyEvent(object sender, Global.GlobalKeyboardHookEventArgs e){
			switch (e.KeyboardState){
				case Global.KeyboardState.KeyDown: OnKeyDown(e.KeyboardData.VirtualCode); break;
				case Global.KeyboardState.KeyUp	 : OnKeyUp	(e.KeyboardData.VirtualCode); break;
				case Global.KeyboardState.SysKeyDown: break;
				case Global.KeyboardState.SysKeyUp: break;
				default:
					throw new System.ArgumentOutOfRangeException();
			}
			if (System.Enum.IsDefined(typeof (Key), e.KeyboardData.VirtualCode))
				e.Handled = _cancelEvent;
		}

		public readonly Dictionary<Key, bool> ActiveKeys = new Dictionary<Key, bool>{
			{Key.W, false},
			{Key.A, false},
			{Key.S, false},
			{Key.D, false}
		};

		public void OnKeyDown(Key key){
			ResetButtons();

			var hasChangd = false;

			if (key == Key.Back)	{Delete			= true;	hasChangd = true;}
			if (key == Key.Space)	{Space			= true;	hasChangd = true;}
			if (key == Key.E)		{ChangeSymbols	= true;	hasChangd = true;}
			if (key == Key.Q)		{ChangeCase		= true;	hasChangd = true;}
			if (key == Key.Z)		{MoveLeft		= true;	hasChangd = true;}
			if (key == Key.X)		{MoveRight		= true;	hasChangd = true;}
			if (key == Key.F12)		{OpenClose		= true;	hasChangd = true;}
			if (key == Key.Enter)	{Enter			= true;	hasChangd = true;}

			_charPos = new Vector2();
			if (new[]{
				Key.Up, Key.Left, Key.Right, Key.Down
			}.Contains(key)){
				hasChangd = true;
				if (key == Key.Up) _charPos.Y = 1;
				if (key == Key.Left) _charPos.X = 1;
				if (key == Key.Down) _charPos.Y = -1;
				if (key == Key.Right) _charPos.X = -1;
			}

			if (ActiveKeys.ContainsKey(key))
				ActiveKeys[key] = true;

			if (HandleBlocks(key))
				hasChangd = true;

			if (hasChangd && KeyChange != null)
				KeyChange(this);
			_charPos = new Vector2();
			ResetButtons();
		}

		public void OnKeyUp(Key key){
			var hasChangd = false;

			if (key == Key.Q){ ChangeCase	 = false; hasChangd = true; }
			if (key == Key.E){ ChangeSymbols = false; hasChangd = true; }

			if (ActiveKeys.ContainsKey(key))
				ActiveKeys[key] = false;

			if (HandleBlocks(key))
				hasChangd = true;

			if (hasChangd && KeyChange != null)
				KeyChange(this);
		}

		private bool HandleBlocks(Key key){
			if (new[]{
				Key.W, Key.A, Key.S, Key.D
			}.Contains(key) == false)
				return false;
			_blockPos = new Vector2();

			if (ActiveKeys[Key.W]) _blockPos.Y = 1;
			if (ActiveKeys[Key.A]) _blockPos.X = 1;
			if (ActiveKeys[Key.S]) _blockPos.Y = -1;
			if (ActiveKeys[Key.D]) _blockPos.X = -1;

			return true;
		}

		private Vector2 _blockPos;
		private Vector2 _charPos;

		public event System.Action<IInput> KeyChange;

		public Vector2 BlockPos
		{
			get { return _blockPos; }
			set { _blockPos = value; }
		}

		public Vector2 CharPos
		{
			get { return _charPos; }
			set { _charPos = value; }
		}

		public void ResetButtons(){
			//ChangeCase = false;
			//ChangeSymbols = false;
			MoveLeft = false;
			MoveRight = false;
			Delete = false;
			Space = false;
			OpenClose = false;
		}

		public bool ChangeCase { get; set; }
		public bool ChangeSymbols { get; set; }
		public bool MoveLeft { get; set; }
		public bool MoveRight { get; set; }
		public bool Delete { get; set; }
		public bool Space { get; set; }
		public bool Enter { get; set; }
		public bool OpenClose { get; set; }
		public bool Show { get; set; }
	}
}
