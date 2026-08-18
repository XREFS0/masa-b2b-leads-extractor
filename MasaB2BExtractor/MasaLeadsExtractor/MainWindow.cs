using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;

namespace MasaLeadsExtractor
{
	// Token: 0x02000017 RID: 23
	public partial class MainWindow : KryptonForm
	{
		// Token: 0x06000081 RID: 129 RVA: 0x00008290 File Offset: 0x00006490
		public MainWindow()
		{
			this.InitializeComponent();
			DarkTheme.ApplyDarkTheme(this);
			this.InitializeBackgroundWorker();
			Startup.LanguagesManager.InitFields(Startup.LanguagesFiles[Startup.AppSettings.Language]);
			Startup.LanguagesManager.InitControl(this, base.Controls);
			Startup.LanguagesManager.InitMenu(this);
			Startup.LanguagesManager.InitTableColumns(this.dgvResults);
			this.LoadCategories();
			this.LoadLocations();
			this.LoadTasks();
			this.ValidateRegistration();
		}

		// Token: 0x06000082 RID: 130 RVA: 0x00008314 File Offset: 0x00006514
		private void InitializeBackgroundWorker()
		{
			this._scrapingWorker = new BackgroundWorker();
			this._scrapingWorker.WorkerReportsProgress = true;
			this._scrapingWorker.WorkerSupportsCancellation = true;
			this._scrapingWorker.DoWork += this.ScrapingWorker_DoWork;
			this._scrapingWorker.ProgressChanged += this.ScrapingWorker_ProgressChanged;
			this._scrapingWorker.RunWorkerCompleted += this.ScrapingWorker_RunWorkerCompleted;
		}

		// Token: 0x06000083 RID: 131 RVA: 0x0000838C File Offset: 0x0000658C
		private void ScrapingWorker_DoWork(object sender, DoWorkEventArgs e)
		{
			BackgroundWorker worker = sender as BackgroundWorker;
			Startup.StopDataCollection = false;
			int TaskIndex = 0;
			MethodInvoker cachedDeleteTask = null;
			while (TaskIndex < Startup.AppSettings.Tasks.Count && !Startup.StopDataCollection)
			{
				if (worker.CancellationPending)
				{
					e.Cancel = true;
					return;
				}
				try
				{
					new MapsResultCollector(TaskIndex, this, Startup.AppSettings.ExtractEmails);
					GC.Collect();
					MethodInvoker methodInvoker;
					if ((methodInvoker = cachedDeleteTask) == null)
					{
						methodInvoker = (cachedDeleteTask = delegate
						{
							if (this.dgvTasks.Rows.Count > TaskIndex)
							{
								int CurrentTaskId = 0;
								int.TryParse(this.dgvTasks.Rows[TaskIndex].Cells[0].Value.ToString(), out CurrentTaskId);
								for (int i = 0; i < Startup.AppSettings.Tasks.Count; i++)
								{
									if (CurrentTaskId == Startup.AppSettings.Tasks[i].TaskId)
									{
										Startup.AppSettings.Tasks.RemoveAt(i);
										break;
									}
								}
								Startup.AppSettings.Save(Startup.SettingsFileName);
								this.LoadTasks();
							}
						});
					}
					MethodInvoker deleteTask = methodInvoker;
					if (base.InvokeRequired)
					{
						base.Invoke(deleteTask);
					}
					else
					{
						deleteTask();
					}
				}
				catch (Exception)
				{
				}
			}
		}

		// Token: 0x06000084 RID: 132 RVA: 0x00007925 File Offset: 0x00005B25
		private void ScrapingWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
		{
		}

		// Token: 0x06000085 RID: 133 RVA: 0x00008458 File Offset: 0x00006658
		private void ScrapingWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			this.lblInfo.Text = "Done!";
			this.tspProgress.Value = 0;
			if (e.Cancelled || Startup.StopDataCollection)
			{
				if (!Startup.IsDemoLimit)
				{
					MessageBox.Show("Stopped by user!");
					return;
				}
			}
			else
			{
				if (e.Error != null)
				{
					MessageBox.Show("Error during extraction: " + e.Error.Message);
					return;
				}
				MessageBox.Show("Processing is done!");
			}
		}

		// Token: 0x06000087 RID: 135 RVA: 0x000085E8 File Offset: 0x000067E8
		private void ValidateRegistration()
		{
			Startup.IsDemoVersion = false;
			Startup.AppSettings = AppConfiguration.Load(Startup.SettingsFileName);
			this.Text = "MASA B2B Leads Extractor";
		}

