using System;
using System.Drawing;
using System.Windows.Forms;

namespace MasaLeadsExtractor
{
	// Token: 0x02000018 RID: 24
	public static class DarkTheme
	{
		// Token: 0x060000B9 RID: 185 RVA: 0x0000CB68 File Offset: 0x0000AD68
		public static void ApplyDarkTheme(Form form)
		{
			form.BackColor = DarkTheme.DarkBackground;
			form.ForeColor = DarkTheme.TextColor;
			DarkTheme.ApplyToControls(form.Controls);
		}

		// Token: 0x060000BA RID: 186 RVA: 0x0000CB8C File Offset: 0x0000AD8C
		private static void ApplyToControls(Control.ControlCollection controls)
		{
			foreach (object obj in controls)
			{
				Control control = (Control)obj;
				if (control.Controls.Count > 0)
				{
					DarkTheme.ApplyToControls(control.Controls);
				}
				DataGridView dgv = control as DataGridView;
				if (dgv != null)
				{
					DarkTheme.StyleDataGridView(dgv);
				}
				else
				{
					Button btn = control as Button;
					if (btn != null)
					{
						DarkTheme.StyleButton(btn);
					}
					else
					{
						TextBox txt = control as TextBox;
						if (txt != null)
						{
							DarkTheme.StyleTextBox(txt);
						}
						else
						{
							ComboBox cmb = control as ComboBox;
							if (cmb != null)
							{
								DarkTheme.StyleComboBox(cmb);
							}
							else
							{
								CheckedListBox clb = control as CheckedListBox;
								if (clb != null)
								{
									DarkTheme.StyleCheckedListBox(clb);
								}
								else
								{
									GroupBox gb = control as GroupBox;
									if (gb != null)
									{
										DarkTheme.StyleGroupBox(gb);
									}
									else
									{
										Panel pnl = control as Panel;
										if (pnl != null)
										{
											DarkTheme.StylePanel(pnl);
										}
										else
										{
											MenuStrip ms = control as MenuStrip;
											if (ms != null)
											{
												DarkTheme.StyleMenuStrip(ms);
											}
											else
											{
												StatusStrip ss = control as StatusStrip;
												if (ss != null)
												{
													DarkTheme.StyleStatusStrip(ss);
												}
												else
												{
													SplitContainer sc = control as SplitContainer;
													if (sc != null)
													{
														DarkTheme.StyleSplitContainer(sc);
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x060000BB RID: 187 RVA: 0x0000CCDC File Offset: 0x0000AEDC
		private static void StyleDataGridView(DataGridView dgv)
		{
			dgv.BackgroundColor = DarkTheme.DarkControl;
			dgv.ForeColor = DarkTheme.TextColor;
			dgv.GridColor = DarkTheme.GridLineColor;
			dgv.BorderStyle = BorderStyle.None;
			dgv.EnableHeadersVisualStyles = false;
			dgv.ColumnHeadersDefaultCellStyle.BackColor = DarkTheme.DarkPanel;
			dgv.ColumnHeadersDefaultCellStyle.ForeColor = DarkTheme.TextColor;
			dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = DarkTheme.DarkPanel;
			dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = DarkTheme.TextColor;
			dgv.ColumnHeadersDefaultCellStyle.Font = new Font(dgv.Font, FontStyle.Bold);
			dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
			dgv.RowHeadersDefaultCellStyle.BackColor = DarkTheme.DarkPanel;
			dgv.RowHeadersDefaultCellStyle.ForeColor = DarkTheme.TextColor;
			dgv.RowHeadersDefaultCellStyle.SelectionBackColor = DarkTheme.DarkPanel;
			dgv.DefaultCellStyle.BackColor = DarkTheme.DarkControl;
			dgv.DefaultCellStyle.ForeColor = DarkTheme.TextColor;
			dgv.DefaultCellStyle.SelectionBackColor = DarkTheme.SelectionColor;
			dgv.DefaultCellStyle.SelectionForeColor = DarkTheme.TextColor;
			dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(38, 44, 39);
			dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
		}

		// Token: 0x060000BC RID: 188 RVA: 0x0000CE04 File Offset: 0x0000B004
		private static void StyleButton(Button btn)
		{
			btn.BackColor = DarkTheme.DarkControl;
			btn.ForeColor = DarkTheme.TextColor;
			btn.FlatStyle = FlatStyle.Flat;
			btn.FlatAppearance.BorderColor = DarkTheme.BorderColor;
			btn.FlatAppearance.BorderSize = 1;
			btn.FlatAppearance.MouseOverBackColor = DarkTheme.AccentGreen;
			btn.FlatAppearance.MouseDownBackColor = DarkTheme.AccentGreenHover;
			btn.Cursor = Cursors.Hand;
		}

		// Token: 0x060000BD RID: 189 RVA: 0x0000CE75 File Offset: 0x0000B075
		private static void StyleTextBox(TextBox txt)
		{
			txt.BackColor = DarkTheme.DarkControl;
			txt.ForeColor = DarkTheme.TextColor;
			txt.BorderStyle = BorderStyle.FixedSingle;
		}

		// Token: 0x060000BE RID: 190 RVA: 0x0000CE94 File Offset: 0x0000B094
		private static void StyleComboBox(ComboBox cmb)
		{
			cmb.BackColor = DarkTheme.DarkControl;
			cmb.ForeColor = DarkTheme.TextColor;
			cmb.FlatStyle = FlatStyle.Flat;
		}

		// Token: 0x060000BF RID: 191 RVA: 0x0000CEB3 File Offset: 0x0000B0B3
		private static void StyleCheckedListBox(CheckedListBox clb)
		{
			clb.BackColor = DarkTheme.DarkControl;
			clb.ForeColor = DarkTheme.TextColor;
			clb.BorderStyle = BorderStyle.FixedSingle;
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x0000CED2 File Offset: 0x0000B0D2
		private static void StyleGroupBox(GroupBox gb)
		{
			gb.BackColor = DarkTheme.DarkBackground;
			gb.ForeColor = DarkTheme.AccentGreen;
		}

		// Token: 0x060000C1 RID: 193 RVA: 0x0000CEEA File Offset: 0x0000B0EA
		private static void StylePanel(Panel pnl)
		{
			pnl.BackColor = DarkTheme.DarkBackground;
		}

		// Token: 0x060000C2 RID: 194 RVA: 0x0000CEF7 File Offset: 0x0000B0F7
		private static void StyleMenuStrip(MenuStrip ms)
		{
			ms.BackColor = DarkTheme.DarkPanel;
			ms.ForeColor = DarkTheme.TextColor;
			ms.Renderer = new DarkTheme.DarkMenuRenderer();
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x0000CEF7 File Offset: 0x0000B0F7
		private static void StyleStatusStrip(StatusStrip ss)
		{
			ss.BackColor = DarkTheme.DarkPanel;
			ss.ForeColor = DarkTheme.TextColor;
			ss.Renderer = new DarkTheme.DarkMenuRenderer();
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x0000CF1A File Offset: 0x0000B11A
		private static void StyleSplitContainer(SplitContainer sc)
		{
			sc.BackColor = DarkTheme.BorderColor;
			sc.Panel1.BackColor = DarkTheme.DarkBackground;
			sc.Panel2.BackColor = DarkTheme.DarkBackground;
		}

		// Token: 0x040000BA RID: 186
		public static readonly Color DarkBackground = Color.FromArgb(30, 36, 32);

		// Token: 0x040000BB RID: 187
		public static readonly Color DarkControl = Color.FromArgb(45, 52, 47);

		// Token: 0x040000BC RID: 188
		public static readonly Color DarkPanel = Color.FromArgb(22, 26, 23);

		// Token: 0x040000BD RID: 189
		public static readonly Color LightGray = Color.FromArgb(100, 110, 102);

		// Token: 0x040000BE RID: 190
		public static readonly Color MediumGray = Color.FromArgb(70, 78, 72);

		// Token: 0x040000BF RID: 191
		public static readonly Color TextColor = Color.FromArgb(220, 230, 222);

		// Token: 0x040000C0 RID: 192
		public static readonly Color AccentGreen = Color.FromArgb(0, 166, 81);

		// Token: 0x040000C1 RID: 193
		public static readonly Color AccentGreenHover = Color.FromArgb(0, 200, 98);

		// Token: 0x040000C2 RID: 194
		public static readonly Color BorderColor = Color.FromArgb(55, 64, 57);

		// Token: 0x040000C3 RID: 195
		public static readonly Color GridLineColor = Color.FromArgb(40, 46, 41);

		// Token: 0x040000C4 RID: 196
		public static readonly Color SelectionColor = Color.FromArgb(0, 100, 55);

		// Token: 0x040000C5 RID: 197
		public static readonly Color WarningRed = Color.FromArgb(220, 50, 47);

		// Token: 0x040000C6 RID: 198
		public static readonly Color SuccessGreen = Color.FromArgb(80, 200, 120);

		// Token: 0x0200003F RID: 63
		private class DarkMenuRenderer : ToolStripProfessionalRenderer
		{
			// Token: 0x06000130 RID: 304 RVA: 0x000117EE File Offset: 0x0000F9EE
			public DarkMenuRenderer()
				: base(new DarkTheme.DarkColorTable())
			{
			}

			// Token: 0x06000131 RID: 305 RVA: 0x00007925 File Offset: 0x00005B25
			protected override void OnRenderToolStripBorder(ToolStripRenderEventArgs e)
			{
			}
		}

		// Token: 0x02000040 RID: 64
		private class DarkColorTable : ProfessionalColorTable
		{
			// Token: 0x1700000E RID: 14
			// (get) Token: 0x06000132 RID: 306 RVA: 0x000117FB File Offset: 0x0000F9FB
			public override Color MenuItemSelected
			{
				get
				{
					return DarkTheme.AccentGreen;
				}
			}

			// Token: 0x1700000F RID: 15
			// (get) Token: 0x06000133 RID: 307 RVA: 0x000117FB File Offset: 0x0000F9FB
			public override Color MenuItemSelectedGradientBegin
			{
				get
				{
					return DarkTheme.AccentGreen;
				}
			}

			// Token: 0x17000010 RID: 16
			// (get) Token: 0x06000134 RID: 308 RVA: 0x000117FB File Offset: 0x0000F9FB
			public override Color MenuItemSelectedGradientEnd
			{
				get
				{
					return DarkTheme.AccentGreen;
				}
			}

			// Token: 0x17000011 RID: 17
			// (get) Token: 0x06000135 RID: 309 RVA: 0x00011802 File Offset: 0x0000FA02
			public override Color MenuItemPressedGradientBegin
			{
				get
				{
					return DarkTheme.AccentGreenHover;
				}
			}

			// Token: 0x17000012 RID: 18
			// (get) Token: 0x06000136 RID: 310 RVA: 0x00011802 File Offset: 0x0000FA02
			public override Color MenuItemPressedGradientEnd
			{
				get
				{
					return DarkTheme.AccentGreenHover;
				}
			}

			// Token: 0x17000013 RID: 19
			// (get) Token: 0x06000137 RID: 311 RVA: 0x00011809 File Offset: 0x0000FA09
			public override Color MenuItemBorder
			{
				get
				{
					return DarkTheme.BorderColor;
				}
			}

			// Token: 0x17000014 RID: 20
			// (get) Token: 0x06000138 RID: 312 RVA: 0x00011809 File Offset: 0x0000FA09
			public override Color MenuBorder
			{
				get
				{
					return DarkTheme.BorderColor;
				}
			}

			// Token: 0x17000015 RID: 21
			// (get) Token: 0x06000139 RID: 313 RVA: 0x00011810 File Offset: 0x0000FA10
			public override Color ToolStripDropDownBackground
			{
				get
				{
					return DarkTheme.DarkPanel;
				}
			}

			// Token: 0x17000016 RID: 22
			// (get) Token: 0x0600013A RID: 314 RVA: 0x00011810 File Offset: 0x0000FA10
			public override Color ImageMarginGradientBegin
			{
				get
				{
					return DarkTheme.DarkPanel;
				}
			}

			// Token: 0x17000017 RID: 23
			// (get) Token: 0x0600013B RID: 315 RVA: 0x00011810 File Offset: 0x0000FA10
			public override Color ImageMarginGradientMiddle
			{
				get
				{
					return DarkTheme.DarkPanel;
				}
			}

			// Token: 0x17000018 RID: 24
			// (get) Token: 0x0600013C RID: 316 RVA: 0x00011810 File Offset: 0x0000FA10
			public override Color ImageMarginGradientEnd
			{
				get
				{
					return DarkTheme.DarkPanel;
				}
			}

			// Token: 0x17000019 RID: 25
			// (get) Token: 0x0600013D RID: 317 RVA: 0x00011810 File Offset: 0x0000FA10
			public override Color MenuStripGradientBegin
			{
				get
				{
					return DarkTheme.DarkPanel;
				}
			}

			// Token: 0x1700001A RID: 26
			// (get) Token: 0x0600013E RID: 318 RVA: 0x00011810 File Offset: 0x0000FA10
			public override Color MenuStripGradientEnd
			{
				get
				{
					return DarkTheme.DarkPanel;
				}
			}

			// Token: 0x1700001B RID: 27
			// (get) Token: 0x0600013F RID: 319 RVA: 0x00011810 File Offset: 0x0000FA10
			public override Color StatusStripGradientBegin
			{
				get
				{
					return DarkTheme.DarkPanel;
				}
			}

			// Token: 0x1700001C RID: 28
			// (get) Token: 0x06000140 RID: 320 RVA: 0x00011810 File Offset: 0x0000FA10
			public override Color StatusStripGradientEnd
			{
				get
				{
					return DarkTheme.DarkPanel;
				}
			}
		}
	}
}
