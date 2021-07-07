namespace OlxFlat
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
            this.btnParseOlxDetails = new System.Windows.Forms.Button();
            this.btnOlxList_LoadFromWeb = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnOlxDetails_LoadFromWeb = new System.Windows.Forms.Button();
            this.btnOlxDetails_Parse = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOlxList_Parse
            // 
            this.btnOlxList_Parse.Location = new System.Drawing.Point(283, 70);
            this.btnOlxList_Parse.Name = "btnOlxList_Parse";
            this.btnOlxList_Parse.Size = new System.Drawing.Size(221, 28);
            this.btnOlxList_Parse.TabIndex = 0;
            this.btnOlxList_Parse.Text = "2. Parse OLX list files and Save to DB";
            this.btnOlxList_Parse.UseVisualStyleBackColor = true;
            this.btnOlxList_Parse.Click += new System.EventHandler(this.btnOlxList_Parse_Click);
            // 
            // btnParseOlxDetails
            // 
            this.btnParseOlxDetails.Location = new System.Drawing.Point(33, 70);
            this.btnParseOlxDetails.Name = "btnParseOlxDetails";
            this.btnParseOlxDetails.Size = new System.Drawing.Size(130, 28);
            this.btnParseOlxDetails.TabIndex = 1;
            this.btnParseOlxDetails.Text = "Parse OLX details files";
            this.btnParseOlxDetails.UseVisualStyleBackColor = true;
            this.btnParseOlxDetails.Click += new System.EventHandler(this.btnParseOlxDetails_Click);
            // 
            // btnOlxList_LoadFromWeb
            // 
            this.btnOlxList_LoadFromWeb.Location = new System.Drawing.Point(283, 25);
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
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 275);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(634, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(118, 17);
            this.lblStatus.Text = "toolStripStatusLabel1";
            // 
            // btnOlxDetails_LoadFromWeb
            // 
            this.btnOlxDetails_LoadFromWeb.Location = new System.Drawing.Point(283, 116);
            this.btnOlxDetails_LoadFromWeb.Name = "btnOlxDetails_LoadFromWeb";
            this.btnOlxDetails_LoadFromWeb.Size = new System.Drawing.Size(170, 31);
            this.btnOlxDetails_LoadFromWeb.TabIndex = 4;
            this.btnOlxDetails_LoadFromWeb.Text = "3. Load OLX details from Web";
            this.btnOlxDetails_LoadFromWeb.UseVisualStyleBackColor = true;
            this.btnOlxDetails_LoadFromWeb.Click += new System.EventHandler(this.btnOlxDetails_LoadFromWeb_Click);
            // 
            // btnOlxDetails_Parse
            // 
            this.btnOlxDetails_Parse.Location = new System.Drawing.Point(283, 166);
            this.btnOlxDetails_Parse.Name = "btnOlxDetails_Parse";
            this.btnOlxDetails_Parse.Size = new System.Drawing.Size(221, 28);
            this.btnOlxDetails_Parse.TabIndex = 5;
            this.btnOlxDetails_Parse.Text = "4. Parse OLX details files and Save to DB";
            this.btnOlxDetails_Parse.UseVisualStyleBackColor = true;
            this.btnOlxDetails_Parse.Click += new System.EventHandler(this.btnOlxDetails_Parse_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 297);
            this.Controls.Add(this.btnOlxDetails_Parse);
            this.Controls.Add(this.btnOlxDetails_LoadFromWeb);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnOlxList_LoadFromWeb);
            this.Controls.Add(this.btnParseOlxDetails);
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
        private System.Windows.Forms.Button btnParseOlxDetails;
        private System.Windows.Forms.Button btnOlxList_LoadFromWeb;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.Button btnOlxDetails_LoadFromWeb;
        private System.Windows.Forms.Button btnOlxDetails_Parse;
    }
}

