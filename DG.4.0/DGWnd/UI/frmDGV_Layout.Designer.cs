namespace DGWnd.UI {
  partial class frmDGV_Layout {
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
      this.dgvLayout = new UserControls.UC_DGVLayout();
      this.SuspendLayout();
      // 
      // dgvLayout
      // 
      this.dgvLayout.Dock = System.Windows.Forms.DockStyle.Fill;
      this.dgvLayout.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.dgvLayout.Location = new System.Drawing.Point(0, 0);
      this.dgvLayout.Name = "dgvLayout";
      this.dgvLayout.Size = new System.Drawing.Size(966, 599);
      this.dgvLayout.TabIndex = 0;
      // 
      // frmDGV_Layout
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(966, 599);
      this.Controls.Add(this.dgvLayout);
      this.Name = "frmDGV_Layout";
      this.Text = "Налаштування DGVCube";
      this.ResumeLayout(false);

    }

    #endregion

    public UserControls.UC_DGVLayout dgvLayout;



  }
}