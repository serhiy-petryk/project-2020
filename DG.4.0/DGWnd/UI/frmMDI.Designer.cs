namespace DGWnd.UI {
  partial class frmMDI {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMDI));
      this.splitter1 = new System.Windows.Forms.Splitter();
      this.menuStrip1 = new System.Windows.Forms.MenuStrip();
      this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuWindow = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuWHorizontal = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuWVertical = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuWCascade = new System.Windows.Forms.ToolStripMenuItem();
      this.mnuWArrange = new System.Windows.Forms.ToolStripMenuItem();
      this.stripWnds = new System.Windows.Forms.ToolStrip();
      this.stripBtns = new System.Windows.Forms.ToolStrip();
      this.btnDataDefinitionList = new System.Windows.Forms.ToolStripButton();
      this.btnMemoryInUsed = new System.Windows.Forms.ToolStripButton();
      this.btnDependentObjectManager = new System.Windows.Forms.ToolStripButton();
      this.btnLog = new System.Windows.Forms.ToolStripButton();
      this.btnWebDownloader = new System.Windows.Forms.ToolStripButton();
      this.btnClearSqlCache = new System.Windows.Forms.ToolStripButton();
      this.menuStrip1.SuspendLayout();
      this.stripBtns.SuspendLayout();
      this.SuspendLayout();
      // 
      // splitter1
      // 
      this.splitter1.Location = new System.Drawing.Point(0, 24);
      this.splitter1.Name = "splitter1";
      this.splitter1.Size = new System.Drawing.Size(3, 532);
      this.splitter1.TabIndex = 20;
      this.splitter1.TabStop = false;
      // 
      // menuStrip1
      // 
      this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuWindow});
      this.menuStrip1.Location = new System.Drawing.Point(0, 0);
      this.menuStrip1.MdiWindowListItem = this.mnuWindow;
      this.menuStrip1.Name = "menuStrip1";
      this.menuStrip1.Size = new System.Drawing.Size(1064, 24);
      this.menuStrip1.TabIndex = 22;
      this.menuStrip1.Text = "menuStrip1";
      // 
      // mnuFile
      // 
      this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuExit,
            this.toolStripSeparator1});
      this.mnuFile.Name = "mnuFile";
      this.mnuFile.Size = new System.Drawing.Size(37, 20);
      this.mnuFile.Text = "&File";
      // 
      // mnuExit
      // 
      this.mnuExit.Name = "mnuExit";
      this.mnuExit.Size = new System.Drawing.Size(92, 22);
      this.mnuExit.Text = "E&xit";
      this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(89, 6);
      // 
      // mnuEdit
      // 
      this.mnuEdit.Name = "mnuEdit";
      this.mnuEdit.Size = new System.Drawing.Size(39, 20);
      this.mnuEdit.Text = "&Edit";
      // 
      // mnuWindow
      // 
      this.mnuWindow.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuWHorizontal,
            this.mnuWVertical,
            this.mnuWCascade,
            this.mnuWArrange});
      this.mnuWindow.Name = "mnuWindow";
      this.mnuWindow.Size = new System.Drawing.Size(63, 20);
      this.mnuWindow.Text = "&Window";
      // 
      // mnuWHorizontal
      // 
      this.mnuWHorizontal.Name = "mnuWHorizontal";
      this.mnuWHorizontal.Size = new System.Drawing.Size(151, 22);
      this.mnuWHorizontal.Text = "Tile &Horizontal";
      this.mnuWHorizontal.Click += new System.EventHandler(this.mnuWHorizontal_Click);
      // 
      // mnuWVertical
      // 
      this.mnuWVertical.Name = "mnuWVertical";
      this.mnuWVertical.Size = new System.Drawing.Size(151, 22);
      this.mnuWVertical.Text = "Tile &Vertical";
      this.mnuWVertical.Click += new System.EventHandler(this.mnuWVertical_Click);
      // 
      // mnuWCascade
      // 
      this.mnuWCascade.Name = "mnuWCascade";
      this.mnuWCascade.Size = new System.Drawing.Size(151, 22);
      this.mnuWCascade.Text = "&Cascade";
      this.mnuWCascade.Click += new System.EventHandler(this.mnuWCascade_Click);
      // 
      // mnuWArrange
      // 
      this.mnuWArrange.Name = "mnuWArrange";
      this.mnuWArrange.Size = new System.Drawing.Size(151, 22);
      this.mnuWArrange.Text = "&Arrange Icons";
      this.mnuWArrange.Click += new System.EventHandler(this.mnuWArrange_Click);
      // 
      // stripWnds
      // 
      this.stripWnds.GripMargin = new System.Windows.Forms.Padding(2, 0, 2, 0);
      this.stripWnds.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.stripWnds.Location = new System.Drawing.Point(3, 49);
      this.stripWnds.Name = "stripWnds";
      this.stripWnds.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
      this.stripWnds.Size = new System.Drawing.Size(1061, 25);
      this.stripWnds.TabIndex = 26;
      this.stripWnds.Text = "toolStrip2";
      // 
      // stripBtns
      // 
      this.stripBtns.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.stripBtns.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDataDefinitionList,
            this.btnMemoryInUsed,
            this.btnDependentObjectManager,
            this.btnLog,
            this.btnWebDownloader,
            this.btnClearSqlCache
      });
      this.stripBtns.Location = new System.Drawing.Point(3, 24);
      this.stripBtns.Name = "stripBtns";
      this.stripBtns.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
      this.stripBtns.Size = new System.Drawing.Size(1061, 25);
      this.stripBtns.TabIndex = 28;
      this.stripBtns.Text = "toolStrip1";
      this.stripBtns.ItemAdded += new System.Windows.Forms.ToolStripItemEventHandler(this.mainMenu_ItemAdded);
      // 
      // btnDataDefinitionList
      // 
      this.btnDataDefinitionList.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnDataDefinitionList.Image = ((System.Drawing.Image)(resources.GetObject("btnDataDefinitionList.Image")));
      this.btnDataDefinitionList.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnDataDefinitionList.Name = "btnDataDefinitionList";
      this.btnDataDefinitionList.Size = new System.Drawing.Size(86, 22);
      this.btnDataDefinitionList.Text = "Список даних";
      this.btnDataDefinitionList.Click += new System.EventHandler(this.btnDataDefinitionList_Click);
      // 
      // btnMemoryInUsed
      // 
      this.btnMemoryInUsed.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnMemoryInUsed.Image = ((System.Drawing.Image)(resources.GetObject("btnMemoryInUsed.Image")));
      this.btnMemoryInUsed.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnMemoryInUsed.Name = "btnMemoryInUsed";
      this.btnMemoryInUsed.Size = new System.Drawing.Size(128, 22);
      this.btnMemoryInUsed.Text = "Використання памяті";
      this.btnMemoryInUsed.Click += new System.EventHandler(this.btnMemoryInUsed_Click);
      // 
      // btnDependentObjectManager
      // 
      this.btnDependentObjectManager.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnDependentObjectManager.Image = ((System.Drawing.Image)(resources.GetObject("btnDependentObjectManager.Image")));
      this.btnDependentObjectManager.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnDependentObjectManager.Name = "btnDependentObjectManager";
      this.btnDependentObjectManager.Size = new System.Drawing.Size(100, 22);
      this.btnDependentObjectManager.Text = "Залежні об`єкти";
      this.btnDependentObjectManager.Click += new System.EventHandler(this.btnDependentObjectManager_Click);
      // 
      // btnLog
      // 
      this.btnLog.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnLog.Name = "btnDependentObjectManager";
      this.btnLog.Size = new System.Drawing.Size(100, 22);
      this.btnLog.Text = "Журнал";
      this.btnLog.Click += new System.EventHandler(this.btnLog_Click);
      // 
      // btnWebDownloader
      // 
      this.btnWebDownloader.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnWebDownloader.Image = ((System.Drawing.Image)(resources.GetObject("btnWebDownloader.Image")));
      this.btnWebDownloader.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnWebDownloader.Name = "btnWebDownloader";
      this.btnWebDownloader.Size = new System.Drawing.Size(102, 22);
      this.btnWebDownloader.Text = "Web Downloader";
      this.btnWebDownloader.Click += new System.EventHandler(this.btnWebDownloader_Click);
      // 
      // btnClearSqlCache
      // 
      this.btnClearSqlCache.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnClearSqlCache.Name = "btnClearSqlCache";
      this.btnClearSqlCache.Size = new System.Drawing.Size(102, 22);
      this.btnClearSqlCache.Text = "Clear Sql Cache";
      this.btnClearSqlCache.Click += new System.EventHandler(this.btnClearSqlCache_Click);
      // 
      // frmMDI
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(1064, 556);
      this.Controls.Add(this.stripWnds);
      this.Controls.Add(this.stripBtns);
      this.Controls.Add(this.splitter1);
      this.Controls.Add(this.menuStrip1);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.IsMdiContainer = true;
      this.MainMenuStrip = this.menuStrip1;
      this.Name = "frmMDI";
      this.Text = "DGV Cube";
      this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
      this.Load += new System.EventHandler(this.frmMDI_Load);
      this.menuStrip1.ResumeLayout(false);
      this.menuStrip1.PerformLayout();
      this.stripBtns.ResumeLayout(false);
      this.stripBtns.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    //    private System.Windows.Forms.ContextMenu contextMenu1;
    /*    private System.Windows.Forms.MenuItem menuItem4;
        private System.Windows.Forms.MenuItem menuItem5;
        private System.Windows.Forms.MenuItem menuItem6;
        private System.Windows.Forms.MenuItem menuItem7;
        private System.Windows.Forms.MenuItem menuItem8;*/
    private System.Windows.Forms.Splitter splitter1;
    private System.Windows.Forms.MenuStrip menuStrip1;
    private System.Windows.Forms.ToolStripMenuItem mnuFile;
    private System.Windows.Forms.ToolStripMenuItem mnuExit;
    private System.Windows.Forms.ToolStripMenuItem mnuEdit;
    private System.Windows.Forms.ToolStripMenuItem mnuWindow;
    private System.Windows.Forms.ToolStripMenuItem mnuWHorizontal;
    private System.Windows.Forms.ToolStripMenuItem mnuWVertical;
    private System.Windows.Forms.ToolStripMenuItem mnuWCascade;
    private System.Windows.Forms.ToolStripMenuItem mnuWArrange;
    private System.Windows.Forms.ToolStrip stripWnds;
    private System.Windows.Forms.ToolStrip stripBtns;
    //    private System.Windows.Forms.ToolStripMenuItem testToolStripMenuItem1;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripButton btnDataDefinitionList;
    private System.Windows.Forms.ToolStripButton btnMemoryInUsed;
    private System.Windows.Forms.ToolStripButton btnDependentObjectManager;
    private System.Windows.Forms.ToolStripButton btnLog;
    private System.Windows.Forms.ToolStripButton btnWebDownloader;
    private System.Windows.Forms.ToolStripButton btnClearSqlCache;
  }
}