﻿namespace OlxFlat
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
            this.btnOlxList_Parse = new System.Windows.Forms.Button();
            this.btnOlxList_LoadFromWeb = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblFirst = new System.Windows.Forms.ToolStripStatusLabel();
            this.lblSecond = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnOlxDetails_LoadFromWeb = new System.Windows.Forms.Button();
            this.btnOlxDetails_Parse = new System.Windows.Forms.Button();
            this.btnUpdateOlxData = new System.Windows.Forms.Button();
            this.btnOlxUpdateAll = new System.Windows.Forms.Button();
            this.btnDomRiaList_LoadFromWeb = new System.Windows.Forms.Button();
            this.btnDomRiaParseDetails = new System.Windows.Forms.Button();
            this.btnDomRiaUpdateAll = new System.Windows.Forms.Button();
            this.btnRealEstateList_LoadFromWeb = new System.Windows.Forms.Button();
            this.btnRealEstateList_Parse = new System.Windows.Forms.Button();
            this.btnRealEstateDetails_LoadFromWeb = new System.Windows.Forms.Button();
            this.btnRealEstateDetails_Parse = new System.Windows.Forms.Button();
            this.btnRealEstateUpdateAll = new System.Windows.Forms.Button();
            this.btnUpdateRealEstateData = new System.Windows.Forms.Button();
            this.btnUpdateDomRiaData = new System.Windows.Forms.Button();
            this.btnVN_House_List_LoadFromWeb = new System.Windows.Forms.Button();
            this.btnVNHousesParse = new System.Windows.Forms.Button();
            this.btnVN_House_Details_LoadFromWeb = new System.Windows.Forms.Button();
            this.btnVn_House_Details_Parse = new System.Windows.Forms.Button();
            this.btnUpdateVnData = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOlxList_Parse
            // 
            this.btnOlxList_Parse.Location = new System.Drawing.Point(350, 70);
            this.btnOlxList_Parse.Name = "btnOlxList_Parse";
            this.btnOlxList_Parse.Size = new System.Drawing.Size(221, 28);
            this.btnOlxList_Parse.TabIndex = 0;
            this.btnOlxList_Parse.Text = "2. Parse OLX list files and Save to DB";
            this.btnOlxList_Parse.UseVisualStyleBackColor = true;
            this.btnOlxList_Parse.Click += new System.EventHandler(this.btnOlxList_Parse_Click);
            // 
            // btnOlxList_LoadFromWeb
            // 
            this.btnOlxList_LoadFromWeb.Location = new System.Drawing.Point(350, 25);
            this.btnOlxList_LoadFromWeb.Name = "btnOlxList_LoadFromWeb";
            this.btnOlxList_LoadFromWeb.Size = new System.Drawing.Size(170, 31);
            this.btnOlxList_LoadFromWeb.TabIndex = 2;
            this.btnOlxList_LoadFromWeb.Text = "1. Load OLX list from Web";
            this.btnOlxList_LoadFromWeb.UseVisualStyleBackColor = true;
            this.btnOlxList_LoadFromWeb.Click += new System.EventHandler(this.btnOlxList_LoadFromWeb_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblFirst,
            this.lblSecond});
            this.statusStrip1.Location = new System.Drawing.Point(0, 369);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1158, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblFirst
            // 
            this.lblFirst.Name = "lblFirst";
            this.lblFirst.Size = new System.Drawing.Size(42, 17);
            this.lblFirst.Text = "lblFirst";
            // 
            // lblSecond
            // 
            this.lblSecond.Name = "lblSecond";
            this.lblSecond.Size = new System.Drawing.Size(59, 17);
            this.lblSecond.Text = "lblSecond";
            // 
            // btnOlxDetails_LoadFromWeb
            // 
            this.btnOlxDetails_LoadFromWeb.Location = new System.Drawing.Point(350, 116);
            this.btnOlxDetails_LoadFromWeb.Name = "btnOlxDetails_LoadFromWeb";
            this.btnOlxDetails_LoadFromWeb.Size = new System.Drawing.Size(170, 31);
            this.btnOlxDetails_LoadFromWeb.TabIndex = 4;
            this.btnOlxDetails_LoadFromWeb.Text = "3. Load OLX details from Web";
            this.btnOlxDetails_LoadFromWeb.UseVisualStyleBackColor = true;
            this.btnOlxDetails_LoadFromWeb.Click += new System.EventHandler(this.btnOlxDetails_LoadFromWeb_Click);
            // 
            // btnOlxDetails_Parse
            // 
            this.btnOlxDetails_Parse.Location = new System.Drawing.Point(350, 166);
            this.btnOlxDetails_Parse.Name = "btnOlxDetails_Parse";
            this.btnOlxDetails_Parse.Size = new System.Drawing.Size(221, 28);
            this.btnOlxDetails_Parse.TabIndex = 5;
            this.btnOlxDetails_Parse.Text = "4. Parse OLX details files and Save to DB";
            this.btnOlxDetails_Parse.UseVisualStyleBackColor = true;
            this.btnOlxDetails_Parse.Click += new System.EventHandler(this.btnOlxDetails_Parse_Click);
            // 
            // btnUpdateOlxData
            // 
            this.btnUpdateOlxData.Location = new System.Drawing.Point(350, 213);
            this.btnUpdateOlxData.Name = "btnUpdateOlxData";
            this.btnUpdateOlxData.Size = new System.Drawing.Size(170, 28);
            this.btnUpdateOlxData.TabIndex = 6;
            this.btnUpdateOlxData.Text = "5. Update OLX tables in DB";
            this.btnUpdateOlxData.UseVisualStyleBackColor = true;
            this.btnUpdateOlxData.Click += new System.EventHandler(this.btnUpdateOlxData_Click);
            // 
            // btnOlxUpdateAll
            // 
            this.btnOlxUpdateAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnOlxUpdateAll.Location = new System.Drawing.Point(27, 103);
            this.btnOlxUpdateAll.Name = "btnOlxUpdateAll";
            this.btnOlxUpdateAll.Size = new System.Drawing.Size(278, 57);
            this.btnOlxUpdateAll.TabIndex = 7;
            this.btnOlxUpdateAll.Text = "Update all OLX by one click";
            this.btnOlxUpdateAll.UseVisualStyleBackColor = true;
            this.btnOlxUpdateAll.Click += new System.EventHandler(this.btnOlxUpdateAll_Click);
            // 
            // btnDomRiaList_LoadFromWeb
            // 
            this.btnDomRiaList_LoadFromWeb.Location = new System.Drawing.Point(599, 25);
            this.btnDomRiaList_LoadFromWeb.Name = "btnDomRiaList_LoadFromWeb";
            this.btnDomRiaList_LoadFromWeb.Size = new System.Drawing.Size(193, 31);
            this.btnDomRiaList_LoadFromWeb.TabIndex = 8;
            this.btnDomRiaList_LoadFromWeb.Text = "1. Load DomRia list from Web";
            this.btnDomRiaList_LoadFromWeb.UseVisualStyleBackColor = true;
            this.btnDomRiaList_LoadFromWeb.Click += new System.EventHandler(this.btnDomRiaList_LoadFromWeb_Click);
            // 
            // btnDomRiaParseDetails
            // 
            this.btnDomRiaParseDetails.Location = new System.Drawing.Point(599, 70);
            this.btnDomRiaParseDetails.Name = "btnDomRiaParseDetails";
            this.btnDomRiaParseDetails.Size = new System.Drawing.Size(248, 28);
            this.btnDomRiaParseDetails.TabIndex = 9;
            this.btnDomRiaParseDetails.Text = "2. Parse DomRia details files and Save to DB";
            this.btnDomRiaParseDetails.UseVisualStyleBackColor = true;
            this.btnDomRiaParseDetails.Click += new System.EventHandler(this.btnDomRiaParseDetails_Click);
            // 
            // btnDomRiaUpdateAll
            // 
            this.btnDomRiaUpdateAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnDomRiaUpdateAll.Location = new System.Drawing.Point(27, 25);
            this.btnDomRiaUpdateAll.Name = "btnDomRiaUpdateAll";
            this.btnDomRiaUpdateAll.Size = new System.Drawing.Size(278, 57);
            this.btnDomRiaUpdateAll.TabIndex = 10;
            this.btnDomRiaUpdateAll.Text = "Update all DomRia by one click";
            this.btnDomRiaUpdateAll.UseVisualStyleBackColor = true;
            this.btnDomRiaUpdateAll.Click += new System.EventHandler(this.btnDomRiaUpdateAll_Click);
            // 
            // btnRealEstateList_LoadFromWeb
            // 
            this.btnRealEstateList_LoadFromWeb.Location = new System.Drawing.Point(868, 25);
            this.btnRealEstateList_LoadFromWeb.Name = "btnRealEstateList_LoadFromWeb";
            this.btnRealEstateList_LoadFromWeb.Size = new System.Drawing.Size(256, 31);
            this.btnRealEstateList_LoadFromWeb.TabIndex = 11;
            this.btnRealEstateList_LoadFromWeb.Text = "1. Load RealEstate list from Web";
            this.btnRealEstateList_LoadFromWeb.UseVisualStyleBackColor = true;
            this.btnRealEstateList_LoadFromWeb.Click += new System.EventHandler(this.btnRealEstateList_LoadFromWeb_Click);
            // 
            // btnRealEstateList_Parse
            // 
            this.btnRealEstateList_Parse.Location = new System.Drawing.Point(868, 70);
            this.btnRealEstateList_Parse.Name = "btnRealEstateList_Parse";
            this.btnRealEstateList_Parse.Size = new System.Drawing.Size(256, 28);
            this.btnRealEstateList_Parse.TabIndex = 12;
            this.btnRealEstateList_Parse.Text = "2. Parse RealEstate list files and Save to DB";
            this.btnRealEstateList_Parse.UseVisualStyleBackColor = true;
            this.btnRealEstateList_Parse.Click += new System.EventHandler(this.btnRealEstateList_Parse_Click);
            // 
            // btnRealEstateDetails_LoadFromWeb
            // 
            this.btnRealEstateDetails_LoadFromWeb.Location = new System.Drawing.Point(868, 116);
            this.btnRealEstateDetails_LoadFromWeb.Name = "btnRealEstateDetails_LoadFromWeb";
            this.btnRealEstateDetails_LoadFromWeb.Size = new System.Drawing.Size(256, 31);
            this.btnRealEstateDetails_LoadFromWeb.TabIndex = 13;
            this.btnRealEstateDetails_LoadFromWeb.Text = "3. Load RealEstate details from Web";
            this.btnRealEstateDetails_LoadFromWeb.UseVisualStyleBackColor = true;
            this.btnRealEstateDetails_LoadFromWeb.Click += new System.EventHandler(this.btnRealEstateDetails_LoadFromWeb_Click);
            // 
            // btnRealEstateDetails_Parse
            // 
            this.btnRealEstateDetails_Parse.Location = new System.Drawing.Point(868, 166);
            this.btnRealEstateDetails_Parse.Name = "btnRealEstateDetails_Parse";
            this.btnRealEstateDetails_Parse.Size = new System.Drawing.Size(256, 28);
            this.btnRealEstateDetails_Parse.TabIndex = 14;
            this.btnRealEstateDetails_Parse.Text = "4. Parse RealEstate details files and Save to DB";
            this.btnRealEstateDetails_Parse.UseVisualStyleBackColor = true;
            this.btnRealEstateDetails_Parse.Click += new System.EventHandler(this.btnRealEstateDetails_Parse_Click);
            // 
            // btnRealEstateUpdateAll
            // 
            this.btnRealEstateUpdateAll.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btnRealEstateUpdateAll.Location = new System.Drawing.Point(27, 184);
            this.btnRealEstateUpdateAll.Name = "btnRealEstateUpdateAll";
            this.btnRealEstateUpdateAll.Size = new System.Drawing.Size(278, 57);
            this.btnRealEstateUpdateAll.TabIndex = 15;
            this.btnRealEstateUpdateAll.Text = "Update all RealEstate by one click";
            this.btnRealEstateUpdateAll.UseVisualStyleBackColor = true;
            this.btnRealEstateUpdateAll.Click += new System.EventHandler(this.btnRealEstateUpdateAll_Click);
            // 
            // btnUpdateRealEstateData
            // 
            this.btnUpdateRealEstateData.Location = new System.Drawing.Point(868, 213);
            this.btnUpdateRealEstateData.Name = "btnUpdateRealEstateData";
            this.btnUpdateRealEstateData.Size = new System.Drawing.Size(256, 28);
            this.btnUpdateRealEstateData.TabIndex = 16;
            this.btnUpdateRealEstateData.Text = "5. Update RealEstate tables in DB";
            this.btnUpdateRealEstateData.UseVisualStyleBackColor = true;
            this.btnUpdateRealEstateData.Click += new System.EventHandler(this.btnUpdateRealEstateData_Click);
            // 
            // btnUpdateDomRiaData
            // 
            this.btnUpdateDomRiaData.Location = new System.Drawing.Point(599, 117);
            this.btnUpdateDomRiaData.Name = "btnUpdateDomRiaData";
            this.btnUpdateDomRiaData.Size = new System.Drawing.Size(193, 28);
            this.btnUpdateDomRiaData.TabIndex = 18;
            this.btnUpdateDomRiaData.Text = "3. Update DomRia tables in DB";
            this.btnUpdateDomRiaData.UseVisualStyleBackColor = true;
            this.btnUpdateDomRiaData.Click += new System.EventHandler(this.btnUpdateDomRiaData_Click);
            // 
            // btnVN_House_List_LoadFromWeb
            // 
            this.btnVN_House_List_LoadFromWeb.Location = new System.Drawing.Point(599, 184);
            this.btnVN_House_List_LoadFromWeb.Name = "btnVN_House_List_LoadFromWeb";
            this.btnVN_House_List_LoadFromWeb.Size = new System.Drawing.Size(193, 31);
            this.btnVN_House_List_LoadFromWeb.TabIndex = 19;
            this.btnVN_House_List_LoadFromWeb.Text = "1. Load VN House List from Web";
            this.btnVN_House_List_LoadFromWeb.UseVisualStyleBackColor = true;
            this.btnVN_House_List_LoadFromWeb.Click += new System.EventHandler(this.btnVN_Houses_LoadFromWeb_Click);
            // 
            // btnVNHousesParse
            // 
            this.btnVNHousesParse.Location = new System.Drawing.Point(599, 221);
            this.btnVNHousesParse.Name = "btnVNHousesParse";
            this.btnVNHousesParse.Size = new System.Drawing.Size(193, 31);
            this.btnVNHousesParse.TabIndex = 20;
            this.btnVNHousesParse.Text = "2. Parse VN.Houses and Save to DB";
            this.btnVNHousesParse.UseVisualStyleBackColor = true;
            this.btnVNHousesParse.Click += new System.EventHandler(this.btnVNHousesParse_Click);
            // 
            // btnVN_House_Details_LoadFromWeb
            // 
            this.btnVN_House_Details_LoadFromWeb.Location = new System.Drawing.Point(599, 258);
            this.btnVN_House_Details_LoadFromWeb.Name = "btnVN_House_Details_LoadFromWeb";
            this.btnVN_House_Details_LoadFromWeb.Size = new System.Drawing.Size(193, 31);
            this.btnVN_House_Details_LoadFromWeb.TabIndex = 21;
            this.btnVN_House_Details_LoadFromWeb.Text = "3. Load VN House details from Web";
            this.btnVN_House_Details_LoadFromWeb.UseVisualStyleBackColor = true;
            this.btnVN_House_Details_LoadFromWeb.Click += new System.EventHandler(this.btnVN_House_Details_LoadFromWeb_Click);
            // 
            // btnVn_House_Details_Parse
            // 
            this.btnVn_House_Details_Parse.Location = new System.Drawing.Point(599, 295);
            this.btnVn_House_Details_Parse.Name = "btnVn_House_Details_Parse";
            this.btnVn_House_Details_Parse.Size = new System.Drawing.Size(248, 28);
            this.btnVn_House_Details_Parse.TabIndex = 22;
            this.btnVn_House_Details_Parse.Text = "4. Parse VN House details files and Save to DB";
            this.btnVn_House_Details_Parse.UseVisualStyleBackColor = true;
            this.btnVn_House_Details_Parse.Click += new System.EventHandler(this.btnVn_House_Details_Parse_Click);
            // 
            // btnUpdateVnData
            // 
            this.btnUpdateVnData.Location = new System.Drawing.Point(599, 329);
            this.btnUpdateVnData.Name = "btnUpdateVnData";
            this.btnUpdateVnData.Size = new System.Drawing.Size(193, 28);
            this.btnUpdateVnData.TabIndex = 23;
            this.btnUpdateVnData.Text = "5. Update VN tables in DB";
            this.btnUpdateVnData.UseVisualStyleBackColor = true;
            this.btnUpdateVnData.Click += new System.EventHandler(this.btnUpdateVnData_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1158, 391);
            this.Controls.Add(this.btnUpdateVnData);
            this.Controls.Add(this.btnVn_House_Details_Parse);
            this.Controls.Add(this.btnVN_House_Details_LoadFromWeb);
            this.Controls.Add(this.btnVNHousesParse);
            this.Controls.Add(this.btnVN_House_List_LoadFromWeb);
            this.Controls.Add(this.btnUpdateDomRiaData);
            this.Controls.Add(this.btnUpdateRealEstateData);
            this.Controls.Add(this.btnRealEstateUpdateAll);
            this.Controls.Add(this.btnRealEstateDetails_Parse);
            this.Controls.Add(this.btnRealEstateDetails_LoadFromWeb);
            this.Controls.Add(this.btnRealEstateList_Parse);
            this.Controls.Add(this.btnRealEstateList_LoadFromWeb);
            this.Controls.Add(this.btnDomRiaUpdateAll);
            this.Controls.Add(this.btnDomRiaParseDetails);
            this.Controls.Add(this.btnDomRiaList_LoadFromWeb);
            this.Controls.Add(this.btnOlxUpdateAll);
            this.Controls.Add(this.btnUpdateOlxData);
            this.Controls.Add(this.btnOlxDetails_Parse);
            this.Controls.Add(this.btnOlxDetails_LoadFromWeb);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnOlxList_LoadFromWeb);
            this.Controls.Add(this.btnOlxList_Parse);
            this.Name = "Form1";
            this.Text = "Form1";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnOlxList_Parse;
        private System.Windows.Forms.Button btnOlxList_LoadFromWeb;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblFirst;
        private System.Windows.Forms.Button btnOlxDetails_LoadFromWeb;
        private System.Windows.Forms.Button btnOlxDetails_Parse;
        private System.Windows.Forms.Button btnUpdateOlxData;
        private System.Windows.Forms.ToolStripStatusLabel lblSecond;
        private System.Windows.Forms.Button btnOlxUpdateAll;
        private System.Windows.Forms.Button btnDomRiaList_LoadFromWeb;
        private System.Windows.Forms.Button btnDomRiaParseDetails;
        private System.Windows.Forms.Button btnDomRiaUpdateAll;
        private System.Windows.Forms.Button btnRealEstateList_LoadFromWeb;
        private System.Windows.Forms.Button btnRealEstateList_Parse;
        private System.Windows.Forms.Button btnRealEstateDetails_LoadFromWeb;
        private System.Windows.Forms.Button btnRealEstateDetails_Parse;
        private System.Windows.Forms.Button btnRealEstateUpdateAll;
        private System.Windows.Forms.Button btnUpdateRealEstateData;
        private System.Windows.Forms.Button btnUpdateDomRiaData;
        private System.Windows.Forms.Button btnVN_House_List_LoadFromWeb;
        private System.Windows.Forms.Button btnVNHousesParse;
        private System.Windows.Forms.Button btnVN_House_Details_LoadFromWeb;
        private System.Windows.Forms.Button btnVn_House_Details_Parse;
        private System.Windows.Forms.Button btnUpdateVnData;
    }
}

