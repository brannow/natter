using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using FBO.Classes;

namespace FBO
{
    public partial class Main : Form
    {
        private static readonly Random rnd = new Random();

        private static int failoverCount = 0;

        // DLL libraries used to manage hotkeys
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        const string KERNEL32 = "Kernel32.dll";

        [DllImport(KERNEL32)]
        public extern static void CopyMemory(IntPtr dest, IntPtr src, uint length);

        const int MYACTION_HOTKEY_ID_START = 5519881;
        const int MYACTION_HOTKEY_ID_END = 5519882;
        const int MYACTION_HOTKEY_ID_ENDSHIFT = 5519883;

        private static int Catches = 0;
        private static int Misses = 0;

        public struct Colors
        {
            public Color mouseOut;
            public Color mouseOver;
        }

        private int sen = 10;
        private int fishingTime = 30;
        private int bubblerFadeInTime = 2300;
        private double spalshThreshold = 0.2;
        private Dictionary<String, Colors> colors;
        private Colors searchColor;
        private static DateTime FishingStarted;

        protected static Thread fboThread = null;

        private enum ProcessDPIAwareness
        {
            ProcessDPIUnaware = 0,
            ProcessSystemDPIAware = 1,
            ProcessPerMonitorDPIAware = 2
        }

        [DllImport("shcore.dll")]
        private static extern int SetProcessDpiAwareness(ProcessDPIAwareness value);

        private static void SetDpiAwareness()
        {
            try
            {
                if (Environment.OSVersion.Version.Major >= 6)
                {
                    SetProcessDpiAwareness(ProcessDPIAwareness.ProcessPerMonitorDPIAware);
                }
            }
            catch (EntryPointNotFoundException)//this exception occures if OS does not implement this API, just ignore it.
            {
            }
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == 0x0312)
            {
                if (m.WParam.ToInt32() == MYACTION_HOTKEY_ID_START)
                {
                    Debug.WriteLine("HK PRESSED START");
                    StartFBO();

                } else if (m.WParam.ToInt32() == MYACTION_HOTKEY_ID_END || m.WParam.ToInt32() == MYACTION_HOTKEY_ID_ENDSHIFT)
                {
                    Debug.WriteLine("HK PRESSED END");
                    StopFBO();
                }
            }
            base.WndProc(ref m);
        }

        public Main()
        {
            // Modifier keys codes: Alt = 1, Ctrl = 2, Shift = 4, Win = 8
            // Compute the addition of each combination of the keys you want to be pressed
            // ALT+CTRL = 1 + 2 = 3 , CTRL+SHIFT = 2 + 4 = 6...

            // CTRL + F7 = START
            // F7 = END (also SHIFT + F7)
            RegisterHotKey(this.Handle, MYACTION_HOTKEY_ID_START, 2, (int)Keys.F7);
            RegisterHotKey(this.Handle, MYACTION_HOTKEY_ID_END, 0, (int)Keys.F7);
            RegisterHotKey(this.Handle, MYACTION_HOTKEY_ID_ENDSHIFT, 4, (int)Keys.F7);
            InitializeComponent();
            this.TopMost = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            IniFile.Init("config.ini");
            LoadIniFile();
            SetDpiAwareness();
            ScreenDetect.InitDetector();
        }

