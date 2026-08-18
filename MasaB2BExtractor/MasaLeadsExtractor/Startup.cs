using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace MasaLeadsExtractor
{
	// Token: 0x0200001A RID: 26
	internal static class Startup
	{
		// Token: 0x060000C9 RID: 201 RVA: 0x0000D180 File Offset: 0x0000B380
		public static void RequestDelay()
		{
			if (Startup.AppSettings.IsRandomDelay)
			{
				Thread.Sleep((int)(2000.0 * (double)Startup.Rnd.Next(Startup.AppSettings.DelayFrom, Startup.AppSettings.DelayTo)));
			}
		}

		// Token: 0x060000CA RID: 202 RVA: 0x0000D1BD File Offset: 0x0000B3BD
		public static void Pause()
		{
			if (Startup.AppSettings.ProxyAuthentification)
			{
				Thread.Sleep(Startup.AppSettings.Numeric);
			}
		}

		// Token: 0x060000CB RID: 203 RVA: 0x0000D1DC File Offset: 0x0000B3DC
		[STAThread]
		private static void Main()
		{
			string thisprocessname = Process.GetCurrentProcess().ProcessName;
			if (Process.GetProcesses().Count<Process>((Process p) => p.ProcessName == thisprocessname) > 1)
			{
				MessageBox.Show("MASA B2B Leads Extractor is already running in another instance.");
				return;
			}
			string destFile = string.Format("AutoExport_{0}.txt", DateTime.Now.ToString("dd-MM-yyyy"));
			Startup.ExportFile = string.Format("{0}\\MASA B2B Leads Extractor\\" + destFile, Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			Startup.SettingsFileName = string.Format("{0}\\MASA B2B Leads Extractor\\settings.cfg", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			Startup.SettingDir = string.Format("{0}\\MASA B2B Leads Extractor", Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData));
			if (!Directory.Exists(Startup.SettingDir))
			{
				Directory.CreateDirectory(Startup.SettingDir);
			}
			if (!File.Exists(Startup.SettingsFileName))
			{
				Startup.SettingsFileName = string.Format("{0}\\settings.cfg", Startup.SettingDir);
			}
			Startup.Rnd = new Random(DateTime.Now.Millisecond);
			Startup.AppSettings = AppConfiguration.Load(Startup.SettingsFileName);
			Startup.LanguagesManager = new LocalizationManager();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainWindow());
		}

		// Token: 0x040000CB RID: 203
		public static string SettingsFileName;

		// Token: 0x040000CC RID: 204
		public static bool IsDemoVersion = false;

		// Token: 0x040000CD RID: 205
		public static string ExportFile;

		// Token: 0x040000CE RID: 206
		public static string SettingDir;

		// Token: 0x040000CF RID: 207
		public static Random Rnd;

		// Token: 0x040000D0 RID: 208
		public static AppConfiguration AppSettings;

		// Token: 0x040000D1 RID: 209
		public static ConnectionSettings DbConfig;

		// Token: 0x040000D2 RID: 210
		public static DataRepository AppDatabase;

		// Token: 0x040000D3 RID: 211
		public static LocalizationManager LanguagesManager;

		// Token: 0x040000D4 RID: 212
		public static string[] LanguagesFiles = new string[] { "languages\\lang-en.txt", "languages\\lang-it.txt", "languages\\lang-ge.txt", "languages\\lang-fr.txt", "languages\\lang-sp.txt" };

		// Token: 0x040000D5 RID: 213
		public static bool StopDataCollection = false;

		// Token: 0x040000D6 RID: 214
		public static bool IsDemoLimit = false;
	}
}
