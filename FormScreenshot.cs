using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MouseSpotlight
{
    public partial class FormScreenshot : Form
    {
        public FormScreenshot()
        {
            InitializeComponent();
        }

        private void FormScreenshot_Load(object sender, EventArgs e)
        {

        }


        public void LoadImage(string path, int Left, int Top)
        {
            if (System.IO.File.Exists(path))
            {
                using (FileStream lStream = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    Image img = Image.FromStream(lStream);
                    pictureBox1.Image = img;
                }

                this.Left = Left;
                this.Top = Top;
                this.TopMost = true;
                this.Show();
            }
        }


    }
}
