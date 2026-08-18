using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

namespace MasaLeadsExtractor
{
	// Token: 0x0200001C RID: 28
	public class AppConfiguration
	{
		// Token: 0x060000D9 RID: 217
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern uint VerLanguageName(uint wLang, [Out] char[] szLang, int nSize);

		// Token: 0x060000DA RID: 218 RVA: 0x0000DE88 File Offset: 0x0000C088
		public AppConfiguration()
		{
			this.NumberOfResultsPerZipCode = 30;
			CultureInfo Culture = CultureInfo.CurrentCulture;
			if (Culture.Name.IndexOf("it") > -1)
			{
				this.Language = 1;
			}
			else if (Culture.Name.IndexOf("de") > -1)
			{
				this.Language = 2;
			}
			else if (Culture.Name.IndexOf("fr") > -1)
			{
				this.Language = 3;
			}
			else if (Culture.Name.IndexOf("es") > -1)
			{
				this.Language = 4;
			}
			else
			{
				this.Language = 0;
			}
			this.ColumnsToShow = new bool[15];
			this.ColumnsToExport = new bool[15];
			for (int i = 0; i < 15; i++)
			{
				this.ColumnsToShow[i] = true;
				this.ColumnsToExport[i] = true;
			}
			this.ExtractEmails = true;
			this.AutoExport = false;
			this.ProxySourcesList = new string[]
			{
				"http://gatherproxy.com/proxylist/country/?c=United%20States", "http://gatherproxy.com/proxylist/country/?c=Canada", "http://txt.proxyspy.net/proxy.txt", "http://dogdev.net/Proxy/US?port=8080", "", "", "", "", "", "",
				"", "", "", ""
			};
			this.CSVDelimiter = 1;
			this.CSVEncoding = 2;
			this.Categories = new List<string>();
			this.Locations = new List<TargetLocation>();
			this.Tasks = new List<ExtractionTask>();
		}

		// Token: 0x060000DB RID: 219 RVA: 0x0000E018 File Offset: 0x0000C218
		public bool Save(string FName)
		{
			XmlSerializer writer = new XmlSerializer(typeof(AppConfiguration));
			bool flag;
			try
			{
				StreamWriter file = new StreamWriter(FName);
				writer.Serialize(file, this);
				file.Close();
				flag = true;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		// Token: 0x060000DC RID: 220 RVA: 0x0000E064 File Offset: 0x0000C264
		public static AppConfiguration Load(string FName)
		{
			XmlSerializer reader = new XmlSerializer(typeof(AppConfiguration));
			AppConfiguration settings2;
			try
			{
				StreamReader file = new StreamReader(FName);
				AppConfiguration settings = (AppConfiguration)reader.Deserialize(file);
				file.Close();
				settings2 = settings;
			}
			catch
			{
				settings2 = new AppConfiguration();
			}
			return settings2;
		}

		// Token: 0x040000E6 RID: 230
		public int Language;

		// Token: 0x040000E7 RID: 231
		public string RegEmail;

		// Token: 0x040000E8 RID: 232
		public string RegCode;

		// Token: 0x040000E9 RID: 233
		public string RegPCName;

		// Token: 0x040000EA RID: 234
		public bool[] ColumnsToShow;

		// Token: 0x040000EB RID: 235
		public bool[] ColumnsToExport;

		// Token: 0x040000EC RID: 236
		public bool AutoExport;

		// Token: 0x040000ED RID: 237
		public string AutoExportPath;

		// Token: 0x040000EE RID: 238
		public bool ExtractEmails;

		// Token: 0x040000EF RID: 239
		public int NumberOfResultsPerZipCode;

		// Token: 0x040000F0 RID: 240
		public int ExportType;

		// Token: 0x040000F1 RID: 241
		public int CSVDelimiter;

		// Token: 0x040000F2 RID: 242
		public int CSVEncoding;

		// Token: 0x040000F3 RID: 243
		public int ConnectionType;

		// Token: 0x040000F4 RID: 244
		public string ProxyConnector;

		// Token: 0x040000F5 RID: 245
		public int ProxyPort;

		// Token: 0x040000F6 RID: 246
		public bool ProxyAuthentification;

		// Token: 0x040000F7 RID: 247
		public string ProxyAuthLogin;

		// Token: 0x040000F8 RID: 248
		public int Numeric;

		// Token: 0x040000F9 RID: 249
		public string ProxyAuthPassword;

		// Token: 0x040000FA RID: 250
		public string[] ProxyList;

		// Token: 0x040000FB RID: 251
		public string[] ProxySourcesList;

		// Token: 0x040000FC RID: 252
		public bool IsRandomDelay;

		// Token: 0x040000FD RID: 253
		public int DelayFrom;

		// Token: 0x040000FE RID: 254
		public int DelayTo;

		// Token: 0x040000FF RID: 255
		public List<string> Categories;

		// Token: 0x04000100 RID: 256
		public List<TargetLocation> Locations;

		// Token: 0x04000101 RID: 257
		public List<ExtractionTask> Tasks;
	}
}
