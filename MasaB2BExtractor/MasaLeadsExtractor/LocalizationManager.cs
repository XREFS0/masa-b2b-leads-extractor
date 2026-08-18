using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace MasaLeadsExtractor
{
	// Token: 0x02000014 RID: 20
	public class LocalizationManager
	{
		// Token: 0x06000063 RID: 99 RVA: 0x0000650C File Offset: 0x0000470C
		public void InitFields(string FileName)
		{
			string[] Lines = File.ReadAllLines(FileName);
			this.ExitMessage = Lines[0].Split(new char[] { '*' })[1];
			this.NoFreeProxiesMessage = Lines[1].Split(new char[] { '*' })[1];
			this.DeleteSomeRows = Lines[2].Split(new char[] { '*' })[1];
			this.DeleteAllRows = Lines[3].Split(new char[] { '*' })[1];
			this.TotalProxiesMessage = Lines[4].Split(new char[] { '*' })[1];
			this.WrongCodeMessage = Lines[5].Split(new char[] { '*' })[1];
			this.FullVersionMessage = Lines[6].Split(new char[] { '*' })[1];
			this.MakeSearchFirst = Lines[7].Split(new char[] { '*' })[1];
			this.NoDataToExport = Lines[8].Split(new char[] { '*' })[1];
			this.NoDataSelectedToExport = Lines[9].Split(new char[] { '*' })[1];
			this.WorkIsDone = Lines[10].Split(new char[] { '*' })[1];
			this.StoppedByUser = Lines[11].Split(new char[] { '*' })[1];
			this.FieldsData = new List<string[]>();
			for (int i = this.NbrStaticMessages; i < Lines.Length; i++)
			{
				this.FieldsData.Add(Lines[i].Split(new char[] { '*' }));
			}
		}

		// Token: 0x06000064 RID: 100 RVA: 0x000066A4 File Offset: 0x000048A4
		public void InitControl(Form Form, Control.ControlCollection Controls)
		{
			foreach (object obj in Controls)
			{
				Control Ctrl = (Control)obj;
				this.SetControlText(Form.Name, Ctrl);
				this.InitControl(Form, Ctrl.Controls);
			}
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000670C File Offset: 0x0000490C
		private void SetControlText(string FormName, Control Ctrl)
		{
			for (int i = 0; i < this.FieldsData.Count; i++)
			{
				if (this.FieldsData[i][0] == FormName && this.FieldsData[i][1] == Ctrl.Name)
				{
					try
					{
						Ctrl.Text = this.FieldsData[i][2];
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x06000066 RID: 102 RVA: 0x0000678C File Offset: 0x0000498C
		public void InitMenu(MainWindow mf)
		{
			foreach (object obj in mf.menuStrip.Items)
			{
				ToolStripItem Item = (ToolStripItem)obj;
				foreach (ToolStripItem tsi in this.GetAllChildren(Item))
				{
					for (int i = 0; i < this.FieldsData.Count; i++)
					{
						if (this.FieldsData[i][0] == "Menu" && tsi.Name == this.FieldsData[i][1])
						{
							tsi.Text = this.FieldsData[i][2];
						}
					}
				}
			}
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00006870 File Offset: 0x00004A70
		public void InitTableColumns(DataGridView dgv)
		{
			foreach (object obj in dgv.Columns)
			{
				DataGridViewColumn col = (DataGridViewColumn)obj;
				for (int i = 0; i < this.FieldsData.Count; i++)
				{
					if (this.FieldsData[i][0] == "DataGridView" && col.Name == this.FieldsData[i][1])
					{
						col.HeaderText = this.FieldsData[i][2];
					}
				}
			}
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00006924 File Offset: 0x00004B24
		public void ExportFields(string ExportFileName)
		{
			File.WriteAllText(ExportFileName, "");
			File.AppendAllText(ExportFileName, string.Format("Messages*{0}*{1}", this.ExitMessage, Environment.NewLine));
			File.AppendAllText(ExportFileName, string.Format("Messages*{0}*{1}", this.NoFreeProxiesMessage, Environment.NewLine));
			File.AppendAllText(ExportFileName, string.Format("Messages*{0}*{1}", this.DeleteSomeRows, Environment.NewLine));
			File.AppendAllText(ExportFileName, string.Format("Messages*{0}*{1}", this.DeleteAllRows, Environment.NewLine));
			File.AppendAllText(ExportFileName, string.Format("Messages*{0}*{1}", this.TotalProxiesMessage, Environment.NewLine));
			File.AppendAllText(ExportFileName, string.Format("Messages*{0}*{1}", this.WrongCodeMessage, Environment.NewLine));
			File.AppendAllText(ExportFileName, string.Format("Messages*{0}*{1}", this.FullVersionMessage, Environment.NewLine));
			File.AppendAllText(ExportFileName, string.Format("Messages*{0}*{1}", this.MakeSearchFirst, Environment.NewLine));
			File.AppendAllText(ExportFileName, string.Format("Messages*{0}*{1}", this.NoDataToExport, Environment.NewLine));
			File.AppendAllText(ExportFileName, string.Format("Messages*{0}*{1}", this.NoDataSelectedToExport, Environment.NewLine));
			File.AppendAllText(ExportFileName, string.Format("Messages*{0}*{1}", this.WorkIsDone, Environment.NewLine));
			File.AppendAllText(ExportFileName, string.Format("Messages*{0}*{1}", this.StoppedByUser, Environment.NewLine));
			MainWindow mf = new MainWindow();
			this.SaveControls(ExportFileName, "MainWindow", mf.Controls);
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00006A98 File Offset: 0x00004C98
		public void SaveControls(string ExportFileName, string FormName, Control.ControlCollection Controls)
		{
			foreach (object obj in Controls)
			{
				Control Ctrl = (Control)obj;
				File.AppendAllText(ExportFileName, string.Format("{0}*{1}*{2}{3}", new object[]
				{
					FormName,
					Ctrl.Name,
					Ctrl.Text,
					Environment.NewLine
				}));
				this.SaveControls(ExportFileName, FormName, Ctrl.Controls);
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00006B28 File Offset: 0x00004D28
		private ToolStripItem[] GetAllChildren(ToolStripItem item)
		{
			List<ToolStripItem> Items = new List<ToolStripItem> { item };
			if (item is ToolStripMenuItem)
			{
				{
					IEnumerator enumerator = ((ToolStripMenuItem)item).DropDownItems.GetEnumerator();
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						ToolStripItem i = (ToolStripItem)obj;
						Items.AddRange(this.GetAllChildren(i));
					}
					goto IL_0101;
				}
			}
			if (item is ToolStripSplitButton)
			{
				{
					IEnumerator enumerator = ((ToolStripSplitButton)item).DropDownItems.GetEnumerator();
					while (enumerator.MoveNext())
					{
						object obj2 = enumerator.Current;
						ToolStripItem j = (ToolStripItem)obj2;
						Items.AddRange(this.GetAllChildren(j));
					}
					goto IL_0101;
				}
			}
			if (item is ToolStripDropDownButton)
			{
				foreach (object obj3 in ((ToolStripDropDownButton)item).DropDownItems)
				{
					ToolStripItem k = (ToolStripItem)obj3;
					Items.AddRange(this.GetAllChildren(k));
				}
			}
			IL_0101:
			return Items.ToArray();
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00006C64 File Offset: 0x00004E64
		public void SaveMenuItems(string ExportFileName, ToolStripItemCollection ItemsCollection)
		{
			foreach (object obj in ItemsCollection)
			{
				ToolStripItem Item = (ToolStripItem)obj;
				foreach (ToolStripItem tsi in this.GetAllChildren(Item))
				{
					File.AppendAllText(ExportFileName, string.Format("{0}*{1}*{2}{3}", new object[]
					{
						"Menu",
						tsi.Name,
						tsi.Text,
						Environment.NewLine
					}));
				}
			}
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00006D0C File Offset: 0x00004F0C
		public void SaveTableColumns(string ExportFileName, DataGridView dgv)
		{
			foreach (object obj in dgv.Columns)
			{
				DataGridViewColumn col = (DataGridViewColumn)obj;
				File.AppendAllText(ExportFileName, string.Format("{0}*{1}*{2}{3}", new object[]
				{
					"DataGridView",
					col.Name,
					col.HeaderText,
					Environment.NewLine
				}));
			}
		}

		// Token: 0x04000048 RID: 72
		public string ExitMessage = "Do you really want to exit?";

		// Token: 0x04000049 RID: 73
		public string NoFreeProxiesMessage = "No one working free proxy server available!";

		// Token: 0x0400004A RID: 74
		public string DeleteSomeRows = "Do you really want to delete {0} rows from list?";

		// Token: 0x0400004B RID: 75
		public string DeleteAllRows = "Do you really want to delete all rows from list?";

		// Token: 0x0400004C RID: 76
		public string TotalProxiesMessage = "Total proxies {0}, checked {1}, available {2}";

		// Token: 0x0400004D RID: 77
		public string WrongCodeMessage = "Wrong code or email. Please try again!";

		// Token: 0x0400004E RID: 78
		public string FullVersionMessage = "Upgrade to PRO version to remove trial limitations! Do you want to buy the full version now?";

		// Token: 0x0400004F RID: 79
		public string MakeSearchFirst = "Please make first the search you want and click on GET DATA only after the results appears in the page";

		// Token: 0x04000050 RID: 80
		public string NoDataToExport = "No data to export. Please make first the search and then click on GET DATA";

		// Token: 0x04000051 RID: 81
		public string NoDataSelectedToExport = "No data is selected to export. Please make first the selection and then click on Export";

		// Token: 0x04000052 RID: 82
		public string WorkIsDone = "Processing is done!";

		// Token: 0x04000053 RID: 83
		public string StoppedByUser = "Stopped by user!";

		// Token: 0x04000054 RID: 84
		public string SelectCategory = "Please select category!";

		// Token: 0x04000055 RID: 85
		public string DeleteCategory = "Do you really want to delete '{0}'?";

		// Token: 0x04000056 RID: 86
		private int NbrStaticMessages = 12;

		// Token: 0x04000057 RID: 87
		public List<string[]> FieldsData;
	}
}
