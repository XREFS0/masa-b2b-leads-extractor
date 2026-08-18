using System;
using System.Collections.Generic;

namespace MasaLeadsExtractor
{
	// Token: 0x0200000B RID: 11
	public class DataChunk
	{
		// Token: 0x06000035 RID: 53 RVA: 0x000034B0 File Offset: 0x000016B0
		public DataChunk(string InputString)
		{
			this.Data = new List<string>();
			this.Parse(new List<string> { InputString });
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00003530 File Offset: 0x00001730
		private void Parse(List<string> InputData)
		{
			this.AddedLines = 0;
			for (int i = 0; i < InputData.Count; i++)
			{
				if (InputData[i][0] == '[' && InputData[i][InputData[i].Length - 1] == ']')
				{
					InputData[i] = InputData[i].Substring(1, InputData[i].Length - 2);
				}
				int PrevPos = 0;
				int Pos = 0;
				int Counter = 0;
				while (Pos < InputData[i].Length - 2)
				{
					if (InputData[i][Pos] == '[')
					{
						Counter++;
					}
					if (InputData[i][Pos] == ']')
					{
						Counter--;
					}
					if (Counter == 0)
					{
						for (int j = 0; j < this.Splitters.Length; j++)
						{
							if (InputData[i].Substring(Pos, 3) == this.Splitters[j])
							{
								string v = InputData[i].Substring(PrevPos + 1, Pos - PrevPos);
								if (v != "null")
								{
									this.Data.Add(v);
								}
								PrevPos = Pos + 1;
								this.AddedLines++;
							}
						}
					}
					Pos++;
				}
			}
		}

		// Token: 0x04000016 RID: 22
		public List<string> Data;

		// Token: 0x04000017 RID: 23
		private string[] Splitters = new string[] { "],\"", "\",[", "\",n", "n,\"", "l,n", "l,[", "],[", "],n" };

		// Token: 0x04000018 RID: 24
		public bool IsCompleted;

		// Token: 0x04000019 RID: 25
		private int AddedLines;
	}
}
