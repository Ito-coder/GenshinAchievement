using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;

namespace WindowCapture
{
    public class WindowCapture
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        public static Bitmap? Capture(string windowTitle, int left, int top, int width, int height)
        {
            var proc = Process.GetProcesses().ToList().FirstOrDefault(a => a.MainWindowTitle == windowTitle);

            if (proc == null) return null;

            GetWindowRect(proc.MainWindowHandle, out var rect);
            int rectWidth = Math.Min(width, rect.right - rect.left - left);
            int rectHeight = Math.Min(height, rect.bottom - rect.top - top);

            Bitmap bmp = new(rectWidth, rectHeight);

            Graphics newGraphics = Graphics.FromImage(bmp);
            newGraphics.CopyFromScreen(new Point(rect.left + left, rect.top + top), new Point(0, 0), bmp.Size);
            newGraphics.Dispose();

            return bmp;
        }
        public static List<string> WindowTitleList()
        {
            return Process.GetProcesses().ToList().Select(a => a.MainWindowTitle).ToList();
        }
    }
}
