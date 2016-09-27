
using System.Linq;

namespace ControllerOSK.Views {
	public partial class MainWindow  {
		public MainWindow(){
			System.Windows.Media.RenderOptions.SetBitmapScalingMode(this, System.Windows.Media.BitmapScalingMode.HighQuality);
			InitializeComponent();
			Background = System.Windows.Media.Brushes.Transparent;
			InputControl.OnKey += InputControlOnOnKey;
			InputControl.InputSystem.KeyChange += OpenCloseHandle;
			TextBox.Text = "";
			System.Windows.Controls.Canvas.SetLeft(Caret, 0);
			Loaded += OnLoaded;

			SystemTray.ShowHideClick += OpenClose_OnClick;
			SystemTray.ExitClick += MenuItemExit_OnClick;
			SystemTray.KeyboardHookClick += KeyboardHook_OnClick;
			SystemTray.XInputClick += XInput_OnClick;
			SystemTray.SkinPick += SystemTrayOnSkinPick;
			InputControl.SizeChanged += InputControlOnSizeChanged;
		}

		private void InputControlOnSizeChanged(object sender, System.Windows.SizeChangedEventArgs sizeChangedEventArgs){
			ImageAreaRow.Height = new System.Windows.GridLength(InputControl.Height);
			Height = InfoAreaHeight.Height.Value + InputControl.Height;
			Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2f - Height / 2;
			Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2f - Width / 2;
		}

		private const string CurrentSkinFile = "\\Skins\\currentSkin.txt";

		private void SystemTrayOnSkinPick(string s){
			InputControl.SetSkin(s);
			System.IO.File.WriteAllText(System.Environment.CurrentDirectory + CurrentSkinFile, System.IO.Path.GetFileName(s));
		}

		private void OnLoaded(object sender, System.Windows.RoutedEventArgs routedEventArgs){
			var readSkin = System.IO.File.ReadAllText(System.Environment.CurrentDirectory + CurrentSkinFile);
			InputControl.SetSkin(System.Environment.CurrentDirectory + "\\Skins\\" + readSkin);

			InputControl.InputSystem.Disable();
			Hide();
			ResetText();
			InvalidateVisual();
			_isEnabled = false;
		}

		private void OpenCloseHandle(Input.IInput obj) {
			Dispatcher.Invoke(() => {
				InputSystemOnKeyChange(obj);
			});
		}

		private void MainWindow_OnMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e){
			if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
				DragMove();
		}

		public Input.InputType InputType { get; set; }

		private void OpenClose_OnClick() {
			if (_isEnabled) {
				InputControl.InputSystem.Disable();
				Hide();
				ResetText();
				InvalidateVisual();
				_isEnabled = false;
			}
			else {
				InputControl.InputSystem.Enable();
				Show();
				_isEnabled = true;
			}
		}

		private void MenuItemExit_OnClick(){
			Close();
		}

		private void KeyboardHook_OnClick(){
			if (InputType == Input.InputType.Keyboard) return;
			InputType = Input.InputType.Keyboard;
			InputControl.InputSystem.KeyChange -= OpenCloseHandle;
			InputControl.SetControlSystem(InputType);
			InputControl.InputSystem.KeyChange += OpenCloseHandle;
		}

		private void XInput_OnClick(){
			if (InputType == Input.InputType.XInput) return;
			InputType = Input.InputType.XInput;
			InputControl.InputSystem.KeyChange -= OpenCloseHandle;
			InputControl.SetControlSystem(InputType);
			InputControl.InputSystem.KeyChange += OpenCloseHandle;

		}
	}
}
