using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MasaLeadsExtractor
{
	// Token: 0x02000020 RID: 32
	public partial class LocationsImportDialog : Form
	{
		// Token: 0x060000F8 RID: 248 RVA: 0x00010AC0 File Offset: 0x0000ECC0
		public LocationsImportDialog()
		{
			this.InitializeComponent();
			DarkTheme.ApplyDarkTheme(this);
			this.Refresh();
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00010ADC File Offset: 0x0000ECDC
		private void btnLoadFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Text files|*.txt",
				InitialDirectory = Application.StartupPath
			};
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				this.tbUploadLocations.Text = File.ReadAllText(openFileDialog.FileName);
			}
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00010B24 File Offset: 0x0000ED24
		private void btnApply_Click(object sender, EventArgs e)
		{
			this.Ok = true;
			base.Close();
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00010B33 File Offset: 0x0000ED33
		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Ok = false;
			base.Close();
		}

		// Token: 0x04000140 RID: 320
		public bool Ok;
	}
}
