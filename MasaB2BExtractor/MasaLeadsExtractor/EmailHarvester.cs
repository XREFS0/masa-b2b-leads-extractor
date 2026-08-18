using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace MasaLeadsExtractor
{
	// Token: 0x0200000C RID: 12
	public static class EmailHarvester
	{
		// Token: 0x06000037 RID: 55 RVA: 0x00003674 File Offset: 0x00001874
		public static string GetEmail(string Url, string[] ContactPageUrls)
		{
			string result = "";
			Exception error = null;
			ManualResetEvent doneEvent = new ManualResetEvent(false);
			new Thread(new ThreadStart(delegate
			{
				try
				{
					result = EmailHarvester.GetEmailInternal(Url, ContactPageUrls) ?? "";
				}
				catch (Exception ex)
				{
					error = ex;
				}
				finally
				{
					doneEvent.Set();
				}
			}))
			{
				IsBackground = true
			}.Start();
			if (!doneEvent.WaitOne(4000))
			{
				return "";
			}
			if (error != null)
			{
				return "";
			}
			return result;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x000036FC File Offset: 0x000018FC
		private static string GetEmailInternal(string Url, string[] ContactPageUrls)
		{
			string Email = "";
			if (string.IsNullOrWhiteSpace(Url))
			{
				return Email;
			}
			if (!Url.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
			{
				Url = "http://" + Url.Trim(new char[] { '/' });
			}
			Url = Url.Replace("https://", "http://");
			Stopwatch sw = Stopwatch.StartNew();
			string page = WebRequester.GetPage(Url, null, 5000);
			if (string.IsNullOrEmpty(page))
			{
				return Email;
			}
			string clearPage = WebRequester.ClearString(page);
			List<string[]> items = WebRequester.ParseHTML(clearPage, "(mailto\\:|)([\\w\\.\\-]+)@((([\\-\\w]+\\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\\.){3}[0-9]{1,3}))");
			if (items.Count > 0)
			{
				Email = EmailHarvester.FindCorrectEmail(items).Replace("mailto:", "");
				if (!string.IsNullOrEmpty(Email))
				{
					return Email;
				}
			}
			if (sw.ElapsedMilliseconds > 4000L)
			{
				return Email;
			}
			List<string[]> list = WebRequester.ParseHTML(clearPage, "href=(\"|'|)(.*?)(\"|'|)[>|\\s]");
			string contactsPageUrl = "";
			foreach (string[] link in list)
			{
				if (sw.ElapsedMilliseconds > 4000L)
				{
					break;
				}
				if (link.Length >= 3)
				{
					string href = link[2];
					if (!string.IsNullOrEmpty(href))
					{
						bool isContactLink = false;
						foreach (string contactKey in ContactPageUrls)
						{
							if (href.IndexOf(contactKey, StringComparison.InvariantCultureIgnoreCase) >= 0)
							{
								isContactLink = true;
								break;
							}
						}
						if (isContactLink)
						{
							try
							{
								contactsPageUrl = new Uri(new Uri(Url), href).ToString();
							}
							catch
							{
								if (href.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
								{
									contactsPageUrl = href;
								}
								else
								{
									contactsPageUrl = Url.TrimEnd(new char[] { '/' }) + "/" + href.TrimStart(new char[] { '/' });
								}
							}
							string websiteName = Url.Replace("http://", "").Replace("https://", "").Replace("www.", "");
							websiteName = websiteName.Split(new char[] { '/' })[0];
							if ((string.IsNullOrEmpty(websiteName) || contactsPageUrl.IndexOf(websiteName, StringComparison.InvariantCultureIgnoreCase) != -1) && !string.IsNullOrEmpty(contactsPageUrl))
							{
								if (sw.ElapsedMilliseconds > 4000L)
								{
									break;
								}
								string contactPage = WebRequester.GetPage(contactsPageUrl, null, 4000);
								if (!string.IsNullOrEmpty(contactPage))
								{
									List<string[]> mailItems = WebRequester.ParseHTML(WebRequester.ClearString(contactPage), "(mailto\\:|)([\\w\\.\\-]+)@((([\\-\\w]+\\.)+[a-zA-Z]{2,4})|(([0-9]{1,3}\\.){3}[0-9]{1,3}))");
									if (mailItems.Count > 0)
									{
										Email = EmailHarvester.FindCorrectEmail(mailItems).Replace("mailto:", "");
										if (!string.IsNullOrEmpty(Email))
										{
											return Email;
										}
									}
								}
							}
						}
					}
				}
			}
			return Email;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x000039D4 File Offset: 0x00001BD4
		private static string FindCorrectEmail(List<string[]> Items)
		{
			foreach (string[] email in Items)
			{
				if (email[0].IndexOf("@mail.com", StringComparison.InvariantCultureIgnoreCase) == -1 && email[0].IndexOf("example", StringComparison.InvariantCultureIgnoreCase) == -1 && email[0].IndexOf("sentry", StringComparison.InvariantCultureIgnoreCase) == -1 && email[0].IndexOf(".jpg", StringComparison.InvariantCultureIgnoreCase) == -1 && email[0].IndexOf(".wix", StringComparison.InvariantCultureIgnoreCase) == -1 && email[0].IndexOf(".png", StringComparison.InvariantCultureIgnoreCase) == -1)
				{
					return email[0];
				}
			}
			return "";
		}

		// Token: 0x0400001A RID: 26
		private const int GlobalEmailTimeoutMs = 4000;
	}
}
