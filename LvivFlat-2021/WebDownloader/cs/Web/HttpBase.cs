using System;
using System.IO;
using System.Net;
using System.Collections.Specialized;
using System.Text;

namespace WebDownloader {
	/// <summary>
	///  ///////////////////////////////// Class csHttpSync
	/// </summary>
	public class csHttpSync : csHttpBase {
		public csHttpSync(csJob pJob, object pUserData)
			: base(pJob, pUserData) {
		}

		protected override void ExecuteStep() {
			try {
				base.ExecuteStep();
				if (!String.IsNullOrEmpty(this.requestData.bodyData)) { // post body data
					using (Stream stream = this.request.GetRequestStream()) {
						base.ProcessRequestStream(stream);
					}
				}
				using (HttpWebResponse response = (HttpWebResponse)this.request.GetResponse()) {
					base.ProcessResponse(response);
				}
				//        this.response = (HttpWebResponse)this.request.GetResponse();
				//      base.ProcessResponse();
			}
			catch (Exception ex) {
				lastException = ex;
				ProcessError(ex);
			}
			finally {
				/*    if (this.response != null) {
          response.Close();
          ((IDisposable)response).Dispose();
          response = null;
        }*/
			}
		}
	}

	/// <summary>
	///  ///////////////////////////////// Class csHttpAsync
	/// </summary>
	public class csHttpAsync : csHttpBase {

		//    public bool finished;
		public event EventHandler OnFinished;

		public csHttpAsync(csJob pJob, object pUserData)
			: base(pJob, pUserData) {
		}

		protected override void ExecuteStep() {
			//      this.finished = false;
			base.ExecuteStep();
			try {
				if (this.requestData.bodyData != null)
					this.request.BeginGetRequestStream(this.SendDataHandler, null);
				else
					this.request.BeginGetResponse(this.GetResponseHandler, null);
			}
			catch (Exception ex) { ProcessError(ex); }
		}

		private void SendDataHandler(IAsyncResult asynchronousResult) {
			try {
				using (Stream stream = request.EndGetRequestStream(asynchronousResult)) {
					base.ProcessRequestStream(stream);
				}
				request.BeginGetResponse(this.GetResponseHandler, null);
			}
			catch (Exception ex) { ProcessError(ex); }
		}

		private void GetResponseHandler(IAsyncResult asynchronousResult) {
			try {
				using (HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult)) {
					base.ProcessResponse(response);
				}
				//        this.response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
				//      base.ProcessResponse();
			}
			catch (Exception ex) {
				lastException = ex;
				ProcessError(ex);
			}
			finally {
				/*if (this.response != null) {
          response.Close();
          ((IDisposable)response).Dispose();
          response = null;
        }*/
			}
		}

