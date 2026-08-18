using System;
using System.Collections.Generic;

namespace MasaLeadsExtractor
{
	// Token: 0x02000015 RID: 21
	public class TargetLocation
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00006E4D File Offset: 0x0000504D
		public TargetLocation()
		{
			this.States = new List<DataEntry>();
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00006E60 File Offset: 0x00005060
		public override string ToString()
		{
			string str = "";
			if (this.States.Count > 0)
			{
				str = this.Country.Name;
				foreach (DataEntry s in this.States)
				{
					str += string.Format(", {0}", s);
				}
				return str;
			}
			if (this.Country != null)
			{
				str = this.Country.Name;
			}
			if (this.State != null)
			{
				str += string.Format(", {0}", this.State.Name);
			}
			if (this.City != null)
			{
				str += string.Format(", {0}", this.City.Name);
			}
			if (this.ZipCode != null)
			{
				str += string.Format(", {0}", this.ZipCode.Name);
			}
			return str;
		}

		// Token: 0x04000058 RID: 88
		public DataEntry Country;

		// Token: 0x04000059 RID: 89
		public DataEntry State;

		// Token: 0x0400005A RID: 90
		public DataEntry City;

		// Token: 0x0400005B RID: 91
		public DataEntry ZipCode;

		// Token: 0x0400005C RID: 92
		public List<DataEntry> States;
	}
}
