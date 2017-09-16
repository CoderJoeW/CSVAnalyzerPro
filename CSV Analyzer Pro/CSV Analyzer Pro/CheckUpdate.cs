using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace CSV_Analyzer_Pro {
    public partial class CheckUpdate : Form {
        string thisVersion = "0.0.2";
        string thisLicenseKey = "alpha";

        public CheckUpdate() {
            InitializeComponent();
        }

        private void CheckUpdate_Load(object sender, EventArgs e) {
            WebClient wc = new WebClient();
            System.IO.Stream stream = wc.OpenRead("http://cof.ftp.sh/CsvAnalyzerPro/update.php");
            using(System.IO.StreamReader reader = new System.IO.StreamReader(stream)) {
                string text = reader.ReadToEnd();
                string[] texts = text.Split(',');

                label1.Text = "Checking For Update";
                progressBar1.Value = 10;

                if (thisVersion != texts[0]) {
                    //Update Available
                    UpdateAvailable form = new UpdateAvailable();
                    form.Show();
                }else {
                    //No update Available
                    progressBar1.Value = 75;
                    label1.Text = "Checking License Key";
                    //Check License
                    if (thisLicenseKey != texts[1]) {
                        //Invalid License
                        InvalidLicense form = new InvalidLicense();
                        form.Show();
                    }else {
                        //Valid License
                        progressBar1.Value = 100;
                        label1.Text = "All up to date.";
                        //Load
                        Form1 form = new Form1();

                        form.Show();
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            this.Hide();
        }
    }
}