		protected override void Finished(bool errorFlag) {
			base.Finished(errorFlag);
			if (this.OnFinished != null) this.OnFinished(this, new EventArgs());
		}
	}

	/// <summary>
	///  ///////////////////////////////// Class csHttpBase
	/// </summary>
	public abstract class csHttpBase {

		public const int httpMaxAttempts = 2;
		public delegate void dlgSetDataForRequest(int thisBatchStep, int totalBatchSteps, csHttpBase.csHttpRequestData requestData, object userData);
		//    public bool setCookie;

		public static string CookiesToString(CookieCollection cookies) {
			if (cookies == null) return null;
			else {
				StringBuilder sb = new StringBuilder();
				foreach (Cookie c in cookies) {
					sb.Append(c.Name + "=" + c.Value + "; ");
				}
				return sb.ToString().Trim();
			}
		}

		public class csHttpRequestData {
			public string method;
			public string url;
			public NameValueCollection headers;
			public string bodyData;
			public string filename;
			public bool cancelFlag = false;
//            public bool allowAutoRedirect = false;
            public bool allowAutoRedirect = true;
            public bool setCookie = false;
			public object userData;
		}
		////////////////////////  Variables /////////////////////////////////
		public csJob job;
		public HttpWebRequest request;
		//    public HttpWebResponse response;
		CookieCollection responseCookies;
		public csHttpRequestData requestData = new csHttpRequestData();
		public event EventHandler OnFinished;
		public event EventHandler OnAfterGetResponse;
		public event EventHandler OnPayload;
		//    public event EventHandler OnFinished;
		public int payloadBufferSize = 8 * 1024;
		public string payloadString;
		public object userData = null;
		public int maxAttempts = httpMaxAttempts;
		protected int currAttempt;
		public int jobMessID = -1;
		public bool jobCancelFlag = false;
		string responseBody;
		public bool silentMode = false;
		public bool errorFlag = false;
		public Exception lastException;
    //		public bool breakOnError = true;
    //    public bool breakOnError = false;
	  private Encoding responseEncoding = Encoding.UTF8;

    public csHttpBase(csJob pJob, object pUserData) {
			this.job = pJob; this.userData = pUserData;
		}

		/*public Stream GetResponseStream() {
      return response.GetResponseStream();
    }*/

		public void CreateRequest() {
			HttpWebRequest referer = this.request;
			this.request = (HttpWebRequest)WebRequest.Create(this.requestData.url);
			this.request.Method = this.requestData.method.Trim().ToUpper();
			if (this.request.Method == "POST")
				this.request.ContentType = @"application/x-www-form-urlencoded";//@"application/x-www-form-urlencoded; charset=UTF-8"
			if (referer == null) { // First call
				if (this.requestData.setCookie) {
					this.request.CookieContainer = csWinApi.GetUriCookieContainer(this.GetUrl());
					if (this.request.CookieContainer == null)
						this.request.CookieContainer = new CookieContainer();
				}
				this.request.AllowAutoRedirect = this.requestData.allowAutoRedirect;

				this.request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
				this.request.Timeout = 1 * 30 * 1000;
				this.request.Accept = "*/*";
				this.request.KeepAlive = true;
				//        this.request.UserAgent = @"Mozilla/4.0 (compatible; MSIE 6.0)";
				//				this.request.UserAgent = @"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0; InfoPath.1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
                //this.request.UserAgent = @"Mozilla/4.0 (compatible; MSIE 8.0; Windows NT 5.1; Trident/4.0)";
                this.request.UserAgent = @"Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36";
				this.request.Headers.Add("Cache-Control", "no-cache");
				/*				if (this.requestData.cookies != null) {
                  foreach (Cookie c in this.requestData.cookies)
                    this.request.CookieContainer.Add(c);
                }*/
				this.CreateHeader(this.requestData.headers);
			}
			else {
				this.request.Credentials = referer.Credentials;
				if (this.requestData.setCookie) {
					this.request.CookieContainer = csWinApi.GetUriCookieContainer(this.GetUrl());
					if (this.request.CookieContainer == null) this.request.CookieContainer = new CookieContainer();
					//				this.request.CookieContainer = referer.CookieContainer;
				}
				this.request.AllowAutoRedirect = referer.AllowAutoRedirect;
				this.request.AutomaticDecompression = referer.AutomaticDecompression;
				this.request.Timeout = referer.Timeout;
				this.CreateHeader(referer.Headers);
				/*				if (this.requestData.cookies != null) {
                  foreach (Cookie c in this.requestData.cookies)
                    this.request.CookieContainer.Add(c);
                }*/
				this.CreateHeader(this.requestData.headers);
				if (this.responseCookies != null && this.requestData.setCookie) {
					foreach (Cookie c in this.responseCookies)
						this.request.CookieContainer.Add(c);
				}
				/*if (this.response != null) {
          if (this.response.Cookies != null && this.requestData.setCookie) {
            foreach (Cookie c in this.response.Cookies)
              this.request.CookieContainer.Add(c);
          }
        }*/
			}
		}

		private void CreateHeader(NameValueCollection headers) {
			if (headers != null) {
				foreach (string s in headers.Keys) {
					switch (s.Trim().ToLower()) {
						case "host":
						case "content-length":
						case "accept-encoding":
							break;
							case "accept": this.request.Accept = headers[s]; break;
							case "user-agent": this.request.UserAgent = headers[s]; break;
							case "content-type": this.request.ContentType = headers[s]; break;
						case "keep-alive":
							case "keepalive": this.request.KeepAlive = bool.Parse(headers[s]); break;
							case "expect": this.request.Expect = headers[s]; break;
						case "connection":
						case "proxy-connection":
							switch (headers[s].Trim().ToLower()) {
								case "keepalive":
								case "keep-alive":
									this.request.KeepAlive = true; break;
									case "close": this.request.KeepAlive = false; break;
									default: throw new Exception("Method 'CreateRequest'. '" + headers[s] +
									                             "' is invalid value for 'Proxy-connection' header entry.");
							}
							break;
						case "set-cookie":
						case "cookie":
							if (this.requestData.setCookie) {
								//								string sCookie = headers[s].Replace("; ", ";"); // ??? add only 1 cookie
								//							this.request.CookieContainer.SetCookies(new Uri(GetUrl()), sCookie);
								string[] sCookies = headers[s].Split(';');
								Uri uri = new Uri(GetUrl());
								for (int i = 0; i < sCookies.Length; i++) {
									int k = sCookies[i].IndexOf('=');
									if (k > 0) {
										string cName = sCookies[i].Substring(0, k).Trim();
										string cValue = sCookies[i].Substring(k + 1).Trim();
										Cookie cookie = new Cookie(cName, cValue, uri.LocalPath, uri.Host);
										this.request.CookieContainer.Add(cookie);
									}
								}
							}
							break;
							case "referer": this.request.Referer = headers[s]; break;
							case "proxy-authorization": break;
						default:
							if (this.request.Headers.GetValues(s) != null)
								this.request.Headers.Remove(s);
							this.request.Headers.Add(s, headers[s]); break;
					}
				}
			}
		}
		private string GetUrl() {
			return this.request.RequestUri.ToString();
		}
		public string GetResponseBody() {
			if (String.IsNullOrEmpty(this.requestData.filename)) return this.responseBody;
			else return File.ReadAllText(this.requestData.filename, this.responseEncoding);
		}

		public void Execute() {
			this.currAttempt = 0;
			this.ExecuteStep();
		}
		protected virtual void ExecuteStep() {
			if (this.jobCancelFlag) return;
			/*			System.Net.Cache.RequestCachePolicy policy =
              new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            request.CachePolicy = policy;
            request.CookieContainer = new CookieContainer();*/

			this.errorFlag = false;
			this.lastException = null;
			this.responseBody = null;
			this.currAttempt++;
			if (currAttempt == 1) {
				if (!this.silentMode) jobMessID = csJob.ShowMessage(job, "Start download from " + GetUrl(), csJob.MessageType.start);
			}
			else {
				if (!this.silentMode) csJob.ShowMessage(job, "Attempt " + currAttempt + ". Start download for " + GetUrl(), csJob.MessageType.start, jobMessID);
			}
		}

		protected void ProcessRequestStream(Stream requestStream) {
			byte[] bytes = Encoding.UTF8.GetBytes(this.requestData.bodyData);
			requestStream.Write(bytes, 0, bytes.Length);
			requestStream.Close();
		}

		protected void ProcessResponse( HttpWebResponse response ) {
			try {
			  var contentType = new System.Net.Mime.ContentType(response.Headers[HttpResponseHeader.ContentType]);
			  if (!string.IsNullOrEmpty(contentType.CharSet))
			  {
			    this.responseEncoding = Encoding.GetEncoding(contentType.CharSet);
			  }

        if (!string.IsNullOrEmpty(this.requestData.filename)) { // SaveToFile mode
					if (File.Exists(this.requestData.filename)) File.Delete(this.requestData.filename);
					long bytes;
					using (Stream stream = response.GetResponseStream()) {
						bytes = csUtilsFile.SaveStreamToFile(stream, this.requestData.filename);
						if (bytes == 0) throw new Exception("No data get from response");
					}
					if (!this.silentMode) csJob.ShowMessage(job, bytes.ToString("N0") + " bytes saved from " + GetUrl() + " to " + this.requestData.filename, csJob.MessageType.done, jobMessID);
				}
				else {
					if (this.OnPayload == null) { // SaveToString(responseBody) mode
						using (Stream stream = response.GetResponseStream()) {
							this.responseBody = csUtilsFile.GetStringFromStream(stream, this.responseEncoding);
						}
					}
					else {// Payload mode
						byte[] bb = new byte[this.payloadBufferSize];
						StringBuilder sb = new StringBuilder();
						int cnt = 0;
						using (Stream stream = response.GetResponseStream()) {
							do {
								cnt = stream.Read(bb, 0, this.payloadBufferSize);
								if (cnt > 0) {
									this.payloadString = this.responseEncoding.GetString(bb, 0, cnt);
									this.OnPayload(this, new EventArgs());
								}
							}
							while (cnt > 0 && this.job.CheckCancelAndPause());
						}
					}
					if (!this.silentMode) csJob.ShowMessage(job, GetUrl() + " uploaded", csJob.MessageType.done, jobMessID);
				}
				response.Close();
				if (this.OnAfterGetResponse != null) {
					this.OnAfterGetResponse(this, new EventArgs());
				}
				this.Finished(false);
			}
			catch (Exception ex) {
				this.lastException = ex;
				ProcessError(ex);
			}
			finally {
				/*if (this.response != null) {
          response.Close();
          ((IDisposable)response).Dispose();
          response = null;
        }*/
			}
		}

		protected void ProcessError(Exception ex) {
			/*if (this.response != null) {
        response.Close();
        ((IDisposable)response).Dispose();
      }*/
			string s = (this.currAttempt > 1 ? "Attempt: " + this.currAttempt.ToString() + ". " : "") +
				"Error while download from " + GetUrl() + ". Error message: " + ex.ToString();
			if (this.currAttempt >= this.maxAttempts) {
				if (!this.silentMode) csJob.ShowMessage(job, s, csJob.MessageType.error, jobMessID);
				this.Finished(true);
			}
			else {
				if (!this.silentMode) csJob.ShowMessage(job, s, csJob.MessageType.warning);
				if (job != null && !this.job.CheckCancelAndPause())
					this.Finished(true);
				else {
					this.CreateRequest();
					ExecuteStep();
				}
			}
		}

		protected virtual void Finished(bool errorFlag) {
			this.errorFlag = errorFlag;
			if (this.OnFinished != null) {
				this.OnFinished(this, new EventArgs());
			}
			if (this.errorFlag)
				csJob.ShowError(job, new Exception("Break on error while download from " + GetUrl())); // break
//			bool x = this.job.CancellationPending;
			this.jobCancelFlag = this.job != null && !this.job.CheckCancelAndPause();
		}
	}

}
