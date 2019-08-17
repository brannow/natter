using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace FBO
{
    public partial class CropWindow : Form
    {
        private Main del;
        private const int defaultHeight = 500;
        private const int WM_SIZING = 0x214;
        private const int WMSZ_LEFT = 1;
        private const int WMSZ_RIGHT = 2;
        private const int WMSZ_TOP = 3;
        private const int WMSZ_BOTTOM = 6;
        private const int WM_NCHITTEST = 0x84;
        private const int WS_EX_TOOLWINDOW = 0x00000080;
        private const int cGrip = 16;      // Grip size

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_NCHITTEST)
            {  // Trap WM_NCHITTEST
                Point pos = new Point(m.LParam.ToInt32());
                pos = this.PointToClient(pos);
                if (pos.X >= this.ClientSize.Width - cGrip && pos.Y >= this.ClientSize.Height - cGrip)
                {
                    m.Result = (IntPtr)17; // HTBOTTOMRIGHT
                    return;
                }

                base.WndProc(ref m);
                if ((int)m.Result == 0x1)
                    m.Result = (IntPtr)0x2;
                return;
            }

            if (m.Msg == WM_SIZING)
            {
                this.Invalidate();
            }

            base.WndProc(ref m);
        }

        public CropWindow(Main del)
        {
            this.del = del;
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // border
            // optional ?
            Pen p = new Pen(Color.FromArgb(40, 40, 40), 4);
            e.Graphics.DrawRectangle(p, new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height));
            p.Dispose();

            e.Graphics.Dispose();
        }

        private void LockButton_Click(object sender, EventArgs e)
        {
            this.del.TransferCropArea(this);
        }
    }
}
