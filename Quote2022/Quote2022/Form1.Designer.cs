
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
            this.btnTradingViewScreenerParse = new System.Windows.Forms.Button();
            this.btnScreenerTradingViewDownload = new System.Windows.Forms.Button();
            this.btnMinuteYahooCheck = new System.Windows.Forms.Button();
            this.btnDayYahooDownload = new System.Windows.Forms.Button();
            this.btnSymbolsYahooLookupParse = new System.Windows.Forms.Button();
            this.btnSymbolsYahooLookupDownload = new System.Windows.Forms.Button();
            this.btnUpdateTradingDays = new System.Windows.Forms.Button();
            this.btnTimeSalesNasdaqSaveSummary = new System.Windows.Forms.Button();
            this.btnRefreshSpitsData = new System.Windows.Forms.Button();
            this.btnTimeSalesNasdaqSaveLog = new System.Windows.Forms.Button();
            this.btnSymbolsQuantumonlineParse = new System.Windows.Forms.Button();
            this.btnSymbolsQuantumonlineDownload = new System.Windows.Forms.Button();
            this.btnTimeSalesNasdaqDownload = new System.Windows.Forms.Button();
            this.btnRefreshSymbolsData = new System.Windows.Forms.Button();
            this.btnSymbolsNasdaqParse = new System.Windows.Forms.Button();
            this.btnSymbolsStockanalysisParse = new System.Windows.Forms.Button();
            this.btnSymbolsStockanalysisDownload = new System.Windows.Forms.Button();
            this.btnQuantumonlineProfilesParse = new System.Windows.Forms.Button();
            this.btnSplitInvestingHistoryParse = new System.Windows.Forms.Button();
            this.btnStockSplitHistoryParse = new System.Windows.Forms.Button();
            this.btnScreenerNasdaqParse = new System.Windows.Forms.Button();
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
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnIntradayPrintDetails = new System.Windows.Forms.Button();
            this.gbIntradayDataList = new System.Windows.Forms.GroupBox();
            this.clbIntradayDataList = new System.Windows.Forms.CheckedListBox();
            this.btnIntradayGenerateReport = new System.Windows.Forms.Button();
            this.cbUseZipCache = new System.Windows.Forms.CheckBox();
            this.button4 = new System.Windows.Forms.Button();
            this.btnPrepareYahooMinuteTextCache = new System.Windows.Forms.Button();
            this.btnCheckYahooMinuteData = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.cbUseLastQuotes = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbPartialDayBy90 = new System.Windows.Forms.RadioButton();
            this.rbFullDayBy90 = new System.Windows.Forms.RadioButton();
            this.rbPartialDayBy30 = new System.Windows.Forms.RadioButton();
            this.rbFullDayBy30 = new System.Windows.Forms.RadioButton();
            this.btnToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabLoader.SuspendLayout();
            this.tabLayers.SuspendLayout();
            this.gbDataSet.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.gbIntradayDataList.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 437);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1077, 22);
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
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1077, 437);
            this.tabControl1.TabIndex = 12;
            // 
            // tabLoader
            // 
            this.tabLoader.Controls.Add(this.btnTradingViewScreenerParse);
            this.tabLoader.Controls.Add(this.btnScreenerTradingViewDownload);
            this.tabLoader.Controls.Add(this.btnMinuteYahooCheck);
            this.tabLoader.Controls.Add(this.btnDayYahooDownload);
            this.tabLoader.Controls.Add(this.btnSymbolsYahooLookupParse);
            this.tabLoader.Controls.Add(this.btnSymbolsYahooLookupDownload);
            this.tabLoader.Controls.Add(this.btnUpdateTradingDays);
            this.tabLoader.Controls.Add(this.btnTimeSalesNasdaqSaveSummary);
            this.tabLoader.Controls.Add(this.btnRefreshSpitsData);
            this.tabLoader.Controls.Add(this.btnTimeSalesNasdaqSaveLog);
            this.tabLoader.Controls.Add(this.btnSymbolsQuantumonlineParse);
            this.tabLoader.Controls.Add(this.btnSymbolsQuantumonlineDownload);
            this.tabLoader.Controls.Add(this.btnTimeSalesNasdaqDownload);
            this.tabLoader.Controls.Add(this.btnRefreshSymbolsData);
            this.tabLoader.Controls.Add(this.btnSymbolsNasdaqParse);
            this.tabLoader.Controls.Add(this.btnSymbolsStockanalysisParse);
            this.tabLoader.Controls.Add(this.btnSymbolsStockanalysisDownload);
            this.tabLoader.Controls.Add(this.btnQuantumonlineProfilesParse);
            this.tabLoader.Controls.Add(this.btnSplitInvestingHistoryParse);
            this.tabLoader.Controls.Add(this.btnStockSplitHistoryParse);
            this.tabLoader.Controls.Add(this.btnScreenerNasdaqParse);
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
            this.tabLoader.Size = new System.Drawing.Size(1069, 411);
            this.tabLoader.TabIndex = 0;
            this.tabLoader.Text = "Loader";
            this.tabLoader.UseVisualStyleBackColor = true;
            // 
            // btnTradingViewScreenerParse
            // 
            this.btnTradingViewScreenerParse.Location = new System.Drawing.Point(185, 162);
            this.btnTradingViewScreenerParse.Name = "btnTradingViewScreenerParse";
            this.btnTradingViewScreenerParse.Size = new System.Drawing.Size(173, 23);
            this.btnTradingViewScreenerParse.TabIndex = 48;
            this.btnTradingViewScreenerParse.Text = "TradingView Screener Parse";
            this.btnTradingViewScreenerParse.UseVisualStyleBackColor = true;
            this.btnTradingViewScreenerParse.Click += new System.EventHandler(this.btnTradingViewScreenerParse_Click);
            // 
            // btnScreenerTradingViewDownload
            // 
            this.btnScreenerTradingViewDownload.Location = new System.Drawing.Point(185, 133);
            this.btnScreenerTradingViewDownload.Name = "btnScreenerTradingViewDownload";
            this.btnScreenerTradingViewDownload.Size = new System.Drawing.Size(173, 23);
            this.btnScreenerTradingViewDownload.TabIndex = 47;
            this.btnScreenerTradingViewDownload.Text = "TradingView Screener Download";
            this.btnScreenerTradingViewDownload.UseVisualStyleBackColor = true;
            this.btnScreenerTradingViewDownload.Click += new System.EventHandler(this.btnScreenerTradingViewDownload_Click);
            // 
            // btnMinuteYahooCheck
            // 
            this.btnMinuteYahooCheck.Location = new System.Drawing.Point(753, 246);
            this.btnMinuteYahooCheck.Name = "btnMinuteYahooCheck";
            this.btnMinuteYahooCheck.Size = new System.Drawing.Size(146, 23);
            this.btnMinuteYahooCheck.TabIndex = 46;
            this.btnMinuteYahooCheck.Text = "Minute Yahoo Check";
            this.btnToolTip.SetToolTip(this.btnMinuteYahooCheck, "Copy data from text file to DB and then convert DB data to text and compare data");
            this.btnMinuteYahooCheck.UseVisualStyleBackColor = true;
            this.btnMinuteYahooCheck.Click += new System.EventHandler(this.btnMinuteYahooCheck_Click);
            // 
            // btnDayYahooDownload
            // 
            this.btnDayYahooDownload.Location = new System.Drawing.Point(15, 6);
            this.btnDayYahooDownload.Name = "btnDayYahooDownload";
            this.btnDayYahooDownload.Size = new System.Drawing.Size(146, 23);
            this.btnDayYahooDownload.TabIndex = 45;
            this.btnDayYahooDownload.Text = "?DayYahoo Download";
            this.btnDayYahooDownload.UseVisualStyleBackColor = true;
            this.btnDayYahooDownload.Click += new System.EventHandler(this.btnDayYahooDownload_Click);
            // 
            // btnSymbolsYahooLookupParse
            // 
            this.btnSymbolsYahooLookupParse.Location = new System.Drawing.Point(185, 64);
            this.btnSymbolsYahooLookupParse.Name = "btnSymbolsYahooLookupParse";
            this.btnSymbolsYahooLookupParse.Size = new System.Drawing.Size(181, 23);
            this.btnSymbolsYahooLookupParse.TabIndex = 44;
            this.btnSymbolsYahooLookupParse.Text = "Symbols Yahoo Lookup Parse";
            this.btnSymbolsYahooLookupParse.UseVisualStyleBackColor = true;
            this.btnSymbolsYahooLookupParse.Click += new System.EventHandler(this.btnSymbolsYahooLookupParse_Click);
            // 
            // btnSymbolsYahooLookupDownload
            // 
            this.btnSymbolsYahooLookupDownload.Location = new System.Drawing.Point(185, 35);
            this.btnSymbolsYahooLookupDownload.Name = "btnSymbolsYahooLookupDownload";
            this.btnSymbolsYahooLookupDownload.Size = new System.Drawing.Size(181, 23);
            this.btnSymbolsYahooLookupDownload.TabIndex = 43;
            this.btnSymbolsYahooLookupDownload.Text = "Symbols Yahoo Lookup Download";
            this.btnSymbolsYahooLookupDownload.UseVisualStyleBackColor = true;
            this.btnSymbolsYahooLookupDownload.Click += new System.EventHandler(this.btnSymbolsYahooLookupDownload_Click);
            // 
            // btnUpdateTradingDays
            // 
            this.btnUpdateTradingDays.Location = new System.Drawing.Point(15, 243);
            this.btnUpdateTradingDays.Name = "btnUpdateTradingDays";
            this.btnUpdateTradingDays.Size = new System.Drawing.Size(146, 23);
            this.btnUpdateTradingDays.TabIndex = 42;
            this.btnUpdateTradingDays.Text = "Update Trading Days";
            this.btnUpdateTradingDays.UseVisualStyleBackColor = true;
            this.btnUpdateTradingDays.Click += new System.EventHandler(this.btnUpdateTradingDays_Click);
            // 
            // btnTimeSalesNasdaqSaveSummary
            // 
            this.btnTimeSalesNasdaqSaveSummary.Enabled = false;
            this.btnTimeSalesNasdaqSaveSummary.Location = new System.Drawing.Point(753, 162);
            this.btnTimeSalesNasdaqSaveSummary.Name = "btnTimeSalesNasdaqSaveSummary";
            this.btnTimeSalesNasdaqSaveSummary.Size = new System.Drawing.Size(178, 23);
            this.btnTimeSalesNasdaqSaveSummary.TabIndex = 41;
            this.btnTimeSalesNasdaqSaveSummary.Text = "TimeSalesNasdaq Save Summary";
            this.btnToolTip.SetToolTip(this.btnTimeSalesNasdaqSaveSummary, "Copy data from text file to DB and then convert DB data to text and compare data");
            this.btnTimeSalesNasdaqSaveSummary.UseVisualStyleBackColor = true;
            this.btnTimeSalesNasdaqSaveSummary.Click += new System.EventHandler(this.btnTimeSalesNasdaqSaveSummary_Click);
            // 
            // btnRefreshSpitsData
            // 
            this.btnRefreshSpitsData.Location = new System.Drawing.Point(364, 321);
            this.btnRefreshSpitsData.Name = "btnRefreshSpitsData";
            this.btnRefreshSpitsData.Size = new System.Drawing.Size(146, 23);
            this.btnRefreshSpitsData.TabIndex = 40;
            this.btnRefreshSpitsData.Text = "Refresh Spits Data";
            this.btnRefreshSpitsData.UseVisualStyleBackColor = true;
            this.btnRefreshSpitsData.Click += new System.EventHandler(this.btnRefreshSpitsData_Click);
            // 
            // btnTimeSalesNasdaqSaveLog
            // 
            this.btnTimeSalesNasdaqSaveLog.Enabled = false;
            this.btnTimeSalesNasdaqSaveLog.Location = new System.Drawing.Point(753, 133);
            this.btnTimeSalesNasdaqSaveLog.Name = "btnTimeSalesNasdaqSaveLog";
            this.btnTimeSalesNasdaqSaveLog.Size = new System.Drawing.Size(178, 23);
            this.btnTimeSalesNasdaqSaveLog.TabIndex = 38;
            this.btnTimeSalesNasdaqSaveLog.Text = "TimeSalesNasdaq Save Log";
            this.btnToolTip.SetToolTip(this.btnTimeSalesNasdaqSaveLog, "Copy data from text file to DB and then convert DB data to text and compare data");
            this.btnTimeSalesNasdaqSaveLog.UseVisualStyleBackColor = true;
            this.btnTimeSalesNasdaqSaveLog.Click += new System.EventHandler(this.btnTimeSalesNasdaqSaveLog_Click);
            // 
            // btnSymbolsQuantumonlineParse
            // 
            this.btnSymbolsQuantumonlineParse.Location = new System.Drawing.Point(753, 44);
            this.btnSymbolsQuantumonlineParse.Name = "btnSymbolsQuantumonlineParse";
            this.btnSymbolsQuantumonlineParse.Size = new System.Drawing.Size(187, 23);
            this.btnSymbolsQuantumonlineParse.TabIndex = 37;
            this.btnSymbolsQuantumonlineParse.Text = "SymbolsQuantumonline Parse";
            this.btnSymbolsQuantumonlineParse.UseVisualStyleBackColor = true;
            this.btnSymbolsQuantumonlineParse.Click += new System.EventHandler(this.btnSymbolsQuantumonlineParse_Click);
            // 
            // btnSymbolsQuantumonlineDownload
            // 
            this.btnSymbolsQuantumonlineDownload.Location = new System.Drawing.Point(753, 6);
            this.btnSymbolsQuantumonlineDownload.Name = "btnSymbolsQuantumonlineDownload";
            this.btnSymbolsQuantumonlineDownload.Size = new System.Drawing.Size(187, 23);
            this.btnSymbolsQuantumonlineDownload.TabIndex = 36;
            this.btnSymbolsQuantumonlineDownload.Text = "SymbolsQuantumonline Download";
            this.btnSymbolsQuantumonlineDownload.UseVisualStyleBackColor = true;
            this.btnSymbolsQuantumonlineDownload.Click += new System.EventHandler(this.btnSymbolsQuantumonlineDownload_Click);
            // 
            // btnTimeSalesNasdaqDownload
            // 
            this.btnTimeSalesNasdaqDownload.Enabled = false;
            this.btnTimeSalesNasdaqDownload.Location = new System.Drawing.Point(15, 82);
            this.btnTimeSalesNasdaqDownload.Name = "btnTimeSalesNasdaqDownload";
            this.btnTimeSalesNasdaqDownload.Size = new System.Drawing.Size(159, 23);
            this.btnTimeSalesNasdaqDownload.TabIndex = 35;
            this.btnTimeSalesNasdaqDownload.Text = "TimeSalesNasdaq Download";
            this.btnTimeSalesNasdaqDownload.UseVisualStyleBackColor = true;
            this.btnTimeSalesNasdaqDownload.Click += new System.EventHandler(this.btnTimeSalesNasdaqDownload_Click);
            // 
            // btnRefreshSymbolsData
            // 
            this.btnRefreshSymbolsData.Location = new System.Drawing.Point(185, 321);
            this.btnRefreshSymbolsData.Name = "btnRefreshSymbolsData";
            this.btnRefreshSymbolsData.Size = new System.Drawing.Size(146, 23);
            this.btnRefreshSymbolsData.TabIndex = 34;
            this.btnRefreshSymbolsData.Text = "Refresh Symbols Data";
            this.btnRefreshSymbolsData.UseVisualStyleBackColor = true;
            this.btnRefreshSymbolsData.Click += new System.EventHandler(this.btnRefreshSymbolsData_Click);
            // 
            // btnSymbolsNasdaqParse
            // 
            this.btnSymbolsNasdaqParse.Location = new System.Drawing.Point(185, 243);
            this.btnSymbolsNasdaqParse.Name = "btnSymbolsNasdaqParse";
            this.btnSymbolsNasdaqParse.Size = new System.Drawing.Size(146, 23);
            this.btnSymbolsNasdaqParse.TabIndex = 31;
            this.btnSymbolsNasdaqParse.Text = "Symbols Nasdaq Parse";
            this.btnSymbolsNasdaqParse.UseVisualStyleBackColor = true;
            this.btnSymbolsNasdaqParse.Click += new System.EventHandler(this.btnSymbolsNasdaqParse_Click);
            // 
            // btnSymbolsStockanalysisParse
            // 
            this.btnSymbolsStockanalysisParse.Location = new System.Drawing.Point(539, 217);
            this.btnSymbolsStockanalysisParse.Name = "btnSymbolsStockanalysisParse";
            this.btnSymbolsStockanalysisParse.Size = new System.Drawing.Size(194, 23);
            this.btnSymbolsStockanalysisParse.TabIndex = 30;
            this.btnSymbolsStockanalysisParse.Text = "Symbols Stockanalysis Parse";
            this.btnSymbolsStockanalysisParse.UseVisualStyleBackColor = true;
            this.btnSymbolsStockanalysisParse.Click += new System.EventHandler(this.btnSymbolsStockanalysisParse_Click);
            // 
            // btnSymbolsStockanalysisDownload
            // 
            this.btnSymbolsStockanalysisDownload.Location = new System.Drawing.Point(539, 177);
            this.btnSymbolsStockanalysisDownload.Name = "btnSymbolsStockanalysisDownload";
            this.btnSymbolsStockanalysisDownload.Size = new System.Drawing.Size(194, 23);
            this.btnSymbolsStockanalysisDownload.TabIndex = 29;
            this.btnSymbolsStockanalysisDownload.Text = "Symbols Stockanalysis Download";
            this.btnSymbolsStockanalysisDownload.UseVisualStyleBackColor = true;
            this.btnSymbolsStockanalysisDownload.Click += new System.EventHandler(this.btnSymbolsStockanalysisDownload_Click);
            // 
            // btnQuantumonlineProfilesParse
            // 
            this.btnQuantumonlineProfilesParse.Location = new System.Drawing.Point(753, 82);
            this.btnQuantumonlineProfilesParse.Name = "btnQuantumonlineProfilesParse";
            this.btnQuantumonlineProfilesParse.Size = new System.Drawing.Size(164, 23);
            this.btnQuantumonlineProfilesParse.TabIndex = 28;
            this.btnQuantumonlineProfilesParse.Text = "Quantumonline Profiles Parse";
            this.btnQuantumonlineProfilesParse.UseVisualStyleBackColor = true;
            this.btnQuantumonlineProfilesParse.Click += new System.EventHandler(this.btnQuantumonlineProfilesParse_Click);
            // 
            // btnSplitInvestingHistoryParse
            // 
            this.btnSplitInvestingHistoryParse.Location = new System.Drawing.Point(539, 6);
            this.btnSplitInvestingHistoryParse.Name = "btnSplitInvestingHistoryParse";
            this.btnSplitInvestingHistoryParse.Size = new System.Drawing.Size(146, 23);
            this.btnSplitInvestingHistoryParse.TabIndex = 27;
            this.btnSplitInvestingHistoryParse.Text = "Split InvestingHistory Parse";
            this.btnSplitInvestingHistoryParse.UseVisualStyleBackColor = true;
            this.btnSplitInvestingHistoryParse.Click += new System.EventHandler(this.btnSplitInvestingHistoryParse_Click);
            // 
            // btnStockSplitHistoryParse
            // 
            this.btnStockSplitHistoryParse.Location = new System.Drawing.Point(539, 44);
            this.btnStockSplitHistoryParse.Name = "btnStockSplitHistoryParse";
            this.btnStockSplitHistoryParse.Size = new System.Drawing.Size(146, 23);
            this.btnStockSplitHistoryParse.TabIndex = 26;
            this.btnStockSplitHistoryParse.Text = "StockSplitHistory Parse";
            this.btnStockSplitHistoryParse.UseVisualStyleBackColor = true;
            this.btnStockSplitHistoryParse.Click += new System.EventHandler(this.btnStockSplitHistoryParse_Click);
            // 
            // btnScreenerNasdaqParse
            // 
            this.btnScreenerNasdaqParse.Location = new System.Drawing.Point(185, 203);
            this.btnScreenerNasdaqParse.Name = "btnScreenerNasdaqParse";
            this.btnScreenerNasdaqParse.Size = new System.Drawing.Size(173, 23);
            this.btnScreenerNasdaqParse.TabIndex = 25;
            this.btnScreenerNasdaqParse.Text = "Nasdaq Stock Screener Parse";
            this.btnScreenerNasdaqParse.UseVisualStyleBackColor = true;
            this.btnScreenerNasdaqParse.Click += new System.EventHandler(this.btnParseScreenerNasdaqParse_Click);
            // 
            // btnDailyEoddataCheck
            // 
            this.btnDailyEoddataCheck.Enabled = false;
            this.btnDailyEoddataCheck.Location = new System.Drawing.Point(753, 217);
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
            this.btnSplitEoddataParse.Location = new System.Drawing.Point(364, 203);
            this.btnSplitEoddataParse.Name = "btnSplitEoddataParse";
            this.btnSplitEoddataParse.Size = new System.Drawing.Size(146, 23);
            this.btnSplitEoddataParse.TabIndex = 23;
            this.btnSplitEoddataParse.Text = "Split Eoddata Parse";
            this.btnSplitEoddataParse.UseVisualStyleBackColor = true;
            this.btnSplitEoddataParse.Click += new System.EventHandler(this.btnSplitEoddataParse_Click);
            // 
            // btnSplitInvestingParse
            // 
            this.btnSplitInvestingParse.Location = new System.Drawing.Point(364, 243);
            this.btnSplitInvestingParse.Name = "btnSplitInvestingParse";
            this.btnSplitInvestingParse.Size = new System.Drawing.Size(146, 23);
            this.btnSplitInvestingParse.TabIndex = 22;
            this.btnSplitInvestingParse.Text = "Split Investing.com Parse";
            this.btnSplitInvestingParse.UseVisualStyleBackColor = true;
            this.btnSplitInvestingParse.Click += new System.EventHandler(this.btnSplitInvestingParse_Click);
            // 
            // btnSplitYahooParse
            // 
            this.btnSplitYahooParse.Location = new System.Drawing.Point(539, 82);
            this.btnSplitYahooParse.Name = "btnSplitYahooParse";
            this.btnSplitYahooParse.Size = new System.Drawing.Size(146, 23);
            this.btnSplitYahooParse.TabIndex = 21;
            this.btnSplitYahooParse.Text = "Split Yahoo Parse (zip)";
            this.btnSplitYahooParse.UseVisualStyleBackColor = true;
            this.btnSplitYahooParse.Click += new System.EventHandler(this.btnSplitYahooParse_Click);
            // 
            // btnTemp
            // 
            this.btnTemp.Location = new System.Drawing.Point(778, 321);
            this.btnTemp.Name = "btnTemp";
            this.btnTemp.Size = new System.Drawing.Size(81, 23);
            this.btnTemp.TabIndex = 17;
            this.btnTemp.Text = "Temp";
            this.btnTemp.UseVisualStyleBackColor = true;
            this.btnTemp.Click += new System.EventHandler(this.btnTemp_Click);
            // 
            // btnSymbolsEoddataParse
            // 
            this.btnSymbolsEoddataParse.Location = new System.Drawing.Point(185, 281);
            this.btnSymbolsEoddataParse.Name = "btnSymbolsEoddataParse";
            this.btnSymbolsEoddataParse.Size = new System.Drawing.Size(146, 23);
            this.btnSymbolsEoddataParse.TabIndex = 16;
            this.btnSymbolsEoddataParse.Text = "Symbols Eoddata Parse";
            this.btnSymbolsEoddataParse.UseVisualStyleBackColor = true;
            this.btnSymbolsEoddataParse.Click += new System.EventHandler(this.btnSymbolsEoddataParse_Click);
            // 
            // btnDayEoddataParse
            // 
            this.btnDayEoddataParse.Location = new System.Drawing.Point(15, 281);
            this.btnDayEoddataParse.Name = "btnDayEoddataParse";
            this.btnDayEoddataParse.Size = new System.Drawing.Size(146, 23);
            this.btnDayEoddataParse.TabIndex = 15;
            this.btnDayEoddataParse.Text = "Daily Eoddata Parse";
            this.btnDayEoddataParse.UseVisualStyleBackColor = true;
            this.btnDayEoddataParse.Click += new System.EventHandler(this.btnDayEoddataParse_Click);
            // 
            // btnNanexSymbols
            // 
            this.btnNanexSymbols.Location = new System.Drawing.Point(185, 6);
            this.btnNanexSymbols.Name = "btnNanexSymbols";
            this.btnNanexSymbols.Size = new System.Drawing.Size(146, 23);
            this.btnNanexSymbols.TabIndex = 14;
            this.btnNanexSymbols.Text = "?Nanex Symbols";
            this.btnNanexSymbols.UseVisualStyleBackColor = true;
            this.btnNanexSymbols.Click += new System.EventHandler(this.btnSymbolsNanex_Click);
            // 
            // btnDayYahooIndexesParse
            // 
            this.btnDayYahooIndexesParse.Location = new System.Drawing.Point(15, 203);
            this.btnDayYahooIndexesParse.Name = "btnDayYahooIndexesParse";
            this.btnDayYahooIndexesParse.Size = new System.Drawing.Size(146, 23);
            this.btnDayYahooIndexesParse.TabIndex = 13;
            this.btnDayYahooIndexesParse.Text = "DayYahoo Indexes Parse";
            this.btnDayYahooIndexesParse.UseVisualStyleBackColor = true;
            this.btnDayYahooIndexesParse.Click += new System.EventHandler(this.btnDayYahooIndexesParse_Click);
            // 
            // btnDayYahooParse
            // 
            this.btnDayYahooParse.Location = new System.Drawing.Point(15, 35);
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
            this.tabLayers.Size = new System.Drawing.Size(1069, 411);
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
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnIntradayPrintDetails);
            this.tabPage1.Controls.Add(this.gbIntradayDataList);
            this.tabPage1.Controls.Add(this.btnIntradayGenerateReport);
            this.tabPage1.Controls.Add(this.cbUseZipCache);
            this.tabPage1.Controls.Add(this.button4);
            this.tabPage1.Controls.Add(this.btnPrepareYahooMinuteTextCache);
            this.tabPage1.Controls.Add(this.btnCheckYahooMinuteData);
            this.tabPage1.Controls.Add(this.button3);
            this.tabPage1.Controls.Add(this.cbUseLastQuotes);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1069, 411);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Intraday";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnIntradayPrintDetails
            // 
            this.btnIntradayPrintDetails.Location = new System.Drawing.Point(536, 150);
            this.btnIntradayPrintDetails.Name = "btnIntradayPrintDetails";
            this.btnIntradayPrintDetails.Size = new System.Drawing.Size(75, 23);
            this.btnIntradayPrintDetails.TabIndex = 29;
            this.btnIntradayPrintDetails.Text = "Print Details";
            this.btnIntradayPrintDetails.UseVisualStyleBackColor = true;
            this.btnIntradayPrintDetails.Click += new System.EventHandler(this.btnIntradayPrintDetails_Click);
            // 
            // gbIntradayDataList
            // 
            this.gbIntradayDataList.Controls.Add(this.clbIntradayDataList);
            this.gbIntradayDataList.Location = new System.Drawing.Point(210, 6);
            this.gbIntradayDataList.Name = "gbIntradayDataList";
            this.gbIntradayDataList.Size = new System.Drawing.Size(254, 370);
            this.gbIntradayDataList.TabIndex = 28;
            this.gbIntradayDataList.TabStop = false;
            this.gbIntradayDataList.Text = "Data List";
            // 
            // clbIntradayDataList
            // 
            this.clbIntradayDataList.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.clbIntradayDataList.CheckOnClick = true;
            this.clbIntradayDataList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.clbIntradayDataList.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.clbIntradayDataList.FormattingEnabled = true;
            this.clbIntradayDataList.Location = new System.Drawing.Point(3, 16);
            this.clbIntradayDataList.Name = "clbIntradayDataList";
            this.clbIntradayDataList.Size = new System.Drawing.Size(248, 351);
            this.clbIntradayDataList.TabIndex = 27;
            // 
            // btnIntradayGenerateReport
            // 
            this.btnIntradayGenerateReport.Location = new System.Drawing.Point(470, 350);
            this.btnIntradayGenerateReport.Name = "btnIntradayGenerateReport";
            this.btnIntradayGenerateReport.Size = new System.Drawing.Size(100, 23);
            this.btnIntradayGenerateReport.TabIndex = 27;
            this.btnIntradayGenerateReport.Text = "Generate report";
            this.btnIntradayGenerateReport.UseVisualStyleBackColor = true;
            this.btnIntradayGenerateReport.Click += new System.EventHandler(this.btnIntradayGenerateReport_Click);
            // 
            // cbUseZipCache
            // 
            this.cbUseZipCache.Checked = true;
            this.cbUseZipCache.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbUseZipCache.Location = new System.Drawing.Point(20, 256);
            this.cbUseZipCache.Name = "cbUseZipCache";
            this.cbUseZipCache.Size = new System.Drawing.Size(205, 24);
            this.cbUseZipCache.TabIndex = 21;
            this.cbUseZipCache.Text = "Use Zip Cache of Yahoo Minute data";
            this.cbUseZipCache.UseVisualStyleBackColor = true;
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(536, 74);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(75, 23);
            this.button4.TabIndex = 19;
            this.button4.Text = "button4";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // btnPrepareYahooMinuteTextCache
            // 
            this.btnPrepareYahooMinuteTextCache.Location = new System.Drawing.Point(17, 45);
            this.btnPrepareYahooMinuteTextCache.Name = "btnPrepareYahooMinuteTextCache";
            this.btnPrepareYahooMinuteTextCache.Size = new System.Drawing.Size(184, 23);
            this.btnPrepareYahooMinuteTextCache.TabIndex = 18;
            this.btnPrepareYahooMinuteTextCache.Text = "Prepare Yahoo Minute Text Cache";
            this.btnPrepareYahooMinuteTextCache.UseVisualStyleBackColor = true;
            this.btnPrepareYahooMinuteTextCache.Click += new System.EventHandler(this.btnPrepareYahooMinuteZipCache_Click);
            // 
            // btnCheckYahooMinuteData
            // 
            this.btnCheckYahooMinuteData.Location = new System.Drawing.Point(17, 16);
            this.btnCheckYahooMinuteData.Name = "btnCheckYahooMinuteData";
            this.btnCheckYahooMinuteData.Size = new System.Drawing.Size(184, 23);
            this.btnCheckYahooMinuteData.TabIndex = 17;
            this.btnCheckYahooMinuteData.Text = "Check Yahoo Minute Data";
            this.btnCheckYahooMinuteData.UseVisualStyleBackColor = true;
            this.btnCheckYahooMinuteData.Click += new System.EventHandler(this.btnCheckYahooMinuteData_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(536, 45);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 15;
            this.button3.Text = "button3";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // cbUseLastQuotes
            // 
            this.cbUseLastQuotes.Location = new System.Drawing.Point(20, 230);
            this.cbUseLastQuotes.Name = "cbUseLastQuotes";
            this.cbUseLastQuotes.Size = new System.Drawing.Size(138, 24);
            this.cbUseLastQuotes.TabIndex = 5;
            this.cbUseLastQuotes.Text = "Use Last Quotes";
            this.cbUseLastQuotes.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbPartialDayBy90);
            this.groupBox1.Controls.Add(this.rbFullDayBy90);
            this.groupBox1.Controls.Add(this.rbPartialDayBy30);
            this.groupBox1.Controls.Add(this.rbFullDayBy30);
            this.groupBox1.Location = new System.Drawing.Point(17, 94);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(187, 121);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Trading time range";
            // 
            // rbPartialDayBy90
            // 
            this.rbPartialDayBy90.AutoSize = true;
            this.rbPartialDayBy90.Location = new System.Drawing.Point(3, 85);
            this.rbPartialDayBy90.Name = "rbPartialDayBy90";
            this.rbPartialDayBy90.Size = new System.Drawing.Size(168, 17);
            this.rbPartialDayBy90.TabIndex = 7;
            this.rbPartialDayBy90.Text = "From 9:45 to 15:45 (by 90 min)";
            this.rbPartialDayBy90.UseVisualStyleBackColor = true;
            // 
            // rbFullDayBy90
            // 
            this.rbFullDayBy90.AutoSize = true;
            this.rbFullDayBy90.Location = new System.Drawing.Point(3, 62);
            this.rbFullDayBy90.Name = "rbFullDayBy90";
            this.rbFullDayBy90.Size = new System.Drawing.Size(138, 17);
            this.rbFullDayBy90.TabIndex = 6;
            this.rbFullDayBy90.Text = "Full day (by 90/105 min)";
            this.rbFullDayBy90.UseVisualStyleBackColor = true;
            // 
            // rbPartialDayBy30
            // 
            this.rbPartialDayBy30.AutoSize = true;
            this.rbPartialDayBy30.Location = new System.Drawing.Point(3, 39);
            this.rbPartialDayBy30.Name = "rbPartialDayBy30";
            this.rbPartialDayBy30.Size = new System.Drawing.Size(174, 17);
            this.rbPartialDayBy30.TabIndex = 5;
            this.rbPartialDayBy30.Text = "From 10:00 to 15:30 (by 30 min)";
            this.rbPartialDayBy30.UseVisualStyleBackColor = true;
            // 
            // rbFullDayBy30
            // 
            this.rbFullDayBy30.AutoSize = true;
            this.rbFullDayBy30.Checked = true;
            this.rbFullDayBy30.Dock = System.Windows.Forms.DockStyle.Top;
            this.rbFullDayBy30.Location = new System.Drawing.Point(3, 16);
            this.rbFullDayBy30.Name = "rbFullDayBy30";
            this.rbFullDayBy30.Size = new System.Drawing.Size(181, 17);
            this.rbFullDayBy30.TabIndex = 4;
            this.rbFullDayBy30.TabStop = true;
            this.rbFullDayBy30.Text = "Full day (by 30 min)";
            this.rbFullDayBy30.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1077, 459);
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
            this.tabPage1.ResumeLayout(false);
            this.gbIntradayDataList.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.Button btnScreenerNasdaqParse;
        private System.Windows.Forms.Button btnStockSplitHistoryParse;
        private System.Windows.Forms.Button btnSplitInvestingHistoryParse;
        private System.Windows.Forms.ToolTip btnToolTip;
        private System.Windows.Forms.Button btnQuantumonlineProfilesParse;
        private System.Windows.Forms.Button btnSymbolsStockanalysisDownload;
        private System.Windows.Forms.Button btnSymbolsStockanalysisParse;
        private System.Windows.Forms.Button btnSymbolsNasdaqParse;
        private System.Windows.Forms.Button btnRefreshSymbolsData;
        private System.Windows.Forms.Button btnTimeSalesNasdaqDownload;
        private System.Windows.Forms.Button btnSymbolsQuantumonlineDownload;
        private System.Windows.Forms.Button btnSymbolsQuantumonlineParse;
        private System.Windows.Forms.Button btnTimeSalesNasdaqSaveLog;
        private System.Windows.Forms.Button btnRefreshSpitsData;
        private System.Windows.Forms.Button btnTimeSalesNasdaqSaveSummary;
        private System.Windows.Forms.Button btnUpdateTradingDays;
        private System.Windows.Forms.Button btnSymbolsYahooLookupDownload;
        private System.Windows.Forms.Button btnSymbolsYahooLookupParse;
        private System.Windows.Forms.Button btnDayYahooDownload;
        private System.Windows.Forms.Button btnMinuteYahooCheck;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbPartialDayBy30;
        private System.Windows.Forms.RadioButton rbFullDayBy30;
        private System.Windows.Forms.RadioButton rbPartialDayBy90;
        private System.Windows.Forms.RadioButton rbFullDayBy90;
        private System.Windows.Forms.CheckBox cbUseLastQuotes;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button btnCheckYahooMinuteData;
        private System.Windows.Forms.Button btnPrepareYahooMinuteTextCache;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button btnScreenerTradingViewDownload;
        private System.Windows.Forms.Button btnTradingViewScreenerParse;
        private System.Windows.Forms.CheckBox cbUseZipCache;
        private System.Windows.Forms.Button btnIntradayGenerateReport;
        private System.Windows.Forms.GroupBox gbIntradayDataList;
        private System.Windows.Forms.CheckedListBox clbIntradayDataList;
        private System.Windows.Forms.Button btnIntradayPrintDetails;
    }
}

