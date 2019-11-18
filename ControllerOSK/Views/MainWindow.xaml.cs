
using System.Linq;
using System.Timers;

namespace ControllerOSK.Views {
	public partial class MainWindow  {
		public MainWindow(){
			System.Windows.Media.RenderOptions.SetBitmapScalingMode(this, System.Windows.Media.BitmapScalingMode.HighQuality);
			InitializeComponent();
            if (System.Diagnostics.Debugger.IsAttached) {
                ShowActivated = true;
                Topmost = false;
                ShowInTaskbar = true;
            }
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

            _controllerEvent.RightAnalogStick_Changed += ControllerEvent_RightAnalogStick_Changed;
            _controllerEvent.ButtonRightAnalogStick_Up += ControllerEvent_ButtonRightAnalogStick_Up;
            _controllerEvent.LeftTrigger_Changed += ControllerEvent_Trigger_Changed;
            _controllerEvent.RightTrigger_Changed += ControllerEvent_Trigger_Changed;
            _controllerEvent.Enabled = true;
            _renderLoopTimer.Elapsed += RenderLoop_Elapsed;
            _renderLoopTimer.Start();
        }

        private void RenderLoop_Elapsed(object sender, ElapsedEventArgs e) {
            const double Threshold = 0.01f;
            const float Speed = 40;
            if (System.Math.Abs(_rightThumbPosition.X) < Threshold && System.Math.Abs(_rightThumbPosition.Y) < Threshold)
                return;
            if (_isEnabled == false)
                return;
            Dispatcher.Invoke(() => {
                _renderLoopTimer.Elapsed -= RenderLoop_Elapsed;
                if (_triggerIsDown) {
                    // scale
                    _currentScale += _rightThumbPosition.Y/10;
                    if (_currentScale < 0)
                        _currentScale = 0;
                    else if (_currentScale > 1.36f)
                        _currentScale = 1.36f;

                    var height = _startSize.Y * _currentScale;
                    var heightDiff = height - Height;
                    Height = height;
                    Top -= heightDiff * RenderTransformOrigin.Y;

                    var width = _startSize.X * _currentScale;
                    var widthDiff = width - Width;
                    Width = width;
                    Left -= widthDiff * RenderTransformOrigin.X;
                }
                else {
                    // move
                    var top = Top + -_rightThumbPosition.Y * Speed;
                    var left = Left + _rightThumbPosition.X * Speed;

                    if (HelperArea.Visibility == System.Windows.Visibility.Visible) {
                        if (top + ActualHeight > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height)
                            Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - Height;
                        else if (top < 0)
                            Top = 0;
                        else
                            Top = top;

                        if (left + ActualWidth > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width)
                            Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - Width;
                        else if (left < 0)
                            Left = 0;
                        else
                            Left = left;
                    }
                    else {
                        if (top + InputControl.ActualHeight * _currentScale > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height)
                            Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height - InputControl.ActualHeight * _currentScale;
                        else if (top < 0)
                            Top = 0;
                        else
                            Top = top;

                        var helperToInputControlSizeDiff = (HelperArea.ActualWidth - InputControl.ActualWidth) / 2 * _currentScale;

                        if (left + helperToInputControlSizeDiff + InputControl.ActualWidth * _currentScale > System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width)
                            Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width - helperToInputControlSizeDiff - InputControl.ActualWidth * _currentScale;
                        else if (left < -helperToInputControlSizeDiff)
                            Left = -helperToInputControlSizeDiff;
                        else
                            Left = left;
                    }
                }
                _renderLoopTimer.Elapsed += RenderLoop_Elapsed;
            });
        }

        private void ControllerEvent_Trigger_Changed(int player, float value) {
            _triggerIsDown = value > 0.5;
        }

        private void ControllerEvent_ButtonRightAnalogStick_Up(int player) {
            Dispatcher.Invoke(() => {
                if(HelperArea.Visibility == System.Windows.Visibility.Hidden)
                    HelperArea.Visibility = System.Windows.Visibility.Visible;
                else
                    HelperArea.Visibility = System.Windows.Visibility.Hidden;
            });
        }

        private void ControllerEvent_RightAnalogStick_Changed(int player, Vector2 value) {
            _rightThumbPosition = value;
        }


        private Timer _renderLoopTimer = new Timer(16) { AutoReset = true };
        private Input.JoystickEventDispatcher _controllerEvent = new Input.JoystickEventDispatcher();
        private Vector2 _startPosition;
        private Vector2 _startSize;
        private Vector2 _rightThumbPosition;
        private float _currentScale = 1;
        private bool _triggerIsDown = false;

		private void InputControlOnSizeChanged(object sender, System.Windows.SizeChangedEventArgs sizeChangedEventArgs){
			ImageAreaRow.Height = new System.Windows.GridLength(InputControl.Height);
			Height = InfoAreaHeight.Height.Value + InputControl.Height;
			Top = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / 2f - Height / 2;
			Left = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / 2f - Width / 2;

            _startSize = new Vector2((float) ActualWidth, (float) ActualHeight);
            _startPosition = new Vector2(
                (float)Left,
			    (float)Top
            );
            InputControl.SizeChanged -= InputControlOnSizeChanged;

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
			Reset();
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
				Reset();
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
