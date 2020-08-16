using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DGWnd.Misc;

namespace DGWnd.UI {
  public partial class frmDGV : Form
  {

    private bool _cancelLoading = false;
    private bool _isDisposing = false;
    private Stopwatch _loadDataTimer;
    private int? _loadTime;
    private bool _noRaiseEvent = false;

    public frmDGV()
    {
      InitializeComponent();
      waitSpinner.BackColor = DefaultBackColor; // Error in Designer
    }

    private void frmDGV_Load(object sender, EventArgs e) {
      this.lblStatus.Text = "";
      this.lblRecords.Text = "";
      this.lblRecords.Alignment = ToolStripItemAlignment.Right;
      //      lblStatistics_CheckedChanged( lblStatistics, new EventArgs());
      this.dgv._OnCellViewModeChanged += Dgv_OnCellViewModeChanged;
      Dgv_OnCellViewModeChanged(this.dgv, new EventArgs());
      dgv.DataSourceChanged += Dgv_DataSourceChanged;
      btnCancel.Visible = false;
      lblSum.Checked = true;
      tsUpper.Enabled = false;
      waitSpinner.Visible = false;
      lblStatistics.Visible = false;
    }

    public void Bind(DGCore.Sql.DataSourceBase ds, string layoutID, string startUpParameters, string startUpLayoutName, DGCore.UserSettings.DGV settings) =>
      Task.Run(() => { dgv.Bind(ds, layoutID, startUpParameters, startUpLayoutName, settings); });
    private void btnCancel_Click(object sender, EventArgs e) => _cancelLoading = true;

    private void Dgv_DataSourceChanged(object sender, EventArgs e)
    {
      if (dgv.DataSource != null)
      {
        dgv.DataSource.DataStateChanged -= dgv_OnDataChangedEventHandler;
        dgv.DataSource.DataStateChanged += dgv_OnDataChangedEventHandler;
      }
    }

    private void btnChangeLayout_Click(object sender, EventArgs e)
    {
      using (frmDGV_Layout frm = new frmDGV_Layout())
      {
        UserControls.UC_DGVLayout uc = frm.dgvLayout;
        uc.Bind(dgv, ((DGCore.UserSettings.IUserSettingSupport<DGCore.UserSettings.DGV>)dgv).GetSettings());
        frm.ShowDialog();
      }
    }

    private void btnToggleGrid_Click(object sender, EventArgs e)
    {
      dgv.CellBorderStyle = (dgv.CellBorderStyle == DataGridViewCellBorderStyle.None ? DataGridViewCellBorderStyle.Single : DataGridViewCellBorderStyle.None);
      dgv._layoutCount++;
    }

    private void btnFont_Click(object sender, EventArgs e)
    {
      using (var fontDialog = new FontDialog { ShowColor = false, Font = dgv.Font, Color = ForeColor })
        if (fontDialog.ShowDialog() != DialogResult.Cancel)
          dgv.Font = fontDialog.Font;
    }

    private void Dgv_OnCellViewModeChanged(object sender, EventArgs e)
    {
      _noRaiseEvent = true;
      cbCellViewMode.SelectedIndex = (int)this.dgv._CellViewMode;
      _noRaiseEvent = false;
    }

    private void btnGroupLevel_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {
      var showUpperLevel = !e.ClickedItem.Text.Contains("не показ");
      var s = e.ClickedItem.Text.Split(' ')[0];
      int.TryParse(s, out var groupLevel);
      dgv.DataSource.A_SetGroupLevel(groupLevel == 0? (int?)null: groupLevel, showUpperLevel);
    }

    private void btnSortUp_Click(object sender, EventArgs e)
    {
      var cell = dgv.CurrentCell;
      if (cell != null)
        dgv.DataSource.A_ApplySorting(cell.OwningColumn.DataPropertyName, cell.OwningRow.DataBoundItem, ListSortDirection.Ascending);
    }

    private void btnSortDown_Click(object sender, EventArgs e)
    {
      var cell = dgv.CurrentCell;
      if (cell != null)
        dgv.DataSource.A_ApplySorting(cell.OwningColumn.DataPropertyName, cell.OwningRow.DataBoundItem, ListSortDirection.Descending);
    }

    private void btnRemoveSort_Click(object sender, EventArgs e)
    {
      var cell = dgv.CurrentCell;
      if (cell != null)
        dgv.DataSource.A_RemoveSorting(cell.OwningColumn.DataPropertyName, cell.OwningRow.DataBoundItem);
    }

