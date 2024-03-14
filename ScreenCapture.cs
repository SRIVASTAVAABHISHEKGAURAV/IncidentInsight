using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace IncidentInsight
{
    public static class ScreenCapture
    {
        [DllImport("user32.dll")]
        private static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindowDC(IntPtr ptr);

        [DllImport("user32.dll")]
        private static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDc);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll")]
        private static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);

        [DllImport("gdi32.dll")]
        private static extern IntPtr SelectObject(IntPtr hdc, IntPtr hObject);

        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int wDest, int hDest, IntPtr hdcSrc, int xSrc, int ySrc, CopyPixelOperation rop);

        [DllImport("gdi32.dll")]
        private static extern int DeleteDC(IntPtr hdc);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        public enum CopyPixelOperation : int
        {
            SRCCOPY = 0x00CC0020
        }

        public static void CaptureScreen(string filePath)
        {
            IntPtr desktopWindowHandle = GetDesktopWindow();
            IntPtr desktopDeviceContext = GetWindowDC(desktopWindowHandle);

            RECT desktopRect;
            GetWindowRect(desktopWindowHandle, out desktopRect);
            int width = desktopRect.Right - desktopRect.Left;
            int height = desktopRect.Bottom - desktopRect.Top;

            IntPtr compatibleDeviceContext = CreateCompatibleDC(desktopDeviceContext);
            IntPtr compatibleBitmapHandle = CreateCompatibleBitmap(desktopDeviceContext, width, height);

            IntPtr previousBitmapHandle = SelectObject(compatibleDeviceContext, compatibleBitmapHandle);
            BitBlt(compatibleDeviceContext, 0, 0, width, height, desktopDeviceContext, 0, 0, CopyPixelOperation.SRCCOPY);

            Bitmap bitmap = Bitmap.FromHbitmap(compatibleBitmapHandle);
            SelectObject(compatibleDeviceContext, previousBitmapHandle);
            DeleteDC(compatibleDeviceContext);
            ReleaseDC(desktopWindowHandle, desktopDeviceContext);

            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
            bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
