using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace MasaLeadsExtractor
{
	// Token: 0x02000009 RID: 9
	public class DataRepository
	{
		// Token: 0x06000029 RID: 41 RVA: 0x00002ED0 File Offset: 0x000010D0
		public DataRepository(ConnectionSettings AppConfiguration)
		{
			string myConnectionString = string.Format("server={0};port={1};uid={2};pwd={3};database={4};Convert Zero Datetime=True", new object[] { AppConfiguration.MySqlServer, AppConfiguration.Port, AppConfiguration.User, AppConfiguration.Password, AppConfiguration.DataRepository });
			try
			{
				this.Connection = new MySqlConnection();
				this.Connection.ConnectionString = myConnectionString;
				this.Connection.Open();
			}
			catch (MySqlException ex)
			{
				MessageBox.Show(string.Format("DataRepository connection error!{0}{1}", Environment.NewLine, ex.Message));
			}
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002F80 File Offset: 0x00001180
		public long Insert(string Table, List<string> Values, bool ReturnLastInsertID)
		{
			MySqlCommand Command = this.Connection.CreateCommand();
			Command.CommandText = string.Format("INSERT INTO `{0}` VALUES(null ", Table);
			foreach (string Value in Values)
			{
				MySqlCommand mySqlCommand = Command;
				mySqlCommand.CommandText += string.Format(", \"{0}\"", Value);
			}
			MySqlCommand mySqlCommand2 = Command;
			mySqlCommand2.CommandText += ")";
			Command.ExecuteNonQuery();
			if (ReturnLastInsertID)
			{
				return Command.LastInsertedId;
			}
			return 0L;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x0000302C File Offset: 0x0000122C
		public void Insert(string Table, List<string> Values)
		{
			MySqlCommand Command = this.Connection.CreateCommand();
			Command.CommandText = string.Format("INSERT INTO `{0}` VALUES(", Table);
			foreach (string Value in Values)
			{
				MySqlCommand mySqlCommand = Command;
				mySqlCommand.CommandText += string.Format("\"{0}\",", Value);
			}
			Command.CommandText = Command.CommandText.Substring(0, Command.CommandText.Length - 1) + ")";
			try
			{
				Command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				File.AppendAllText("db_operations.log", string.Format("{0} {1}{2}", ex.Message, Command.CommandText, Environment.NewLine));
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00003114 File Offset: 0x00001314
		public List<object[]> Select(string Table, string Term)
		{
			List<object[]> Results = new List<object[]>();
			MySqlCommand mySqlCommand = this.Connection.CreateCommand();
			mySqlCommand.CommandText = string.Format("SELECT * FROM `{0}` WHERE {1}", Table, Term);
			MySqlDataReader Reader = mySqlCommand.ExecuteReader();
			while (Reader.Read())
			{
				try
				{
					object[] values = new object[Reader.FieldCount];
					Reader.GetValues(values);
					Results.Add(values);
				}
				catch
				{
				}
			}
			Reader.Close();
			return Results;
		}

		// Token: 0x0600002D RID: 45 RVA: 0x0000318C File Offset: 0x0000138C
		public List<object[]> Select(string Request)
		{
			List<object[]> Results = new List<object[]>();
			if (this.Connection.State != ConnectionState.Open)
			{
				this.Connection.Open();
			}
			MySqlCommand Command = this.Connection.CreateCommand();
			Command.CommandText = Request;
			MySqlDataReader Reader = null;
			try
			{
				Reader = Command.ExecuteReader();
			}
			catch
			{
			}
			if (Reader != null)
			{
				while (Reader.Read())
				{
					try
					{
						object[] values = new object[Reader.FieldCount];
						Reader.GetValues(values);
						Results.Add(values);
					}
					catch (Exception e)
					{
						File.AppendAllText(this.SQLlog, string.Format("{0}{1}", e.Message, Environment.NewLine));
					}
				}
				Reader.Close();
			}
			return Results;
		}

		// Token: 0x0600002E RID: 46 RVA: 0x0000324C File Offset: 0x0000144C
		public int IntScalarSelect(string Request)
		{
			MySqlCommand mySqlCommand = this.Connection.CreateCommand();
			mySqlCommand.CommandText = string.Format(Request, Array.Empty<object>());
			MySqlDataReader Reader = mySqlCommand.ExecuteReader();
			while (Reader.Read())
			{
				try
				{
					return Reader.GetInt32(0);
				}
				catch
				{
				}
			}
			Reader.Close();
			return 0;
		}

		// Token: 0x0600002F RID: 47 RVA: 0x000032AC File Offset: 0x000014AC
		public void DoRequest(string Request)
		{
			MySqlCommand Command = this.Connection.CreateCommand();
			Command.CommandText = string.Format(Request, Array.Empty<object>());
			try
			{
				Command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				File.AppendAllText("db_operations.log", string.Format("{0} {1}{2}", ex.Message, Command.CommandText, Environment.NewLine));
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00003318 File Offset: 0x00001518
		public long GetLastId()
		{
			MySqlCommand Command = this.Connection.CreateCommand();
			Command.CommandText = string.Format("SELECT LAST_INSERT_ID()", Array.Empty<object>());
			try
			{
				return (long)Command.ExecuteScalar();
			}
			catch (Exception ex)
			{
				File.AppendAllText("db_operations.log", string.Format("{0} {1}{2}", ex.Message, Command.CommandText, Environment.NewLine));
			}
			return 0L;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00003390 File Offset: 0x00001590
		public void Update(string Table, string[] Fields, string[] Values, string Term)
		{
			MySqlCommand Command = this.Connection.CreateCommand();
			string UpdateData = "";
			for (int i = 0; i < Values.Length; i++)
			{
				if (i == 0)
				{
					UpdateData += string.Format("`{0}`=\"{1}\"", Fields[i], Values[i]);
				}
				else
				{
					UpdateData += string.Format(", `{0}`=\"{1}\"", Fields[i], Values[i]);
				}
			}
			Command.CommandText = string.Format("UPDATE `{0}` SET {1} WHERE {2}", Table, UpdateData, Term);
			try
			{
				Command.ExecuteNonQuery();
			}
			catch (Exception ex)
			{
				File.AppendAllText("db_operations.log", string.Format("{0} {1}{2}", ex.Message, Command.CommandText, Environment.NewLine));
			}
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00003448 File Offset: 0x00001648
		public DataEntry GetObject(string Table, int Id)
		{
			DataEntry Obj = null;
			List<object[]> Items = this.Select(string.Format("SELECT Id,name FROM {0} WHERE Id={1}", Table, Id));
			if (Items.Count > 0)
			{
				Obj = new DataEntry
				{
					Id = Convert.ToInt32(Items[0][0]),
					Name = (string)Items[0][1]
				};
			}
			return Obj;
		}

		// Token: 0x04000012 RID: 18
		public MySqlConnection Connection;

		// Token: 0x04000013 RID: 19
		private string SQLlog = "SQLlog.txt";
	}
}
