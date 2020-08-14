namespace DGWnd.UI {
  partial class frmSetFilter {
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
      this.panel1 = new System.Windows.Forms.Panel();
      this.cbNot = new System.Windows.Forms.CheckBox();
      this.btnClearFilter = new System.Windows.Forms.Button();
      this.btnCancel = new System.Windows.Forms.Button();
      this.btnOK = new System.Windows.Forms.Button();
      this.dgvList = new System.Windows.Forms.DataGridView();
      this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewComboBoxColumn1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.dataGridViewComboBoxColumn2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.bsFilterItem = new System.Windows.Forms.BindingSource(this.components);
      this.filterOperandDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewComboBoxColumn3 = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.dataGridViewComboBoxColumn4 = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.colCB_Value1 = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.colCB_Value2 = new System.Windows.Forms.DataGridViewComboBoxColumn();
      this.Col_value1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.Col_value2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
      this.panel1.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvList)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.bsFilterItem)).BeginInit();
      this.SuspendLayout();
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.cbNot);
      this.panel1.Controls.Add(this.btnClearFilter);
      this.panel1.Controls.Add(this.btnCancel);
      this.panel1.Controls.Add(this.btnOK);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 455);
      this.panel1.Margin = new System.Windows.Forms.Padding(4);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(476, 91);
      this.panel1.TabIndex = 0;
      // 
      // cbNot
      // 
      this.cbNot.AutoSize = true;
      this.cbNot.Location = new System.Drawing.Point(16, 17);
      this.cbNot.Margin = new System.Windows.Forms.Padding(4);
      this.cbNot.Name = "cbNot";
      this.cbNot.Size = new System.Drawing.Size(404, 21);
      this.cbNot.TabIndex = 3;
      this.cbNot.Text = "Окрім (будуть попадати всі дані, окрім вказаних у виразі)";
      this.cbNot.UseVisualStyleBackColor = true;
      // 
      // btnClearFilter
      // 
      this.btnClearFilter.Location = new System.Drawing.Point(317, 48);
      this.btnClearFilter.Margin = new System.Windows.Forms.Padding(4);
      this.btnClearFilter.Name = "btnClearFilter";
      this.btnClearFilter.Size = new System.Drawing.Size(144, 28);
      this.btnClearFilter.TabIndex = 2;
      this.btnClearFilter.Text = "Очистити фільтр";
      this.btnClearFilter.UseVisualStyleBackColor = true;
      this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
      // 
      // btnCancel
      // 
      this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btnCancel.Location = new System.Drawing.Point(156, 48);
      this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
      this.btnCancel.Name = "btnCancel";
      this.btnCancel.Size = new System.Drawing.Size(100, 28);
      this.btnCancel.TabIndex = 1;
      this.btnCancel.Text = "Відмінити";
      this.btnCancel.UseVisualStyleBackColor = true;
      this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
      // 
      // btnOK
      // 
      this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
      this.btnOK.Location = new System.Drawing.Point(16, 48);
      this.btnOK.Margin = new System.Windows.Forms.Padding(4);
      this.btnOK.Name = "btnOK";
      this.btnOK.Size = new System.Drawing.Size(120, 28);
      this.btnOK.TabIndex = 0;
      this.btnOK.Text = "Підтвердити";
      this.btnOK.UseVisualStyleBackColor = true;
      this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
      // 
      // dgvList
      // 
      this.dgvList.AllowDrop = true;
      this.dgvList.AutoGenerateColumns = false;
      this.dgvList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
      this.dgvList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
      this.dgvList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.filterOperandDataGridViewTextBoxColumn,
            this.colCB_Value1,
            this.colCB_Value2,
            this.Col_value1,
            this.Col_value2});
      this.dgvList.DataSource = this.bsFilterItem;
      this.dgvList.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvList.Location = new System.Drawing.Point(0, 0);
      this.dgvList.Margin = new System.Windows.Forms.Padding(4);
      this.dgvList.Name = "dgvList";
      this.dgvList.Size = new System.Drawing.Size(476, 455);
      this.dgvList.TabIndex = 1;
      this.dgvList.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dgvList_CellBeginEdit);
      this.dgvList.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.dgvList_CellFormatting);
      this.dgvList.CellParsing += new System.Windows.Forms.DataGridViewCellParsingEventHandler(this.dgvList_CellParsing);
      this.dgvList.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dgvList_DataError);
      this.dgvList.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.dgvList_EditingControlShowing);
      this.dgvList.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgvList_RowPrePaint);
      // 
      // dataGridViewTextBoxColumn1
      // 
      this.dataGridViewTextBoxColumn1.DataPropertyName = "Value1";
      this.dataGridViewTextBoxColumn1.HeaderText = "Вираз1";
      this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
      this.dataGridViewTextBoxColumn1.Width = 69;
      // 
      // dataGridViewTextBoxColumn2
      // 
      this.dataGridViewTextBoxColumn2.DataPropertyName = "Value2";
      this.dataGridViewTextBoxColumn2.HeaderText = "Вираз2";
      this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
      this.dataGridViewTextBoxColumn2.Width = 69;
      // 
      // dataGridViewComboBoxColumn1
      // 
      this.dataGridViewComboBoxColumn1.DataPropertyName = "Value1";
      this.dataGridViewComboBoxColumn1.HeaderText = "Вираз1";
      this.dataGridViewComboBoxColumn1.Name = "dataGridViewComboBoxColumn1";
      this.dataGridViewComboBoxColumn1.Width = 69;
      // 
      // dataGridViewComboBoxColumn2
      // 
      this.dataGridViewComboBoxColumn2.DataPropertyName = "Value2";
      this.dataGridViewComboBoxColumn2.HeaderText = "Вираз2";
      this.dataGridViewComboBoxColumn2.Name = "dataGridViewComboBoxColumn2";
      this.dataGridViewComboBoxColumn2.Width = 68;
      // 
      // bsFilterItem
      // 
      this.bsFilterItem.DataSource = typeof(DGCore.Filters.FilterLineSubitem);
      // 
      // filterOperandDataGridViewTextBoxColumn
      // 
      this.filterOperandDataGridViewTextBoxColumn.DataPropertyName = "FilterOperand";
      this.filterOperandDataGridViewTextBoxColumn.HeaderText = "Операнд";
      this.filterOperandDataGridViewTextBoxColumn.Name = "filterOperandDataGridViewTextBoxColumn";
      // 
      // dataGridViewComboBoxColumn3
      // 
      this.dataGridViewComboBoxColumn3.DataPropertyName = "Value1";
      this.dataGridViewComboBoxColumn3.HeaderText = "Вираз1";
      this.dataGridViewComboBoxColumn3.Name = "dataGridViewComboBoxColumn3";
      this.dataGridViewComboBoxColumn3.Width = 86;
      // 
      // dataGridViewComboBoxColumn4
      // 
      this.dataGridViewComboBoxColumn4.DataPropertyName = "Value2";
      this.dataGridViewComboBoxColumn4.HeaderText = "Вираз2";
      this.dataGridViewComboBoxColumn4.Name = "dataGridViewComboBoxColumn4";
      this.dataGridViewComboBoxColumn4.Width = 87;
      // 
      // dataGridViewTextBoxColumn3
      // 
      this.dataGridViewTextBoxColumn3.DataPropertyName = "Value1";
      this.dataGridViewTextBoxColumn3.HeaderText = "Value1";
      this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
      this.dataGridViewTextBoxColumn3.Width = 86;
      // 
      // dataGridViewTextBoxColumn4
      // 
      this.dataGridViewTextBoxColumn4.DataPropertyName = "Value2";
      this.dataGridViewTextBoxColumn4.HeaderText = "Value2";
      this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
      this.dataGridViewTextBoxColumn4.Width = 87;
      // 
      // colCB_Value1
      // 
      this.colCB_Value1.DataPropertyName = "Value1";
      this.colCB_Value1.HeaderText = "Вираз1";
      this.colCB_Value1.Name = "colCB_Value1";
      // 
      // colCB_Value2
      // 
      this.colCB_Value2.DataPropertyName = "Value2";
      this.colCB_Value2.HeaderText = "Вираз2";
      this.colCB_Value2.Name = "colCB_Value2";
      // 
      // Col_value1
      // 
      this.Col_value1.DataPropertyName = "Value1";
      this.Col_value1.HeaderText = "Вираз1";
      this.Col_value1.Name = "Col_value1";
      // 
      // Col_value2
      // 
      this.Col_value2.DataPropertyName = "Value2";
      this.Col_value2.HeaderText = "Вираз2";
      this.Col_value2.Name = "Col_value2";
      // 
      // frmSetFilter
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(476, 546);
      this.Controls.Add(this.dgvList);
      this.Controls.Add(this.panel1);
      this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.Margin = new System.Windows.Forms.Padding(4);
      this.Name = "frmSetFilter";
      this.Text = "Установити фільтр";
      this.Load += new System.EventHandler(this.frmSetFilter_Load);
      this.panel1.ResumeLayout(false);
      this.panel1.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.dgvList)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.bsFilterItem)).EndInit();
      this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.DataGridView dgvList;
    private System.Windows.Forms.Button btnClearFilter;
    private System.Windows.Forms.Button btnCancel;
    private System.Windows.Forms.Button btnOK;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn1;
    private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn2;
    private System.Windows.Forms.CheckBox cbNot;
    private System.Windows.Forms.BindingSource bsFilterItem;
    private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn3;
    private System.Windows.Forms.DataGridViewComboBoxColumn dataGridViewComboBoxColumn4;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private System.Windows.Forms.DataGridViewTextBoxColumn filterOperandDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewComboBoxColumn colCB_Value1;
    private System.Windows.Forms.DataGridViewComboBoxColumn colCB_Value2;
    private System.Windows.Forms.DataGridViewTextBoxColumn Col_value1;
    private System.Windows.Forms.DataGridViewTextBoxColumn Col_value2;

  }
}