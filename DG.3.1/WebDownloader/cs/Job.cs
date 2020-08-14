using System;
using System.Text;
using System.Windows.Forms;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.IO;

namespace WebDownloader {

  public class csJob : BackgroundWorker {
    ////////////////////  Static function + enums //////////////////////////////////
    public enum MessageType { start, done, error, warning, info, noImage };
    public delegate void dlgJobExecute(csJob job, object[] args);

    public static csJob Run(string title, dlgJobExecute call, object[] args) {
      return RunGeneral(title, call, args, true);
    }
    public static csJob RunNoLog(string title, dlgJobExecute call, object[] args) {
      //			System.Net.WebRequestMethods.Ftp
      return RunGeneral(title, call, args, true);
    }
    static csJob RunGeneral(string title, dlgJobExecute call, object[] args, bool logFile) {
      csJob job = new csJob();
      job.logFile = logFile; job.objCall = call; job.objArgs = args;
      if (title != null)
        job.PrepareForm(title);
      else // No form
        job.RunPoint();
      return job;
    }
    public static int ShowMessage(csJob job, string pMessage, MessageType messType, int pRowNo) {
      if (job == null) return 0;
      else return job.ShowMessage(pMessage, messType, pRowNo);
    }
    public static int ShowMessage(csJob job, string pMessage, MessageType messType) {
      return csJob.ShowMessage(job, pMessage, messType, -1);
    }
    public static void ShowError(csJob job, Exception ex) {
      if (job == null) throw ex;
      else job.ShowError(ex);
    }

    ////////////////////////////  Class start ////////////////////////////////
    public System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
    Thread thread = Thread.CurrentThread;
    //    SynchronizationContext uiContext = SynchronizationContext.Current; 
    private frmJobViewer frm;
    private string logFileName = null;
    private DateTime startTime;
    private bool logFile;
    private dlgJobExecute objCall;
    private DoWorkEventArgs objWorkEventArgs;
    private object[] objArgs;
    public List<MessageType> listLog = new List<MessageType>();

    private bool flagRun;
    private bool flagPause;
    private int agentMessageOffset;
    public bool breakOnError = true;
    public object userData;

    private csJob() {
      base.WorkerReportsProgress = false;
      base.WorkerSupportsCancellation = true;
    }

    /////////////////////////   Entry Point ////////////////////////

    public void RunPoint() {
      startTime = DateTime.Now;
      if (this.logFile) this.SetLogFileName();
      flagRun = true; this.flagPause = false;
      this.listLog.Clear();
      if (this.frm != null) this.frm.Init();
      this.OnChangeStatus();

      this.DoWork -= new DoWorkEventHandler(this.RunPointContinue);
      this.DoWork += new DoWorkEventHandler(this.RunPointContinue);
      this.RunWorkerAsync(this.objArgs);
    }
    public void RunPointContinue(object sender, DoWorkEventArgs e) {
      this.objWorkEventArgs = e; // Save event argument to analyse when job finished
      this.objCall(this, this.objArgs);
    }

    /////////////////////////  Show functions ////////////////////////
    private void ShowError(Exception ex) {
      // Changed position at 2009-07-15 CancelAsync and ShowMessage
      if (this.breakOnError && this.IsBusy) {
        this.CancelAsync();
        this.btnResume_Click(this, new EventArgs());
        //				this.CancelJob();
      }
      string message = ex.Message + (ex.Source == null ? "" : Environment.NewLine + ex.Source) +
            (ex.StackTrace == null ? "" : Environment.NewLine + ex.StackTrace);
      this.ShowMessage(message, MessageType.error, -1);
    }
    private int ShowMessage(string pMessage, MessageType messType) {
      return ShowMessage(pMessage, messType, -1);
    }
    private int ShowMessage(string pMessage, MessageType messType, int pRowNo) {
      SaveToLogFile(pMessage, messType, pRowNo);
      lock (this.listLog) {
        if (pRowNo < 0 || pRowNo >= this.listLog.Count) {
          this.listLog.Add(messType);
          pRowNo = this.listLog.Count - 1;
        }
        else this.listLog[pRowNo] = messType;
      }
      if (this.frm != null) {
        //Example INVOKE using SynchronizationContext //(csIni.GetUIContext()).Send(xxx, new object[] { pMessage, messType, pRowNo });
        //Example using object delegate	//frm.Invoke((frmJobViewer.dlgShowMessage)frm.ShowMessage, new object[] { pMessage, messType, pRowNo+1 }); 
        frm.ShowMessage(pMessage, messType, pRowNo);
      }
      if (!this.CheckCancelAndPause() && this.IsBusy && messType!= MessageType.error) 
      	throw new Exception("Job canceled");
      return pRowNo;
    }
    void MessageTotal() {
      double secs = ((TimeSpan)(DateTime.Now - this.startTime)).TotalSeconds;
      int errors = 0; int starts = 0; int dones = 0; int infos = 0; int warnings = 0;
      for (int i = 0; i < this.listLog.Count; i++) {
        switch (this.listLog[i]) {
          case MessageType.start: starts++; break;
          case MessageType.done: dones++; break;
          case MessageType.error: errors++; break;
          case MessageType.warning: warnings++; break;
          case MessageType.info: infos++; break;
        }
      }
      StringBuilder sb = new StringBuilder();
      if (errors != 0) sb.Append(errors.ToString() + " - ERRORS; ");
      if (starts != 0) sb.Append(starts.ToString() + " - Start; ");
      if (dones != 0) sb.Append(dones.ToString() + " - Done; ");
      if (warnings != 0) sb.Append(warnings.ToString() + " - Warnings; ");
      if (infos != 0) sb.Append(infos.ToString() + " - Info; ");
      string s;
      if (sb.ToString() != "")
        s = "Total items: " + this.listLog.Count.ToString() + " (" + sb.Remove(sb.Length - 2, 2) + ")";
      else
        s = "Total items: " + this.listLog.Count.ToString();
      this.ShowMessage(s + ". Duration: " + Convert.ToInt32(secs) + " seconds.", MessageType.noImage);
    }

