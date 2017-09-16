using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSV_Analyzer_Pro {
    public partial class UpdateAvailable : Form {
        string updateURL = "http://cof.ftp.sh/CsvAnalyzerPro/index.php";

        public UpdateAvailable() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            System.Diagnostics.Process.Start(updateURL);
        }
    }
}
