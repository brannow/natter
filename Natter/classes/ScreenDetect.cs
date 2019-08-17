using System;
using System.Drawing;
using System.Windows.Forms;

namespace FBO.Classes
{ 
    class ScreenDetect
    {
        private static Rectangle screenCrop;

        private const double swimmerRatioW = 0.041; // 60 / 1440
        private const double swimmerRatioH = 0.0625; // 90 / 1440

        private static Point SwimmerRelocator; 

        private static readonly Random rnd = new Random();
        private static Size swimmerSize;
        private static Size swimmerSizeHalf;

        private static Int64 swimmerTotalSampelsCount = 0;
        private static double swimmerTotal = 0;
        private static double highestDelta = 0;
        protected static DateTime LastPreviewTime;
        protected static TimeSpan PreviewInverval = TimeSpan.FromMilliseconds(500);

        public static bool Stop = false;

        public static void InitDetector()
        {
            int w = (int)(Screen.PrimaryScreen.Bounds.Width * 0.2);
            int h = (int)(Screen.PrimaryScreen.Bounds.Height * 0.15);
            int x = (int)(Screen.PrimaryScreen.Bounds.Width / 2 - w / 2);
            int y = (int)(Screen.PrimaryScreen.Bounds.Height / 2 - h  /2);

            int simmerH = Convert.ToInt32(Math.Ceiling(Screen.PrimaryScreen.Bounds.Height * swimmerRatioH));
            int simmerW = Convert.ToInt32(Math.Ceiling(Screen.PrimaryScreen.Bounds.Height * swimmerRatioW));
            swimmerSize = new Size(simmerH, simmerW);
            swimmerSizeHalf = new Size(swimmerSize.Width / 2, swimmerSize.Height / 2);

            int sx = Convert.ToInt32(swimmerSizeHalf.Width / 3); //- swimmerSizeHalf.Width;
            int sy = swimmerSize.Height - Convert.ToInt32(swimmerSize.Height / 8);// - swimmerSizeHalf.Height;
            SwimmerRelocator = new Point(sx, sy);

            SetCustomCrop(new Rectangle(x, y, w, h));
        }

        public static void SetCustomCrop(Rectangle customCrop)
        {
            screenCrop = customCrop;
        }

        public static Rectangle GetCustomCrop()
        {
            return screenCrop;
        }

        public static void MoveCursorAway()
        {
            MouseControl.Move(new Point(Cursor.Position.X + rnd.Next(100, 300), Cursor.Position.Y + rnd.Next(50, 150)));
        }

        /// <summary>
        /// c = mouse off color
        /// mc = mouse over color
        /// </summary>
        /// <param name="c"></param>
        /// <param name="mc"></param>
        /// <param name="sensitivity"></param>
        /// <returns></returns>
        public static Point FindColorOnScreen(Color c, Color mc, int sensitivity)
        {
            Bitmap b = CaptureScreen();
            Point p = SpiralSearch(b, c, mc, sensitivity);
            b.Dispose();
            return p;
        }

        public static bool DetectSpash(Point location, TimeSpan remainingTime, double threshold, Main del)
        {
            var startTime = DateTime.UtcNow;
            swimmerTotalSampelsCount = 0;
            swimmerTotal = 0;
            highestDelta = 0;
            while (DateTime.UtcNow - startTime < remainingTime)
            {
                if (Stop)
                {
                    return false;
                }

                if (ScreenDetect.SearchForSpalsh(location, threshold, del))
                {
                    return true;
                }
            }

            return false;
        }

        protected static bool SearchForSpalsh(Point location, double threshold, Main del)
        {
            Bitmap b = CaptureSwimmer(location);
            if (DateTime.UtcNow - LastPreviewTime > PreviewInverval)
            {
                LastPreviewTime = DateTime.UtcNow;
                del.UpdatePreviewImage(b);
            }
            
            double tone = CalulateImageColorTone(b);
            b.Dispose();
            if (swimmerTotalSampelsCount > 150)
            {
                double avg = swimmerTotal / swimmerTotalSampelsCount;
                double delta = Math.Abs(tone - avg);

                if (highestDelta < delta)
                {
                    highestDelta = delta;
                    del.UpdateConfidence(highestDelta);
                }

                if (delta > threshold)
                {
                    del.UpdateConfidence(delta);
                    return true;
                }
            }

            swimmerTotalSampelsCount++;
            swimmerTotal += tone;

            return false;
        }

