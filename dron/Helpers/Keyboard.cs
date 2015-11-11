using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace dron.Helpers
{
    class Keyboard : IControl
    {
        public bool W { get; set; }
        public bool S { get; set; }
        public bool A { get; set; }
        public bool D { get; set; }
        public bool UP { get; set; }
        public bool DOWN { get; set; }
        public bool LEFT { get; set; }
        public bool RIGHT { get; set; }

        public Keyboard()
        {
            Refresh();
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
                if (UP && DOWN)
                {
                    return 0;
                }
                else if (UP)
                {
                    return Instructions.FloatConversion(Instructions.Speed);
                }
                else if (DOWN)
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
                if (LEFT && RIGHT)
                {
                    return 0;
                }
                else if (RIGHT)
                {
                    return Instructions.FloatConversion(Instructions.Speed);
                }
                else if (LEFT)
                {
                    return Instructions.FloatConversion(-Instructions.Speed);
                }
                return 0;
            }
        }

        public void Refresh()
        {
            var key = KeyboardInfo.GetKeyState(Keys.W);
            W = key.IsPressed;
            key = KeyboardInfo.GetKeyState(Keys.S);
            S = key.IsPressed;
            key = KeyboardInfo.GetKeyState(Keys.A);
            A = key.IsPressed;
            key = KeyboardInfo.GetKeyState(Keys.D);
            D = key.IsPressed;
            key = KeyboardInfo.GetKeyState(Keys.Up);
            UP = key.IsPressed;
            key = KeyboardInfo.GetKeyState(Keys.Down);
            DOWN = key.IsPressed;
            key = KeyboardInfo.GetKeyState(Keys.Left);
            LEFT = key.IsPressed;
            key = KeyboardInfo.GetKeyState(Keys.Right);
            RIGHT = key.IsPressed;
        }
    }

    /******************************************************/
    /*          NULLFX FREE SOFTWARE LICENSE              */
    /******************************************************/
    /*  GetKeyState Utility                               */
    /*  by: Steve Whitley                                 */
    /*  © 2005 NullFX Software                            */
    /*                                                    */
    /* NULLFX SOFTWARE DISCLAIMS ALL WARRANTIES,          */
    /* RESPONSIBILITIES, AND LIABILITIES ASSOCIATED WITH  */
    /* USE OF THIS CODE IN ANY WAY, SHAPE, OR FORM        */
    /* REGARDLESS HOW IMPLICIT, EXPLICIT, OR OBSCURE IT   */
    /* IS. IF THERE IS ANYTHING QUESTIONABLE WITH REGARDS */
    /* TO THIS SOFTWARE BREAKING AND YOU GAIN A LOSS OF   */
    /* ANY NATURE, WE ARE NOT THE RESPONSIBLE PARTY. USE  */
    /* OF THIS SOFTWARE CREATES ACCEPTANCE OF THESE TERMS */
    /*                                                    */
    /* USE OF THIS CODE MUST RETAIN ALL COPYRIGHT NOTICES */
    /* AND LICENSES (MEANING THIS TEXT).                  */
    /*                                                    */
    /******************************************************/
    public class KeyboardInfo
    {
        private KeyboardInfo() { }
        [DllImport("user32")]
        private static extern short GetKeyState(int vKey);
        public static KeyStateInfo GetKeyState(Keys key)
        {
            short keyState = GetKeyState((int)key);
            byte[] bits = BitConverter.GetBytes(keyState);
            bool toggled = bits[0] > 0, pressed = bits[1] > 0;
            return new KeyStateInfo(key, pressed, toggled);
        }
    }

    public struct KeyStateInfo
    {
        Keys _key;
        bool _isPressed,
            _isToggled;
        public KeyStateInfo(Keys key, bool ispressed, bool istoggled)
        {
            _key = key;
            _isPressed = ispressed;
            _isToggled = istoggled;
        }
        public static KeyStateInfo Default
        {
            get
            {
                return new KeyStateInfo(Keys.None, false, false);
            }
        }
        public Keys Key
        {
            get { return _key; }
        }
        public bool IsPressed
        {
            get { return _isPressed; }
        }
        public bool IsToggled
        {
            get { return _isToggled; }
        }
    }   
}
