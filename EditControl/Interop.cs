using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace EditControl
{
    public static class Interop
    {
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr SendMessageW(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam = default,
            IntPtr lParam = default);

        public unsafe static IntPtr SendMessageW<T>(
            IntPtr hWnd,
            uint Msg,
            IntPtr wParam,
            ref T lParam) where T : unmanaged
        {
            fixed (void* l = &lParam)
            {
                return SendMessageW(hWnd, Msg, wParam, (IntPtr)l);
            }
        }

        public const uint EM_GETSEL              = 0x00B0;
        public const uint EM_SETSEL              = 0x00B1;
        public const uint EM_GETRECT             = 0x00B2;
        public const uint EM_SETRECT             = 0x00B3;
        public const uint EM_SETRECTNP           = 0x00B4;
        public const uint EM_SCROLL              = 0x00B5;
        public const uint EM_LINESCROLL          = 0x00B6;
        public const uint EM_SCROLLCARET         = 0x00B7;
        public const uint EM_GETMODIFY           = 0x00B8;
        public const uint EM_SETMODIFY           = 0x00B9;
        public const uint EM_GETLINECOUNT        = 0x00BA;
        public const uint EM_LINEINDEX           = 0x00BB;
        public const uint EM_SETHANDLE           = 0x00BC;
        public const uint EM_GETHANDLE           = 0x00BD;
        public const uint EM_GETTHUMB            = 0x00BE;
        public const uint EM_LINELENGTH          = 0x00C1;
        public const uint EM_REPLACESEL          = 0x00C2;
        public const uint EM_GETLINE             = 0x00C4;
        public const uint EM_LIMITTEXT           = 0x00C5;
        public const uint EM_CANUNDO             = 0x00C6;
        public const uint EM_UNDO                = 0x00C7;
        public const uint EM_FMTLINES            = 0x00C8;
        public const uint EM_LINEFROMCHAR        = 0x00C9;
        public const uint EM_SETTABSTOPS         = 0x00CB;
        public const uint EM_SETPASSWORDCHAR     = 0x00CC;
        public const uint EM_EMPTYUNDOBUFFER     = 0x00CD;
        public const uint EM_GETFIRSTVISIBLELINE = 0x00CE;
        public const uint EM_SETREADONLY         = 0x00CF;
        public const uint EM_SETWORDBREAKPROC    = 0x00D0;
        public const uint EM_GETWORDBREAKPROC    = 0x00D1;
        public const uint EM_GETPASSWORDCHAR     = 0x00D2;
        public const uint EM_SETMARGINS          = 0x00D3;
        public const uint EM_GETMARGINS          = 0x00D4;
        public const uint EM_SETLIMITTEXT        = EM_LIMITTEXT;
        public const uint EM_GETLIMITTEXT        = 0x00D5;
        public const uint EM_POSFROMCHAR         = 0x00D6;
        public const uint EM_CHARFROMPOS         = 0x00D7;
        public const uint EM_SETIMESTATUS        = 0x00D8;
        public const uint EM_GETIMESTATUS        = 0x00D9;
        public const uint EM_ENABLEFEATURE       = 0x00DA;

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;

            public RECT(int left, int top, int right, int bottom)
            {
                this.left = left;
                this.top = top;
                this.right = right;
                this.bottom = bottom;
            }

            public RECT(Rectangle r)
            {
                left = r.Left;
                top = r.Top;
                right = r.Right;
                bottom = r.Bottom;
            }

            public static implicit operator Rectangle(RECT r) => Rectangle.FromLTRB(r.left, r.top, r.right, r.bottom);
            public static implicit operator RECT(Rectangle r) => new RECT(r);
            public int X => left;
            public int Y => top;
            public int Width => right - left;
            public int Height => bottom - top;
            public Size Size => new Size(Width, Height);
            public override string ToString() => $"{{{left}, {top}, {right}, {bottom} (LTRB)}}";
        }

        internal static class PARAM
        {
            public static IntPtr FromLowHigh(int low, int high) => (IntPtr)ToInt(low, high);
            public static IntPtr FromLowHighUnsigned(int low, int high) => (IntPtr)ToInt(low, high);
            public static int ToInt(int low, int high) => (high << 16) | (low & 0xffff);
            public static int HIWORD(int n) => (n >> 16) & 0xffff;
            public static int LOWORD(int n) => n & 0xffff;
            public static int LOWORD(IntPtr n) => LOWORD((int)(long)n);
            public static int HIWORD(IntPtr n) => HIWORD((int)(long)n);
            public static int SignedHIWORD(IntPtr n) => SignedHIWORD((int)(long)n);
            public static int SignedLOWORD(IntPtr n) => SignedLOWORD((int)(long)n);
            public static int SignedHIWORD(int n) => (short)HIWORD(n);
            public static int SignedLOWORD(int n) => (short)LOWORD(n);
            public static IntPtr FromColor(Color color) => (IntPtr)ColorTranslator.ToWin32(color);
            public static int ToInt(IntPtr param) => (int)(long)param;
            public static uint ToUInt(IntPtr param) => (uint)(long)param;
            public static long ToLong(IntPtr param) => (long)param;
            public static ulong ToULong(IntPtr param) => (ulong)(long)param;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct TEXTMETRICW
        {
            public int tmHeight;
            public int tmAscent;
            public int tmDescent;
            public int tmInternalLeading;
            public int tmExternalLeading;
            public int tmAveCharWidth;
            public int tmMaxCharWidth;
            public int tmWeight;
            public int tmOverhang;
            public int tmDigitizedAspectX;
            public int tmDigitizedAspectY;
            public char tmFirstChar;
            public char tmLastChar;
            public char tmDefaultChar;
            public char tmBreakChar;
            public byte tmItalic;
            public byte tmUnderlined;
            public byte tmStruckOut;
            public TMPF tmPitchAndFamily;
            public byte tmCharSet;
        }

        [Flags]
        public enum TMPF : byte
        {
            FIXED_PITCH = 0x01,
            VECTOR = 0x02,
            TRUETYPE = 0x04,
            DEVICE = 0x08,
        }

        [DllImport("gdi32.dll", ExactSpelling = true, CharSet = CharSet.Unicode)]
        public static extern int GetTextMetricsW(IntPtr hdc, ref TEXTMETRICW lptm);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
    }
}
