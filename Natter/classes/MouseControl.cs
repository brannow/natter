using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Input;

namespace FBO.Classes
{
    public class MouseControl
    {
        private static readonly Random rnd = new Random();
        private static readonly object Lock = new object();
        private static readonly Mouse.NormalDistribution midpointDistribution = new Mouse.NormalDistribution();

        public static void Move(Point p)
        {
            Move(p.X, p.Y);
        }

        public static void Move(int x, int y)
        {
            //For simplicity, ensure that the following is not executed in parallel
            lock (Lock)
            {
                bool isSlowMove = true;

                //declare the original pointer position
                int originalX = Cursor.Position.X;
                int originalY = Cursor.Position.Y;

                // get a random gauss accuracy
                double gaussX = midpointDistribution.NextGaussian();
                gaussX = gaussX - Math.Truncate(gaussX);
                double gaussY = midpointDistribution.NextGaussian();
                gaussY = gaussY - Math.Truncate(gaussY);

                //calculate an X-Y offset to mimic the accuracy of a human
                int targetX = (int)((double)x + (20.0 * gaussX));
                int targetY = (int)((double)y + (20.0 * gaussY));

                double xLen = targetX - originalX;
                double yLen = targetY - originalY;
                double xMid = originalX + (xLen / 2);
                double yMid = originalY + (yLen / 2);

                double dx = ((xLen) * gaussX) / 2;
                double dy = ((yLen) * gaussY) / 2;

                //Find a co-ordinate normal to the straight line between start and end point, starting at the midpoint and normally distributed
                //This is reduced by a factor of 4 to model the arc of a right handed user.
                int bezierMidPointX = Convert.ToInt32(xMid + dx);
                int bezierMidPointY = Convert.ToInt32(yMid + dy);


                Mouse.BezierCurve bc = new Mouse.BezierCurve();
                double[] input = new double[] { originalX, originalY, bezierMidPointX, bezierMidPointY, targetX, targetY };
                int dist = 800;

                double[] output = new double[dist];

                //co-ords are couplets of doubles hence the / 2
                bc.Bezier2D(input, (int)Math.Floor((double)dist / 2), output);

                int pauseCounter = 0;
                Point lastPoint = new Point(originalX, originalY);
                for (int i = 0; i < dist; i += 4)
                {
                    pauseCounter++;
                    lastPoint.X = (int)output[i];
                    lastPoint.Y = (int)output[i + 1];
                    Cursor.Position = lastPoint;
                    if (pauseCounter % 4 == 0)
                    {
                        Thread.Sleep(2);
                    }
                    if (isSlowMove && pauseCounter % 7 == 0)
                    {
                        Thread.Sleep(1);
                    }
                }

                int diffX = x - targetX;
                int diffY = y - targetY;
                int maxSteps = Math.Max(Math.Max(Math.Abs(diffX), Math.Abs(diffY)), 2) / 2;
                if (maxSteps > 1)
                {
                    double singleStepX = (double)diffX / maxSteps;
                    double singleStepY = (double)diffY / maxSteps;
                    double curentDoubleX = (double)targetX;
                    double curentDoubleY = (double)targetY;
                    for (int i = 0; i < maxSteps / 2; i++)
                    {
                        curentDoubleX += singleStepX;
                        curentDoubleY += singleStepY;
                        lastPoint.X = (int)Math.Floor(curentDoubleX);
                        lastPoint.Y = (int)Math.Floor(curentDoubleY);
                        Cursor.Position = lastPoint;
                        Thread.Sleep(5);
                    }
                }
                Cursor.Position = new Point(x, y);
            }
        }

        public static void RightClick()
        {
            Thread.Sleep(rnd.Next(2, 30));
            mouse_event((int)(MouseEventFlags.RIGHTDOWN), 0, 0, 0, 0);
            Thread.Sleep(rnd.Next(120, 600));
            mouse_event((int)(MouseEventFlags.RIGHTUP), 0, 0, 0, 0);
            Thread.Sleep(rnd.Next(2, 30));
        }

        public static void LeftClick()
        {
            Thread.Sleep(rnd.Next(2, 30));
            mouse_event((int)(MouseEventFlags.LEFTDOWN), 0, 0, 0, 0);
            Thread.Sleep(rnd.Next(120, 600));
            mouse_event((int)(MouseEventFlags.LEFTUP), 0, 0, 0, 0);
            Thread.Sleep(rnd.Next(2, 30));
        }

        public static void HoldShift()
        {
            if (!IsKeyDown((Keys)0x10))
            {
                keybd_event(0x10,
                      0x45,
                      KEYEVENTF_EXTENDEDKEY | 0,
                      0);
            }
        }

        public static void RandomWiggle()
        {
            Point rp = Cursor.Position;
            for (uint i = 0; i < rnd.Next(-1, 5); ++i)
            {
                rp.X += rnd.Next(-8, 8);
                rp.Y += rnd.Next(-8, 8);
                MouseControl.Move(rp);
                Thread.Sleep(rnd.Next(5, 10));
            }
        }

        public static void ReleaseShift()
        {
            if (IsKeyDown((Keys)0x10))
            {
                keybd_event(0x10,
                      0x45,
                      KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP,
                      0);
            }
        }

        public static void PressOne()
        {
            ReleaseShift();

            keybd_event(0x31,
                      0x45,
                      KEYEVENTF_EXTENDEDKEY | 0,
                      0);
            System.Threading.Thread.Sleep(rnd.Next(20, 80));
            keybd_event(0x31,
                      0x45,
                      KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP,
                      0);

        }

        private static KeyStates GetKeyState(Keys key)
        {
            KeyStates state = KeyStates.None;

            short retVal = GetKeyState((int)key);

            //If the high-order bit is 1, the key is down
            //otherwise, it is up.
            if ((retVal & 0x8000) == 0x8000)
                state |= KeyStates.Down;

            //If the low-order bit is 1, the key is toggled.
            if ((retVal & 1) == 1)
                state |= KeyStates.Toggled;

            return state;
        }

        public static bool IsKeyDown(Keys key)
        {
            return KeyStates.Down == (GetKeyState(key) & KeyStates.Down);
        }

        public static bool IsKeyToggled(Keys key)
        {
            return KeyStates.Toggled == (GetKeyState(key) & KeyStates.Toggled);
        }

        [DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        public const int KEYEVENTF_KEYUP = 0x02;
        public const int KEYEVENTF_EXTENDEDKEY = 0x01;

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        private static extern short GetKeyState(int keyCode);

        [Flags]
        private enum KeyStates
        {
            None = 0,
            Down = 1,
            Toggled = 2
        }

        [Flags]
        public enum MouseEventFlags
        {
            LEFTDOWN = 0x00000002,
            LEFTUP = 0x00000004,
            MIDDLEDOWN = 0x00000020,
            MIDDLEUP = 0x00000040,
            MOVE = 0x00000001,
            ABSOLUTE = 0x00008000,
            RIGHTDOWN = 0x00000008,
            RIGHTUP = 0x00000010
        }
    }
}
