namespace DGWnd.UserControls {
  partial class UC_DBFilter {
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
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UC_DBFilter));
      this.toolStrip2 = new System.Windows.Forms.ToolStrip();
      this.btnApply = new System.Windows.Forms.ToolStripButton();
      this.sepApply = new System.Windows.Forms.ToolStripSeparator();
      this.btnSelectLayout = new System.Windows.Forms.ToolStripSplitButton();
      this.btnSaveLayout = new System.Windows.Forms.ToolStripButton();
      this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
      this.btnClear = new System.Windows.Forms.ToolStripButton();
      this.btnItemsCode = new System.Windows.Forms.ToolStripButton();
      this.ucFilter = new UserControls.UC_Filter();
      this.toolStrip2.SuspendLayout();
      this.SuspendLayout();
      // 
      // toolStrip2
      // 
      this.toolStrip2.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnApply,
            this.sepApply,
            this.btnSelectLayout,
            this.btnSaveLayout,
            this.toolStripSeparator1,
            this.btnClear,
            this.btnItemsCode});
      this.toolStrip2.Location = new System.Drawing.Point(0, 0);
      this.toolStrip2.Name = "toolStrip2";
      this.toolStrip2.Size = new System.Drawing.Size(855, 25);
      this.toolStrip2.TabIndex = 13;
      this.toolStrip2.Text = "toolStrip2";
      // 
      // btnApply
      // 
      this.btnApply.Image = global::DGWnd.Properties.Resources.clock;
      this.btnApply.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnApply.Margin = new System.Windows.Forms.Padding(8, 1, 0, 2);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new System.Drawing.Size(123, 22);
      this.btnApply.Text = "Завантажити дані";
      this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
      // 
      // sepApply
      // 
      this.sepApply.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
      this.sepApply.Name = "sepApply";
      this.sepApply.Size = new System.Drawing.Size(6, 25);
      // 
      // btnSelectLayout
      // 
      this.btnSelectLayout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnSelectLayout.Image = global::DGWnd.Properties.Resources.OpenFile;
      this.btnSelectLayout.ImageTransparentColor = System.Drawing.Color.White;
      this.btnSelectLayout.Name = "btnSelectLayout";
      this.btnSelectLayout.Size = new System.Drawing.Size(32, 22);
      this.btnSelectLayout.Text = "Вибрати налаштування";
      this.btnSelectLayout.ButtonClick += new System.EventHandler(this.btnSelectLayout_Click);
      this.btnSelectLayout.DropDownOpening += new System.EventHandler(this.btnSelectLayout_DropDownOpening);
      this.btnSelectLayout.DropDownItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.btnSelectLayout_DropDownItemClicked);
      // 
      // btnSaveLayout
      // 
      this.btnSaveLayout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
      this.btnSaveLayout.Image = global::DGWnd.Properties.Resources.SaveFormDesignHS;
      this.btnSaveLayout.ImageTransparentColor = System.Drawing.Color.White;
      this.btnSaveLayout.Name = "btnSaveLayout";
      this.btnSaveLayout.Size = new System.Drawing.Size(23, 22);
      this.btnSaveLayout.Text = "Записати налаштування";
      this.btnSaveLayout.Click += new System.EventHandler(this.btnSaveLayout_Click);
      // 
      // toolStripSeparator1
      // 
      this.toolStripSeparator1.Name = "toolStripSeparator1";
      this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
      // 
      // btnClear
      // 
      this.btnClear.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
      this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
      this.btnClear.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new System.Drawing.Size(104, 22);
      this.btnClear.Text = "Очистити фільтр";
      this.btnClear.ToolTipText = "Очистити фільтр";
      this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
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
      // ucFilter
      // 
      this.ucFilter.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucFilter.Location = new System.Drawing.Point(0, 25);
      this.ucFilter.Name = "ucFilter";
      this.ucFilter.Size = new System.Drawing.Size(855, 482);
      this.ucFilter.TabIndex = 14;
      // 
      // UC_DBFilter
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.ucFilter);
      this.Controls.Add(this.toolStrip2);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.Name = "UC_DBFilter";
      this.Size = new System.Drawing.Size(855, 507);
      this.toolStrip2.ResumeLayout(false);
      this.toolStrip2.PerformLayout();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.ToolStrip toolStrip2;
    private System.Windows.Forms.ToolStripButton btnApply;
    private System.Windows.Forms.ToolStripSeparator sepApply;
    private UC_Filter ucFilter;
    private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
    private System.Windows.Forms.ToolStripButton btnClear;
    private System.Windows.Forms.ToolStripButton btnItemsCode;
    private System.Windows.Forms.ToolStripButton btnSaveLayout;
    private System.Windows.Forms.ToolStripSplitButton btnSelectLayout;
  }
}
