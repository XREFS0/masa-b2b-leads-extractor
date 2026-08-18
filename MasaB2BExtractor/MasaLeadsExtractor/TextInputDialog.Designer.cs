namespace MasaLeadsExtractor
{
	// Token: 0x02000013 RID: 19
	public partial class TextInputDialog : global::System.Windows.Forms.Form
	{
		// Token: 0x06000061 RID: 97 RVA: 0x000061B7 File Offset: 0x000043B7
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000062 RID: 98 RVA: 0x000061D8 File Offset: 0x000043D8
		private void InitializeComponent()
		{
			global::System.ComponentModel.ComponentResourceManager resources = new global::System.ComponentModel.ComponentResourceManager(typeof(global::MasaLeadsExtractor.TextInputDialog));
			this.lblPrompt = new global::System.Windows.Forms.Label();
			this.tbValue = new global::System.Windows.Forms.TextBox();
			this.btnApply = new global::System.Windows.Forms.Button();
			this.btnCancel = new global::System.Windows.Forms.Button();
			base.SuspendLayout();
			this.lblPrompt.AutoSize = true;
			this.lblPrompt.Location = new global::System.Drawing.Point(12, 6);
			this.lblPrompt.Name = "lblPrompt";
			this.lblPrompt.Size = new global::System.Drawing.Size(27, 13);
			this.lblPrompt.TabIndex = 0;
			this.lblPrompt.Text = "Title";
			this.tbValue.Location = new global::System.Drawing.Point(12, 22);
			this.tbValue.Name = "tbValue";
			this.tbValue.Size = new global::System.Drawing.Size(260, 20);
			this.tbValue.TabIndex = 1;
			this.tbValue.KeyUp += new global::System.Windows.Forms.KeyEventHandler(this.tbValue_KeyUp);
			this.btnApply.Image = (global::System.Drawing.Image)resources.GetObject("btnApplyImage");
			this.btnApply.ImageAlign = global::System.Drawing.ContentAlignment.MiddleLeft;
			this.btnApply.Location = new global::System.Drawing.Point(149, 46);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new global::System.Drawing.Size(50, 23);
			this.btnApply.TabIndex = 2;
			this.btnApply.Text = "Ok";
			this.btnApply.TextAlign = global::System.Drawing.ContentAlignment.MiddleRight;
			this.btnApply.UseVisualStyleBackColor = true;
			this.btnApply.Click += new global::System.EventHandler(this.btnApply_Click);
			this.btnCancel.Image = (global::System.Drawing.Image)resources.GetObject("btnCancelImage");
			this.btnCancel.ImageAlign = global::System.Drawing.ContentAlignment.MiddleLeft;
			this.btnCancel.Location = new global::System.Drawing.Point(205, 46);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new global::System.Drawing.Size(67, 23);
			this.btnCancel.TabIndex = 3;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.TextAlign = global::System.Drawing.ContentAlignment.MiddleRight;
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new global::System.EventHandler(this.btnCancel_Click);
			base.AutoScaleDimensions = new global::System.Drawing.SizeF(6f, 13f);
			base.AutoScaleMode = global::System.Windows.Forms.AutoScaleMode.Font;
			base.ClientSize = new global::System.Drawing.Size(284, 71);
			base.Controls.Add(this.btnCancel);
			base.Controls.Add(this.btnApply);
			base.Controls.Add(this.tbValue);
			base.Controls.Add(this.lblPrompt);
			base.FormBorderStyle = global::System.Windows.Forms.FormBorderStyle.FixedSingle;
			base.Icon = (global::System.Drawing.Icon)resources.GetObject("appIcon");
			base.MaximizeBox = false;
			base.MinimizeBox = false;
			base.Name = "TextInputDialog";
			base.StartPosition = global::System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "TextInputDialog";
			base.ResumeLayout(false);
			base.PerformLayout();
		}

		// Token: 0x04000043 RID: 67
		private global::System.ComponentModel.IContainer components;

		// Token: 0x04000044 RID: 68
		private global::System.Windows.Forms.Label lblPrompt;

		// Token: 0x04000045 RID: 69
		private global::System.Windows.Forms.TextBox tbValue;

		// Token: 0x04000046 RID: 70
		private global::System.Windows.Forms.Button btnApply;

		// Token: 0x04000047 RID: 71
		private global::System.Windows.Forms.Button btnCancel;
	}
}
