using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace MasaLeadsExtractor
{
	// Token: 0x02000013 RID: 19
	public partial class TextInputDialog : Form
	{
		// Token: 0x0600005D RID: 93 RVA: 0x000060DC File Offset: 0x000042DC
		public TextInputDialog(string Value, string Title, string Prompt)
		{
			this.InitializeComponent();
			DarkTheme.ApplyDarkTheme(this);
			this.lblPrompt.Text = Prompt;
			this.Text = Title;
			this.tbValue.Text = Value;
		}

		// Token: 0x0600005E RID: 94 RVA: 0x00006110 File Offset: 0x00004310
		private void btnApply_Click(object sender, EventArgs e)
		{
			this.OkPressed = true;
			this.Value = this.tbValue.Text;
			string[] w = this.tbValue.Text.Trim().Split(new char[] { ' ' });
			if (w.Length <= 2)
			{
				base.Close();
				return;
			}
			MessageBox.Show("Please use keywords not longer than two words!");
			this.tbValue.Text = string.Format("{0} {1}", w[0], w[1]);
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00006189 File Offset: 0x00004389
		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.OkPressed = false;
			this.Value = "";
			base.Close();
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000061A3 File Offset: 0x000043A3
		private void tbValue_KeyUp(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.Return)
			{
				this.btnApply_Click(null, null);
			}
		}

		// Token: 0x04000041 RID: 65
		public bool OkPressed;

		// Token: 0x04000042 RID: 66
		public string Value;
	}
}
