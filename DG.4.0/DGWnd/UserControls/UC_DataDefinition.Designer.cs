namespace DGWnd.UserControls {
  partial class UCDataDefinition {
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UCDataDefinition));
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.tlpDataDefinition = new System.Windows.Forms.TableLayoutPanel();
            this.label2 = new System.Windows.Forms.Label();
            this.treeView = new DGWnd.Misc.TreeViewOnDemand();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.tlpParameter = new System.Windows.Forms.TableLayoutPanel();
            this.lblFilter = new System.Windows.Forms.Label();
            this.pg = new System.Windows.Forms.PropertyGrid();
            this.lblError = new System.Windows.Forms.Label();
            this.ucFilterDB = new DGWnd.UserControls.UC_DBFilter();
            this.lblDataSettingName = new System.Windows.Forms.Label();
            this.cbDataSettingName = new System.Windows.Forms.ComboBox();
            this.cmTreeView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.miTree_Open = new System.Windows.Forms.ToolStripMenuItem();
            this.miTree_Settings = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tlpDataDefinition.SuspendLayout();
            this.tlpParameter.SuspendLayout();
            this.cmTreeView.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.tlpDataDefinition);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.tlpParameter);
            this.splitContainer2.Size = new System.Drawing.Size(969, 480);
            this.splitContainer2.SplitterDistance = 322;
            this.splitContainer2.TabIndex = 15;
            // 
            // tlpDataDefinition
            // 
            this.tlpDataDefinition.ColumnCount = 1;
            this.tlpDataDefinition.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDataDefinition.Controls.Add(this.label2, 0, 0);
            this.tlpDataDefinition.Controls.Add(this.treeView, 0, 1);
            this.tlpDataDefinition.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpDataDefinition.Location = new System.Drawing.Point(0, 0);
            this.tlpDataDefinition.Name = "tlpDataDefinition";
            this.tlpDataDefinition.RowCount = 2;
            this.tlpDataDefinition.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpDataDefinition.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpDataDefinition.Size = new System.Drawing.Size(322, 480);
            this.tlpDataDefinition.TabIndex = 0;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(316, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Список задач";
            this.label2.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.treeView.HideSelection = false;
            this.treeView.ImageIndex = 0;
            this.treeView.ImageList = this.imageList1;
            this.treeView.Location = new System.Drawing.Point(3, 19);
            this.treeView.Name = "treeView";
            this.treeView.SelectedImageIndex = 0;
            this.treeView.Size = new System.Drawing.Size(316, 458);
            this.treeView.TabIndex = 2;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseClick);
            this.treeView.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView_NodeMouseDoubleClick);
            // 
            // imageList1
            // 
            this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "folder.bmp");
            this.imageList1.Images.SetKeyName(1, "doc16.ico");
            // 
            // tlpParameter
            // 
            this.tlpParameter.ColumnCount = 2;
            this.tlpParameter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpParameter.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpParameter.Controls.Add(this.lblFilter, 0, 1);
            this.tlpParameter.Controls.Add(this.pg, 0, 4);
            this.tlpParameter.Controls.Add(this.lblError, 0, 3);
            this.tlpParameter.Controls.Add(this.ucFilterDB, 0, 2);
            this.tlpParameter.Controls.Add(this.lblDataSettingName, 0, 0);
            this.tlpParameter.Controls.Add(this.cbDataSettingName, 1, 0);
            this.tlpParameter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpParameter.Location = new System.Drawing.Point(0, 0);
            this.tlpParameter.Name = "tlpParameter";
            this.tlpParameter.RowCount = 5;
            this.tlpParameter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpParameter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpParameter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpParameter.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpParameter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpParameter.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpParameter.Size = new System.Drawing.Size(643, 480);
            this.tlpParameter.TabIndex = 0;
            // 
            // lblFilter
            // 
            this.lblFilter.AutoSize = true;
            this.tlpParameter.SetColumnSpan(this.lblFilter, 2);
            this.lblFilter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblFilter.Location = new System.Drawing.Point(3, 31);
            this.lblFilter.Name = "lblFilter";
            this.lblFilter.Size = new System.Drawing.Size(637, 16);
            this.lblFilter.TabIndex = 1;
            this.lblFilter.Text = "Фільтр";
            this.lblFilter.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // pg
            // 
            this.tlpParameter.SetColumnSpan(this.pg, 2);
            this.pg.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pg.LineColor = System.Drawing.SystemColors.ControlDark;
            this.pg.Location = new System.Drawing.Point(3, 274);
            this.pg.Name = "pg";
            this.pg.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            this.pg.Size = new System.Drawing.Size(637, 203);
            this.pg.TabIndex = 3;
            this.pg.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pg_PropertyValueChanged);
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.tlpParameter.SetColumnSpan(this.lblError, 2);
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblError.ForeColor = System.Drawing.Color.Red;
            this.lblError.Location = new System.Drawing.Point(3, 255);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(50, 16);
            this.lblError.TabIndex = 4;
            this.lblError.Text = "label1";
            // 
            // ucFilterDB
            // 
            this.tlpParameter.SetColumnSpan(this.ucFilterDB, 2);
            this.ucFilterDB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ucFilterDB.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.ucFilterDB.Location = new System.Drawing.Point(3, 50);
            this.ucFilterDB.Name = "ucFilterDB";
            this.ucFilterDB.Size = new System.Drawing.Size(637, 202);
            this.ucFilterDB.TabIndex = 5;
            // 
            // lblDataSettingName
            // 
            this.lblDataSettingName.AutoSize = true;
            this.lblDataSettingName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblDataSettingName.Location = new System.Drawing.Point(3, 0);
            this.lblDataSettingName.Name = "lblDataSettingName";
            this.lblDataSettingName.Size = new System.Drawing.Size(145, 31);
            this.lblDataSettingName.TabIndex = 8;
            this.lblDataSettingName.Text = "Налаштування даних";
            this.lblDataSettingName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbDataSettingName
            // 
            this.cbDataSettingName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cbDataSettingName.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cbDataSettingName.FormattingEnabled = true;
            this.cbDataSettingName.Location = new System.Drawing.Point(154, 3);
            this.cbDataSettingName.MaxDropDownItems = 22;
            this.cbDataSettingName.Name = "cbDataSettingName";
            this.cbDataSettingName.Size = new System.Drawing.Size(486, 24);
            this.cbDataSettingName.TabIndex = 9;
            // 
            // cmTreeView
            // 
            this.cmTreeView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miTree_Open,
            this.miTree_Settings});
            this.cmTreeView.Name = "cmTreeView";
            this.cmTreeView.Size = new System.Drawing.Size(190, 48);
            // 
            // miTree_Open
            // 
            this.miTree_Open.Name = "miTree_Open";
            this.miTree_Open.Size = new System.Drawing.Size(189, 22);
            this.miTree_Open.Text = "Відкрити набір даних";
            this.miTree_Open.Click += new System.EventHandler(this.miTree_Open_Click);
            // 
            // miTree_Settings
            // 
            this.miTree_Settings.Name = "miTree_Settings";
            this.miTree_Settings.Size = new System.Drawing.Size(189, 22);
            this.miTree_Settings.Text = "Налаштування";
            this.miTree_Settings.Click += new System.EventHandler(this.miTree_Settings_Click);
            // 
            // UCDataDefinition
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "UCDataDefinition";
            this.Size = new System.Drawing.Size(969, 480);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tlpDataDefinition.ResumeLayout(false);
            this.tlpDataDefinition.PerformLayout();
            this.tlpParameter.ResumeLayout(false);
            this.tlpParameter.PerformLayout();
            this.cmTreeView.ResumeLayout(false);
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.SplitContainer splitContainer2;
    private System.Windows.Forms.TableLayoutPanel tlpDataDefinition;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TableLayoutPanel tlpParameter;
    private System.Windows.Forms.Label lblFilter;
    private System.Windows.Forms.PropertyGrid pg;
    private System.Windows.Forms.Label lblError;
    private Misc.TreeViewOnDemand treeView;
    private System.Windows.Forms.ImageList imageList1;
    private UC_DBFilter ucFilterDB;
    private System.Windows.Forms.Label lblDataSettingName;
    private System.Windows.Forms.ComboBox cbDataSettingName;
    private System.Windows.Forms.ContextMenuStrip cmTreeView;
    private System.Windows.Forms.ToolStripMenuItem miTree_Open;
    private System.Windows.Forms.ToolStripMenuItem miTree_Settings;
  }
}
