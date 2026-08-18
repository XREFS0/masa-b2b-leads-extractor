using System;
using System.Threading;

// Token: 0x02000006 RID: 6
public class ProxyConnector
{
	// Token: 0x0600001D RID: 29 RVA: 0x00002DB4 File Offset: 0x00000FB4
	public ProxyConnector()
	{
		this.Checked = false;
		this.Processed = false;
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00002DCC File Offset: 0x00000FCC
	private void DoCheckProxy()
	{
		string SourcePageHTML = WebRequester.GetPage("http://www.equibase.com/profiles/Results.cfm?type=Horse&refno=8685211&registry=T&rbt=TB", this);
		this.CanUse = SourcePageHTML.IndexOf("Aldous Snow") > -1;
		this.Checked = true;
	}

	// Token: 0x0600001F RID: 31 RVA: 0x00002E00 File Offset: 0x00001000
	public void CheckProxy()
	{
		this.Checked = false;
		this.Processed = false;
		new Thread(new ThreadStart(this.DoCheckProxy)).Start();
	}

	// Token: 0x06000020 RID: 32 RVA: 0x00002E28 File Offset: 0x00001028
	public void CheckProxyAndWait()
	{
		string SourcePageHTML = WebRequester.GetPage("http://www.google.com", this);
		this.CanUse = SourcePageHTML.IndexOf("Google") > -1;
	}

	// Token: 0x0400000A RID: 10
	public string IP;

	// Token: 0x0400000B RID: 11
	public int Port;

	// Token: 0x0400000C RID: 12
	public bool CanUse;

	// Token: 0x0400000D RID: 13
	public bool Checked;

	// Token: 0x0400000E RID: 14
	public bool Processed;
}
