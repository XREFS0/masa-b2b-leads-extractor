using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MasaLeadsExtractor.Interop.Excel
{
	// Token: 0x02000030 RID: 48
	[CompilerGenerated]
	[Guid("000208D5-0000-0000-C000-000000000046")]
	[DefaultMember("_Default")]
	[TypeIdentifier]
	[ComImport]
	public interface _ExcelApplication
	{
		// Token: 0x0600010E RID: 270
		void _VtblGap1_45();

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600010F RID: 271
		[DispId(572)]
		ExcelWorkbooks ExcelWorkbooks
		{
			[DispId(572)]
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[return: MarshalAs(UnmanagedType.Interface)]
			get;
		}

		// Token: 0x06000110 RID: 272
		void _VtblGap2_60();

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000111 RID: 273
		[DispId(0)]
		string _Default
		{
			[DispId(0)]
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[return: MarshalAs(UnmanagedType.BStr)]
			get;
		}

		// Token: 0x06000112 RID: 274
		void _VtblGap3_116();

		// Token: 0x06000113 RID: 275
		[DispId(302)]
		[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
		void Quit();

		// Token: 0x06000114 RID: 276
		void _VtblGap4_51();

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000115 RID: 277
		// (set) Token: 0x06000116 RID: 278
		[DispId(558)]
		bool Visible
		{
			[DispId(558)]
			[LCIDConversion(0)]
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			get;
			[LCIDConversion(0)]
			[DispId(558)]
			[MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
			[param: In]
			set;
		}
	}
}
