using DGWnd.ThirdParty.Oli;

namespace DGWnd.UserControls {
  partial class UC_ColumnSettings {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.dgvTotals = new System.Windows.Forms.DataGridView();
            this.displayNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalFunctionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.totalDecimalPlacesDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.clbAllColumns = new DGWnd.ThirdParty.Oli.DragDropCheckedListBox();
            this.lbFrozenColumns = new DGWnd.ThirdParty.Oli.DragDropListBox();
            this.clbGroups = new DGWnd.ThirdParty.Oli.DragDropCheckedListBox();
            this.clbSorts = new DGWnd.ThirdParty.Oli.DragDropCheckedListBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cbShowTotalRow = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsTotalLine = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTotals)).BeginInit();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsTotalLine)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.22106F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 21.33681F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.22106F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 2F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 26.22106F));
            this.tableLayoutPanel1.Controls.Add(this.dgvTotals, 2, 5);
            this.tableLayoutPanel1.Controls.Add(this.label7, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.label6, 6, 2);
            this.tableLayoutPanel1.Controls.Add(this.label5, 4, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 4, 1);
            this.tableLayoutPanel1.Controls.Add(this.label3, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.label2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.clbAllColumns, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lbFrozenColumns, 2, 3);
            this.tableLayoutPanel1.Controls.Add(this.clbGroups, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.clbSorts, 6, 3);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 2, 4);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 6;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(622, 423);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // dgvTotals
            // 
            this.dgvTotals.AllowDrop = true;
            this.dgvTotals.AllowUserToAddRows = false;
            this.dgvTotals.AllowUserToDeleteRows = false;
            this.dgvTotals.AllowUserToResizeRows = false;
            this.dgvTotals.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTotals.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTotals.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvTotals.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTotals.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.displayNameDataGridViewTextBoxColumn,
            this.totalFunctionDataGridViewTextBoxColumn,
            this.totalDecimalPlacesDataGridViewTextBoxColumn});
            this.tableLayoutPanel1.SetColumnSpan(this.dgvTotals, 5);
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTotals.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvTotals.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvTotals.Location = new System.Drawing.Point(166, 214);
            this.dgvTotals.Name = "dgvTotals";
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvTotals.RowsDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvTotals.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvTotals.Size = new System.Drawing.Size(453, 206);
            this.dgvTotals.TabIndex = 14;
            this.dgvTotals.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.dgV_ReoderColumnsAndRows1_RowPrePaint);
            // 
            // displayNameDataGridViewTextBoxColumn
            // 
            this.displayNameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.displayNameDataGridViewTextBoxColumn.DataPropertyName = "DisplayName";
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.displayNameDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.displayNameDataGridViewTextBoxColumn.HeaderText = "Назва колонки";
            this.displayNameDataGridViewTextBoxColumn.Name = "displayNameDataGridViewTextBoxColumn";
            this.displayNameDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.displayNameDataGridViewTextBoxColumn.Width = 101;
            // 
            // totalFunctionDataGridViewTextBoxColumn
            // 
            this.totalFunctionDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.totalFunctionDataGridViewTextBoxColumn.DataPropertyName = "TotalFunction";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.totalFunctionDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle3;
            this.totalFunctionDataGridViewTextBoxColumn.HeaderText = "Функція підсумку";
            this.totalFunctionDataGridViewTextBoxColumn.MinimumWidth = 40;
            this.totalFunctionDataGridViewTextBoxColumn.Name = "totalFunctionDataGridViewTextBoxColumn";
            this.totalFunctionDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.totalFunctionDataGridViewTextBoxColumn.Width = 115;
            // 
            // totalDecimalPlacesDataGridViewTextBoxColumn
            // 
            this.totalDecimalPlacesDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.totalDecimalPlacesDataGridViewTextBoxColumn.DataPropertyName = "DecimalPlaces";
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.totalDecimalPlacesDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle4;
            this.totalDecimalPlacesDataGridViewTextBoxColumn.HeaderText = "Знаки округлення результату підсумку";
            this.totalDecimalPlacesDataGridViewTextBoxColumn.MinimumWidth = 40;
            this.totalDecimalPlacesDataGridViewTextBoxColumn.Name = "totalDecimalPlacesDataGridViewTextBoxColumn";
            this.totalDecimalPlacesDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.totalDecimalPlacesDataGridViewTextBoxColumn.Width = 194;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this.label7, 7);
            this.label7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label7.ForeColor = System.Drawing.Color.Blue;
            this.label7.Location = new System.Drawing.Point(3, 0);
            this.label7.Margin = new System.Windows.Forms.Padding(3, 0, 3, 6);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(616, 34);
            this.label7.TabIndex = 13;
            this.label7.Text = "За допомогою миші Ви можете переміщати колонки з одного вікна списка в інше. Для " +
    "вилучення колонки із вікна, потрібно перетянути її у вікно \"Список усіх колонок\"" +
    "";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label6.Location = new System.Drawing.Point(462, 76);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(157, 17);
            this.label6.TabIndex = 5;
            this.label6.Text = "Сортування";
            this.label6.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label5.Location = new System.Drawing.Point(299, 76);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(155, 17);
            this.label5.TabIndex = 4;
            this.label5.Text = "Групування";
            this.label5.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tableLayoutPanel1.SetColumnSpan(this.label4, 3);
            this.label4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label4.ForeColor = System.Drawing.Color.Blue;
            this.label4.Location = new System.Drawing.Point(299, 40);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(320, 36);
            this.label4.TabIndex = 3;
            this.label4.Text = "Відмічені колонки означають порядок зменшення при групуванні і сортуванні";
            this.label4.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(3, 40);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(155, 36);
            this.label3.TabIndex = 2;
            this.label3.Text = "Відмічені колонки будуть невидимі";
            this.label3.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(166, 40);
            this.label2.Name = "label2";
            this.tableLayoutPanel1.SetRowSpan(this.label2, 2);
            this.label2.Size = new System.Drawing.Size(125, 53);
            this.label2.TabIndex = 1;
            this.label2.Text = "Фіксовані колонки";
            this.label2.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 76);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(155, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Список усіх колонок";
            this.label1.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // clbAllColumns
            // 
            this.clbAllColumns.AllowDrop = true;
            this.clbAllColumns.CheckOnClick = true;
            this.clbAllColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbAllColumns.FormattingEnabled = true;
            this.clbAllColumns.IsDragDropMoveSource = false;
            this.clbAllColumns.Location = new System.Drawing.Point(3, 96);
            this.clbAllColumns.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.clbAllColumns.Name = "clbAllColumns";
            this.tableLayoutPanel1.SetRowSpan(this.clbAllColumns, 3);
            this.clbAllColumns.Size = new System.Drawing.Size(155, 327);
            this.clbAllColumns.TabIndex = 8;
            this.clbAllColumns.Dropped += new System.EventHandler<DGWnd.ThirdParty.Oli.DroppedEventArgs>(this.ListBox_Dropped);
            this.clbAllColumns.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ItemCheckChanges);
            // 
            // lbFrozenColumns
            // 
            this.lbFrozenColumns.AllowDrop = true;
            this.lbFrozenColumns.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbFrozenColumns.FormattingEnabled = true;
            this.lbFrozenColumns.IsDragDropMoveSource = false;
            this.lbFrozenColumns.ItemHeight = 16;
            this.lbFrozenColumns.Location = new System.Drawing.Point(166, 96);
            this.lbFrozenColumns.Name = "lbFrozenColumns";
            this.lbFrozenColumns.Size = new System.Drawing.Size(125, 84);
            this.lbFrozenColumns.TabIndex = 9;
            this.lbFrozenColumns.Dropped += new System.EventHandler<DGWnd.ThirdParty.Oli.DroppedEventArgs>(this.ListBox_Dropped);
            // 
            // clbGroups
            // 
            this.clbGroups.AllowDrop = true;
            this.clbGroups.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbGroups.FormattingEnabled = true;
            this.clbGroups.IsDragDropMoveSource = false;
            this.clbGroups.Location = new System.Drawing.Point(299, 96);
            this.clbGroups.Name = "clbGroups";
            this.clbGroups.Size = new System.Drawing.Size(155, 84);
            this.clbGroups.TabIndex = 10;
            this.clbGroups.Dropped += new System.EventHandler<DGWnd.ThirdParty.Oli.DroppedEventArgs>(this.ListBox_Dropped);
            this.clbGroups.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ItemCheckChanges);
            // 
            // clbSorts
            // 
            this.clbSorts.AllowDrop = true;
            this.clbSorts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbSorts.FormattingEnabled = true;
            this.clbSorts.IsDragDropMoveSource = false;
            this.clbSorts.Location = new System.Drawing.Point(462, 96);
            this.clbSorts.Name = "clbSorts";
            this.clbSorts.Size = new System.Drawing.Size(157, 84);
            this.clbSorts.TabIndex = 11;
            this.clbSorts.Dropped += new System.EventHandler<DGWnd.ThirdParty.Oli.DroppedEventArgs>(this.ListBox_Dropped);
            this.clbSorts.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ItemCheckChanges);
            // 
            // panel2
            // 
            this.panel2.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.SetColumnSpan(this.panel2, 5);
            this.panel2.Controls.Add(this.cbShowTotalRow);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(166, 186);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(453, 22);
            this.panel2.TabIndex = 12;
            // 
            // cbShowTotalRow
            // 
            this.cbShowTotalRow.AutoSize = true;
            this.cbShowTotalRow.Dock = System.Windows.Forms.DockStyle.Right;
            this.cbShowTotalRow.Location = new System.Drawing.Point(228, 0);
            this.cbShowTotalRow.Margin = new System.Windows.Forms.Padding(0);
            this.cbShowTotalRow.Name = "cbShowTotalRow";
            this.cbShowTotalRow.Size = new System.Drawing.Size(225, 22);
            this.cbShowTotalRow.TabIndex = 9;
            this.cbShowTotalRow.Text = "Показати підсумковий рядок?";
            this.cbShowTotalRow.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Dock = System.Windows.Forms.DockStyle.Left;
            this.label8.Location = new System.Drawing.Point(0, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(153, 17);
            this.label8.TabIndex = 8;
            this.label8.Text = "Колонки з підсумками";
            this.label8.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn1.DataPropertyName = "DisplayName";
            this.dataGridViewTextBoxColumn1.HeaderText = "Назва колонки";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn2.DataPropertyName = "TotalFunction";
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn2.DefaultCellStyle = dataGridViewCellStyle7;
            this.dataGridViewTextBoxColumn2.HeaderText = "Функція підсумку";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 40;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.dataGridViewTextBoxColumn3.DataPropertyName = "DecimalPlaces";
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.dataGridViewTextBoxColumn3.DefaultCellStyle = dataGridViewCellStyle8;
            this.dataGridViewTextBoxColumn3.HeaderText = "Знаки округлення результату підсумку";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // bsTotalLine
            // 
            this.bsTotalLine.DataSource = typeof(DGCore.Misc.TotalLine);
            // 
            // UC_ColumnSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "UC_ColumnSettings";
            this.Size = new System.Drawing.Size(622, 423);
            this.Load += new System.EventHandler(this.PDC_ColumnSettings_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTotals)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsTotalLine)).EndInit();
            this.ResumeLayout(false);

    }

    #endregion

    private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
    private DragDropCheckedListBox clbAllColumns;
    private DragDropListBox lbFrozenColumns;
    private DragDropCheckedListBox clbGroups;
    private DragDropCheckedListBox clbSorts;
    private System.Windows.Forms.Panel panel2;
    private System.Windows.Forms.CheckBox cbShowTotalRow;
    private System.Windows.Forms.Label label8;
    private System.Windows.Forms.Label label7;
    private System.Windows.Forms.DataGridView dgvTotals;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
      private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private System.Windows.Forms.BindingSource bsTotalLine;
    private System.Windows.Forms.DataGridViewTextBoxColumn displayNameDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn totalFunctionDataGridViewTextBoxColumn;
    private System.Windows.Forms.DataGridViewTextBoxColumn totalDecimalPlacesDataGridViewTextBoxColumn;
  }
}
