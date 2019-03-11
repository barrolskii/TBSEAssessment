namespace TBSEAssessmentOne
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.button1 = new System.Windows.Forms.Button();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.dataGridView1 = new System.Windows.Forms.DataGridView();
			this.richTextBox2 = new System.Windows.Forms.RichTextBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
			this.SuspendLayout();
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(791, 110);
			this.button1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(145, 42);
			this.button1.TabIndex = 0;
			this.button1.Text = "Load file";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// richTextBox1
			// 
			this.richTextBox1.Location = new System.Drawing.Point(12, 12);
			this.richTextBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.Size = new System.Drawing.Size(311, 545);
			this.richTextBox1.TabIndex = 1;
			this.richTextBox1.Text = "";
			this.richTextBox1.TextChanged += new System.EventHandler(this.richTextBox1_TextChanged);
			// 
			// dataGridView1
			// 
			this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dataGridView1.Location = new System.Drawing.Point(340, 110);
			this.dataGridView1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.dataGridView1.Name = "dataGridView1";
			this.dataGridView1.RowTemplate.Height = 24;
			this.dataGridView1.Size = new System.Drawing.Size(435, 198);
			this.dataGridView1.TabIndex = 2;
			// 
			// richTextBox2
			// 
			this.richTextBox2.Location = new System.Drawing.Point(340, 12);
			this.richTextBox2.Name = "richTextBox2";
			this.richTextBox2.Size = new System.Drawing.Size(735, 73);
			this.richTextBox2.TabIndex = 3;
			this.richTextBox2.Text = "";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(835, 524);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(240, 22);
			this.textBox1.TabIndex = 4;
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1087, 568);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.richTextBox2);
			this.Controls.Add(this.dataGridView1);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.button1);
			this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
			this.Name = "Form1";
			this.Text = "Form1";
			((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.DataGridView dataGridView1;
		private System.Windows.Forms.RichTextBox richTextBox2;
		private System.Windows.Forms.TextBox textBox1;
	}
}

