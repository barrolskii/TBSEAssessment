﻿using System;
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

			Stores = new Dictionary<string, Store>();
			queueDate = new ConcurrentQueue<Date>();
			queueOrder = new ConcurrentQueue<Order>();

			InitComboBoxes();
			InitDataGridViewBoxes();

            comboBox11.Hide();

            // Hide the supplier search combo boxes and buttons until relevant data is added
            comboBox9.Hide();
            comboBox10.Hide();
            label6.Hide();
            label7.Hide();
            button5.Hide();

            comboBox11.Hide();
            comboBox12.Hide();
            label11.Hide();
            label12.Hide();
            button6.Hide();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            openFileDialog.Filter = "CSV files|*.csv";
            openFileDialog.Title = "File";


            var t2 = Task.Factory.StartNew(() => Parallel.ForEach(queueOrder, order =>
            {
                queueOrder.TryDequeue(out order);
            }));

            var t3 = Task.Factory.StartNew(() => Parallel.ForEach(queueDate, date =>
            {
                queueDate.TryDequeue(out date);
            }));

            var t4 = Task.Factory.StartNew(() => Stores.Clear());

            Task.WaitAll(t2, t3, t4);

            t1 = new Task(() => ReadAllData());

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
                comboBox10.Invoke(new Action(() => comboBox10.Items.Add(newStore.storeCode)));
            }

			string[] fileNames = Directory.GetFiles(folderPath);

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


            string[] suppliers = queueOrder.AsParallel().Select(order => order.supplier).Distinct().OrderBy(order => order).ToArray();
            string[] supplierTypes = queueOrder.AsParallel().Select(order => order.supplierType).Distinct().OrderBy(order => order).ToArray();


            Stopwatch tsw = new Stopwatch();
            tsw.Start(); // 00:00:00.0007343

            TaskFactory TF = new TaskFactory(TaskScheduler.Default);

            List<Task> TL = new List<Task>();

            TL.Add(TF.StartNew(() => InvokeComboBox(comboBox4, suppliers),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default));

            TL.Add(TF.StartNew(() => InvokeComboBox(comboBox5, supplierTypes),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default));

            TL.Add(TF.StartNew(() => InvokeComboBox(comboBox7, suppliers),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default));

            TL.Add(TF.StartNew(() => InvokeComboBox(comboBox8, supplierTypes),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default));

            Task.WaitAll(TL.ToArray());

            /*comboBox4.Invoke(new Action(() => { comboBox4.Items.AddRange(suppliers); }));
            comboBox5.Invoke(new Action(() => { comboBox5.Items.AddRange(supplierTypes); }));
            comboBox7.Invoke(new Action(() => { comboBox7.Items.AddRange(suppliers); }));
            comboBox8.Invoke(new Action(() => { comboBox8.Items.AddRange(supplierTypes); }));*/

            tsw.Stop();
            richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Time to load: " + tsw.Elapsed + '\n'));

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

			/* End of testing section */
			#endregion

			stopwatch.Stop();

			#region Debugging messages
			textBox3.Invoke(new Action(() => textBox3.Text = "Total cost: £" + costs));

			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Time to load: " + stopwatch.Elapsed + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Dictionary key count: " + Stores.Keys.Count + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Queue count: " + queueDate.Count + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Queue order count: " + queueOrder.Count + '\n'));


			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Total cost of orders: " + costs + '\n'));

			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Total 2013 dates: " + linqDates.Length + '\n'));
			richTextBox1.Invoke(new Action(() => richTextBox1.Text += "Total 2013 dates: " + plinqDates.Length + '\n'));
			#endregion

			/* Enable combo boxes for search queries */
			comboBox1.Invoke(new Action(() => comboBox1.Enabled = true));
			comboBox2.Invoke(new Action(() => comboBox2.Enabled = true));
			comboBox3.Invoke(new Action(() => comboBox3.Enabled = true));

            comboBox12.DataSource = comboBox9.Items;
            comboBox11.DataSource = comboBox10.Items;
        }

        // TODO: rework the code that this calls because this might not even be needed
        private void InvokeComboBox(ComboBox comboBox, string[] items)
        {
            comboBox.Invoke(new Action(() => comboBox.Items.AddRange(items)));
        }

		private void button5_Click(object sender, EventArgs e)
		{
            // Supplier
            if (comboBox4.Text != "" && comboBox9.Text != "" && comboBox10.Text != "")
            {
                string supplier = comboBox4.Text;
                string store = comboBox10.Text;
                int week = Convert.ToInt32(comboBox9.Text);

                double totalCost = queueOrder.Where(order => order.supplier == supplier && order.date.week == week && order.store.storeCode == store)
                                             .Select(order => order.cost).Sum();

                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Supplier cost in a week for a store: " + totalCost + '\n'));
            }
            else if (comboBox4.Text != "" && comboBox9.Text != "")
            {
                string supplier = comboBox4.Text;
                int week = Convert.ToInt32(comboBox9.Text);

                double totalCost = queueOrder.Where(order => order.supplier == supplier && order.date.week == week)
                                                                     .Select(order => order.cost).Sum();

                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Supplier cost in a week: " + totalCost + '\n'));
            }
            else if (comboBox4.Text != "" && comboBox10.Text != "")
            {
                string supplier = comboBox4.Text;
                string store = comboBox10.Text;

                double totalCost = queueOrder.Where(order => order.supplier == supplier && order.store.storeCode == store)
                                             .Select(order => order.cost).Sum();

                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Supplier cost for a store: " + totalCost + '\n'));
            }
            else
            {
                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Please choose a week, a store or both\n"));
            }
		}

        private void button6_Click(object sender, EventArgs e)
        {
            // Supplier type
            if (comboBox5.Text != "" && comboBox12.Text != "" && comboBox11.Text != "")
            {
                string supplierType = comboBox5.Text;
                int week = Convert.ToInt32(comboBox12.Text);
                string store = comboBox11.Text;

                double totalCost = queueOrder.Where(order => order.supplierType == supplierType && order.date.week == week && order.store.storeCode == store)
                                                                         .Select(order => order.cost).Sum();

                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Supplier type cost in a week for a store: " + totalCost + '\n'));
            }
            else if (comboBox5.Text != "" && comboBox12.Text != "")
            {
                int week = Convert.ToInt32(comboBox12.Text);
                string supplierType = comboBox5.Text;

                double totalCost = queueOrder.Where(order => order.supplierType == supplierType && order.date.week == week)
                                                                         .Select(order => order.cost).Sum();

                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Supplier type cost in a week: " + totalCost + '\n'));
            }
            else if (comboBox5.Text != "" && comboBox11.Text != "")
            {
                string supplierType = comboBox5.Text;
                string store = comboBox11.Text;

                double totalCost = queueOrder.Where(order => order.supplierType == supplierType && order.store.storeCode == store)
                                                                         .Select(order => order.cost).Sum();

                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Supplier type cost for a store: " + totalCost + '\n'));
            }
            else
            {
                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Please choose a week, a store or both\n"));
            }
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
		}

		private void button4_Click(object sender, EventArgs e)
		{

		}

		private void InitComboBoxes()
		{
			/* Disable use of combo boxes until data is loaded in */
			comboBox1.Enabled = false;
			comboBox2.Enabled = false;
			comboBox3.Enabled = false;

            /* 
             * Add empty string options for the combo boxes so the user can leave specific
             * search fields blank
             */
            comboBox9.Items.Add("");
            comboBox10.Items.Add("");

            /* 
			 * Populate the second combo box to have a selection of weeks and populate the 
			 * full store search combo box and supplier search combo box
			 */
            for (int i = 1; i <= 52; i++)
			{
				comboBox2.Items.Add(i);
				comboBox6.Items.Add(i);
                comboBox9.Items.Add(i);
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

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            string store = comboBox1.Text;

            double costOfAllOrdersToAStore = queueOrder.Where(order => order.store.storeCode == store)
                                                       .Select(order => order.cost)
                                                       .Sum();

            double test = queueOrder.AsParallel().Where(order => order.store.storeCode == store)
                                                 .Select(order => order.cost).Sum();

            richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Cost of all orders for " + store + ": £" + costOfAllOrdersToAStore + "\n"));
            richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Cost of all orders for " + store + ": £" + test + "\n"));
        }

        private void comboBox6_TextChanged(object sender, EventArgs e)
        {
            int week = Convert.ToInt32(comboBox6.Text);

            double costOfAllStoresInAWeek = queueOrder.Where(order => order.date.week == week)
                                                      .Select(order => order.cost)
                                                      .Sum();

            richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Cost of all orders for week " + week + ": £" + costOfAllStoresInAWeek + "\n"));
        }

        private void comboBox7_TextChanged(object sender, EventArgs e)
        {
            string supplier = comboBox7.Text;

            double costOfAllOrdersForASupplier = queueOrder.Where(order => order.supplier == supplier)
                                                           .Select(order => order.cost)
                                                           .Sum();

            richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Cost of all orders for " + supplier + ": £" + costOfAllOrdersForASupplier + "\n"));
        }

        private void comboBox8_TextChanged(object sender, EventArgs e)
        {
            string supplierType = comboBox8.Text;

            double costOfAllOrdersToASupplierType = queueOrder.Where(order => order.supplierType == supplierType)
                                                              .Select(order => order.cost)
                                                              .Sum();

            richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Cost of all orders for " + supplierType + ": £" + costOfAllOrdersToASupplierType + "\n"));
        }

        private void comboBox4_TextChanged(object sender, EventArgs e)
        {
            comboBox9.Show();
            comboBox10.Show();
            label6.Show();
            label7.Show();
            button5.Show();
        }

        private void comboBox5_TextChanged(object sender, EventArgs e)
        {
            comboBox11.Show();
            comboBox12.Show();
            label11.Show();
            label12.Show();
            button6.Show();
        }
    }
}