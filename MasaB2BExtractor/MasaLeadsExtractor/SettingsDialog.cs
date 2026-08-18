using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace MasaLeadsExtractor
{
	// Token: 0x0200001D RID: 29
	public partial class SettingsDialog : KryptonForm
	{
		// Token: 0x060000DD RID: 221 RVA: 0x0000E0B8 File Offset: 0x0000C2B8
		public SettingsDialog()
		{
			this.InitializeComponent();
			DarkTheme.ApplyDarkTheme(this);
			this.IsInitSettings = true;
			foreach (string ColumnName in this.ColumnNames)
			{
				this.cblExport.Items.Add(ColumnName);
			}
			Startup.LanguagesManager.InitControl(this, base.Controls);
		}

		// Token: 0x060000DE RID: 222 RVA: 0x0000E1A8 File Offset: 0x0000C3A8
		private void btnOk_Click(object sender, EventArgs e)
		{
			AppConfiguration AppSettings = AppConfiguration.Load(Startup.SettingsFileName);
			int.TryParse(this.tbNumberOfResultsPerZipCode.Text, out AppSettings.NumberOfResultsPerZipCode);
			AppSettings.Language = this.cbLanguage.SelectedIndex;
			for (int i = 0; i < this.cblExport.Items.Count; i++)
			{
				AppSettings.ColumnsToExport[i] = this.cblExport.GetItemChecked(i);
			}
			AppSettings.ExtractEmails = this.cbExtractEmails.Checked;
			AppSettings.AutoExport = this.rbAutoExport.Checked;
			AppSettings.AutoExportPath = this.tbExportPath.Text;
			AppSettings.ExportType = this.cbExportType.SelectedIndex;
			AppSettings.CSVDelimiter = this.cbCSVDelimiter.SelectedIndex;
			AppSettings.CSVEncoding = this.cbCSVEncoding.SelectedIndex;
			if (this.rbNoProxy.Checked)
			{
				AppSettings.ConnectionType = 0;
			}
			else if (this.rbUseSingleProxy.Checked)
			{
				AppSettings.ConnectionType = 1;
			}
			else if (this.rbRundomProxyList.Checked)
			{
				AppSettings.ConnectionType = 2;
			}
			else if (this.rbFreeProxiesList.Checked)
			{
				AppSettings.ConnectionType = 3;
			}
			else if (this.rbUseVPN.Checked)
			{
				AppSettings.ConnectionType = 4;
			}
			AppSettings.IsRandomDelay = this.cbRandomDelay.Checked;
			AppSettings.DelayFrom = this.tbDelayFrom.Value;
			AppSettings.DelayTo = this.tbDelayTo.Value;
			AppSettings.ProxyConnector = this.tbProxyServerIP.Text;
			int.TryParse(this.tbProxyServerIP.Text, out AppSettings.ProxyPort);
			AppSettings.ProxyAuthentification = this.cbAuthentification.Checked;
			AppSettings.ProxyAuthLogin = this.tbProxyAuthUsername.Text;
			AppSettings.Numeric = (int)this.numericUpDown1.Value;
			AppSettings.ProxyAuthPassword = this.tbProxyAuthPassword.Text;
			AppSettings.ProxyList = this.tbRandomProxyList.Text.Split(new char[] { '\r' });
			AppSettings.ProxySourcesList = this.tbFreeProxiesList.Text.Split(new char[] { '\r' });
			AppSettings.Save(Startup.SettingsFileName);
			base.Close();
		}

		// Token: 0x060000DF RID: 223 RVA: 0x0000E3E4 File Offset: 0x0000C5E4
		private void SettingsDialog_Shown(object sender, EventArgs e)
		{
			AppConfiguration AppSettings = AppConfiguration.Load(Startup.SettingsFileName);
			this.tbNumberOfResultsPerZipCode.Text = AppSettings.NumberOfResultsPerZipCode.ToString();
			this.cbLanguage.SelectedIndex = AppSettings.Language;
			for (int i = 0; i < this.cblExport.Items.Count; i++)
			{
				if (AppSettings.ColumnsToExport[i])
				{
					this.cblExport.SetItemChecked(i, true);
				}
			}
			this.tbExportPath.Text = AppSettings.AutoExportPath;
			if (AppSettings.AutoExport)
			{
				this.rbAutoExport.Checked = true;
				this.tbExportPath.Enabled = true;
			}
			else
			{
				this.tbExportPath.Enabled = false;
				this.rbManualExport.Checked = true;
			}
			this.cbExtractEmails.Checked = AppSettings.ExtractEmails;
			this.cbExportType.SelectedIndex = AppSettings.ExportType;
			this.cbCSVDelimiter.SelectedIndex = AppSettings.CSVDelimiter;
			this.cbCSVEncoding.SelectedIndex = AppSettings.CSVEncoding;
			this.tbDelayFrom.Value = AppSettings.DelayFrom;
			this.tbDelayTo.Value = AppSettings.DelayTo;
			this.tbDelayFrom.Enabled = AppSettings.IsRandomDelay;
			this.tbDelayTo.Enabled = AppSettings.IsRandomDelay;
			this.cbRandomDelay.Checked = AppSettings.IsRandomDelay;
			this.tbProxyServerIP.Text = AppSettings.ProxyConnector;
			this.tbProxyServerPort.Text = AppSettings.ProxyPort.ToString();
			if (AppSettings.ProxyList != null)
			{
				foreach (string p in AppSettings.ProxyList)
				{
					TextBox textBox = this.tbRandomProxyList;
					textBox.Text += string.Format("{0}{1}", p, Environment.NewLine);
				}
			}
			if (AppSettings.ProxySourcesList != null)
			{
				foreach (string p2 in AppSettings.ProxySourcesList)
				{
					TextBox textBox2 = this.tbFreeProxiesList;
					textBox2.Text += string.Format("{0}{1}", p2, Environment.NewLine);
				}
			}
			switch (AppSettings.ConnectionType)
			{
			case 0:
				this.rbNoProxy.Checked = true;
				break;
			case 1:
				this.rbUseSingleProxy.Checked = true;
				this.tbProxyServerIP.Enabled = true;
				this.tbProxyServerPort.Enabled = true;
				break;
			case 2:
				this.rbRundomProxyList.Checked = true;
				this.tbRandomProxyList.Enabled = true;
				break;
			case 3:
				this.rbFreeProxiesList.Checked = true;
				this.tbFreeProxiesList.Enabled = true;
				break;
			case 4:
				this.rbUseVPN.Checked = true;
				break;
			}
			this.cbAuthentification.Checked = AppSettings.ProxyAuthentification;
			this.tbProxyAuthUsername.Enabled = AppSettings.ProxyAuthentification;
			this.tbProxyAuthPassword.Enabled = AppSettings.ProxyAuthentification;
			this.IsInitSettings = false;
		}

		// Token: 0x060000E0 RID: 224 RVA: 0x0000D57B File Offset: 0x0000B77B
		private void btnCancel_Click(object sender, EventArgs e)
		{
			base.Close();
		}

		// Token: 0x060000E1 RID: 225 RVA: 0x0000E6C4 File Offset: 0x0000C8C4
		private void rbNoProxy_CheckedChanged(object sender, EventArgs e)
		{
			this.tbProxyServerIP.Enabled = false;
			this.tbProxyServerPort.Enabled = false;
			this.tbProxyAuthPassword.Enabled = true;
			this.tbProxyAuthUsername.Enabled = true;
			this.cbAuthentification.Enabled = true;
			this.tbRandomProxyList.Enabled = false;
			this.tbFreeProxiesList.Enabled = false;
		}

		// Token: 0x060000E2 RID: 226 RVA: 0x0000E728 File Offset: 0x0000C928
		private void rbUseSingleProxy_CheckedChanged(object sender, EventArgs e)
		{
			this.tbProxyServerIP.Enabled = true;
			this.tbProxyServerPort.Enabled = true;
			this.tbProxyAuthPassword.Enabled = true;
			this.tbProxyAuthUsername.Enabled = true;
			this.cbAuthentification.Enabled = true;
			this.tbRandomProxyList.Enabled = false;
			this.tbFreeProxiesList.Enabled = false;
		}

		// Token: 0x060000E3 RID: 227 RVA: 0x0000E78C File Offset: 0x0000C98C
		private void rbRundomProxyList_CheckedChanged(object sender, EventArgs e)
		{
			this.tbProxyServerIP.Enabled = false;
			this.tbProxyServerPort.Enabled = false;
			this.tbProxyAuthPassword.Enabled = true;
			this.tbProxyAuthUsername.Enabled = true;
			this.cbAuthentification.Enabled = true;
			this.tbRandomProxyList.Enabled = true;
			this.tbFreeProxiesList.Enabled = false;
		}

		// Token: 0x060000E4 RID: 228 RVA: 0x0000E7F0 File Offset: 0x0000C9F0
		private void rbFreeProxiesList_CheckedChanged(object sender, EventArgs e)
		{
			this.tbProxyServerIP.Enabled = false;
			this.tbProxyServerPort.Enabled = false;
			this.tbProxyAuthPassword.Enabled = true;
			this.tbProxyAuthUsername.Enabled = true;
			this.cbAuthentification.Enabled = true;
			this.tbRandomProxyList.Enabled = false;
			this.tbFreeProxiesList.Enabled = true;
		}

		// Token: 0x060000E5 RID: 229 RVA: 0x0000E851 File Offset: 0x0000CA51
		private void rbUseVPN_CheckedChanged(object sender, EventArgs e)
		{
		}

		// Token: 0x060000E6 RID: 230 RVA: 0x0000E873 File Offset: 0x0000CA73
		private void cbRandomDelay_CheckedChanged(object sender, EventArgs e)
		{
			this.tbDelayFrom.Enabled = this.cbRandomDelay.Checked;
			this.tbDelayTo.Enabled = this.cbRandomDelay.Checked;
		}

		// Token: 0x060000E7 RID: 231 RVA: 0x0000E8A4 File Offset: 0x0000CAA4
		private void tbDelayFrom_ValueChanged(object sender, EventArgs e)
		{
			if (this.tbDelayTo.Value < this.tbDelayFrom.Value && this.tbDelayFrom.Value + 1 <= this.tbDelayTo.Maximum)
			{
				this.tbDelayTo.Value = this.tbDelayFrom.Value + 1;
			}
		}

		// Token: 0x060000E8 RID: 232 RVA: 0x0000E8FC File Offset: 0x0000CAFC
		private void tbDelayTo_ValueChanged(object sender, EventArgs e)
		{
			if (this.tbDelayTo.Value < this.tbDelayFrom.Value && this.tbDelayTo.Value - 1 >= this.tbDelayFrom.Minimum)
			{
				this.tbDelayFrom.Value = this.tbDelayTo.Value - 1;
			}
		}

		// Token: 0x060000E9 RID: 233 RVA: 0x0000E953 File Offset: 0x0000CB53
		private void cbAuthentification_CheckedChanged(object sender, EventArgs e)
		{
			this.tbProxyAuthPassword.Enabled = this.cbAuthentification.Checked;
			this.tbProxyAuthUsername.Enabled = this.cbAuthentification.Checked;
		}

		// Token: 0x060000EA RID: 234 RVA: 0x0000E981 File Offset: 0x0000CB81
		private void rbAutoExport_CheckedChanged(object sender, EventArgs e)
		{
			this.tbExportPath.Enabled = this.rbAutoExport.Checked;
			this.btnChooseExportFolder.Enabled = this.rbAutoExport.Checked;
		}

		// Token: 0x060000EB RID: 235 RVA: 0x0000E9B0 File Offset: 0x0000CBB0
		private void btnChooseExportFolder_Click(object sender, EventArgs e)
		{
			FolderBrowserDialog fbd = new FolderBrowserDialog();
			fbd.SelectedPath = Application.StartupPath;
			if (fbd.ShowDialog() == DialogResult.OK)
			{
				this.tbExportPath.Text = fbd.SelectedPath;
			}
		}

		// Token: 0x060000EC RID: 236 RVA: 0x0000E9E8 File Offset: 0x0000CBE8
		private void cbExportType_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.cbCSVDelimiter.Enabled = this.cbExportType.SelectedIndex == 0;
			this.cbCSVEncoding.Enabled = this.cbExportType.SelectedIndex == 0;
		}

		// Token: 0x060000ED RID: 237 RVA: 0x00007925 File Offset: 0x00005B25
		private void cblExport_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		// Token: 0x060000EE RID: 238 RVA: 0x0000EA1C File Offset: 0x0000CC1C
		private void button1_Click(object sender, EventArgs e)
		{
			Process.Start("explorer.exe", Startup.SettingDir);
		}

		// Token: 0x04000102 RID: 258
		private bool IsInitSettings;

		// Token: 0x04000103 RID: 259
		private string[] ColumnNames = new string[]
		{
			"Category", "Real Category", "Business Name", "Full Address", "City", "State", "Postal Code", "Country", "Phone", "Email",
			"Website", "Latitude", "Longitude", "Map Link", "Details Link"
		};
	}
}
