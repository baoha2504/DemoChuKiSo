using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace DemoChuKiSo
{
    public partial class FormTXT : Form
    {
        public FormTXT()
        {
            InitializeComponent();
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { _message = value; }
        }
        private void FormTXT_Load(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(_message, FileMode.Open);
            StreamReader rd = new StreamReader(fs, Encoding.UTF8);
            gunaTextBox1.Text = rd.ReadToEnd();
            rd.Close();
        }
    }
}
