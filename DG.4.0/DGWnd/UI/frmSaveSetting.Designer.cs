namespace DGWnd.UI {
  partial class frmSaveSetting {
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
            this.SettingId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.allowViewOthersDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.allowEditOthersDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.createdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dCreatedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updatedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dUpdatedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bs = new System.Windows.Forms.BindingSource(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnSaveSettingChanges = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.button1 = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.cbAllowEditToOthers = new System.Windows.Forms.CheckBox();
            this.cbAllowViewToOthers = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtNewSetting = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs)).BeginInit();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Arial", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SettingId,
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
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(974, 272);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dataGridView1_CellFormatting);
            this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
            this.dataGridView1.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_RowEnter);
            this.dataGridView1.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dataGridView1_RowPrePaint);
            // 
            // SettingId
            // 
            this.SettingId.DataPropertyName = "SettingId";
            this.SettingId.HeaderText = "Налаштування";
            this.SettingId.Name = "SettingId";
            this.SettingId.ReadOnly = true;
            // 
            // allowViewOthersDataGridViewCheckBoxColumn
            // 
            this.allowViewOthersDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.allowViewOthersDataGridViewCheckBoxColumn.DataPropertyName = "AllowViewOthers";
            this.allowViewOthersDataGridViewCheckBoxColumn.HeaderText = "Показувати іншим користувачам?";
            this.allowViewOthersDataGridViewCheckBoxColumn.Name = "allowViewOthersDataGridViewCheckBoxColumn";
            // 
            // allowEditOthersDataGridViewCheckBoxColumn
            // 
            this.allowEditOthersDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.allowEditOthersDataGridViewCheckBoxColumn.DataPropertyName = "AllowEditOthers";
            this.allowEditOthersDataGridViewCheckBoxColumn.HeaderText = "Редагування іншими користувачами?";
            this.allowEditOthersDataGridViewCheckBoxColumn.Name = "allowEditOthersDataGridViewCheckBoxColumn";
            this.allowEditOthersDataGridViewCheckBoxColumn.Width = 105;
            // 
            // createdDataGridViewTextBoxColumn
            // 
            this.createdDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.createdDataGridViewTextBoxColumn.DataPropertyName = "Created";
            this.createdDataGridViewTextBoxColumn.HeaderText = "Автор";
            this.createdDataGridViewTextBoxColumn.Name = "createdDataGridViewTextBoxColumn";
            this.createdDataGridViewTextBoxColumn.ReadOnly = true;
            this.createdDataGridViewTextBoxColumn.Width = 67;
            // 
            // dCreatedDataGridViewTextBoxColumn
            // 
            this.dCreatedDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dCreatedDataGridViewTextBoxColumn.DataPropertyName = "DCreated";
            this.dCreatedDataGridViewTextBoxColumn.HeaderText = "Створено";
            this.dCreatedDataGridViewTextBoxColumn.Name = "dCreatedDataGridViewTextBoxColumn";
            this.dCreatedDataGridViewTextBoxColumn.ReadOnly = true;
            this.dCreatedDataGridViewTextBoxColumn.Width = 88;
            // 
            // updatedDataGridViewTextBoxColumn
            // 
            this.updatedDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.updatedDataGridViewTextBoxColumn.DataPropertyName = "Updated";
            this.updatedDataGridViewTextBoxColumn.HeaderText = "Оновлено";
            this.updatedDataGridViewTextBoxColumn.Name = "updatedDataGridViewTextBoxColumn";
            this.updatedDataGridViewTextBoxColumn.ReadOnly = true;
            this.updatedDataGridViewTextBoxColumn.Width = 91;
            // 
            // dUpdatedDataGridViewTextBoxColumn
            // 
            this.dUpdatedDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dUpdatedDataGridViewTextBoxColumn.DataPropertyName = "DUpdated";
            this.dUpdatedDataGridViewTextBoxColumn.HeaderText = "Оновлено";
            this.dUpdatedDataGridViewTextBoxColumn.Name = "dUpdatedDataGridViewTextBoxColumn";
            this.dUpdatedDataGridViewTextBoxColumn.ReadOnly = true;
            this.dUpdatedDataGridViewTextBoxColumn.Width = 91;
            // 
            // bs
            // 
            this.bs.DataSource = typeof(DGCore.UserSettings.UserSettingsDbObject);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel1.Controls.Add(this.btnDelete);
            this.panel1.Controls.Add(this.btnSaveSettingChanges);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 272);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(974, 35);
            this.panel1.TabIndex = 1;
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(197, 6);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(251, 23);
            this.btnDelete.TabIndex = 6;
            this.btnDelete.Text = "Вилучити запис налаштування";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnSaveSettingChanges
            // 
            this.btnSaveSettingChanges.Location = new System.Drawing.Point(524, 6);
            this.btnSaveSettingChanges.Name = "btnSaveSettingChanges";
            this.btnSaveSettingChanges.Size = new System.Drawing.Size(251, 23);
            this.btnSaveSettingChanges.TabIndex = 5;
            this.btnSaveSettingChanges.Text = "Записати змінені налаштування";
            this.btnSaveSettingChanges.UseVisualStyleBackColor = true;
            this.btnSaveSettingChanges.Click += new System.EventHandler(this.btnSaveSettingChanges_Click);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Controls.Add(this.cbAllowEditToOthers);
            this.panel2.Controls.Add(this.cbAllowViewToOthers);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.txtNewSetting);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 307);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(974, 114);
            this.panel2.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(842, 75);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(112, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Вийти";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(566, 75);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(251, 23);
            this.btnOK.TabIndex = 4;
            this.btnOK.Text = "Записати нове налаштування";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // cbAllowEditToOthers
            // 
            this.cbAllowEditToOthers.AutoSize = true;
            this.cbAllowEditToOthers.Checked = true;
            this.cbAllowEditToOthers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAllowEditToOthers.Location = new System.Drawing.Point(17, 78);
            this.cbAllowEditToOthers.Name = "cbAllowEditToOthers";
            this.cbAllowEditToOthers.Size = new System.Drawing.Size(356, 20);
            this.cbAllowEditToOthers.TabIndex = 3;
            this.cbAllowEditToOthers.Text = "Інші користувачі можуть коригувати це налаштування?";
            this.cbAllowEditToOthers.UseVisualStyleBackColor = true;
            // 
            // cbAllowViewToOthers
            // 
            this.cbAllowViewToOthers.AutoSize = true;
            this.cbAllowViewToOthers.Checked = true;
            this.cbAllowViewToOthers.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAllowViewToOthers.Location = new System.Drawing.Point(17, 51);
            this.cbAllowViewToOthers.Name = "cbAllowViewToOthers";
            this.cbAllowViewToOthers.Size = new System.Drawing.Size(229, 20);
            this.cbAllowViewToOthers.TabIndex = 2;
            this.cbAllowViewToOthers.Text = "Показувати іншим користувачам?";
            this.cbAllowViewToOthers.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Нове налаштування";
            // 
            // txtNewSetting
            // 
            this.txtNewSetting.Location = new System.Drawing.Point(241, 18);
            this.txtNewSetting.Name = "txtNewSetting";
            this.txtNewSetting.Size = new System.Drawing.Size(380, 22);
            this.txtNewSetting.TabIndex = 0;
            // 
            // frmSaveSetting
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(974, 421);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Arial", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.Name = "frmSaveSetting";
            this.Text = "Записати налаштування";
            this.Load += new System.EventHandler(this.frmSaveSetting_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bs)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.DataGridView dataGridView1;
    private System.Windows.Forms.BindingSource bs;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.DataGridViewTextBoxColumn settingIDDataGridViewTextBoxColumn;
    private System.Windows.Forms.CheckBox cbAllowEditToOthers;
    private System.Windows.Forms.CheckBox cbAllowViewToOthers;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox txtNewSetting;
    private System.Windows.Forms.Button button1;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.Button btnSaveSettingChanges;
    private System.Windows.Forms.Button btnDelete;
    private System.Windows.Forms.DataGridViewTextBoxColumn SettingId;
    private System.Windows.Forms.DataGridViewCheckBoxColumn allowViewOthersDataGridViewCheckBoxColumn;
    private System.Windows.Forms.DataGridViewCheckBoxColumn allowEditOthersDataGridViewCheckBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn createdDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn dCreatedDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn updatedDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn dUpdatedDataGridViewTextBoxColumn;
  }
}