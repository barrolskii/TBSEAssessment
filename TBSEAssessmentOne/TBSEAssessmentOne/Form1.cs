using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TBSEAssessmentOne
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory() + "\\StoreData";
			openFileDialog.Filter = "CSV files|*.csv";
			openFileDialog.Title = "File";

			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				MessageBox.Show(openFileDialog.FileName);
			}
		}
	}
}
