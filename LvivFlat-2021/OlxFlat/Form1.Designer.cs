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
            this.btnParseOlx = new System.Windows.Forms.Button();
            this.btnParseOlxDetails = new System.Windows.Forms.Button();
            this.btnLoadFromWeb = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnParseOlx
            // 
            this.btnParseOlx.Location = new System.Drawing.Point(33, 25);
            this.btnParseOlx.Name = "btnParseOlx";
            this.btnParseOlx.Size = new System.Drawing.Size(189, 28);
            this.btnParseOlx.TabIndex = 0;
            this.btnParseOlx.Text = "Parse OLX files and Save to DB";
            this.btnParseOlx.UseVisualStyleBackColor = true;
            this.btnParseOlx.Click += new System.EventHandler(this.btnParseOlx_Click);
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
            // btnLoadFromWeb
            // 
            this.btnLoadFromWeb.Location = new System.Drawing.Point(403, 30);
            this.btnLoadFromWeb.Name = "btnLoadFromWeb";
            this.btnLoadFromWeb.Size = new System.Drawing.Size(170, 31);
            this.btnLoadFromWeb.TabIndex = 2;
            this.btnLoadFromWeb.Text = "Load from Web";
            this.btnLoadFromWeb.UseVisualStyleBackColor = true;
            this.btnLoadFromWeb.Click += new System.EventHandler(this.btnLoadFromWeb_Click);
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 297);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btnLoadFromWeb);
            this.Controls.Add(this.btnParseOlxDetails);
            this.Controls.Add(this.btnParseOlx);
            this.Name = "Form1";
            this.Text = "Form1";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnParseOlx;
        private System.Windows.Forms.Button btnParseOlxDetails;
        private System.Windows.Forms.Button btnLoadFromWeb;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}

