using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using XInputDotNetPure;

namespace ControllerOSK.Input
{
    public class JoystickEventDispatcher : IDisposable {
        private bool _enabled;
        private const int PollInterval = 16;

        public bool Enabled {
            get { return _enabled; }
            set {
                var changed = _enabled != value;
                _enabled = value;
                if (changed == false)
                    return;

                if (value)
                    new Thread(Poll).Start();
                else
                    _lastStates = new GamePadState[4];
            }
        }

        private GamePadState[] _lastStates = new GamePadState[4];

        private static float MathfAbs(float value) {
            if (value < 0)
                return -value;
            return value;
        }

        private void Poll() {
            while (true) {
                for (var i = 0; i < 4; i++) {
                    var newState = GamePad.GetState((PlayerIndex)i, GamePadDeadZone.Circular);
                    if (newState.IsConnected == false)
                        continue;

                    var oldState = _lastStates[i];
                    _lastStates[i] = newState;

                    if (newState.PacketNumber <= oldState.PacketNumber)
                        continue;

                    if (oldState.PacketNumber == 0)
                        oldState = newState;

                    const float analogThreshold = 0.01f;

                    if (LeftAnalogStick_Changed != null
                    &&(MathfAbs(newState.ThumbSticks.Left.X - oldState.ThumbSticks.Left.X) > analogThreshold
                    || MathfAbs(newState.ThumbSticks.Left.Y - oldState.ThumbSticks.Left.Y) > analogThreshold))
                        LeftAnalogStick_Changed(i, new Vector2(
                            newState.ThumbSticks.Left.X,
                            newState.ThumbSticks.Left.Y
                        ));

                    if (RightAnalogStick_Changed != null
                    &&(MathfAbs(newState.ThumbSticks.Right.X - oldState.ThumbSticks.Right.X) > analogThreshold
                    || MathfAbs(newState.ThumbSticks.Right.Y - oldState.ThumbSticks.Right.Y) > analogThreshold))
                        RightAnalogStick_Changed(i, new Vector2(
                            newState.ThumbSticks.Right.X,
                            newState.ThumbSticks.Right.Y
                        ));

                    if (LeftTrigger_Changed != null
                    && MathfAbs(newState.Triggers.Left - oldState.Triggers.Left) > analogThreshold)
                        LeftTrigger_Changed(i, newState.Triggers.Left);

                    if (RightTrigger_Changed != null
                    && MathfAbs(newState.Triggers.Right - oldState.Triggers.Right) > analogThreshold)
                        RightTrigger_Changed(i, newState.Triggers.Right);

                    DispatchButton_UpDown(newState.Buttons.A, oldState.Buttons.A, i, ButtonA_Down, ButtonA_Up);
                    DispatchButton_UpDown(newState.Buttons.B, oldState.Buttons.B, i, ButtonB_Down, ButtonB_Up);
                    DispatchButton_UpDown(newState.Buttons.X, oldState.Buttons.X, i, ButtonX_Down, ButtonX_Up);
                    DispatchButton_UpDown(newState.Buttons.Y, oldState.Buttons.Y, i, ButtonY_Down, ButtonY_Up);
                    DispatchButton_UpDown(newState.Buttons.Back, oldState.Buttons.Back, i, ButtonBack_Down, ButtonBack_Up);
                    DispatchButton_UpDown(newState.Buttons.Start, oldState.Buttons.Start, i, ButtonStart_Down, ButtonStart_Up);
                    DispatchButton_UpDown(newState.Buttons.Guide, oldState.Buttons.Guide, i, ButtonGuide_Down, ButtonGuide_Up);
                    DispatchButton_UpDown(newState.DPad.Left, oldState.DPad.Left, i, ButtonDPadLeft_Down, ButtonDPadLeft_Up);
                    DispatchButton_UpDown(newState.DPad.Right, oldState.DPad.Right, i, ButtonDPadRight_Down, ButtonDPadRight_Up);
                    DispatchButton_UpDown(newState.DPad.Up, oldState.DPad.Up, i, ButtonDPadUp_Down, ButtonDPadUp_Up);
                    DispatchButton_UpDown(newState.DPad.Down, oldState.DPad.Down, i, ButtonDPadDown_Down, ButtonDPadDown_Up);
                    DispatchButton_UpDown(newState.Buttons.LeftShoulder, oldState.Buttons.LeftShoulder, i, ButtonLeftBumper_Down, ButtonLeftBumper_Up);
                    DispatchButton_UpDown(newState.Buttons.RightShoulder, oldState.Buttons.RightShoulder, i, ButtonRightBumper_Down, ButtonRightBumper_Up);
                    DispatchButton_UpDown(newState.Buttons.LeftStick, oldState.Buttons.LeftStick, i, ButtonLeftAnalogStick_Down, ButtonLeftAnalogStick_Up);
                    DispatchButton_UpDown(newState.Buttons.RightStick, oldState.Buttons.RightStick, i, ButtonRightAnalogStick_Down, ButtonRightAnalogStick_Up);
                    if (_enabled)
                        Thread.Sleep(PollInterval);
                    else
                        return;
                }
            }
        }

