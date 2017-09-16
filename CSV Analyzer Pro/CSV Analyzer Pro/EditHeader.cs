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
    public partial class EditHeader : Form {
        private readonly Action<string> _UpdateHeader;

        public EditHeader() {
            InitializeComponent();
        }

        private void EditHeader_Load(object sender, EventArgs e) {

        }

        public EditHeader(Action<string> updateHeader):this() {
            _UpdateHeader = updateHeader;
        }

        public TextBox TextBox1 {
            get {
                return textBox1;
            }
            set {
                
            }
        }

        public TextBox TextBox2 {
            get {
                return textBox2;
            }
            set {

            }
        }

        private void button1_Click(object sender, EventArgs e) {
            int index = int.Parse(textBox2.Text);
            string indexVal = index.ToString() + "," + textBox1.Text;
            _UpdateHeader(indexVal);
            this.Close();
        }
    }
}
