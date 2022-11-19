using System;
using System.Net;
using System.Windows.Forms;

namespace WebDownloader {
  public partial class frmWebDownloader : Form {

    private static string exampleFileName = csIni.pathExe + @"\DownloaderExamples.txt";
    private string[] urls;
    static private string[] bodies;
    private string[] filenames;
    private bool postFlag = false;
    private int sessions;

    public frmWebDownloader() {
      InitializeComponent();
    }

    private void btnExamples_Click(object sender, EventArgs e) {
      csUtils.AsynRunCmd("notepad " + exampleFileName);
    }

    private bool PrepareUrls() { // Return value: true == success
      string[] ss = txtParams.Text.Split(new string[] { Environment.NewLine,";","^","|","," }, StringSplitOptions.RemoveEmptyEntries);
      urls = new string[ss.Length]; 
      filenames = new string[ss.Length];
      bodies = new string[ss.Length];
      postFlag = this.btnPost.Checked;
      string s1 = txtUrlTemplate.Text.Trim();
      string s2 = txtFilenameTemplate.Text.Trim();
      string s3 =txtBody.Text.Trim();
      if (String.IsNullOrEmpty(s3) && postFlag) {
        if (MessageBox.Show("Are you sure to make POST with blank body?", "", MessageBoxButtons.OKCancel)!= DialogResult.OK) {
          return false;
        }
      }
      if (!string.IsNullOrEmpty(s3) && !postFlag) {
        if (MessageBox.Show("Are you sure to make GET with not blank body?", "", MessageBoxButtons.OKCancel)!= DialogResult.OK) {
          return false;
        }
      }
      for (int i = 0; i < ss.Length; i++) {
        string s = ss[i].Trim();
        urls[i] = String.Format(s1, s);
        filenames[i] = String.Format(s2, s.Replace("/","_"));
        if (postFlag && !String.IsNullOrEmpty(s3)) {
          bodies[i] = String.Format(s3, s);
        }
      }
      this.sessions = Convert.ToInt32( this.txtSessions.Value);
      if (this.sessions >= 1 && this.sessions <= 500) return true;
      else {
        MessageBox.Show("Error! 'Sessions' field must be a number between 1 and 500.");
        return false;
      }
    }

    private void btnRun_Click(object sender, EventArgs e)
    {
        if (this.PrepareUrls())
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            csJob.Run("Завантаження із Web", this.HttpAsyncExecute, null);
        }
    }

        public void HttpAsyncExecute(csJob job, object[] args) {
      using (csHttpFileUploader x = new csHttpFileUploader(job, this.sessions)) {
        x.silentMode = this.cbSilent.Checked;
        if (this.postFlag) x.OnSetDataForRequest += new csHttpBase.dlgSetDataForRequest(x_OnSetDataForRequest);
        x.Execute( this.urls, this.filenames);
      }
    }

    static void x_OnSetDataForRequest(int thisBatchStep, int totalBatchSteps, csHttpBase.csHttpRequestData requestData, object userData) {
      if (totalBatchSteps <=bodies.Length ) {
        requestData.method = "POST";
        requestData.bodyData = bodies[totalBatchSteps-1];
      }
      else {
        requestData.cancelFlag = true;
      }
    }

    
  }
}