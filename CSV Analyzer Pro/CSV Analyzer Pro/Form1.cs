using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.VisualBasic.FileIO;
using System.Threading;
using System.Reflection;

namespace CSV_Analyzer_Pro{
    public partial class Form1 : Form{
        #region Globals
        DataSet ds = new DataSet();

        BackgroundWorker worker;
        
        string path = "";

        bool _exiting = false;
        #endregion

        #region Initializing
        public Form1(){
            InitializeComponent();

            //Initiate worker
            worker = new BackgroundWorker();

            //Worker Event Handlers
            worker.DoWork += new DoWorkEventHandler(worker_DoWork);
            worker.ProgressChanged += new ProgressChangedEventHandler(worker_ProgressChanged);
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(worker_RunWorkerCompleted);

            //Enable Progress Reorting
            worker.WorkerReportsProgress = true;
        }

        private void Form1_Load(object sender, EventArgs e) {
            LoadCommands();
        }

        private void LoadCommands() {
            //Register KeyDown Commands
            tabControl1.KeyDown += (s, e) => this.OnKeyCommands(s, e);
        }
        #endregion

        #region Buttons and Clickers
        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            Browse();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            Thread th = new Thread(() => Save(path));
            th.Start();

            //Retired
            //Save(path);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveAs();
        }

        private void addNewRowToolStripMenuItem_Click(object sender, EventArgs e) {
            InsertRowNew();
        }

        private void insertAfterToolStripMenuItem_Click(object sender, EventArgs e) {
            InsertRowAfter();
        }

        private void insertBeforeToolStripMenuItem_Click(object sender, EventArgs e) {
            InsertRowBefore();
        }

        private void insertNewToolStripMenuItem_Click(object sender, EventArgs e) {
            InsertColumnNew();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            NewWindow();
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e) {
            MessageBox.Show("This feature is currently disabled");
            //OnSearchCalled();
        }

        #endregion

        #region Worker Functions
        private void worker_DoWork(object sender, DoWorkEventArgs e) {
            OpenCSVFile();
            worker.ReportProgress(1, DateTime.Now);
            worker.ReportProgress(20, DateTime.Now);
            worker.ReportProgress(60, DateTime.Now);
            worker.ReportProgress(80, DateTime.Now);

            // Lets sleep for a little bit here:  
            //Thread.Sleep(5000);

            //res 
            //e.Result = "Done";
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) { 
            //MessageBox.Show(e.Result.ToString());
        }

