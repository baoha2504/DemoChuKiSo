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
using System.Security.Cryptography;

namespace DemoChuKiSo
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        string filegui;
        string filechuki;
        string chuki;
        string filenhan;
        bool check = false;
        bool checkTaoKhoa = false;
        bool kixong = false;

        static string ComputeSha256Hash(string rawData)
        {
            // Create a SHA256   
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array  
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                // Convert byte array to a string   
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        private void btChonFileGui_Click(object sender, EventArgs e)
        {
            if (checkTaoKhoa == true)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    txtFileGui.Text = dlg.FileName;
                    FileStream fs = new FileStream(txtFileGui.Text, FileMode.Open);
                    StreamReader rd = new StreamReader(fs, Encoding.UTF8);
                    filegui = rd.ReadToEnd();
                    txtSHAGui.Text = ComputeSha256Hash(filegui);
                    rd.Close();
                }
            } 
            else
            {
                MessageBox.Show("Bạn chưa tạo khóa", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btChonFileNhan_Click(object sender, EventArgs e)
        {
            if (kixong == true)
            {

                if (checkTaoKhoa == true)
                {
                    OpenFileDialog dlg = new OpenFileDialog();
                    if (dlg.ShowDialog() == DialogResult.OK)
                    {
                        txtFileNhan.Text = dlg.FileName;
                        FileStream fs = new FileStream(txtFileNhan.Text, FileMode.Open);
                        StreamReader rd = new StreamReader(fs, Encoding.UTF8);
                        filenhan = rd.ReadToEnd();
                        txtSHANhan.Text = ComputeSha256Hash(filenhan);
                        rd.Close();
                    }
                }
                else
                {
                    MessageBox.Show("Bạn chưa tạo khóa", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Chưa kí xong!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        int RSA_soP, RSA_soQ, RSA_soN, RSA_soE, RSA_soD, RSA_soPhi_n;
        public int RSA_d_dau = 0;

        private int RSA_ChonSoNgauNhien()
        {
            Random rd = new Random();
            return rd.Next(11, 101);// tốc độ chậm nên chọn số bé
        }
        //"Hàm kiểm tra nguyên tố"
        private bool RSA_kiemTraNguyenTo(int xi)
        {
            bool kiemtra = true;
            if (xi == 2 || xi == 3)
            {
                // kiemtra = true;
                return kiemtra;
            }
            else
            {
                if (xi == 1 || xi % 2 == 0 || xi % 3 == 0)
                {
                    kiemtra = false;
                }
                else
                {
                    for (int i = 5; i <= Math.Sqrt(xi); i = i + 6)
                        if (xi % i == 0 || xi % (i + 2) == 0)
                        {
                            kiemtra = false;
                            break;
                        }
                }
            }
            return kiemtra;
        }
        // "Hàm kiểm tra hai số nguyên tố cùng nhau"
        private bool RSA_nguyenToCungNhau(int ai, int bi)
        {
            bool ktx_;
            // giải thuật Euclid;
            int temp;
            while (bi != 0)
            {
                temp = ai % bi;
                ai = bi;
                bi = temp;
            }
            if (ai == 1) { ktx_ = true; }
            else ktx_ = false;
            return ktx_;
        }
        private bool nguyenToCungNhau(int ai, int bi)// "Hàm kiểm tra hai số nguyên tố cùng nhau"
        {
            bool ktx_;
            // giải thuật Euclid;
            int temp;
            while (bi != 0)
            {
                temp = ai % bi;
                ai = bi;
                bi = temp;
            }
            if (ai == 1) { ktx_ = true; }
            else ktx_ = false;
            return ktx_;
        }

        // "Hàm lấy mod"
        public int RSA_mod(int mx, int ex, int nx)
        {

            //Sử dụng thuật toán "bình phương nhân"
            //Chuyển e sang hệ nhị phân
            int[] a = new int[100];
            int k = 0;
            do
            {
                a[k] = ex % 2;
                k++;
                ex = ex / 2;
            }
            while (ex != 0);
            //Quá trình lấy dư
            int kq = 1;
            for (int i = k - 1; i >= 0; i--)
            {
                kq = (kq * kq) % nx;
                if (a[i] == 1)
                    kq = (kq * mx) % nx;
            }
            return kq;
        }
        private void RSA_taoKhoa()
        {
            //Tinh n=p*q
            RSA_soN = RSA_soP * RSA_soQ;
            Key_CK_1.Text = RSA_soN.ToString();
            Key_BM_1.Text = RSA_soN.ToString();
            //Tính Phi(n)=(p-1)*(q-1)
            RSA_soPhi_n = (RSA_soP - 1) * (RSA_soQ - 1);
            txtE.Text = RSA_soPhi_n.ToString();
            //Tính e là một số ngẫu nhiên có giá trị 0< e <phi(n) và là số nguyên tố cùng nhau với Phi(n)
            do
            {
                Random RSA_rd = new Random();
                RSA_soE = RSA_rd.Next(2, RSA_soPhi_n);
            }
            while (!nguyenToCungNhau(RSA_soE, RSA_soPhi_n));
            txtE.Text = RSA_soE.ToString();
            Key_CK_2.Text = RSA_soE.ToString();
            //Tính d là nghịch đảo modular của e
            RSA_soD = 0;
            int i = 2;
            while (((1 + i * RSA_soPhi_n) % RSA_soE) != 0 || RSA_soD <= 0)
            {
                i++;
                RSA_soD = (1 + i * RSA_soPhi_n) / RSA_soE;
            }
            Key_BM_2.Text = RSA_soD.ToString();
        }

        private void btKiemTra_Click(object sender, EventArgs e)
        {
            if (txtFileNhan.Text != string.Empty)
            {
                OpenFileDialog dlg = new OpenFileDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    FileStream fs = new FileStream(dlg.FileName, FileMode.Open);
                    StreamReader rd = new StreamReader(fs, Encoding.UTF8);
                    chuki = rd.ReadToEnd();
                    rd.Close();
                }
                try
                {
                    RSA_GiaiMa(chuki);

                    if (txtSHANhan.Text == txtChuKiNhan.Text)
                    {
                        MessageBox.Show("Tài liệu được xác thực thành công", "Thông báo", MessageBoxButtons.OK);
                        check = true;
                    }
                    else
                    {
                        MessageBox.Show("Tài liệu sai hoặc bị đã bị sửa đổi", "Thông báo", MessageBoxButtons.OK);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            } else
            {
                MessageBox.Show("Chưa nhập tài liệu để kiểm tra", "Thông báo", MessageBoxButtons.OK);
            }
        }

        private void btMoFile_Click(object sender, EventArgs e)
        {
            if (check == true)
            {
                if (txtFileNhan.Text.Contains(".txt"))
                {
                    FormTXT form = new FormTXT();
                    form.Message = txtFileGui.Text;
                    form.ShowDialog();
                }
                else if (txtFileNhan.Text.Contains(".png") || txtFileNhan.Text.Contains(".jpg"))
                {
                    FormImage form = new FormImage();
                    form.Message = txtFileGui.Text;
                    form.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Không thể mở file!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Bạn chưa xác thực thành công!", "Thông báo", MessageBoxButtons.OK);
            }
        }

        public void RSA_MaHoa(string ChuoiVao) // hàm mã hóa
        {
            // taoKhoa();
            // Chuyen xau thanh ma Unicode
            byte[] mh_temp1 = Encoding.Unicode.GetBytes(ChuoiVao);
            string base64 = Convert.ToBase64String(mh_temp1);

            // Chuyen xau thanh ma Unicode
            int[] mh_temp2 = new int[base64.Length];
            for (int i = 0; i < base64.Length; i++)
            {
                mh_temp2[i] = (int)base64[i];
            }

            //Mảng a chứa các kí tự đã mã hóa
            int[] mh_temp3 = new int[mh_temp2.Length];
            for (int i = 0; i < mh_temp2.Length; i++)
            {
                mh_temp3[i] = RSA_mod(mh_temp2[i], RSA_soE, RSA_soN); // mã hóa
            }

            //Chuyển sang kiểu kí tự trong bảng mã Unicode
            string str = "";
            for (int i = 0; i < mh_temp3.Length; i++)
            {
                str = str + (char)mh_temp3[i];
            }
            byte[] data = Encoding.Unicode.GetBytes(str);
            txtChuKiGui.Text = Convert.ToBase64String(data);
            chuki = Convert.ToBase64String(data);
            //rsa_banMaHoaGuiDen.Text = Convert.ToBase64String(data);

        }

        
        public void RSA_GiaiMa(string ChuoiVao) // hàm giải mã
        {
            byte[] temp2 = Convert.FromBase64String(ChuoiVao);
            string giaima = Encoding.Unicode.GetString(temp2);

            int[] b = new int[giaima.Length];
            for (int i = 0; i < giaima.Length; i++)
            {
                b[i] = (int)giaima[i];
            }
            //Giải mã
            int[] c = new int[b.Length];
            for (int i = 0; i < c.Length; i++)
            {
                c[i] = RSA_mod(b[i], RSA_soD, RSA_soN);// giải mã
            }

            string str = "";
            for (int i = 0; i < c.Length; i++)
            {
                str = str + (char)c[i];
            }
            byte[] data2 = Convert.FromBase64String(str);
            txtChuKiNhan.Text = Encoding.Unicode.GetString(data2);
        }

        private void btTaoChuKi_Click(object sender, EventArgs e)
        {
            if (txtSHAGui.Text == "")
            {
                MessageBox.Show("Bạn chưa nhập bản rõ kí!", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            else
            {
                // thực hiện mã hóa
                OpenFileDialog dlg = new OpenFileDialog();
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    filechuki = dlg.FileName;
                }
                try
                {
                    RSA_MaHoa(txtSHAGui.Text);
                    FileStream fs = new FileStream(filechuki, FileMode.Create);//Tạo file mới tên là test.txt            
                    StreamWriter sWriter = new StreamWriter(fs, Encoding.UTF8);//fs là 1 FileStream 
                    sWriter.WriteLine(txtChuKiGui.Text);
                    kixong = true;
                    // Ghi và đóng file
                    sWriter.Flush();
                    fs.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void btCreatKey_Click(object sender, EventArgs e)
        {
            RSA_soP = RSA_soQ = 0;
            do
            {
                RSA_soP = RSA_ChonSoNgauNhien();
                RSA_soQ = RSA_ChonSoNgauNhien();
            }
            while (RSA_soP == RSA_soQ || !RSA_kiemTraNguyenTo(RSA_soP) || !RSA_kiemTraNguyenTo(RSA_soQ));
            txtP.Text = RSA_soP.ToString();
            txtQ.Text = RSA_soQ.ToString();
            RSA_taoKhoa();
            checkTaoKhoa = true;
        }

        private void btRefreshKey_Click(object sender, EventArgs e)
        {
            txtP.Text = string.Empty;
            txtQ.Text = string.Empty;
            txtE.Text = string.Empty;
            Key_CK_1.Text = string.Empty;
            Key_CK_2.Text = string.Empty;
            Key_BM_1.Text = string.Empty;
            Key_BM_2.Text = string.Empty;
            txtFileGui.Text = string.Empty;
            txtFileNhan.Text = string.Empty;
            txtSHAGui.Text = string.Empty;
            txtSHANhan.Text = string.Empty;
            txtChuKiGui.Text = string.Empty;
            txtChuKiNhan.Text = string.Empty;
        }
    }
}
