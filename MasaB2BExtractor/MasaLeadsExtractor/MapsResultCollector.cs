using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;

namespace MasaLeadsExtractor
{
	// Token: 0x02000010 RID: 16
	public class MapsResultCollector
	{
		// Token: 0x06000044 RID: 68 RVA: 0x000042F8 File Offset: 0x000024F8
		public MapsResultCollector(int TaskIndex, MainWindow MainWindow, bool EmailMining = false)
		{
			this._mainWindow = MainWindow;
			this._TaskIndex = TaskIndex;
			this._TaskId = MainWindow.dgvTasks.Rows[TaskIndex].Cells[0].Value.ToString();
			Thread.Sleep(500);
			this._q = this.BuildSearchRequest(MainWindow.dgvTasks.Rows[TaskIndex]);
			this._Category = MainWindow.dgvTasks.Rows[TaskIndex].Cells[1].Value.ToString();
			this._State = MainWindow.dgvTasks.Rows[TaskIndex].Cells[4].Value.ToString();
			if (Startup.AppSettings.IsRandomDelay)
			{
				this.UpdateUIThreadSafe(string.Format("Random Delay Enabled. Working on task {0}. Searching for links.", this._TaskId), 0);
			}
			else
			{
				this.UpdateUIThreadSafe(string.Format("Working on task {0}. Searching for links.", this._TaskId), 0);
			}
			int Iterations = 0;
			bool gotResults = false;
			for (;;)
			{
				string ResponseData = this.GetLinksData(this._q);
				if (Startup.StopDataCollection)
				{
					break;
				}
				if (!string.IsNullOrEmpty(ResponseData) && this.ParseBingResults(ResponseData, EmailMining) > 0)
				{
					gotResults = true;
				}
				if (!gotResults)
				{
					Iterations++;
					if (Iterations >= 3)
					{
						goto IL_01C4;
					}
					string[] q_parts = this._q.Split(new char[] { '+' });
					this._q = "";
					for (int i = 0; i < q_parts.Length - 1; i++)
					{
						this._q = this._q + q_parts[i] + "+";
					}
					if (this._q.Length > 1)
					{
						this._q = this._q.Substring(0, this._q.Length - 1);
					}
				}
				if (gotResults || Startup.StopDataCollection)
				{
					goto IL_01C4;
				}
			}
			return;
			IL_01C4:
			if (Startup.AppSettings.IsRandomDelay)
			{
				Startup.RequestDelay();
				return;
			}
			Thread.Sleep(750);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x000044E8 File Offset: 0x000026E8
		private void UpdateUIThreadSafe(string text, int progress)
		{
			if (this._mainWindow == null || this._mainWindow.IsDisposed || !this._mainWindow.IsHandleCreated)
			{
				return;
			}
			if (this._mainWindow.InvokeRequired)
			{
				this._mainWindow.BeginInvoke(new MapsResultCollector.UpdateUIDelegate(this.UpdateUIThreadSafe), new object[] { text, progress });
				return;
			}
			if (!string.IsNullOrEmpty(text))
			{
				this._mainWindow.lblInfo.Text = text;
			}
			if (progress >= 0 && progress <= 100)
			{
				this._mainWindow.tspProgress.Value = progress;
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00004584 File Offset: 0x00002784
		private int ParseBingResults(string html, bool emailMining)
		{
			if (string.IsNullOrEmpty(html))
			{
				return 0;
			}
			int totalExtracted = 0;
			MatchCollection matches = Regex.Matches(html, "data-entity=\\\"(\\{.*?\\})\\\"", RegexOptions.Singleline);
			int totalMatches = matches.Count;
			if (totalMatches == 0)
			{
				return 0;
			}
			int updateInterval = Math.Max(1, totalMatches / 20);
			int nextUpdateAt = updateInterval;
			foreach (object obj in matches)
			{
				Match i = (Match)obj;
				Thread.Sleep(50);
				if (Startup.StopDataCollection)
				{
					break;
				}
				if (i.Success)
				{
					string encodedJson = i.Groups[1].Value;
					if (!string.IsNullOrEmpty(encodedJson))
					{
						string json = WebUtility.HtmlDecode(encodedJson);
						try
						{
							JObject entity = JObject.Parse(json)["entity"] as JObject;
							if (entity != null)
							{
								LocationData dataItem = new LocationData();
								dataItem.Category = this._Category;
								dataItem.RealCategory = ((string)entity["primaryCategoryName"]) ?? this._Category;
								dataItem.BusinessName = ((string)entity["title"]) ?? "";
								dataItem.Address = ((string)entity["address"]) ?? "";
								dataItem.City = "";
								dataItem.PostalCode = "";
								if (!string.IsNullOrEmpty(dataItem.Address))
								{
									List<string[]> StrParse = WebRequester.ParseHTML(dataItem.Address, "(\\d{5,6}) ([^,]+),");
									if (StrParse.Count > 0)
									{
										dataItem.PostalCode = StrParse[0][1];
										dataItem.City = StrParse[0][2];
									}
									else
									{
										StrParse = WebRequester.ParseHTML(dataItem.Address, "(\\d{5,6}) (.*)");
										if (StrParse.Count > 0)
										{
											dataItem.PostalCode = StrParse[0][1];
											dataItem.City = WebUtility.HtmlDecode(StrParse[0][2]);
										}
										else
										{
											StrParse = WebRequester.ParseHTML(dataItem.Address, ", ([^\\d]+)\\s(\\d{5,6}), (.*)");
											if (StrParse.Count > 0)
											{
												dataItem.City = StrParse[0][1];
												dataItem.PostalCode = StrParse[0][2];
											}
											else
											{
												StrParse = WebRequester.ParseHTML(dataItem.Address, ", (.*), (.*) (\\d{5,6})");
												if (StrParse.Count > 0)
												{
													dataItem.City = StrParse[0][1];
													dataItem.PostalCode = StrParse[0][3];
												}
											}
										}
									}
								}
								dataItem.State = this._State;
								dataItem.Phone = ((string)entity["phone"]) ?? "";
								string website = ((string)entity["website"]) ?? "";
								if (!string.IsNullOrEmpty(website))
								{
									website = website.Trim();
									if (!website.StartsWith("http", StringComparison.InvariantCultureIgnoreCase))
									{
										website = "http://" + website;
									}
								}
								dataItem.Website = website;
								string lat = "";
								string lon = "";
								dataItem.Latitude = lat;
								dataItem.Longitude = lon;
								dataItem.MapLink = "";
								dataItem.DetailsLink = "";
								dataItem.Email = "";
								if (emailMining)
								{
									try
									{
										string[] contactPages = new string[] { "conta", "contact", "kontact" };
										string websiteToUse = dataItem.Website;
										bool flag = string.IsNullOrWhiteSpace(websiteToUse);
										bool isFacebook = !flag && websiteToUse.IndexOf("facebook.com", StringComparison.InvariantCultureIgnoreCase) >= 0;
										if (flag || isFacebook)
										{
											try
											{
												string externalWebsite = WebSearchEngine.SearchWebOnStartpage(dataItem.BusinessName, dataItem.City, dataItem.PostalCode);
												if (!string.IsNullOrWhiteSpace(externalWebsite))
												{
													websiteToUse = externalWebsite;
												}
											}
											catch
											{
											}
										}
										if (!string.IsNullOrWhiteSpace(websiteToUse))
										{
											dataItem.Email = EmailHarvester.GetEmail(websiteToUse, contactPages);
										}
									}
									catch
									{
									}
								}
this.OutputData(dataItem);
							MapsResultCollector.WriteToFile(string.Concat(new string[]
								{
									dataItem.Category, "\t", dataItem.RealCategory, "\t", dataItem.BusinessName, "\t", dataItem.Address, "\t", dataItem.City, "\t",
									dataItem.PostalCode, "\t", dataItem.State, "\t", dataItem.Phone, "\t", dataItem.Website, "\t", dataItem.Email
								}));
								totalExtracted++;
								if (totalExtracted >= nextUpdateAt || totalExtracted == totalMatches)
								{
									string infoText = (Startup.AppSettings.IsRandomDelay ? string.Format("Random Delay enabled. Working on task {0}. Extracted {1} results.", this._TaskId, totalExtracted) : string.Format("Working on task {0}. Extracted {1} results.", this._TaskId, totalExtracted));
									int progress = (int)Math.Round(100.0 * (double)totalExtracted / (double)totalMatches);
									if (progress < 0)
									{
										progress = 0;
									}
									if (progress > 100)
									{
										progress = 100;
									}
									this.UpdateUIThreadSafe(infoText, progress);
									nextUpdateAt += updateInterval;
								}
							}
						}
						catch (Exception ex)
						{
							MapsResultCollector.Log("Error parsing Bing data-entity: " + ex.Message);
						}
					}
				}
			}
			return totalExtracted;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004B60 File Offset: 0x00002D60
		private static string ConvertToString(JToken token)
		{
			if (token == null)
			{
				return "";
			}
			if (token.Type == JTokenType.Float || token.Type == JTokenType.Integer)
			{
				return token.Value<double>().ToString(CultureInfo.InvariantCulture);
			}
			return token.ToString();
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00004BA4 File Offset: 0x00002DA4
		private string BuildBingMapLink(string latitude, string longitude, string title, string address)
		{
			if (string.IsNullOrEmpty(latitude) || string.IsNullOrEmpty(longitude))
			{
				return "";
			}
			string cp = string.Format(CultureInfo.InvariantCulture, "{0}~{1}", latitude, longitude);
			string q = Uri.EscapeDataString((title + " ").Trim());
			return "https://www.bing.com/maps?cp=" + cp + "&q=" + q;
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00004C00 File Offset: 0x00002E00
		private List<string> GetLinks(string Request)
		{
			BrowserAutomator BrowserAutomator = new BrowserAutomator(Request);
			int Iter = 0;
			while (!BrowserAutomator.Completed && !Startup.StopDataCollection)
			{
				Iter++;
				int v = (int)Math.Round((double)(100f * (float)Iter / 75f));
				if (v < 100)
				{
					this.UpdateUIThreadSafe(null, v);
				}
				Thread.Sleep(300);
			}
			List<string> DataUrls = new List<string>();
			if (Startup.StopDataCollection)
			{
				return DataUrls;
			}
			string[] _DataUrls = BrowserAutomator.Response.Split(new char[] { '\r' });
			for (int i = 0; i < _DataUrls.Length; i++)
			{
				DataUrls.Add(_DataUrls[i].Replace("\n", ""));
			}
			return DataUrls;
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00004CB0 File Offset: 0x00002EB0
		private string GetLinksData(string Request)
		{
			BrowserAutomator BrowserAutomator = new BrowserAutomator(Request);
			int Iter = 0;
			while (!BrowserAutomator.Completed && !Startup.StopDataCollection)
			{
				Iter++;
				int v = (int)Math.Round((double)(100f * (float)Iter / 75f));
				if (v < 100)
				{
					this.UpdateUIThreadSafe(null, v);
				}
				Thread.Sleep(100);
			}
			if (Startup.StopDataCollection)
			{
				return "";
			}
			return BrowserAutomator.Response;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00004D18 File Offset: 0x00002F18
		private void OutputData(LocationData DataItem)
		{
			object[] row = new object[]
			{
				DataItem.Category,
				DataItem.RealCategory,
				DataItem.BusinessName,
				DataItem.Address,
				DataItem.City,
				DataItem.State,
				DataItem.PostalCode,
				this._mainWindow.dgvTasks.Rows[this._TaskIndex].Cells[3].Value.ToString(),
				DataItem.Phone,
				DataItem.Email,
				DataItem.Website,
				DataItem.Latitude,
				DataItem.Longitude,
				DataItem.MapLink,
				DataItem.DetailsLink
			};
			MethodInvoker uiUpdate = delegate
			{
				this._mainWindow.dgvResults.Rows.Add(row);
				int resultCount = (this._mainWindow.dgvResults.AllowUserToAddRows ? Math.Max(0, this._mainWindow.dgvResults.Rows.Count - 1) : this._mainWindow.dgvResults.Rows.Count);
				this._mainWindow.toolStripStatusLabel1.Text = string.Format("Extracted results: {0}", resultCount);
			};
			if (this._mainWindow.dgvResults.InvokeRequired)
			{
				this._mainWindow.dgvResults.BeginInvoke(uiUpdate);
				return;
			}
			uiUpdate();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00004E30 File Offset: 0x00003030
		private bool IsInList(List<string> Urls, string Url)
		{
			using (List<string>.Enumerator enumerator = Urls.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current == Url)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00004E88 File Offset: 0x00003088
		private string BuildSearchRequest(DataGridViewRow Row)
		{
			string q = Uri.EscapeDataString(Row.Cells[1].Value.ToString()) + " in ";
			string Loc = "";
			for (int i = 6; i >= 3; i--)
			{
				if (Row.Cells[i].Value != null && Row.Cells[i].Value.ToString().IndexOf("All") == -1)
				{
					Loc += string.Format("+{0}", Uri.EscapeDataString(Row.Cells[i].Value.ToString()));
				}
			}
			if (Loc == "")
			{
				Loc = "+" + Uri.EscapeDataString(Row.Cells[2].Value.ToString());
			}
			return q + Loc;
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00004F69 File Offset: 0x00003169
		public static void WriteToFile(string s)
		{
			FileStream fileStream = new FileStream(Startup.ExportFile, FileMode.Append, FileAccess.Write);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.WriteLine(s);
			streamWriter.Flush();
			streamWriter.Close();
			fileStream.Close();
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00004F94 File Offset: 0x00003194
		private static void Log(string Msg)
		{
			try
			{
				File.AppendAllText("log.txt", string.Format("{0}: {1}{2}", DateTime.Now, Msg, Environment.NewLine));
			}
			catch
			{
			}
		}

		// Token: 0x04000031 RID: 49
		private int _TaskIndex;

		// Token: 0x04000032 RID: 50
		private string _TaskId;

		// Token: 0x04000033 RID: 51
		private string _Category;

		// Token: 0x04000034 RID: 52
		private string _State;

		// Token: 0x04000035 RID: 53
		private string _q;

		// Token: 0x04000036 RID: 54
		private MainWindow _mainWindow;

		// Token: 0x04000037 RID: 55
		public static Random Rnd1;

		// Token: 0x04000038 RID: 56
		private List<string> PageUrls;

		// Token: 0x0200003C RID: 60
		// (Invoke) Token: 0x06000129 RID: 297
		private delegate void UpdateUIDelegate(string text, int progress);
	}
}
