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
        string thisVersion = "0.0.3";
        string thisLicenseKey = "alpha";

        public CheckUpdate() {
            InitializeComponent();
            CheckUpdate_Load();
        }

        private void CheckUpdate_Load() {
            System.Threading.Thread updaterUI = new System.Threading.Thread(() => ShowDialog());
            updaterUI.Start();

            WebClient wc = new WebClient();
            string text = null;
            bool connectionFailed = true;

            while (text == null && connectionFailed) {
                label1.Text = "Connecting To Update Server";
                progressBar1.Value = 15;

                try {
                    System.IO.Stream stream = wc.OpenRead("http://deathcrow.altervista.org/update.php");
                    using (System.IO.StreamReader reader = new System.IO.StreamReader(stream)) {
                        text = reader.ReadToEnd();
                    }
                    connectionFailed = false;
                }
                catch (Exception ne) {
                    connectionFailed = true;
                    string errorMessage = string.Format("There was a problem connecting to the update server : {0}", ne.Message);

                    DialogResult userResponse = MessageBox.Show(errorMessage, "Failed to connect to server", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error);
                    if (userResponse == DialogResult.Cancel) {
                        updaterUI.Abort();
                        Application.Exit();
                        Environment.Exit(1);
                    }
                }

            }

            string[] texts = text.Split(','); // It would be nice to have something that checks the server did not give a malformed response here

            label1.Text = "Checking For Update";
            progressBar1.Value = 25;

            if (thisVersion != texts[0]) {
                //Update Available
                UpdateAvailable form = new UpdateAvailable();
                form.Show();
            } else {
                //No update Available
                progressBar1.Value = 75;
                label1.Text = "Checking License Key";
                //Check License
                if (thisLicenseKey != texts[1]) {
                    //Invalid License
                    InvalidLicense form = new InvalidLicense();
                    form.Show();
                } else {
                    //Valid License
                    progressBar1.Value = 100;
                    label1.Text = "All up to date.";
                    //Load
                    Form1 form = new Form1();

                    updaterUI.Abort();
                    form.Show();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            this.Hide();
        }
    }
}
