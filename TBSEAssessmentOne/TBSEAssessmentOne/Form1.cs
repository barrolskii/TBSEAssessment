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
		Task t1;

		public Form1()
		{
			InitializeComponent();

			t1 = new Task(() => ReadAllData());

			/* Disable use of combo boxes until data is loaded in */
			comboBox1.Enabled = false;
			comboBox2.Enabled = false;
			comboBox3.Enabled = false;

			/* Populate the second combo box to have a selection of weeks */
			for (int i = 1; i <= 52; i++)
				comboBox2.Items.Add(i);

			/*
			 * Only two years to choose from in the files so this combo box
			 * only needs two options
			 */
			comboBox3.Items.Add(2013);
			comboBox3.Items.Add(2014);

			dataGridView1.ColumnCount = 2;
			dataGridView1.Columns[0].Name = "StoreCode";
			dataGridView1.Columns[1].Name = "StoreName";

			dataGridView2.ColumnCount = 3;
			dataGridView2.Columns[0].Name = "Supplier";
			dataGridView2.Columns[1].Name = "Supplier type";
			dataGridView2.Columns[2].Name = "Cost";
		}

		private void button1_Click(object sender, EventArgs e)
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
			openFileDialog.Filter = "CSV files|*.csv";
			openFileDialog.Title = "File";


			if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
			{
				richTextBox2.Text = openFileDialog.FileName;

				t1.Start();
			}
		}

		private void ReadAllData()
		{
			string folderPath = "StoreData";
			string storeCodesFile = "StoreCodes.csv";

			Dictionary<string, Store> Stores = new Dictionary<string, Store>();

			string storeCodesFilePath = Directory.GetCurrentDirectory() + @"\" + storeCodesFile;
			string[] storeList = File.ReadAllLines(storeCodesFilePath);

			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();

			foreach (var s in storeList)
			{
				string[] splitStoreData = s.Split(',');

				Store newStore = new Store { storeCode = splitStoreData[0], storeLocation = splitStoreData[1] };
				if (!Stores.ContainsKey(newStore.storeCode))
					Stores.Add(newStore.storeCode, newStore);

				dataGridView1.Invoke(new Action(() => dataGridView1.Rows.Add(newStore.storeCode, newStore.storeLocation)));
				comboBox1.Invoke(new Action(() => comboBox1.Items.Add(newStore.storeCode)));
			}

			string[] fileNames = Directory.GetFiles(folderPath);

			ConcurrentQueue<Date> queueDate = new ConcurrentQueue<Date>();
			ConcurrentQueue<Order> queueOrder = new ConcurrentQueue<Order>();

			Parallel.ForEach(fileNames, file =>
			{
				string fileName = Path.GetFileNameWithoutExtension(file);
				string[] fileNameSplit = fileName.Split('_');

				Store store = Stores[fileNameSplit[0]];
				Date date = new Date { week = Convert.ToInt32(fileNameSplit[1]), year = Convert.ToInt32(fileNameSplit[2]) };
				queueDate.Enqueue(date);

				string[] data = File.ReadAllLines(file);
				foreach (var s in data)
				{
					string[] fileData = s.Split(',');

					Order order = new Order
					{
						store = store,
						date = date,
						supplier = fileData[0],
						supplierType = fileData[1],
						cost = Convert.ToDouble(fileData[2])
					};

					queueOrder.Enqueue(order);
				}

			});


			List<string> suppliers = new List<string>();
			List<string> supplierTypes = new List<string>();

			foreach(var order in queueOrder)
			{
				if (!suppliers.Contains(order.supplier))
					suppliers.Add(order.supplier);

				if (!supplierTypes.Contains(order.supplierType))
					supplierTypes.Add(order.supplierType);
			}

			comboBox4.Invoke(new Action(() => 
			{
				foreach (var s in suppliers)
				{
					comboBox4.Items.Add(s);
				}
					
			}));

			comboBox5.Invoke(new Action(() =>
			{
				foreach (var s in supplierTypes)
				{
					comboBox5.Items.Add(s);
				}

			}));

			/* LINQ and PLINQ testing */

			double costs = queueOrder.Sum(num => num.cost); // 197186552.639997

			Date[] linqDates = (from date in queueDate
								where date.year == 2013
								select date).ToArray();

			Date[] plinqDates = (from date in queueDate.AsParallel()
								 where date.year == 2013
								 select date).ToArray();

			string supplier = "Cleaning"; // 18940385.4499995
			double supplierCost = queueOrder.Where(order => order.supplierType == supplier).Select(order => order.cost).Sum();
								  

			/* End of testing section */

			stopwatch.Stop();

			textBox3.Invoke(new Action(() => textBox3.Text = "Total cost: £" + costs));

			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Time to load: " + stopwatch.Elapsed + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Dictionary key count: " + Stores.Keys.Count + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Queue count: " + queueDate.Count + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Queue order count: " + queueOrder.Count + '\n'));

			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Total cost of orders: " + costs + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Supplier total: " + supplierCost + '\n'));

			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Total 2013 dates: " + linqDates.Length + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Total 2013 dates: " + plinqDates.Length + '\n'));


			/* Enable combo boxes for search queries*/
			comboBox1.Invoke(new Action(() => comboBox1.Enabled = true));
			comboBox2.Invoke(new Action(() => comboBox2.Enabled = true));
			comboBox3.Invoke(new Action(() => comboBox3.Enabled = true));
		}

		private void button2_Click(object sender, EventArgs e)
		{
			double totalCost = 0;

			Stopwatch sw = new Stopwatch();
			sw.Start();

			string fileToSearch = storeCSVFileLocation + "StoreData/" + comboBox1.Text + '_' + comboBox2.Text + '_' + comboBox3.Text + ".csv";
			string[] storeData = File.ReadAllLines(fileToSearch);

			/* If the cell rows already contain data then remove them */
			if (dataGridView2.Rows.Count >= 2)
			{
				richTextBox1.Text += "Rows begin deleted" + '\n';
				dataGridView2.Rows.Clear();
			}

			foreach (var s in storeData)
			{
				string[] orderSplit = s.Split(',');
				dataGridView2.Rows.Add(orderSplit[0], orderSplit[1], orderSplit[2]);
				totalCost += Convert.ToDouble(orderSplit[2]);
			};

			textBox2.Text = "Total cost: £" + totalCost;

			sw.Stop();

			/* Output for debgging purposes */
			richTextBox1.Text += "Time: " + sw.Elapsed + '\n';
			richTextBox1.Text += "Row count: " + dataGridView2.Rows.Count + '\n'; // 628
			richTextBox1.Text += "Total cost: " + totalCost + '\n'; // 22440.79
		}


		private void button3_Click(object sender, EventArgs e)
		{
			
		}

		private void button4_Click(object sender, EventArgs e)
		{
			//ReadAllFilesAndCountTotalCost();
			//ReadAllFilesInWeekRangeAndTotalCost();
			//TotalCostForAllOrdersForAStore();
			//CostOfAllOrdersToASupplier();
			CostOfAllOrdersToASupplierType();
			//CostOfAllOrdersInAWeekForASupplierType();
			//CostOfAllOrdersForASupplierTypeForAStore();
			//CostOfAllOrdersInAWeekForASupplierTypeForAStore();

			//t1.Start();
		}

		private void CostOfAllOrdersInAWeekForASupplierTypeForAStore()
		{
			string folderPath = "StoreData";
			string supplierType = "Cleaning";
			string store = "ABE1";
			string week = "1";
			double supplierTotal = 0.0;

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();


			string[] fileNames = Directory.GetFiles(folderPath);

			foreach (var filePath in fileNames)
			{
				string[] splitPath = filePath.Split('_');
				string[] splitName = splitPath[0].Split('\\');
				if (splitName[1] == store && splitPath[1] == week)
				{
					richTextBox4.Text += filePath + '\n';

					string fileNameExt = Path.GetFileName(filePath);
					string[] orderData = File.ReadAllLines(folderPath + @"\" + fileNameExt);

					foreach (var orderInfo in orderData)
					{
						string[] orderSplit = orderInfo.Split(',');
						if (orderSplit[1] == supplierType)
						{
							supplierTotal += Convert.ToDouble(orderSplit[2]);
						}
					}
				}
			}


			stopWatch.Stop();

			richTextBox4.Text += "TimeToLoad: " + stopWatch.Elapsed + '\n';
			richTextBox4.Text += "Total cost: " + supplierTotal + '\n';
		}

		private void CostOfAllOrdersForASupplierTypeForAStore()
		{
			string folderPath = "StoreData";
			string supplierType = "Cleaning";
			string store = "ABE1";
			double supplierTotal = 0.0;

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			string[] fileNames = Directory.GetFiles(folderPath);

			foreach (var filePath in fileNames)
			{
				string[] splitPath = filePath.Split('_');
				string[] splitName = splitPath[0].Split('\\');
				if (splitName[1] == store)
				{
					string fileNameExt = Path.GetFileName(filePath);
					string[] orderData = File.ReadAllLines(folderPath + @"\" + fileNameExt);

					foreach (var orderInfo in orderData)
					{
						string[] orderSplit = orderInfo.Split(',');
						if (orderSplit[1] == supplierType)
						{
							supplierTotal += Convert.ToDouble(orderSplit[2]);
						}
					}
				}
			}
			stopWatch.Stop();

			richTextBox4.Text += "TimeToLoad: " + stopWatch.Elapsed + '\n';
			richTextBox4.Text += "Total cost: " + supplierTotal + '\n';
		}

		private void CostOfAllOrdersInAWeekForASupplierType()
		{
			string folderPath = "StoreData";
			int weekToSearch = 1;
			string supplierType = "Cleaning";
			double supplierTotal = 0.0;

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			string[] fileNames = Directory.GetFiles(folderPath);

			foreach (var filePath in fileNames)
			{
				string[] splitName = filePath.Split('_');
				if (Convert.ToInt32(splitName[1]) == weekToSearch)
				{
					string fileNameExt = Path.GetFileName(filePath);
					string[] orderData = File.ReadAllLines(folderPath + @"\" + fileNameExt);

					foreach (var orderInfo in orderData)
					{
						string[] orderSplit = orderInfo.Split(',');
						if (orderSplit[1] == supplierType)
						{
							supplierTotal += Convert.ToDouble(orderSplit[2]);
						}
					}
				}
			}

			stopWatch.Stop();

			richTextBox4.Text += "TimeToLoad: " + stopWatch.Elapsed + '\n';
			richTextBox4.Text += "Total cost: " + supplierTotal + '\n';
		}

		private void CostOfAllOrdersToASupplierType()
		{
			string folderPath = "StoreData";
			string supplierType = "Cleaning";
			double supplierTotal = 0.0;

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			string[] fileNames = Directory.GetFiles(folderPath);
			foreach (var filePath in fileNames)
			{
				string fileNameExt = Path.GetFileName(filePath);
				string fileName = Path.GetFileNameWithoutExtension(filePath);


				string[] orderData = File.ReadAllLines(folderPath + @"\" + fileNameExt);
				foreach (var orderInfo in orderData)
				{
					string[] orderSplit = orderInfo.Split(',');
					if (orderSplit[1] == supplierType)
					{
						supplierTotal += Convert.ToDouble(orderSplit[2]);
					}
				}
			}

			stopWatch.Stop();
			richTextBox4.Text += "TimeToLoad: " + stopWatch.Elapsed + '\n';
			richTextBox4.Text += "Total cost: " + supplierTotal + '\n';
		}

		private void CostOfAllOrdersToASupplier()
		{
			string folderPath = "StoreData";
			string supplier = "Walkers";
			double supplierTotal = 0.0;

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			string[] fileNames = Directory.GetFiles(folderPath);
			foreach (var filePath in fileNames)
			{
				string fileNameExt = Path.GetFileName(filePath);
				string fileName = Path.GetFileNameWithoutExtension(filePath);


				string[] orderData = File.ReadAllLines(folderPath + @"\" + fileNameExt);
				foreach (var orderInfo in orderData)
				{
					string[] orderSplit = orderInfo.Split(',');
					if (orderSplit[0] == supplier)
					{
						supplierTotal += Convert.ToDouble(orderSplit[2]);
					}
				}
			}

			stopWatch.Stop();
			richTextBox4.Text += "TimeToLoad: " + stopWatch.Elapsed + '\n';
			richTextBox4.Text += "Total cost: " + supplierTotal + '\n';
		}

		private void TotalCostForAllOrdersForAStore()
		{
			string selectedStore = "ABE1";
			string folderPath = "StoreData";
			double totalCost = 0;

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			string[] fileNames = Directory.GetFiles(folderPath);

			foreach (var filePath in fileNames)
			{
				string[] splitPath = filePath.Split('_');
				string[] splitName = splitPath[0].Split('\\');
				if (splitName[1] == selectedStore)
				{
					richTextBox4.Text += filePath + '\n';
					string fileNameExt = Path.GetFileName(filePath);
					string[] orderData = File.ReadAllLines(folderPath + @"\" + fileNameExt);

					foreach (var orderInfo in orderData)
					{
						string[] orderSplit = orderInfo.Split(',');
						totalCost += Convert.ToDouble(orderSplit[2]);
					}
				}
			}

			stopWatch.Stop();

			richTextBox4.Text += "TimeToLoad: " + stopWatch.Elapsed + '\n';
			richTextBox4.Text += "Total cost: " + totalCost + '\n';
		}

		private void ReadAllFilesInWeekRangeAndTotalCost()
		{
			string folderPath = "StoreData";
			int weekToSearch = 1;
			double totalCost = 0;

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			string[] fileNames = Directory.GetFiles(folderPath);

			foreach (var filePath in fileNames)
			{
				string[] splitName = filePath.Split('_');
				if (Convert.ToInt32(splitName[1]) == weekToSearch)
				{
					string fileNameExt = Path.GetFileName(filePath);
					string[] orderData = File.ReadAllLines(folderPath + @"\" + fileNameExt);

					foreach (var orderInfo in orderData)
					{
						string[] orderSplit = orderInfo.Split(',');
						totalCost += Convert.ToDouble(orderSplit[2]);
					}
				}
			}

			stopWatch.Stop();

			richTextBox4.Text += "TimeToLoad: " + stopWatch.Elapsed + '\n';
			richTextBox4.Text += "Total cost: " + totalCost + '\n';
		}

		private void ReadAllFilesAndCountTotalCost()
		{
			// Read each file and sum the total cost for each store
			string folderPath = "StoreData";
			string storeCodesFile = "StoreCodes.csv";
			double totalCost = 0;
			int totalLines = 0;


			Dictionary<string, Store> B3stores = new Dictionary<string, Store>();

			Stopwatch stopWatch = new Stopwatch();
			stopWatch.Start();

			string B3storeCodesFilePath = Directory.GetCurrentDirectory() + @"\" + storeCodesFile;
			string[] B3storeCodesData = File.ReadAllLines(B3storeCodesFilePath);
			foreach (var storeData in B3storeCodesData)
			{
				string[] storeDataSplit = storeData.Split(',');
				Store store = new Store { storeCode = storeDataSplit[0], storeLocation = storeDataSplit[1] };
				if (!B3stores.ContainsKey(store.storeCode))
					B3stores.Add(store.storeCode, store);
			}

			string[] fileNames = Directory.GetFiles(folderPath);
			foreach (var filePath in fileNames)
			{
				string fileNameExt = Path.GetFileName(filePath);
				string fileName = Path.GetFileNameWithoutExtension(filePath);

				string[] orderData = File.ReadAllLines(folderPath + @"\" + fileNameExt);
				foreach (var orderInfo in orderData)
				{
					string[] orderSplit = orderInfo.Split(',');
					totalCost += Convert.ToDouble(orderSplit[2]);
					totalLines++;
				}
			}

			stopWatch.Stop();

			richTextBox4.Text += "Time: " + stopWatch.Elapsed + '\n';
			richTextBox4.Text += "Count: " + fileNames.Length + '\n';
			richTextBox4.Text += "Total cost: " + totalCost + '\n'; // np: 197186552.640005 
			richTextBox4.Text += "Total lines: " + totalLines; // np: 5254571
		}
	}
}