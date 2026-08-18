using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace MasaLeadsExtractor.Properties
{
	// Token: 0x02000007 RID: 7
	[GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "17.0.0.0")]
	[DebuggerNonUserCode]
	[CompilerGenerated]
	internal class Resources
	{
		// Token: 0x06000021 RID: 33 RVA: 0x000021AE File Offset: 0x000003AE
		internal Resources()
		{
		}

		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000022 RID: 34 RVA: 0x00002E55 File Offset: 0x00001055
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static ResourceManager ResourceManager
		{
			get
			{
				if (Resources.resourceMan == null)
				{
					Resources.resourceMan = new ResourceManager("MasaLeadsExtractor.Properties.Resources", typeof(Resources).Assembly);
				}
				return Resources.resourceMan;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002E81 File Offset: 0x00001081
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002E88 File Offset: 0x00001088
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		internal static CultureInfo Culture
		{
			get
			{
				return Resources.resourceCulture;
			}
			set
			{
				Resources.resourceCulture = value;
			}
		}

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00002E90 File Offset: 0x00001090
		internal static Bitmap Icon
		{
			get
			{
				return (Bitmap)Resources.ResourceManager.GetObject("Icon", Resources.resourceCulture);
			}
		}

		// Token: 0x0400000F RID: 15
		private static ResourceManager resourceMan;

		// Token: 0x04000010 RID: 16
		private static CultureInfo resourceCulture;
	}
}