    private void btnFilterOnValue_Click(object sender, EventArgs e) {
      var cell = dgv.CurrentCell;
      if (cell != null)
        dgv.DataSource.A_SetByValueFilter(cell.OwningColumn.DataPropertyName, cell.Value);
    }

    private void btnFilterOnValueClear_Click(object sender, EventArgs e) => dgv.DataSource.A_ClearByValueFilter();
    private void txtFastFilter_TextChanged(object sender, EventArgs e) => dgv.DataSource.A_FastFilterChanged(this.txtFastFilter.Text);
    private void btnFind_Click(object sender, EventArgs e) => dgv.Find_OpenForm();
    private void btnPrint_Click(object sender, EventArgs e)
    {
      // To increase the print of page we need to activate PageSettings == it is doing only once at the first run of DGV print (through PrintDialog call)
      var printer = new ThirdParty.DGVPrinter();
      printer.PrintRowHeaders = true;
      printer.PrintMargins = new Margins(100, 40, 40, 40);
      //      printer.PrintMargins = new Margins(0, 0, 0, 0);

      // Title
      printer.Title = this.Text;
      printer.TitleFont = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
      printer.TitleSpacing = 0;
      printer.TitleAlignment = StringAlignment.Near;

      // Subtitle
      var subHeaders = GetSubheaders_ExcelAndPrint();
      if (subHeaders.Length == 0)
      {
        printer.SubTitle = null;
      }
      else
      {
        printer.SubTitle = string.Join(Environment.NewLine, subHeaders);
        printer.SubTitleAlignment = StringAlignment.Near;
        printer.SubTitleFormatFlags = StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
        printer.SubTitleSpacing = 0;
        printer.SubTitleFont = new Font("Tahoma", 8, FontStyle.Regular, GraphicsUnit.Point); ;
      }
      // No footer

      printer.PageNumbers = true;
      printer.PageNumberInHeader = true;
      printer.PageNumberOnSeparateLine = false;
      printer.PageText = DateTime.Now.ToString("G") + " / Сторінка ";
      printer.PartText = " - Частина ";
      printer.PageSeparator = " з ";
      printer.ShowTotalPageNumber = true;
      //      printer.pa

      printer.PorportionalColumns = false;
      printer.HeaderCellAlignment = StringAlignment.Center;
      //      printer.FooterSpacing = 0;
      printer.printDocument.PrinterSettings.PrintRange = (this.dgv.SelectedCells.Count < 2 ? PrintRange.AllPages : PrintRange.Selection);

      /*      // Logo image
            DGVPrinter.ImbeddedImage logo=new DGVPrinter.ImbeddedImage();
      //      logo.theImage = Image.FromFile(@"c:\BatLogo.43.29.TIF");
            logo.theImage = Image.FromFile(@"c:\BatLogo.56.38.TIF");
            //      logo.theImage = Image.FromFile(@"c:\batlogo.87.50.jpg");
            logo.ImageX = 0;
            logo.ImageY = 0;
            logo.ImageLocation = DGVPrinter.Location.Header;
            logo.ImageAlignment = DGVPrinter.Alignment.Left;
            printer.ImbeddedImageList.Add(logo);*/

      //      printer.PrintPreviewDataGridView(this.dgv);

      printer.SP_OwnerDraw -= dgv.DrawPrint_OwnerDraw;
      printer.SP_OwnerDraw += dgv.DrawPrint_OwnerDraw;
      printer.SP_PrintPreviewNoDisplay(this.dgv);
      printer.SP_OwnerDraw -= dgv.DrawPrint_OwnerDraw;
    }

    private void btnClone_Click(object sender, EventArgs e)
    {
      var newForm = new frmDGV {Text = Text};
      // Set window title
      var topForm = TopLevelControl;
      if (topForm?.GetType().Name == "frmMDI")
      {
        var mi = topForm.GetType().GetMethod("AttachNewChildForm");
        mi.Invoke(topForm, new object[] { newForm });
      }
      else
        newForm.Show();

      var settings = ((DGCore.UserSettings.IUserSettingSupport<DGCore.UserSettings.DGV>)dgv).GetSettings();
      newForm.Bind(dgv.DataSource.UnderlyingData, dgv._layoutID, dgv._startUpParameters, dgv._lastAppliedLayoutName, settings);
    }


    private void btnRequery_Click(object sender, EventArgs e) => dgv.DataSource.RequeryData();

