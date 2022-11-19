using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace WebDownloader {
  /// <summary>
  ///  ///////////////////////////////// Class csHttpBatchUsingAsync //////////////////////
  /// </summary>
  public class csHttpFileUploader : IDisposable {

    System.Threading.AutoResetEvent wait = new System.Threading.AutoResetEvent(false);
    private readonly csJob job;
    private readonly int sessions;
    public event csHttpBase.dlgSetDataForRequest OnSetDataForRequest;
    public event EventHandler OnAfterGetResponse;
    public event EventHandler OnFinished;
    public event EventHandler OnPayload;
    public int totalBatchSteps;
    public bool silentMode = false;
    public csHttpAsync[] https;
    public int httpMaxAtempts = csHttpBase.httpMaxAttempts;
    public bool setCookie = true;
    private int[] httpSteps;
    List<string> urls;
    List<string> filenames;
		public object userData;

    public csHttpFileUploader(csJob pJob, int pSessions) {
      this.job = pJob; this.sessions = pSessions;
    }

    //    public void Execute(string[] pUrls, string[] pFilenames) {
    //  }
    public void Execute(string[] pUrls, string[] pFilenames) {
      this.Execute(new List<string>(pUrls), new List<string>(pFilenames));
    }
    public void Execute(List<string> pUrls, List<string> pFilenames) {
      this._log.Clear();

      this.urls = pUrls; this.filenames = pFilenames;
      if (pFilenames != null) {
        for (int i = 0; i < pFilenames.Count; i++) {
          string fn = pFilenames[i];
          if (!String.IsNullOrEmpty(fn)) {
            string path = Path.GetDirectoryName(fn);
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
          }
        }
      }
      int sessionCount = Math.Min(this.sessions, this.urls.Count);
      https = new csHttpAsync[sessionCount];
      httpSteps = new int[sessionCount];
      // Create new httpAsync objects
      for (int i = 0; i < sessionCount; i++) {
        https[i] = new csHttpAsync(this.job, i);
        https[i].maxAttempts = httpMaxAtempts;
        https[i].silentMode = this.silentMode;
        if (this.OnAfterGetResponse != null) https[i].OnAfterGetResponse += this.OnAfterGetResponse;
        if (this.OnFinished != null) https[i].OnFinished += this.OnFinished;
        if (this.OnPayload != null) https[i].OnPayload += this.OnPayload;
        https[i].OnFinished += this.csHttpAsync_OnFinished;
      }
      // Init HttpAsync objects
      if (this.OnSetDataForRequest != null || urls != null) {
        for (int i = 0; i < sessionCount; i++) {
          this.CallNextStep(https[i], false);
        }
      }
      // Wait while not finished
      if (CheckLiveObjects()) wait.WaitOne();
    }

    private void csHttpAsync_OnFinished(object sender, EventArgs args) {
      this.CallNextStep((csHttpAsync)sender, true);
    }


    List<string> _log = new List<string>();
    private void CallNextStep(csHttpAsync http, bool checkFlag) {
      int thisNo = (int)http.userData;
      lock (this) {
        this.httpSteps[thisNo]++;
        _log.Add(thisNo + ";" + totalBatchSteps + ";" + urls.Count);
        if (this.urls != null && totalBatchSteps < urls.Count) {// Upload files = set data by default
          this.totalBatchSteps++;
          http.requestData.method = "GET";
          http.requestData.url = urls[totalBatchSteps - 1];
          http.requestData.filename = (this.filenames == null ? null : filenames[totalBatchSteps - 1]);
          http.requestData.bodyData = null;
          http.requestData.cancelFlag = false;
        }
        else {
          http.requestData.cancelFlag = true;
        }
        if (this.OnSetDataForRequest != null)
          this.OnSetDataForRequest(this.httpSteps[thisNo], this.totalBatchSteps, http.requestData, this.userData);
        if (!http.requestData.cancelFlag)
          http.requestData.cancelFlag = (job != null) && !job.CheckCancelAndPause();
      }
      if (!http.requestData.cancelFlag) {// New request
        http.CreateRequest();
                Thread.Sleep(300);
        http.Execute();
      }
      else {
        if (checkFlag) this.CheckLiveObjects();
      }
    }

    private bool CheckLiveObjects() {
      bool flag = false; // No live objects
      lock (this) {
        for (int i = 0; i < https.Length && !flag; i++) {
          if (!https[i].requestData.cancelFlag) flag = true;   // Live object exists
        }
      }
      if (!flag) {
      }
      if (!flag) wait.Set();
      return flag;
    }

    public void Dispose() {
      if (https != null) {
        for (int i = 0; i < https.Length; i++) {
          https[i].OnAfterGetResponse -= this.OnAfterGetResponse;
          https[i].OnFinished -= this.csHttpAsync_OnFinished;
          https[i].OnFinished -= this.OnFinished;
          /*if (https[i].response != null) {
            ((IDisposable)https[i].response).Dispose();
            https[i].response = null;
          }*/
        }
      }
      this.OnAfterGetResponse = null;
      this.OnSetDataForRequest = null;
      this.OnFinished = null;
    }
  }

}
