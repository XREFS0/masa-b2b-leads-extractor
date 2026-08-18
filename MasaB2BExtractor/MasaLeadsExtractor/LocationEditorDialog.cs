using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace MasaLeadsExtractor
{
	// Token: 0x02000016 RID: 22
	public partial class LocationEditorDialog : Form
	{
		// Token: 0x06000070 RID: 112 RVA: 0x00006F64 File Offset: 0x00005164
		public LocationEditorDialog(TargetLocation TargetLocation, string Title)
		{
			this.InitializeComponent();
			DarkTheme.ApplyDarkTheme(this);
			this.Refresh();
			LocationEditorDialog.DbConfig = ConnectionSettings.LoadAndDecript("GBE_DB_FIXED_KEY_2026");
			LocationEditorDialog.AppDatabase = new DataRepository(LocationEditorDialog.DbConfig);
			this.SelectedLocation = TargetLocation;
			this.Text = Title;
			foreach (object[] Row in LocationEditorDialog.AppDatabase.Select("SELECT Id, name FROM country ORDER BY name"))
			{
				DataEntry obj = new DataEntry
				{
					Id = Convert.ToInt32(Row[0]),
					Name = (string)Row[1]
				};
				this.cbCountry.Items.Add(obj);
			}
			DataEntry otherCountries = new DataEntry
			{
				Id = -155,
				Name = "OTHER COUNTRIES"
			};
			this.cbCountry.Items.Add(otherCountries);
			if (this.SelectedLocation != null)
			{
				if (TargetLocation.Country != null)
				{
					this.SetCountry(TargetLocation.Country.Id);
				}
				if (TargetLocation.State != null)
				{
					this.SetState(TargetLocation.State.Id);
				}
				if (TargetLocation.City != null)
				{
					this.SetCity(TargetLocation.City.Id);
				}
				if (TargetLocation.ZipCode != null)
				{
					this.SetZipCode(TargetLocation.ZipCode.Id);
				}
				if (this.SelectedLocation.States.Count <= 0)
				{
					return;
				}
				using (List<DataEntry>.Enumerator enumerator2 = this.SelectedLocation.States.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						DataEntry State = enumerator2.Current;
						for (int i = 0; i < this.clbStates.Items.Count; i++)
						{
							if (State.ToString() == this.clbStates.Items[i].ToString())
							{
								this.clbStates.SetItemChecked(i, true);
							}
						}
					}
					return;
				}
			}
			this.SelectedLocation = new TargetLocation();
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00007184 File Offset: 0x00005384
		private void cbCountry_SelectedIndexChanged(object sender, EventArgs e)
		{
			this.Refresh();
			DataEntry Country = (DataEntry)this.cbCountry.SelectedItem;
			DataEntry selectedCountry = this.cbCountry.SelectedItem as DataEntry;
			if (selectedCountry != null && selectedCountry.Id == -155)
			{
			}
			List<object[]> States = LocationEditorDialog.AppDatabase.Select(string.Format("SELECT Id, name, code FROM region WHERE country_id={0} AND NOT name='' ORDER BY name", Country.Id));
			if (States.Count > 0)
			{
				this.cbState.Enabled = true;
				this.clbStates.Items.Clear();
				this.cbState.Items.Clear();
				DataEntry objAllStates = new DataEntry
				{
					Id = -1,
					Name = "All states"
				};
				this.cbState.Items.Add(objAllStates);
				foreach (object[] Row in States)
				{
					DataEntry obj = new DataEntry
					{
						Id = Convert.ToInt32(Row[0]),
						Name = (string)Row[1]
					};
					this.cbState.Items.Add(obj);
					this.clbStates.Items.Add(obj);
				}
				this.cbState.SelectedIndex = 0;
				return;
			}
			this.cbState.Enabled = false;
			List<object[]> list = LocationEditorDialog.AppDatabase.Select(string.Format("SELECT Id, name FROM city WHERE country_id={0} AND NOT name='' ORDER BY name", Country.Id));
			this.cbCity.Items.Clear();
			DataEntry objAllItems = new DataEntry
			{
				Id = -1,
				Name = "All cities"
			};
			this.cbCity.Items.Add(objAllItems);
			foreach (object[] Row2 in list)
			{
				DataEntry obj2 = new DataEntry
				{
					Id = Convert.ToInt32(Row2[0]),
					Name = (string)Row2[1]
				};
				this.cbCity.Items.Add(obj2);
			}
			this.cbCity.SelectedIndex = 0;
		}

		// Token: 0x06000072 RID: 114 RVA: 0x000073E0 File Offset: 0x000055E0
		private void cbState_SelectedIndexChanged(object sender, EventArgs e)
		{
			DataEntry Country = (DataEntry)this.cbCountry.SelectedItem;
			DataEntry State = (DataEntry)this.cbState.SelectedItem;
			List<object[]> list = LocationEditorDialog.AppDatabase.Select(string.Format("SELECT Id, name FROM city WHERE region_id={0} AND country_id={1} AND NOT name='' ORDER BY name", State.Id, Country.Id));
			this.cbCity.Items.Clear();
			DataEntry objAllItems = new DataEntry
			{
				Id = -1,
				Name = "All cities"
			};
			this.cbCity.Items.Add(objAllItems);
			foreach (object[] Row in list)
			{
				DataEntry obj = new DataEntry
				{
					Id = Convert.ToInt32(Row[0]),
					Name = (string)Row[1]
				};
				this.cbCity.Items.Add(obj);
			}
			this.cbCity.SelectedIndex = 0;
		}

		// Token: 0x06000073 RID: 115 RVA: 0x000074F4 File Offset: 0x000056F4
		private void cbCity_SelectedIndexChanged(object sender, EventArgs e)
		{
			DataEntry City = (DataEntry)this.cbCity.SelectedItem;
			List<object[]> list = LocationEditorDialog.AppDatabase.Select(string.Format("SELECT city_id, name FROM zip_codes WHERE city_id={0} ORDER BY name", City.Id));
			this.cbZipCodes.Items.Clear();
			DataEntry objAllItems = new DataEntry
			{
				Id = -1,
				Name = "All zip codes"
			};
			this.cbZipCodes.Items.Add(objAllItems);
			foreach (object[] Row in list)
			{
				DataEntry obj = new DataEntry
				{
					Id = 0,
					Name = (string)Row[1]
				};
				this.cbZipCodes.Items.Add(obj);
			}
			this.cbZipCodes.SelectedIndex = 0;
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000075E0 File Offset: 0x000057E0
		private void btnSelectAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.clbStates.Items.Count; i++)
			{
				this.clbStates.SetItemChecked(i, true);
			}
		}

		// Token: 0x06000075 RID: 117 RVA: 0x00007618 File Offset: 0x00005818
		private void btnClearAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.clbStates.Items.Count; i++)
			{
				this.clbStates.SetItemChecked(i, false);
			}
		}

		// Token: 0x06000076 RID: 118 RVA: 0x0000764D File Offset: 0x0000584D
		private void btnApply_Click(object sender, EventArgs e)
		{
			this.Ok = true;
			this.GetParameters();
			LocationEditorDialog.AppDatabase.Connection.Close();
			base.Close();
		}

		// Token: 0x06000077 RID: 119 RVA: 0x00007671 File Offset: 0x00005871
		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.Ok = false;
			LocationEditorDialog.AppDatabase.Connection.Close();
			base.Close();
		}

		// Token: 0x06000078 RID: 120 RVA: 0x00007690 File Offset: 0x00005890
		private void GetParameters()
		{
			if (this.clbStates.CheckedItems.Count > 0)
			{
				this.SelectedLocation.Country = (DataEntry)this.cbCountry.SelectedItem;
				this.SelectedLocation.States.Clear();
				for (int i = 0; i < this.clbStates.Items.Count; i++)
				{
					if (this.clbStates.GetItemChecked(i))
					{
						DataEntry State = (DataEntry)this.clbStates.Items[i];
						this.SelectedLocation.States.Add(State);
					}
				}
			}
			else
			{
				this.SelectedLocation.States.Clear();
			}
			if (this.cbCountry.SelectedIndex > -1 && this.clbStates.CheckedItems.Count == 0)
			{
				this.SelectedLocation.Country = (DataEntry)this.cbCountry.SelectedItem;
				this.SelectedLocation.State = (DataEntry)this.cbState.SelectedItem;
				this.SelectedLocation.City = (DataEntry)this.cbCity.SelectedItem;
				this.SelectedLocation.ZipCode = (DataEntry)this.cbZipCodes.SelectedItem;
			}
		}

		// Token: 0x06000079 RID: 121 RVA: 0x000077CC File Offset: 0x000059CC
		public void SetCountry(int Id)
		{
			for (int i = 0; i < this.cbCountry.Items.Count; i++)
			{
				if (((DataEntry)this.cbCountry.Items[i]).Id == Id)
				{
					this.cbCountry.SelectedIndex = i;
					return;
				}
			}
		}

		// Token: 0x0600007A RID: 122 RVA: 0x00007820 File Offset: 0x00005A20
		public void SetState(int Id)
		{
			for (int i = 0; i < this.cbState.Items.Count; i++)
			{
				if (((DataEntry)this.cbState.Items[i]).Id == Id)
				{
					this.cbState.SelectedIndex = i;
					return;
				}
			}
		}

		// Token: 0x0600007B RID: 123 RVA: 0x00007874 File Offset: 0x00005A74
		public void SetCity(int Id)
		{
			for (int i = 0; i < this.cbCity.Items.Count; i++)
			{
				if (((DataEntry)this.cbCity.Items[i]).Id == Id)
				{
					this.cbCity.SelectedIndex = i;
					return;
				}
			}
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000078C8 File Offset: 0x00005AC8
		public void SetZipCode(int Id)
		{
			for (int i = 0; i < this.cbZipCodes.Items.Count; i++)
			{
				if (((DataEntry)this.cbZipCodes.Items[i]).Id == Id)
				{
					this.cbZipCodes.SelectedIndex = i;
					return;
				}
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x0000791B File Offset: 0x00005B1B
		private void button1_Click(object sender, EventArgs e)
		{
			this.LocationEditForm_Load(sender, e);
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00007925 File Offset: 0x00005B25
		private void LocationEditForm_Load(object sender, EventArgs e)
		{
		}

		// Token: 0x0400005D RID: 93
		public bool Ok;

		// Token: 0x0400005E RID: 94
		public TargetLocation SelectedLocation;

		// Token: 0x0400005F RID: 95
		public static ConnectionSettings DbConfig;

		// Token: 0x04000060 RID: 96
		public static DataRepository AppDatabase;
	}
}
