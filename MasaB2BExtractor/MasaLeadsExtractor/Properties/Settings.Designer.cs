using System;
using System.CodeDom.Compiler;
using System.Configuration;
using System.Runtime.CompilerServices;

namespace MasaLeadsExtractor.Properties
{
	// Token: 0x02000008 RID: 8
	[CompilerGenerated]
	[GeneratedCode("Microsoft.VisualStudio.Editors.SettingsDesigner.SettingsSingleFileGenerator", "17.11.0.0")]
	internal sealed partial class AppConfiguration : ApplicationSettingsBase
	{
		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000026 RID: 38 RVA: 0x00002EAB File Offset: 0x000010AB
		public static AppConfiguration Default
		{
			get
			{
				return AppConfiguration.defaultInstance;
			}
		}

		// Token: 0x04000011 RID: 17
		private static AppConfiguration defaultInstance = (AppConfiguration)SettingsBase.Synchronized(new AppConfiguration());
	}
}
