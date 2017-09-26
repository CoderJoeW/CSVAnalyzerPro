using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CSV_Analyzer_Pro.Core.Windows
{
    public partial class Filter : Form
    {
        #region Globals
        private readonly Action<string,string,string> _Filter;
        string filterState = "";
        string queryState = "";

        public Filter(Action<string,string,string> filter) : this() {
            _Filter = filter;
        }

        public string QueryString {
            get {
                return editControl1.Text;
            }
        }
        #endregion

        #region Initilize
        public Filter() {
            InitializeComponent();
        }

        private void Filter_Load(object sender, EventArgs e) {
            InitEditControl();
            comboBox2.SelectedIndexChanged += (s, ex) => this.SetQueryState(s, ex);
            comboBox1.SelectedIndexChanged += (s, ex) => this.SetFilterState(s, ex);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {

        }

        #endregion

        #region Helpers
        private void InitEditControl() {
            this.editControl1.ShowHorizontalSplitters = false;
            this.editControl1.ShowVerticalSplitters = false;
            this.editControl1.SaveOnClose = false;
        }
        #endregion

        #region Buttons and Clickers
        private void button1_Click(object sender, EventArgs e) {
            _Filter(filterState,queryState,QueryString);
            this.Close();
        }

        private void splitInHalfToolStripMenuItem_Click(object sender, EventArgs e) {
            this.editControl1.SplitVertically();
        }

        private void splitVerticleToolStripMenuItem_Click(object sender, EventArgs e) {
            this.editControl1.SplitHorizontally();
        }

        private void fourQuadrantsToolStripMenuItem_Click(object sender, EventArgs e) {
            this.editControl1.SplitFourQuadrants();
        }
        #endregion

        #region EventHandlers
        public void SetFilterState(object sender, EventArgs e) {
            if(comboBox1.SelectedIndex == 0) {
                filterState = "Row";
            }
        }
        public void SetQueryState(object sender, EventArgs e) {
            if(comboBox2.SelectedIndex == 0) {
                queryState = "Like";
                editControl1.Text = "ColumnName LIKE '*'";
            } else if(comboBox2.SelectedIndex == 1) {
                queryState = "Custom";
                editControl1.Text = "Insert Custom Query";
            }
        }
        #endregion

        #region ExitHandling
        private void Filter_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            
        }
        #endregion
    }
}
