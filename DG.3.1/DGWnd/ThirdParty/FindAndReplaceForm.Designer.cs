namespace DGWnd.ThirdParty
{
    partial class FindAndReplaceForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
      this.tabControl1 = new System.Windows.Forms.TabControl();
      this.FindPage = new System.Windows.Forms.TabPage();
      this.gbSearch = new System.Windows.Forms.GroupBox();
      this.rbActiveColumn1 = new System.Windows.Forms.RadioButton();
      this.rbSelection1 = new System.Windows.Forms.RadioButton();
      this.rbAllTable1 = new System.Windows.Forms.RadioButton();
      this.FindButton1 = new System.Windows.Forms.Button();
      this.FindOptionGroupBox1 = new System.Windows.Forms.GroupBox();
      this.UseComboBox1 = new System.Windows.Forms.ComboBox();
      this.UseCheckBox1 = new System.Windows.Forms.CheckBox();
      this.SearchUpCheckBox1 = new System.Windows.Forms.CheckBox();
      this.MatchCellCheckBox1 = new System.Windows.Forms.CheckBox();
      this.MatchCaseCheckBox1 = new System.Windows.Forms.CheckBox();
      this.FindWhatTextBox1 = new System.Windows.Forms.TextBox();
      this.FindLabel1 = new System.Windows.Forms.Label();
      this.ReplacePage = new System.Windows.Forms.TabPage();
      this.ReplaceButton = new System.Windows.Forms.Button();
      this.ReplaceAllButton = new System.Windows.Forms.Button();
      this.FindButton2 = new System.Windows.Forms.Button();
      this.FindOptionGroup2 = new System.Windows.Forms.GroupBox();
      this.UseComboBox2 = new System.Windows.Forms.ComboBox();
      this.UseCheckBox2 = new System.Windows.Forms.CheckBox();
      this.SearchUpCheckBox2 = new System.Windows.Forms.CheckBox();
      this.MatchCellCheckBox2 = new System.Windows.Forms.CheckBox();
      this.MatchCaseCheckBox2 = new System.Windows.Forms.CheckBox();
      this.ReplaceWithTextBox = new System.Windows.Forms.TextBox();
      this.ReplaceLabel = new System.Windows.Forms.Label();
      this.FindWhatTextBox2 = new System.Windows.Forms.TextBox();
      this.FindLabel2 = new System.Windows.Forms.Label();
      this.gbFindAndReplace = new System.Windows.Forms.GroupBox();
      this.rbActiveColumn2 = new System.Windows.Forms.RadioButton();
      this.rbSelection2 = new System.Windows.Forms.RadioButton();
      this.rbAllTable2 = new System.Windows.Forms.RadioButton();
      this.tabControl1.SuspendLayout();
      this.FindPage.SuspendLayout();
      this.gbSearch.SuspendLayout();
      this.FindOptionGroupBox1.SuspendLayout();
      this.ReplacePage.SuspendLayout();
      this.FindOptionGroup2.SuspendLayout();
      this.gbFindAndReplace.SuspendLayout();
      this.SuspendLayout();
      // 
      // tabControl1
      // 
      this.tabControl1.Controls.Add(this.FindPage);
      this.tabControl1.Controls.Add(this.ReplacePage);
      this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.tabControl1.Location = new System.Drawing.Point(0, 0);
      this.tabControl1.Name = "tabControl1";
      this.tabControl1.SelectedIndex = 0;
      this.tabControl1.Size = new System.Drawing.Size(561, 270);
      this.tabControl1.TabIndex = 0;
      // 
      // FindPage
      // 
      this.FindPage.Controls.Add(this.gbSearch);
      this.FindPage.Controls.Add(this.FindButton1);
      this.FindPage.Controls.Add(this.FindOptionGroupBox1);
      this.FindPage.Controls.Add(this.FindWhatTextBox1);
      this.FindPage.Controls.Add(this.FindLabel1);
      this.FindPage.Location = new System.Drawing.Point(4, 22);
      this.FindPage.Name = "FindPage";
      this.FindPage.Padding = new System.Windows.Forms.Padding(3);
      this.FindPage.Size = new System.Drawing.Size(553, 244);
      this.FindPage.TabIndex = 0;
      this.FindPage.Text = "Пошук";
      // 
      // gbSearch
      // 
      this.gbSearch.Controls.Add(this.rbActiveColumn1);
      this.gbSearch.Controls.Add(this.rbSelection1);
      this.gbSearch.Controls.Add(this.rbAllTable1);
      this.gbSearch.Location = new System.Drawing.Point(8, 32);
      this.gbSearch.Name = "gbSearch";
      this.gbSearch.Size = new System.Drawing.Size(120, 90);
      this.gbSearch.TabIndex = 12;
      this.gbSearch.TabStop = false;
      this.gbSearch.Text = "Де шукати?";
      // 
      // rbActiveColumn1
      // 
      this.rbActiveColumn1.AutoSize = true;
      this.rbActiveColumn1.Location = new System.Drawing.Point(6, 64);
      this.rbActiveColumn1.Name = "rbActiveColumn1";
      this.rbActiveColumn1.Size = new System.Drawing.Size(112, 17);
      this.rbActiveColumn1.TabIndex = 3;
      this.rbActiveColumn1.Text = "Активна колонка";
      this.rbActiveColumn1.UseVisualStyleBackColor = true;
      // 
      // rbSelection1
      // 
      this.rbSelection1.AutoSize = true;
      this.rbSelection1.Location = new System.Drawing.Point(6, 41);
      this.rbSelection1.Name = "rbSelection1";
      this.rbSelection1.Size = new System.Drawing.Size(110, 17);
      this.rbSelection1.TabIndex = 2;
      this.rbSelection1.Text = "Виділений регіон";
      this.rbSelection1.UseVisualStyleBackColor = true;
      // 
      // rbAllTable1
      // 
      this.rbAllTable1.AutoSize = true;
      this.rbAllTable1.Checked = true;
      this.rbAllTable1.Location = new System.Drawing.Point(6, 19);
      this.rbAllTable1.Name = "rbAllTable1";
      this.rbAllTable1.Size = new System.Drawing.Size(89, 17);
      this.rbAllTable1.TabIndex = 1;
      this.rbAllTable1.TabStop = true;
      this.rbAllTable1.Text = "Уся таблиця";
      this.rbAllTable1.UseVisualStyleBackColor = true;
      // 
      // FindButton1
      // 
      this.FindButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.FindButton1.Location = new System.Drawing.Point(14, 192);
      this.FindButton1.Name = "FindButton1";
      this.FindButton1.Size = new System.Drawing.Size(75, 23);
      this.FindButton1.TabIndex = 11;
      this.FindButton1.Text = "Шукати далі";
      this.FindButton1.UseVisualStyleBackColor = true;
      this.FindButton1.Click += new System.EventHandler(this.FindButton1_Click);
      // 
      // FindOptionGroupBox1
      // 
      this.FindOptionGroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.FindOptionGroupBox1.Controls.Add(this.UseComboBox1);
      this.FindOptionGroupBox1.Controls.Add(this.UseCheckBox1);
      this.FindOptionGroupBox1.Controls.Add(this.SearchUpCheckBox1);
      this.FindOptionGroupBox1.Controls.Add(this.MatchCellCheckBox1);
      this.FindOptionGroupBox1.Controls.Add(this.MatchCaseCheckBox1);
      this.FindOptionGroupBox1.Location = new System.Drawing.Point(138, 32);
      this.FindOptionGroupBox1.Name = "FindOptionGroupBox1";
      this.FindOptionGroupBox1.Size = new System.Drawing.Size(409, 122);
      this.FindOptionGroupBox1.TabIndex = 10;
      this.FindOptionGroupBox1.TabStop = false;
      this.FindOptionGroupBox1.Text = "Параметри пошуку";
      // 
      // UseComboBox1
      // 
      this.UseComboBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.UseComboBox1.Enabled = false;
      this.UseComboBox1.FormattingEnabled = true;
      this.UseComboBox1.Items.AddRange(new object[] {
            "Регулярні вирази",
            "Шаблони"});
      this.UseComboBox1.Location = new System.Drawing.Point(35, 91);
      this.UseComboBox1.Name = "UseComboBox1";
      this.UseComboBox1.Size = new System.Drawing.Size(368, 21);
      this.UseComboBox1.TabIndex = 4;
      this.UseComboBox1.SelectedIndexChanged += new System.EventHandler(this.UseComboBox1_SelectedIndexChanged);
      // 
      // UseCheckBox1
      // 
      this.UseCheckBox1.AutoSize = true;
      this.UseCheckBox1.Location = new System.Drawing.Point(16, 75);
      this.UseCheckBox1.Name = "UseCheckBox1";
      this.UseCheckBox1.Size = new System.Drawing.Size(117, 17);
      this.UseCheckBox1.TabIndex = 3;
      this.UseCheckBox1.Text = "Використовувати:";
      this.UseCheckBox1.UseVisualStyleBackColor = true;
      this.UseCheckBox1.CheckedChanged += new System.EventHandler(this.UseCheckBox1_CheckedChanged);
      // 
      // SearchUpCheckBox1
      // 
      this.SearchUpCheckBox1.AutoSize = true;
      this.SearchUpCheckBox1.Location = new System.Drawing.Point(16, 56);
      this.SearchUpCheckBox1.Name = "SearchUpCheckBox1";
      this.SearchUpCheckBox1.Size = new System.Drawing.Size(90, 17);
      this.SearchUpCheckBox1.TabIndex = 2;
      this.SearchUpCheckBox1.Text = "Пошук вгору";
      this.SearchUpCheckBox1.UseVisualStyleBackColor = true;
      this.SearchUpCheckBox1.CheckedChanged += new System.EventHandler(this.SearchUpCheckBox1_CheckedChanged);
      // 
      // MatchCellCheckBox1
      // 
      this.MatchCellCheckBox1.AutoSize = true;
      this.MatchCellCheckBox1.Location = new System.Drawing.Point(16, 38);
      this.MatchCellCheckBox1.Name = "MatchCellCheckBox1";
      this.MatchCellCheckBox1.Size = new System.Drawing.Size(191, 17);
      this.MatchCellCheckBox1.TabIndex = 1;
      this.MatchCellCheckBox1.Text = "Співпадання тексту всієї клітини";
      this.MatchCellCheckBox1.UseVisualStyleBackColor = true;
      this.MatchCellCheckBox1.CheckedChanged += new System.EventHandler(this.MatchCellCheckBox1_CheckedChanged);
      // 
      // MatchCaseCheckBox1
      // 
      this.MatchCaseCheckBox1.AutoSize = true;
      this.MatchCaseCheckBox1.Location = new System.Drawing.Point(16, 19);
      this.MatchCaseCheckBox1.Name = "MatchCaseCheckBox1";
      this.MatchCaseCheckBox1.Size = new System.Drawing.Size(171, 17);
      this.MatchCaseCheckBox1.TabIndex = 0;
      this.MatchCaseCheckBox1.Text = "З урахуванням регістру букв";
      this.MatchCaseCheckBox1.UseVisualStyleBackColor = true;
      this.MatchCaseCheckBox1.CheckedChanged += new System.EventHandler(this.MatchCaseCheckBox1_CheckedChanged);
      // 
      // FindWhatTextBox1
      // 
      this.FindWhatTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.FindWhatTextBox1.Location = new System.Drawing.Point(83, 6);
      this.FindWhatTextBox1.Name = "FindWhatTextBox1";
      this.FindWhatTextBox1.Size = new System.Drawing.Size(462, 20);
      this.FindWhatTextBox1.TabIndex = 5;
      this.FindWhatTextBox1.TextChanged += new System.EventHandler(this.FindWhatTextBox1_TextChanged);
      this.FindWhatTextBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FindWhatTextBox1_KeyPress);
      // 
      // FindLabel1
      // 
      this.FindLabel1.AutoSize = true;
      this.FindLabel1.Location = new System.Drawing.Point(2, 10);
      this.FindLabel1.Name = "FindLabel1";
      this.FindLabel1.Size = new System.Drawing.Size(65, 13);
      this.FindLabel1.TabIndex = 4;
      this.FindLabel1.Text = "Що шукати:";
      // 
      // ReplacePage
      // 
      this.ReplacePage.BackColor = System.Drawing.Color.White;
      this.ReplacePage.Controls.Add(this.gbFindAndReplace);
      this.ReplacePage.Controls.Add(this.ReplaceButton);
      this.ReplacePage.Controls.Add(this.ReplaceAllButton);
      this.ReplacePage.Controls.Add(this.FindButton2);
      this.ReplacePage.Controls.Add(this.FindOptionGroup2);
      this.ReplacePage.Controls.Add(this.ReplaceWithTextBox);
      this.ReplacePage.Controls.Add(this.ReplaceLabel);
      this.ReplacePage.Controls.Add(this.FindWhatTextBox2);
      this.ReplacePage.Controls.Add(this.FindLabel2);
      this.ReplacePage.Location = new System.Drawing.Point(4, 22);
      this.ReplacePage.Name = "ReplacePage";
      this.ReplacePage.Padding = new System.Windows.Forms.Padding(3);
      this.ReplacePage.Size = new System.Drawing.Size(553, 244);
      this.ReplacePage.TabIndex = 1;
      this.ReplacePage.Text = "Заміна";
      // 
      // ReplaceButton
      // 
      this.ReplaceButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReplaceButton.Location = new System.Drawing.Point(14, 213);
      this.ReplaceButton.Name = "ReplaceButton";
      this.ReplaceButton.Size = new System.Drawing.Size(75, 23);
      this.ReplaceButton.TabIndex = 14;
      this.ReplaceButton.Text = "Заміна";
      this.ReplaceButton.UseVisualStyleBackColor = true;
      this.ReplaceButton.Click += new System.EventHandler(this.ReplaceButton_Click);
      // 
      // ReplaceAllButton
      // 
      this.ReplaceAllButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.ReplaceAllButton.Location = new System.Drawing.Point(95, 213);
      this.ReplaceAllButton.Name = "ReplaceAllButton";
      this.ReplaceAllButton.Size = new System.Drawing.Size(91, 23);
      this.ReplaceAllButton.TabIndex = 13;
      this.ReplaceAllButton.Text = "Замінити все";
      this.ReplaceAllButton.UseVisualStyleBackColor = true;
      this.ReplaceAllButton.Click += new System.EventHandler(this.ReplaceAllButton_Click);
      // 
      // FindButton2
      // 
      this.FindButton2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
      this.FindButton2.Location = new System.Drawing.Point(12, 173);
      this.FindButton2.Name = "FindButton2";
      this.FindButton2.Size = new System.Drawing.Size(91, 23);
      this.FindButton2.TabIndex = 12;
      this.FindButton2.Text = "Шукати далі";
      this.FindButton2.UseVisualStyleBackColor = true;
      this.FindButton2.Click += new System.EventHandler(this.FindButton2_Click);
      // 
      // FindOptionGroup2
      // 
      this.FindOptionGroup2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.FindOptionGroup2.Controls.Add(this.UseComboBox2);
      this.FindOptionGroup2.Controls.Add(this.UseCheckBox2);
      this.FindOptionGroup2.Controls.Add(this.SearchUpCheckBox2);
      this.FindOptionGroup2.Controls.Add(this.MatchCellCheckBox2);
      this.FindOptionGroup2.Controls.Add(this.MatchCaseCheckBox2);
      this.FindOptionGroup2.Location = new System.Drawing.Point(134, 58);
      this.FindOptionGroup2.Name = "FindOptionGroup2";
      this.FindOptionGroup2.Size = new System.Drawing.Size(411, 122);
      this.FindOptionGroup2.TabIndex = 8;
      this.FindOptionGroup2.TabStop = false;
      this.FindOptionGroup2.Text = "Параметри пошуку";
      // 
      // UseComboBox2
      // 
      this.UseComboBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.UseComboBox2.Enabled = false;
      this.UseComboBox2.FormattingEnabled = true;
      this.UseComboBox2.Items.AddRange(new object[] {
            "Регулярні вирази",
            "Шаблони"});
      this.UseComboBox2.Location = new System.Drawing.Point(35, 91);
      this.UseComboBox2.Name = "UseComboBox2";
      this.UseComboBox2.Size = new System.Drawing.Size(370, 21);
      this.UseComboBox2.TabIndex = 4;
      this.UseComboBox2.SelectedIndexChanged += new System.EventHandler(this.UseComboBox2_SelectedIndexChanged);
      // 
      // UseCheckBox2
      // 
      this.UseCheckBox2.AutoSize = true;
      this.UseCheckBox2.Location = new System.Drawing.Point(16, 75);
      this.UseCheckBox2.Name = "UseCheckBox2";
      this.UseCheckBox2.Size = new System.Drawing.Size(117, 17);
      this.UseCheckBox2.TabIndex = 3;
      this.UseCheckBox2.Text = "Використовувати:";
      this.UseCheckBox2.UseVisualStyleBackColor = true;
      this.UseCheckBox2.CheckedChanged += new System.EventHandler(this.UseCheckBox2_CheckedChanged);
      // 
      // SearchUpCheckBox2
      // 
      this.SearchUpCheckBox2.AutoSize = true;
      this.SearchUpCheckBox2.Location = new System.Drawing.Point(16, 56);
      this.SearchUpCheckBox2.Name = "SearchUpCheckBox2";
      this.SearchUpCheckBox2.Size = new System.Drawing.Size(90, 17);
      this.SearchUpCheckBox2.TabIndex = 2;
      this.SearchUpCheckBox2.Text = "Пошук вгору";
      this.SearchUpCheckBox2.UseVisualStyleBackColor = true;
      this.SearchUpCheckBox2.CheckedChanged += new System.EventHandler(this.SearchUpCheckBox2_CheckedChanged);
      // 
      // MatchCellCheckBox2
      // 
      this.MatchCellCheckBox2.AutoSize = true;
      this.MatchCellCheckBox2.Location = new System.Drawing.Point(16, 38);
      this.MatchCellCheckBox2.Name = "MatchCellCheckBox2";
      this.MatchCellCheckBox2.Size = new System.Drawing.Size(191, 17);
      this.MatchCellCheckBox2.TabIndex = 1;
      this.MatchCellCheckBox2.Text = "Співпадання тексту всієї клітини";
      this.MatchCellCheckBox2.UseVisualStyleBackColor = true;
      this.MatchCellCheckBox2.CheckedChanged += new System.EventHandler(this.MatchCellCheckBox2_CheckedChanged);
      // 
      // MatchCaseCheckBox2
      // 
      this.MatchCaseCheckBox2.AutoSize = true;
      this.MatchCaseCheckBox2.Location = new System.Drawing.Point(16, 19);
      this.MatchCaseCheckBox2.Name = "MatchCaseCheckBox2";
      this.MatchCaseCheckBox2.Size = new System.Drawing.Size(171, 17);
      this.MatchCaseCheckBox2.TabIndex = 0;
      this.MatchCaseCheckBox2.Text = "З урахуванням регістру букв";
      this.MatchCaseCheckBox2.UseVisualStyleBackColor = true;
      this.MatchCaseCheckBox2.CheckedChanged += new System.EventHandler(this.MatchCaseCheckBox2_CheckedChanged);
      // 
      // ReplaceWithTextBox
      // 
      this.ReplaceWithTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.ReplaceWithTextBox.Location = new System.Drawing.Point(83, 32);
      this.ReplaceWithTextBox.Name = "ReplaceWithTextBox";
      this.ReplaceWithTextBox.Size = new System.Drawing.Size(457, 20);
      this.ReplaceWithTextBox.TabIndex = 5;
      // 
      // ReplaceLabel
      // 
      this.ReplaceLabel.AutoSize = true;
      this.ReplaceLabel.Location = new System.Drawing.Point(2, 36);
      this.ReplaceLabel.Name = "ReplaceLabel";
      this.ReplaceLabel.Size = new System.Drawing.Size(71, 13);
      this.ReplaceLabel.TabIndex = 4;
      this.ReplaceLabel.Text = "Замінити на:";
      // 
      // FindWhatTextBox2
      // 
      this.FindWhatTextBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
      this.FindWhatTextBox2.Location = new System.Drawing.Point(83, 6);
      this.FindWhatTextBox2.Name = "FindWhatTextBox2";
      this.FindWhatTextBox2.Size = new System.Drawing.Size(457, 20);
      this.FindWhatTextBox2.TabIndex = 3;
      this.FindWhatTextBox2.TextChanged += new System.EventHandler(this.FindWhatTextBox2_TextChanged);
      // 
      // FindLabel2
      // 
      this.FindLabel2.AutoSize = true;
      this.FindLabel2.Location = new System.Drawing.Point(2, 10);
      this.FindLabel2.Name = "FindLabel2";
      this.FindLabel2.Size = new System.Drawing.Size(65, 13);
      this.FindLabel2.TabIndex = 2;
      this.FindLabel2.Text = "Що шукати:";
      // 
      // gbFindAndReplace
      // 
      this.gbFindAndReplace.Controls.Add(this.rbActiveColumn2);
      this.gbFindAndReplace.Controls.Add(this.rbSelection2);
      this.gbFindAndReplace.Controls.Add(this.rbAllTable2);
      this.gbFindAndReplace.Location = new System.Drawing.Point(8, 58);
      this.gbFindAndReplace.Name = "gbFindAndReplace";
      this.gbFindAndReplace.Size = new System.Drawing.Size(120, 90);
      this.gbFindAndReplace.TabIndex = 15;
      this.gbFindAndReplace.TabStop = false;
      this.gbFindAndReplace.Text = "Де шукати?";
      // 
      // rbActiveColumn2
      // 
      this.rbActiveColumn2.AutoSize = true;
      this.rbActiveColumn2.Location = new System.Drawing.Point(6, 64);
      this.rbActiveColumn2.Name = "rbActiveColumn2";
      this.rbActiveColumn2.Size = new System.Drawing.Size(112, 17);
      this.rbActiveColumn2.TabIndex = 3;
      this.rbActiveColumn2.Text = "Активна колонка";
      this.rbActiveColumn2.UseVisualStyleBackColor = true;
      // 
      // rbSelection2
      // 
      this.rbSelection2.AutoSize = true;
      this.rbSelection2.Location = new System.Drawing.Point(6, 41);
      this.rbSelection2.Name = "rbSelection2";
      this.rbSelection2.Size = new System.Drawing.Size(110, 17);
      this.rbSelection2.TabIndex = 2;
      this.rbSelection2.Text = "Виділений регіон";
      this.rbSelection2.UseVisualStyleBackColor = true;
      // 
      // rbAllTable2
      // 
      this.rbAllTable2.AutoSize = true;
      this.rbAllTable2.Checked = true;
      this.rbAllTable2.Location = new System.Drawing.Point(6, 19);
      this.rbAllTable2.Name = "rbAllTable2";
      this.rbAllTable2.Size = new System.Drawing.Size(89, 17);
      this.rbAllTable2.TabIndex = 1;
      this.rbAllTable2.TabStop = true;
      this.rbAllTable2.Text = "Уся таблиця";
      this.rbAllTable2.UseVisualStyleBackColor = true;
      // 
      // FindAndReplaceForm
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(561, 270);
      this.Controls.Add(this.tabControl1);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "FindAndReplaceForm";
      this.Text = "Пошук і Заміна";
      this.Activated += new System.EventHandler(this.FindAndReplaceForm_Activated);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FindAndReplaceForm_FormClosed);
      this.tabControl1.ResumeLayout(false);
      this.FindPage.ResumeLayout(false);
      this.FindPage.PerformLayout();
      this.gbSearch.ResumeLayout(false);
      this.gbSearch.PerformLayout();
      this.FindOptionGroupBox1.ResumeLayout(false);
      this.FindOptionGroupBox1.PerformLayout();
      this.ReplacePage.ResumeLayout(false);
      this.ReplacePage.PerformLayout();
      this.FindOptionGroup2.ResumeLayout(false);
      this.FindOptionGroup2.PerformLayout();
      this.gbFindAndReplace.ResumeLayout(false);
      this.gbFindAndReplace.PerformLayout();
      this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage FindPage;
        private System.Windows.Forms.TabPage ReplacePage;
        private System.Windows.Forms.TextBox FindWhatTextBox1;
        private System.Windows.Forms.Label FindLabel1;
        private System.Windows.Forms.GroupBox FindOptionGroup2;
        private System.Windows.Forms.CheckBox MatchCellCheckBox2;
        private System.Windows.Forms.CheckBox MatchCaseCheckBox2;
        private System.Windows.Forms.TextBox ReplaceWithTextBox;
        private System.Windows.Forms.Label ReplaceLabel;
        private System.Windows.Forms.TextBox FindWhatTextBox2;
        private System.Windows.Forms.Label FindLabel2;
        private System.Windows.Forms.CheckBox SearchUpCheckBox2;
        private System.Windows.Forms.ComboBox UseComboBox2;
        private System.Windows.Forms.CheckBox UseCheckBox2;
        private System.Windows.Forms.GroupBox FindOptionGroupBox1;
        private System.Windows.Forms.ComboBox UseComboBox1;
        private System.Windows.Forms.CheckBox UseCheckBox1;
        private System.Windows.Forms.CheckBox SearchUpCheckBox1;
        private System.Windows.Forms.CheckBox MatchCellCheckBox1;
        private System.Windows.Forms.CheckBox MatchCaseCheckBox1;
        private System.Windows.Forms.Button FindButton1;
        private System.Windows.Forms.Button FindButton2;
        private System.Windows.Forms.Button ReplaceButton;
        private System.Windows.Forms.Button ReplaceAllButton;
        private System.Windows.Forms.GroupBox gbSearch;
        private System.Windows.Forms.RadioButton rbActiveColumn1;
        private System.Windows.Forms.RadioButton rbSelection1;
        private System.Windows.Forms.RadioButton rbAllTable1;
        private System.Windows.Forms.GroupBox gbFindAndReplace;
        private System.Windows.Forms.RadioButton rbActiveColumn2;
        private System.Windows.Forms.RadioButton rbSelection2;
        private System.Windows.Forms.RadioButton rbAllTable2;
    }
}