    ///////////////////////////////////////  Log File //////////////////////////////
    private void SaveToLogFile(string pMessage, MessageType messType, int pRowNo) {
      if (this.logFileName == null) SetLogFileName();
      lock (this.logFileName) {
        using (StreamWriter sw = File.AppendText(this.logFileName)) {
          sw.WriteLine(messType.ToString() + "#" + DateTime.Now.ToString("HH:mm:ss.fff") +
            "#" + pRowNo.ToString() + "#" + pMessage);
          sw.Flush(); sw.Close();
        }
      }
    }
    void SetLogFileName() {
      string template = "#." + this.objCall.Method.ReflectedType.Name + "." + this.objCall.Method.Name;
      string s = template.Replace("#", DateTime.Today.ToString("yyyy-MM-dd")).Replace(".", "-") + ".log";
      this.logFileName = csUtilsFile.GetNearestNewFileName(csIni.pathLog, s);
      using (StreamWriter sw = new StreamWriter(this.logFileName)) {
        sw.WriteLine("MessageType#Time#MessageNo#Message");	// Write header of log file
        sw.Flush(); sw.Close();
      }
    }


    ///////////////////////////////////////  Form //////////////////////////////
    private void PrepareForm(string title) {
      this.breakOnError = false;
      this.frm = new frmJobViewer();

      //      this.frm.job = this;

      this.frm.Text = title;
      foreach (Form x in Application.OpenForms) {
        if (x.IsMdiContainer && x.Visible) {
          this.frm.MdiParent = x;
          break;
        }
      }
      this.frm.Show();
      csUtils.MDIAttachForm(this.frm);
      this.frm.cbBreakOnError.Text = this.frm.cbBreakOnError.Items[(this.breakOnError ? 0 : 1)].ToString();
      this.frm.btnRun.Click -= new EventHandler(btnRun_Click);
      this.frm.btnRun.Click += new EventHandler(btnRun_Click);
      this.frm.btnPause.Click -= new EventHandler(btnPause_Click);
      this.frm.btnPause.Click += new EventHandler(btnPause_Click);
      this.frm.btnResume.Click += new EventHandler(btnResume_Click);
      this.frm.btnResume.Click += new EventHandler(btnResume_Click);
      this.frm.btnCancel.Click -= new EventHandler(btnCancel_Click);
      this.frm.btnCancel.Click += new EventHandler(btnCancel_Click);
      this.frm.btnOpenTraceFile.Click -= new EventHandler(btnOpenTraceFile_Click);
      this.frm.btnOpenTraceFile.Click += new EventHandler(btnOpenTraceFile_Click);
      this.frm.FormClosing -= new System.Windows.Forms.FormClosingEventHandler(frm_FormClosing);
      this.frm.FormClosing += new System.Windows.Forms.FormClosingEventHandler(frm_FormClosing);
      this.frm.cbBreakOnError.TextChanged -= new EventHandler(cbBreakOnError_TextChanged);
      this.frm.cbBreakOnError.TextChanged += new EventHandler(cbBreakOnError_TextChanged);
      this.frm.Disposed -= new EventHandler(frm_Disposed);
      this.frm.Disposed += new EventHandler(frm_Disposed);
      this.OnChangeStatus();
    }

    void frm_Disposed(object sender, EventArgs e) {
      this.Dispose();
    }


