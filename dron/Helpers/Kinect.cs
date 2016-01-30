using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using dron.Helpers;
using Microsoft.Kinect;

namespace dron
{
    internal class Kinect : PropertyChangedNotification, IControl
    {
        private KinectSensor sensor;
        public KinectSensor Sensor
        {
            get { return sensor; }
            set { SetProperty(ref sensor, value); }
        }
        public bool W { get; set; }
        public bool S { get; set; }
        public bool A { get; set; }
        public bool D { get; set; }
        public bool Up { get; set; }
        public bool Down { get; set; }
        public bool Left { get; set; }
        public bool Right { get; set; }

        public bool IsSkeletonTracked
        {
            get { return isSkeletonTracked; }
            set { SetProperty(ref isSkeletonTracked, value); }

        }

        public JointCollection Joints
        {
            get { return joints; }
            set
            {
                SetProperty(ref joints, value);
                UpdateValues();
            }
        }

        public string ActiveCommands
        {
            get { return activeCommands; }
            set { SetProperty(ref activeCommands, value); }
        }

        private void UpdateValues()
        {
            Head = $"X:{Joints[JointType.Head].Position.X.ToString("F2")} Y:{Joints[JointType.Head].Position.Y.ToString("F2")} Z:{Joints[JointType.Head].Position.Z.ToString("F2")}";
            ActiveCommand();
            LeftHand = $"X:{Joints[JointType.HandLeft].Position.X.ToString("F2")} Y:{Joints[JointType.HandLeft].Position.Y.ToString("F2")} Z:{Joints[JointType.HandLeft].Position.Z.ToString("F2")}";
            RightHand = $"X:{Joints[JointType.HandRight].Position.X.ToString("F2")} Y:{Joints[JointType.HandRight].Position.Y.ToString("F2")} Z:{Joints[JointType.HandRight].Position.Z.ToString("F2")}";
            LeftElbow = $"X:{Joints[JointType.ElbowLeft].Position.X.ToString("F2")} Y:{Joints[JointType.ElbowLeft].Position.Y.ToString("F2")} Z:{Joints[JointType.ElbowLeft].Position.Z.ToString("F2")}";
            RightElbow = $"X:{Joints[JointType.ElbowRight].Position.X.ToString("F2")} Y:{Joints[JointType.ElbowRight].Position.Y.ToString("F2")} Z:{Joints[JointType.ElbowRight].Position.Z.ToString("F2")}";
            ShoulderCenter = $"X:{Joints[JointType.ShoulderCenter].Position.X.ToString("F2")} Y:{Joints[JointType.ShoulderCenter].Position.Y.ToString("F2")} Z:{Joints[JointType.ShoulderCenter].Position.Z.ToString("F2")}";
            LeftHip = $"X:{Joints[JointType.HipLeft].Position.X.ToString("F2")} Y:{Joints[JointType.HipLeft].Position.Y.ToString("F2")} Z:{Joints[JointType.HipLeft].Position.Z.ToString("F2")}";
            RightHip = $"X:{Joints[JointType.HipRight].Position.X.ToString("F2")} Y:{Joints[JointType.HipRight].Position.Y.ToString("F2")} Z:{Joints[JointType.HipRight].Position.Z.ToString("F2")}";
        }

        private void ActiveCommand()
        {
            var properties = typeof(Kinect).GetProperties();
            var activeCommands = properties.Where(x => x.PropertyType == typeof (bool) && Convert.ToBoolean(x.GetValue(this, null)) && x.Name!="IsSkeletonTracked").Select(x=>x.Name).ToList();
            ActiveCommands = activeCommands.Any() ? activeCommands?.Aggregate((x, y) => $"{x}, {y}") : null;
        }

        public string Head
        {
            get { return head; }
            set { SetProperty(ref head, value); }
        }

        public string LeftHand
        {
            get { return leftHand; }
            set { SetProperty(ref leftHand, value); }
        }

        public string RightHand
        {
            get { return rightHand; }
            set { SetProperty(ref rightHand, value); }
        }

        public string LeftElbow
        {
            get { return leftElbow; }
            set { SetProperty(ref leftElbow, value); }
        }

        public string RightElbow
        {
            get { return rightElbow; }
            set { SetProperty(ref rightElbow, value); }
        }

        public string ShoulderCenter
        {
            get { return shoulderCenter; }
            set { SetProperty(ref shoulderCenter, value); }
        }

        public string LeftHip
        {
            get { return leftHip; }
            set { SetProperty(ref leftHip, value); }
        }

        public string RightHip
        {
            get { return rightHip; }
            set { SetProperty(ref rightHip, value); }
        }


        public EventHandler SkeletonTracked;
        private JointCollection joints;
        private string head;
        private string leftHand;
        private string rightHand;
        private string leftElbow;
        private string rightElbow;
        private string shoulderCenter;
        private string leftHip;
        private string rightHip;
        private string activeCommands;
        private bool isSkeletonTracked;

        public Kinect()
        {
            sensor = KinectSensor.KinectSensors.FirstOrDefault(x => x.Status == KinectStatus.Connected);

            if (sensor==null) return;
            sensor.SkeletonStream.Enable();
            sensor.SkeletonFrameReady += this.SensorSkeletonFrameReady;
            try
            {
                sensor.DepthStream.Enable();
                sensor.ColorStream.Enable();
                sensor.Start();
            }
            catch (IOException)
            {
                sensor = null;
            }
        }