		// Token: 0x06000088 RID: 136 RVA: 0x000086B8 File Offset: 0x000068B8
		private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (this._scrapingWorker != null && this._scrapingWorker.IsBusy)
			{
				this._scrapingWorker.CancelAsync();
				Startup.StopDataCollection = true;
			}
			if (MessageBox.Show(Startup.LanguagesManager.ExitMessage, "MASA B2B Leads Extractor", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				e.Cancel = false;
				return;
			}
			e.Cancel = true;
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00008714 File Offset: 0x00006914
		private void LoadCategories()
		{
			this.cbCategories.Items.Clear();
			foreach (string c in Startup.AppSettings.Categories)
			{
				this.cbCategories.Items.Add(c);
			}
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00008788 File Offset: 0x00006988
		private void btnCategoriesAdd_Click(object sender, EventArgs e)
		{
			TextInputDialog inputTextForm = new TextInputDialog("", "Add category", "Category");
			inputTextForm.ShowDialog();
			if (inputTextForm.OkPressed)
			{
				Startup.AppSettings.Categories.Add(inputTextForm.Value);
				this.LoadCategories();
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x000087D4 File Offset: 0x000069D4
		private void btnCategoriesEdit_Click(object sender, EventArgs e)
		{
			if (this.cbCategories.SelectedItem != null)
			{
				TextInputDialog inputTextForm = new TextInputDialog(this.cbCategories.SelectedItem.ToString(), "Edit category", "Category");
				inputTextForm.ShowDialog();
				if (inputTextForm.OkPressed)
				{
					Startup.AppSettings.Categories[this.cbCategories.SelectedIndex] = inputTextForm.Value;
				}
				this.LoadCategories();
				return;
			}
			MessageBox.Show("Please select category!");
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00008850 File Offset: 0x00006A50
		private void btnCategoriesDelete_Click(object sender, EventArgs e)
		{
			if (this.cbCategories.CheckedItems.Count > 0 && MessageBox.Show("Do you wand to delete checked items?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				for (int i = 0; i < this.cbCategories.Items.Count; i++)
				{
					if (this.cbCategories.GetItemChecked(i))
					{
						for (int j = 0; j < Startup.AppSettings.Categories.Count; j++)
						{
							if (Startup.AppSettings.Categories[j] == this.cbCategories.Items[i].ToString())
							{
								Startup.AppSettings.Categories.RemoveAt(j);
							}
						}
					}
				}
				this.LoadCategories();
				return;
			}
			MessageBox.Show(Startup.LanguagesManager.SelectCategory);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00008920 File Offset: 0x00006B20
		private void btnCategoriesUpload_Click(object sender, EventArgs e)
		{
			CategoriesImportDialog uploadForm = new CategoriesImportDialog();
			uploadForm.ShowDialog();
			if (uploadForm.UseThem)
			{
				foreach (string Category in uploadForm.tbUploadCategories.Lines)
				{
					if (Category.Trim() != "")
					{
						Startup.AppSettings.Categories.Add(Category);
					}
				}
				this.LoadCategories();
			}
		}

		// Token: 0x0600008E RID: 142 RVA: 0x00008988 File Offset: 0x00006B88
		private void btnCategoriesSelectAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.cbCategories.Items.Count; i++)
			{
				this.cbCategories.SetItemChecked(i, true);
			}
		}

		// Token: 0x0600008F RID: 143 RVA: 0x000089C0 File Offset: 0x00006BC0
		private void btnCategoriesClearSelection_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.cbCategories.Items.Count; i++)
			{
				this.cbCategories.SetItemChecked(i, false);
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x000089F8 File Offset: 0x00006BF8
		private void LoadLocations()
		{
			this.cbLocations.Items.Clear();
			foreach (TargetLocation i in Startup.AppSettings.Locations)
			{
				this.cbLocations.Items.Add(i);
			}
		}

		// Token: 0x06000091 RID: 145 RVA: 0x00008A6C File Offset: 0x00006C6C
		private void btnLocationsAdd_Click(object sender, EventArgs e)
		{
			LocationEditorDialog locationEditForm = new LocationEditorDialog(null, "Add location");
			locationEditForm.ShowDialog();
			if (locationEditForm.Ok)
			{
				TargetLocation NewLocation = locationEditForm.SelectedLocation;
				if (NewLocation.ToString() != "")
				{
					Startup.AppSettings.Locations.Add(NewLocation);
					Startup.AppSettings.Save(Startup.SettingsFileName);
				}
			}
			this.LoadLocations();
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00008AD4 File Offset: 0x00006CD4
		private void btnLocationsEdit_Click(object sender, EventArgs e)
		{
			if (this.cbLocations.SelectedIndex > -1)
			{
				LocationEditorDialog locationEditForm = new LocationEditorDialog((TargetLocation)this.cbLocations.SelectedItem, "Edit location");
				locationEditForm.ShowDialog();
				if (locationEditForm.Ok)
				{
					Startup.AppSettings.Locations[this.cbLocations.SelectedIndex] = locationEditForm.SelectedLocation;
					Startup.AppSettings.Save(Startup.SettingsFileName);
				}
				this.LoadLocations();
				return;
			}
			MessageBox.Show("Please select location!");
		}

		// Token: 0x06000093 RID: 147 RVA: 0x00008B5C File Offset: 0x00006D5C
		private void btnLocationsDelete_Click(object sender, EventArgs e)
		{
			if (this.cbLocations.CheckedItems.Count > 0 && MessageBox.Show("Do you wand to delete checked items?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				for (int i = 0; i < this.cbLocations.Items.Count; i++)
				{
					if (this.cbLocations.GetItemChecked(i))
					{
						for (int j = 0; j < Startup.AppSettings.Locations.Count; j++)
						{
							if (Startup.AppSettings.Locations[j].ToString() == this.cbLocations.Items[i].ToString())
							{
								Startup.AppSettings.Locations.RemoveAt(j);
							}
						}
					}
				}
				this.LoadLocations();
				return;
			}
			MessageBox.Show("Please select location!");
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00008C30 File Offset: 0x00006E30
		private void btnLocationsUpload_Click(object sender, EventArgs e)
		{
			LocationsImportDialog uploadLocationsForm = new LocationsImportDialog();
			uploadLocationsForm.ShowDialog();
			if (uploadLocationsForm.Ok)
			{
				foreach (string text in uploadLocationsForm.tbUploadLocations.Lines)
				{
					if (uploadLocationsForm.tbCountry.Text.Trim() != "")
					{
						string.Format("{0}, ", uploadLocationsForm.tbCountry.Text.Trim());
					}
					if (text.Trim() != "")
					{
						TargetLocation NewLocation = new TargetLocation();
						Startup.AppSettings.Locations.Add(NewLocation);
					}
				}
				this.LoadLocations();
			}
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00008CD8 File Offset: 0x00006ED8
		private void btnLocationsSelectAll_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.cbLocations.Items.Count; i++)
			{
				this.cbLocations.SetItemChecked(i, true);
			}
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00008D10 File Offset: 0x00006F10
		private void btnLocationsClearSelection_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.cbLocations.Items.Count; i++)
			{
				this.cbLocations.SetItemChecked(i, false);
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00008D48 File Offset: 0x00006F48
		private void LoadTasks()
		{
			this.dgvTasks.Rows.Clear();
			for (int i = 0; i < Startup.AppSettings.Tasks.Count; i++)
			{
				this.dgvTasks.Rows.Add(new object[]
				{
					Startup.AppSettings.Tasks[i].TaskId,
					Startup.AppSettings.Tasks[i].Category,
					Startup.AppSettings.Tasks[i].TargetLocation,
					Startup.AppSettings.Tasks[i].Country,
					Startup.AppSettings.Tasks[i].State,
					Startup.AppSettings.Tasks[i].City,
					Startup.AppSettings.Tasks[i].ZipCode
				});
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00008E4C File Offset: 0x0000704C
		private void startToolStripMenuItem_Click(object sender, EventArgs e)
		{
			MainWindow.DbConfig = ConnectionSettings.LoadAndDecript("GBE_DB_FIXED_KEY_2026");
			MainWindow.AppDatabase = new DataRepository(MainWindow.DbConfig);
			Startup.StopDataCollection = false;
			Startup.IsDemoLimit = false;
			if ((this.dgvTasks.Rows.Count > 0 && this.cbCategories.SelectedItems.Count > 0 && this.cbLocations.SelectedItems.Count > 0 && MessageBox.Show("Do you want to replace previous tasks?", "Tasks", MessageBoxButtons.YesNo) == DialogResult.Yes) || (this.dgvTasks.Rows.Count == 0 && this.cbCategories.SelectedItems.Count > 0 && this.cbLocations.SelectedItems.Count > 0))
			{
				this.dgvTasks.Rows.Clear();
				Startup.AppSettings.Tasks.Clear();
				for (int i = 0; i < this.cbCategories.Items.Count; i++)
				{
					if (this.cbCategories.GetItemChecked(i))
					{
						for (int j = 0; j < this.cbLocations.Items.Count; j++)
						{
							if (this.cbLocations.GetItemChecked(j))
							{
								TargetLocation TaskLocation = Startup.AppSettings.Locations[j];
								string Category = Startup.AppSettings.Categories[i];
								if (TaskLocation.States.Count == 0 && TaskLocation.State != null && TaskLocation.State.Id == -1)
								{
									List<DataEntry> Cities = this.GetCitiesByCountry(TaskLocation.Country);
									for (int CityIndex = 0; CityIndex < Cities.Count; CityIndex++)
									{
										this.lblInfo.Text = string.Format("Adding locations for {0}... {1}/{2} ", TaskLocation.State.Name, CityIndex, Cities.Count);
										int v = (int)Math.Round((double)(100f * (float)CityIndex / (float)Cities.Count));
										if (v <= 100)
										{
											this.tspProgress.Value = v;
										}
										List<DataEntry> ZipCodes = this.GetZipCodes(Cities[CityIndex]);
										for (int ZipCodeIndex = 0; ZipCodeIndex < ZipCodes.Count; ZipCodeIndex++)
										{
											ExtractionTask NewTask = new ExtractionTask
											{
												TaskId = Startup.AppSettings.Tasks.Count + 1,
												Category = Category,
												TargetLocation = "",
												Country = TaskLocation.Country.Name,
												State = TaskLocation.State.Name,
												City = Cities[CityIndex].Name,
												ZipCode = ZipCodes[ZipCodeIndex].Name
											};
											Startup.AppSettings.Tasks.Add(NewTask);
										}
										if (CityIndex % 20 == 0)
										{
											Application.DoEvents();
										}
									}
								}
								else if (TaskLocation.States.Count > 0)
								{
									for (int k = 0; k < TaskLocation.States.Count; k++)
									{
										List<DataEntry> Cities2 = this.GetCities(TaskLocation.States[k]);
										for (int CityIndex2 = 0; CityIndex2 < Cities2.Count; CityIndex2++)
										{
											this.lblInfo.Text = string.Format("Adding locations for {0}... {1}/{2} ", TaskLocation.States[k].Name, CityIndex2, Cities2.Count);
											int v2 = (int)Math.Round((double)(100f * (float)CityIndex2 / (float)Cities2.Count));
											if (v2 <= 100)
											{
												this.tspProgress.Value = v2;
											}
											List<DataEntry> ZipCodes2 = this.GetZipCodes(Cities2[CityIndex2]);
											for (int ZipCodeIndex2 = 0; ZipCodeIndex2 < ZipCodes2.Count; ZipCodeIndex2++)
											{
												ExtractionTask NewTask2 = new ExtractionTask
												{
													TaskId = Startup.AppSettings.Tasks.Count + 1,
													Category = Category,
													TargetLocation = "",
													Country = TaskLocation.Country.Name,
													State = TaskLocation.States[k].Name,
													City = Cities2[CityIndex2].Name,
													ZipCode = ZipCodes2[ZipCodeIndex2].Name
												};
												Startup.AppSettings.Tasks.Add(NewTask2);
											}
											if (CityIndex2 % 20 == 0)
											{
												Application.DoEvents();
											}
										}
									}
								}
								else if (TaskLocation.States.Count == 0 && TaskLocation.City != null && TaskLocation.City.Id == -1)
								{
									List<DataEntry> Cities3 = this.GetCities(TaskLocation.State);
									for (int CityIndex3 = 0; CityIndex3 < Cities3.Count; CityIndex3++)
									{
										this.lblInfo.Text = string.Format("Adding locations for {0}... {1}/{2} ", TaskLocation.State.Name, CityIndex3, Cities3.Count);
										int v3 = (int)Math.Round((double)(100f * (float)CityIndex3 / (float)Cities3.Count));
										if (v3 <= 100)
										{
											this.tspProgress.Value = v3;
										}
										List<DataEntry> ZipCodes3 = this.GetZipCodes(Cities3[CityIndex3]);
										for (int ZipCodeIndex3 = 0; ZipCodeIndex3 < ZipCodes3.Count; ZipCodeIndex3++)
										{
											ExtractionTask NewTask3 = new ExtractionTask
											{
												TaskId = Startup.AppSettings.Tasks.Count + 1,
												Category = Category,
												TargetLocation = "",
												Country = TaskLocation.Country.Name,
												State = TaskLocation.State.Name,
												City = Cities3[CityIndex3].Name,
												ZipCode = ZipCodes3[ZipCodeIndex3].Name
											};
											Startup.AppSettings.Tasks.Add(NewTask3);
										}
										if (CityIndex3 % 20 == 0)
										{
											Application.DoEvents();
										}
									}
								}
								else if (TaskLocation.States.Count == 0 && TaskLocation.City != null && TaskLocation.ZipCode.Id == -1)
								{
									List<DataEntry> ZipCodes4 = this.GetZipCodes(TaskLocation.City);
									for (int ZipCodeIndex4 = 0; ZipCodeIndex4 < ZipCodes4.Count; ZipCodeIndex4++)
									{
										ExtractionTask NewTask4 = new ExtractionTask
										{
											TaskId = Startup.AppSettings.Tasks.Count + 1,
											Category = Category,
											TargetLocation = "",
											Country = TaskLocation.Country.Name,
											State = TaskLocation.State.Name,
											City = TaskLocation.City.Name,
											ZipCode = ZipCodes4[ZipCodeIndex4].Name
										};
										Startup.AppSettings.Tasks.Add(NewTask4);
									}
								}
								else if (TaskLocation.States.Count == 0)
								{
									ExtractionTask NewTask5 = new ExtractionTask
									{
										TaskId = Startup.AppSettings.Tasks.Count + 1,
										Category = Category,
										TargetLocation = "",
										Country = ((TaskLocation.Country != null) ? TaskLocation.Country.Name : ""),
										State = ((TaskLocation.State != null) ? TaskLocation.State.Name : ""),
										City = ((TaskLocation.City != null) ? TaskLocation.City.Name : ""),
										ZipCode = ((TaskLocation.ZipCode != null) ? TaskLocation.ZipCode.Name : "")
									};
									Startup.AppSettings.Tasks.Add(NewTask5);
								}
							}
						}
					}
				}
				Startup.AppSettings.Save(Startup.SettingsFileName);
				this.LoadTasks();
			}
			else if (this.dgvTasks.Rows.Count == 0 && (this.cbCategories.SelectedItems.Count == 0 || this.cbLocations.SelectedItems.Count == 0))
			{
				MessageBox.Show("Select at least one category and one location!");
				MainWindow.AppDatabase.Connection.Close();
				return;
			}
			MainWindow.AppDatabase.Connection.Close();
			if (this.dgvTasks.Rows.Count == 0)
			{
				MessageBox.Show("There are no tasks!");
				return;
			}
			this.dgvResults.Rows.Clear();
			if (!this._scrapingWorker.IsBusy)
			{
				this._scrapingWorker.RunWorkerAsync();
				return;
			}
			MessageBox.Show("Extraction already in progress!");
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000096D0 File Offset: 0x000078D0
		private void stopToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (this._scrapingWorker != null && this._scrapingWorker.IsBusy)
			{
				this._scrapingWorker.CancelAsync();
			}
			Startup.StopDataCollection = true;
			Application.DoEvents();
			foreach (Process p in Process.GetProcesses())
			{
				if (p.ProcessName == "phantomjs")
				{
					try
					{
						p.Kill();
					}
					catch
					{
					}
				}
			}
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00009750 File Offset: 0x00007950
		private void exportDataToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00007925 File Offset: 0x00005B25
		private void exportKMLToolStripMenuItem_Click(object sender, EventArgs e)
		{
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00009770 File Offset: 0x00007970
		private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new SettingsDialog().ShowDialog();
			Startup.AppSettings = AppConfiguration.Load(Startup.SettingsFileName);
			Startup.LanguagesManager.InitFields(Startup.LanguagesFiles[Startup.AppSettings.Language]);
			Startup.LanguagesManager.InitControl(this, base.Controls);
			Startup.LanguagesManager.InitMenu(this);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x000097EA File Offset: 0x000079EA
		// Token: 0x060000A0 RID: 160 RVA: 0x0000981C File Offset: 0x00007A1C
		private List<DataEntry> GetCitiesByCountry(DataEntry Country)
		{
			List<DataEntry> Cities = new List<DataEntry>();
			foreach (object[] Row in MainWindow.AppDatabase.Select(string.Format("SELECT Id, name FROM city WHERE country_id={0} ORDER BY name", Country.Id)))
			{
				DataEntry obj = new DataEntry
				{
					Id = Convert.ToInt32(Row[0]),
					Name = (string)Row[1]
				};
				Cities.Add(obj);
			}
			return Cities;
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x000098B4 File Offset: 0x00007AB4
		private List<DataEntry> GetCities(DataEntry State)
		{
			List<DataEntry> Cities = new List<DataEntry>();
			foreach (object[] Row in MainWindow.AppDatabase.Select(string.Format("SELECT Id, name FROM city WHERE region_id={0} ORDER BY name", State.Id)))
			{
				DataEntry obj = new DataEntry
				{
					Id = Convert.ToInt32(Row[0]),
					Name = (string)Row[1]
				};
				Cities.Add(obj);
			}
			return Cities;
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x0000994C File Offset: 0x00007B4C
		private List<DataEntry> GetZipCodes(DataEntry City)
		{
			List<DataEntry> ZipCodes = new List<DataEntry>();
			foreach (object[] Row in MainWindow.AppDatabase.Select(string.Format("SELECT name FROM zip_codes WHERE city_id={0} ORDER BY name", City.Id)))
			{
				DataEntry obj = new DataEntry
				{
					Id = 0,
					Name = (string)Row[0]
				};
				ZipCodes.Add(obj);
			}
			return ZipCodes;
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x000099DC File Offset: 0x00007BDC
		private void btnTasksSelectAll_Click(object sender, EventArgs e)
		{
			this.dgvTasks.SelectAll();
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x000099EC File Offset: 0x00007BEC
		private void btnTasksClearSelection_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < this.dgvTasks.Rows.Count; i++)
			{
				this.dgvTasks.Rows[i].Selected = false;
			}
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00009A2C File Offset: 0x00007C2C
		private void btnTasksDeleteSelected_Click(object sender, EventArgs e)
		{
			int CurrentTaskId = 0;
			foreach (object obj in this.dgvTasks.SelectedRows)
			{
				DataGridViewRow row = (DataGridViewRow)obj;
				int.TryParse(this.dgvTasks.Rows[row.Index].Cells[0].Value.ToString(), out CurrentTaskId);
				for (int i = 0; i < Startup.AppSettings.Tasks.Count; i++)
				{
					if (CurrentTaskId == Startup.AppSettings.Tasks[i].TaskId)
					{
						Startup.AppSettings.Tasks.RemoveAt(i);
						break;
					}
				}
				Startup.AppSettings.Save(Startup.SettingsFileName);
				this.dgvTasks.Rows.RemoveAt(row.Index);
			}
			Startup.AppSettings.Save(Startup.SettingsFileName);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00009B3C File Offset: 0x00007D3C
		private void btnTasksSaveTasks_Click(object sender, EventArgs e)
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "Tasks|*.tsk",
				InitialDirectory = Application.StartupPath
			};
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string FileContent = "";
				for (int i = 0; i < this.dgvTasks.Rows.Count; i++)
				{
					string Line = "";
					for (int j = 0; j < this.dgvTasks.ColumnCount; j++)
					{
						Line = Line + this.dgvTasks.Rows[i].Cells[j].Value.ToString() + "|";
					}
					FileContent = FileContent + Line.Substring(0, Line.Length - 1) + Environment.NewLine;
				}
				File.WriteAllText(saveFileDialog.FileName, FileContent);
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x00009C10 File Offset: 0x00007E10
		private void btnTasksLoadTasks_Click(object sender, EventArgs e)
		{
			if (this.dgvTasks.Rows.Count > 0 && MessageBox.Show("Do you want to replace previous tasks?", "Tasks", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Tasks|*.tsk",
				InitialDirectory = Application.StartupPath
			};
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				this.dgvTasks.Rows.Clear();
				Startup.AppSettings.Tasks.Clear();
				string[] Content = File.ReadAllLines(openFileDialog.FileName);
				for (int i = 0; i < Content.Length; i++)
				{
					string[] Values = Content[i].Split(new char[] { '|' });
					DataGridViewRowCollection rows = this.dgvTasks.Rows;
					object[] array = Values;
					rows.Add(array);
					ExtractionTask NewTask = new ExtractionTask
					{
						TaskId = Startup.AppSettings.Tasks.Count + 1,
						Category = Values[1],
						TargetLocation = "",
						Country = Values[3],
						State = Values[4],
						City = Values[5],
						ZipCode = Values[6]
					};
					Startup.AppSettings.Tasks.Add(NewTask);
					Startup.AppSettings.Save(Startup.SettingsFileName);
				}
			}
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00009D4C File Offset: 0x00007F4C
		private string GetDataFilePath()
		{
			string appFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MASA B2B Leads Extractor");
			if (!Directory.Exists(appFolder))
			{
				Directory.CreateDirectory(appFolder);
			}
			return Path.Combine(appFolder, "saved_data.xml");
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x00009D88 File Offset: 0x00007F88
		private void SaveDataGridViewData()
		{
			try
			{
				string filePath = this.GetDataFilePath();
				DataTable dt = new DataTable("SavedData");
				int columnCount = this.dgvResults.Columns.Count;
				for (int i = 0; i < columnCount; i++)
				{
					dt.Columns.Add(this.dgvResults.Columns[i].HeaderText, typeof(string));
				}
				foreach (object obj in ((IEnumerable)this.dgvResults.Rows))
				{
					DataGridViewRow row = (DataGridViewRow)obj;
					if (!row.IsNewRow)
					{
						DataRow dr = dt.NewRow();
						for (int j = 0; j < columnCount; j++)
						{
							DataRow dataRow = dr;
							int num = j;
							object value = row.Cells[j].Value;
							dataRow[num] = ((value != null) ? value.ToString() : null) ?? string.Empty;
						}
						dt.Rows.Add(dr);
					}
				}
				dt.WriteXml(filePath, XmlWriteMode.WriteSchema);
				Console.WriteLine("Dati salvati in: " + filePath);
			}
			catch (Exception ex)
			{
				Console.WriteLine("Errore salvataggio dati: " + ex.Message);
			}
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00009EFC File Offset: 0x000080FC
		private void LoadDataGridViewData()
		{
			try
			{
				string filePath = this.GetDataFilePath();
				if (File.Exists(filePath))
				{
					DataTable dt = new DataTable();
					dt.ReadXml(filePath);
					this.dgvResults.Rows.Clear();
					foreach (object obj in dt.Rows)
					{
						DataRow dr = (DataRow)obj;
						int rowIndex = this.dgvResults.Rows.Add();
						for (int i = 0; i < Math.Min(dt.Columns.Count, this.dgvResults.Columns.Count); i++)
						{
							this.dgvResults.Rows[rowIndex].Cells[i].Value = dr[i].ToString();
						}
					}
					Console.WriteLine(string.Format("Caricati {0} record da: {1}", dt.Rows.Count, filePath));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Errore caricamento dati: " + ex.Message);
			}
		}

		// Token: 0x060000AB RID: 171 RVA: 0x0000A040 File Offset: 0x00008240
		private void btnSelectAll_Click(object sender, EventArgs e)
		{
			this.dgvResults.SelectAll();
		}

		// Token: 0x060000AC RID: 172 RVA: 0x0000A04D File Offset: 0x0000824D
		private void btnClearSelection_Click(object sender, EventArgs e)
		{
			this.dgvResults.ClearSelection();
		}

		// Token: 0x060000AD RID: 173 RVA: 0x0000A05C File Offset: 0x0000825C
		private void btnDeleteSelected_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(string.Format(Startup.LanguagesManager.DeleteSomeRows, this.dgvResults.SelectedRows.Count), "Delete", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
			{
				foreach (object obj in this.dgvResults.SelectedRows)
				{
					DataGridViewRow item = (DataGridViewRow)obj;
					this.dgvResults.Rows.RemoveAt(item.Index);
				}
			}
		}

		// Token: 0x060000AE RID: 174 RVA: 0x0000A0FC File Offset: 0x000082FC
		private void btnDeleteAll_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show(string.Format(Startup.LanguagesManager.DeleteAllRows, this.dgvResults.SelectedRows.Count), "Delete", MessageBoxButtons.YesNoCancel) == DialogResult.Yes)
			{
				this.dgvResults.Rows.Clear();
			}
		}

		// Token: 0x060000AF RID: 175 RVA: 0x0000A14C File Offset: 0x0000834C
		private void btnExport_Click(object sender, EventArgs e)
		{
			if (this.dgvResults.Rows.Count == 0)
			{
				MessageBox.Show(Startup.LanguagesManager.NoDataToExport);
			}
			this.dgvResults.SelectAll();
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			if (Startup.AppSettings.ExportType == 0)
			{
				saveFileDialog.Filter = "CSV files|*.csv";
			}
			else if (Startup.AppSettings.ExportType == 1)
			{
				saveFileDialog.Filter = "Excel files|*.xls";
			}
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				if (Startup.AppSettings.ExportType == 0)
				{
					ExportService.SaveToCSV(Startup.AppSettings, saveFileDialog.FileName, this.dgvResults, true);
					return;
				}
				if (Startup.AppSettings.ExportType == 1)
				{
					ExportService.SaveToXLS(Startup.AppSettings, saveFileDialog.FileName, this.dgvResults, true);
					return;
				}
			}
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x0000A220 File Offset: 0x00008420
		private void btnOpenWebsite_Click(object sender, EventArgs e)
		{
			if (this.dgvResults.SelectedRows.Count > 0)
			{
				string Url = "";
				if (this.dgvResults.Rows[this.dgvResults.SelectedRows[0].Index].Cells[10].Value != null)
				{
					Url = this.dgvResults.Rows[this.dgvResults.SelectedRows[0].Index].Cells[10].Value.ToString();
				}
				if (Url != "")
				{
					Process.Start(new ProcessStartInfo(Url));
				}
			}
		}

		// Token: 0x060000B1 RID: 177 RVA: 0x0000A2D8 File Offset: 0x000084D8
		private void btnOpenMap_Click(object sender, EventArgs e)
		{
			string Url = "";
			if (this.dgvResults.Rows[this.dgvResults.SelectedRows[0].Index].Cells[13].Value != null)
			{
				Url = this.dgvResults.Rows[this.dgvResults.SelectedRows[0].Index].Cells[13].Value.ToString();
			}
			if (Url != "")
			{
				Process.Start(new ProcessStartInfo(Url));
			}
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x0000A37C File Offset: 0x0000857C
		private void btnOpenDetails_Click(object sender, EventArgs e)
		{
			string Url = "";
			if (this.dgvResults.Rows[this.dgvResults.SelectedRows[0].Index].Cells[14].Value != null)
			{
				Url = this.dgvResults.Rows[this.dgvResults.SelectedRows[0].Index].Cells[14].Value.ToString();
			}
			if (Url != "")
			{
				Process.Start(new ProcessStartInfo(Url));
			}
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x0000A41E File Offset: 0x0000861E
		private void MainWindow_FormClosed(object sender, FormClosedEventArgs e)
		{
			this.SaveDataGridViewData();
			Startup.AppSettings.Save(Startup.SettingsFileName);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x0000A438 File Offset: 0x00008638
		private void dgvResults_CellClick(object sender, DataGridViewCellEventArgs e)
		{
			if ((e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 13 || e.ColumnIndex == 14) && e.RowIndex > -1 && this.dgvResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null)
			{
				try
				{
					string url = this.dgvResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();
					if (e.ColumnIndex == 9)
					{
						url = string.Format("mailto:{0}", url);
					}
					Process.Start(new ProcessStartInfo(url));
				}
				catch
				{
				}
			}
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x0000A514 File Offset: 0x00008714
		private void dgvResults_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.RowIndex > 0 && (e.ColumnIndex == 9 || e.ColumnIndex == 10 || e.ColumnIndex == 13 || e.ColumnIndex == 14) && this.dgvResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null && this.dgvResults.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString() != "")
			{
				this.Cursor = Cursors.Hand;
				return;
			}
			this.Cursor = Cursors.Default;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000A5D5 File Offset: 0x000087D5
		private void MainWindow_Load(object sender, EventArgs e)
		{
			this.LoadDataGridViewData();
		}

		// Token: 0x04000071 RID: 113
		public static ConnectionSettings DbConfig;

		// Token: 0x04000072 RID: 114
		public static DataRepository AppDatabase;

		// Token: 0x04000073 RID: 115
		private BackgroundWorker _scrapingWorker;
	}
}
