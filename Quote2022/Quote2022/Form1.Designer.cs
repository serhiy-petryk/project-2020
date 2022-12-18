
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
            this.components = new System.ComponentModel.Container();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabLoader = new System.Windows.Forms.TabPage();
            this.btnSymbolsNasdaqParse = new System.Windows.Forms.Button();
            this.btnSymbolsStockanalysisParse = new System.Windows.Forms.Button();
            this.btnSymbolsStockanalysisDownload = new System.Windows.Forms.Button();
            this.btnQuantumonlineProfilesParse = new System.Windows.Forms.Button();
            this.btnSplitInvestingHistoryParse = new System.Windows.Forms.Button();
            this.btnStockSplitHistoryParse = new System.Windows.Forms.Button();
            this.btnNasdaqStockScreener = new System.Windows.Forms.Button();
            this.btnDailyEoddataCheck = new System.Windows.Forms.Button();
            this.btnSplitEoddataParse = new System.Windows.Forms.Button();
            this.btnSplitInvestingParse = new System.Windows.Forms.Button();
            this.btnSplitYahooParse = new System.Windows.Forms.Button();
            this.btnTemp = new System.Windows.Forms.Button();
            this.btnSymbolsEoddataParse = new System.Windows.Forms.Button();
            this.btnDayEoddataParse = new System.Windows.Forms.Button();
            this.btnNanexSymbols = new System.Windows.Forms.Button();
            this.btnDayYahooIndexesParse = new System.Windows.Forms.Button();
            this.btnDayYahooParse = new System.Windows.Forms.Button();
            this.tabLayers = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.gbDataSet = new System.Windows.Forms.GroupBox();
            this.cb2013 = new System.Windows.Forms.CheckBox();
            this.cb2022 = new System.Windows.Forms.CheckBox();
            this.btnAlgorithm1 = new System.Windows.Forms.Button();
            this.btnToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.btnSymbolsNasdaqParseAll = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabLoader.SuspendLayout();
            this.tabLayers.SuspendLayout();
            this.gbDataSet.SuspendLayout();
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
            this.tabLoader.Controls.Add(this.btnSymbolsNasdaqParseAll);
            this.tabLoader.Controls.Add(this.btnSymbolsNasdaqParse);
            this.tabLoader.Controls.Add(this.btnSymbolsStockanalysisParse);
            this.tabLoader.Controls.Add(this.btnSymbolsStockanalysisDownload);
            this.tabLoader.Controls.Add(this.btnQuantumonlineProfilesParse);
            this.tabLoader.Controls.Add(this.btnSplitInvestingHistoryParse);
            this.tabLoader.Controls.Add(this.btnStockSplitHistoryParse);
            this.tabLoader.Controls.Add(this.btnNasdaqStockScreener);
            this.tabLoader.Controls.Add(this.btnDailyEoddataCheck);
            this.tabLoader.Controls.Add(this.btnSplitEoddataParse);
            this.tabLoader.Controls.Add(this.btnSplitInvestingParse);
            this.tabLoader.Controls.Add(this.btnSplitYahooParse);
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
            // btnSymbolsNasdaqParse
            // 
            this.btnSymbolsNasdaqParse.Location = new System.Drawing.Point(185, 120);
            this.btnSymbolsNasdaqParse.Name = "btnSymbolsNasdaqParse";
            this.btnSymbolsNasdaqParse.Size = new System.Drawing.Size(146, 23);
            this.btnSymbolsNasdaqParse.TabIndex = 31;
            this.btnSymbolsNasdaqParse.Text = "Symbols Nasdaq Parse";
            this.btnSymbolsNasdaqParse.UseVisualStyleBackColor = true;
            this.btnSymbolsNasdaqParse.Click += new System.EventHandler(this.btnSymbolsNasdaqParse_Click);
            // 
            // btnSymbolsStockanalysisParse
            // 
            this.btnSymbolsStockanalysisParse.Location = new System.Drawing.Point(539, 121);
            this.btnSymbolsStockanalysisParse.Name = "btnSymbolsStockanalysisParse";
            this.btnSymbolsStockanalysisParse.Size = new System.Drawing.Size(194, 23);
            this.btnSymbolsStockanalysisParse.TabIndex = 30;
            this.btnSymbolsStockanalysisParse.Text = "Symbols Stockanalysis Parse";
            this.btnSymbolsStockanalysisParse.UseVisualStyleBackColor = true;
            this.btnSymbolsStockanalysisParse.Click += new System.EventHandler(this.btnSymbolsStockanalysisParse_Click);
            // 
            // btnSymbolsStockanalysisDownload
            // 
            this.btnSymbolsStockanalysisDownload.Location = new System.Drawing.Point(539, 82);
            this.btnSymbolsStockanalysisDownload.Name = "btnSymbolsStockanalysisDownload";
            this.btnSymbolsStockanalysisDownload.Size = new System.Drawing.Size(194, 23);
            this.btnSymbolsStockanalysisDownload.TabIndex = 29;
            this.btnSymbolsStockanalysisDownload.Text = "Symbols Stockanalysis Download";
            this.btnSymbolsStockanalysisDownload.UseVisualStyleBackColor = true;
            this.btnSymbolsStockanalysisDownload.Click += new System.EventHandler(this.btnSymbolsStockanalysisDownload_Click);
            // 
            // btnQuantumonlineProfilesParse
            // 
            this.btnQuantumonlineProfilesParse.Location = new System.Drawing.Point(354, 121);
            this.btnQuantumonlineProfilesParse.Name = "btnQuantumonlineProfilesParse";
            this.btnQuantumonlineProfilesParse.Size = new System.Drawing.Size(164, 23);
            this.btnQuantumonlineProfilesParse.TabIndex = 28;
            this.btnQuantumonlineProfilesParse.Text = "Quantumonline Profiles Parse";
            this.btnQuantumonlineProfilesParse.UseVisualStyleBackColor = true;
            this.btnQuantumonlineProfilesParse.Click += new System.EventHandler(this.btnQuantumonlineProfilesParse_Click);
            // 
            // btnSplitInvestingHistoryParse
            // 
            this.btnSplitInvestingHistoryParse.Location = new System.Drawing.Point(354, 6);
            this.btnSplitInvestingHistoryParse.Name = "btnSplitInvestingHistoryParse";
            this.btnSplitInvestingHistoryParse.Size = new System.Drawing.Size(146, 23);
            this.btnSplitInvestingHistoryParse.TabIndex = 27;
            this.btnSplitInvestingHistoryParse.Text = "Split InvestingHistory Parse";
            this.btnSplitInvestingHistoryParse.UseVisualStyleBackColor = true;
            this.btnSplitInvestingHistoryParse.Click += new System.EventHandler(this.btnSplitInvestingHistoryParse_Click);
            // 
            // btnStockSplitHistoryParse
            // 
            this.btnStockSplitHistoryParse.Location = new System.Drawing.Point(354, 44);
            this.btnStockSplitHistoryParse.Name = "btnStockSplitHistoryParse";
            this.btnStockSplitHistoryParse.Size = new System.Drawing.Size(146, 23);
            this.btnStockSplitHistoryParse.TabIndex = 26;
            this.btnStockSplitHistoryParse.Text = "StockSplitHistory Parse";
            this.btnStockSplitHistoryParse.UseVisualStyleBackColor = true;
            this.btnStockSplitHistoryParse.Click += new System.EventHandler(this.btnStockSplitHistoryParse_Click);
            // 
            // btnNasdaqStockScreener
            // 
            this.btnNasdaqStockScreener.Location = new System.Drawing.Point(539, 6);
            this.btnNasdaqStockScreener.Name = "btnNasdaqStockScreener";
            this.btnNasdaqStockScreener.Size = new System.Drawing.Size(146, 23);
            this.btnNasdaqStockScreener.TabIndex = 25;
            this.btnNasdaqStockScreener.Text = "Nasdaq Stock Screener";
            this.btnNasdaqStockScreener.UseVisualStyleBackColor = true;
            this.btnNasdaqStockScreener.Click += new System.EventHandler(this.btnNasdaqStockScreener_Click);
            // 
            // btnDailyEoddataCheck
            // 
            this.btnDailyEoddataCheck.Location = new System.Drawing.Point(738, 6);
            this.btnDailyEoddataCheck.Name = "btnDailyEoddataCheck";
            this.btnDailyEoddataCheck.Size = new System.Drawing.Size(146, 23);
            this.btnDailyEoddataCheck.TabIndex = 24;
            this.btnDailyEoddataCheck.Text = "Daily Eoddata Check";
            this.btnToolTip.SetToolTip(this.btnDailyEoddataCheck, "Copy data from text file to DB and then convert DB data to text and compare data");
            this.btnDailyEoddataCheck.UseVisualStyleBackColor = true;
            this.btnDailyEoddataCheck.Click += new System.EventHandler(this.btnDailyEoddataCheck_Click);
            // 
            // btnSplitEoddataParse
            // 
            this.btnSplitEoddataParse.Location = new System.Drawing.Point(354, 177);
            this.btnSplitEoddataParse.Name = "btnSplitEoddataParse";
            this.btnSplitEoddataParse.Size = new System.Drawing.Size(146, 23);
            this.btnSplitEoddataParse.TabIndex = 23;
            this.btnSplitEoddataParse.Text = "Split Eoddata Parse";
            this.btnSplitEoddataParse.UseVisualStyleBackColor = true;
            this.btnSplitEoddataParse.Click += new System.EventHandler(this.btnSplitEoddataParse_Click);
            // 
            // btnSplitInvestingParse
            // 
            this.btnSplitInvestingParse.Location = new System.Drawing.Point(354, 217);
            this.btnSplitInvestingParse.Name = "btnSplitInvestingParse";
            this.btnSplitInvestingParse.Size = new System.Drawing.Size(146, 23);
            this.btnSplitInvestingParse.TabIndex = 22;
            this.btnSplitInvestingParse.Text = "Split Investing.com Parse";
            this.btnSplitInvestingParse.UseVisualStyleBackColor = true;
            this.btnSplitInvestingParse.Click += new System.EventHandler(this.btnSplitInvestingParse_Click);
            // 
            // btnSplitYahooParse
            // 
            this.btnSplitYahooParse.Location = new System.Drawing.Point(354, 82);
            this.btnSplitYahooParse.Name = "btnSplitYahooParse";
            this.btnSplitYahooParse.Size = new System.Drawing.Size(146, 23);
            this.btnSplitYahooParse.TabIndex = 21;
            this.btnSplitYahooParse.Text = "Split Yahoo Parse (zip)";
            this.btnSplitYahooParse.UseVisualStyleBackColor = true;
            this.btnSplitYahooParse.Click += new System.EventHandler(this.btnSplitYahooParse_Click);
            // 
            // btnTemp
            // 
            this.btnTemp.Location = new System.Drawing.Point(738, 44);
            this.btnTemp.Name = "btnTemp";
            this.btnTemp.Size = new System.Drawing.Size(81, 23);
            this.btnTemp.TabIndex = 17;
            this.btnTemp.Text = "Temp";
            this.btnTemp.UseVisualStyleBackColor = true;
            this.btnTemp.Click += new System.EventHandler(this.btnTemp_Click);
            // 
            // btnSymbolsEoddataParse
            // 
            this.btnSymbolsEoddataParse.Location = new System.Drawing.Point(185, 158);
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
            this.btnNanexSymbols.Location = new System.Drawing.Point(185, 44);
            this.btnNanexSymbols.Name = "btnNanexSymbols";
            this.btnNanexSymbols.Size = new System.Drawing.Size(146, 23);
            this.btnNanexSymbols.TabIndex = 14;
            this.btnNanexSymbols.Text = "?Nanex Symbols";
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
            this.btnDayYahooParse.Text = "?DayYahoo Parse";
            this.btnDayYahooParse.UseVisualStyleBackColor = true;
            this.btnDayYahooParse.Click += new System.EventHandler(this.btnDayYahooParse_Click);
            // 
            // tabLayers
            // 
            this.tabLayers.Controls.Add(this.button1);
            this.tabLayers.Controls.Add(this.gbDataSet);
            this.tabLayers.Controls.Add(this.btnAlgorithm1);
            this.tabLayers.Location = new System.Drawing.Point(4, 22);
            this.tabLayers.Name = "tabLayers";
            this.tabLayers.Padding = new System.Windows.Forms.Padding(3);
            this.tabLayers.Size = new System.Drawing.Size(949, 304);
            this.tabLayers.TabIndex = 1;
            this.tabLayers.Text = "Layers";
            this.tabLayers.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(426, 97);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 2;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // gbDataSet
            // 
            this.gbDataSet.Controls.Add(this.cb2013);
            this.gbDataSet.Controls.Add(this.cb2022);
            this.gbDataSet.Location = new System.Drawing.Point(23, 17);
            this.gbDataSet.Name = "gbDataSet";
            this.gbDataSet.Size = new System.Drawing.Size(76, 68);
            this.gbDataSet.TabIndex = 1;
            this.gbDataSet.TabStop = false;
            this.gbDataSet.Text = "Data Set";
            // 
            // cb2013
            // 
            this.cb2013.AutoSize = true;
            this.cb2013.Location = new System.Drawing.Point(6, 42);
            this.cb2013.Name = "cb2013";
            this.cb2013.Size = new System.Drawing.Size(50, 17);
            this.cb2013.TabIndex = 3;
            this.cb2013.Text = "2013";
            this.cb2013.UseVisualStyleBackColor = true;
            // 
            // cb2022
            // 
            this.cb2022.AutoSize = true;
            this.cb2022.Checked = true;
            this.cb2022.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cb2022.Location = new System.Drawing.Point(6, 19);
            this.cb2022.Name = "cb2022";
            this.cb2022.Size = new System.Drawing.Size(50, 17);
            this.cb2022.TabIndex = 2;
            this.cb2022.Text = "2022";
            this.cb2022.UseVisualStyleBackColor = true;
            // 
            // btnAlgorithm1
            // 
            this.btnAlgorithm1.Location = new System.Drawing.Point(23, 97);
            this.btnAlgorithm1.Name = "btnAlgorithm1";
            this.btnAlgorithm1.Size = new System.Drawing.Size(75, 23);
            this.btnAlgorithm1.TabIndex = 0;
            this.btnAlgorithm1.Text = "Algorithm 1";
            this.btnAlgorithm1.UseVisualStyleBackColor = true;
            this.btnAlgorithm1.Click += new System.EventHandler(this.btnAlgorithm1_Click);
            // 
            // btnSymbolsNasdaqParseAll
            // 
            this.btnSymbolsNasdaqParseAll.Location = new System.Drawing.Point(185, 6);
            this.btnSymbolsNasdaqParseAll.Name = "btnSymbolsNasdaqParseAll";
            this.btnSymbolsNasdaqParseAll.Size = new System.Drawing.Size(146, 23);
            this.btnSymbolsNasdaqParseAll.TabIndex = 32;
            this.btnSymbolsNasdaqParseAll.Text = "Symbols Nasdaq Parse (all)";
            this.btnSymbolsNasdaqParseAll.UseVisualStyleBackColor = true;
            this.btnSymbolsNasdaqParseAll.Click += new System.EventHandler(this.btnSymbolsNasdaqParseAll_Click);
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
            this.gbDataSet.ResumeLayout(false);
            this.gbDataSet.PerformLayout();
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
        private System.Windows.Forms.Button btnTemp;
        private System.Windows.Forms.Button btnSymbolsEoddataParse;
        private System.Windows.Forms.Button btnDayEoddataParse;
        private System.Windows.Forms.Button btnNanexSymbols;
        private System.Windows.Forms.Button btnDayYahooIndexesParse;
        private System.Windows.Forms.Button btnDayYahooParse;
        private System.Windows.Forms.Button btnSplitEoddataParse;
        private System.Windows.Forms.Button btnAlgorithm1;
        private System.Windows.Forms.GroupBox gbDataSet;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox cb2013;
        private System.Windows.Forms.CheckBox cb2022;
        private System.Windows.Forms.Button btnDailyEoddataCheck;
        private System.Windows.Forms.Button btnNasdaqStockScreener;
        private System.Windows.Forms.Button btnStockSplitHistoryParse;
        private System.Windows.Forms.Button btnSplitInvestingHistoryParse;
        private System.Windows.Forms.ToolTip btnToolTip;
        private System.Windows.Forms.Button btnQuantumonlineProfilesParse;
        private System.Windows.Forms.Button btnSymbolsStockanalysisDownload;
        private System.Windows.Forms.Button btnSymbolsStockanalysisParse;
        private System.Windows.Forms.Button btnSymbolsNasdaqParse;
        private System.Windows.Forms.Button btnSymbolsNasdaqParseAll;
    }
}

