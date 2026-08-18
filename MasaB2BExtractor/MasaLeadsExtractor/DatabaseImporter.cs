using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;

namespace MasaLeadsExtractor
{
	// Token: 0x02000012 RID: 18
	public static class DatabaseImporter
	{
		// Token: 0x06000057 RID: 87 RVA: 0x000059F4 File Offset: 0x00003BF4
		public static void ImportCountries()
		{
			string[] Data = File.ReadAllLines("geo\\world.sql");
			int Counter = 0;
			string ValuesBlock = "";
			int BlockCounter = 0;
			foreach (string Item in Data)
			{
				if (Item.Trim() != "")
				{
					string[] Values = Item.Replace("\\'", "`").Split(new char[] { ',' });
					ValuesBlock += string.Format("({0}, {1}, {2}, '{3}'),", new object[]
					{
						Values[0].Trim(),
						Values[1].Trim(),
						Values[2].Trim(),
						Values[5].Replace("'", "").Trim()
					});
					BlockCounter++;
					if (BlockCounter % 1000 == 0)
					{
						Startup.AppDatabase.DoRequest("INSERT INTO city VALUES " + ValuesBlock.Substring(0, ValuesBlock.Length - 1));
						BlockCounter = 0;
						ValuesBlock = "";
					}
				}
				Counter++;
				float num = 100f * (float)Counter / (float)Data.Length;
			}
			if (ValuesBlock != "")
			{
				Startup.AppDatabase.DoRequest("INSERT INTO city VALUES " + ValuesBlock.Substring(0, ValuesBlock.Length - 1));
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00005B44 File Offset: 0x00003D44
		public static void ImportZipCodes()
		{
			File.WriteAllText(DatabaseImporter.LogFileName, "Country,State,City,CityId,ZipCode" + Environment.NewLine);
			string[] Data = File.ReadAllLines("geo\\zip-code.csv");
			string ValuesBlock = "";
			for (int ItemIndex = 0; ItemIndex < Data.Length; ItemIndex++)
			{
				string[] Values = Data[ItemIndex].Replace("\"", "").Split(new char[] { ',' });
				if (Values.Length > 4 && Values[1] != "" && Values[2] != "" && Values[3] != "" && Values[4] != "")
				{
					List<object[]> City = LocationEditorDialog.AppDatabase.Select(string.Format("SELECT * FROM city AS t1 LEFT OUTER JOIN country AS t2 ON t1.country_id=t2.Id LEFT OUTER JOIN region AS t3 ON t1.region_id=t3.Id WHERE t1.name='{0}' AND t2.code='{1}' AND t3.code='{2}'", Values[3].Trim(), Values[1].ToLower().Trim(), Values[2].Trim()));
					if (City.Count > 0)
					{
						try
						{
							Startup.AppDatabase.DoRequest(string.Format("INSERT INTO zip_codes VALUES (null, '{0}', '{1}')", City[0][0], Values[4].Trim()));
							File.AppendAllText(DatabaseImporter.LogFileName, string.Format("{0},{1},{2},{3},{4}{5}", new object[]
							{
								Values[1],
								Values[2],
								Values[3],
								City[0][0],
								Values[4],
								Environment.NewLine
							}));
						}
						catch (Exception ex)
						{
							File.AppendAllText(DatabaseImporter.LogFileName, string.Format("{0},{1},{2},{3},{4},{5}{6}", new object[]
							{
								Values[1],
								Values[2],
								Values[3],
								City[0][0],
								Values[4],
								ex.Message,
								Environment.NewLine
							}));
						}
					}
				}
				float num = 100f * (float)ItemIndex / (float)Data.Length;
			}
			if (ValuesBlock != "")
			{
				Startup.AppDatabase.DoRequest("INSERT INTO city VALUES " + ValuesBlock.Substring(0, ValuesBlock.Length - 1));
			}
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00005D58 File Offset: 0x00003F58
		public static void ImportZipCodesFromFile()
		{
			string ZipCodesData = File.ReadAllText("geo\\allCountries.txt");
			List<string> Requests = new List<string>();
			List<object[]> Countries = LocationEditorDialog.AppDatabase.Select("SELECT * FROM `country` WHERE Id in (51, 62, 69, 100, 230)");
			int CountryCounter = 0;
			foreach (object[] Country in Countries)
			{
				CountryCounter++;
				List<object[]> CountryData = DatabaseImporter.GetCountryData(Country[0].ToString());
				int Cntr = 0;
				foreach (object[] item in CountryData)
				{
					string Template = string.Format("{0}\t(.*?)\t{1}\t{2}", item[2].ToString().ToUpper(), item[10].ToString().Replace(";", "").Replace(")", "")
						.Replace("(", ""), item[4].ToString().Replace(";", "").Replace(")", "")
						.Replace("(", ""));
					List<string[]> ZipData = WebRequester.ParseHTML(ZipCodesData, Template);
					if (ZipData.Count > 0)
					{
						foreach (string[] zData in ZipData)
						{
							Requests.Add(string.Format("INSERT INTO zip_codes VALUES ('{0}', '{1}')", item[7], zData[1]));
							if (Requests.Count >= 100)
							{
								DatabaseImporter.SaveData(ref Requests);
							}
						}
					}
					Cntr++;
					Console.WriteLine(string.Format("{0}/{1}... ({2:n2}%) - {3} {4}/{5}", new object[]
					{
						Cntr,
						CountryData.Count,
						100f * (float)Cntr / (float)CountryData.Count,
						Country[1],
						CountryCounter,
						Countries.Count
					}));
				}
				DatabaseImporter.SaveData(ref Requests);
				Console.WriteLine(string.Format("Done for {0}!", Country[1]));
			}
			Console.WriteLine("Absolutely all done!");
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00005FD4 File Offset: 0x000041D4
		public static List<object[]> GetCountryData(string Id)
		{
			return LocationEditorDialog.AppDatabase.Select(string.Format("SELECT * FROM `country` AS t1 RIGHT JOIN `region` AS t2 ON t2.country_id=t1.Id RIGHT JOIN `city` AS t3 ON t3.region_id=t2.Id WHERE t1.Id={0}", Id));
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00005FEC File Offset: 0x000041EC
		public static void SaveData(ref List<string> Requests)
		{
			string request = "";
			foreach (string rqst in Requests)
			{
				request += string.Format("{0};{1}", rqst, Environment.NewLine);
			}
			if (LocationEditorDialog.AppDatabase.Connection.State != ConnectionState.Open)
			{
				LocationEditorDialog.AppDatabase.Connection.Open();
			}
			MySqlCommand cmd = new MySqlCommand(request, LocationEditorDialog.AppDatabase.Connection);
			cmd.CommandTimeout = 600;
			try
			{
				cmd.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				File.AppendAllText(DatabaseImporter.LogFileName, string.Format("{0}{1}{2}{1}", ex.Message, Environment.NewLine, request));
			}
			Requests.Clear();
		}

		// Token: 0x04000040 RID: 64
		private static string LogFileName = "post-codes.log";
	}
}
