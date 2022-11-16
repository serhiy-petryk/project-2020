
namespace Quote2022
{
    partial class Form1
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
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLoader = new System.Windows.Forms.TabPage();
            this.btnSplitEoddataParse = new System.Windows.Forms.Button();
            this.btnSplitInvestingParse = new System.Windows.Forms.Button();
            this.btnSplitYahooParse = new System.Windows.Forms.Button();
            this.btnSplitTickertechParse = new System.Windows.Forms.Button();
            this.btnSymbolsTickertechParse = new System.Windows.Forms.Button();
            this.btnDayTickertechParse = new System.Windows.Forms.Button();
            this.btnTemp = new System.Windows.Forms.Button();
            this.btnSymbolsEoddataParse = new System.Windows.Forms.Button();
            this.btnDayEoddataParse = new System.Windows.Forms.Button();
            this.btnNanexSymbols = new System.Windows.Forms.Button();
            this.btnDayYahooIndexesParse = new System.Windows.Forms.Button();
            this.btnDayYahooParse = new System.Windows.Forms.Button();
            this.tabLayers = new System.Windows.Forms.TabPage();
            this.btnAlgorithm1 = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabLoader.SuspendLayout();
            this.tabLayers.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 330);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(957, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(118, 17);
            this.statusLabel.Text = "toolStripStatusLabel1";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabLoader);
            this.tabControl1.Controls.Add(this.tabLayers);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(957, 330);
            this.tabControl1.TabIndex = 12;
            // 
            // tabLoader
            // 
            this.tabLoader.Controls.Add(this.btnSplitEoddataParse);
            this.tabLoader.Controls.Add(this.btnSplitInvestingParse);
            this.tabLoader.Controls.Add(this.btnSplitYahooParse);
            this.tabLoader.Controls.Add(this.btnSplitTickertechParse);
            this.tabLoader.Controls.Add(this.btnSymbolsTickertechParse);
            this.tabLoader.Controls.Add(this.btnDayTickertechParse);
            this.tabLoader.Controls.Add(this.btnTemp);
            this.tabLoader.Controls.Add(this.btnSymbolsEoddataParse);
            this.tabLoader.Controls.Add(this.btnDayEoddataParse);
            this.tabLoader.Controls.Add(this.btnNanexSymbols);
            this.tabLoader.Controls.Add(this.btnDayYahooIndexesParse);
            this.tabLoader.Controls.Add(this.btnDayYahooParse);
            this.tabLoader.Location = new System.Drawing.Point(4, 22);
            this.tabLoader.Name = "tabLoader";
            this.tabLoader.Padding = new System.Windows.Forms.Padding(3);
            this.tabLoader.Size = new System.Drawing.Size(949, 304);
            this.tabLoader.TabIndex = 0;
            this.tabLoader.Text = "Loader";
            this.tabLoader.UseVisualStyleBackColor = true;
            // 
            // btnSplitEoddataParse
            // 
            this.btnSplitEoddataParse.Location = new System.Drawing.Point(385, 160);
            this.btnSplitEoddataParse.Name = "btnSplitEoddataParse";
            this.btnSplitEoddataParse.Size = new System.Drawing.Size(146, 23);
            this.btnSplitEoddataParse.TabIndex = 23;
            this.btnSplitEoddataParse.Text = "Split Eoddata Parse";
            this.btnSplitEoddataParse.UseVisualStyleBackColor = true;
            this.btnSplitEoddataParse.Click += new System.EventHandler(this.btnSplitEoddataParse_Click);
            // 
            // btnSplitInvestingParse
            // 
            this.btnSplitInvestingParse.Location = new System.Drawing.Point(385, 120);
            this.btnSplitInvestingParse.Name = "btnSplitInvestingParse";
            this.btnSplitInvestingParse.Size = new System.Drawing.Size(146, 23);
            this.btnSplitInvestingParse.TabIndex = 22;
            this.btnSplitInvestingParse.Text = "Split Investing.com Parse";
            this.btnSplitInvestingParse.UseVisualStyleBackColor = true;
            this.btnSplitInvestingParse.Click += new System.EventHandler(this.btnSplitInvestingParse_Click);
            // 
            // btnSplitYahooParse
            // 
            this.btnSplitYahooParse.Location = new System.Drawing.Point(385, 82);
            this.btnSplitYahooParse.Name = "btnSplitYahooParse";
            this.btnSplitYahooParse.Size = new System.Drawing.Size(146, 23);
            this.btnSplitYahooParse.TabIndex = 21;
            this.btnSplitYahooParse.Text = "Split Yahoo Parse (zip)";
            this.btnSplitYahooParse.UseVisualStyleBackColor = true;
            this.btnSplitYahooParse.Click += new System.EventHandler(this.btnSplitYahooParse_Click);
            // 
            // btnSplitTickertechParse
            // 
            this.btnSplitTickertechParse.Location = new System.Drawing.Point(385, 44);
            this.btnSplitTickertechParse.Name = "btnSplitTickertechParse";
            this.btnSplitTickertechParse.Size = new System.Drawing.Size(146, 23);
            this.btnSplitTickertechParse.TabIndex = 20;
            this.btnSplitTickertechParse.Text = "Split Tickertech Parse";
            this.btnSplitTickertechParse.UseVisualStyleBackColor = true;
            this.btnSplitTickertechParse.Click += new System.EventHandler(this.btnSplitTickertechParse_Click);
            // 
            // btnSymbolsTickertechParse
            // 
            this.btnSymbolsTickertechParse.Location = new System.Drawing.Point(202, 82);
            this.btnSymbolsTickertechParse.Name = "btnSymbolsTickertechParse";
            this.btnSymbolsTickertechParse.Size = new System.Drawing.Size(146, 23);
            this.btnSymbolsTickertechParse.TabIndex = 19;
            this.btnSymbolsTickertechParse.Text = "Symbols Tickertech Parse";
            this.btnSymbolsTickertechParse.UseVisualStyleBackColor = true;
            this.btnSymbolsTickertechParse.Click += new System.EventHandler(this.btnSymbolsTickertechParse_Click);
            // 
            // btnDayTickertechParse
            // 
            this.btnDayTickertechParse.Location = new System.Drawing.Point(385, 6);
            this.btnDayTickertechParse.Name = "btnDayTickertechParse";
            this.btnDayTickertechParse.Size = new System.Drawing.Size(146, 23);
            this.btnDayTickertechParse.TabIndex = 18;
            this.btnDayTickertechParse.Text = "Daily Tickertech Parse";
            this.btnDayTickertechParse.UseVisualStyleBackColor = true;
            this.btnDayTickertechParse.Click += new System.EventHandler(this.btnDayTickertechParse_Click);
            // 
            // btnTemp
            // 
            this.btnTemp.Location = new System.Drawing.Point(601, 6);
            this.btnTemp.Name = "btnTemp";
            this.btnTemp.Size = new System.Drawing.Size(81, 23);
            this.btnTemp.TabIndex = 17;
            this.btnTemp.Text = "Temp";
            this.btnTemp.UseVisualStyleBackColor = true;
            this.btnTemp.Click += new System.EventHandler(this.btnTemp_Click);
            // 
            // btnSymbolsEoddataParse
            // 
            this.btnSymbolsEoddataParse.Location = new System.Drawing.Point(202, 44);
            this.btnSymbolsEoddataParse.Name = "btnSymbolsEoddataParse";
            this.btnSymbolsEoddataParse.Size = new System.Drawing.Size(146, 23);
            this.btnSymbolsEoddataParse.TabIndex = 16;
            this.btnSymbolsEoddataParse.Text = "Symbols Eoddata Parse";
            this.btnSymbolsEoddataParse.UseVisualStyleBackColor = true;
            this.btnSymbolsEoddataParse.Click += new System.EventHandler(this.btnSymbolsEoddataParse_Click);
            // 
            // btnDayEoddataParse
            // 
            this.btnDayEoddataParse.Location = new System.Drawing.Point(15, 82);
            this.btnDayEoddataParse.Name = "btnDayEoddataParse";
            this.btnDayEoddataParse.Size = new System.Drawing.Size(146, 23);
            this.btnDayEoddataParse.TabIndex = 15;
            this.btnDayEoddataParse.Text = "Daily Eoddata Parse";
            this.btnDayEoddataParse.UseVisualStyleBackColor = true;
            this.btnDayEoddataParse.Click += new System.EventHandler(this.btnDayEoddataParse_Click);
            // 
            // btnNanexSymbols
            // 
            this.btnNanexSymbols.Location = new System.Drawing.Point(202, 6);
            this.btnNanexSymbols.Name = "btnNanexSymbols";
            this.btnNanexSymbols.Size = new System.Drawing.Size(146, 23);
            this.btnNanexSymbols.TabIndex = 14;
            this.btnNanexSymbols.Text = "Nanex Symbols";
            this.btnNanexSymbols.UseVisualStyleBackColor = true;
            this.btnNanexSymbols.Click += new System.EventHandler(this.btnSymbolsNanex_Click);
            // 
            // btnDayYahooIndexesParse
            // 
            this.btnDayYahooIndexesParse.Location = new System.Drawing.Point(15, 44);
            this.btnDayYahooIndexesParse.Name = "btnDayYahooIndexesParse";
            this.btnDayYahooIndexesParse.Size = new System.Drawing.Size(146, 23);
            this.btnDayYahooIndexesParse.TabIndex = 13;
            this.btnDayYahooIndexesParse.Text = "DayYahoo Indexes Parse";
            this.btnDayYahooIndexesParse.UseVisualStyleBackColor = true;
            this.btnDayYahooIndexesParse.Click += new System.EventHandler(this.btnDayYahooIndexesParse_Click);
            // 
            // btnDayYahooParse
            // 
            this.btnDayYahooParse.Location = new System.Drawing.Point(15, 6);
            this.btnDayYahooParse.Name = "btnDayYahooParse";
            this.btnDayYahooParse.Size = new System.Drawing.Size(146, 23);
            this.btnDayYahooParse.TabIndex = 12;
            this.btnDayYahooParse.Text = "DayYahoo Parse";
            this.btnDayYahooParse.UseVisualStyleBackColor = true;
            this.btnDayYahooParse.Click += new System.EventHandler(this.btnDayYahooParse_Click);
            // 
            // tabLayers
            // 
            this.tabLayers.Controls.Add(this.btnAlgorithm1);
            this.tabLayers.Location = new System.Drawing.Point(4, 22);
            this.tabLayers.Name = "tabLayers";
            this.tabLayers.Padding = new System.Windows.Forms.Padding(3);
            this.tabLayers.Size = new System.Drawing.Size(949, 304);
            this.tabLayers.TabIndex = 1;
            this.tabLayers.Text = "Layers";
            this.tabLayers.UseVisualStyleBackColor = true;
            // 
            // btnAlgorithm1
            // 
            this.btnAlgorithm1.Location = new System.Drawing.Point(24, 18);
            this.btnAlgorithm1.Name = "btnAlgorithm1";
            this.btnAlgorithm1.Size = new System.Drawing.Size(75, 23);
            this.btnAlgorithm1.TabIndex = 0;
            this.btnAlgorithm1.Text = "Algorithm 1";
            this.btnAlgorithm1.UseVisualStyleBackColor = true;
            this.btnAlgorithm1.Click += new System.EventHandler(this.btnAlgorithm1_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(957, 352);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabLoader.ResumeLayout(false);
            this.tabLayers.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabLoader;
        private System.Windows.Forms.TabPage tabLayers;
        private System.Windows.Forms.Button btnSplitInvestingParse;
        private System.Windows.Forms.Button btnSplitYahooParse;
        private System.Windows.Forms.Button btnSplitTickertechParse;
        private System.Windows.Forms.Button btnSymbolsTickertechParse;
        private System.Windows.Forms.Button btnDayTickertechParse;
        private System.Windows.Forms.Button btnTemp;
        private System.Windows.Forms.Button btnSymbolsEoddataParse;
        private System.Windows.Forms.Button btnDayEoddataParse;
        private System.Windows.Forms.Button btnNanexSymbols;
        private System.Windows.Forms.Button btnDayYahooIndexesParse;
        private System.Windows.Forms.Button btnDayYahooParse;
        private System.Windows.Forms.Button btnSplitEoddataParse;
        private System.Windows.Forms.Button btnAlgorithm1;
    }
}

