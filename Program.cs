using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseSpotlight
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isNotRunning;  //互斥体判断
            System.Threading.Mutex instance = new System.Threading.Mutex(true, Application.ProductName, out isNotRunning);   //同步基元变量
            if (!isNotRunning)  // 如果不是未运行状态
            {
                MessageBox.Show("程序已在运行");
                Environment.Exit(1);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormMain());
        }
    }
}
