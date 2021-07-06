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
            this.SuspendLayout();
            // 
            // btnParseOlx
            // 
            this.btnParseOlx.Location = new System.Drawing.Point(33, 25);
            this.btnParseOlx.Name = "btnParseOlx";
            this.btnParseOlx.Size = new System.Drawing.Size(111, 28);
            this.btnParseOlx.TabIndex = 0;
            this.btnParseOlx.Text = "Parse OLX files";
            this.btnParseOlx.UseVisualStyleBackColor = true;
            this.btnParseOlx.Click += new System.EventHandler(this.btnParseOlx_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(634, 297);
            this.Controls.Add(this.btnParseOlx);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnParseOlx;
    }
}

