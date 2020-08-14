namespace DGWnd.UI {
  partial class frmDataDefinition {
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
      this.ucDataDefinition1 = new UserControls.UCDataDefinition();
      this.SuspendLayout();
      // 
      // ucDataDefinition1
      // 
      this.ucDataDefinition1.Dock = System.Windows.Forms.DockStyle.Fill;
      this.ucDataDefinition1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
      this.ucDataDefinition1.Location = new System.Drawing.Point(0, 0);
      this.ucDataDefinition1.Name = "ucDataDefinition1";
      this.ucDataDefinition1.Size = new System.Drawing.Size(862, 548);
      this.ucDataDefinition1.TabIndex = 0;
      // 
      // frmDataDefinition
      // 
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.ClientSize = new System.Drawing.Size(862, 548);
      this.Controls.Add(this.ucDataDefinition1);
      this.Name = "frmDataDefinition";
      this.Text = "Список завдань";
      this.Load += new System.EventHandler(this.frmDataDefinition_Load);
      this.ResumeLayout(false);

    }

    #endregion

    private UserControls.UCDataDefinition ucDataDefinition1;


  }
}