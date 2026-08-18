using System;
using System.Collections.Generic;
using System.Threading;

namespace MasaLeadsExtractor
{
	// Token: 0x02000021 RID: 33
	public static class WebSearchEngine
	{
		// Token: 0x060000FE RID: 254 RVA: 0x000110AC File Offset: 0x0000F2AC
		public static string GetWeb(string Url, string[] ContactPageUrls)
		{
			if (string.IsNullOrWhiteSpace(Url))
			{
				return "";
			}
			string result = "";
			bool completed = false;
			Exception error = null;
			string text;
			try
			{
				ManualResetEvent resetEvent = new ManualResetEvent(false);
				ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					try
					{
						result = WebSearchEngine.GetWebInternal(Url);
						completed = true;
					}
					catch (Exception ex)
					{
						error = ex;
					}
					finally
					{
						resetEvent.Set();
					}
				});
				if ((resetEvent.WaitOne(3000) & completed) && error == null)
				{
					text = result;
				}
				else
				{
					text = "";
				}
			}
			catch
			{
				text = "";
			}
			return text;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x0001115C File Offset: 0x0000F35C
		public static string SearchWebOnStartpage(string businessName, string city, string postalCode)
		{
			if (string.IsNullOrWhiteSpace(businessName))
			{
				return "";
			}
			string result = "";
			bool completed = false;
			string text;
			try
			{
				ManualResetEvent resetEvent = new ManualResetEvent(false);
				ThreadPool.QueueUserWorkItem(delegate(object state)
				{
					try
					{
						result = WebSearchEngine.SearchWebOnStartpageInternal(businessName, city, postalCode);
						completed = true;
					}
					catch
					{
					}
					finally
					{
						resetEvent.Set();
					}
				});
				if (resetEvent.WaitOne(3000) & completed)
				{
					text = result;
				}
				else
				{
					text = "";
				}
			}
			catch
			{
				text = "";
			}
			return text;
		}

		// Token: 0x06000100 RID: 256 RVA: 0x0001120C File Offset: 0x0000F40C
		private static string SearchWebOnStartpageInternal(string businessName, string city, string postalCode)
		{
			try
			{
				string query = businessName;
				if (!string.IsNullOrWhiteSpace(city))
				{
					query = query + " " + city;
				}
				if (!string.IsNullOrWhiteSpace(postalCode))
				{
					query = query + " " + postalCode;
				}
				string encodedQuery = Uri.EscapeDataString(query);
				string page = WebRequester.GetPage("https://www.startpage.com/do/dsearch?qsr=it&query=" + encodedQuery, null, 3000);
				if (string.IsNullOrEmpty(page))
				{
					return "";
				}
				List<string[]> results = WebRequester.ParseHTML(page, "<div class=\"result(.*?)\">(.*?)</div>");
				if (results.Count > 0)
				{
					List<string[]> links = WebRequester.ParseHTML(results[0][2], "<a href=\"(.*?)\"");
					if (links.Count > 0)
					{
						string url = links[0][1];
						if (url.Contains("http"))
						{
							try
							{
								Uri uri = new Uri(url);
								return uri.Scheme + "://" + uri.Host;
							}
							catch
							{
								return url;
							}
						}
					}
				}
				List<string[]> altResults = WebRequester.ParseHTML(page, "class=\"result\"(.*?)</div>");
				if (altResults.Count > 0)
				{
					List<string[]> links2 = WebRequester.ParseHTML(altResults[0][1], "href=\"(https?://[^\"]+)\"");
					if (links2.Count > 0)
					{
						string url2 = links2[0][1];
						try
						{
							Uri uri2 = new Uri(url2);
							return uri2.Scheme + "://" + uri2.Host;
						}
						catch
						{
							return url2;
						}
					}
				}
			}
			catch
			{
			}
			return "";
		}

		// Token: 0x06000101 RID: 257 RVA: 0x000113BC File Offset: 0x0000F5BC
		private static string GetWebInternal(string Url)
		{
			string Web = "";
			try
			{
				Url = Url.Replace("https:", "http:");
				string Page = WebRequester.GetPage(Url, null, 3000);
				if (string.IsNullOrEmpty(Page))
				{
					return "";
				}
				string ClearPage = WebRequester.ClearString(Page);
				List<string[]> ItemsA = WebRequester.ParseHTML(ClearPage, "\"(?i:WWW)\"(.*?)href=\"(.*?)\" (.*?)scheda_azienda__cta_sitoweb\"");
				if (ItemsA.Count > 0)
				{
					Web = ItemsA[0][2].Replace(" ", "");
					return Web;
				}
				List<string[]> ItemsB = WebRequester.ParseHTML(ClearPage, "data-pag=\"multilink(.*?)\"");
				if (ItemsB.Count > 0)
				{
					Web = ItemsB[0][1].Replace("/http", "http");
					return Web;
				}
				List<string[]> ItemsC = WebRequester.ParseHTML(ClearPage, "sito web(.*?)href=\"(.*?)\"");
				if (ItemsC.Count > 0)
				{
					Web = ItemsC[0][1].Replace("/http", "http");
				}
			}
			catch
			{
			}
			return Web;
		}

		// Token: 0x06000102 RID: 258 RVA: 0x000114BC File Offset: 0x0000F6BC
		private static string FindCorrectWeb(List<string[]> Items)
		{
			try
			{
				foreach (string[] web in Items)
				{
					if (!string.IsNullOrEmpty(web[0]))
					{
						string webLower = web[0].ToLower();
						if (!webLower.Contains("+100060602430") && !webLower.Contains("@mail.com") && !webLower.Contains("example") && !webLower.Contains(".png"))
						{
							return web[0].Replace(" ", "");
						}
					}
				}
			}
			catch
			{
			}
			return "";
		}

		// Token: 0x0400014A RID: 330
		private const int TIMEOUT_MS = 3000;
	}
}
