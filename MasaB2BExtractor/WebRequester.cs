using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

// Token: 0x02000005 RID: 5
public static class WebRequester
{
	// Token: 0x06000014 RID: 20 RVA: 0x00002790 File Offset: 0x00000990
	public static string GetPage(string Url, ProxyConnector Proxy)
	{
		return WebRequester.GetPage(Url, Proxy, 5000);
	}

	// Token: 0x06000015 RID: 21 RVA: 0x000027A0 File Offset: 0x000009A0
	public static string GetPage(string Url, ProxyConnector Proxy, int timeoutMs)
	{
		string result = "";
		Exception error = null;
		HttpWebRequest request = null;
		ManualResetEvent done = new ManualResetEvent(false);
		ThreadPool.QueueUserWorkItem(delegate(object _)
		{
			try
			{
				result = WebRequester.GetPageInner(Url, Proxy, timeoutMs, out request);
			}
			catch (Exception ex)
			{
				error = ex;
			}
			finally
			{
				done.Set();
			}
		});
		if (!done.WaitOne(timeoutMs))
		{
			try
			{
				HttpWebRequest request2 = request;
				if (request2 != null)
				{
					request2.Abort();
				}
			}
			catch
			{
			}
			return "";
		}
		if (error != null)
		{
			return "";
		}
		return result;
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00002854 File Offset: 0x00000A54
	private static string GetPageInner(string Url, ProxyConnector Proxy, int timeoutMs, out HttpWebRequest myHttpWebRequest)
	{
		myHttpWebRequest = null;
		string text;
		try
		{
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.DefaultConnectionLimit = 5000;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			myHttpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
			myHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36";
			myHttpWebRequest.KeepAlive = false;
			myHttpWebRequest.MaximumAutomaticRedirections = 5;
			myHttpWebRequest.AllowAutoRedirect = true;
			if (Proxy != null)
			{
				WebProxy myProxy = new WebProxy(string.Format("{0}:{1}", Proxy.IP, Proxy.Port), false);
				myHttpWebRequest.Proxy = myProxy;
			}
			myHttpWebRequest.Timeout = timeoutMs;
			myHttpWebRequest.ReadWriteTimeout = timeoutMs;
			using (HttpWebResponse myHttpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse())
			{
				using (Stream dataStream = myHttpWebResponse.GetResponseStream())
				{
					using (StreamReader reader = new StreamReader(dataStream))
					{
						text = reader.ReadToEnd();
					}
				}
			}
		}
		catch (WebException)
		{
			text = "";
		}
		catch (Exception)
		{
			text = "";
		}
		return text;
	}

	// Token: 0x06000017 RID: 23 RVA: 0x00002988 File Offset: 0x00000B88
	public static string GetPage(string Url, string PostData, ProxyConnector Proxy)
	{
		string text;
		try
		{
			ServicePointManager.Expect100Continue = false;
			ServicePointManager.DefaultConnectionLimit = 5000;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
			HttpWebRequest myHttpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
			myHttpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36";
			myHttpWebRequest.KeepAlive = false;
			myHttpWebRequest.MaximumAutomaticRedirections = 5;
			myHttpWebRequest.AllowAutoRedirect = true;
			if (Proxy != null)
			{
				WebProxy myProxy = new WebProxy(string.Format("{0}:{1}", Proxy.IP, Proxy.Port), false);
				myHttpWebRequest.Proxy = myProxy;
			}
			myHttpWebRequest.Timeout = 5000;
			myHttpWebRequest.Method = "POST";
			byte[] byteArray = Encoding.UTF8.GetBytes(PostData);
			myHttpWebRequest.ContentType = "application/x-www-form-urlencoded";
			myHttpWebRequest.ContentLength = (long)byteArray.Length;
			Stream requestStream = myHttpWebRequest.GetRequestStream();
			requestStream.Write(byteArray, 0, byteArray.Length);
			requestStream.Close();
			HttpWebResponse httpWebResponse = (HttpWebResponse)myHttpWebRequest.GetResponse();
			Stream responseStream = httpWebResponse.GetResponseStream();
			StreamReader streamReader = new StreamReader(responseStream);
			string responseFromServer = streamReader.ReadToEnd();
			streamReader.Close();
			streamReader.Dispose();
			responseStream.Close();
			responseStream.Dispose();
			httpWebResponse.Close();
			text = responseFromServer;
		}
		catch
		{
			text = "";
		}
		return text;
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00002AC4 File Offset: 0x00000CC4
	public static string GetMarkeredText(string BPMarker, string EPMarker, string HTML, ref int StartPos)
	{
		int BeginPos = HTML.IndexOf(BPMarker, StartPos, StringComparison.InvariantCultureIgnoreCase);
		if (BeginPos <= -1)
		{
			return "";
		}
		int EndPos = HTML.IndexOf(EPMarker, BeginPos, StringComparison.InvariantCultureIgnoreCase);
		if (EndPos > -1)
		{
			StartPos = EndPos + EPMarker.Length;
			string Text = "";
			try
			{
				Text = HTML.Substring(BeginPos + BPMarker.Length, EndPos - BeginPos - BPMarker.Length);
			}
			catch
			{
			}
			return Text;
		}
		StartPos = HTML.Length - 1;
		string Text2 = "";
		try
		{
			Text2 = HTML.Substring(BeginPos + BPMarker.Length, HTML.Length - BeginPos - BPMarker.Length);
		}
		catch
		{
		}
		return Text2;
	}

	// Token: 0x06000019 RID: 25 RVA: 0x00002B74 File Offset: 0x00000D74
	public static string ClearTags(string HTML)
	{
		HTML = HTML.Trim().Replace("\n", string.Empty);
		HTML = HTML.Trim().Replace("\r", string.Empty);
		HTML = HTML.Trim().Replace("\t", string.Empty);
		HTML = HTML.Trim().Replace("&nbsp;", " ");
		return Regex.Replace(HTML, "<[^>]*>", " ").Trim();
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00002BF4 File Offset: 0x00000DF4
	public static List<string[]> ParseHTML(string HTML, string Template)
	{
		List<string[]> Results = new List<string[]>();
		if (string.IsNullOrEmpty(HTML) || string.IsNullOrEmpty(Template))
		{
			return Results;
		}
		try
		{
			foreach (object obj in WebRequester._regexCache.GetOrAdd(Template, (string t) => new Regex(t, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline)).Matches(HTML))
			{
				Match match = (Match)obj;
				string[] values = new string[match.Groups.Count];
				for (int i = 0; i < match.Groups.Count; i++)
				{
					values[i] = match.Groups[i].Value;
				}
				Results.Add(values);
			}
		}
		catch (ArgumentException)
		{
		}
		return Results;
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00002CE8 File Offset: 0x00000EE8
	public static string ClearString(string Source)
	{
		string text;
		try
		{
			Source = Source.Replace("   ", " ");
			char[] Result = Source.ToCharArray();
			char[] CharsToRemove = new char[] { '\n', '\r', '\t' };
			for (int i = 0; i < Source.Length - 1; i++)
			{
				if (Source[i] == ' ' && Source[i + 1] == ' ')
				{
					Result[i] = '*';
					Result[i + 1] = '*';
				}
				for (int j = 0; j < CharsToRemove.Length; j++)
				{
					if (Result[i] == CharsToRemove[j])
					{
						Result[i] = '*';
					}
				}
			}
			text = new string(Result).Replace("*", "");
		}
		catch
		{
			text = "";
		}
		return text;
	}

	// Token: 0x04000009 RID: 9
	private static readonly ConcurrentDictionary<string, Regex> _regexCache = new ConcurrentDictionary<string, Regex>();

	// Token: 0x02000037 RID: 55
	public struct Brand
	{
		// Token: 0x04000155 RID: 341
		public string Name;

		// Token: 0x04000156 RID: 342
		public string Url;
	}

	// Token: 0x02000038 RID: 56
	public struct Parameter
	{
		// Token: 0x04000157 RID: 343
		public string Name;

		// Token: 0x04000158 RID: 344
		public string Value;
	}
}
