using System;
using System.Collections;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MasaLeadsExtractor.Interop.Excel
{
	// Token: 0x02000028 RID: 40
	[CompilerGenerated]
	[InterfaceType(ComInterfaceType.InterfaceIsIDispatch)]
	[DefaultMember("_Default")]
	[Guid("00020846-0000-0000-C000-000000000046")]
	[TypeIdentifier]
	[ComImport]
	public interface ExcelRange : IEnumerable
	{
		// Token: 0x06000103 RID: 259
		void _VtblGap1_164();

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000104 RID: 260
		[DispId(138)]
		object Text
		{
			[DispId(138)]
			[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[return: MarshalAs(UnmanagedType.Struct)]
			get;
		}

		// Token: 0x06000105 RID: 261
		void _VtblGap2_8();

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000106 RID: 262
		// (set) Token: 0x06000107 RID: 263
		[DispId(6)]
		object Value
		{
			[DispId(6)]
			[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[return: MarshalAs(UnmanagedType.Struct)]
			get;
			[DispId(6)]
			[MethodImpl(MethodImplOptions.PreserveSig | MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[param: MarshalAs(UnmanagedType.Struct)]
			[param: In]
			[param: Optional]
			set;
		}
	}
}
