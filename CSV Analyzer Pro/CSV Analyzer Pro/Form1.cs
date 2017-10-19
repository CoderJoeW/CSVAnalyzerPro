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

namespace CSV_Analyzer_Pro{
    public partial class Form1 : Form{
        #region Globals
        PluginLoader loader = new PluginLoader();

        DataSet ds = new DataSet();

        BackgroundWorker worker;
        
        string path = "";

        bool _exiting = false;

        string Filters = "csv files (*.csv)|*.csv";

        List<TabExtraInfo> tabMetadataList = new List<TabExtraInfo>();
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
            
            //Create tab metadata for Welcome page
            TabExtraInfo welcomePage = new TabExtraInfo(0, "N/A");
            tabMetadataList.Insert(0, welcomePage);
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
                TabExtraInfo tabInfo;
                tabInfo = tabMetadataList.ElementAt(tabControl1.SelectedIndex);
                Thread th = new Thread(() => Save(tabInfo.GetAssocaitedFileName()));
                th.Start();
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e) {
            if (!IsWelcomePage()) {
                SaveAs();
            }
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e) {
            SaveAll();
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
            TabExtraInfo tabInfo;
            tabInfo = tabMetadataList.ElementAt(tabControl1.SelectedIndex);
            bool hasUnsavedChanges = tabInfo.QueryHasUnsavedChanges();
            if (e.Button == MouseButtons.Right) {

                if (hasUnsavedChanges) {
                    PromptUnsavedChangesCloseTab(tabInfo);
                } else { 
                    DialogResult result = MessageBox.Show("Do you really want to close this file?", "Confirmation", MessageBoxButtons.YesNoCancel);
                    if (result == DialogResult.Yes) {
                        DeleteTab();
                    }
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

        private void Model_QueryCellInfo(object sender, Syncfusion.Windows.Forms.Grid.GridQueryCellInfoEventArgs e) {
            if (e.Style.CellType != "ColumnHeaderCell" && (e.RowIndex % 2 == 0))
                e.Style.BackColor = Color.LightCyan;
            else
                e.Style.BackColor = Color.GhostWhite;
        }

        private void AddUnsavedMarkOnCellChanged(object sender, GridCellsChangedEventArgs e) {
            int index = tabControl1.SelectedIndex;
            TabExtraInfo tabInfo;
            tabInfo = tabMetadataList.ElementAt(index);
            if (!tabInfo.QueryHasUnsavedChanges()) {
                tabInfo.SetHasUnsavedChanges(true);
                tabControl1.SelectedTab.Text = "*" + tabControl1.SelectedTab.Text; // Add asterisk to denote unsaved changes
            }
        }
        
        private void FillRangeWithValueOnCellChanged(object sender, GridCellsChangedEventArgs e)
        {
            var eventRange = e.Range;

            if (eventRange.Top != eventRange.Bottom || eventRange.Left != eventRange.Right)
            {
                return;
            }

            var grid = tabControl1.SelectedTab.Controls.OfType<GridDataBoundGrid>().First();

            var modifiedCell = grid[eventRange.Top, eventRange.Left];
            var modifiedCellValue = modifiedCell.CellValue;
                
            var selectedRanges = grid.Selections.Ranges;
            foreach (GridRangeInfo selectedRange in selectedRanges)
            {
                UpdateRange(selectedRange, grid, modifiedCellValue);
            }
        }

        private static void UpdateRange(GridRangeInfo selectedRange, GridDataBoundGrid grid, object modifiedCellValue)
        {
            for (var row = selectedRange.Top; row <= selectedRange.Bottom; row++)
            {
                for (var col = selectedRange.Left; col <= selectedRange.Right; col++)
                {
                    UpdateCell(grid, modifiedCellValue, row, col);
                }
            }
        }

        private static void UpdateCell(GridDataBoundGrid grid, object modifiedCellValue, int row, int col)
        {
            if (grid[row, col].CellValue != modifiedCellValue)
            {
                grid[row, col].CellValue = modifiedCellValue;
            }
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

            bool safeToOpen = true;
            TabExtraInfo tabInfo;
            tabInfo = tabMetadataList.ElementAt(tabControl1.SelectedIndex);
            if (tabInfo.QueryHasUnsavedChanges()) {
                safeToOpen = PromptUnsavedChangesOpenTab(tabInfo); // Checking for unsaved changes since opening a new file deletes the current one
            }

            if (!safeToOpen) {
                return;
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
                dbg.MouseDown += (s, e) => this.OnColumnHeaderMouseClick(s, e);
                //Bind data source
                dbg.DataSource = ds.Tables[index.ToString()];
            }
            TabExtraInfo tabInfo;
            tabInfo = tabMetadataList.ElementAt(tabControl1.SelectedIndex);
            tabInfo.SetAssociatedFileName(tabControl1.SelectedTab.Text);
            tabInfo.SetHasUnsavedChanges(false);
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
            dbg.Model.CellsChanged += new Syncfusion.Windows.Forms.Grid.GridCellsChangedEventHandler(AddUnsavedMarkOnCellChanged);
            dbg.Model.CellsChanged += new Syncfusion.Windows.Forms.Grid.GridCellsChangedEventHandler(FillRangeWithValueOnCellChanged);
            #endregion
        }

        public bool IsWelcomePage() {
            if(tabControl1.SelectedIndex == 0) {
                return true;
            } else {
                return false;
            }
        }

        private void PromptUnsavedChangesCloseTab(TabExtraInfo tabInfo) {
            UnsavedChangesSingleFile unsavedChangesBox = new UnsavedChangesSingleFile();
            unsavedChangesBox.ShowDialog();

            switch (unsavedChangesBox.GetUserAnswer()) {
               case UnsavedChangesSingleFile.saveAndClose:
                    Save(tabInfo.GetAssocaitedFileName(), tabControl1.SelectedIndex);
                    DeleteTab();
                    break;
               case UnsavedChangesSingleFile.closeWithoutSaving:
                    DeleteTab();
                    break;
               case UnsavedChangesSingleFile.Cancel:
                    break;
            }
        }

        private bool PromptUnsavedChangesOpenTab(TabExtraInfo tabInfo) {
            UnsavedChangesSingleFile unsavedChangesBox = new UnsavedChangesSingleFile();
            unsavedChangesBox.ShowDialog();

            switch (unsavedChangesBox.GetUserAnswer()) {
               case UnsavedChangesSingleFile.saveAndClose:
                    Save(tabInfo.GetAssocaitedFileName(), tabControl1.SelectedIndex);
                    return true;
               case UnsavedChangesSingleFile.closeWithoutSaving:
                    return true;
               case UnsavedChangesSingleFile.Cancel:
                    return false;
               default:
                    return false; // Should be unreachable
            }
        }
        #endregion

        #region Exiting Functions
        protected override void OnFormClosing(FormClosingEventArgs e) {
            base.OnFormClosing(e);

            bool hasUnsavedChanges = false;

            foreach (TabExtraInfo tabInfo in tabMetadataList) { 
                if (tabInfo.QueryHasUnsavedChanges()) {
                    hasUnsavedChanges = true;
                }
            }

            if (e.CloseReason == CloseReason.WindowsShutDown) { return; }
            if (!_exiting && !hasUnsavedChanges) {
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
            else if (!_exiting && hasUnsavedChanges) {
                PromptUnsavedChangesExit(e);
            }   
        }

        private void PromptUnsavedChangesExit(FormClosingEventArgs e) {
            UnsavedChangesMultipleFiles unsavedChangesBox = new UnsavedChangesMultipleFiles();
            unsavedChangesBox.ShowDialog();

            switch (unsavedChangesBox.GetUserAnswer())
            {
                case UnsavedChangesMultipleFiles.saveAllAndClose:
                    SaveAll();
                    ExitAll();
                    break;
                case UnsavedChangesMultipleFiles.closeWithoutSaving:
                    ExitAll();
                    break;
                case UnsavedChangesMultipleFiles.Cancel:
                    e.Cancel = true;
                    break;
            }
        }

        private void ExitAll() {
            _exiting = true;
            try {
                Environment.Exit(0);
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

            //GridCardView card = new GridCardView();
            //card.CaptionField = "ProductName";
            //card.WireGrid(dbg);
            DataTable dt = new DataTable();

            tb.Text = "New";

            tb.Controls.Add(dbg);
            tabControl1.TabPages.Add(tb);
            tabControl1.SelectedTab = tb;
            
            TabExtraInfo newTab = new TabExtraInfo(tabControl1.TabCount-1, "N/A");
            tabMetadataList.Insert(tabControl1.TabCount-1, newTab);
        }

        private void Save(string filePath, int index=-1) {
            if (index == -1) {
                index = tabControl1.SelectedIndex;
                // I found that the usage of SelectedIndex in the Save functions resticted them to the current tab, which was a problem when trying to implement SaveAll.
                // This should allow saving of data from any tab without interfering with current functions by defaulting to SelectedIndex if no other index is specified.
            }

            if (filePath == "N/A") {
                return; // Skipping tabs with associatedFileName of N/A, mainly the Welcome page
            }

            TabExtraInfo tabInfo;
            tabInfo = tabMetadataList.ElementAt(index);

            StringBuilder sb = new StringBuilder();

            IEnumerable<string> columnNames = ds.Tables[index.ToString()].Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            foreach (DataRow row in ds.Tables[index.ToString()].Rows) {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            bool success;

            try {
                File.WriteAllText(filePath, sb.ToString());
                success = true;
            } catch (Exception e) {
                Console.WriteLine("An Exception occured when trying to save file");
                success = false;
            }

            if (success && tabInfo.QueryHasUnsavedChanges()) { // Avoiding removal of unsaved changes marker where it does not exist or where save has failed
                tabInfo.SetHasUnsavedChanges(false);
                tabControl1.TabPages[index].Text = tabControl1.TabPages[index].Text.Substring(1); // Return substring without unsaved changed marker
            }
        }

        private void SaveAs() {
            int index = tabControl1.SelectedIndex;

            string savePath = "";

            SaveFileDialog saveAs = new SaveFileDialog();
            saveAs.Filter = "csv files (*.csv)|*.csv";
            saveAs.FilterIndex = 1;

            if (saveAs.ShowDialog() == DialogResult.OK) {
                savePath = saveAs.FileName;
            }

            TabExtraInfo tabInfo;
            tabInfo = tabMetadataList.ElementAt(tabControl1.SelectedIndex);
            tabInfo.SetAssociatedFileName(savePath);
            tabControl1.SelectedTab.Text = savePath;

            Thread th = new Thread(() => Save(savePath));
            th.Start();

            //Retired
            //Save(savePath);
        }

        private void SaveAll() {
            int i;
            TabExtraInfo tabInfo;
            for (i = 0; i < tabControl1.TabCount; i++) {
                tabInfo = tabMetadataList.ElementAt(i);
                Save(tabInfo.GetAssocaitedFileName(), i);
            }
        }

        private void DeleteTab() {
            tabMetadataList.RemoveAt(tabControl1.SelectedIndex);
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
