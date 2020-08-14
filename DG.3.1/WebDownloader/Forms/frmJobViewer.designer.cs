namespace WebDownloader {
  partial class frmJobViewer {
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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      this.pnlStatus = new System.Windows.Forms.Panel();
      this.stripStatus = new System.Windows.Forms.StatusStrip();
      this.lblStatusShort = new System.Windows.Forms.ToolStripStatusLabel();
      this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
      this.pnlStatusRight = new System.Windows.Forms.Panel();
      this.stripStatusRight = new System.Windows.Forms.StatusStrip();
      this.pbStatus = new System.Windows.Forms.ToolStripProgressBar();
      this.lblProgressBar = new System.Windows.Forms.ToolStripStatusLabel();
      this.grid = new System.Windows.Forms.DataGridView();
      this.Img = new System.Windows.Forms.DataGridViewImageColumn();
      this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.panel1 = new System.Windows.Forms.Panel();
      this.stripUp = new System.Windows.Forms.ToolStrip();
      this.btnRun = new System.Windows.Forms.ToolStripButton();
      this.cbBreakOnError = new System.Windows.Forms.ToolStripComboBox();
      this.btnPause = new System.Windows.Forms.ToolStripButton();
      this.btnResume = new System.Windows.Forms.ToolStripButton();
      this.btnCancel = new System.Windows.Forms.ToolStripButton();
      this.btnOpenTraceFile = new System.Windows.Forms.ToolStripButton();
      this.cbOutputMode = new System.Windows.Forms.ToolStripDropDownButton();
      this.btnModeScroll = new System.Windows.Forms.ToolStripMenuItem();
      this.btnModeDirect = new System.Windows.Forms.ToolStripMenuItem();
      this.btnModeReverse = new System.Windows.Forms.ToolStripMenuItem();
      this.dtpRunAt = new System.Windows.Forms.DateTimePicker();
      this.label1 = new System.Windows.Forms.Label();
      this.pnlStatus.SuspendLayout();
      this.stripStatus.SuspendLayout();
      this.pnlStatusRight.SuspendLayout();
      this.stripStatusRight.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.grid)).BeginInit();
      this.panel1.SuspendLayout();
      this.stripUp.SuspendLayout();
      this.SuspendLayout();
      // 
      // pnlStatus
      // 
      this.pnlStatus.Controls.Add(this.stripStatus);
      this.pnlStatus.Controls.Add(this.pnlStatusRight);
      this.pnlStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.pnlStatus.Location = new System.Drawing.Point(0, 408);
      this.pnlStatus.Name = "pnlStatus";
      this.pnlStatus.Size = new System.Drawing.Size(854, 22);
      this.pnlStatus.TabIndex = 29;
      // 
      // stripStatus
      // 
      this.stripStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatusShort,
            this.lblStatus});
      this.stripStatus.Location = new System.Drawing.Point(0, 0);
      this.stripStatus.Name = "stripStatus";
      this.stripStatus.Size = new System.Drawing.Size(604, 22);
      this.stripStatus.SizingGrip = false;
      this.stripStatus.TabIndex = 2;
      this.stripStatus.Text = "statusStrip2";
      // 
      // lblStatusShort
      // 
      this.lblStatusShort.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
      this.lblStatusShort.Name = "lblStatusShort";
      this.lblStatusShort.Size = new System.Drawing.Size(58, 18);
      this.lblStatusShort.Text = "Unknown";
      // 
      // lblStatus
      // 
      this.lblStatus.Name = "lblStatus";
      this.lblStatus.Size = new System.Drawing.Size(118, 17);
      this.lblStatus.Text = "toolStripStatusLabel1";
      // 
      // pnlStatusRight
      // 
      this.pnlStatusRight.Controls.Add(this.stripStatusRight);
      this.pnlStatusRight.Dock = System.Windows.Forms.DockStyle.Right;
      this.pnlStatusRight.Location = new System.Drawing.Point(604, 0);
      this.pnlStatusRight.Name = "pnlStatusRight";
      this.pnlStatusRight.Size = new System.Drawing.Size(250, 22);
      this.pnlStatusRight.TabIndex = 1;
      // 
      // stripStatusRight
      // 
      this.stripStatusRight.Dock = System.Windows.Forms.DockStyle.Fill;
      this.stripStatusRight.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pbStatus,
            this.lblProgressBar});
      this.stripStatusRight.Location = new System.Drawing.Point(0, 0);
      this.stripStatusRight.Name = "stripStatusRight";
      this.stripStatusRight.Size = new System.Drawing.Size(250, 22);
      this.stripStatusRight.TabIndex = 0;
      this.stripStatusRight.Text = "statusStrip1";
      // 
      // pbStatus
      // 
      this.pbStatus.Margin = new System.Windows.Forms.Padding(2, 4, 2, 0);
      this.pbStatus.Name = "pbStatus";
      this.pbStatus.Size = new System.Drawing.Size(100, 18);
      // 
      // lblProgressBar
      // 
      this.lblProgressBar.Margin = new System.Windows.Forms.Padding(0, 4, 0, 0);
      this.lblProgressBar.Name = "lblProgressBar";
      this.lblProgressBar.Size = new System.Drawing.Size(41, 18);
      this.lblProgressBar.Text = "Label2";
      // 
      // grid
      // 
      this.grid.AllowUserToAddRows = false;
      this.grid.AllowUserToDeleteRows = false;
      this.grid.AllowUserToResizeColumns = false;
      this.grid.AllowUserToResizeRows = false;
      this.grid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCellsExceptHeader;
      this.grid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;
      this.grid.BackgroundColor = System.Drawing.SystemColors.Control;
      this.grid.BorderStyle = System.Windows.Forms.BorderStyle.None;
      this.grid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
      this.grid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.grid.ColumnHeadersVisible = false;
      this.grid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Img,
            this.Time,
            this.Message});
      dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(0, 1, 0, 1);
      dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.MistyRose;
      dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
      dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.grid.DefaultCellStyle = dataGridViewCellStyle3;
      this.grid.Dock = System.Windows.Forms.DockStyle.Fill;
      this.grid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
      this.grid.Location = new System.Drawing.Point(0, 28);
      this.grid.Margin = new System.Windows.Forms.Padding(2);
      this.grid.Name = "grid";
      this.grid.ReadOnly = true;
      this.grid.RowHeadersVisible = false;
      this.grid.RowTemplate.Resizable = System.Windows.Forms.DataGridViewTriState.True;
      this.grid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
      this.grid.Size = new System.Drawing.Size(854, 380);
      this.grid.TabIndex = 31;
      // 
      // Img
      // 
      this.Img.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
      this.Img.HeaderText = "Image";
      this.Img.MinimumWidth = 16;
      this.Img.Name = "Img";
      this.Img.ReadOnly = true;
      this.Img.Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.Img.Width = 16;
      // 
      // Time
      // 
      this.Time.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
      this.Time.DataPropertyName = "Time";
      dataGridViewCellStyle1.Format = "T";
      dataGridViewCellStyle1.NullValue = null;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.Time.DefaultCellStyle = dataGridViewCellStyle1;
      this.Time.HeaderText = "Time";
      this.Time.Name = "Time";
      this.Time.ReadOnly = true;
      this.Time.Width = 5;
      // 
      // Message
      // 
      this.Message.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
      this.Message.DataPropertyName = "Message";
      dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.Message.DefaultCellStyle = dataGridViewCellStyle2;
      this.Message.HeaderText = "Message";
      this.Message.MinimumWidth = 100;
      this.Message.Name = "Message";
      this.Message.ReadOnly = true;
      this.Message.Resizable = System.Windows.Forms.DataGridViewTriState.False;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.stripUp);
      this.panel1.Controls.Add(this.dtpRunAt);
      this.panel1.Controls.Add(this.label1);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
      this.panel1.Location = new System.Drawing.Point(0, 0);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(854, 28);
      this.panel1.TabIndex = 32;
      // 
      // stripUp
      // 
      this.stripUp.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
      this.stripUp.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRun,
            this.cbBreakOnError,
            this.btnPause,
            this.btnResume,
            this.btnCancel,
            this.btnOpenTraceFile,
            this.cbOutputMode});
      this.stripUp.Location = new System.Drawing.Point(168, 0);
      this.stripUp.Name = "stripUp";
      this.stripUp.Size = new System.Drawing.Size(686, 25);
      this.stripUp.TabIndex = 29;
      this.stripUp.Text = "toolStrip1";
      // 
      // btnRun
      // 
      this.btnRun.BackColor = System.Drawing.SystemColors.Control;
      this.btnRun.Image = global::WebDownloader.Properties.Resources.imgBtnStandard;
      this.btnRun.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnRun.Name = "btnRun";
      this.btnRun.Size = new System.Drawing.Size(72, 22);
      this.btnRun.Text = "Run task";
      // 
      // cbBreakOnError
      // 
      this.cbBreakOnError.Items.AddRange(new object[] {
            "Break on Error",
            "Continue on Error"});
      this.cbBreakOnError.Margin = new System.Windows.Forms.Padding(2, 0, 1, 0);
      this.cbBreakOnError.Name = "cbBreakOnError";
      this.cbBreakOnError.Size = new System.Drawing.Size(110, 25);
      // 
      // btnPause
      // 
      this.btnPause.Image = global::WebDownloader.Properties.Resources.imgBtnStandard;
      this.btnPause.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnPause.Margin = new System.Windows.Forms.Padding(12, 1, 0, 2);
      this.btnPause.Name = "btnPause";
      this.btnPause.Size = new System.Drawing.Size(58, 22);
      this.btnPause.Text = "Pause";
      // 
      // btnResume
      // 
      this.btnResume.Image = global::WebDownloader.Properties.Resources.imgBtnStandard;
      this.btnResume.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnResume.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
      this.btnResume.Name = "btnResume";
      this.btnResume.Size = new System.Drawing.Size(69, 22);
      this.btnResume.Text = "Resume";
      // 
      // btnCancel
      // 
      this.btnCancel.Image = global::WebDownloader.Properties.Resources.imgBtnStandard;
      this.btnCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnCancel.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(63, 22);
      this.btnCancel.Text = "Cancel";
      // 
      // btnOpenTraceFile
      // 
      this.btnOpenTraceFile.Image = global::WebDownloader.Properties.Resources.imgBtnStandard;
      this.btnOpenTraceFile.ImageTransparentColor = System.Drawing.Color.Magenta;
      this.btnOpenTraceFile.Margin = new System.Windows.Forms.Padding(12, 1, 0, 2);
      this.btnOpenTraceFile.Name = "btnOpenTraceFile";
      this.btnOpenTraceFile.Size = new System.Drawing.Size(108, 22);
      this.btnOpenTraceFile.Text = "Open trace flag";
      // 
      // cbOutputMode
      // 
      this.cbOutputMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnModeScroll,
            this.btnModeDirect,
            this.btnModeReverse});
      this.cbOutputMode.Image = global::WebDownloader.Properties.Resources.imgBtnStandard;
      this.cbOutputMode.Margin = new System.Windows.Forms.Padding(2, 1, 0, 2);
      this.cbOutputMode.Name = "cbOutputMode";
      this.cbOutputMode.Size = new System.Drawing.Size(108, 22);
      this.cbOutputMode.Text = "Output mode";
      // 
      // btnModeScroll
      // 
      this.btnModeScroll.Name = "btnModeScroll";
      this.btnModeScroll.Size = new System.Drawing.Size(145, 22);
      this.btnModeScroll.Tag = "1";
      this.btnModeScroll.Text = "Scroll";
      this.btnModeScroll.Click += new System.EventHandler(this.btnMode_Click);
      // 
      // btnModeDirect
      // 
      this.btnModeDirect.Name = "btnModeDirect";
      this.btnModeDirect.Size = new System.Drawing.Size(145, 22);
      this.btnModeDirect.Tag = "2";
      this.btnModeDirect.Text = "Direct order";
      this.btnModeDirect.Click += new System.EventHandler(this.btnMode_Click);
      // 
      // btnModeReverse
      // 
      this.btnModeReverse.Name = "btnModeReverse";
      this.btnModeReverse.Size = new System.Drawing.Size(145, 22);
      this.btnModeReverse.Tag = "3";
      this.btnModeReverse.Text = "Reverse order";
      this.btnModeReverse.Click += new System.EventHandler(this.btnMode_Click);
      // 
      // dtpRunAt
      // 
      this.dtpRunAt.CustomFormat = "yyyy-MM-dd HH:mm:ss";
      this.dtpRunAt.Dock = System.Windows.Forms.DockStyle.Left;
      this.dtpRunAt.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
      this.dtpRunAt.Location = new System.Drawing.Point(39, 0);
      this.dtpRunAt.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
      this.dtpRunAt.Name = "dtpRunAt";
      this.dtpRunAt.Size = new System.Drawing.Size(129, 20);
      this.dtpRunAt.TabIndex = 30;
      // 
      // label1
      // 
      this.label1.AutoSize = true;
      this.label1.Dock = System.Windows.Forms.DockStyle.Left;
      this.label1.Location = new System.Drawing.Point(0, 0);
      this.label1.Margin = new System.Windows.Forms.Padding(0);
      this.label1.Name = "label1";
      this.label1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
      this.label1.Size = new System.Drawing.Size(39, 18);
      this.label1.TabIndex = 31;
      this.label1.Text = "Run at";
      // 
      // frmJobViewer
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.SystemColors.Control;
      this.ClientSize = new System.Drawing.Size(854, 430);
      this.Controls.Add(this.grid);
      this.Controls.Add(this.panel1);
      this.Controls.Add(this.pnlStatus);
      this.Name = "frmJobViewer";
      this.Text = "frmJobViewer";
      this.pnlStatus.ResumeLayout(false);
      this.pnlStatus.PerformLayout();
      this.stripStatus.ResumeLayout(false);
      this.stripStatus.PerformLayout();
      this.pnlStatusRight.ResumeLayout(false);
      this.pnlStatusRight.PerformLayout();
      this.stripStatusRight.ResumeLayout(false);
      this.stripStatusRight.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.grid)).EndInit();
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      this.stripUp.ResumeLayout(false);
      this.stripUp.PerformLayout();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel pnlStatus;
    private System.Windows.Forms.StatusStrip stripStatus;
    private System.Windows.Forms.Panel pnlStatusRight;
    private System.Windows.Forms.StatusStrip stripStatusRight;
    private System.Windows.Forms.ToolStripProgressBar pbStatus;
    private System.Windows.Forms.ToolStripStatusLabel lblProgressBar;
    private System.Windows.Forms.DataGridViewImageColumn Img;
    private System.Windows.Forms.DataGridViewTextBoxColumn Time;
    private System.Windows.Forms.DataGridViewTextBoxColumn Message;
    private System.Windows.Forms.DataGridView grid;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.ToolStrip stripUp;
    public System.Windows.Forms.ToolStripButton btnRun;
    public System.Windows.Forms.ToolStripComboBox cbBreakOnError;
    public System.Windows.Forms.ToolStripButton btnPause;
    public System.Windows.Forms.ToolStripButton btnResume;
    public System.Windows.Forms.ToolStripButton btnCancel;
    public System.Windows.Forms.ToolStripButton btnOpenTraceFile;
    private System.Windows.Forms.ToolStripDropDownButton cbOutputMode;
    private System.Windows.Forms.ToolStripMenuItem btnModeScroll;
    private System.Windows.Forms.ToolStripMenuItem btnModeDirect;
    private System.Windows.Forms.ToolStripMenuItem btnModeReverse;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    public System.Windows.Forms.DateTimePicker dtpRunAt;
    public System.Windows.Forms.ToolStripStatusLabel lblStatusShort;

  }
}