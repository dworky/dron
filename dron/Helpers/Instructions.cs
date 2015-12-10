using System;
using System.Text;

namespace dron
{
    static class Instructions
    {
        public enum Order { UP, DOWN, FORWARD, BACK, LEFT, RIGHT, ROTATELEFT, ROTATERIGHT, HOVER };      
        public static int Sequence = 0;
        public static Byte[] CurrentCommand = Instructions.MakeCommandPCMD(flag: 0);
        public static float Speed = 0.25f;
        public static Byte[] EmergencySterring(Order order)
        {
            switch (order)
            {
                case Order.UP:
                    return MakeCommandPCMD(gaz: FloatConversion(Speed));
                case Order.DOWN:
                    return MakeCommandPCMD(gaz: FloatConversion(-Speed));
                case Order.FORWARD:
                    return MakeCommandPCMD(pitch: FloatConversion(Speed));
                case Order.BACK:
                    return MakeCommandPCMD(pitch: FloatConversion(-Speed));
                case Order.LEFT:
                    return MakeCommandPCMD(roll: FloatConversion(-Speed));
                case Order.RIGHT:
                    return MakeCommandPCMD(roll: FloatConversion(Speed));
                case Order.ROTATELEFT:
                    return MakeCommandPCMD(yaw: FloatConversion(-Speed));
                case Order.ROTATERIGHT:
                    return MakeCommandPCMD(yaw: FloatConversion(Speed));
                case Order.HOVER:
                    return MakeCommandPCMD(flag: 0);
                default:
                    return MakeCommandPCMD(flag: 0);
            }
        }

        public static int FloatConversion(float value)
        {
            return BitConverter.ToInt32(BitConverter.GetBytes(value), 0);
        }

        /// <summary>
        /// Takeoff/Landing/Emergency stop command
        /// </summary>
        public static Byte[] MakeCommandREF(int input)
        {
            return Encoding.ASCII.GetBytes(String.Format("AT*REF={0},{1}\r", Sequence++, input));           
        }

        /// <summary>
        /// Move the drone: 
        /// (flag == 0) => hover,
        /// roll - left-right tilt,
        /// pitch - front-back tilt,
        /// gaz - vertical speed,
        /// yaw - angular speed
        /// </summary>
        public static Byte[] MakeCommandPCMD(int flag = 1, int roll = 0, int pitch = 0, int gaz = 0, int yaw = 0)
        {
            //Console.WriteLine(String.Format("AT*PCMD={0},{1},{2},{3},{4},{5}\r",
            //    Sequence, flag, roll, pitch, gaz, yaw));
            return Encoding.ASCII.GetBytes(String.Format("AT*PCMD={0},{1},{2},{3},{4},{5}\r", 
                Sequence++, flag, roll, pitch, gaz, yaw));         
        }


        /// <summary>
        /// Set a flight animation on the drone
        /// </summary>
        public static Byte[] MakeCommandANIM(int animation, int duration)
        {
            return Encoding.ASCII.GetBytes(String.Format("AT*ANIM={0},{1},{2}\r",
                Sequence++, animation, duration));
        }

        public static Byte[] MakeCommandCalibrate()
        {
            return Encoding.ASCII.GetBytes(String.Format("AT*FTRIM={0}\r", Sequence++));
        }
    }
}
