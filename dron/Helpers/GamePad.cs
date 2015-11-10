using System;
using SharpDX.XInput;

namespace dron.Helpers
{
    class GamePad : IControl
    {
        Controller controller;
        public delegate void OnConnectionChange(bool status);
        OnConnectionChange onConnectionChange;
        bool status;
        float leftTrigger, rightTrigger, leftThumbX, leftThumbY, rightThumbX, rightThumbY;
        public bool Connected { get { return controller.IsConnected; } }
        public float LeftTrigger { get { return leftTrigger / 255; } }
        public float RightTrigger { get { return rightTrigger / 255; } }
        public float LeftThumbX { get { return leftThumbX / 32768; } }
        public float LeftThumbY { get { return leftThumbY / 32768; } }
        public float RightThumbX { get { return rightThumbX / 32768; } }
        public float RightThumbY { get { return rightThumbX / 32768; } }
        public bool A { get; private set; }
        public bool B { get; private set; }
        public bool X { get; private set; }
        public bool Y { get; private set; }
        public bool Start { get; private set; }
        public bool Back { get; private set; }
        public bool DPadUp { get; private set; }
        public bool DPadDown { get; private set; }
        public bool DPadLeft { get; private set; }
        public bool LeftShoulder { get; private set; }
        public bool RightShoulder { get; private set; }
        public bool LeftThumb { get; private set; }
        public bool RightThumb { get; private set; }

        public int Roll
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Pitch
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Gaz
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int Yaw
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public GamePad(OnConnectionChange function)
        {
            onConnectionChange += function;
            controller = new Controller(UserIndex.One);
            status = Connected;
            onConnectionChange.Invoke(status);
        }

        public void Update()
        {
            if (Connected)
            {
                if (status != Connected)
                {
                    status = Connected;
                    onConnectionChange.Invoke(status);
                }                
                State state = controller.GetState();
                leftTrigger = state.Gamepad.LeftTrigger;
                rightTrigger = state.Gamepad.RightTrigger;
                leftThumbX = state.Gamepad.LeftThumbX;
                leftThumbY = state.Gamepad.LeftThumbY;
                rightThumbX = state.Gamepad.RightThumbX;
                rightThumbY = state.Gamepad.RightThumbY;
                A = state.Gamepad.Buttons == GamepadButtonFlags.A;
                B = state.Gamepad.Buttons == GamepadButtonFlags.B;
                X = state.Gamepad.Buttons == GamepadButtonFlags.X;
                Y = state.Gamepad.Buttons == GamepadButtonFlags.Y;
                Start = state.Gamepad.Buttons == GamepadButtonFlags.Start;
                Back = state.Gamepad.Buttons == GamepadButtonFlags.Back;
                DPadUp = state.Gamepad.Buttons == GamepadButtonFlags.DPadUp;
                DPadDown = state.Gamepad.Buttons == GamepadButtonFlags.DPadDown;
                DPadLeft = state.Gamepad.Buttons == GamepadButtonFlags.DPadLeft;
                LeftShoulder = state.Gamepad.Buttons == GamepadButtonFlags.LeftShoulder;
                RightShoulder = state.Gamepad.Buttons == GamepadButtonFlags.RightShoulder;
                LeftThumb = state.Gamepad.Buttons == GamepadButtonFlags.LeftThumb;
                RightThumb = state.Gamepad.Buttons == GamepadButtonFlags.RightThumb;
            }
            else
            {
                if (status != Connected)
                {
                    status = Connected;
                    onConnectionChange.Invoke(status);
                }
                leftTrigger = 0;
                rightTrigger = 0;
                leftThumbX = 0;
                leftThumbY = 0;
                rightThumbX = 0;
                rightThumbY = 0;
                A = false;
                B = false;
                X = false;
                Y = false;
                Start = false;
                Back = false;
                DPadUp = false;
                DPadDown = false;
                DPadLeft = false;
                LeftShoulder = false;
                RightShoulder = false;
                LeftThumb = false;
                RightThumb = false;
            }
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }
    }
}
