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
    public partial class Search : Form {
        private readonly Action<string> _Search;
        public Search() {
            InitializeComponent();
        }

        private void Search_Load(object sender, EventArgs e) {

        }

        public Search(Action<string> search):this() {
            _Search = search;
        }

        private void button1_Click(object sender, EventArgs e) {
            int index = int.Parse(textBox2.Text);
            string indexVal = textBox1.Text + "," + index.ToString();
            _Search(indexVal);
            this.Close();
        }
    }
}
