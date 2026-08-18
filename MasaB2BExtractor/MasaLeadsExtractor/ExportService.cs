using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace MasaLeadsExtractor
{
	// Token: 0x0200000D RID: 13
	public static class ExportService
	{
		// Token: 0x0600003A RID: 58 RVA: 0x00003A90 File Offset: 0x00001C90
		public static void InitHeader(DataGridView dgv)
		{
			ExportService.Columns = new List<string>();
			for (int i = 0; i < dgv.Columns.Count - 4; i++)
			{
				ExportService.Columns.Add(dgv.Columns[i].HeaderText);
			}
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00003ADC File Offset: 0x00001CDC
		public static string BuildCSVLine(AppConfiguration AppSettings, DataGridView dgv, int RowIndex)
		{
			string Line = "";
			for (int i = 0; i < dgv.Columns.Count - 4; i++)
			{
				string Value = "";
				try
				{
					Value = dgv.Rows[RowIndex].Cells[i].Value.ToString();
				}
				catch
				{
				}
				if (AppSettings.ColumnsToExport[i])
				{
					Line += string.Format("\"{0}\"{1}", Value, ExportService.CSVDelimiters[AppSettings.CSVDelimiter]);
				}
			}
			if (Line.Length > 0)
			{
				Line = Line.Substring(0, Line.Length - 1) + Environment.NewLine;
			}
			return Line;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00003B90 File Offset: 0x00001D90
		private static string GetRowKey(AppConfiguration AppSettings, DataGridView dgv, int RowIndex)
		{
			string key = "";
			for (int i = 0; i < dgv.Columns.Count - 4; i++)
			{
				if (AppSettings.ColumnsToExport[i])
				{
					string Value = "";
					try
					{
						Value = dgv.Rows[RowIndex].Cells[i].Value.ToString();
					}
					catch
					{
					}
					key = key + Value + "|";
				}
			}
			return key;
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00003C10 File Offset: 0x00001E10
		private static void ShowExportSummaryPopup(int totalRowsProcessed, int uniqueRowsExported)
		{
			MessageBox.Show("Export Done and duplicated removed!\n\n" + string.Format("Tot Processed lines: {0}\n", totalRowsProcessed) + string.Format("Unique records exported: {0}", uniqueRowsExported), "Export detail", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00003C4C File Offset: 0x00001E4C
		public static void SaveToCSV(AppConfiguration AppSettings, string FileName, DataGridView dgv, bool removeDuplicates = true)
		{
			if (dgv.Columns.Count < 4)
			{
				MessageBox.Show("DataGridView must have at least 4 columns", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			ExportService.InitHeader(dgv);
			Encoding FileEncoding = Encoding.UTF8;
			if (AppSettings.CSVEncoding == 0)
			{
				FileEncoding = Encoding.ASCII;
			}
			else if (AppSettings.CSVEncoding == 1)
			{
				FileEncoding = Encoding.UTF7;
			}
			else if (AppSettings.CSVEncoding == 2)
			{
				FileEncoding = Encoding.UTF8;
			}
			File.WriteAllText(FileName, "", FileEncoding);
			string Line = "";
			for (int i = 0; i < dgv.Columns.Count - 4; i++)
			{
				if (AppSettings.ColumnsToExport[i])
				{
					Line += string.Format("{0}{1}", ExportService.Columns[i], ExportService.CSVDelimiters[AppSettings.CSVDelimiter]);
				}
			}
			if (Line.Length > 0)
			{
				Line = Line.Substring(0, Line.Length - 1) + Environment.NewLine;
			}
			File.AppendAllText(FileName, Line, FileEncoding);
			int chunkSize = 1000;
			int totalRows = ((dgv.SelectedRows.Count > 0) ? dgv.SelectedRows.Count : dgv.Rows.Count);
			int totalProcessed = 0;
			int uniqueExported = 0;
			HashSet<string> uniqueRows = (removeDuplicates ? new HashSet<string>() : null);
			for (int chunkStart = 0; chunkStart < totalRows; chunkStart += chunkSize)
			{
				StringBuilder chunkBuilder = new StringBuilder();
				int chunkEnd = Math.Min(chunkStart + chunkSize, totalRows);
				if (dgv.SelectedRows.Count > 0)
				{
					for (int j = chunkStart; j < chunkEnd; j++)
					{
						int rowIndex = dgv.SelectedRows[dgv.SelectedRows.Count - 1 - j].Index;
						string rowKey = (removeDuplicates ? ExportService.GetRowKey(AppSettings, dgv, rowIndex) : "");
						string csvLine = ExportService.BuildCSVLine(AppSettings, dgv, rowIndex);
						if (!removeDuplicates || (removeDuplicates && !uniqueRows.Contains(rowKey)))
						{
							if (removeDuplicates)
							{
								uniqueRows.Add(rowKey);
							}
							chunkBuilder.Append(csvLine);
							uniqueExported++;
						}
						totalProcessed++;
					}
				}
				else
				{
					for (int k = chunkStart; k < chunkEnd; k++)
					{
						string rowKey2 = (removeDuplicates ? ExportService.GetRowKey(AppSettings, dgv, k) : "");
						string csvLine2 = ExportService.BuildCSVLine(AppSettings, dgv, k);
						if (!removeDuplicates || (removeDuplicates && !uniqueRows.Contains(rowKey2)))
						{
							if (removeDuplicates)
							{
								uniqueRows.Add(rowKey2);
							}
							chunkBuilder.Append(csvLine2);
							uniqueExported++;
						}
						totalProcessed++;
					}
				}
				File.AppendAllText(FileName, chunkBuilder.ToString(), FileEncoding);
				if (chunkStart % (chunkSize * 10) == 0)
				{
					GC.Collect();
					GC.WaitForPendingFinalizers();
				}
			}
			ExportService.ShowExportSummaryPopup(totalProcessed, uniqueExported);
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00003EDC File Offset: 0x000020DC
		public static void SaveToXLS(AppConfiguration AppSettings, string FileName, DataGridView dgv, bool removeDuplicates = true)
		{
			if (dgv.Columns.Count < 4)
			{
				MessageBox.Show("DataGridView should have at least 4 columns", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			ExportService.InitHeader(dgv);
			SpreadsheetDocument doc = new SpreadsheetDocument();
			doc.Create();
			int ColIndex = 0;
			for (int i = 0; i < dgv.Columns.Count - 4; i++)
			{
				if (AppSettings.ColumnsToExport[i])
				{
					doc.SetCellValue(0, ColIndex, ExportService.Columns[i]);
					ColIndex++;
				}
			}
			int chunkSize = 1000;
			int totalRows = ((dgv.SelectedRows.Count > 0) ? dgv.SelectedRows.Count : dgv.Rows.Count);
			int totalProcessed = 0;
			int uniqueExported = 0;
			int excelRowIndex = 1;
			HashSet<string> uniqueRows = (removeDuplicates ? new HashSet<string>() : null);
			for (int chunkStart = 0; chunkStart < totalRows; chunkStart += chunkSize)
			{
				int chunkEnd = Math.Min(chunkStart + chunkSize, totalRows);
				if (dgv.SelectedRows.Count > 0)
				{
					for (int j = chunkStart; j < chunkEnd; j++)
					{
						int rowIndex = dgv.SelectedRows[dgv.SelectedRows.Count - 1 - j].Index;
						string rowKey = (removeDuplicates ? ExportService.GetRowKey(AppSettings, dgv, rowIndex) : "");
						if (!removeDuplicates || (removeDuplicates && !uniqueRows.Contains(rowKey)))
						{
							if (removeDuplicates)
							{
								uniqueRows.Add(rowKey);
							}
							ColIndex = 0;
							for (int k = 0; k < dgv.Columns.Count - 4; k++)
							{
								if (AppSettings.ColumnsToExport[k])
								{
									string Value = "";
									try
									{
										Value = dgv.Rows[rowIndex].Cells[k].Value.ToString();
									}
									catch
									{
									}
									doc.SetCellValue(excelRowIndex, ColIndex, Value);
									ColIndex++;
								}
							}
							excelRowIndex++;
							uniqueExported++;
						}
						totalProcessed++;
					}
				}
				else
				{
					for (int l = chunkStart; l < chunkEnd; l++)
					{
						string rowKey2 = (removeDuplicates ? ExportService.GetRowKey(AppSettings, dgv, l) : "");
						if (!removeDuplicates || (removeDuplicates && !uniqueRows.Contains(rowKey2)))
						{
							if (removeDuplicates)
							{
								uniqueRows.Add(rowKey2);
							}
							ColIndex = 0;
							for (int m = 0; m < dgv.Columns.Count - 4; m++)
							{
								if (AppSettings.ColumnsToExport[m])
								{
									string Value2 = "";
									try
									{
										Value2 = dgv.Rows[l].Cells[m].Value.ToString();
									}
									catch
									{
									}
									doc.SetCellValue(excelRowIndex, ColIndex, Value2);
									ColIndex++;
								}
							}
							excelRowIndex++;
							uniqueExported++;
						}
						totalProcessed++;
					}
				}
				if (chunkStart % (chunkSize * 10) == 0)
				{
					GC.Collect();
					GC.WaitForPendingFinalizers();
				}
			}
			doc.Save(FileName);
			doc.Close();
			ExportService.ShowExportSummaryPopup(totalProcessed, uniqueExported);
		}

		// Token: 0x0400001B RID: 27
		private static List<string> Columns;

		// Token: 0x0400001C RID: 28
		private static string[] CSVDelimiters = new string[] { ",", ";" };
	}
}
