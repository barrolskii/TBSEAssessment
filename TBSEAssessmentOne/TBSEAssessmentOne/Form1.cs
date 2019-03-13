using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace TBSEAssessmentOne
{
	public partial class Form1 : Form
	{
		string storeCSVFileLocation;

		public Form1()
		{
			InitializeComponent();

			comboBox1.Enabled = false;
			comboBox2.Enabled = false;
			comboBox3.Enabled = false;

			/* Populate the second combo box to have a selection of weeks */
			for (int i = 1; i <= 52; i++)
				comboBox2.Items.Add(i);

			/* Only two years to choose from in the files so this combo box
			 * only needs two options
			 */
			comboBox3.Items.Add(2013);
			comboBox3.Items.Add(2014);

			dataGridView1.ColumnCount = 2;
			dataGridView1.Columns[0].Name = "StoreCode";
			dataGridView1.Columns[1].Name = "StoreName";

			dataGridView2.ColumnCount = 3;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
			openFileDialog.Filter = "CSV files|*.csv";
			openFileDialog.Title = "File";

			int fileCount = 0;

			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
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

					dataGridView1.Rows.Add(newStore.storeCode, newStore.storeLocation);
					comboBox1.Items.Add(newStore.storeCode);
				}

				/* When the store list file is loaded in the folder containing the individual store
				 * files will be in the same directory as it 
				 */
				storeCSVFileLocation = openFileDialog.InitialDirectory + @"/StoreData/";

				string[] fileNames = Directory.GetFiles(openFileDialog.InitialDirectory + @"/StoreData");
				ConcurrentQueue<string> foo = new ConcurrentQueue<string>();

				Stopwatch stopwatch = new Stopwatch();
				stopwatch.Start();

				ConcurrentQueue<string> queue = new ConcurrentQueue<string>();
				string[] testOrderData = File.ReadAllLines(openFileDialog.InitialDirectory + @"/StoreData/ABE1_1_2013.csv");
				Parallel.ForEach(testOrderData, data =>
				{
					queue.Enqueue(data);
				});

				foreach (var s in queue)
				{
					string[] orderSplit = s.Split(',');
				}


				stopwatch.Stop();
				textBox1.Text += "Time to load: " + stopwatch.Elapsed;
				richTextBox1.Text += "File count = " + fileCount + '\n';
				richTextBox1.Text += "Queue count = " + foo.Count+ '\n';

				/* Enable combo boxes for search queries*/
				comboBox1.Enabled = true;
				comboBox2.Enabled = true;
				comboBox3.Enabled = true;
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

		private void button2_Click(object sender, EventArgs e)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			string fileToSearch = storeCSVFileLocation + comboBox1.Text + '_' + comboBox2.Text + '_' + comboBox3.Text + ".csv";
			string[] storeData = File.ReadAllLines(fileToSearch);
			ConcurrentQueue<string> queue = new ConcurrentQueue<string>();

			Parallel.ForEach(storeData, data =>
			{
				queue.Enqueue(data);
			});

			if (dataGridView2.Rows.Count >= 2)
			{
				for (int i = 0; i < dataGridView2.Rows.Count - 1; i++)
				{
					dataGridView2.Rows.RemoveAt(i);
				}
			}

			foreach (var s in queue)
			{
				string[] orderSplit = s.Split(',');
				dataGridView2.Rows.Add(orderSplit[0], orderSplit[1], orderSplit[2]);
			}

			sw.Stop();
			richTextBox1.Text += "Time: " + sw.Elapsed + '\n';
		}
	}
}