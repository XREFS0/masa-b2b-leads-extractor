using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MasaLeadsExtractor
{
	// Token: 0x0200000F RID: 15
	public class BusinessProfileScraper
	{
		// Token: 0x06000042 RID: 66 RVA: 0x000041F8 File Offset: 0x000023F8
		public BusinessProfileScraper(string PageUrl)
		{
			this._PageUrl = PageUrl;
			this.CompanyData = new LocationData();
			this.Done = false;
			this.MainThread = new Thread(new ThreadStart(this.GetData));
			this.MainThread.Start();
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00004248 File Offset: 0x00002448
		public void GetData()
		{
			Process process = new Process();
			ProcessStartInfo startInfo = new ProcessStartInfo
			{
				WindowStyle = ProcessWindowStyle.Hidden,
				UseShellExecute = false,
				RedirectStandardOutput = true,
				CreateNoWindow = true,
				FileName = "phantomjs.exe",
				Arguments = string.Format("\"{0}\\{1}\" {2}", Directory.GetCurrentDirectory(), "index_page.js", this._PageUrl)
			};
			process.StartInfo = startInfo;
			process.Start();
			string OutputHtml = process.StandardOutput.ReadToEnd();
			File.WriteAllText("debug_page.html", OutputHtml);
			if (WebRequester.ParseHTML(OutputHtml, "").Count > 0)
			{
				this.CompanyData.Category = "";
			}
			this.Done = true;
		}

		// Token: 0x0400002D RID: 45
		private Thread MainThread;

		// Token: 0x0400002E RID: 46
		private string _PageUrl;

		// Token: 0x0400002F RID: 47
		public LocationData CompanyData;

		// Token: 0x04000030 RID: 48
		public bool Done;
	}
}