        private void LoadIniFile()
        {
            /*
[Color]
# two different color samples can be placed here (best practice mouse out and mouse over)
Moonglave=0,28,47;0,28,47
Durotar=22,14,30;22,14,30
[Setting]
# lower value = more pinpoint, but may not find bubbler (lower value = more strict)
Sensitivity=20
# lower value = more pinpoint, but may not notice event (lower value = more strict)
SplashThreshold=0.3
# entire fishing time
FishingTime=27
# milliseconds until bubbler is complete visible
BubblerFadeInTime=2300


             */
            if (!IniFile.ExistsIniFile())
            {
                IniFile.IniWriteValue("Color", "Moonglave", "0,28,47;0,28,47");
                IniFile.IniWriteValue("Color", "Durotar", "22,14,30;22,14,30");
                // bubbler color detection range (lower value is more strict)
                IniFile.IniWriteValue("Setting", "Sensitivity", "10");
                // number delta the splash make (lower this value will result in a more prune detection, many false positive)
                IniFile.IniWriteValue("Setting", "SplashThreshold", "0.55");
                // complete fishing time in seconds 
                IniFile.IniWriteValue("Setting", "FishingTime", "30");
                // in miliseconds, time until bubbler is complete visible so we can detect the color
                IniFile.IniWriteValue("Setting", "BubblerFadeInTime", "2300");
            }

            int debug = Convert.ToInt32(IniFile.IniReadValue("Setting", "debug", "0"));
            sen = Convert.ToInt32(IniFile.IniReadValue("Setting", "Sensitivity", "0"));
            spalshThreshold = Convert.ToDouble(IniFile.IniReadValue("Setting", "SplashThreshold", "0").Replace(',', '.'), CultureInfo.InvariantCulture);
            fishingTime = Convert.ToInt32(IniFile.IniReadValue("Setting", "FishingTime", "0"));
            bubblerFadeInTime = Convert.ToInt32(IniFile.IniReadValue("Setting", "BubblerFadeInTime", "0"));
            Dictionary<String, String> parsedColors = IniFile.ReadSection("Color");
            colors = new Dictionary<string, Colors>(parsedColors.Count);

            foreach (KeyValuePair<string, string> entry in parsedColors)
            {
                // do something with entry.Value or entry.Key
                String[] grp = entry.Value.Split(';');
                
                if (grp.Length > 0)
                {
                    Colors c = new Colors
                    {
                        mouseOut = ExtractColorFromString(grp[0])
                    };
                    if (grp.Length > 1)
                    {
                        c.mouseOver = ExtractColorFromString(grp[1]);
                    }
                    else
                    {
                        c.mouseOver = Color.White;
                    }
                    colors.Add(entry.Key, c);
                }
            }

            comboBox1.DataSource = new BindingSource(colors, null);

#if DEBUG
            PreviewImage.Show();
            PreviewImage.Visible = true;
            Confidence.Show();
            Confidence.Visible = true;
#else
            if (debug == 1) 
            {
                PreviewImage.Show();
                PreviewImage.Visible = true;
                Confidence.Show();
                Confidence.Visible = true;
            } 
            else 
            {
                PreviewImage.Hide();
                PreviewImage.Visible = false;
                Confidence.Hide();
                Confidence.Visible = false;
            }
#endif

        }

        protected Color ExtractColorFromString(String rgbString)
        {
            String[] col = rgbString.Split(',');
            int[] rgb = new int[col.Length];
            int rgbCount = 0;
            foreach (String c in col)
            {
                string cc = c.Trim();
                if (!cc.Equals(""))
                {
                    rgb[rgbCount] = Convert.ToInt32(cc);
                    rgbCount++;
                }
            }
            if (rgbCount == 3)
            {
                return Color.FromArgb(rgb[0], rgb[1], rgb[2]);
            }

            return Color.White;
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            StartFBO();
        }

        protected void StartFBO()
        {
            Process wowProcess = null;
            Process[] processes = Process.GetProcessesByName("WoW");
            if (processes.Length > 0)
            {
                wowProcess = processes[0];
            }
        
            // game not found but thread is running / cancel all!
            if (wowProcess == null)
            {
                if (fboThread != null)
                {
                    StopFBO();
                }
                SetStatusText("Game not found");
                return;
            }

            if (fboThread != null)
            {
                SetStatusText("IDLE");
                StopFBO();
            }
            else
            {
                SetForegroundWindow(wowProcess.MainWindowHandle);
                Thread.Sleep(1000);
                SetStatusText("START");
                Catches = 0;
                Misses = 0;
                FishingStarted = DateTime.UtcNow;
                UpdateStatus();
                UpdatePhishPerHour();
                fboThread = new Thread(ThreadFBOStart);
                fboThread.Start();
            }
        }

        public static Bitmap RemoveAlphaChannel(Bitmap bitmap)
        {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            Bitmap bitmapDest = (Bitmap)new Bitmap(bitmap.Width, bitmap.Height, PixelFormat.Format24bppRgb);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            BitmapData dataDest = bitmapDest.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            CopyMemory(dataDest.Scan0, data.Scan0, (uint)data.Stride * (uint)data.Height);
            bitmap.UnlockBits(data);
            bitmapDest.UnlockBits(dataDest);
            return bitmapDest;
        }

        private Bitmap previewCache = null;

        public void UpdatePreviewImage(Bitmap preview)
        {
            if (!PreviewImage.Visible)
            {
                return;
            }

            Bitmap copy = RemoveAlphaChannel(preview);
            if (PreviewImage.InvokeRequired)
            {
                PreviewImage.BeginInvoke((MethodInvoker)delegate ()
                {
                    if (previewCache!= null)
                    {
                        previewCache.Dispose();
                    }
                    
                    PreviewImage.Image = copy;
                    previewCache = copy;
                });
            }
            else
            {
                if (previewCache != null)
                {
                    previewCache.Dispose();
                }

                PreviewImage.Image = copy;
                previewCache = copy;
            }
        }

        public void UpdateConfidence(double delta)
        {
            if (!Confidence.Visible)
            {
                return;
            }

            if (Confidence.InvokeRequired)
            {
                Confidence.BeginInvoke((MethodInvoker)delegate () { Confidence.Text = delta.ToString(); });
            }
            else
            {
                Confidence.Text = delta.ToString();
            }
        }

