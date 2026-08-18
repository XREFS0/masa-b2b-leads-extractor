using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Token: 0x02000004 RID: 4
public class SpreadsheetDocument
{
	// Token: 0x0600000A RID: 10 RVA: 0x0000234C File Offset: 0x0000054C
	public SpreadsheetDocument()
	{
		this.excelApp = Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("00024500-0000-0000-C000-000000000046")));
		this.Invoke(this.excelApp, "Visible", new object[] { false }, true);
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00002380 File Offset: 0x00000580
	public void Open(string FileName)
	{
		object workbooks = this.Invoke(this.excelApp, "Workbooks", null, false, true);
		this.excelWorkbook = this.Invoke(workbooks, "Open", new object[] { FileName });
		this.excelWorksheet = this.GetFirstWorksheet();
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00002448 File Offset: 0x00000648
	public void Create()
	{
		object workbooks = this.Invoke(this.excelApp, "Workbooks", null, false, true);
		this.excelWorkbook = this.Invoke(workbooks, "Add", new object[] { Type.Missing });
		this.excelWorksheet = this.GetFirstWorksheet();
	}

	// Token: 0x0600000D RID: 13 RVA: 0x000024CC File Offset: 0x00000644
	public void Save(string FileName)
	{
		try
		{
			this.Invoke(this.excelWorkbook, "SaveAs", new object[]
			{
				FileName,
				Type.Missing,
				Type.Missing,
				Type.Missing,
				Type.Missing,
				Type.Missing,
				Type.Missing,
				Type.Missing,
				Type.Missing,
				Type.Missing,
				Type.Missing,
				Type.Missing
			});
		}
		catch
		{
		}
	}

	// Token: 0x0600000E RID: 14 RVA: 0x00002530 File Offset: 0x00000730
	public string GetCellValue(int Row, int Col)
	{
		string Value = "error";
		try
		{
			string CellName = string.Format("{0}{1}", (char)(65 + Col), 1 + Row);
			object Range = this.GetRange(CellName);
			object text = this.Invoke(Range, "Text", null, false, true);
			Value = ((text != null) ? text.ToString().Trim() : "");
		}
		catch
		{
		}
		return Value;
	}

	// Token: 0x0600000F RID: 15 RVA: 0x0000261C File Offset: 0x00000638
	public void SetCellValue(int Row, int Col, string Value)
	{
		string CellName = string.Format("{0}{1}", (char)(65 + Col), 1 + Row);
		object range = this.GetRange(CellName);
		if (Value.Length < 10)
		{
			Value = Value.Replace(",", ".");
		}
		this.Invoke(range, "Value", new object[] { Type.Missing, Value }, true);
	}

	// Token: 0x06000010 RID: 16 RVA: 0x0000267E File Offset: 0x00000660
	public MasaLeadsExtractor.Interop.Excel.ExcelRange GetUsedRange()
	{
		return null;
	}

	// Token: 0x06000011 RID: 17 RVA: 0x0000268C File Offset: 0x00000660
	public void SetWorksheet(int Index)
	{
		object worksheets = this.Invoke(this.excelWorkbook, "Worksheets", null, false, true);
		this.excelWorksheet = this.Invoke(worksheets, "Item", new object[] { Index }, false, true);
	}

	// Token: 0x06000012 RID: 18 RVA: 0x000026F4 File Offset: 0x00000660
	public void Close()
	{
		this.Invoke(this.excelWorkbook, "Close", new object[] { true, Type.Missing, Type.Missing });
		this.Invoke(this.excelApp, "Quit", null);
		this.ReleaseObject(this.excelWorksheet);
		this.ReleaseObject(this.excelWorkbook);
		this.ReleaseObject(this.excelApp);
	}

	// Token: 0x06000013 RID: 19 RVA: 0x0000274C File Offset: 0x00000660
	private object GetFirstWorksheet()
	{
		object worksheets = this.Invoke(this.excelWorkbook, "Worksheets", null, false, true);
		return this.Invoke(worksheets, "Item", new object[] { 1 }, false, true);
	}

	// Token: 0x06000014 RID: 20 RVA: 0x0000274C File Offset: 0x00000660
	private object GetRange(string CellName)
	{
		return this.Invoke(this.excelWorksheet, "Range", new object[] { CellName, Type.Missing }, false, true);
	}

	// Token: 0x06000015 RID: 21 RVA: 0x0000274C File Offset: 0x00000660
	private object Invoke(object target, string name, object[] args, bool isSet = false, bool isGet = false)
	{
		BindingFlags flags;
		if (isSet)
		{
			flags = BindingFlags.SetProperty;
		}
		else if (isGet)
		{
			flags = BindingFlags.GetProperty;
		}
		else
		{
			flags = BindingFlags.InvokeMethod;
		}
		return target.GetType().InvokeMember(name, flags, null, target, args);
	}

	// Token: 0x06000016 RID: 22 RVA: 0x0000274C File Offset: 0x00000660
	private void ReleaseObject(object obj)
	{
		try
		{
			Marshal.ReleaseComObject(obj);
			obj = null;
		}
		catch
		{
			obj = null;
		}
		finally
		{
			GC.Collect();
		}
	}

	// Token: 0x04000006 RID: 6
	private object excelApp;

	// Token: 0x04000007 RID: 7
	private object excelWorkbook;

	// Token: 0x04000008 RID: 8
	private object excelWorksheet;
}