        private void worker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
            //Do Nothing
        }
        #endregion

        #region EventHandlers
        private void OnTabMouseUp(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Right) {
                DialogResult result = MessageBox.Show("Do you really wanna delete this page? All unsaved data will be lost!", "Confirmation", MessageBoxButtons.YesNoCancel);
                if (result == DialogResult.Yes) {
                    DeleteTab();
                }
            }
        }

        private void OnColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e) {
            int index = tabControl1.SelectedIndex;
            EditHeader eh = new EditHeader(this.UpdateHeader);
            eh.TextBox1.Text = ds.Tables[index.ToString()].Columns[e.ColumnIndex].ToString();
            eh.TextBox2.Text = e.ColumnIndex.ToString();
            eh.Show();
        }

        private void OnSearchCalled() {
            int index = tabControl1.SelectedIndex;
            Search search = new Search(this.SearchFor);
            search.Show();
        }

        private void OnKeyCommands(object sender,KeyEventArgs e) {
            //Psuedo code
        }
        #endregion

        #region Main Functions
        private void UpdateHeader(string indexVal) {
            int pageIndex = tabControl1.SelectedIndex;
            string[] array = indexVal.Split(',');
            int index = int.Parse(array[0]);
            //Debug.WriteLine("Index: " + index + " String: " + array[1] + " Table Check: " + ds.Tables[index.ToString()].ToString() + " Column Check: " + ds.Tables[index.ToString()].Columns[index].ColumnName.ToString());
            ds.Tables[pageIndex.ToString()].Columns[index].ColumnName = array[1];
            DisableSortMode(tabControl1.SelectedTab.Controls.OfType<DataGridView>().First());
        }

        private void SearchFor(string val) {
            DataGridView dgv = tabControl1.SelectedTab.Controls.OfType<DataGridView>().First();
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            int pageIndex = tabControl1.SelectedIndex;
            string[] array = val.Split(',');
            string searchValue = array[0];
            int index = int.Parse(array[1]);

            HandleSearch(dgv, searchValue, index);
        }

        private void HandleSearch(DataGridView dgv,string searchValue,int index) {
            //BROKEN CODE
            try {
                bool valueRes = true;
                foreach (DataGridViewRow row in dgv.Rows) {
                    if (row.Cells[index].Value.ToString().Equals(searchValue)) {
                        int rowIndex = row.Index;
                        dgv.Rows[rowIndex].Selected = true;
                        rowIndex++;
                        valueRes = false;
                        dgv.SelectionMode = DataGridViewSelectionMode.CellSelect;
                    }
                }
                if (valueRes != false) {
                    MessageBox.Show("Record is not avalable for this Name" + searchValue, "Not Found");
                    return;
                }
            } catch (Exception e) {
                MessageBox.Show("Error: " + e.Message);
            }
        }

        private void DisableSortMode(DataGridView dgv) {
            foreach (DataGridViewColumn column in dgv.Columns) {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }
        #endregion

        #region DataHandling
        private void Browse() {
            if (tabControl1.SelectedIndex == 0) {
                NewWindow();
            }
            OpenFileDialog csvSearch = new OpenFileDialog();
            csvSearch.Filter = "csv files (*.csv)|*.csv";
            csvSearch.FilterIndex = 1;
            csvSearch.Multiselect = false;

            if (csvSearch.ShowDialog() == DialogResult.OK) {
                int index = tabControl1.SelectedIndex;
                //Clear Datasource
                if (ds.Tables.CanRemove(ds.Tables[index.ToString()])) {
                    ds.Tables.Remove(ds.Tables[index.ToString()]);
                }
                DataTable dt = ds.Tables.Add(index.ToString());
                path = csvSearch.FileName;
                tabControl1.SelectedTab.Text = csvSearch.FileName;
                //Background worker
                if (!worker.IsBusy) {
                    worker.RunWorkerAsync();
                } else {
                    MessageBox.Show("Worker is busy..Please Wait");
                }

                //Retired
                //OpenCSVFile();
            }
        }

        private void OpenCSVFile() {
            CheckForIllegalCrossThreadCalls = false;
            using (TextFieldParser csvParser = new TextFieldParser(path)) {
                csvParser.TextFieldType = FieldType.Delimited;
                csvParser.SetDelimiters(",");

                int index = this.tabControl1.SelectedIndex;

                bool firstLine = true;

                while (!csvParser.EndOfData) {
                    //proccessing
                    string[] fields = csvParser.ReadFields();

                    if (firstLine) {
                        foreach (var val in fields) {
                            ds.Tables[index.ToString()].Columns.Add(val);
                        }

                        firstLine = false;

                        continue;
                    }
                    //get row data
                    ds.Tables[index.ToString()].Rows.Add(fields);
                }
                //Get gridview
                DataGridView dgv = tabControl1.SelectedTab.Controls.OfType<DataGridView>().First();
                //Attach event handler
                dgv.ColumnHeaderMouseClick += (s, e) => this.OnColumnHeaderMouseClick(s, e);
                //Bind data source
                dgv.DataSource = ds.Tables[index.ToString()];
                DisableSortMode(dgv);
            }
        }
        #endregion

        #region Helpers
        private void DoubleBuffering(DataGridView dgv, bool setting) {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }
        #endregion

        #region Exiting Functions
        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);

            if (e.CloseReason == CloseReason.WindowsShutDown) return;
            if (!_exiting) {
                switch (MessageBox.Show(this, "Are you sure you want to exit?", "Closing", MessageBoxButtons.YesNo)) {
                    case DialogResult.No:
                        e.Cancel = true;
                        break;
                    case DialogResult.Yes:
                        ExitAll();
                        break;
                    default:
                        break;
                }
            }
        }

        private void ExitAll() {
            _exiting = true;
            try {
                Environment.Exit(1);
            }catch(Exception e) {
                MessageBox.Show("There was an error trying to shutdown.\n\n Try closing all pages and then exiting.");
            }
        }
        #endregion

        #region Commands
        private void NewWindow() {
            TabPage tb = new TabPage();
            CustomDgv dgv = new CustomDgv();
            DataTable dt = new DataTable();

            tb.Text = "New..";

            #region DataGridView Contructing
            dgv.Dock = DockStyle.Fill;
            dgv.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dgv.BackgroundColor = Color.White;
            dgv.RowHeadersVisible = false;
            dgv.GridColor = Color.LightGray;
            dgv.BorderStyle = BorderStyle.None;
            dgv.EnableHeadersVisualStyles = false;
            dgv.VirtualMode = true;

            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            dgv.ColumnHeadersHeight = 30;
            DoubleBuffering(dgv, true);
            //dgv.GetType.InvokeMember("DoubleBuffered", Reflection.BindingFlags.NonPublic Or _Reflection.BindingFlags.Instance Or System.Reflection.BindingFlags.SetProperty, Nothing, dgv, New Object() { True});

            //dgv.Columns(1).DefaultCellStyle.BackColor = Color.Yellow

            //Retired
            //dgv.Anchor = (AnchorStyles.Top | AnchorStyles.Left);
            //dgv.ScrollBars = ScrollBars.Both;
            #endregion

            tb.Controls.Add(dgv);
            tabControl1.TabPages.Add(tb);
            tabControl1.SelectedTab = tb;
        }

        private void Save(string filePath) {
            int index = tabControl1.SelectedIndex;

            if(index == 0){return;}

            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = ds.Tables[index.ToString()].Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in ds.Tables[index.ToString()].Rows) {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            try {
                File.WriteAllText(filePath, sb.ToString());
            } catch (Exception e) {
                Console.WriteLine("An Exception occured when trying to save file");
            }

        }

        private void SaveAs() {
            int index = tabControl1.SelectedIndex;

            if(index == 0){return;}

            string savePath = "";

            SaveFileDialog saveAs = new SaveFileDialog();
            saveAs.Filter = "csv files (*.csv)|*.csv";
            saveAs.FilterIndex = 1;

            if (saveAs.ShowDialog() == DialogResult.OK) {
                savePath = saveAs.FileName;
            }

            Thread th = new Thread(() => Save(savePath));
            th.Start();

            //Retired
            //Save(savePath);
        }

        private void DeleteTab() {
            Debug.WriteLine("Delete Tab Called");
            int index = tabControl1.SelectedIndex;
            tabControl1.TabPages.RemoveAt(index);
        }

        private void InsertRowNew() {
            int index = tabControl1.SelectedIndex;

            if(index == 0){return;}

            ds.Tables[index.ToString()].Rows.Add("");
        }

        private void InsertRowAfter() {
            int tabCIndex = tabControl1.SelectedIndex;

            if(tabCIndex == 0){return;}

            DataGridView dgv = tabControl1.SelectedTab.Controls.OfType<DataGridView>().First();
            DataRow dr;
            dr = ds.Tables[tabCIndex.ToString()].NewRow();
            int index = dgv.CurrentCell.RowIndex;
            Debug.WriteLine("Index: " + index);
            ds.Tables[tabCIndex.ToString()].Rows.InsertAt(dr, index + 1);
        }

        private void InsertRowBefore() {
            int tabCIndex = tabControl1.SelectedIndex;

            if(tabCIndex == 0){return;}

            DataGridView dgv = tabControl1.SelectedTab.Controls.OfType<DataGridView>().First();
            DataRow dr;
            dr = ds.Tables[tabCIndex.ToString()].NewRow();
            int index = dgv.CurrentCell.RowIndex;
            ds.Tables[tabCIndex.ToString()].Rows.InsertAt(dr, index);
        }

        private void InsertColumnNew() {
            int index = tabControl1.SelectedIndex;

            if(index == 0){return;}

            ds.Tables[index.ToString()].Columns.Add("");
            DisableSortMode(tabControl1.SelectedTab.Controls.OfType<DataGridView>().First());
        }
        #endregion

        #region Unimplemented Experimental Code
        //Experimental Test Code
        //Paste Multiple Cells
        //Needs enhancements and handler
        //Add code to NewWindow function
        private void paste() {
            DataGridView dgv = tabControl1.SelectedTab.Controls.OfType<DataGridView>().First();
            if (dgv.SelectedCells.Count < 1) return;

            string[] lines;
            int row = dgv.Rows.Count;
            int col = dgv.Columns.Count;
            int maxrow = 0;
            int maxcol = 0;
            int cellcount = dgv.SelectedCells.Count;

            //find selection extents
            foreach (DataGridViewCell cell in dgv.SelectedCells) {
                if (cell.RowIndex < row) row = cell.RowIndex;
                if (cell.RowIndex > maxrow) maxrow = cell.RowIndex;
                if (cell.ColumnIndex < col) col = cell.ColumnIndex;
                if (cell.ColumnIndex > maxcol) maxcol = cell.ColumnIndex;
            }

            if (Clipboard.GetText().Contains(Environment.NewLine) || Clipboard.GetText().Contains('\t')) {
                //multiple cells copied;
                lines = Clipboard.GetText().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

                string[] values;
                for (int i = 0; i < lines.Length; i++) {
                    values = lines[i].Split('\t');

                    if (row >= dgv.Rows.Count || dgv.Rows[row].IsNewRow || (cellcount > 1 && row > maxrow)) continue;
                    for (int j = 0; j < values.Length; j++) {
                        if (col + j >= dgv.Columns.Count || (cellcount > 1 && col + j > maxcol)) continue;
                        dgv.Rows[row].Cells[col + j].Value = values[j];
                    }

                    row++;
                }
            } else {
                //single cell
                string value = Clipboard.GetText().Trim();
                foreach (DataGridViewCell dc in dgv.SelectedCells) {
                    dc.Value = value;
                }
            }
        }

        private void panel5_Paint(object sender, PaintEventArgs e) {

        }
        #endregion
    }
}
