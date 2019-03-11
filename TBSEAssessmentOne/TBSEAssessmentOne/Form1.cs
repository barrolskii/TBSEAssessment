using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace TBSEAssessmentOne
{
	public partial class Form1 : Form
	{
		public Form1()
		{
			InitializeComponent();

			dataGridView1.ColumnCount = 2;
			dataGridView1.Columns[0].Name = "StoreCode";
			dataGridView1.Columns[1].Name = "StoreName";
		}

		private void button1_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
			openFileDialog.Filter = "CSV files|*.csv";
			openFileDialog.Title = "File";

			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				MessageBox.Show("foo");
				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();
				richTextBox2.Text = openFileDialog.FileName;

				Dictionary<string, Store> Stores = new Dictionary<string, Store>();
				HashSet<Date> dates = new HashSet<Date>();
				List<Order> orders = new List<Order>();

				string[] storeList = File.ReadAllLines(openFileDialog.FileName);

				foreach (var s in storeList)
				{
					string[] splitStoreData = s.Split(',');

					Store newStore = new Store { storeCode = splitStoreData[0], storeLocation = splitStoreData[1] };
					if (!Stores.ContainsKey(newStore.storeCode))
						Stores.Add(newStore.storeCode, newStore);
				}

				string[] fileNames = Directory.GetFiles(openFileDialog.InitialDirectory + @"/StoreData");
				foreach (var filePath in fileNames)
				{
					string fileNameExt = Path.GetFileName(filePath);
					string fileName = Path.GetFileNameWithoutExtension(filePath);

					string[] fileNameSplit = fileName.Split('_');
					Store store = Stores[fileNameSplit[0]];
					Date date = new Date { week = Convert.ToInt32(fileNameSplit[1]), year = Convert.ToInt32(fileNameSplit[2]) };
					dates.Add(date);

					string[] orderData = File.ReadAllLines(openFileDialog.InitialDirectory + @"/StoreData/" + fileNameExt);
					foreach (var orderInfo in orderData)
					{
						string[] orderSplit = orderInfo.Split(',');
						Order order = new Order
						{
							store = store,

							date = date,
							supplier = orderSplit[0],
							supplierType = orderSplit[1],
							cost = Convert.ToDouble(orderSplit[2])
						};

						orders.Add(order);
					}
				}


				stopwatch.Stop();
				textBox1.Text += "Time to load: " + stopwatch.Elapsed;
			}
		}

		class Store
		{
			public string storeCode { get; set; }
			public string storeLocation { get; set; }
		}

		class Order
		{
			public Store store { get; set; }

			public Date date { get; set; }
			public string supplier { get; set; }
			public string supplierType { get; set; }
			public double cost { get; set; }
		}

		class Date
		{
			public int week { get; set; }
			public int year { get; set; }
		}

		private void richTextBox1_TextChanged(object sender, EventArgs e)
		{
			
		}
	}
}
