using System;
using System.Runtime.InteropServices;
using System.Threading;
using SharpDX;
using ExileCore;
using System.Windows.Forms;


namespace Druzil.Poe.Libs
{
    public class Mouse
    {
        public const int MouseLeftButtonDown = 0x02;
        public const int MouseLeftButtonUp   = 0x04;

        public const int MouseMiddleButtonDown = 0x0020;
        public const int MoueMiddleButtonUp   = 0x0040;

        public const int MouseRightButtonDown = 0x0008;
        public const int MouseRightButtonUp   = 0x0010;
        public const int MouseWheel      = 0x800;

        // 
        private const int MovementDelay = 50;
        private const int ClickDelay    = 1;

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        private static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

		[DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(UInt16 virtualKeyCode);
        //Virtual key codes
        //found at http://msdn.microsoft.com/en-us/library/dd375731(v=VS.85).aspx
        private const UInt16 VK_MBUTTON = 0x04;//middle mouse button
        private const UInt16 VK_LBUTTON = 0x01;//left mouse button
        private const UInt16 VK_RBUTTON = 0x02;//right mouse button


        /// <summary>
        ///     Sets the cursor position relative to the game window.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="gameWindow"></param>
        /// <returns></returns>
        public static bool SetCursorPos(int x, int y, RectangleF gameWindow) => SetCursorPos(x + (int) gameWindow.X, y + (int) gameWindow.Y);

        /// <summary>
        ///     Sets the cursor position to the center of a given rectangle relative to the game window
        /// </summary>
        /// <param name="position"></param>
        /// <param name="gameWindow"></param>
        /// <returns></returns>
        public static bool SetCurosPosToCenterOfRec(RectangleF position, RectangleF gameWindow)
        {
            return SetCursorPos((int)(gameWindow.X + position.Center.X), (int)(gameWindow.Y + position.Center.Y));
        }

        /// <summary>
        ///     Retrieves the cursor's position, in screen coordinates.
        /// </summary>
        /// <see>See MSDN documentation for further information.</see>
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);

        public static SharpDX.Point GetCursorPosition()
        {
            GetCursorPos(out Point lpPoint);
            return lpPoint;
        }

        public static void LeftClick(int extraDelay, int startDelay = 0)
        {
            if (startDelay > 0)
            {
                Thread.Sleep(startDelay);
            }
            LeftMouseDown();
            Thread.Sleep(ClickDelay / 2);
            LeftMouseUp();
            Thread.Sleep(ClickDelay);
        }

        public static void RightClick(int extraDelay, int startDelay = 0)
        {
            if (startDelay > 0)
            {
                Thread.Sleep(startDelay);
            }
            RightMouseDown();
            Thread.Sleep(ClickDelay / 2);
            RightMouseUp();
            Thread.Sleep(ClickDelay);
        }

        public static void LeftMouseDown() { mouse_event(MouseLeftButtonDown, 0, 0, 0, 0); }

        public static void LeftMouseUp() { mouse_event(MouseLeftButtonUp, 0, 0, 0, 0); }

        public static void RightMouseDown() { mouse_event(MouseRightButtonDown, 0, 0, 0, 0); }

        public static void RightMouseUp() { mouse_event(MouseRightButtonUp, 0, 0, 0, 0); }


        public static void SetCursorPosAndLeftClick(int posX , int posY, int extraDelay)
        {
            SetCursorPos(posX, posY);
            Thread.Sleep(MovementDelay + extraDelay);
            mouse_event(MouseLeftButtonDown, 0, 0, 0, 0);
            Thread.Sleep(ClickDelay);
            mouse_event(MouseLeftButtonUp, 0, 0, 0, 0);
        }

        public static void SetCursorPosAndLeftClick(Vector2 coords, int extraDelay)
        {
            int posX = (int) coords.X;
            int posY = (int) coords.Y;
            SetCursorPosAndLeftClick(posX, posY, extraDelay);
        }

        public static void SetCursorPosAndLeftClick(Vector2 coords, Vector2 windowOffset, int extraDelay)
        {
            var posX = (int)(coords.X + windowOffset.X);
            var posY = (int)(coords.Y + windowOffset.Y);
            SetCursorPosAndLeftClick(posX, posY, extraDelay);
        }

        public static void SetCursorPosAndLeftClick(Vector2 coords, Vector2 windowOffset)
        {
            var posX = (int)(coords.X + windowOffset.X);
            var posY = (int)(coords.Y + windowOffset.Y);
            SetCursorPosAndLeftClick(posX, posY, 0);
        }

        public static void SetCursorPosAndLeftClick(Vector2 coords)
        {
            SetCursorPosAndLeftClick(coords, 0);
        }

        public static void VerticalScroll(bool forward, int clicks)
        {
            if (forward)
                mouse_event(MouseWheel, 0, 0, clicks * 120, 0);
            else
                mouse_event(MouseWheel, 0, 0, -(clicks * 120), 0);
        }
        ////////////////////////////////////////////////////////////


        [StructLayout(LayoutKind.Sequential)]
        public struct Point
        {
            public int X;
            public int Y;