    ///////////////////////////////// Event Handlers ///////////////////////
    void cbBreakOnError_TextChanged(object sender, EventArgs e) {
      this.breakOnError = (((ToolStripComboBox)sender).SelectedIndex == 0);
    }
    void frm_FormClosing(object sender, System.Windows.Forms.FormClosingEventArgs e) {
      if (this.flagRun || this.flagPause) e.Cancel = true;
    }
    void btnOpenTraceFile_Click(object sender, EventArgs e) {
      if (this.logFileName != null) csUtils.AsynRunCmd("notepad " + this.logFileName);
    }
    void btnCancel_Click(object sender, EventArgs e) {
      this.CancelJob();
    }
    public void CancelJob() {
      if (this.IsBusy && !this.CancellationPending) {
        this.CancelAsync();
        this.objWorkEventArgs.Cancel = true;
        if (flagPause) {
          this.flagPause = false;
          Monitor.Enter(this);
          Monitor.PulseAll(this);
          Monitor.Exit(this);
        }
        this.OnChangeStatus();
      }
    }
    void btnPause_Click(object sender, EventArgs e) {
      this.flagPause = true;
      this.OnChangeStatus();
    }
    void btnResume_Click(object sender, EventArgs e) {// windows Thread
      if (flagPause) {
        this.flagPause = false;
        Monitor.Enter(this);
        Monitor.PulseAll(this);
        Monitor.Exit(this);
      }
      this.OnChangeStatus();
    }
    void btnRun_Click(object sender, EventArgs e) {
      if (this.timer.Enabled) {
        this.timer.Stop();
      }
      if (this.frm.dtpRunAt.Value > DateTime.Now) {
        this.timer.Tick -= new EventHandler(timer_Tick);
        this.timer.Tick += new EventHandler(timer_Tick);
        this.timer.Interval = Convert.ToInt32(((TimeSpan)(this.frm.dtpRunAt.Value - DateTime.Now)).TotalMilliseconds);
        this.timer.Start();
        this.ShowMessage("Procedure will be run at " + this.frm.dtpRunAt.Value.ToString("yyyy-MM-dd HH:mm:ss") +
          " in " + ((TimeSpan)(this.frm.dtpRunAt.Value - DateTime.Now)).TotalSeconds.ToString("N0") + " seconds",
          MessageType.info);
        this.OnChangeStatus();
      }
      else {
        this.timer_Tick(null, null);
      }
    }
    void timer_Tick(object sender, EventArgs e) {
      this.timer.Tick -= new EventHandler(timer_Tick);
      this.timer.Enabled = false;
      this.RunPoint();
    }
    public bool CheckCancelAndPause() {
      if (this.CancellationPending || !this.IsBusy) {
        return false; // Cancel
      }
      if (this.flagPause) {
        lock (this) {
          Monitor.Wait(this);
        }
      }
      return true;
    }
    protected override void OnRunWorkerCompleted(RunWorkerCompletedEventArgs e) {
      base.OnRunWorkerCompleted(e);
      if (e.Cancelled || this.objWorkEventArgs.Cancel) {
        this.ShowMessage("Job canceled!", MessageType.error);
      }
      else if (e.Error != null) {
        this.ShowMessage("Worker exception. " + e.Error.Message, MessageType.error);
        Monitor.Enter(this);
        Monitor.PulseAll(this);
        Monitor.Exit(this);
      }
      this.Clear();
    }

    private void Clear() {
      if (flagRun) MessageTotal();
      this.flagRun = false;
      this.flagPause = false;
      this.OnChangeStatus();
    }

    ///////////////////////////////////////  Buttons & status //////////////////////////////
    private void OnChangeStatus() {
      if (this.frm != null) {
        string status = "";
        this.frm.SenEnabledToolStripItem(this.frm.btnRun, !this.flagRun);
        this.frm.SenEnabledToolStripItem(this.frm.btnPause, this.flagRun && !this.flagPause && !this.CancellationPending);
        this.frm.SenEnabledToolStripItem(this.frm.btnResume, this.flagPause);
        this.frm.SenEnabledToolStripItem(this.frm.btnCancel, this.flagRun && !this.CancellationPending);
        this.frm.SenEnabledToolStripItem(this.frm.btnOpenTraceFile, this.logFileName != null);
        if (!this.flagRun && !this.timer.Enabled) status = "Idle";
        if (!this.flagRun && this.timer.Enabled) status = "Waiting for run";
        if (this.flagRun) status = "Runing";
        if (this.flagRun && this.flagPause) status = "Paused";
        if (this.flagRun && this.CancellationPending) status = "Canceling";
        if (status == "") status = "Unknown status";
        this.frm.SetTextOfToolStripItem(this.frm.lblStatusShort, status);
      }
    }

  }
}