        public void Stop()
        {
            sensor?.Stop();
            sensor?.Dispose();
        }
    
        private void SensorSkeletonFrameReady(object sender, SkeletonFrameReadyEventArgs e)
        {

            var skeletons = new Skeleton[0];
            using (var skeletonFrame = e.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    skeletons = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    skeletonFrame.CopySkeletonDataTo(skeletons);

                }
            }
            foreach (var skeleton in skeletons)
            {
                Joints = skeleton.Joints;
                IsSkeletonTracked = skeleton.TrackingState == SkeletonTrackingState.Tracked;
                SkeletonTracked?.Invoke(IsSkeletonTracked, new EventArgs());
                var trackedJoints = Joints
                    .Where(x => x.TrackingState == JointTrackingState.Tracked)
                    .Select(x=> $"{x.JointType.ToString()} X:{x.Position.X.ToString("F2")} Y:{x.Position.Y.ToString("F2")} Z:{x.Position.Z.ToString("F2")}")
                    .ToList();         
                if (Joints[JointType.HandLeft].Position.Y < Joints[JointType.HipLeft].Position.Y &&
                    Joints[JointType.HandRight].Position.Y < Joints[JointType.HipRight].Position.Y || !Joints.Any(x => x.Position.Z > 0))
                    return;

                if (Joints[JointType.HandRight].Position.Y > Joints[JointType.Spine].Position.Y &&
                    Joints[JointType.HandLeft].Position.Y > Joints[JointType.Spine].Position.Y &&
                    Joints[JointType.ElbowLeft].Position.Y < Joints[JointType.Spine].Position.Y &&
                    Joints[JointType.ElbowRight].Position.Y < Joints[JointType.Spine].Position.Y)
                {
                    S = true;
                }

                if (Joints[JointType.HandRight].Position.Y > Joints[JointType.ShoulderCenter].Position.Y  &&
                    Joints[JointType.HandLeft].Position.Y > Joints[JointType.ShoulderCenter].Position.Y &&
                    Joints[JointType.ElbowLeft].Position.Y < Joints[JointType.ShoulderCenter].Position.Y &&
                    Joints[JointType.ElbowRight].Position.Y < Joints[JointType.ShoulderCenter].Position.Y)
                {
                    W = true;
                }

                if (Joints[JointType.HandRight].Position.Y > Joints[JointType.ShoulderCenter].Position.Y &&
                    Joints[JointType.ElbowRight].Position.Y > Joints[JointType.ShoulderCenter].Position.Y &&
                    Joints[JointType.HandRight].Position.Y > Joints[JointType.Head].Position.Y &&
                    Joints[JointType.HandLeft].Position.Y < Joints[JointType.Spine].Position.Y)
                {
                    Up = true;
                }

                if (Joints[JointType.HandLeft].Position.Y > Joints[JointType.ShoulderCenter].Position.Y &&
                    Joints[JointType.ElbowLeft].Position.Y > Joints[JointType.ShoulderCenter].Position.Y &&
                    Joints[JointType.HandLeft].Position.Y > Joints[JointType.Head].Position.Y &&
                    Joints[JointType.HandRight].Position.Y < Joints[JointType.Spine].Position.Y)
                {
                    Down = true;
                }

                if (Joints[JointType.HandLeft].Position.X > Joints[JointType.HipCenter].Position.X)
                {
                    Right = true;
                }
                if (Joints[JointType.HandRight].Position.X < Joints[JointType.HipCenter].Position.X)
                {
                    Left = true;
                }

                if (Joints[JointType.HandLeft].Position.Y > Joints[JointType.Spine].Position.Y &&
                    Joints[JointType.ElbowLeft].Position.Y > Joints[JointType.Spine].Position.Y &&
                    Joints[JointType.HandLeft].Position.Y < Joints[JointType.Head].Position.Y &&
                    Joints[JointType.ElbowLeft].Position.Y < Joints[JointType.Head].Position.Y &&
                     Joints[JointType.HandRight].Position.Y < Joints[JointType.Spine].Position.Y)
                {
                    A = true;
                }

            }
        }

        public void Reset()
        {
            Left = Right = W = A = S = D = Up = Down = false;
        }

        public int Gaz
        {
            get
            {
                if (W && S)
                {
                    return 0;
                }
                else if (W)
                {
                    return Instructions.FloatConversion(Instructions.Speed);
                }
                else if (S)
                {
                    return Instructions.FloatConversion(-Instructions.Speed);
                }
                return 0;
            }
        }

        public int Pitch
        {
            get
            {
                if (Up && Down)
                {
                    return 0;
                }
                else if (Up)
                {
                    return Instructions.FloatConversion(Instructions.Speed);
                }
                else if (Down)
                {
                    return Instructions.FloatConversion(-Instructions.Speed);
                }
                return 0;
            }
        }

        public int Roll
        {
            get
            {
                if (A && D)
                {
                    return 0;
                }
                else if (D)
                {
                    return Instructions.FloatConversion(Instructions.Speed);
                }
                else if (A)
                {
                    return Instructions.FloatConversion(-Instructions.Speed);
                }
                return 0;
            }
        }

        public int Yaw
        {
            get
            {
                if (Left && Right)
                {
                    return 0;
                }
                else if (Right)
                {
                    return Instructions.FloatConversion(Instructions.Speed);
                }
                else if (Left)
                {
                    return Instructions.FloatConversion(-Instructions.Speed);
                }
                return 0;
            }
        }

        public void Refresh()
        {
        }
    }
}
