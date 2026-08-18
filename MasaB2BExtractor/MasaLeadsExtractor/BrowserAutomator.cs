using System;
using System.Threading;

namespace MasaLeadsExtractor
{
	// Token: 0x02000019 RID: 25
	public class BrowserAutomator
	{
		// Token: 0x060000C6 RID: 198 RVA: 0x0000D040 File Offset: 0x0000B240
		public BrowserAutomator(string Request)
		{
			this._Request = Request;
			this.Completed = false;
			try
			{
				this.MainThread = new Thread(new ThreadStart(this.RunProcess));
				this.MainThread.Start();
			}
			catch
			{
			}
		}

		// Token: 0x060000C7 RID: 199 RVA: 0x0000D098 File Offset: 0x0000B298
		private void RunProcess()
		{
			try
			{
				string page = WebRequester.GetPage(string.Format("https://www.bing.com/maps/overlaybfpr?q={0}&count={1}", this._Request, Startup.AppSettings.NumberOfResultsPerZipCode), null, 5000);
				this.Response = WebRequester.ClearString(page);
			}
			catch (Exception)
			{
				this.Response = "";
			}
			finally
			{
				this.Completed = true;
			}
		}

		// Token: 0x040000C7 RID: 199
		public bool Completed;

		// Token: 0x040000C8 RID: 200
		public string Response;

		// Token: 0x040000C9 RID: 201
		private Thread MainThread;

		// Token: 0x040000CA RID: 202
		private string _Request;
	}
}
