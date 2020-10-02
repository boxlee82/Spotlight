using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseSpotlight
{
    class Win32Helper
    {

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern IntPtr GetDesktopWindow();

        /// <summary>
        /// 该函数检索一指定窗口的客户区域或整个屏幕的显示设备上下文环境的句柄，
        /// 以后可以在GDI函数中使用该句柄来在设备上下文环境中绘图。
        /// </summary>
        /// <param name="hWnd">设备上下文环境被检索的窗口的句柄，如果该值为NULL，GetDC则检索整个屏幕的设备上下文环境。</param>
        /// <returns>如果成功，返回指定窗口客户区的设备上下文环境；如果失败，返回值为Null。</returns>
        [DllImport("user32")]
        public static extern IntPtr GetDC(IntPtr hWnd);
        /// <summary>
        /// 该函数释放设备上下文环境（DC）供其他应用程序使用。函数的效果与设备上下文环境类型有关。
        /// 它只释放公用的和设备上下文环境，对于类或私有的则无效。
        /// </summary>
        /// <param name="hWnd">指向要释放的设备上下文环境所在的窗口的句柄。</param>
        /// <param name="hDC">指向要释放的设备上下文环境的句柄。</param>
        /// <returns>如果释放成功，则返回值为1；如果没有释放成功，则返回值为0。</returns>
        [DllImport("user32")]
        public static extern int ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32")]
        public static extern bool GetCursorPos(out System.Drawing.Point pt);
        /// <summary>
        /// 该函数检索指定坐标点的像素的RGB颜色值。
        /// </summary>
        /// <param name="hDC">设备环境句柄。</param>
        /// <param name="nXPos">指定要检查的像素点的逻辑X轴坐标。</param>
        /// <param name="nYPos">指定要检查的像素点的逻辑Y轴坐标。</param>
        /// <returns>返回值是该象像点的RGB值。如果指定的像素点在当前剪辑区之外；那么返回值是CLR_INVALID。</returns>
        [DllImport("gdi32")]
        public static extern uint GetPixel(IntPtr hDC, int nXPos, int nYPos);
        [DllImport("gdi32")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        public const int HORZRES = 8;
        public const int VERTRES = 10;
        public const int DESKTOPVERTRES = 117;
        public const int DESKTOPHORZRES = 118;
        /// <summary>
        /// 获取当前鼠标位置颜色
        /// </summary>
        /// <returns></returns>
        public static System.Drawing.Color GetPixelColor()
        {
            Point pnt = new Point(0, 0);
            IntPtr hdc = GetDC(IntPtr.Zero);
            GetCursorPos(out pnt);

            float ScaleX = (float)GetDeviceCaps(hdc, DESKTOPHORZRES) / (float)GetDeviceCaps(hdc, HORZRES);
            float ScaleY = (float)GetDeviceCaps(hdc, DESKTOPVERTRES) / (float)GetDeviceCaps(hdc, VERTRES);
            uint pixel = GetPixel(hdc, (int)(pnt.X * ScaleX), (int)(pnt.Y * ScaleY));
            ReleaseDC(IntPtr.Zero, hdc);
            Color color = Color.FromArgb((int)(pixel & 0x000000FF),
            (int)(pixel & 0x0000FF00) >> 8,
            (int)(pixel & 0x00FF0000) >> 16);
            return color;
        }


        /// <summary>
        /// 获取屏幕缩放比例
        /// </summary>
        /// <returns></returns>
        public static double GetScale()
        {
            double Scale = 1.0;

            IntPtr DesktopHwnd = Win32Helper.GetDesktopWindow();
            using (Graphics DesktopGr = Graphics.FromHwnd(DesktopHwnd))
            {
                IntPtr DesktopHdc = DesktopGr.GetHdc();
                int XRes = Win32Helper.GetDeviceCaps(DesktopHdc, (int)Win32Helper.DESKTOPHORZRES);
                int YRes = Win32Helper.GetDeviceCaps(DesktopHdc, (int)Win32Helper.DESKTOPVERTRES);

                Scale = (double)XRes / (double)Screen.PrimaryScreen.Bounds.Width;
            }

            return Scale;
        }
    }

}
