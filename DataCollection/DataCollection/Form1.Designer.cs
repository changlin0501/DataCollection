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
			this.txt_wendu = new System.Windows.Forms.TextBox();
			this.text_yali = new System.Windows.Forms.TextBox();
			this.text_dianya = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.text_yaliB = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.text_dianliu = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// txt_wendu
			// 
			this.txt_wendu.Location = new System.Drawing.Point(292, 52);
			this.txt_wendu.Name = "txt_wendu";
			this.txt_wendu.Size = new System.Drawing.Size(100, 25);
			this.txt_wendu.TabIndex = 0;
			// 
			// text_yali
			// 
			this.text_yali.Location = new System.Drawing.Point(292, 110);
			this.text_yali.Name = "text_yali";
			this.text_yali.Size = new System.Drawing.Size(100, 25);
			this.text_yali.TabIndex = 1;
			// 
			// text_dianya
			// 
			this.text_dianya.Location = new System.Drawing.Point(292, 229);
			this.text_dianya.Name = "text_dianya";
			this.text_dianya.Size = new System.Drawing.Size(100, 25);
			this.text_dianya.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(152, 62);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(37, 15);
			this.label1.TabIndex = 2;
			this.label1.Text = "温度";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(152, 120);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(45, 15);
			this.label2.TabIndex = 2;
			this.label2.Text = "压力A";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(152, 232);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(37, 15);
			this.label3.TabIndex = 2;
			this.label3.Text = "电压";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(152, 175);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(45, 15);
			this.label4.TabIndex = 2;
			this.label4.Text = "压力B";
			// 
			// text_yaliB
			// 
			this.text_yaliB.Location = new System.Drawing.Point(292, 165);
			this.text_yaliB.Name = "text_yaliB";
			this.text_yaliB.Size = new System.Drawing.Size(100, 25);
			this.text_yaliB.TabIndex = 1;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(152, 301);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(37, 15);
			this.label5.TabIndex = 2;
			this.label5.Text = "电流";
			// 
			// text_dianliu
			// 
			this.text_dianliu.Location = new System.Drawing.Point(292, 296);
			this.text_dianliu.Name = "text_dianliu";
			this.text_dianliu.Size = new System.Drawing.Size(100, 25);
			this.text_dianliu.TabIndex = 1;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(610, 415);
			this.Controls.Add(this.text_dianliu);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.text_yaliB);
			this.Controls.Add(this.text_dianya);
			this.Controls.Add(this.text_yali);
			this.Controls.Add(this.txt_wendu);
			this.Name = "Form1";
			this.Text = "Form1";
			this.Load += new System.EventHandler(this.Form1_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox txt_wendu;
		private System.Windows.Forms.TextBox text_yali;
		private System.Windows.Forms.TextBox text_dianya;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox text_yaliB;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox text_dianliu;
	}
}

