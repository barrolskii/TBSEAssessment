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
		Dictionary<string, Store> Stores;
		ConcurrentQueue<Date> queueDate;
		ConcurrentQueue<Order> queueOrder;


		public Form1()
		{
			InitializeComponent();

			t1 = new Task(() => ReadAllData());

			Stores = new Dictionary<string, Store>();
			queueDate = new ConcurrentQueue<Date>();
			 queueOrder = new ConcurrentQueue<Order>();

			InitComboBoxes();
			InitDataGridViewBoxes();
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

			//Dictionary<string, Store> Stores = new Dictionary<string, Store>();

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

			//ConcurrentQueue<Date> queueDate = new ConcurrentQueue<Date>();
			//ConcurrentQueue<Order> queueOrder = new ConcurrentQueue<Order>();

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

			comboBox7.Invoke(new Action(() =>
			{
				foreach (var s in suppliers)
				{
					comboBox7.Items.Add(s);
				}

			}));

			comboBox8.Invoke(new Action(() =>
			{
				foreach (var s in supplierTypes)
				{
					comboBox8.Items.Add(s);
				}

			}));

			#region LINQ and PLINQ testing
			/* LINQ and PLINQ testing */

			// Total cost of all orders
			double costs = queueOrder.Sum(num => num.cost); // 197186552.639997

			Date[] linqDates = (from date in queueDate
								where date.year == 2013
								select date).ToArray();

			Date[] plinqDates = (from date in queueDate.AsParallel()
								 where date.year == 2013
								 select date).ToArray();

			string supplierType = "Cleaning"; // 18940385.4499995
			string supplier = "Surf";

			// Total cost to a supplier and a supplier type
			double supplierAndTypeCost = queueOrder.Where(order => order.supplier == supplier && order.supplierType == supplierType).Select(order => order.cost).Sum();

			// Total cost for a supplier type for a store
			string storeCode = "ABE1";
			double costForSupplierTypeSingleStore = queueOrder.Where(order => order.store.storeCode == storeCode && order.supplierType == supplierType).Select(order => order.cost).Sum();


			// Total cost for a supplier type for a store in a week
			int week = 1;
			double costForSupplierTypeSingleStoreInWeek = queueOrder.Where(order => order.store.storeCode == storeCode && order.supplierType == supplierType && order.date.week == week)
																	.Select(order => order.cost).Sum();

			// Total cost for all orders for a single store
			double costOfAllOrdersToSingleStore = queueOrder.Where(order => order.store.storeCode == storeCode).Select(order => order.cost).Sum();


			// Cost of all orders in a week for a supplier
			double costOfAllOrdersToASupplierInAWeek = queueOrder.Where(order => order.supplier == supplier && order.date.week == week).Select(order => order.cost).Sum();

			/* End of testing section */
			#endregion

			stopwatch.Stop();

			#region Debugging messages
			textBox3.Invoke(new Action(() => textBox3.Text = "Total cost: £" + costs));

			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Time to load: " + stopwatch.Elapsed + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Dictionary key count: " + Stores.Keys.Count + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Queue count: " + queueDate.Count + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Queue order count: " + queueOrder.Count + '\n'));

			if (comboBox7.Text == "")
				richTextBox1.Invoke(new Action(() => richTextBox1.Text += "ComboBoxval: " + comboBox7.Text + '\n'));

			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Total cost of orders: " + costs + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Supplier and type total: " + supplierAndTypeCost + '\n'));

			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "\nCost of supplier type for store: " + costForSupplierTypeSingleStore + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "\nCost of supplier type for store in a week: " + costForSupplierTypeSingleStoreInWeek + '\n'));

			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "\nCost of all orders to a single store: " + costOfAllOrdersToSingleStore + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "\nCost of all orders to a supplier in a week: " + costOfAllOrdersToASupplierInAWeek + '\n'));

			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Total 2013 dates: " + linqDates.Length + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Total 2013 dates: " + plinqDates.Length + '\n'));
			#endregion

			/* Enable combo boxes for search queries */
			comboBox1.Invoke(new Action(() => comboBox1.Enabled = true));
			comboBox2.Invoke(new Action(() => comboBox2.Enabled = true));
			comboBox3.Invoke(new Action(() => comboBox3.Enabled = true));
		}

		private void button5_Click(object sender, EventArgs e)
		{
			if (comboBox4.Text != "")
			{

			}

			if (comboBox5.Text != "")
			{

			}
		}

		private void button6_Click(object sender, EventArgs e)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			if (comboBox6.Text != "")
			{
				int weekToSearch = Convert.ToInt32(comboBox6.Text);
				double costOfAllOrdersInAWeek = queueOrder.Where(order => order.date.week == weekToSearch).Select(order => order.cost).Sum();

				richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Total cost: £" + costOfAllOrdersInAWeek + '\n'));
			}

			if (comboBox7.Text != "")
			{
				string supplier = comboBox7.Text;
				double supplierCost = queueOrder.Where(order => order.supplier == supplier).Select(order => order.cost).Sum();

				richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Total cost: £" + supplierCost + '\n'));
			}

			if (comboBox8.Text != "")
			{
				string supplierType = comboBox8.Text;
				double supplierTypeCost = queueOrder.Where(order => order.supplierType == supplierType).Select(order => order.cost).Sum();

				richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Total cost: £" + supplierTypeCost + '\n'));
			}

			sw.Stop();
			richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Time: " + sw.Elapsed + '\n'));
		}

		// Function covers total cost for all orders in a week for a store
		private void button2_Click(object sender, EventArgs e)
		{
			double totalCost = 0;

			Dictionary<string, double> SupplierCost = new Dictionary<string, double>();

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

				if (!SupplierCost.Keys.Contains(orderSplit[0]))
				{
					SupplierCost.Add(orderSplit[0], Convert.ToDouble(orderSplit[2]));
				}
				else
				{
					SupplierCost[orderSplit[0]] += Convert.ToDouble(orderSplit[2]);
				}
			}

			foreach (var s in SupplierCost)
			{
				chart1.Series["Supplier costs"].Points.AddXY(s.Key, s.Value);
			}

			textBox2.Text = "Total cost: £" + totalCost;

			sw.Stop();

			tabControl1.SelectedTab = tabPage2;

			/* Output for debgging purposes */
			richTextBox1.Text += "Time: " + sw.Elapsed + '\n';
			richTextBox1.Text += "Row count: " + dataGridView2.Rows.Count + '\n'; // 628
			richTextBox1.Text += "Total cost: " + totalCost + '\n'; // 22440.79
		}


		private void button3_Click(object sender, EventArgs e)
		{
			Stores.Clear();

			Date date;
			foreach (var d in queueDate)
				queueDate.TryDequeue(out date);

			Order order;
			foreach (var o in queueOrder)
				queueOrder.TryDequeue(out order);



			/*string folderPath = "StoreData";
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
					dataGridView2.Rows.Add(splitPath);

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
			richTextBox4.Text += "Total cost: " + supplierTotal + '\n';*/
		}

		private void button4_Click(object sender, EventArgs e)
		{
			//ReadAllFilesAndCountTotalCost();
			//ReadAllFilesInWeekRangeAndTotalCost();
			//TotalCostForAllOrdersForAStore();
			//CostOfAllOrdersToASupplier();
			//CostOfAllOrdersToASupplierType();
			CostOfAllOrdersInAWeekForASupplierType();
			//CostOfAllOrdersForASupplierTypeForAStore();
			//CostOfAllOrdersInAWeekForASupplierTypeForAStore();
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
			string supplier = "Heinz";
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
						if (orderSplit[0] == supplier)
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

		private void InitComboBoxes()
		{
			/* Disable use of combo boxes until data is loaded in */
			comboBox1.Enabled = false;
			comboBox2.Enabled = false;
			comboBox3.Enabled = false;

			/* 
			 * Populate the second combo box to have a selection of weeks and populate the 
			 * full store search combo box
			 */
			for (int i = 1; i <= 52; i++)
			{
				comboBox2.Items.Add(i);
				comboBox6.Items.Add(i);
			}

			/*
			 * Only two years to choose from in the files so this combo box
			 * only needs two options
			 */
			comboBox3.Items.Add(2013);
			comboBox3.Items.Add(2014);
		}

		private void InitDataGridViewBoxes()
		{
			dataGridView1.ColumnCount = 2;
			dataGridView1.Columns[0].Name = "StoreCode";
			dataGridView1.Columns[1].Name = "StoreName";

			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


			dataGridView2.ColumnCount = 3;
			dataGridView2.Columns[0].Name = "Supplier";
			dataGridView2.Columns[1].Name = "Supplier type";
			dataGridView2.Columns[2].Name = "Cost";

			dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		}
	}
}