    private string[] GetSubheaders_ExcelAndPrint() {
      List<string> subHeaders = new List<string>();
      if (!string.IsNullOrEmpty(this.dgv._lastAppliedLayoutName)) subHeaders.Add("Останнє налаштування: " + this.dgv._lastAppliedLayoutName);
      string s1 = this.dgv._startUpParameters;
      if (!string.IsNullOrEmpty(s1)) subHeaders.Add("Початкові параметри: " + s1);
      s1 = this.dgv.DataSource.WhereFilter.GetStringPresentation();
      if (!string.IsNullOrEmpty(s1)) subHeaders.Add("Фільтр даних: " + s1);
      if (this.dgv.DataSource.FilterByValue != null) {
        s1 = this.dgv.DataSource.FilterByValue.GetStringPresentation();
        if (!string.IsNullOrEmpty(s1)) subHeaders.Add("Фільтр по виразу клітинки: " + s1);
      }
      s1 = this.dgv.DataSource.TextFastFilter;
      if (!string.IsNullOrEmpty(s1)) subHeaders.Add("Текст швидкого фільтру: " + s1);
      return subHeaders.ToArray();
    }

    private void dgv_OnDataChangedEventHandler(object sender, DGCore.Sql.DataSourceBase.SqlDataEventArgs e) {
      switch (e.EventKind) {
        case DGCore.Sql.DataSourceBase.DataEventKind.Clear:
          this.UIThreadAsync(() =>
          {
            this.btnCancel.Visible = true;
            this.btnCancel.Select();
            dgv.Visible = false;
            ((IList)dgv.DataSource).Clear(); // need to remove bug: error on refresh sorted (ascendingby amount) GLDOCLINE
            _cancelLoading = false;
            ActivateWaitSpinner();
            _loadTime = null;
            _loadDataTimer = new Stopwatch();
            _loadDataTimer.Start();
          });
          break;
        case DGCore.Sql.DataSourceBase.DataEventKind.Loaded:
          _cancelLoading = false;
          this.UIThreadAsync(() =>
          {
            _loadDataTimer.Stop();
            _loadTime = Convert.ToInt32(_loadDataTimer.ElapsedMilliseconds);
            btnCancel.Visible = false;
            lblStatus.Text = @"Завантаження даних закінчено";
            dgv.DataSource.RefreshData();
            if (!dgv.Visible)
              dgv.Visible = true;
          });
          break;
        case DGCore.Sql.DataSourceBase.DataEventKind.Loading:
          if (_cancelLoading)
            e.CancelFlag = true;
          this.UIThreadAsync(() =>
          {
            lblStatus.Text = $@"Завантажено {e.RecordCount:N0} елементів";
            ActivateWaitSpinner();
            lblStatistics.Visible = false;
          });
          break;
        case DGCore.Sql.DataSourceBase.DataEventKind.BeforeRefresh:
          this.UIThreadAsync(() =>
          {
            lblStatus.Text = $@"Обробка даних ...";
            ActivateWaitSpinner();
            lblStatistics.Visible = false;
          });
          break;
        case DGCore.Sql.DataSourceBase.DataEventKind.Refreshed:
          this.UIThreadAsync(() =>
          {
            waitSpinner.Visible = false;
            var prefix = _loadTime.HasValue ? "Дані завантажені за " + _loadTime.Value.ToString("N0") + " мілісекунд. " : "";
            _loadTime = null;
            lblStatus.Text = prefix + "Дані оновлені. Час оновлення: " + this.dgv.DataSource.LastRefreshedTimeInMsecs + " мілісекунд";
            var totalRows = dgv.DataSource.UnderlyingData.GetData(false).Count;
            var dgvRows = dgv.DataSource.FilteredRowCount;
            prefix = "";
            if (dgv.DataSource.UnderlyingData.IsPartiallyLoaded)
              prefix = "Дані завантажені частково. ";
            lblRecords.Text = prefix + "Елементів: " + (totalRows == dgvRows ? "" : totalRows.ToString("N0") + " / ") +
                              dgvRows.ToString("N0");
            lblStatistics.Visible = true;
          });
          break;
      }
      this.UIThreadAsync(SetButtonState);
    }

    private void frmDGV_FormClosed(object sender, FormClosedEventArgs e)
    {
      if (_isDisposing)
        return;

      _isDisposing = true;
      _cancelLoading = true;
      dgv.Visible = false;
      while (btnCancel.Visible)
      {// data is loading
        Application.DoEvents();
        System.Threading.Thread.Sleep(300);
      }
    }