        protected static Bitmap CaptureSwimmer(Point location)
        {
            //Bitmap Buffer = new Bitmap(swimmerSize.Width, swimmerSizeHalf.Height);
            Bitmap Buffer = new Bitmap(swimmerSize.Width, swimmerSize.Height);
            Graphics Context = Graphics.FromImage(Buffer);
            int sx = location.X - SwimmerRelocator.X;
            int sy = location.Y - SwimmerRelocator.Y;
            Context.CopyFromScreen(sx, sy, 0, 0, Buffer.Size);
            Context.Dispose();
            return Buffer;
        }

        protected static double CalulateImageColorTone(Bitmap bitmap)
        {
            int r = 0,
                g = 0,
                b = 0,
                total = 0;
            for (int y = 0; y < bitmap.Size.Height; y++)
            {
                for (int x = 0; x < bitmap.Size.Width; x++)
                {
                    Color c = bitmap.GetPixel(x, y);
                    r += (int)c.R;
                    g += (int)c.G;
                    b += (int)c.B;
                    total++;
                }
            }

            r = r / total;
            g = g / total;
            b = b / total;

            return Math.Sqrt(r + g + b);
        }

        protected static Bitmap CaptureScreen()
        {
            Bitmap Buffer = new Bitmap(screenCrop.Size.Width, screenCrop.Size.Height);
            Graphics Context = Graphics.FromImage(Buffer);
            Context.CopyFromScreen(screenCrop.Left, screenCrop.Top, 0, 0, Buffer.Size);
            Context.Dispose();
            return Buffer;
        }

        protected static Point SpiralSearch(Bitmap btm, Color matchColor, Color matchColor2, int sensitivity)
        {
            int X = btm.Size.Width;
            int Y = btm.Size.Height;
            if (X % 2 != 0)
                X -= 1;
            if (Y % 2 != 0)
                Y -= 1;

            int XHalf = X / 2;
            int YHalf = Y / 2;
            // we just dont want a match as default so sensitivity + 1 != (p1 < sensitivity)
            double p = sensitivity + 1, p2 = sensitivity + 1;

            bool isNotWhite = matchColor2.Name != "White";

            Color c;
            int x, y, dx, dy, realX, realY;
            x = y = dx = 0;
            dy = -1;
            int t = Math.Max(X, Y);
            int maxI = t * t;
            int skipCount = 0;
            for (int i = 0; i < maxI; i++)
            {
                if ((-X / 2 <= x) && (x <= X / 2) && (-Y / 2 <= y) && (y <= Y / 2))
                {
                    realX = x + XHalf;
                    realY = y + YHalf;

                    if (realX >= 0 && realY >= 0 && realX < X && realY < Y)
                    {
                        skipCount++;
                        if (skipCount % 2 == 0)
                            continue;

                        c = btm.GetPixel(realX, realY);
                        p = CheckColor(c, matchColor);
                        if (isNotWhite)
                        {
                            p2 = CheckColor(c, matchColor2);
                        }
                        if (p < sensitivity || p2 < sensitivity)
                        {
                            return new Point(screenCrop.Left + realX, screenCrop.Top + realY);
                        }
                    }
                }
                if ((x == y) || ((x < 0) && (x == -y)) || ((x > 0) && (x == 1 - y)))
                {
                    t = dx;
                    dx = -dy;
                    dy = t;
                }
                x += dx;
                y += dy;
            }

            return new Point(-1, -1);
        }

        protected static double CheckColor(Color c, Color sampleColor)
        {
            double dbl_input_red = Convert.ToDouble(c.R);
            double dbl_input_green = Convert.ToDouble(c.G);
            double dbl_input_blue = Convert.ToDouble(c.B);

            double dbl_test_red, dbl_test_green, dbl_test_blue;

            dbl_test_red = Math.Pow(Convert.ToDouble(sampleColor.R) - dbl_input_red, 2.0);
            dbl_test_green = Math.Pow(Convert.ToDouble
                (sampleColor.G) - dbl_input_green, 2.0);
            dbl_test_blue = Math.Pow(Convert.ToDouble
                (sampleColor.B) - dbl_input_blue, 2.0);

            return Math.Sqrt(dbl_test_blue + dbl_test_green + dbl_test_red);
        }
    }
}
