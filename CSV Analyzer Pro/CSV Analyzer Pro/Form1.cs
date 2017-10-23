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
using Syncfusion.Windows.Forms.Grid.Grouping;
using Syncfusion.Windows.Forms.Spreadsheet;

namespace CSV_Analyzer_Pro{
    public partial class Form1 : Form{
        #region Globals
        PluginLoader loader = new PluginLoader();

        DataSet ds = new DataSet();

        BackgroundWorker worker;
        ShortcutHandler shortcutHandler;

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

            tabControl1.KeyUp += new KeyEventHandler(KeyUpReporter);
            tabControl1.KeyDown += new KeyEventHandler(KeyDownReporter);
            tabControl1.KeyDown += new KeyEventHandler(ShortcutChecker);
            shortcutHandler = new ShortcutHandler();
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
            if (!IsWelcomePage()) {
                Thread th = new Thread(() => Save(path));
                th.Start();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!IsWelcomePage()) {
                SaveAs(path);
            }
        }

        private void addNewRowToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!IsWelcomePage()) {
                InsertRowNew();
            }
        }

        private void insertAfterToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!IsWelcomePage()) {
                InsertRowAfter();
            }
        }

        private void insertBeforeToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!IsWelcomePage()) {
                InsertRowBefore();
            }
        }

        private void insertNewToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!IsWelcomePage()) {
                InsertColumnNew();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e) {
            NewWindow();
        }

        private void FindToolStripMenuItem_Click(object sender, EventArgs e) {
            
        }

        private void setToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!IsWelcomePage()) {
                Core.Windows.Filter filter = new Core.Windows.Filter(this.HandleFilter);
                filter.Show();
            }
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!IsWelcomePage()) {
                HandleFilter("Row", "Like", "");
            }
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

        private void OnColumnHeaderMouseClick(object sender, MouseEventArgs e) {
            int row, col;
            GridDataBoundGrid dbg = sender as GridDataBoundGrid;
            if(dbg.PointToRowCol(new Point(e.X,e.Y),out row,out col)){
                if(dbg.Model[row,(col - 1)].CellType == "ColumnHeaderCell"){
                    int tableIndex = tabControl1.SelectedIndex;
                    EditHeader eh = new EditHeader(this.UpdateHeader);
                    eh.TextBox1.Text = ds.Tables[tableIndex.ToString()].Columns[(col - 1)].ToString();
                    eh.TextBox2.Text = (col - 1).ToString();
                    eh.Show();
                }
            }
        }

        private void OnKeyCommands(object sender,KeyEventArgs e) {
            //Psuedo code
        }

        private void KeyDownReporter(object sender, KeyEventArgs e) {
            shortcutHandler.ReportKeyDown(e.KeyCode.ToString());
        }

        private void KeyUpReporter(object sender, KeyEventArgs e) {
            shortcutHandler.ReportKeyUp(e.KeyCode.ToString());
        }

        private void ShortcutChecker(object sender, KeyEventArgs e) {
            switch (shortcutHandler.CheckShortcuts()) {
                case ShortcutHandler.shortcuts.NewWindow:
                    NewWindow();
                    ShortcutHandler.Instance.shortcut = ShortcutHandler.shortcuts.NoShortcut;
                    break;
                case ShortcutHandler.shortcuts.Open:
                    Browse();
                    ShortcutHandler.Instance.shortcut = ShortcutHandler.shortcuts.NoShortcut;
                    break;
                case ShortcutHandler.shortcuts.Save:
                    if (!IsWelcomePage()) {
                        Thread th = new Thread(() => Save(path));
                        th.Start();
                    }
                    ShortcutHandler.Instance.shortcut = ShortcutHandler.shortcuts.NoShortcut;
                    break;
            }
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

        private void HandleFilter(string filterState,string queryState,string queryString) {
            if(filterState == "Row") {
                if (queryState == "Like") {
                    GridDataBoundGrid dbg = tabControl1.SelectedTab.Controls.OfType<GridDataBoundGrid>().First();
                    DataView dv = ((DataTable)dbg.DataSource).DefaultView;
                    dv.RowFilter = queryString;
                    dbg.Refresh();
                }
            }
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
                Open(csvSearch.FileName);
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
                dbg.MouseDown += (s, e) => this.OnColumnHeaderMouseClick(s, e);
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

        private void grid_PrepareViewStyleInfo(object sender, GridPrepareViewStyleInfoEventArgs e) {

            if (e.ColIndex == 0 && e.RowIndex > 0) {

                e.Style.Text = e.RowIndex.ToString();

                e.Style.Font.Bold = false;

            }

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
            dbg.GridVisualStyles = Syncfusion.Windows.Forms.GridVisualStyles.Metro;
            DoubleBuffering(dbg, true);
            dbg.ShowColumnHeaders = true;
            dbg.AllowResizeToFit = true;

            dbg.BaseStylesMap["Row Header"].StyleInfo.CellType = "Header";

            dbg.Model.QueryCellInfo += new Syncfusion.Windows.Forms.Grid.GridQueryCellInfoEventHandler(Model_QueryCellInfo);
            #endregion
        }

        public void InitSpreadsheet(Spreadsheet spreadsheet) {
            spreadsheet.Dock = DockStyle.Fill;

            spreadsheet.Anchor = AnchorStyles.Left | AnchorStyles.Top;
        }

        public void InitSpreadsheetRibbon(SpreadsheetRibbon sRibbon) {
            sRibbon.MenuButtonVisible = false;
        }

        public bool IsWelcomePage() {
            if(tabControl1.SelectedIndex == 0) {
                return true;
            } else {
                return false;
            }
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
                Application.Exit();
            }catch(Exception e) {
                MessageBox.Show("There was an error trying to shutdown.\n\n Try closing all pages and then exiting.");
            }
        }
        #endregion

        #region Commands
        private void NewWindow() {
            TabPage tb = new TabPage();
            Spreadsheet spreadsheet = new Spreadsheet();
            SpreadsheetRibbon sRibbon = new SpreadsheetRibbon() { Spreadsheet = spreadsheet };

            InitSpreadsheet(spreadsheet);
            InitSpreadsheetRibbon(sRibbon);

            DataTable dt = new DataTable();

            tb.Text = "New";

            tb.Controls.Add(spreadsheet);
            tb.Controls.Add(sRibbon);
            tabControl1.TabPages.Add(tb);
            tabControl1.SelectedTab = tb;
        }

        private void NewPluginStoreTab() {
            TabPage tp = new TabPage();

            WebBrowser wb = new WebBrowser();
            System.Uri uri = new System.Uri("https://deathcrow.altervista.org/update/PluginStore/index.php");
            wb.Url = uri;

            tp.Controls.Add(wb);
            tabControl1.TabPages.Add(tp);
            tabControl1.SelectedTab = tp;
        }

        private void Open(string path) {
            Spreadsheet spreadsheet = tabControl1.SelectedTab.Controls.OfType<Spreadsheet>().First();
            spreadsheet.Open(path);
        }

        private void Save(string path) {
            Spreadsheet spreadsheet = tabControl1.SelectedTab.Controls.OfType<Spreadsheet>().First();
            spreadsheet.Save();
        }

        private void SaveAs(string path) {
            Spreadsheet spreadsheet = tabControl1.SelectedTab.Controls.OfType<Spreadsheet>().First();
            spreadsheet.SaveAs(path);
        }

        private void DeleteTab() {
            Debug.WriteLine("Delete Tab Called");
            int index = tabControl1.SelectedIndex;
            tabControl1.TabPages.RemoveAt(index);
        }

        private void InsertRowNew() {
            int index = tabControl1.SelectedIndex;

            GridDataBoundGrid dbg = tabControl1.SelectedTab.Controls.OfType<GridDataBoundGrid>().First();

            ds.Tables[index.ToString()].Rows.Add("");
            dbg.Refresh();
        }

        private void InsertRowAfter() {
            int tabCIndex = tabControl1.SelectedIndex;

            GridDataBoundGrid dbg = tabControl1.SelectedTab.Controls.OfType<GridDataBoundGrid>().First();
            DataRow dr;
            dr = ds.Tables[tabCIndex.ToString()].NewRow();
            int index = dbg.CurrentCell.RowIndex;
            ds.Tables[tabCIndex.ToString()].Rows.InsertAt(dr, index);
            dbg.Refresh();
        }

        private void InsertRowBefore() {
            int tabCIndex = tabControl1.SelectedIndex;

            GridDataBoundGrid dbg = tabControl1.SelectedTab.Controls.OfType<GridDataBoundGrid>().First();
            DataRow dr;
            dr = ds.Tables[tabCIndex.ToString()].NewRow();
            int index = dbg.CurrentCell.RowIndex - 1;
            ds.Tables[tabCIndex.ToString()].Rows.InsertAt(dr, index);
        }

        private void InsertColumnNew() {
            int index = tabControl1.SelectedIndex;

            ds.Tables[index.ToString()].Columns.Add("");
        }
        #endregion

        #region Unimplemented Experimental Code

        private void panel5_Paint(object sender, PaintEventArgs e) {

        }
        #endregion
    }
}
