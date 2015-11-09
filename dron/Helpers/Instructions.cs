using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dron.Helpers
{
    static class Instructions
    {
        public enum Order { UP, DOWN, FORWARD, BACK, LEFT, RIGHT, ROTATELEFT, ROTATERIGHT, HOVER };
        public static int ValueOfSpeed = 1;
        public static int[] Speeds = new int[]{ -1082130432, -1086324736, -1090519040, -1098907648, 1048576000, 1048576000, 1061158912, 1065353216 };
        public static int Sequence = 1;
        public static Byte[] EmergencySterring(Order order)
        {
            switch (order)
            {
                case Order.UP:
                    return MakeCommandPCMD(gaz: 1);
                case Order.DOWN:
                    return MakeCommandPCMD(gaz: -1);
                case Order.FORWARD:
                    return MakeCommandPCMD(pitch: 1);
                case Order.BACK:
                    return MakeCommandPCMD(pitch: -1);
                case Order.LEFT:
                    return MakeCommandPCMD(roll: -1);
                case Order.RIGHT:
                    return MakeCommandPCMD(roll: 1);
                case Order.ROTATELEFT:
                    return MakeCommandPCMD(yaw: -1);
                case Order.ROTATERIGHT:
                    return MakeCommandPCMD(yaw: 1);
                case Order.HOVER:
                    return MakeCommandPCMD(flag: 0);
                default:
                    return MakeCommandPCMD(flag: 0);
            }
        }

        //public static int FloatConversion(float value)
        //{

        //}

        /// <summary>
        /// Takeoff/Landing/Emergency stop command
        /// </summary>
        public static Byte[] MakeCommandREF(int input)
        {
            return Encoding.ASCII.GetBytes(String.Format("AT*REF={0},{1}\r", Sequence, input));
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
            return Encoding.ASCII.GetBytes(String.Format("AT*PCMD={0},{1},{2},{3},{4},{5}\r", 
                Sequence, flag, roll, pitch, gaz, yaw)); 
        }


        /// <summary>
        /// Set a flight animation on the drone
        /// </summary>
        public static Byte[] MakeCommandANIM(int animation, int duration)
        {
            return Encoding.ASCII.GetBytes(String.Format("AT*ANIM={0},{1},{2}\r",
                Sequence, animation, duration));
        }

    }
}
