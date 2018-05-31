using SharpDX.DirectInput;

namespace MissionPlanner.CamJoystick
{
    public static class Extensions
    {
        public static CamJoystickState CurrentCamJoystickState(this SharpDX.DirectInput.Joystick joystick)
        {
            return new CamJoystickState(joystick.GetCurrentState());
        }
    }

    public class CamJoystickState
    {
        internal JoystickState baseJoystickState;

        public CamJoystickState(JoystickState state)
        {
            this.baseJoystickState = state;
        }

        public int[] GetSlider()
        {
            return this.baseJoystickState.Sliders;
        }

        public int[] GetPointOfView()
        {
            return this.baseJoystickState.PointOfViewControllers;
        }

        public bool[] GetButtons()
        {
            return this.baseJoystickState.Buttons;
        }

        public int AZ
        {
            get
            {
                return this.baseJoystickState.AccelerationZ;
            }
        }

        public int AY
        {
            get
            {
                return this.baseJoystickState.AccelerationY;
            }
        }

        public int AX
        {
            get
            {
                return this.baseJoystickState.AccelerationX;
            }
        }

        public int ARz
        {
            get
            {
                return this.baseJoystickState.AngularAccelerationZ;
            }
        }

        public int ARy
        {
            get
            {
                return this.baseJoystickState.AngularAccelerationY;
            }
        }

        public int ARx
        {
            get
            {
                return this.baseJoystickState.AngularAccelerationX;
            }
        }

        public int FRx
        {
            get
            {
                return this.baseJoystickState.TorqueX;
            }
        }

        public int FRy
        {
            get
            {
                return this.baseJoystickState.TorqueY;
            }
        }

        public int FRz
        {
            get
            {
                return this.baseJoystickState.TorqueZ;
            }
        }

        public int FX
        {
            get
            {
                return this.baseJoystickState.ForceX;
            }
        }

        public int FY
        {
            get
            {
                return this.baseJoystickState.ForceY;
            }
        }

        public int FZ
        {
            get
            {
                return this.baseJoystickState.ForceZ;
            }
        }

        public int Rx
        {
            get
            {
                return this.baseJoystickState.RotationX;
            }
        }

        public int Ry
        {
            get
            {
                return this.baseJoystickState.RotationY;
            }
        }

        public int RZ
        {
            get
            {
                return this.baseJoystickState.RotationZ;
            }
        }

        public int VRx
        {
            get
            {
                return this.baseJoystickState.AngularVelocityX;
            }
        }

        public int VRy
        {
            get
            {
                return this.baseJoystickState.AngularVelocityY;
            }
        }

        public int VRz
        {
            get
            {
                return this.baseJoystickState.AngularVelocityZ;
            }
        }

        public int VX
        {
            get
            {
                return this.baseJoystickState.VelocityX;
            }
        }

        public int VY
        {
            get
            {
                return this.baseJoystickState.VelocityY;
            }
        }

        public int VZ
        {
            get
            {
                return this.baseJoystickState.VelocityZ;
            }
        }

        public int X
        {
            get
            {
                return this.baseJoystickState.X;
            }
        }

        public int Y
        {
            get
            {
                return this.baseJoystickState.Y;
            }
        }

        public int Z
        {
            get
            {
                return this.baseJoystickState.Z;
            }
        }
    }
}
