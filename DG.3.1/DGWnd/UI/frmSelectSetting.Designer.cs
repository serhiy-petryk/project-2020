namespace DGWnd.UI {
  partial class frmSelectSetting {
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
      this.components = new System.ComponentModel.Container();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      this.dataGridView1 = new System.Windows.Forms.DataGridView();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnApply = new System.Windows.Forms.Button();
      this.settingIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.allowViewOthersDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.allowEditOthersDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
      this.createdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dCreatedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.updatedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dUpdatedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.bs = new System.Windows.Forms.BindingSource(this.components);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.bs)).BeginInit();
      this.SuspendLayout();
      // 
      // dataGridView1
      // 
      this.dataGridView1.AllowUserToAddRows = false;
      this.dataGridView1.AllowUserToDeleteRows = false;
      this.dataGridView1.AutoGenerateColumns = false;
      this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.settingIDDataGridViewTextBoxColumn,
            this.allowViewOthersDataGridViewCheckBoxColumn,
            this.allowEditOthersDataGridViewCheckBoxColumn,
            this.createdDataGridViewTextBoxColumn,
            this.dCreatedDataGridViewTextBoxColumn,
            this.updatedDataGridViewTextBoxColumn,
            this.dUpdatedDataGridViewTextBoxColumn});
      this.dataGridView1.DataSource = this.bs;
      this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dataGridView1.EnableHeadersVisualStyles = false;
      this.dataGridView1.Location = new System.Drawing.Point(0, 0);
      this.dataGridView1.MultiSelect = false;
      this.dataGridView1.Name = "dataGridView1";
      this.dataGridView1.ReadOnly = true;
      this.dataGridView1.RowHeadersVisible = false;
      this.dataGridView1.Size = new System.Drawing.Size(975, 582);
      this.dataGridView1.TabIndex = 0;
      this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnCancel);
      this.panel1.Controls.Add(this.btnApply);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 582);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(975, 40);
      this.panel1.TabIndex = 1;
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(598, 9);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(75, 23);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "Вийти";
      this.btnCancel.UseVisualStyleBackColor = true;
      // 
      // btnApply
      // 
      this.btnApply.Location = new System.Drawing.Point(378, 9);
      this.btnApply.Name = "btnApply";
      this.btnApply.Size = new System.Drawing.Size(112, 23);
      this.btnApply.TabIndex = 0;
      this.btnApply.Text = "Примінити";
      this.btnApply.UseVisualStyleBackColor = true;
      this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
      // 
      // settingIDDataGridViewTextBoxColumn
      // 
      this.settingIDDataGridViewTextBoxColumn.DataPropertyName = "SettingID";
      this.settingIDDataGridViewTextBoxColumn.HeaderText = "Налаштування";
      this.settingIDDataGridViewTextBoxColumn.Name = "settingIDDataGridViewTextBoxColumn";
      this.settingIDDataGridViewTextBoxColumn.ReadOnly = true;
      // 
      // allowViewOthersDataGridViewCheckBoxColumn
      // 
      this.allowViewOthersDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
      this.allowViewOthersDataGridViewCheckBoxColumn.DataPropertyName = "AllowViewOthers";
      this.allowViewOthersDataGridViewCheckBoxColumn.HeaderText = "Показувати іншим користувачам?";
      this.allowViewOthersDataGridViewCheckBoxColumn.Name = "allowViewOthersDataGridViewCheckBoxColumn";
      this.allowViewOthersDataGridViewCheckBoxColumn.ReadOnly = true;
      this.allowViewOthersDataGridViewCheckBoxColumn.Width = 85;
      // 
      // allowEditOthersDataGridViewCheckBoxColumn
      // 
      this.allowEditOthersDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
      this.allowEditOthersDataGridViewCheckBoxColumn.DataPropertyName = "AllowEditOthers";
      this.allowEditOthersDataGridViewCheckBoxColumn.HeaderText = "Редагування іншими користувачами?";
      this.allowEditOthersDataGridViewCheckBoxColumn.Name = "allowEditOthersDataGridViewCheckBoxColumn";
      this.allowEditOthersDataGridViewCheckBoxColumn.ReadOnly = true;
      this.allowEditOthersDataGridViewCheckBoxColumn.Width = 85;
      // 
      // createdDataGridViewTextBoxColumn
      // 
      this.createdDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.createdDataGridViewTextBoxColumn.DataPropertyName = "Created";
      this.createdDataGridViewTextBoxColumn.HeaderText = "Автор";
      this.createdDataGridViewTextBoxColumn.Name = "createdDataGridViewTextBoxColumn";
      this.createdDataGridViewTextBoxColumn.ReadOnly = true;
      this.createdDataGridViewTextBoxColumn.Width = 73;
      // 
      // dCreatedDataGridViewTextBoxColumn
      // 
      this.dCreatedDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.dCreatedDataGridViewTextBoxColumn.DataPropertyName = "DCreated";
      this.dCreatedDataGridViewTextBoxColumn.HeaderText = "Створено";
      this.dCreatedDataGridViewTextBoxColumn.Name = "dCreatedDataGridViewTextBoxColumn";
      this.dCreatedDataGridViewTextBoxColumn.ReadOnly = true;
      this.dCreatedDataGridViewTextBoxColumn.Width = 97;
      // 
      // updatedDataGridViewTextBoxColumn
      // 
      this.updatedDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.updatedDataGridViewTextBoxColumn.DataPropertyName = "Updated";
      this.updatedDataGridViewTextBoxColumn.HeaderText = "Оновлено";
      this.updatedDataGridViewTextBoxColumn.Name = "updatedDataGridViewTextBoxColumn";
      this.updatedDataGridViewTextBoxColumn.ReadOnly = true;
      this.updatedDataGridViewTextBoxColumn.Width = 99;
      // 
      // dUpdatedDataGridViewTextBoxColumn
      // 
      this.dUpdatedDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
      this.dUpdatedDataGridViewTextBoxColumn.DataPropertyName = "DUpdated";
      this.dUpdatedDataGridViewTextBoxColumn.HeaderText = "Оновлено";
      this.dUpdatedDataGridViewTextBoxColumn.Name = "dUpdatedDataGridViewTextBoxColumn";
      this.dUpdatedDataGridViewTextBoxColumn.ReadOnly = true;
      this.dUpdatedDataGridViewTextBoxColumn.Width = 99;
      // 
      // bs
      // 
      this.bs.DataSource = typeof(DGCore.UserSettings.UserSettingsDbObject);
      // 
      // frmSelectSetting
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(975, 622);
      this.Controls.Add(this.dataGridView1);
      this.Controls.Add(this.panel1);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.Name = "frmSelectSetting";
      this.Text = "Вибрати налаштування";
      this.Load += new System.EventHandler(this.frmSaveSetting_Load);
      ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
      this.panel1.ResumeLayout(false);
      ((System.ComponentModel.ISupportInitialize)(this.bs)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.BindingSource bs;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.DataGridViewTextBoxColumn settingIDDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewCheckBoxColumn allowViewOthersDataGridViewCheckBoxColumn;
    private System.Windows.Forms.DataGridViewCheckBoxColumn allowEditOthersDataGridViewCheckBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn createdDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn dCreatedDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn updatedDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn dUpdatedDataGridViewTextBoxColumn;
    private System.Windows.Forms.Button btnApply;
    private System.Windows.Forms.Button btnCancel;
  }
}