using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MouseSpotlight
{
    public partial class FormMain : Form
    {

        private FormDraw fd;//透明窗体不穿透鼠标
        private bool startdraw = false;//是否开始画图
        private Graphics gs;//画版
        private Pen pen;//画笔
        private Point startpt;//画图起点

        FormScreenshot fs;//截图

        private double scale = 1.0;

        public FormMain()
        {
            InitializeComponent();

            // 获取缩放比例
            scale = Win32Helper.GetScale();


            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;//本窗体最大化
            TransparencyKey = Color.Pink;//背景透明(鼠标穿透)
            DoubleBuffered = true;//双缓存处理
            BackColor = Color.Black;
            Opacity = 0.75d;
            TopMost = true;


            // 画笔
            fd = new FormDraw();//不穿透鼠标透明窗体
            //设置不穿透鼠标透明窗体画板鼠标事件为本显示画图窗体鼠标事件进行同步
            fd.MouseDown += FormMain_MouseDown;//鼠标按下事件
            fd.MouseMove += FormMain_MouseMove;//鼠标移动事件
            fd.MouseUp += FormMain_MouseUp;//鼠标弹起事件

            //不穿透鼠标透明窗体参数设置如下
            fd.WindowState = FormWindowState.Maximized;//最大化
            fd.Opacity = 0.1;//背景透明不穿透鼠标
            fd.TopMost = true;//让不穿透鼠标透明窗体画板为最上层
            gs = CreateGraphics();//创建窗体画板
            pen = new Pen(Color.Red, 5f);//画笔
            //fd.Show();//显示


            fs = new FormScreenshot();
            fs.TopMost = true;
            //fs.Show();
    }


        private KeyboardHook k_hook;

        private void FormMain_Load(object sender, EventArgs e)   
        {
            notifyIcon1.Icon = this.Icon;
            notifyIcon1.Text = this.Text;

            Point ms = Control.MousePosition;
            panelSpotlight.Left = ms.X - (panelSpotlight.Width / 2);
            panelSpotlight.Top = ms.Y - (panelSpotlight.Height / 2);
            panelSpotlight.BackColor = Color.Pink;//背景透明(鼠标穿透)
            // 读取默认参数
            panelSpotlight.Width = config.Default.Width;
            panelSpotlight.Height = config.Default.Height;

            // pictureBoxSpotlight.BackColor = Color.Pink;//背景透明(鼠标穿透)

            k_hook = new KeyboardHook();
            k_hook.KeyDownEvent += new System.Windows.Forms.KeyEventHandler(hook_KeyDown);//钩住键按下
            // k_hook.KeyPressEvent += K_hook_KeyPressEvent;
            k_hook.Start();//安装键盘钩子

            this.Hide();
            fd.Hide();
            fs.Hide();
        }

        private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            k_hook.Stop();
        }

        private void K_hook_KeyPressEvent(object sender, KeyPressEventArgs e)
        {
            int i = (int)e.KeyChar;
            textBoxLocation.Text = e.KeyChar.ToString();
        }

        private void hook_KeyDown(object sender, KeyEventArgs e)
        {
            

            textBoxKey.Text = (e.KeyData).ToString();

            if ((e.KeyData == Keys.Apps) || (e.KeyData == Keys.Escape))
            {
                // apps 或 esx - 退出
                if (this.Visible)
                {
                    this.Hide();
                    fd.Hide();
                    fs.Hide();
                }
                else
                {
                    Point ms = Control.MousePosition;
                    panelSpotlight.Left = ms.X - (panelSpotlight.Width / 2);
                    panelSpotlight.Top = ms.Y - (panelSpotlight.Height / 2);

                    this.Show();
                    fd.Show();
                }
            }
            else if (e.KeyData == Keys.BrowserHome)
            {
                if (this.Visible == false)
                {
                    this.Close();
                }
            }
            else if (this.Visible == false)
            {
                // return;
            }

            // 热键，打开聚光灯模式
            else if (e.KeyData == Keys.Up)
            {
                // 上
                panelSpotlight.Height = panelSpotlight.Height - 10;
                // 保存默认参数
                config.Default.Height = panelSpotlight.Height;
                config.Default.Save();
            }
            else if (e.KeyData == Keys.Down)
            {
                // 下
                panelSpotlight.Height = panelSpotlight.Height + 10;
                // 保存默认参数
                config.Default.Height = panelSpotlight.Height;
                config.Default.Save();
            }
            else if (e.KeyData == Keys.Left)
            {
                // 左
                panelSpotlight.Width = panelSpotlight.Width - 10;
                // 保存默认参数
                config.Default.Width = panelSpotlight.Width;
                config.Default.Save();
            }
            else if (e.KeyData == Keys.Right)
            {
                // 右
                panelSpotlight.Width = panelSpotlight.Width + 10;
                // 保存默认参数
                config.Default.Width = panelSpotlight.Width;
                config.Default.Save();
            }

            else if (e.KeyData == Keys.VolumeDown)
            {
                // 音量减 - 截图
                int imgWidth = (int)(panelSpotlight.Width * this.scale);        //放大后图片的宽度
                int imgHeight = (int)(panelSpotlight.Height * this.scale);      //放大后图片的高度

                // 截取当前位置的缩略图　        
                Bitmap bit = new Bitmap(imgWidth, imgHeight);
                Graphics g = Graphics.FromImage(bit);
                g.CopyFromScreen((int)(panelSpotlight.Left * this.scale), (int)(panelSpotlight.Top * this.scale), 0, 0, bit.Size);
                bit.Save(@"screen.png");
                IntPtr Hdc = g.GetHdc();
                g.ReleaseHdc(Hdc);

                fs.Hide();
            }
            else if (e.KeyData == Keys.VolumeUp)
            {
                // 音量加 - 显示预设的截图
                Point ms = Control.MousePosition;
                int Left = ms.X - (panelSpotlight.Width / 2);
                int Top = ms.Y - (panelSpotlight.Height / 2);
                int magnetic = 50;

                if ((Left < panelSpotlight.Left + magnetic) && (Left > panelSpotlight.Left - magnetic))
                {
                    Left = panelSpotlight.Left;
                }
                if ((Top < panelSpotlight.Top + magnetic) && (Top > panelSpotlight.Top - magnetic))
                {
                    Top = panelSpotlight.Top;
                }

                fs.LoadImage(@"screen.png", Left, Top);
            }

            //             
            else
            {

            }

            if (this.Visible == true)
            {
                // 拦截
                e.SuppressKeyPress = true;
            }
        }

        private void FormMain_MouseDown(object sender, MouseEventArgs e)
        {
            if (this.Visible == false)
            {
                return;
            }

            if (e.Button == MouseButtons.Right)
            {
                startdraw = true;//开始画图
                startpt = e.Location;
            }
            else if (e.Button == MouseButtons.Left)
            {
                Point ms = Control.MousePosition;
                panelSpotlight.Left = ms.X - (panelSpotlight.Width / 2);
                panelSpotlight.Top = ms.Y - (panelSpotlight.Height / 2);


                //this.Show();
                //fs.Show();
            }
        }

        private void FormMain_MouseMove(object sender, MouseEventArgs e)
        {
            textBoxLocation.Text = string.Format("[{0:#####},{1:#####}]", e.Location.X * this.scale, e.Location.Y * this.scale);

            if (startdraw)
            {
                gs.DrawLine(pen, startpt, e.Location);
                startpt = e.Location;
            }
        }

        private void FormMain_MouseUp(object sender, MouseEventArgs e)
        {
            startdraw = false;//结束画图
        }

        private void contextMenuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            this.Close();
        }

        private void notifyIcon1_Click(object sender, EventArgs e)
        {

        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {

        }

    }
}
