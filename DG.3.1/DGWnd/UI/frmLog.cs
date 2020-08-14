using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace DGWnd.UI {
  public partial class frmLog : Form {

    private static List<string> Log = new List<string>();

    public static void AddEntry(string message) => Log.Add(DateTime.Now.ToString("HH:mm:ss.ffff") + " " + message);

    public frmLog() {
      InitializeComponent();
    }

    private void frmLog_Load(object sender, EventArgs e) => txtLog.Text = string.Join(Environment.NewLine, Log);

    private void BtnClear_Click(object sender, EventArgs e)
    {
      Log.Clear();
      txtLog.Text = "";
    }

    private void BtnClose_Click(object sender, EventArgs e) => Close();
  }
}