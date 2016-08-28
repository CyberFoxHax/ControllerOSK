using System.Collections.Generic;
using System.Windows.Input;
using System.Linq;
using Key = ControllerKeyboard.Input.Global.Key;

namespace ControllerKeyboard.Input {
	public class GlobalKeyboardInput : IInput {
		public GlobalKeyboardInput() {
			_globalHandle = new Global.GlobalKeyboardHook();
			_globalHandle.KeyboardPressed += OnKeyEvent;
		}

		~GlobalKeyboardInput(){
			_globalHandle.KeyboardPressed -= OnKeyEvent;
		}

		private readonly Global.GlobalKeyboardHook _globalHandle;

		private void OnKeyEvent(object sender, Global.GlobalKeyboardHookEventArgs e){
			if (e.KeyboardState == Global.KeyboardState.KeyDown)
				OnKeyDown(e.KeyboardData.VirtualCode);

			else if (e.KeyboardState == Global.KeyboardState.KeyUp)
				OnKeyUp(e.KeyboardData.VirtualCode);
		}

		private readonly System.Windows.FrameworkElement _elm;

		public readonly Dictionary<Key, bool> ActiveKeys = new Dictionary<Key, bool>{
			{Key.W, false},
			{Key.A, false},
			{Key.S, false},
			{Key.D, false}
		};

		public void OnKeyDown(Key key) {
			ResetButtons();

			var hasChangd = false;

			if (key == Key.Back) { Delete = true; hasChangd = true; }
			if (key == Key.Space) { Space = true; hasChangd = true; }
			if (key == Key.E) { ChangeCase = true; hasChangd = true; }
			if (key == Key.Q) { ChangeSymbols = true; hasChangd = true; }
			if (key == Key.Z) { MoveLeft = true; hasChangd = true; }
			if (key == Key.X) { MoveRight = true; hasChangd = true; }
			if (key == Key.Escape) { Close = true; hasChangd = true; }
			if (key == Key.F12) { Show = true; hasChangd = true; }

			_charPos = new Vector2();
			if (new[]{
				Key.Up,
				Key.Left,
				Key.Right,
				Key.Down
			}.Contains(key)) {
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

		public void OnKeyUp(Key key) {
			var hasChangd = false;

			if (key == Key.E) { ChangeCase = false; hasChangd = true; }
			if (key == Key.Q) { ChangeSymbols = false; hasChangd = true; }

			if (ActiveKeys.ContainsKey(key))
				ActiveKeys[key] = false;

			if (HandleBlocks(key))
				hasChangd = true;

			if (hasChangd && KeyChange != null)
				KeyChange(this);
		}

		private bool HandleBlocks(Key key) {
			if (new[]{
				Key.W,
				Key.A,
				Key.S,
				Key.D
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

		public Vector2 BlockPos {
			get { return _blockPos; }
			set { _blockPos = value; }
		}

		public Vector2 CharPos {
			get { return _charPos; }
			set { _charPos = value; }
		}

		public void ResetButtons() {
			//ChangeCase = false;
			//ChangeSymbols = false;
			MoveLeft = false;
			MoveRight = false;
			Delete = false;
			Space = false;
			Close = false;
			Show = false;
		}

		public bool ChangeCase { get; set; }
		public bool ChangeSymbols { get; set; }
		public bool MoveLeft { get; set; }
		public bool MoveRight { get; set; }
		public bool Delete { get; set; }
		public bool Space { get; set; }
		public bool Close { get; set; }
		public bool Show { get; set; }
	}
}
