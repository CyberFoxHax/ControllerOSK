using System;
using System.Linq;

namespace ControllerKeyboard.Controls {
	public partial class DisplayInputControl  {
		public DisplayInputControl() {
			InitializeComponent();
			Background = null;

			var children = Children.OfType<InputBlock>().ToArray();

			_elmGrid = new InputBlock[3,3];
			for (int y = 0, i = 0;	y < 3; y++)
			for (var x = 0;			x < 3; x++)
				_elmGrid[x, y] = children[i++];

			var seq = BaseChars;
			for (int i = 0, c = 0; i < seq.Length; i += 4, c++)
				children[c].SetChars(seq.Substring(i, 4));

			SetBlock(children[4]);

			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
			InputSystem = new Input.JoystickInput();
			InputSystem.KeyChange -= InputSystemOnKeyChange;
			InputSystem.KeyChange += InputSystemOnKeyChange;

		}

		private readonly InputBlock[,] _elmGrid;
		private InputBlock _activeElement;
		public Input.IInput InputSystem { get; private set; }

		private void SetBlock(InputBlock elm){
			if (_activeElement != null)
				_activeElement.IsActive = false;

			_activeElement = elm;
			elm.IsActive = true;
		}


		private System.Windows.Controls.UIElementCollection Children {
			get { return Wrapper.Children; }
		}

		public void SwitchUppercase() {
			var children = Children.OfType<InputBlock>().ToArray();
			var seq = BaseCharsUpper;
			for (int i = 0, c = 0; i < seq.Length; i += 4, c++)
				children[c].SetChars(seq.Substring(i, 4));
		}

		public void SwitchLowercase() {
			var children = Children.OfType<InputBlock>().ToArray();
			var seq = BaseChars;
			for (int i = 0, c = 0; i < seq.Length; i += 4, c++)
				children[c].SetChars(seq.Substring(i, 4));
		}

		public void SwitchSymbols() {
			var children = Children.OfType<InputBlock>().ToArray();
			var seq = SymbolChars;
			for (int i = 0, c = 0; i < seq.Length; i += 4, c++)
				children[c].SetChars(seq.Substring(i, 4));
		}

		private void InputSystemOnKeyChange(Input.IInput obj){
			Dispatcher.Invoke(() =>{
				SetBlock(_elmGrid[
					(int)(-obj.BlockPos.X + 1),
					(int)(-obj.BlockPos.Y + 1)
				]);

				if (Math.Abs(obj.CharPos.X) > 0.1f || Math.Abs(obj.CharPos.Y) > 0.1f) {
					var c = _activeElement.GetStr(
						(int)obj.CharPos.X,
						(int)-obj.CharPos.Y
					);
					if (OnKey != null) OnKey(c);
				}
			});
		}

		public event Action<char> OnKey;

		public static readonly string BaseChars;
		public static readonly string BaseCharsUpper;
		public static readonly string SymbolChars;

		static DisplayInputControl(){
			BaseChars = QwertySequence;
			BaseCharsUpper = BaseChars.ToUpper();
			SymbolChars = 
				"!?\"@"+ "%€|&" + "=[]§" +
				"^<>~" + "+-*/" + "°{}¥" +
				".:;," + "_#'-" + "$()£" ;
		}

		private const string AbcSequence =
			"1acb" + "2dfe" + "3gih" +
			"4jlk" + "5mon" + "6prq" +
			"7sut" + "8vxw" + "9y0z";

		private const string QwertySequence =
			"1qew" + "2ryt" + "3uoi" +
			"4psa" + "5dgf" + "6hkj" +
			"7lxz" + "8cbv" + "9n0m";
	}
}
