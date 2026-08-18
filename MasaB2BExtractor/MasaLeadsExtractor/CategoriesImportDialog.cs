using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace MasaLeadsExtractor
{
	// Token: 0x0200001F RID: 31
	public partial class CategoriesImportDialog : Form
	{
		// Token: 0x060000F2 RID: 242 RVA: 0x000105C2 File Offset: 0x0000E7C2
		public CategoriesImportDialog()
		{
			this.InitializeComponent();
			DarkTheme.ApplyDarkTheme(this);
			this.Refresh();
		}

		// Token: 0x060000F3 RID: 243 RVA: 0x000105DC File Offset: 0x0000E7DC
		private void btnApply_Click(object sender, EventArgs e)
		{
			this.UseThem = true;
			base.Close();
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x000105EB File Offset: 0x0000E7EB
		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.UseThem = false;
			base.Close();
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x000105FC File Offset: 0x0000E7FC
		private void btnLoadFile_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Text files|*.txt",
				InitialDirectory = Application.StartupPath
			};
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				this.tbUploadCategories.Text = File.ReadAllText(openFileDialog.FileName);
			}
		}

		// Token: 0x04000138 RID: 312
		public bool UseThem;
	}
}
