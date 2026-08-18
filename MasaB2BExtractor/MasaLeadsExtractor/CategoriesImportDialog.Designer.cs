namespace MasaLeadsExtractor
{
	// Token: 0x0200001F RID: 31
	public partial class CategoriesImportDialog : global::System.Windows.Forms.Form
	{
		// Token: 0x060000F6 RID: 246 RVA: 0x00010644 File Offset: 0x0000E844
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00010664 File Offset: 0x0000E864
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(global::MasaLeadsExtractor.CategoriesImportDialog));
			this.btnCancel = new global::System.Windows.Forms.Button();
			this.btnApply = new global::System.Windows.Forms.Button();
			this.label1 = new global::System.Windows.Forms.Label();
			this.btnLoadFile = new global::System.Windows.Forms.Button();
			this.tbUploadCategories = new global::System.Windows.Forms.TextBox();
			this.label2 = new global::System.Windows.Forms.Label();
			base.SuspendLayout();
			this.btnCancel.Anchor = global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Right;
			this.btnCancel.Image = (global::System.Drawing.Image)resources.GetObject("btnCancelImage");
			this.btnCancel.ImageAlign = global::System.Drawing.ContentAlignment.MiddleLeft;
			this.btnCancel.Location = new global::System.Drawing.Point(405, 226);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new global::System.Drawing.Size(67, 23);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.TextAlign = global::System.Drawing.ContentAlignment.MiddleRight;
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new global::System.EventHandler(this.btnCancel_Click);
			this.btnApply.Anchor = global::System.Windows.Forms.AnchorStyles.Bottom | global::System.Windows.Forms.AnchorStyles.Right;
			this.btnApply.Image = (global::System.Drawing.Image)resources.GetObject("btnApplyImage");
			this.btnApply.ImageAlign = global::System.Drawing.ContentAlignment.MiddleLeft;
			this.btnApply.Location = new global::System.Drawing.Point(349, 226);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new global::System.Drawing.Size(50, 23);
			this.btnApply.TabIndex = 4;
			this.btnApply.Text = "Ok";
			this.btnApply.TextAlign = global::System.Drawing.ContentAlignment.MiddleRight;
			this.btnApply.UseVisualStyleBackColor = true;
			this.btnApply.Click += new global::System.EventHandler(this.btnApply_Click);
			this.label1.Location = new global::System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new global::System.Drawing.Size(342, 30);
			this.label1.TabIndex = 6;
			this.label1.Text = "You can upload multiple categories at one time. Provide a text file with one category per line or enter a list of categories in the field below.\r\n";
			this.btnLoadFile.Location = new global::System.Drawing.Point(397, 9);
			this.btnLoadFile.Name = "btnLoadFile";
			this.btnLoadFile.Size = new global::System.Drawing.Size(75, 23);
			this.btnLoadFile.TabIndex = 7;
			this.btnLoadFile.Text = "Upload file";
			this.btnLoadFile.UseVisualStyleBackColor = true;
			this.btnLoadFile.Click += new global::System.EventHandler(this.btnLoadFile_Click);
			this.tbUploadCategories.Location = new global::System.Drawing.Point(15, 70);
			this.tbUploadCategories.Multiline = true;
			this.tbUploadCategories.Name = "tbUploadCategories";
			this.tbUploadCategories.ScrollBars = global::System.Windows.Forms.ScrollBars.Vertical;
			this.tbUploadCategories.Size = new global::System.Drawing.Size(457, 150);
			this.tbUploadCategories.TabIndex = 8;
			this.label2.AutoSize = true;
			this.label2.Location = new global::System.Drawing.Point(12, 54);
			this.label2.Name = "label2";
			this.label2.Size = new global::System.Drawing.Size(57, 13);
			this.label2.TabIndex = 9;
			this.label2.Text = "Categories";
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(484, 261);
			base.Controls.Add(this.label2);
			base.Controls.Add(this.tbUploadCategories);
			base.Controls.Add(this.btnLoadFile);
			base.Controls.Add(this.label1);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnApply);
			base.Icon = (global::System.Drawing.Icon)resources.GetObject("appIcon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "CategoriesImportDialog";
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Upload categories";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000139 RID: 313
		private global::System.ComponentModel.IContainer components;

		// Token: 0x0400013A RID: 314
		private global::System.Windows.Forms.Button btnCancel;

		// Token: 0x0400013B RID: 315
		private global::System.Windows.Forms.Button btnApply;

		// Token: 0x0400013C RID: 316
		private global::System.Windows.Forms.Label label1;

		// Token: 0x0400013D RID: 317
		private global::System.Windows.Forms.Button btnLoadFile;

		// Token: 0x0400013E RID: 318
		public global::System.Windows.Forms.TextBox tbUploadCategories;

		// Token: 0x0400013F RID: 319
		private global::System.Windows.Forms.Label label2;
	}
}
