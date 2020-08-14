namespace DGWnd.UI {
  partial class frmDGV {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing) {
      if (disposing && (components != null)) {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmDGV));
      this.tsUpper = new System.Windows.Forms.ToolStrip();
      this.btnChangeLayout = new System.Windows.Forms.ToolStripButton();
      this.btnSelectLayout = new System.Windows.Forms.ToolStripSplitButton();
      this.btnSaveLayout = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
      this.btnToggleGrid = new System.Windows.Forms.ToolStripButton();
      this.btnFont = new System.Windows.Forms.ToolStripButton();
      this.cbCellViewMode = new System.Windows.Forms.ToolStripComboBox();
      this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
      this.btnGroupLevel = new System.Windows.Forms.ToolStripDropDownButton();
      this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.btnSortUp = new System.Windows.Forms.ToolStripButton();
      this.btnSortDown = new System.Windows.Forms.ToolStripButton();
      this.btnRemoveSort = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
      this.btnFilterOnValue = new System.Windows.Forms.ToolStripButton();
      this.btnFilterOnValueClear = new System.Windows.Forms.ToolStripButton();
      this.txtFastFilter = new System.Windows.Forms.ToolStripTextBox();
      this.btnFind = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.btnPrint = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
      this.btnClone = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
      this.btnRequery = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
      this.btnSaveAsTempExcleAndOpen = new System.Windows.Forms.ToolStripButton();
      this.btnSaveAsTempTextAndOpen = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
      this.btnCancel = new System.Windows.Forms.ToolStripButton();
      this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
      this.lblStatistics = new System.Windows.Forms.ToolStripDropDownButton();
      this.lblCopyToClipboard = new System.Windows.Forms.ToolStripMenuItem();
      this.lblCount = new System.Windows.Forms.ToolStripMenuItem();
      this.lblAverage = new System.Windows.Forms.ToolStripMenuItem();
      this.lblMax = new System.Windows.Forms.ToolStripMenuItem();
      this.lblMin = new System.Windows.Forms.ToolStripMenuItem();
      this.lblSum = new System.Windows.Forms.ToolStripMenuItem();
      this.lblRecords = new System.Windows.Forms.ToolStripStatusLabel();
      this.statusStrip1 = new System.Windows.Forms.StatusStrip();
      this.waitSpinner = new System.Windows.Forms.PictureBox();
      this.dgv = new DGV.DGVCube();
      this.toolStripSeparator10 = new System.Windows.Forms.ToolStripSeparator();
      this.tsUpper.SuspendLayout();
      this.statusStrip1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.waitSpinner)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.dgv)).BeginInit();
      this.SuspendLayout();
      // 
      // tsUpper
      // 
      this.tsUpper.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.tsUpper.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnChangeLayout,
            this.btnSelectLayout,
            this.btnSaveLayout,
            this.toolStripSeparator9,
            this.btnToggleGrid,
            this.btnFont,
            this.cbCellViewMode,
            this.toolStripSeparator4,
            this.btnGroupLevel,
            this.toolStripSeparator1,
            this.btnSortUp,
            this.btnSortDown,
            this.btnRemoveSort,
            this.toolStripSeparator7,
            this.btnFilterOnValue,
            this.btnFilterOnValueClear,
            this.txtFastFilter,
            this.btnFind,
            this.toolStripSeparator3,
            this.btnPrint,
            this.toolStripSeparator5,
            this.btnClone,
            this.toolStripSeparator6,
            this.btnRequery,
            this.toolStripSeparator2,
            this.btnSaveAsTempExcleAndOpen,
            this.btnSaveAsTempTextAndOpen,
            this.toolStripSeparator8});
      this.tsUpper.Location = new System.Drawing.Point(0, 0);
      this.tsUpper.Name = "tsUpper";
      this.tsUpper.Size = new System.Drawing.Size(995, 31);
      this.tsUpper.TabIndex = 4;
      // 
      // btnChangeLayout
      // 
      this.btnChangeLayout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnChangeLayout.Image = global::DGWnd.Properties.Resources.setting_tools_24x24;
      this.btnChangeLayout.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnChangeLayout.ImageTransparentColor = System.Drawing.Color.White;
      this.btnChangeLayout.Name = "btnChangeLayout";
      this.btnChangeLayout.Size = new System.Drawing.Size(28, 28);
      this.btnChangeLayout.Text = "Змінити налаштування";
      this.btnChangeLayout.Click += new System.EventHandler(this.btnChangeLayout_Click);
      // 
      // btnSelectLayout
      // 
      this.btnSelectLayout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnSelectLayout.Image = global::DGWnd.Properties.Resources.folder_blue_24x24;
      this.btnSelectLayout.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnSelectLayout.ImageTransparentColor = System.Drawing.Color.White;
      this.btnSelectLayout.Name = "btnSelectLayout";
      this.btnSelectLayout.Size = new System.Drawing.Size(40, 28);
      this.btnSelectLayout.Text = "Вибрати налаштування";
      this.btnSelectLayout.ButtonClick += new System.EventHandler(this.btnSelectLayout_Click);
      this.btnSelectLayout.DropDownOpening += new System.EventHandler(this.btnSelectLayout_DropDownOpening);
      this.btnSelectLayout.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.btnSelectLayout_DropDownItemClicked);
      // 
      // btnSaveLayout
      // 
      this.btnSaveLayout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnSaveLayout.Image = global::DGWnd.Properties.Resources.Save_24x24;
      this.btnSaveLayout.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnSaveLayout.ImageTransparentColor = System.Drawing.Color.White;
      this.btnSaveLayout.Name = "btnSaveLayout";
      this.btnSaveLayout.Size = new System.Drawing.Size(28, 28);
      this.btnSaveLayout.Text = "Записати налаштування";
      this.btnSaveLayout.Click += new System.EventHandler(this.btnSaveLayout_Click);
      // 
      // toolStripSeparator9
      // 
      this.toolStripSeparator9.Name = "toolStripSeparator9";
      this.toolStripSeparator9.Size = new System.Drawing.Size(6, 31);
      // 
      // btnToggleGrid
      // 
      this.btnToggleGrid.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnToggleGrid.Image = global::DGWnd.Properties.Resources.grid_24x24;
      this.btnToggleGrid.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnToggleGrid.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnToggleGrid.Name = "btnToggleGrid";
      this.btnToggleGrid.Size = new System.Drawing.Size(28, 28);
      this.btnToggleGrid.Text = "Переключити видімість сітки";
      this.btnToggleGrid.Click += new System.EventHandler(this.btnToggleGrid_Click);
      // 
      // btnFont
      // 
      this.btnFont.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnFont.Image = global::DGWnd.Properties.Resources.font_24x24;
      this.btnFont.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnFont.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnFont.Name = "btnFont";
      this.btnFont.Size = new System.Drawing.Size(28, 28);
      this.btnFont.Text = "Шрифт";
      this.btnFont.Click += new System.EventHandler(this.btnFont_Click);
      // 
      // cbCellViewMode
      // 
      this.cbCellViewMode.Items.AddRange(new object[] {
            "Не встановлено",
            "1 рядок",
            "Переніс слів"});
      this.cbCellViewMode.Name = "cbCellViewMode";
      this.cbCellViewMode.Size = new System.Drawing.Size(140, 31);
      this.cbCellViewMode.Text = "Не встановлено";
      this.cbCellViewMode.ToolTipText = "Режим відображення рядків";
      this.cbCellViewMode.TextChanged += new System.EventHandler(this.cbCellViewMode_TextChanged);
      // 
      // toolStripSeparator4
      // 
      this.toolStripSeparator4.Name = "toolStripSeparator4";
      this.toolStripSeparator4.Size = new System.Drawing.Size(6, 31);
      // 
      // btnGroupLevel
      // 
      this.btnGroupLevel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnGroupLevel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2});
      this.btnGroupLevel.Image = ((System.Drawing.Image)(resources.GetObject("btnGroupLevel.Image")));
      this.btnGroupLevel.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnGroupLevel.Name = "btnGroupLevel";
      this.btnGroupLevel.Size = new System.Drawing.Size(83, 28);
      this.btnGroupLevel.Text = "Рівень груп";
      this.btnGroupLevel.DropDownOpening += new System.EventHandler(this.btnGroupLevel_DropDownOpening);
      this.btnGroupLevel.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.btnGroupLevel_DropDownItemClicked);
      // 
      // toolStripMenuItem2
      // 
      this.toolStripMenuItem2.Name = "toolStripMenuItem2";
      this.toolStripMenuItem2.Size = new System.Drawing.Size(80, 22);
      this.toolStripMenuItem2.Text = "1";
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
      // 
      // btnSortUp
      // 
      this.btnSortUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnSortUp.Image = global::DGWnd.Properties.Resources.sort_asc_24x24;
      this.btnSortUp.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnSortUp.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnSortUp.Name = "btnSortUp";
      this.btnSortUp.Size = new System.Drawing.Size(28, 28);
      this.btnSortUp.Text = "Сортування за зростанням";
      this.btnSortUp.Click += new System.EventHandler(this.btnSortUp_Click);
      // 
      // btnSortDown
      // 
      this.btnSortDown.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnSortDown.Image = global::DGWnd.Properties.Resources.sort_desc_24x24;
      this.btnSortDown.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnSortDown.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnSortDown.Name = "btnSortDown";
      this.btnSortDown.Size = new System.Drawing.Size(28, 28);
      this.btnSortDown.Text = "Сортування за зменшенням";
      this.btnSortDown.Click += new System.EventHandler(this.btnSortDown_Click);
      // 
      // btnRemoveSort
      // 
      this.btnRemoveSort.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnRemoveSort.Image = global::DGWnd.Properties.Resources.sort_cancel_24x24;
      this.btnRemoveSort.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnRemoveSort.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnRemoveSort.Name = "btnRemoveSort";
      this.btnRemoveSort.Size = new System.Drawing.Size(28, 28);
      this.btnRemoveSort.Text = "Відмінити сортування";
      this.btnRemoveSort.Click += new System.EventHandler(this.btnRemoveSort_Click);
      // 
      // toolStripSeparator7
      // 
      this.toolStripSeparator7.Name = "toolStripSeparator7";
      this.toolStripSeparator7.Size = new System.Drawing.Size(6, 31);
      // 
      // btnFilterOnValue
      // 
      this.btnFilterOnValue.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnFilterOnValue.Image = global::DGWnd.Properties.Resources.filter_24x24;
      this.btnFilterOnValue.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnFilterOnValue.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnFilterOnValue.Name = "btnFilterOnValue";
      this.btnFilterOnValue.Size = new System.Drawing.Size(28, 28);
      this.btnFilterOnValue.Text = "Фільтр по виразу клітинки";
      this.btnFilterOnValue.Click += new System.EventHandler(this.btnFilterOnValue_Click);
      // 
      // btnFilterOnValueClear
      // 
      this.btnFilterOnValueClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnFilterOnValueClear.Image = global::DGWnd.Properties.Resources.filter_clear_24x24;
      this.btnFilterOnValueClear.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnFilterOnValueClear.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnFilterOnValueClear.Name = "btnFilterOnValueClear";
      this.btnFilterOnValueClear.Size = new System.Drawing.Size(28, 28);
      this.btnFilterOnValueClear.Text = "Очистити фільтр по виразу клітинки";
      this.btnFilterOnValueClear.Click += new System.EventHandler(this.btnFilterOnValueClear_Click);
      // 
      // txtFastFilter
      // 
      this.txtFastFilter.Name = "txtFastFilter";
      this.txtFastFilter.Size = new System.Drawing.Size(81, 31);
      this.txtFastFilter.ToolTipText = "Текст швидкого фільтру";
      this.txtFastFilter.TextChanged += new System.EventHandler(this.txtFastFilter_TextChanged);
      // 
      // btnFind
      // 
      this.btnFind.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnFind.Image = global::DGWnd.Properties.Resources.find_24x24;
      this.btnFind.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnFind.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnFind.Name = "btnFind";
      this.btnFind.Size = new System.Drawing.Size(28, 28);
      this.btnFind.Text = "Пошук і заміна тексту";
      this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(6, 31);
      // 
      // btnPrint
      // 
      this.btnPrint.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnPrint.Image = global::DGWnd.Properties.Resources.printer_24x24;
      this.btnPrint.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnPrint.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnPrint.Name = "btnPrint";
      this.btnPrint.Size = new System.Drawing.Size(28, 28);
      this.btnPrint.Text = "Друк";
      this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
      // 
      // toolStripSeparator5
      // 
      this.toolStripSeparator5.Name = "toolStripSeparator5";
      this.toolStripSeparator5.Size = new System.Drawing.Size(6, 31);
      // 
      // btnClone
      // 
      this.btnClone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnClone.Image = global::DGWnd.Properties.Resources.windows_24x24;
      this.btnClone.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnClone.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnClone.Name = "btnClone";
      this.btnClone.Size = new System.Drawing.Size(28, 28);
      this.btnClone.Text = "Клонувати";
      this.btnClone.Click += new System.EventHandler(this.btnClone_Click);
      // 
      // toolStripSeparator6
      // 
      this.toolStripSeparator6.Name = "toolStripSeparator6";
      this.toolStripSeparator6.Size = new System.Drawing.Size(6, 31);
      // 
      // btnRequery
      // 
      this.btnRequery.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnRequery.Image = global::DGWnd.Properties.Resources.refresh_24x24;
      this.btnRequery.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnRequery.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnRequery.Name = "btnRequery";
      this.btnRequery.Size = new System.Drawing.Size(28, 28);
      this.btnRequery.Text = "Перезавантажити дані";
      this.btnRequery.Click += new System.EventHandler(this.btnRequery_Click);
      // 
      // toolStripSeparator2
      // 
      this.toolStripSeparator2.Name = "toolStripSeparator2";
      this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
      // 
      // btnSaveAsTempExcleAndOpen
      // 
      this.btnSaveAsTempExcleAndOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnSaveAsTempExcleAndOpen.Image = global::DGWnd.Properties.Resources.excel_24x24_4;
      this.btnSaveAsTempExcleAndOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnSaveAsTempExcleAndOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnSaveAsTempExcleAndOpen.Name = "btnSaveAsTempExcleAndOpen";
      this.btnSaveAsTempExcleAndOpen.Size = new System.Drawing.Size(28, 28);
      this.btnSaveAsTempExcleAndOpen.Text = "Ексель файл (записати в тимчасовий файл і відкрити його)";
      this.btnSaveAsTempExcleAndOpen.Click += new System.EventHandler(this.btnSaveAsTempExcleAndOpen_Click);
      // 
      // btnSaveAsTempTextAndOpen
      // 
      this.btnSaveAsTempTextAndOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnSaveAsTempTextAndOpen.Image = global::DGWnd.Properties.Resources.notepad_24x24;
      this.btnSaveAsTempTextAndOpen.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
      this.btnSaveAsTempTextAndOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnSaveAsTempTextAndOpen.Name = "btnSaveAsTempTextAndOpen";
      this.btnSaveAsTempTextAndOpen.Size = new System.Drawing.Size(28, 28);
      this.btnSaveAsTempTextAndOpen.Text = "Текстовий файл (записати у тимчасовий файл  і відкрити його)";
      this.btnSaveAsTempTextAndOpen.Click += new System.EventHandler(this.btnSaveAsTempTextAndOpen_Click_1);
      // 
      // toolStripSeparator8
      // 
      this.toolStripSeparator8.Name = "toolStripSeparator8";
      this.toolStripSeparator8.Size = new System.Drawing.Size(6, 31);
      // 
      // btnCancel
      // 
      this.btnCancel.BackColor = System.Drawing.Color.Yellow;
      this.btnCancel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnCancel.Font = new System.Drawing.Font("Tahoma", 9F);
      this.btnCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(148, 20);
      this.btnCancel.Text = "Зупинити завантаження";
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // lblStatus
      // 
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new System.Drawing.Size(118, 17);
      this.lblStatus.Text = "toolStripStatusLabel1";
      // 
      // lblStatistics
      // 
      this.lblStatistics.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
      this.lblStatistics.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.lblStatistics.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblCopyToClipboard,
            this.lblCount,
            this.toolStripSeparator10,
            this.lblAverage,
            this.lblMax,
            this.lblMin,
            this.lblSum});
      this.lblStatistics.Image = ((System.Drawing.Image)(resources.GetObject("lblStatistics.Image")));
      this.lblStatistics.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.lblStatistics.Name = "lblStatistics";
      this.lblStatistics.Size = new System.Drawing.Size(49, 20);
      this.lblStatistics.Text = "Сума";
      this.lblStatistics.DropDownOpening += new System.EventHandler(this.lblStatistics_DropDownOpening);
      // 
      // lblCopyToClipboard
      // 
      this.lblCopyToClipboard.ForeColor = System.Drawing.SystemColors.Desktop;
      this.lblCopyToClipboard.Name = "lblCopyToClipboard";
      this.lblCopyToClipboard.Size = new System.Drawing.Size(241, 22);
      this.lblCopyToClipboard.Text = "Копіювати у буфер клавіатури";
      this.lblCopyToClipboard.Click += new System.EventHandler(this.lblCopyToClipboard_Click);
      // 
      // lblCount
      // 
      this.lblCount.CheckOnClick = true;
      this.lblCount.Name = "lblCount";
      this.lblCount.Size = new System.Drawing.Size(241, 22);
      this.lblCount.Text = "lblCount";
      this.lblCount.CheckedChanged += new System.EventHandler(this.lblStatistics_CheckedChanged);
      // 
      // lblAverage
      // 
      this.lblAverage.CheckOnClick = true;
      this.lblAverage.Name = "lblAverage";
      this.lblAverage.Size = new System.Drawing.Size(241, 22);
      this.lblAverage.Text = "lblAverage";
      this.lblAverage.CheckedChanged += new System.EventHandler(this.lblStatistics_CheckedChanged);
      // 
      // lblMax
      // 
      this.lblMax.CheckOnClick = true;
      this.lblMax.Name = "lblMax";
      this.lblMax.Size = new System.Drawing.Size(241, 22);
      this.lblMax.Text = "lblMax";
      this.lblMax.CheckedChanged += new System.EventHandler(this.lblStatistics_CheckedChanged);
      // 
      // lblMin
      // 
      this.lblMin.CheckOnClick = true;
      this.lblMin.Name = "lblMin";
      this.lblMin.Size = new System.Drawing.Size(241, 22);
      this.lblMin.Text = "lblmin";
      this.lblMin.CheckedChanged += new System.EventHandler(this.lblStatistics_CheckedChanged);
      // 
      // lblSum
      // 
      this.lblSum.CheckOnClick = true;
      this.lblSum.Name = "lblSum";
      this.lblSum.Size = new System.Drawing.Size(241, 22);
      this.lblSum.Text = "Сума = ";
      this.lblSum.CheckedChanged += new System.EventHandler(this.lblStatistics_CheckedChanged);
      // 
      // lblRecords
      // 
      this.lblRecords.Name = "lblRecords";
      this.lblRecords.RightToLeft = System.Windows.Forms.RightToLeft.No;
      this.lblRecords.Size = new System.Drawing.Size(632, 17);
      this.lblRecords.Spring = true;
      this.lblRecords.Text = "toolStripStatusLabel1";
      this.lblRecords.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
      // 
      // statusStrip1
      // 
      this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnCancel,
            this.lblStatus,
            this.lblStatistics,
            this.lblRecords});
      this.statusStrip1.Location = new System.Drawing.Point(0, 479);
      this.statusStrip1.Name = "statusStrip1";
      this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
      this.statusStrip1.Size = new System.Drawing.Size(995, 22);
      this.statusStrip1.TabIndex = 3;
      this.statusStrip1.Text = "statusStrip1";
      // 
      // picLoader
      // 
      this.waitSpinner.Anchor = System.Windows.Forms.AnchorStyles.None;
      this.waitSpinner.Image = global::DGWnd.Properties.Resources.spinner;
      this.waitSpinner.Location = new System.Drawing.Point(120, 120);
      this.waitSpinner.Name = "waitSpinner";
      this.waitSpinner.Size = new System.Drawing.Size(60, 60);
      this.waitSpinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
      this.waitSpinner.TabIndex = 8;
      this.waitSpinner.TabStop = false;
      // 
      // dgv
      // 
      this.dgv.AllowUserToOrderColumns = true;
      this.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgv.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgv.Location = new System.Drawing.Point(0, 31);
      this.dgv.Name = "dgv";
      this.dgv.ReadOnly = true;
      this.dgv.RowTemplate.Height = 20;
      this.dgv.Size = new System.Drawing.Size(995, 448);
      this.dgv.TabIndex = 1;
      this.dgv.Visible = false;
      this.dgv.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgv_CellEnter);
      this.dgv.SelectionChanged += new System.EventHandler(this.dgv_SelectionChanged);
      // 
      // toolStripSeparator10
      // 
      this.toolStripSeparator10.Name = "toolStripSeparator10";
      this.toolStripSeparator10.Size = new System.Drawing.Size(238, 6);
      // 
      // frmDGV
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(995, 501);
      this.Controls.Add(this.waitSpinner);
      this.Controls.Add(this.dgv);
      this.Controls.Add(this.tsUpper);
      this.Controls.Add(this.statusStrip1);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.Name = "frmDGV";
      this.Text = "Головна форма DGVCube";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmDGV_FormClosed);
      this.Load += new System.EventHandler(this.frmDGV_Load);
      this.tsUpper.ResumeLayout(false);
      this.tsUpper.PerformLayout();
      this.statusStrip1.ResumeLayout(false);
      this.statusStrip1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.waitSpinner)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.dgv)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.PictureBox waitSpinner;
    private System.Windows.Forms.ToolStrip tsUpper;
    private System.Windows.Forms.ToolStripButton btnToggleGrid;
    private System.Windows.Forms.ToolStripButton btnFont;
    private System.Windows.Forms.ToolStripComboBox cbCellViewMode;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
    private System.Windows.Forms.ToolStripDropDownButton btnGroupLevel;
    private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripButton btnSortUp;
    private System.Windows.Forms.ToolStripButton btnSortDown;
    private System.Windows.Forms.ToolStripButton btnRemoveSort;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
    private System.Windows.Forms.ToolStripButton btnFilterOnValue;
    private System.Windows.Forms.ToolStripButton btnFilterOnValueClear;
    private System.Windows.Forms.ToolStripTextBox txtFastFilter;
    private System.Windows.Forms.ToolStripButton btnFind;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripButton btnPrint;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
    private System.Windows.Forms.ToolStripButton btnClone;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
    private System.Windows.Forms.ToolStripButton btnRequery;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    private System.Windows.Forms.ToolStripButton btnChangeLayout;
    private System.Windows.Forms.ToolStripButton btnSaveLayout;
    private System.Windows.Forms.ToolStripSplitButton btnSelectLayout;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
    private System.Windows.Forms.ToolStripButton btnSaveAsTempExcleAndOpen;
    private System.Windows.Forms.ToolStripButton btnSaveAsTempTextAndOpen;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
    private System.Windows.Forms.ToolStripButton btnCancel;
    private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    private System.Windows.Forms.ToolStripDropDownButton lblStatistics;
    private System.Windows.Forms.ToolStripMenuItem lblCopyToClipboard;
    private System.Windows.Forms.ToolStripMenuItem lblAverage;
    private System.Windows.Forms.ToolStripMenuItem lblCount;
    private System.Windows.Forms.ToolStripMenuItem lblMax;
    private System.Windows.Forms.ToolStripMenuItem lblMin;
    private System.Windows.Forms.ToolStripMenuItem lblSum;
    private System.Windows.Forms.ToolStripStatusLabel lblRecords;
    private System.Windows.Forms.StatusStrip statusStrip1;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator10;
    private DGV.DGVCube dgv;
  }
}