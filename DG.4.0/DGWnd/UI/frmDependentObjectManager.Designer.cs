namespace DGWnd.UI {
  partial class frmDependentObjectManager {
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
      this.txtLog.Size = new System.Drawing.Size(402, 505);
      this.txtLog.TabIndex = 0;
      // 
      // frmDependentObjectManager
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(402, 505);
      this.Controls.Add(this.txtLog);
      this.Name = "frmDependentObjectManager";
      this.Text = "Залежні обєкти";
      this.Load += new System.EventHandler(this.frmDependentObjectManager_Load);
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmDependentObjectManager_FormClosed);
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private System.Windows.Forms.TextBox txtLog;
  }
}