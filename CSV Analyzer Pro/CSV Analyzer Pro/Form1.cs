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
using Syncfusion.Windows.Forms.Grid;
using Syncfusion.GridHelperClasses;
using CSV_Analyzer_Pro.Core.PluginSystem;

namespace CSV_Analyzer_Pro{
    public partial class Form1 : Form{
        #region Globals
        PluginLoader loader = new PluginLoader();

        DataSet ds = new DataSet();

        BackgroundWorker worker;
        
        string path = "";

        bool _exiting = false;

        string Filters = "csv files (*.csv)|*.csv";
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
            try {
                loader.LoadPlugins();
            }catch(Exception exc) {
                Console.WriteLine(string.Format("Plugins couldnt be loaded: {0}", exc.Message));
            }
            LoadCommands();
        }

        private void LoadCommands() {
            //Register Commands
        }
        #endregion

        #region Buttons and Clickers
        private void openToolStripMenuItem_Click(object sender, EventArgs e) {
            Browse();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e) {
            Thread th = new Thread(() => Save(path));
            th.Start();
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

        private void OnColumnHeaderMouseClick(object sender, GridCellClickEventArgs e) {
            Debug.Write("ColumnHeaderClickEventFired");
            GridDataBoundGrid dbg = tabControl1.SelectedTab.Controls.OfType<GridDataBoundGrid>().First();
            if (dbg[e.RowIndex, e.ColIndex].CellType == "ColumnHeaderCell") {
                int index = tabControl1.SelectedIndex;
                EditHeader eh = new EditHeader(this.UpdateHeader);
                eh.TextBox1.Text = ds.Tables[index.ToString()].Columns[e.ColIndex].ToString();
                eh.TextBox2.Text = e.ColIndex.ToString();
                eh.Show();
            }
        }

        private void OnSearchCalled() {
            int index = tabControl1.SelectedIndex;
            Search search = new Search(this.SearchFor);
            search.Show();
        }

        private void OnKeyCommands(object sender,KeyEventArgs e) {
            //Psuedo code
        }

        private void Model_QueryCellInfo(object sender, Syncfusion.Windows.Forms.Grid.GridQueryCellInfoEventArgs e) {
            if (e.Style.CellType != "ColumnHeaderCell" && (e.RowIndex % 2 == 0))
                e.Style.BackColor = Color.LightCyan;
            else
                e.Style.BackColor = Color.GhostWhite;
        }
        #endregion

        #region Main Functions
        private void UpdateHeader(string indexVal) {
            int pageIndex = tabControl1.SelectedIndex;
            string[] array = indexVal.Split(',');
            int index = int.Parse(array[0]);
            //Debug.WriteLine("Index: " + index + " String: " + array[1] + " Table Check: " + ds.Tables[index.ToString()].ToString() + " Column Check: " + ds.Tables[index.ToString()].Columns[index].ColumnName.ToString());
            ds.Tables[pageIndex.ToString()].Columns[index].ColumnName = array[1];
        }

        private void SearchFor(string val) {
            GridDataBoundGrid dbg = tabControl1.SelectedTab.Controls.OfType<GridDataBoundGrid>().First();
            //dbg.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            int pageIndex = tabControl1.SelectedIndex;
            string[] array = val.Split(',');
            string searchValue = array[0];
            int index = int.Parse(array[1]);

            HandleSearch(dbg, searchValue, index);
        }

        private void HandleSearch(GridDataBoundGrid dbg,string searchValue,int index) {
            //Rewite
        }
        #endregion

        #region DataHandling
        private void Browse() {
            if (tabControl1.SelectedIndex == 0) {
                NewWindow();
            }
            OpenFileDialog csvSearch = new OpenFileDialog();
            csvSearch.Filter = Filters;
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
                OpenCSVFile();
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
                GridDataBoundGrid dbg = tabControl1.SelectedTab.Controls.OfType<GridDataBoundGrid>().First();
                //Attach event handler
                //dbg.CellClick += (s, e) => this.OnColumnHeaderMouseClick(s, e);
                //Bind data source
                dbg.DataSource = ds.Tables[index.ToString()];
            }
        }
        #endregion

        #region Helpers
        private void DoubleBuffering(GridDataBoundGrid dbg, bool setting) {
            Type dbgType = dbg.GetType();
            PropertyInfo pi = dbgType.GetProperty("DoubleBuffered",
                  BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dbg, setting, null);
        }

        public void InitGrid(GridDataBoundGrid dbg) {

            #region DataGridView Contructing
            dbg.Dock = DockStyle.Fill;
            dbg.ExcelLikeSelectionFrame = true;
            dbg.ExcelLikeCurrentCell = true;
            dbg.Model.Options.SelectionBorderBrush = new SolidBrush(Color.DarkGreen);
            dbg.Model.Options.SelectionBorderThickness = 4;
            dbg.ListBoxSelectionMode = SelectionMode.None;
            dbg.ShowRowHeaders = false;
            dbg.ThemesEnabled = true;
            dbg.GridVisualStyles = Syncfusion.Windows.Forms.GridVisualStyles.Office2007Blue;
            DoubleBuffering(dbg, true);

            dbg.Model.QueryCellInfo += new Syncfusion.Windows.Forms.Grid.GridQueryCellInfoEventHandler(Model_QueryCellInfo);
            #endregion
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
            GridDataBoundGrid dbg = new GridDataBoundGrid();
            InitGrid(dbg);
            loader.GetPluginByTargetFramework("GridDataBoundGrid", dbg);
            DataTable dt = new DataTable();

            tb.Text = "New..";

            tb.Controls.Add(dbg);
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

            GridDataBoundGrid dbg = tabControl1.SelectedTab.Controls.OfType<GridDataBoundGrid>().First();

            if (index == 0){return;}

            ds.Tables[index.ToString()].Rows.Add("");
            dbg.Refresh();
        }

        private void InsertRowAfter() {
            int tabCIndex = tabControl1.SelectedIndex;

            if(tabCIndex == 0){return;}

            GridDataBoundGrid dbg = tabControl1.SelectedTab.Controls.OfType<GridDataBoundGrid>().First();
            DataRow dr;
            dr = ds.Tables[tabCIndex.ToString()].NewRow();
            int index = dbg.CurrentCell.RowIndex;
            ds.Tables[tabCIndex.ToString()].Rows.InsertAt(dr, index);
            dbg.Refresh();
        }

        private void InsertRowBefore() {
            int tabCIndex = tabControl1.SelectedIndex;

            if(tabCIndex == 0){return;}

            GridDataBoundGrid dbg = tabControl1.SelectedTab.Controls.OfType<GridDataBoundGrid>().First();
            DataRow dr;
            dr = ds.Tables[tabCIndex.ToString()].NewRow();
            int index = dbg.CurrentCell.RowIndex - 1;
            ds.Tables[tabCIndex.ToString()].Rows.InsertAt(dr, index);
        }

        private void InsertColumnNew() {
            int index = tabControl1.SelectedIndex;

            if(index == 0){return;}

            ds.Tables[index.ToString()].Columns.Add("");
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