    //============================
    void SetButtonState() {
      bool layoutVisible = !string.IsNullOrEmpty(this.dgv._layoutID);
      if (this.btnSelectLayout.Visible != layoutVisible) {
        this.btnSelectLayout.Visible = layoutVisible;
        this.btnSaveLayout.Visible = layoutVisible;
        this.btnChangeLayout.Visible = layoutVisible;
      }

      if (this.btnCancel.Visible) {// Data Loading 
        if (this.tsUpper.Enabled) this.tsUpper.Enabled = false;
        return;
      }
      if (!this.tsUpper.Enabled) this.tsUpper.Enabled = true;

      bool groupExist = this.dgv.DataSource.Groups.Count > 0;
      if (this.btnGroupLevel.Enabled != groupExist) {
//        this.btnExpandAll.Enabled = groupExist;
  //      this.btnCollapseAll.Enabled = groupExist;
    //    this.btnShowTreeStructure.Enabled = groupExist;
        this.btnGroupLevel.Enabled = groupExist;
      }

      bool filterByValueExist = this.dgv.DataSource.FilterByValue != null && !this.dgv.DataSource.FilterByValue.IsEmpty;
      if (this.btnFilterOnValueClear.Enabled != filterByValueExist) {
        this.btnFilterOnValueClear.Enabled = filterByValueExist;
      }

      txtFastFilter.Text = dgv.DataSource.TextFastFilter;
    }

    private void cbCellViewMode_TextChanged(object sender, EventArgs e) {
      if (!this._noRaiseEvent)
      {
        this.SetCellViewMode();
        this.dgv.Invalidate();
      }
    }
    private void SetCellViewMode() {
      if (this.cbCellViewMode.Text == this.cbCellViewMode.Items[0].ToString()) {
        this.dgv._CellViewMode = DGCore.Common.Enums.DGCellViewMode.NotSet;
      }
      else if (this.cbCellViewMode.Text == this.cbCellViewMode.Items[1].ToString()) {
        this.dgv._CellViewMode = DGCore.Common.Enums.DGCellViewMode.OneRow;
      }
      else if (this.cbCellViewMode.Text == this.cbCellViewMode.Items[2].ToString()) {
        this.dgv._CellViewMode = DGCore.Common.Enums.DGCellViewMode.WordWrap;
      }
      else {
        MessageBox.Show($@"{this.cbCellViewMode.Text}: неправильний вираз");
      }
    }

    private void dgv_CellEnter(object sender, DataGridViewCellEventArgs e) {
      if (this.dgv._cellLast_PropertyDescriptor == null) {
        if (btnSortUp.Enabled) btnSortUp.Enabled = false;
        if (btnSortDown.Enabled) btnSortDown.Enabled = false;
        if (btnRemoveSort.Enabled) btnRemoveSort.Enabled = false;
        if (btnFilterOnValue.Enabled) btnFilterOnValue.Enabled = false;
      }
      else if (this.dgv.Columns[e.ColumnIndex].SortMode== DataGridViewColumnSortMode.NotSortable) {// can not sort
        if (btnSortUp.Enabled) btnSortUp.Enabled = false;
        if (btnSortDown.Enabled) btnSortDown.Enabled = false;
        if (btnRemoveSort.Enabled) btnRemoveSort.Enabled = false;
        if (btnFilterOnValue.Enabled) btnFilterOnValue.Enabled = true;
      }
      else {
        if (!btnFilterOnValue.Enabled) btnFilterOnValue.Enabled = true;
        bool isGroupColumn = (this.dgv._cellLast_Kind < 1 ? false : this.dgv.DataSource.Groups[this.dgv._cellLast_Kind - 1].PropertyDescriptor == this.dgv._cellLast_PropertyDescriptor);
        var rowValue = this.dgv.CurrentRow.DataBoundItem;
        var groupLevel = rowValue is DGCore.DGVList.IDGVList_GroupItem ? ((DGCore.DGVList.IDGVList_GroupItem) rowValue).Level : -1;
        switch (this.dgv._cellLast_SortDirection) {
          case null:
            if (!btnSortUp.Enabled) btnSortUp.Enabled = true;
            if (!btnSortDown.Enabled) btnSortDown.Enabled = true;
            if (btnRemoveSort.Enabled || isGroupColumn) btnRemoveSort.Enabled = false;
            break;
          case ListSortDirection.Ascending:
            if (btnSortUp.Enabled) btnSortUp.Enabled = false;
            if (!btnSortDown.Enabled) btnSortDown.Enabled = true;
            if (!btnRemoveSort.Enabled && !isGroupColumn) btnRemoveSort.Enabled = true;
            break;
          case ListSortDirection.Descending:
            if (!btnSortUp.Enabled) btnSortUp.Enabled = true;
            if (btnSortDown.Enabled) btnSortDown.Enabled = false;
            if (!btnRemoveSort.Enabled && !isGroupColumn) btnRemoveSort.Enabled = true;
            break;
        }
      }
    }

