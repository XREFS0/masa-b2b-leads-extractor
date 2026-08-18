using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MasaLeadsExtractor.Interop.Excel
{
	// Token: 0x02000029 RID: 41
	[CompilerGenerated]
	[DefaultMember("_Default")]
	[Guid("000208D7-0000-0000-C000-000000000046")]
	[TypeIdentifier]
	[ComImport]
	public interface ExcelSheets : IEnumerable
	{
		// Token: 0x06000108 RID: 264
		void _VtblGap1_8();

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000109 RID: 265
		[DispId(170)]
		object Item
		{
			[DispId(170)]
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[return: MarshalAs(UnmanagedType.IDispatch)]
			get;
		}
	}
}
