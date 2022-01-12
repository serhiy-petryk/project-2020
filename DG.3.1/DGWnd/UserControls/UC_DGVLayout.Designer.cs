namespace DGWnd.UserControls {
  partial class UC_DGVLayout {
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

    #region Component Designer generated code

    /// <summary> 
    /// Required method for Designer support - do not modify 
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UC_DGVLayout));
      this.tabFilterDB = new System.Windows.Forms.TabControl();
      this.tabColumnList = new System.Windows.Forms.TabPage();
      this.ucColumnSettings = new UC_ColumnSettings();
      this.tabFilterItems = new System.Windows.Forms.TabPage();
      this.ucItemFilter = new UserControls.UC_Filter();
      this.btnApply = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.toolStrip2 = new System.Windows.Forms.ToolStrip();
      this.btnClearFilter = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
      this.btnItemsCode = new System.Windows.Forms.ToolStripButton();
      this.tabFilterDB.SuspendLayout();
      this.tabColumnList.SuspendLayout();
      this.tabFilterItems.SuspendLayout();
      this.toolStrip2.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabFilterDB
      // 
      this.tabFilterDB.Controls.Add(this.tabColumnList);
      this.tabFilterDB.Controls.Add(this.tabFilterItems);
      this.tabFilterDB.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabFilterDB.Location = new System.Drawing.Point(0, 25);
      this.tabFilterDB.Name = "tabFilterDB";
      this.tabFilterDB.SelectedIndex = 0;
      this.tabFilterDB.Size = new System.Drawing.Size(710, 400);
      this.tabFilterDB.TabIndex = 1;
      // 
      // tabColumnList
      // 
      this.tabColumnList.Controls.Add(this.ucColumnSettings);
      this.tabColumnList.Location = new System.Drawing.Point(4, 24);
      this.tabColumnList.Name = "tabColumnList";
      this.tabColumnList.Padding = new System.Windows.Forms.Padding(3);
      this.tabColumnList.Size = new System.Drawing.Size(702, 372);
      this.tabColumnList.TabIndex = 1;
      this.tabColumnList.Text = "Колонки";
      this.tabColumnList.UseVisualStyleBackColor = true;
      // 
      // ucColumnSettings
      // 
      this.ucColumnSettings.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucColumnSettings.Location = new System.Drawing.Point(3, 3);
      this.ucColumnSettings.Name = "ucColumnSettings";
      this.ucColumnSettings.Size = new System.Drawing.Size(696, 366);
      this.ucColumnSettings.TabIndex = 0;
      // 
      // tabFilterItems
      // 
      this.tabFilterItems.Controls.Add(this.ucItemFilter);
      this.tabFilterItems.Location = new System.Drawing.Point(4, 24);
      this.tabFilterItems.Name = "tabFilterItems";
      this.tabFilterItems.Padding = new System.Windows.Forms.Padding(3);
      this.tabFilterItems.Size = new System.Drawing.Size(700, 427);
      this.tabFilterItems.TabIndex = 2;
      this.tabFilterItems.Text = "Внутрішній фільтр даних";
      this.tabFilterItems.UseVisualStyleBackColor = true;
      // 
      // ucItemFilter
      // 
      this.ucItemFilter.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucItemFilter.Location = new System.Drawing.Point(3, 3);
      this.ucItemFilter.Name = "ucItemFilter";
      this.ucItemFilter.Size = new System.Drawing.Size(694, 421);
      this.ucItemFilter.TabIndex = 0;
      // 
      // btnApply
      // 
      this.btnApply.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnApply.Image = ((System.Drawing.Image)(resources.GetObject("btnApply.Image")));
      this.btnApply.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnApply.Margin = new System.Windows.Forms.Padding(8, 1, 0, 2);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new System.Drawing.Size(72, 22);
      this.btnApply.Text = "Примінити";
      this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
      // 
      // toolStrip2
      // 
      this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnApply,
            this.toolStripSeparator1,
            this.btnClearFilter,
            this.toolStripSeparator3,
            this.btnItemsCode});
      this.toolStrip2.Location = new System.Drawing.Point(0, 0);
      this.toolStrip2.Name = "toolStrip2";
      this.toolStrip2.Size = new System.Drawing.Size(710, 25);
      this.toolStrip2.TabIndex = 14;
      this.toolStrip2.Text = "toolStrip2";
      // 
      // btnClearFilter
      // 
      this.btnClearFilter.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnClearFilter.Image = ((System.Drawing.Image)(resources.GetObject("btnClearFilter.Image")));
      this.btnClearFilter.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnClearFilter.Name = "btnClearFilter";
      this.btnClearFilter.Size = new System.Drawing.Size(104, 22);
      this.btnClearFilter.Text = "Очистити фільтр";
      this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
      // 
      // toolStripSeparator3
      // 
      this.toolStripSeparator3.Name = "toolStripSeparator3";
      this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
      // 
      // btnItemsCode
      // 
      this.btnItemsCode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnItemsCode.Image = ((System.Drawing.Image)(resources.GetObject("btnItemsCode.Image")));
      this.btnItemsCode.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnItemsCode.Name = "btnItemsCode";
      this.btnItemsCode.Size = new System.Drawing.Size(23, 22);
      this.btnItemsCode.Text = "btnItemsCode";
      this.btnItemsCode.Click += new System.EventHandler(this.btnItemsCode_Click);
      // 
      // UC_DGVLayout
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.tabFilterDB);
      this.Controls.Add(this.toolStrip2);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.Name = "UC_DGVLayout";
      this.Size = new System.Drawing.Size(710, 425);
      this.tabFilterDB.ResumeLayout(false);
      this.tabColumnList.ResumeLayout(false);
      this.tabFilterItems.ResumeLayout(false);
      this.toolStrip2.ResumeLayout(false);
      this.toolStrip2.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TabControl tabFilterDB;
    private System.Windows.Forms.TabPage tabColumnList;
    private System.Windows.Forms.TabPage tabFilterItems;
    private UC_ColumnSettings ucColumnSettings;
    private System.Windows.Forms.ToolStripButton btnApply;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStrip toolStrip2;
    private UC_Filter ucItemFilter;
    private System.Windows.Forms.ToolStripButton btnClearFilter;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
    private System.Windows.Forms.ToolStripButton btnItemsCode;
  }
}