    // ========   Save to file ===========
    private void btnSaveAsTempTextAndOpen_Click_1(object sender, EventArgs e) {
      string fileExtension = "txt";
      string folder = Path.GetTempPath();
      string fn = "DGV_" + this.dgv._layoutID + "." + fileExtension;
      string fullFilename = DGCore.Utils.Tips.GetNearestNewFileName(folder, fn);
      this.Cursor = Cursors.WaitCursor;
      File_SaveAsTextFile(fullFilename);
      this.Cursor = Cursors.Default;
    }
    private void btnSaveAsTempExcleAndOpen_Click(object sender, EventArgs e) {
      string excelDefaultExtension = DGCore.Utils.ExcelApp.GetDefaultExtension();
      string folder = Path.GetTempPath();
      string fn = "DGV_" + this.dgv._layoutID + "." + excelDefaultExtension;
      string fullFilename = DGCore.Utils.Tips.GetNearestNewFileName(folder, fn);
      this.Cursor = Cursors.WaitCursor;
      File_SaveAsExcelFileAndOpen(fullFilename);
      this.Cursor = Cursors.Default;
    }
    void File_SaveAsExcelFileAndOpen(string filename) {
      // Save file
      Utils.DGVSave.SaveDGVToXLSFile(this.dgv, this.Text, GetSubheaders_ExcelAndPrint(), filename);
      // Open file
      if (File.Exists(filename)) {
        using (DGCore.Utils.ExcelApp excel = new DGCore.Utils.ExcelApp(filename, false)) {
          excel.Visible = true;
          excel.ScreenUpdating = true;
          //excel.Activate();
          //excel.SetWindowState(Utils.ExcelApp.xlWindowState.xlMaximized);
        }
      }
    }
    private void File_SaveAsTextFile(string filename) {
      // Save file
      Utils.DGVSave.SaveDGVToTextFile(this.dgv, filename);
      // Open new file
      if (File.Exists(filename)) {
        using (Process p = new Process()) {
          p.StartInfo.FileName = @"notepad.exe";
          p.StartInfo.Arguments = filename;
          p.Start();
        }
      }
    }

    // =========   Statistics ==========
    private void Statistics_UpdateValues() {
      int count; double min; double max; double average; double sum;
      Utils.DGVSelection.Statistics_Recalculate(dgv, out count, out min, out max, out sum, out average);
      lblAverage.Text = @"Середнє = " + average.ToString("#,###.#######").Replace("NaN", "?");
      lblCount.Text = @"Кількість числових елементів = " + count.ToString("N0");
      lblMax.Text = @"Максимальне = " + max.ToString("#,###.#######").Replace("NaN", "?");
      lblMin.Text = @"Мінімальне = " + min.ToString("#,###.#######").Replace("NaN", "?");
      lblSum.Text = @"Сума = " + sum.ToString("#,###.#######").Replace("NaN", "?");
    }
    private void Statistics_UpdateLabel() {
      lblStatistics.Text = _liveStatisticsItem == null ? "Статистика" : _liveStatisticsItem.Text;
    }
    private void dgv_SelectionChanged(object sender, EventArgs e) {
      if (_liveStatisticsItem != null)
        Statistics_UpdateValues();

      Statistics_UpdateLabel();
    }

    private void lblStatistics_DropDownOpening(object sender, EventArgs e) {
      if (_liveStatisticsItem == null) Statistics_UpdateValues();
    }

