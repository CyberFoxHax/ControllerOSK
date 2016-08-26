using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ControllerKeyboard.Controls {
	public partial class DisplayInputControl  {
		public DisplayInputControl() {
			InitializeComponent();
			Background = null;

			var children = Children.OfType<InputBlock>().ToArray();

			_elmGrid = new InputBlock[3,3];
			_elmGrid[0, 0] = children[0];
			_elmGrid[1, 0] = children[1];
			_elmGrid[2, 0] = children[2];
			_elmGrid[0, 1] = children[3];
			_elmGrid[1, 1] = children[4];
			_elmGrid[2, 1] = children[5];
			_elmGrid[0, 2] = children[6];
			_elmGrid[1, 2] = children[7];
			_elmGrid[2, 2] = children[8];

			for (int i = 0, c = 0; i < _qwertySequence.Length; i+=4, c++){
				var child = children[c];
				child.SetChars(_qwertySequence.Substring(i, 4));
			}

			SetBlock(children[4]);
		}

		protected override void OnVisualParentChanged(DependencyObject oldParent){
			if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;
			InputSystem = new Input.KeyboardInput(this.GetParentWindow());
			InputSystem.KeyChange += InputSystemOnKeyChange;
			base.OnVisualParentChanged(oldParent);
		}

		public Input.IInput InputSystem { get; private set; }
		private readonly InputBlock[,] _elmGrid;
		public Brush ActiveBrush = Brushes.DeepSkyBlue;

		private void SetBlock(InputBlock elm){
			if (_activeElement != null)
				_activeElement.Background = null;

			_activeElement = elm;
			elm.Background = ActiveBrush;
		}

		private InputBlock _activeElement;

		private void InputSystemOnKeyChange(Input.IInput obj) {
			SetBlock(_elmGrid[
				(int) (-obj.BlockPos.X + 1),
				(int) (-obj.BlockPos.Y + 1)
			]);

			if (Math.Abs(obj.CharPos.X) > 0.1f || Math.Abs(obj.CharPos.Y) > 0.1f){
				var c = _activeElement.GetStr(
					(int) obj.CharPos.X,
					(int) -obj.CharPos.Y
				);
				if (OnKey != null) OnKey(c);
			}
		}

		public event Action<char> OnKey;

		private const string _abcSequence =
			"1acb" + "2dfe" + "3gih" +
			"4jlk" + "5mon" + "6prq" +
			"7sut" + "8vxw" + "9y0z";

		private const string _qwertySequence =
			"1qew" + "2ryt" + "3uoi" +
			"4psa" + "5dgf" + "6hkj" +
			"7lxz" + "8cbv" + "9n0m";
	}
}