        private static void DispatchButton_UpDown(ButtonState newState, ButtonState oldState, int player, ButtonEvent evt_Down, ButtonEvent evt_Up) {
            if (newState == oldState)
                return;

            if (evt_Down != null && newState == ButtonState.Pressed)
                evt_Down(player);
            else if (evt_Up!= null && newState == ButtonState.Released)
                evt_Up(player);
        }

        public delegate void AnalogEvent<T>(int player, T value);
        public delegate void ButtonEvent(int player);

        public event AnalogEvent<Vector2> LeftAnalogStick_Changed;
        public event AnalogEvent<Vector2> RightAnalogStick_Changed;
        public event AnalogEvent<float> LeftTrigger_Changed;
        public event AnalogEvent<float> RightTrigger_Changed;
        public event ButtonEvent ButtonLeftAnalogStick_Down;
        public event ButtonEvent ButtonLeftAnalogStick_Up;
        public event ButtonEvent ButtonRightAnalogStick_Down;
        public event ButtonEvent ButtonRightAnalogStick_Up;
        public event ButtonEvent ButtonRightBumper_Down;
        public event ButtonEvent ButtonRightBumper_Up;
        public event ButtonEvent ButtonLeftBumper_Down;
        public event ButtonEvent ButtonLeftBumper_Up;
        public event ButtonEvent ButtonA_Down;
        public event ButtonEvent ButtonB_Down;
        public event ButtonEvent ButtonX_Down;
        public event ButtonEvent ButtonY_Down;
        public event ButtonEvent ButtonA_Up;
        public event ButtonEvent ButtonB_Up;
        public event ButtonEvent ButtonX_Up;
        public event ButtonEvent ButtonY_Up;
        public event ButtonEvent ButtonDPadLeft_Down;
        public event ButtonEvent ButtonDPadRight_Down;
        public event ButtonEvent ButtonDPadUp_Down;
        public event ButtonEvent ButtonDPadDown_Down;
        public event ButtonEvent ButtonDPadLeft_Up;
        public event ButtonEvent ButtonDPadRight_Up;
        public event ButtonEvent ButtonDPadUp_Up;
        public event ButtonEvent ButtonDPadDown_Up;
        public event ButtonEvent ButtonBack_Down;
        public event ButtonEvent ButtonStart_Down;
        public event ButtonEvent ButtonGuide_Down;
        public event ButtonEvent ButtonBack_Up;
        public event ButtonEvent ButtonStart_Up;
        public event ButtonEvent ButtonGuide_Up;

        ~JoystickEventDispatcher() {
            Enabled = false;
        }

        public void Dispose() {
            Enabled = false;
        }
    }
}