        public void SetStatusText(string Text)
        {
#if DEBUG
            if (Status.InvokeRequired)
            {
                Status.BeginInvoke((MethodInvoker)delegate () { Console.WriteLine(DateTime.Now + " " + Text); });
            }
            else
            {
                Console.WriteLine(DateTime.Now + " " + Text);
            }
#endif

            if (Status.InvokeRequired)
            {
                Status.BeginInvoke((MethodInvoker)delegate () { Status.Text = Text; });
            }
            else
            {
                Status.Text = Text;
            }
        }

        public void UpdatePhishPerHour()
        {
            UInt32 fph = 0;
            if (Catches > 0)
            {
                TimeSpan diff = DateTime.UtcNow - FishingStarted;
                double offset = (diff.TotalSeconds / Convert.ToDouble(Catches));
                if (offset > 0)
                {
                    fph = Convert.ToUInt32(Math.Abs(3600 / offset));
                }
            }
            
            if (FishPerHourLabel.InvokeRequired)
            {
                FishPerHourLabel.BeginInvoke((MethodInvoker)delegate () { FishPerHourLabel.Text = fph.ToString() + " / h"; });
            }
            else
            {
                FishPerHourLabel.Text = fph.ToString() + " / h";
            }
        }

        public void UpdateStatus()
        {
            if (CatchLabel.InvokeRequired)
            {
                CatchLabel.BeginInvoke((MethodInvoker)delegate () { CatchLabel.Text = Catches.ToString(); });
            }
            else
            {
                CatchLabel.Text = Catches.ToString();
            }

            if (MissLabel.InvokeRequired)
            {
                MissLabel.BeginInvoke((MethodInvoker)delegate () { MissLabel.Text = Misses.ToString(); });
            }
            else
            {
                MissLabel.Text = Misses.ToString();
            }
        }

        protected static void StopFBO()
        {
            MouseControl.ReleaseShift();
            ScreenDetect.Stop = true;
            failoverCount = 0;
            if (fboThread != null)
            {
                fboThread.Abort();
            }
            fboThread = null;
        }

        protected void ThreadFBOStart()
        {
            if (searchColor.mouseOut.Name != "White")
            {
                UpdateConfidence(0);
                ScreenDetect.Stop = false;
                Thread.Sleep(rnd.Next(500, 1500));
                SetStatusText("LURE OUT");
                MouseControl.PressOne();
                DateTime startTime = DateTime.Now;
                Thread.Sleep(bubblerFadeInTime);

                SetStatusText("LOOK FOR COLOR");
                Point p = ScreenDetect.FindColorOnScreen(searchColor.mouseOut, searchColor.mouseOver, sen);
                if (p.X >= 0 && p.Y >= 0)
                {
                    SetStatusText("Color Found");
                    failoverCount = 0;
                    Thread.Sleep(rnd.Next(100, 1000));

                    // add randomness to cursor
                    Point rp = new Point
                    {
                        X = p.X + rnd.Next(-10, 10),
                        Y = p.Y + rnd.Next(-20, 20)
                    };
                    MouseControl.Move(rp);
                    SetStatusText("LOOK FOR SPLASH");
                    TimeSpan timeDiff = DateTime.Now - startTime;
                    TimeSpan remainingTime = TimeSpan.FromSeconds(fishingTime) - timeDiff;
                    if (ScreenDetect.DetectSpash(p, remainingTime, spalshThreshold, this))
                    {
                        SetStatusText("SPLASH FOUND");
                        Thread.Sleep(rnd.Next(400, 1000));
                        SetStatusText("CATCH");
                        Catches++;
                        CatchFish();
                        
                    }
                    else
                    {
                        Misses++;
                    }
                }
                else
                {
                    failoverCount++;
                }

                UpdateStatus();

                if (failoverCount > 3)
                {
                    StopFBO();
                    SetStatusText("IDLE");
                }
                else
                {
                    SetStatusText("RESTART");
                    ThreadFBOStart();
                }
            }
        }

        protected void CatchFish()
        {
            MouseControl.RandomWiggle();
            UpdatePhishPerHour();
            //MouseControl.HoldShift();
            MouseControl.RightClick();
            //MouseControl.ReleaseShift();
            Thread.Sleep(500);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            searchColor = (Colors)comboBox.SelectedValue;
        }

        private void ScanArea_Click(object sender, EventArgs e)
        {
            CropWindow c = new CropWindow(this)
            {
                Visible = true
            };

            Rectangle ca = ScreenDetect.GetCustomCrop();

            c.Size = new Size(ca.Size.Width, ca.Size.Height);
            c.Location = new Point(ca.Location.X, ca.Location.Y);
            c.Show();
        }

        public void TransferCropArea(CropWindow cw)
        {
            ScreenDetect.SetCustomCrop(cw.Bounds);
            cw.Close();
        }
    }
}
