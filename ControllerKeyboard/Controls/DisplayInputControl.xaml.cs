using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

		private readonly InputBlock[,] _elmGrid;
		private static readonly Brush ActiveBrush = Brushes.CornflowerBlue;

		private void SetBlock(InputBlock elm){
			if (_activeElement != null)
				_activeElement.Background = null;

			_activeElement = elm;
			elm.Background = ActiveBrush;
		}

		private InputBlock _activeElement;

		public readonly Dictionary<Key, bool> ActiveKeys = new Dictionary<Key, bool>{
			{Key.W, false},
			{Key.A, false},
			{Key.S, false},
			{Key.D, false},
			{Key.Up, false},
			{Key.Left, false},
			{Key.Right, false},
			{Key.Down, false},
		};

		public new void OnKeyDown(KeyEventArgs e){
			if (new[]{
				Key.Up,
				Key.Left,
				Key.Right,
				Key.Down
			}.Contains(e.Key)){
				var x = 0;
				var y = 0;

				if (e.Key == Key.Up	) y = -1;
				if (e.Key == Key.Left	) x = 1;
				if (e.Key == Key.Down	) y = 1;
				if (e.Key == Key.Right) x = -1;

				var str = _activeElement.GetStr(x, y);

				if (OnKey != null) OnKey(str);
			}

			if (new[]{
				Key.W,
				Key.A,
				Key.S,
				Key.D
			}.Contains(e.Key) == false) return;

			ActiveKeys[e.Key] = true;
			HandleKeys();
		}

		public new void OnKeyUp(KeyEventArgs e){
			if (new[]{
				Key.W,
				Key.A,
				Key.S,
				Key.D
			}.Contains(e.Key) == false) return;
			ActiveKeys[e.Key] = false;
			HandleKeys();
		}

		private void HandleKeys(){
			var xDir = 0;
			var yDir = 0;

			if (ActiveKeys[Key.W]) yDir = -1;
			if (ActiveKeys[Key.A]) xDir =-1;
			if (ActiveKeys[Key.S]) yDir = 1;
			if (ActiveKeys[Key.D]) xDir = 1;

			SetBlock(_elmGrid[xDir + 1, yDir + 1]);
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
