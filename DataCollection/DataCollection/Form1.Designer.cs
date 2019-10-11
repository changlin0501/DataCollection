namespace DataCollection
{
	partial class Form1
	{
		/// <summary>
		/// 必需的设计器变量。
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// 清理所有正在使用的资源。
		/// </summary>
		/// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows 窗体设计器生成的代码

		/// <summary>
		/// 设计器支持所需的方法 - 不要修改
		/// 使用代码编辑器修改此方法的内容。
		/// </summary>
		private void InitializeComponent()
		{
			this.BtnPublish = new System.Windows.Forms.Button();
			this.txtSendMessage = new System.Windows.Forms.TextBox();
			this.txtPubTopic = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// BtnPublish
			// 
			this.BtnPublish.Location = new System.Drawing.Point(223, 331);
			this.BtnPublish.Margin = new System.Windows.Forms.Padding(4);
			this.BtnPublish.Name = "BtnPublish";
			this.BtnPublish.Size = new System.Drawing.Size(127, 45);
			this.BtnPublish.TabIndex = 10;
			this.BtnPublish.Text = "发布";
			this.BtnPublish.UseVisualStyleBackColor = true;
			this.BtnPublish.Click += new System.EventHandler(this.BtnPublish_Click);
			// 
			// txtSendMessage
			// 
			this.txtSendMessage.Location = new System.Drawing.Point(27, 99);
			this.txtSendMessage.Margin = new System.Windows.Forms.Padding(4);
			this.txtSendMessage.Multiline = true;
			this.txtSendMessage.Name = "txtSendMessage";
			this.txtSendMessage.Size = new System.Drawing.Size(565, 203);
			this.txtSendMessage.TabIndex = 9;
			this.txtSendMessage.Text = "{\tGalvanicCurrent:temperature1}";
			// 
			// txtPubTopic
			// 
			this.txtPubTopic.Location = new System.Drawing.Point(108, 38);
			this.txtPubTopic.Margin = new System.Windows.Forms.Padding(4);
			this.txtPubTopic.Name = "txtPubTopic";
			this.txtPubTopic.Size = new System.Drawing.Size(489, 25);
			this.txtPubTopic.TabIndex = 8;
			this.txtPubTopic.Text = "v1/devices/me/telemetry";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(24, 41);
			this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 15);
			this.label2.TabIndex = 7;
			this.label2.Text = "发布主题";
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(610, 415);
			this.Controls.Add(this.BtnPublish);
			this.Controls.Add(this.txtSendMessage);
			this.Controls.Add(this.txtPubTopic);
			this.Controls.Add(this.label2);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button BtnPublish;
		private System.Windows.Forms.TextBox txtSendMessage;
		private System.Windows.Forms.TextBox txtPubTopic;
		private System.Windows.Forms.Label label2;
	}
}

