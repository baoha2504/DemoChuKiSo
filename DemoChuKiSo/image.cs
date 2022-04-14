using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DemoChuKiSo
{
    public partial class FormImage : Form
    {
        public FormImage()
        {
            InitializeComponent();
        }
        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
        private void FormImage_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = Image.FromFile(_message);
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
        }
    }
}
