using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace spMain {
  public partial class frmJobViewer : Form {

    public delegate void dlgToolStripItem(ToolStripItem tsi, object args);
    public delegate void dlgShowMessage(string pMessage, csJob.MessageType messType, int pRowNo);
    public delegate void dlgProgressBar(long totalItems, long count);

    private DataTable dt = new DataTable();
    private bool reverse;
    private int outputMode = 1;
    private object lockObject = new object();

    public frmJobViewer() {
      InitializeComponent();

      // Create data table structure
      dt.Columns.Add("Time", Type.GetType("System.DateTime"));
      dt.Columns.Add("Message", Type.GetType("System.String"));
      // Link data table to grid
      this.grid.AutoGenerateColumns = false;
      this.grid.DataSource = (IListSource)dt;
      this.SetOutputMode(this.outputMode);
      this.lblStatus.Text = ""; 
    }

    public void Init() {
      this.dt.Rows.Clear();

      this.lblStatus.Image = null;
      this.lblStatus.Text = "";
      this.lblProgressBar.Text = "";
      this.lblProgressBar.Visible = false;
      this.pbStatus.Visible = false;
      this.pbStatus.Value = 0;
      if (this.grid.BackgroundColor != System.Drawing.SystemColors.Window) {
        this.grid.BackgroundColor = System.Drawing.SystemColors.Window;
        this.grid.DefaultCellStyle.BackColor = System.Drawing.SystemColors.Window;
      }
    }

    public void ShowMessage(string pMessage, csJob.MessageType messType, int pRowNo) { // window thread
      if (this.InvokeRequired) {
        this.Invoke((dlgShowMessage)this.ShowMessage, pMessage, messType, pRowNo);
        return;
      }
      int k = 0;
      lock (this.lockObject) {
        while (pRowNo > (dt.Rows.Count - 1)) {
          DataRow dr1 = dt.NewRow();
          if (this.reverse)
            dt.Rows.InsertAt(dr1, 0);
          else
            dt.Rows.Add(dr1);
        }
        k = (reverse ? dt.Rows.Count - pRowNo - 1 : pRowNo);
      }
      DataRow dr = dt.Rows[k];
      dr["Time"] = DateTime.Now;
      dr["Message"] = pMessage;

      DataGridViewImageCell imageCell = (DataGridViewImageCell)this.grid.Rows[k].Cells[0];
      switch (messType) {
        case csJob.MessageType.start: imageCell.Value = Properties.Resources.imgStart; break;
				case csJob.MessageType.done: imageCell.Value = Properties.Resources.imgDone; break;
				case csJob.MessageType.error: imageCell.Value = Properties.Resources.imgError; break;
				case csJob.MessageType.warning: imageCell.Value = Properties.Resources.imgWarning; break;
				case csJob.MessageType.info: imageCell.Value = Properties.Resources.imgInfo; break;
				case csJob.MessageType.noImage: imageCell.Value = Properties.Resources.imgBlank; break;
        default: imageCell.Value = Properties.Resources.imgError; break;
      }
      this.lblStatus.Image = (Image)imageCell.Value;
      string[] ss = pMessage.Split("\r\n".ToCharArray());
      if (ss.Length == 1) ss = pMessage.Split("\n".ToCharArray());
      this.lblStatus.Text = ss[0];
      if (this.outputMode == 1) { // Scroll
        this.grid.CurrentCell = imageCell;
        this.Refresh();
      }
    }

    public void SetTextOfToolStripItem(ToolStripItem tsi, object text) {
      if (this.InvokeRequired) {
        this.Invoke((dlgToolStripItem)this.SetTextOfToolStripItem, tsi, text);
        return;
      }
      tsi.Text = (string)text;
    }
    public void SenEnabledToolStripItem(ToolStripItem tsi, object enableFlag) {
      if (this.InvokeRequired) {
        this.Invoke((dlgToolStripItem)this.SenEnabledToolStripItem, tsi, enableFlag);
        return;
      }
      tsi.Enabled = (bool)enableFlag;
    }
    public void ProgressBar(long totalItems, long count) {
      if (this.InvokeRequired) {
        this.Invoke((dlgProgressBar)this.ProgressBar, totalItems, count);
        return;
      }
      if (!this.lblProgressBar.Visible) this.lblProgressBar.Visible = true;
      if (!this.pbStatus.Visible) this.pbStatus.Visible = true;
      this.pbStatus.Value = Convert.ToInt32((count * 100) / totalItems);
      this.lblProgressBar.Text = count + " items of " + totalItems + " (" + this.pbStatus.Value + "%)";
    }

    private void btnMode_Click(object sender, EventArgs e) {
      ToolStripMenuItem x = sender as ToolStripMenuItem;
      SetOutputMode(Convert.ToInt32(x.Tag));
    }

    private void SetOutputMode(int pMode) {
      bool flag = (pMode < 3 && this.outputMode >= 3) || (pMode >= 3 && this.outputMode < 3);
      if (flag) {
        this.reverse = pMode >= 3;
        lock (this.lockObject) {
          for (int i = 0; i < dt.Rows.Count / 2; i++) {
            object x1 = this.dt.Rows[i]["Time"];
            object x2 = this.dt.Rows[i]["Message"];
            this.dt.Rows[i]["Time"] = this.dt.Rows[this.dt.Rows.Count - 1 - i]["Time"];
            this.dt.Rows[i]["Message"] = this.dt.Rows[this.dt.Rows.Count - 1 - i]["Message"];
            this.dt.Rows[dt.Rows.Count - 1 - i]["Time"] = x1;
            this.dt.Rows[dt.Rows.Count - 1 - i]["Message"] = x2;
            object imageCell = this.grid.Rows[i].Cells[0].Value;
            this.grid.Rows[i].Cells[0].Value = this.grid.Rows[dt.Rows.Count - 1 - i].Cells[0].Value;
            this.grid.Rows[dt.Rows.Count - 1 - i].Cells[0].Value = imageCell as System.Drawing.Bitmap;
          }// for
        } // lock
      } // if
      this.outputMode = pMode;
      foreach (ToolStripMenuItem x in this.cbOutputMode.DropDownItems) {
        x.Checked = Convert.ToInt32(x.Tag) == pMode;
        if (x.Checked)
          this.cbOutputMode.Text = "Output mode: " + x.Text;
      }
    }

/*    public csJob job;

    private void btnCancel_Click(object sender, EventArgs e) {
      if (job.thread2 != null) {
        job.CancelJob();
        job.CancellationPending = true;
        job.thread2.Abort();
      }
    }*/

/*  ??? check for backgroud jobs ???
  private void frmJobViewer_FormClosed(object sender, FormClosedEventArgs e) {
//      timer.Stop();
    }*/

  }
}