            public static implicit operator SharpDX.Point(Point point) => new SharpDX.Point(point.X, point.Y);
        }

        #region MyFix

        private static void SetCursorPosition(float x, float y) { SetCursorPos((int) x, (int) y); }

        public static Vector2 GetCursorPositionVector()
        {
            SharpDX.Point currentMousePoint = GetCursorPosition();
            return new Vector2(currentMousePoint.X, currentMousePoint.Y);
        }

        public static void SetCursorPosition(Vector2 end)
        {
            Vector2 cursor       = GetCursorPositionVector();
            Vector2 stepVector2  = new Vector2();
            float   step         = (float) Math.Sqrt(Vector2.Distance(cursor, end)) * 1.618f;
            if (step > 275) step = 240;
            stepVector2.X        = (end.X - cursor.X) / step;
            stepVector2.Y        = (end.Y - cursor.Y) / step;
            float fX             = cursor.X;
            float fY             = cursor.Y;
            for (int j = 0; j < step; j++)
            {
                fX += +stepVector2.X;
                fY += stepVector2.Y;
                SetCursorPosition(fX, fY);
                Thread.Sleep(2);
            }
        }

        public static void SetCursorPosAndLeftClickHuman(Vector2 coords, int extraDelay)
        {
            SetCursorPosition(coords);
            Thread.Sleep(MovementDelay + extraDelay);
            LeftMouseDown();
            Thread.Sleep(MovementDelay + extraDelay);
            LeftMouseUp();
        }

        public static void SetCursorPos(Vector2 vec) { SetCursorPos((int) vec.X, (int) vec.Y); }

        #endregion

        /// <summary>
        /// returns True if its Pressed
        /// </summary>
        /// <returns></returns>
        public static bool isMiddleButtonPressed()
        {
            // GetAsyncKeyState Returns negative when the button is DOWN and 0 when the button is UP
            return GetAsyncKeyState(VK_MBUTTON) !=0 ;
        }

        /// <summary>
        /// Returns negative when the button is DOWN and 0 when the button is UP
        /// </summary>
        /// <returns></returns>
        public static bool isRightButtonPressed()
        {
            // GetAsyncKeyState Returns negative when the button is DOWN and 0 when the button is UP
            return GetAsyncKeyState(VK_RBUTTON) !=0;
        }
        /// <summary>
        /// Returns negative when the button is DOWN and 0 when the button is UP
        /// </summary>
        /// <returns></returns>
        public static bool isLeftButtonPressed()
        {
            // GetAsyncKeyState Returns negative when the button is DOWN and 0 when the button is UP
            return GetAsyncKeyState(VK_LBUTTON) !=0 ;
        }
    }


    public class KeyboardHelper
    {
        private readonly GameController _gameHandle;
        private float _curLatency;

        private const int KEYEVENTF_EXTENDEDKEY = 0x0001;
        private const int KEYEVENTF_KEYUP = 0x0002;
        private const int KEY_PRESSED = 0x8000;
        private const int KEY_TOGGLED = 0x0001;


        public KeyboardHelper(GameController g)
        {
            _gameHandle = g;
        }

        public void SetLatency(float latency)
        {
            _curLatency = latency;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        /*
                [return: MarshalAs(UnmanagedType.Bool)]
                [DllImport("user32.dll", SetLastError = true)]
                private static extern bool PostMessage(IntPtr hWnd, uint msg, UIntPtr wParam, UIntPtr lParam);
        */
        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(Keys vKey);
        [DllImport("USER32.dll")]
        private static extern short GetKeyState(int nVirtKey);
        [DllImport("user32.dll")]
        private static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);



        //public void KeyDown(Keys key)
        //{
        //    SendMessage(_gameHandle.Window.Process.MainWindowHandle, 0x100, (int)key, 0);
        //}

        public static bool IsKeyDown(Keys key)
        {
            return GetKeyState((int)key) < 0;
        }
               

        //public void KeyUp(Keys key)
        //{
        //    SendMessage(_gameHandle.Window.Process.MainWindowHandle, 0x101, (int)key, 0);
        //}


        public static void KeyDown(Keys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | 0, 0);
        }

        public static void KeyUp(Keys key)
        {
            keybd_event((byte)key, 0, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0);
        }

        public static bool IsKeyToggled(Keys key)
        {
            return Convert.ToBoolean(GetKeyState((int)key) & KEY_TOGGLED);
        }


        public static void KeyPress(Keys key)
        {
            KeyDown(key);
            //var lat = (int)(_curLatency);
            //if (lat < 1000)
            //    Thread.Sleep(lat);
            //else
            //    Thread.Sleep(1000);
            Thread.Sleep(30);
            KeyUp(key);
        }


        //public bool KeyPress(Keys key)
        //{
        //    KeyDown(key);
        //    var lat = (int)(_curLatency);
        //    if (lat < 1000)
        //    {
        //        Thread.Sleep(lat);
        //        return true;
        //    }
        //    else
        //    {
        //        Thread.Sleep(1000);
        //        return false;
        //    }
        //}
    }
}