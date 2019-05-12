using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Management;
using System.Threading;
namespace PromoBooster
{
    public partial class Login : Form
    {
        string webData = "";
        public Login()
        {
            InitializeComponent();
            try
            {
                System.Net.WebClient wc = new System.Net.WebClient();
                byte[] raw = wc.DownloadData("http://localhost/LoginFacebookBot.txt");
                webData = System.Text.Encoding.UTF8.GetString(raw);
                
            }
            catch
            {
                MessageBox.Show("Cannot connect to server.. Please contact the owner.");
                Environment.Exit(0);
                return;
            }
        }
        string hwid = "";
        private void Login_Load(object sender, EventArgs e)
        {
            ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""c:""");
            dsk.Get();
            hwid = dsk["VolumeSerialNumber"].ToString();
            textBox1.Text = hwid;
            textBox1.TextAlign = HorizontalAlignment.Center;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool canLog = false;
            string[] lines = webData.Split('\n');
            for (int i = 0; i < lines.Length; i++)
                if (hwid == lines[i].Replace("\r",""))
                    canLog = true;
            if (canLog)
            {
                this.Hide();
                new Form1().ShowDialog();
            }
            else
            {
                MessageBox.Show("Your license code isn't registered/activated YET or your membership has ended.\nPlease contact the owner!", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }
    }
}
