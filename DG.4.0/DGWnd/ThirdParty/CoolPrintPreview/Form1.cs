using System;
using System.Drawing;
using System.Windows.Forms;

namespace DGWnd.ThirdParty.CoolPrintPreview {
  public partial class Form1 : Form {
    DateTime _start;
    int _page = 0;
    Font _font = new Font("Segoe UI", 14);

    public Form1() {
      InitializeComponent();
    }

    void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e) {
      _start = DateTime.Now;
      _page = 0;
    }
    void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e) {
      // fill page with text
      Rectangle rc = e.MarginBounds;
      rc.Height = _font.Height + 10;
      for (int i = 0; ; i++) {
        string text = string.Format("line {0} on page {1}", i + 1, _page + 1);
        e.Graphics.DrawString(text, _font, Brushes.Black, rc);
        rc.Y += rc.Height;
        if (rc.Bottom > e.MarginBounds.Bottom) {
          break;
        }
      }

      // move on to next page
      _page++;
      e.HasMorePages = _chkLongDoc.Checked
          ? _page < 3000
          : _page < 30;
    }
    void printDocument1_EndPrint(object sender, System.Drawing.Printing.PrintEventArgs e) {
      Console.WriteLine("Document rendered in {0} ms",
          DateTime.Now.Subtract(_start).TotalMilliseconds);
    }

    // show standard print preview
    void button1_Click(object sender, EventArgs e) {
      using (PrintPreviewDialog dlg = new PrintPreviewDialog()) {
        dlg.Document = this.printDocument1;
        dlg.ShowDialog(this);
      }
    }

    // show cool print preview
    void button2_Click(object sender, EventArgs e) {
      using (CoolPrintPreviewDialog dlg = new CoolPrintPreviewDialog()) {
        dlg.Document = this.printDocument1;
        dlg.ShowDialog(this);
      }
    }
  }
}