    ToolStripMenuItem _liveStatisticsItem = null;
    private void lblStatistics_CheckedChanged(object sender, EventArgs e) {
      ToolStripMenuItem item = (ToolStripMenuItem)sender;
      if (item.Checked) {
        foreach (ToolStripMenuItem x in lblStatistics.DropDownItems.Cast<object>().Where(x=> !(x is ToolStripSeparator))) {
          if (x.CheckOnClick) {
            if (x != item) x.Checked = false;
          }
        }
        _liveStatisticsItem = item;
        Statistics_UpdateLabel();
      }
      else {
        _liveStatisticsItem = null;
        Statistics_UpdateLabel();
      }
    }

    private void lblCopyToClipboard_Click(object sender, EventArgs e) {
      List<string> ss = new List<string>();
      foreach (ToolStripMenuItem x in lblStatistics.DropDownItems.Cast<object>().Where(x => !(x is ToolStripSeparator)))
      {
        if (x.CheckOnClick) {
          string[] ss1 = x.Text.Split('=');
          ss1[0] = ss1[0].Trim(); ss1[1] = ss1[1].Trim();
          ss.Add(string.Join("\t", ss1));
        }
      }
      Clipboard.Clear();
      Clipboard.SetData("Text", string.Join(Environment.NewLine, ss.ToArray()));
    }

    private void btnGroupLevel_DropDownOpening(object sender, EventArgs e) {
      int currentGroupLevel = dgv.DataSource.ExpandedGroupLevel;
      bool showUpperLevels = dgv.DataSource.ShowGroupsOfUpperLevels;
      this.btnGroupLevel.DropDownItems.Clear();
      ToolStripMenuItem[] items = new ToolStripMenuItem[this.dgv.DataSource.Groups.Count+(this.dgv.DataSource.Groups.Count-1) +1];
      int cnt = 0;
      for (int i = 0; i < this.dgv.DataSource.Groups.Count; i++) {
        items[i] = new ToolStripMenuItem((i + 1).ToString() + " рівень");
        if ((i + 1) == currentGroupLevel && showUpperLevels) items[i].Checked = true;
        cnt++;
      }
      for (int i = 1; i < this.dgv.DataSource.Groups.Count; i++) {
//        int itemNo = this.dgv._groups.Count + i - 1;
        items[cnt] = new ToolStripMenuItem((i + 1).ToString() + " рівень (не показувати рядки вищого рівня)");
        if ((i + 1) == currentGroupLevel && !showUpperLevels) items[cnt].Checked = true;
        cnt++;
      }
      items[cnt] = new ToolStripMenuItem("Вся інформація");
      if (currentGroupLevel == int.MaxValue && showUpperLevels) items[cnt].Checked = true;
      cnt++;
      this.btnGroupLevel.DropDownItems.AddRange(items);
    }

    private void btnSelectLayout_Click(object sender, EventArgs e) {
        DGCore.Utils.Dgv.EndEdit(this);
      using (var frm = new frmSelectSetting(dgv, dgv._lastAppliedLayoutName)) {
        var x = frm.ShowDialog();
        if (!string.IsNullOrEmpty(frm.SelectedSetting)) {
            DGCore.UserSettings.UserSettingsUtils.SetSetting(dgv, frm.SelectedSetting);
          dgv._lastAppliedLayoutName = frm.SelectedSetting;
        }
      }
    }
    private void btnSelectLayout_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
        DGCore.UserSettings.UserSettingsUtils.SetSetting(dgv, e.ClickedItem.Text);
      dgv._lastAppliedLayoutName = e.ClickedItem.Text;
    }
    private void btnSelectLayout_DropDownOpening(object sender, EventArgs e) {
      var keys = DGCore.UserSettings.UserSettingsUtils.GetKeysFromDb(this.dgv);
      var items = new List<ToolStripMenuItem>();
      foreach (var s in keys)
        items.Add(new ToolStripMenuItem(s));
      btnSelectLayout.DropDownItems.Clear();
      btnSelectLayout.DropDownItems.AddRange(items.ToArray());
    }

    private void btnSaveLayout_Click(object sender, EventArgs e) {
        DGCore.Utils.Dgv.EndEdit(this);
      using (var frm = new frmSaveSetting(dgv, dgv._lastAppliedLayoutName))
      {
        var x = frm.ShowDialog();
        if (!string.IsNullOrEmpty(frm.SelectedSetting))
          dgv._lastAppliedLayoutName = frm.SelectedSetting;
      }
    }

    private void ActivateWaitSpinner()
    {
      waitSpinner.Location = new Point {X = (ClientSize.Width - waitSpinner.Width) / 2, Y = (ClientSize.Height - waitSpinner.Height) / 2};
      waitSpinner.Visible = true;
    }
  }
}