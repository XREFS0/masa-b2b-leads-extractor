using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml.Serialization;

// Token: 0x02000003 RID: 3
public class ConnectionSettings
{
	// Token: 0x06000005 RID: 5 RVA: 0x000021B8 File Offset: 0x000003B8
	public bool Save(string FName)
	{
		XmlSerializer writer = new XmlSerializer(typeof(ConnectionSettings));
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

	// Token: 0x06000006 RID: 6 RVA: 0x00002204 File Offset: 0x00000404
	public string GetXML()
	{
		XmlSerializer writer = new XmlSerializer(typeof(ConnectionSettings));
		string text;
		try
		{
			MemoryStream str = new MemoryStream();
			writer.Serialize(str, this);
			byte[] Bytes = str.GetBuffer();
			text = Encoding.UTF8.GetString(Bytes);
		}
		catch
		{
			text = "";
		}
		return text;
	}

	// Token: 0x06000007 RID: 7 RVA: 0x00002260 File Offset: 0x00000460
	public static ConnectionSettings Load(string FName)
	{
		XmlSerializer reader = new XmlSerializer(typeof(ConnectionSettings));
		ConnectionSettings dbsettings2;
		try
		{
			StreamReader file = new StreamReader(FName);
			ConnectionSettings dbsettings = (ConnectionSettings)reader.Deserialize(file);
			file.Close();
			dbsettings2 = dbsettings;
		}
		catch
		{
			dbsettings2 = new ConnectionSettings();
		}
		return dbsettings2;
	}

	// Token: 0x06000008 RID: 8 RVA: 0x000022B4 File Offset: 0x000004B4
	public static ConnectionSettings LoadAndDecript(string Key = "GBE_DB_FIXED_KEY_2026")
	{
		string EncString = "";
		try
		{
			EncString = File.ReadAllText(string.Format("{0}\\db32.dll", Application.StartupPath));
		}
		catch
		{
		}
		if (EncString != "")
		{
			string XML = SecurityHandler.Decrypt(EncString, Key, true);
			try
			{
				XmlSerializer xmlSerializer = new XmlSerializer(typeof(ConnectionSettings));
				MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(XML));
				return (ConnectionSettings)xmlSerializer.Deserialize(ms);
			}
			catch
			{
			}
		}
		return new ConnectionSettings();
	}

	// Token: 0x04000001 RID: 1
	public string MySqlServer;

	// Token: 0x04000002 RID: 2
	public string Port;

	// Token: 0x04000003 RID: 3
	public string User;

	// Token: 0x04000004 RID: 4
	public string Password;

	// Token: 0x04000005 RID: 5
	public string DataRepository;
}
