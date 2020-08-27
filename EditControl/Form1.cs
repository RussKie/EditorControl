using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using static EditControl.Interop;

namespace EditControl
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            timer1.Start();
            base.OnShown(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            UpdateData();
        }

        private void UpdateData()
        {
            Data data = new();

            data.LineCount = (int)SendMessageW(textBox1.Handle, EM_GETLINECOUNT);
            data.FirstVisibleLine = (int)SendMessageW(textBox1.Handle, EM_GETFIRSTVISIBLELINE);

            RECT formatRect = default;
            SendMessageW(textBox1.Handle, EM_GETRECT, default, ref formatRect);
            data.FormattingRect = formatRect;
            data.ControlRect = textBox1.Bounds;
            Rectangle clientRect = textBox1.ClientRectangle;
            data.ControlClientRect = clientRect;

            Point upperLeft = new Point(formatRect.X + 1, formatRect.Y + 1);
            data.UpperLeftChar = GetCharInfo(upperLeft);

            Point upperRight = new Point(formatRect.right - 1, formatRect.Y + 1);
            data.UpperRightChar = GetCharInfo(upperRight);

            Point lowerRight = new Point(formatRect.right - 1, formatRect.bottom - 1);
            data.LowerRightChar = GetCharInfo(lowerRight);

            Point lowerLeft = new Point(formatRect.X + 1, formatRect.bottom - 1);
            data.LowerLeftChar = GetCharInfo(lowerLeft);

            //while (char.IsControl(c) && index > 0)
            //{
            //    index--;
            //    c = textBox1.Text[index];
            //}

            var c = textBox1.GetCharFromPosition(lowerLeft);
            var result = SendMessageW(textBox1.Handle, EM_CHARFROMPOS, default, PARAM.FromLowHigh(lowerLeft.X, lowerLeft.Y));
            textBoxWalkback.Text = $"0x{(int)c:x2} '{c}' LowerLeft [Line {PARAM.HIWORD(result)} Index {PARAM.LOWORD(result)}]";


            IntPtr dc = GetDC(textBox1.Handle);
            TEXTMETRICW tm = default;
            GetTextMetricsW(dc, ref tm);
            ReleaseDC(textBox1.Handle, dc);
            data.FontHeight = tm.tmHeight;

            propertyGrid1.SelectedObject = data;

            return;

            string GetCharInfo(Point point)
            {
                char ch = textBox1.GetCharFromPosition(point);
                IntPtr result = SendMessageW(textBox1.Handle, EM_CHARFROMPOS, default, PARAM.FromLowHigh(point.X, point.Y));
                int index = textBox1.GetCharIndexFromPosition(point);
                return $"0x{(int)ch:x2} '{ch}' ({index}) Line {PARAM.HIWORD(result)} Index {PARAM.LOWORD(result)}";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateData();
        }
    }

    public struct Data
    {
        private const string Bounds = "Bounds";
        private const string Characters = "Characters";
        private const string Font = "Font";
        private const string Lines = "Lines";

        [Category(Lines)]
        [ReadOnly(true)]
        public int LineCount { get; set; }

        [Category(Lines)]
        [ReadOnly(true)]
        public int FirstVisibleLine { get; set; }

        [Category(Bounds)]
        [ReadOnly(true)]
        public RECT FormattingRect { get; set; }

        [Category(Bounds)]
        [ReadOnly(true)]
        public RECT ControlRect { get; set; }

        [Category(Bounds)]
        [ReadOnly(true)]
        public RECT ControlClientRect { get; set; }


        [Category(Characters)]
        [ReadOnly(true)]
        public string UpperLeftChar { get; set; }

        [Category(Characters)]
        [ReadOnly(true)]
        public string LowerLeftChar { get; set; }

        [Category(Characters)]
        [ReadOnly(true)]
        public string UpperRightChar { get; set; }

        [Category(Characters)]
        [ReadOnly(true)]
        public string LowerRightChar { get; set; }

        [Category(Font)]
        [ReadOnly(true)]
        public int FontHeight { get; set; }
    }
}
