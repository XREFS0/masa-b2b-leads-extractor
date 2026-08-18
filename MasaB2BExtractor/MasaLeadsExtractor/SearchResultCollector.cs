using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace MasaLeadsExtractor
{
	// Token: 0x02000011 RID: 17
	public class SearchResultCollector
	{
		// Token: 0x06000050 RID: 80 RVA: 0x00004FDC File Offset: 0x000031DC
		public SearchResultCollector(string Url, string Category, string State, bool EmailMining = false)
		{
			this._Url = Url;
			this._Category = Category;
			this._State = State;
			this._EmailMining = EmailMining;
			this.DataItem = new LocationData();
			this.Done = false;
			new Timer(new TimerCallback(this.TimeoutHappens), null, 10000, 10000);
			this.MainThread = new Thread(new ThreadStart(this.GetData));
			this.MainThread.Start();
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00005068 File Offset: 0x00003268
		public void TimeoutHappens(object State)
		{
			this.MainThread.Abort();
			this.Done = true;
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000507C File Offset: 0x0000327C
		public void GetData()
		{
			string _OutputHtml = "";
			try
			{
				HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(this._Url);
				httpWebRequest.UserAgent = "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 140.0.0.0 Safari / 537.36";
				httpWebRequest.ProtocolVersion = new Version(1, 0);
				httpWebRequest.KeepAlive = false;
				using (HttpWebResponse response = (HttpWebResponse)httpWebRequest.GetResponse())
				{
					using (Stream receiveStream = response.GetResponseStream())
					{
						StreamReader readStream;
						if (string.IsNullOrWhiteSpace(response.CharacterSet))
						{
							readStream = new StreamReader(receiveStream);
						}
						else
						{
							readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
						}
						_OutputHtml = readStream.ReadToEnd().Replace("\\n", "A").Replace("[", "-")
							.Replace("]", "-")
							.Replace("\\", ">");
						Thread.Sleep(1000);
						response.Close();
						readStream.Close();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			GC.Collect();
			try
			{
				if (!string.IsNullOrEmpty(_OutputHtml))
				{
					List<string[]> Items = new List<string[]>();
					this.DataItem.Category = this._Category;
					Items = WebRequester.ParseHTML(_OutputHtml, "null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,null,>\"(.*?)>\"-");
					try
					{
						if (Items.Count > 0)
						{
							this.DataItem.RealCategory = Items[0][1];
						}
						else
						{
							this.DataItem.RealCategory = this.DataItem.Category;
						}
					}
					catch
					{
					}
					Items = WebRequester.ParseHTML(_OutputHtml, "---7,-->\"(.*?)\"-");
					try
					{
						if (Items.Count > 0)
						{
							this.DataItem.BusinessName = Items[0][1].Replace(">>u0026", "").Replace(">", "");
						}
						else
						{
							Items = WebRequester.ParseHTML(_OutputHtml, ",null,null,null,-null,null,null,>\"(.*?)\",");
							try
							{
								if (Items.Count > 0)
								{
									this.DataItem.BusinessName = Items[0][1].Replace(">>u0026", "").Replace(">", "");
								}
								else
								{
									this.DataItem.BusinessName = "N/A";
								}
							}
							catch
							{
							}
						}
					}
					catch
					{
					}
					Items = WebRequester.ParseHTML(_OutputHtml, "-1,-->\"(.*?)>\"");
					try
					{
						if (Items.Count > 0)
						{
							this.DataItem.Address = Items[0][1].Replace(">>u0026", "");
						}
						else
						{
							this.DataItem.Address = "";
						}
					}
					catch
					{
					}
					Items = WebRequester.ParseHTML(_OutputHtml, "-4,-->\"(.*?)>\"");
					try
					{
						if (Items.Count > 0)
						{
							this.DataItem.City = Items[0][1].Replace(">>u0026", "");
						}
						else
						{
							this.DataItem.City = "";
						}
					}
					catch
					{
					}
					List<string[]> StrParse = WebRequester.ParseHTML(this.DataItem.Address, "(\\d{5,6}) ([^,]+),");
					if (StrParse.Count > 0)
					{
						this.DataItem.PostalCode = StrParse[0][1];
						this.DataItem.City = StrParse[0][2];
					}
					else
					{
						StrParse = WebRequester.ParseHTML(this.DataItem.Address, "(\\d{5,6}) (.*)");
						if (StrParse.Count > 0)
						{
							this.DataItem.PostalCode = StrParse[0][1];
							this.DataItem.City = WebUtility.HtmlDecode(StrParse[0][2]);
						}
						else
						{
							StrParse = WebRequester.ParseHTML(this.DataItem.Address, ", ([^\\d]+)\\s(\\d{5,6}), (.*)");
							if (StrParse.Count > 0)
							{
								this.DataItem.City = StrParse[0][1];
								this.DataItem.PostalCode = StrParse[0][2];
							}
							else
							{
								StrParse = WebRequester.ParseHTML(this.DataItem.Address, ", (.*), (.*) (\\d{5,6})");
								if (StrParse.Count > 0)
								{
									this.DataItem.City = StrParse[0][1];
									this.DataItem.PostalCode = StrParse[0][3];
								}
							}
						}
					}
					this.DataItem.State = this._State;
					Items = WebRequester.ParseHTML(_OutputHtml, "https://www.google.com/maps/preview/place/([^/]+)/@(.*?),(.*?),");
					if (Items.Count > 0)
					{
						this.DataItem.Latitude = Items[0][2];
						this.DataItem.Longitude = Items[0][3];
					}
					else
					{
						this.DataItem.Latitude = "";
						this.DataItem.Longitude = "";
					}
					Items = WebRequester.ParseHTML(_OutputHtml, "url\\?q>>u003d(.*?)>>");
					try
					{
						if (Items.Count > 0 && Items[0][1].IndexOf(':') > -1 && Items[0][1].IndexOf("google") == -1)
						{
							this.DataItem.Website = Items[0][1].Split(new char[] { '>' })[0];
						}
						else
						{
							this.DataItem.Website = "";
						}
					}
					catch
					{
						this.Done = true;
					}
					Items = WebRequester.ParseHTML(_OutputHtml, "tel:(.*?)>\"");
					try
					{
						if (Items.Count > 0)
						{
							this.DataItem.Phone = " " + Items[0][1];
						}
						else
						{
							this.DataItem.Phone = "";
						}
					}
					catch
					{
						this.Done = true;
					}
					if (this.DataItem.Website != "" && this._EmailMining)
					{
						this.DataItem.Email = EmailHarvester.GetEmail(this.DataItem.Website, new string[] { "conta" });
					}
					this.DataItem.MapLink = this._Url;
					Items = WebRequester.ParseHTML(_OutputHtml, ">\",null,null,-null,null,null,null,null,null,null,(.*?),(\\d+)-,null,null");
					try
					{
						if (Items.Count > 0 && Items[0][1].IndexOf(">") == -1)
						{
							this.DataItem.DetailsLink = Items[0][1].Replace("null,", "") + " / " + Items[0][2] + " reviews";
						}
						else
						{
							List<string[]> Items2 = WebRequester.ParseHTML(_OutputHtml, ">\"-,null,null,null,(.*?),(\\d+)-,null,null,");
							try
							{
								if (Items2.Count > 0 && Items2[1][1].IndexOf(">") == -1)
								{
									this.DataItem.DetailsLink = Items2[1][1].Replace("null,", "") + " / " + Items2[1][2] + " reviews";
								}
							}
							catch
							{
							}
						}
					}
					catch
					{
					}
					SearchResultCollector.WriteToFile(string.Concat(new string[]
						{
							this.DataItem.Category,
							"\t",
							this.DataItem.RealCategory,
							"\t",
							this.DataItem.BusinessName,
							"\t",
							this.DataItem.Address,
							"\t",
							this.DataItem.City,
							"\t",
							this.DataItem.PostalCode,
							"\t",
							this.DataItem.State,
							"\t",
							this.DataItem.Phone,
							"\t",
							this.DataItem.Website,
							"\t",
							this.DataItem.Email,
							"\t",
							this.DataItem.DetailsLink
						}));
					this.Done = true;
				}
				else
				{
					this.Done = true;
				}
			}
			catch
			{
				this.Done = true;
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00004F69 File Offset: 0x00003169
		public static void WriteToFile(string s)
		{
			FileStream fileStream = new FileStream(Startup.ExportFile, FileMode.Append, FileAccess.Write);
			StreamWriter streamWriter = new StreamWriter(fileStream);
			streamWriter.WriteLine(s);
			streamWriter.Flush();
			streamWriter.Close();
			fileStream.Close();
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00005970 File Offset: 0x00003B70
		public static bool IsAllDigits(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				if (!char.IsDigit(s[i]))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000059A1 File Offset: 0x00003BA1
		public void Dispose()
		{
			this.DataItem = null;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000059AC File Offset: 0x00003BAC
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

		// Token: 0x04000039 RID: 57
		private string _Url;

		// Token: 0x0400003A RID: 58
		private string _Category;

		// Token: 0x0400003B RID: 59
		private string _State;

		// Token: 0x0400003C RID: 60
		private bool _EmailMining;

		// Token: 0x0400003D RID: 61
		private Thread MainThread;

		// Token: 0x0400003E RID: 62
		public LocationData DataItem = new LocationData();

		// Token: 0x0400003F RID: 63
		public bool Done;
	}
}
