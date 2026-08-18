using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MasaLeadsExtractor.Interop.Excel
{
	// Token: 0x02000032 RID: 50
	[CompilerGenerated]
	[Guid("000208D8-0000-0000-C000-000000000046")]
	[TypeIdentifier]
	[ComImport]
	public interface _ExcelWorksheet
	{
		// Token: 0x0600011D RID: 285
		void _VtblGap1_93();

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x0600011E RID: 286
		[DispId(197)]
		ExcelRange ExcelRange
		{
			[DispId(197)]
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		// Token: 0x0600011F RID: 287
		void _VtblGap2_16();

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000120 RID: 288
		[DispId(412)]
		ExcelRange UsedRange
		{
			[DispId(412)]
			[LCIDConversion(0)]
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}
	}
}
