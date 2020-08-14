namespace DGWnd.UI {
  partial class frmLog
  {
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
      this.txtLog = new System.Windows.Forms.TextBox();
      this.panel1 = new System.Windows.Forms.Panel();
      this.btnClose = new System.Windows.Forms.Button();
      this.btnClear = new System.Windows.Forms.Button();
      this.panel1.SuspendLayout();
      this.SuspendLayout();
      // 
      // txtLog
      // 
      this.txtLog.BackColor = System.Drawing.Color.White;
      this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
      this.txtLog.Location = new System.Drawing.Point(0, 0);
      this.txtLog.Multiline = true;
      this.txtLog.Name = "txtLog";
      this.txtLog.ReadOnly = true;
      this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
      this.txtLog.Size = new System.Drawing.Size(850, 348);
      this.txtLog.TabIndex = 0;
      this.txtLog.WordWrap = false;
      // 
      // panel1
      // 
      this.panel1.Controls.Add(this.btnClose);
      this.panel1.Controls.Add(this.btnClear);
      this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
      this.panel1.Location = new System.Drawing.Point(0, 348);
      this.panel1.Name = "panel1";
      this.panel1.Size = new System.Drawing.Size(850, 74);
      this.panel1.TabIndex = 1;
      // 
      // btnClose
      // 
      this.btnClose.Location = new System.Drawing.Point(608, 25);
      this.btnClose.Name = "btnClose";
      this.btnClose.Size = new System.Drawing.Size(75, 23);
      this.btnClose.TabIndex = 1;
      this.btnClose.Text = "Закрити";
      this.btnClose.UseVisualStyleBackColor = true;
      this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);
      // 
      // btnClear
      // 
      this.btnClear.Location = new System.Drawing.Point(159, 21);
      this.btnClear.Name = "btnClear";
      this.btnClear.Size = new System.Drawing.Size(137, 28);
      this.btnClear.TabIndex = 0;
      this.btnClear.Text = "Очистити журнал";
      this.btnClear.UseVisualStyleBackColor = true;
      this.btnClear.Click += new System.EventHandler(this.BtnClear_Click);
      // 
      // frmLog
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(850, 422);
      this.Controls.Add(this.txtLog);
      this.Controls.Add(this.panel1);
      this.Name = "frmLog";
      this.Text = "Журнал";
      this.Load += new System.EventHandler(this.frmLog_Load);
      this.panel1.ResumeLayout(false);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtLog;
    private System.Windows.Forms.Panel panel1;
    private System.Windows.Forms.Button btnClear;
    private System.Windows.Forms.Button btnClose;
  }
}