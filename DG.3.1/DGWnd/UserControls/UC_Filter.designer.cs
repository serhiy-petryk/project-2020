namespace DGWnd.UserControls {
  partial class UC_Filter {
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
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
      System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
      this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dgvFilter = new System.Windows.Forms.DataGridView();
      this.colFilterDisplayName = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colInfo = new System.Windows.Forms.DataGridViewImageColumn();
      this.colFilterIgnoreCase = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.btnEdit = new System.Windows.Forms.DataGridViewImageColumn();
      this.displayNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.ignoreCaseDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.rowsStringDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.StringPresentation = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.bsFilter = new System.Windows.Forms.BindingSource(this.components);
      this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
      this.txtFilterText = new System.Windows.Forms.TextBox();
      ((System.ComponentModel.ISupportInitialize)(this.dgvFilter)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.bsFilter)).BeginInit();
      this.SuspendLayout();
      // 
      // dataGridViewTextBoxColumn7
      // 
      this.dataGridViewTextBoxColumn7.DataPropertyName = "FilterOperand";
      this.dataGridViewTextBoxColumn7.HeaderText = "Операнд";
      this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
      this.dataGridViewTextBoxColumn7.ReadOnly = true;
      this.dataGridViewTextBoxColumn7.Width = 72;
      // 
      // dgvFilter
      // 
      this.dgvFilter.AllowUserToAddRows = false;
      this.dgvFilter.AllowUserToDeleteRows = false;
      this.dgvFilter.AutoGenerateColumns = false;
      this.dgvFilter.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dgvFilter.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
      dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
      dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
      dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.dgvFilter.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
      this.dgvFilter.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
      this.dgvFilter.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colFilterDisplayName,
            this.colInfo,
            this.colFilterIgnoreCase,
            this.btnEdit,
            this.displayNameDataGridViewTextBoxColumn,
            this.ignoreCaseDataGridViewTextBoxColumn,
            this.rowsStringDataGridViewTextBoxColumn,
            this.StringPresentation});
      this.dgvFilter.DataSource = this.bsFilter;
      dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
      dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
      dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
      dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
      dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
      dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
      this.dgvFilter.DefaultCellStyle = dataGridViewCellStyle4;
      this.dgvFilter.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvFilter.Location = new System.Drawing.Point(0, 0);
      this.dgvFilter.Name = "dgvFilter";
      this.dgvFilter.RowHeadersVisible = false;
      this.dgvFilter.ShowCellToolTips = false;
      this.dgvFilter.Size = new System.Drawing.Size(689, 409);
      this.dgvFilter.TabIndex = 6;
      this.dgvFilter.CellMouseLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFilter_CellMouseLeave);
      this.dgvFilter.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFilter_RowEnter);
      this.dgvFilter.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvFilter_ColumnHeaderMouseClick);
      this.dgvFilter.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgv_RowPrePaint);
      this.dgvFilter.CellMouseEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFilter_CellMouseEnter);
      this.dgvFilter.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvFilter_CellClick);
      // 
      // colFilterDisplayName
      // 
      this.colFilterDisplayName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
      this.colFilterDisplayName.DataPropertyName = "DisplayName";
      this.colFilterDisplayName.HeaderText = "Назва колонки (для установки фільтра рядків натисніть тут)";
      this.colFilterDisplayName.Name = "colFilterDisplayName";
      this.colFilterDisplayName.ReadOnly = true;
      this.colFilterDisplayName.Width = 150;
      // 
      // colInfo
      // 
      this.colInfo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
      this.colInfo.HeaderText = "Інфо";
      this.colInfo.Image = global::DGWnd.Properties.Resources.infoBubble_BlankBackground;
      this.colInfo.MinimumWidth = 20;
      this.colInfo.Name = "colInfo";
      this.colInfo.ReadOnly = true;
      this.colInfo.Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.colInfo.Width = 40;
      // 
      // colFilterIgnoreCase
      // 
      this.colFilterIgnoreCase.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
      this.colFilterIgnoreCase.DataPropertyName = "IgnoreCase";
      dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
      dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.colFilterIgnoreCase.DefaultCellStyle = dataGridViewCellStyle2;
      this.colFilterIgnoreCase.HeaderText = "Не враховувати регістр букв";
      this.colFilterIgnoreCase.Name = "colFilterIgnoreCase";
      this.colFilterIgnoreCase.Width = 115;
      // 
      // btnEdit
      // 
      this.btnEdit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
      this.btnEdit.HeaderText = "Редагувати";
      this.btnEdit.Image = global::DGWnd.Properties.Resources.bt_edit;
      this.btnEdit.Name = "btnEdit";
      this.btnEdit.Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.btnEdit.Width = 90;
      // 
      // displayNameDataGridViewTextBoxColumn
      // 
      this.displayNameDataGridViewTextBoxColumn.DataPropertyName = "DisplayName";
      this.displayNameDataGridViewTextBoxColumn.HeaderText = "DisplayName";
      this.displayNameDataGridViewTextBoxColumn.Name = "displayNameDataGridViewTextBoxColumn";
      this.displayNameDataGridViewTextBoxColumn.ReadOnly = true;
      this.displayNameDataGridViewTextBoxColumn.Visible = false;
      // 
      // ignoreCaseDataGridViewTextBoxColumn
      // 
      this.ignoreCaseDataGridViewTextBoxColumn.DataPropertyName = "IgnoreCase";
      this.ignoreCaseDataGridViewTextBoxColumn.HeaderText = "IgnoreCase";
      this.ignoreCaseDataGridViewTextBoxColumn.Name = "ignoreCaseDataGridViewTextBoxColumn";
      this.ignoreCaseDataGridViewTextBoxColumn.Visible = false;
      // 
      // rowsStringDataGridViewTextBoxColumn
      // 
      this.rowsStringDataGridViewTextBoxColumn.DataPropertyName = "RowsString";
      this.rowsStringDataGridViewTextBoxColumn.HeaderText = "RowsString";
      this.rowsStringDataGridViewTextBoxColumn.Name = "rowsStringDataGridViewTextBoxColumn";
      this.rowsStringDataGridViewTextBoxColumn.ReadOnly = true;
      this.rowsStringDataGridViewTextBoxColumn.Visible = false;
      // 
      // StringPresentation
      // 
      dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
      this.StringPresentation.DefaultCellStyle = dataGridViewCellStyle3;
      this.StringPresentation.HeaderText = "Текст фільтра або Інфо";
      this.StringPresentation.Name = "StringPresentation";
      this.StringPresentation.ReadOnly = true;
      // 
      // dataGridViewImageColumn1
      // 
      this.dataGridViewImageColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
      this.dataGridViewImageColumn1.HeaderText = "Інфо";
      this.dataGridViewImageColumn1.Image = global::DGWnd.Properties.Resources.infoBubble;
      this.dataGridViewImageColumn1.MinimumWidth = 20;
      this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
      this.dataGridViewImageColumn1.ReadOnly = true;
      this.dataGridViewImageColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
      this.dataGridViewImageColumn1.Width = 20;
      // 
      // txtFilterText
      // 
      this.txtFilterText.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.txtFilterText.Location = new System.Drawing.Point(0, 409);
      this.txtFilterText.Multiline = true;
      this.txtFilterText.Name = "txtFilterText";
      this.txtFilterText.ReadOnly = true;
      this.txtFilterText.Size = new System.Drawing.Size(689, 20);
      this.txtFilterText.TabIndex = 7;
      this.txtFilterText.Text = "Filter Text";
      // 
      // UC_Filter
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.Controls.Add(this.dgvFilter);
      this.Controls.Add(this.txtFilterText);
      this.Name = "UC_Filter";
      this.Size = new System.Drawing.Size(689, 429);
      ((System.ComponentModel.ISupportInitialize)(this.dgvFilter)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.bsFilter)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.BindingSource bsFilter;
    private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    private System.Windows.Forms.DataGridView dgvFilter;
    private System.Windows.Forms.TextBox txtFilterText;
    private System.Windows.Forms.DataGridViewTextBoxColumn colFilterDisplayName;
    private System.Windows.Forms.DataGridViewImageColumn colInfo;
    private System.Windows.Forms.DataGridViewTextBoxColumn colFilterIgnoreCase;
    private System.Windows.Forms.DataGridViewImageColumn btnEdit;
    private System.Windows.Forms.DataGridViewTextBoxColumn displayNameDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn ignoreCaseDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn rowsStringDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn StringPresentation;
  }
}
