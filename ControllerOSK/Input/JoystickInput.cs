using System;
using XInputDotNetPure;

namespace ControllerOSK.Input {
	public class JoystickInput : IInput, IDisposable {

        public JoystickInput() {
            RegisterEvents();
            CharPos = new Vector2();
        }

        private JoystickEventDispatcher _gamepadEventOpen = new JoystickEventDispatcher();
        private JoystickEventDispatcher _gamepadEventClosed = new JoystickEventDispatcher();

		~JoystickInput(){
            Dispose();
		}

		public void Enable(){
            Console.WriteLine("Enable");
            _gamepadEventOpen.Enabled = true;
            _gamepadEventClosed.Enabled = false;
		}

		public void Disable(){
            Console.WriteLine("Disable");
            _gamepadEventOpen.Enabled = false;
            _gamepadEventClosed.Enabled = true;
		}

		public void Dispose() {
            _gamepadEventOpen.Enabled = false;
            _gamepadEventClosed.Enabled = false;
            _gamepadEventOpen.Dispose();
            _gamepadEventClosed.Dispose();
        }

        private void RemoveEvents() {

        }

        private void DispatchKeyChange(int player) {
            KeyChange?.Invoke(this);
            CharPos = new Vector2();
        }

        private void RegisterEvents() {
            _gamepadEventOpen.LeftAnalogStick_Changed += (p, v) => {
                BlockPos = v;
                KeyChange?.Invoke(this);
            };

            _gamepadEventOpen.LeftTrigger_Changed += (p, v) => {
                var buttonActive = v > 0.5;
                if (ChangeCase == buttonActive)
                    return;

                ChangeCase = buttonActive;
                KeyChange?.Invoke(this);
            };

            _gamepadEventOpen.RightTrigger_Changed += (p, v) => {
                var buttonActive = v > 0.5;
                if (ChangeSymbols == buttonActive)
                    return;

                ChangeSymbols = buttonActive;
                KeyChange?.Invoke(this);
            };

            _gamepadEventOpen.ButtonA_Down += p => CharPos = new Vector2( 0, -1);
            _gamepadEventOpen.ButtonB_Down += p => CharPos = new Vector2(-1,  0);
            _gamepadEventOpen.ButtonX_Down += p => CharPos = new Vector2( 1,  0);
            _gamepadEventOpen.ButtonY_Down += p => CharPos = new Vector2( 0,  1);
            _gamepadEventOpen.ButtonA_Down += DispatchKeyChange;
            _gamepadEventOpen.ButtonB_Down += DispatchKeyChange;
            _gamepadEventOpen.ButtonX_Down += DispatchKeyChange;
            _gamepadEventOpen.ButtonY_Down += DispatchKeyChange;

            _gamepadEventOpen.ButtonDPadLeft_Down += p => {
                MoveLeft = true;
                KeyChange?.Invoke(this);
                MoveLeft = false;
            };

            _gamepadEventOpen.ButtonDPadRight_Down += p => {
                MoveRight = true;
                KeyChange?.Invoke(this);
                MoveRight = false;
            };

            _gamepadEventOpen.ButtonLeftBumper_Down += p => {
                Delete = true;
                KeyChange?.Invoke(this);
                Delete = false;
            };

            _gamepadEventOpen.ButtonRightBumper_Down += p => {
                Space = true;
                KeyChange?.Invoke(this);
                Space = false;
            };

            _gamepadEventOpen.ButtonDPadDown_Down += p => {
                Enter= true;
                KeyChange?.Invoke(this);
                Enter = false;
            };

            _gamepadEventOpen.ButtonBack_Down += p => {
                Console.WriteLine("open:back button");
                OpenClose = true;
                KeyChange?.Invoke(this);
                OpenClose = false;
                Disable();
            };

            _gamepadEventOpen.ButtonDPadUp_Down += p => {
                Console.WriteLine("open:up button");
                OpenClose = true;
                KeyChange?.Invoke(this);
                OpenClose = false;
                Disable();
            };

            _gamepadEventClosed.ButtonBack_Down += p => {
                Console.WriteLine("closed:back button");
                OpenClose = true;
                KeyChange?.Invoke(this);
                OpenClose = false;
                Enable();
            };
        }

		public event System.Action<IInput> KeyChange;
		public Vector2 BlockPos { get; set; }
        public Vector2 CharPos { get; set; }
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
