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
using System.Windows.Forms.DataVisualization.Charting;

namespace TBSEAssessmentOne
{
	public partial class Form1 : Form
	{
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
            InitCharts();

            progressBar1.Hide();
            LblLoadingBar.Hide();
        }

        #region Button click functions

        private void BtnLoadFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = System.IO.Directory.GetCurrentDirectory();
            openFileDialog.Filter = "CSV files|*.csv";
            openFileDialog.Title = "File";

            // Remove any previous data that is stored in the dictionary and the concurrent queues

            TaskFactory TF = new TaskFactory();

            List<Task> TL = new List<Task>();

            TL.Add(TF.StartNew(() => Parallel.ForEach(queueOrder, order =>
            {
                queueOrder.TryDequeue(out order);
            }),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default));


            TL.Add(TF.StartNew(() => Parallel.ForEach(queueDate, date =>
            {
                queueDate.TryDequeue(out date);
            }),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default));


            TL.Add(TF.StartNew(() => Stores.Clear(),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default));

            Task.WaitAll(TL.ToArray());

            LblLoadingBar.Show();
            LblLoadingBar.Text = "Loading store data...";
            progressBar1.Show();

            t1 = new Task(() => ReadAllData());
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                richTextBox2.Text = openFileDialog.FileName;

                t1.Start();
            }
        }

        private void BtnStoreSearch_Click(object sender, EventArgs e)
        {
            double totalCost = 0;

            Dictionary<string, double> SupplierTypeCost = new Dictionary<string, double>();

            string fileToSearch = "StoreData/" + CBStoreSearchStore.Text + '_' + CBStoreSearchWeek.Text + '_' + CBStoreSearchYear.Text + ".csv";
            string[] storeData = File.ReadAllLines(fileToSearch);

            /* If the cell rows already contain data then remove them */
            if (dataGridView2.Rows.Count >= 2)
            {
                dataGridView2.Rows.Clear();
            }

            foreach (var s in storeData)
            {
                string[] orderSplit = s.Split(',');

                dataGridView2.Rows.Add(orderSplit[0], orderSplit[1], orderSplit[2]);
                totalCost += Convert.ToDouble(orderSplit[2]);

                if (!SupplierTypeCost.Keys.Contains(orderSplit[1]))
                {
                    SupplierTypeCost.Add(orderSplit[1], Convert.ToDouble(orderSplit[2]));
                }
                else
                {
                    SupplierTypeCost[orderSplit[1]] += Convert.ToDouble(orderSplit[2]);
                }
            }

            ChartStoreData.Show();

            if (ChartStoreData.Series["Supplier costs"].Points.Count >= 1)
                ChartStoreData.Series["Supplier costs"].Points.Clear();

            List<string> supplierTypeNames = new List<string>();
            foreach (var s in SupplierTypeCost)
            {
                ChartStoreData.Series["Supplier costs"].Points.AddXY(s.Key, s.Value);
                supplierTypeNames.Add(s.Key);
            }

            for (int i = 0; i < ChartStoreData.Series["Supplier costs"].Points.Count; i++)
            {
                ChartStoreData.Series["Supplier costs"].Points[i].LegendText = supplierTypeNames[i];
                ChartStoreData.Series["Supplier costs"].Points[i].Label = ChartStoreData.Series["Supplier costs"].Points[i].YValues[0].ToString("C2");
            }

            textBox2.Text = "Total cost: " + totalCost.ToString("C2");

            TabCtrlDataDisplay.SelectedTab = StoreData;
        }

        private void BtnSupplierSearchSupplier_Click(object sender, EventArgs e)
        {
            // Supplier
            if (CBSupplierSearchSupplier.Text != "" && CBSupplierSearchWeek.Text != "" && CBSupplierSearchStore.Text != "")
            {
                string supplier = CBSupplierSearchSupplier.Text;
                string store = CBSupplierSearchStore.Text;
                int week = Convert.ToInt32(CBSupplierSearchWeek.Text);

                double totalCost = queueOrder.Where(order => order.supplier == supplier && order.date.week == week && order.store.storeCode == store)
                                             .Select(order => order.cost).Sum();

                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Supplier cost in a week for a store: " + totalCost.ToString("C2") + '\n'));
            }
            else if (CBSupplierSearchSupplier.Text != "" && CBSupplierSearchWeek.Text != "")
            {
                string supplier = CBSupplierSearchSupplier.Text;
                int week = Convert.ToInt32(CBSupplierSearchWeek.Text);

                double totalCost = queueOrder.Where(order => order.supplier == supplier && order.date.week == week)
                                                                     .Select(order => order.cost).Sum();

                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Supplier cost in a week: " + totalCost.ToString("C2") + '\n'));
            }
            else if (CBSupplierSearchSupplier.Text != "" && CBSupplierSearchStore.Text != "")
            {
                string supplier = CBSupplierSearchSupplier.Text;
                string store = CBSupplierSearchStore.Text;

                double totalCost = queueOrder.Where(order => order.supplier == supplier && order.store.storeCode == store)
                                             .Select(order => order.cost).Sum();

                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Supplier cost for a store: " + totalCost.ToString("C2") + '\n'));
            }
            else
            {
                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Please choose a week, a store or both\n"));
            }

            TabCtrlDataDisplay.SelectedTab = StoreList;
        }

        private void BtnSupplierSearchSupplierType_Click(object sender, EventArgs e)
        {
            // Supplier type
            if (CBSupplierTypeSearchSupplierType.Text != "" && CBSupplierTypeSearchWeek.Text != "" && CBSupplierTypeSearchStore.Text != "")
            {
                string supplierType = CBSupplierTypeSearchSupplierType.Text;
                int week = Convert.ToInt32(CBSupplierTypeSearchWeek.Text);
                string store = CBSupplierTypeSearchStore.Text;

                double totalCost = queueOrder.Where(order => order.supplierType == supplierType && order.date.week == week && order.store.storeCode == store)
                                                                         .Select(order => order.cost).Sum();

                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Supplier type cost in a week for a store: " + totalCost.ToString("C2") + '\n'));
            }
            else if (CBSupplierTypeSearchSupplierType.Text != "" && CBSupplierTypeSearchWeek.Text != "")
            {
                int week = Convert.ToInt32(CBSupplierTypeSearchWeek.Text);
                string supplierType = CBSupplierTypeSearchSupplierType.Text;

                double totalCost = queueOrder.Where(order => order.supplierType == supplierType && order.date.week == week)
                                                                         .Select(order => order.cost).Sum();

                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Supplier type cost in a week: " + totalCost.ToString("C2") + '\n'));
            }
            else if (CBSupplierTypeSearchSupplierType.Text != "" && CBSupplierTypeSearchStore.Text != "")
            {
                string supplierType = CBSupplierTypeSearchSupplierType.Text;
                string store = CBSupplierTypeSearchStore.Text;

                double totalCost = queueOrder.Where(order => order.supplierType == supplierType && order.store.storeCode == store)
                                                                         .Select(order => order.cost).Sum();

                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Supplier type cost for a store: " + totalCost.ToString("C2") + '\n'));
            }
            else
            {
                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Please choose a week, a store or both\n"));
            }

            TabCtrlDataDisplay.SelectedTab = StoreList;
        }

        #endregion

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
            }

			string[] fileNames = Directory.GetFiles(folderPath);
            progressBar1.Invoke(new Action(() => progressBar1.Maximum = fileNames.Length));
            int loadingBarProgress = 0;

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

                Interlocked.Increment(ref loadingBarProgress);
                progressBar1.Invoke(new Action(() => progressBar1.Value = loadingBarProgress));
			});


            List<string> suppliers = new List<string> { "" };
            suppliers.AddRange(queueOrder.AsParallel().Select(order => order.supplier).Distinct().OrderBy(order => order).ToList());

            List<string> supplierTypes = new List<string> { "" };
            supplierTypes.AddRange(queueOrder.AsParallel().Select(order => order.supplierType).Distinct().OrderBy(order => order).ToList());

            List<string> comboBoxStoreList = new List<string> { "" };
            comboBoxStoreList.AddRange(Stores.Keys.ToList());


            TaskFactory TF = new TaskFactory(TaskScheduler.Default);

            List<Task> TL = new List<Task>();


            TL.Add(TF.StartNew(() => SetComboboxData(CBStoreSearchStore, comboBoxStoreList),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default).ContinueWith((task) => SetComboboxData(CBSupplierSearchStore, comboBoxStoreList))
                                      .ContinueWith((task) => SetComboboxData(CBSupplierTypeSearchStore, comboBoxStoreList))
                                      .ContinueWith((task) => SetComboboxData(CBStoreComparisonStore, comboBoxStoreList))
                                      .ContinueWith((task) => SetComboboxData(CBStoreComparisonStoreToCompare, comboBoxStoreList)));

            TL.Add(TF.StartNew(() => SetComboboxData(CBSupplierSearchSupplier, suppliers),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default).ContinueWith((task) => SetComboboxData(CBAllStoreSearchSupplier, suppliers)));

            TL.Add(TF.StartNew(() => SetComboboxData(CBSupplierTypeSearchSupplierType, supplierTypes),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default).ContinueWith((task) => SetComboboxData(CBAllStoreSearchSupplierType, supplierTypes)));


            TL.Add(TF.StartNew(() => SetDataGridViewDatasource(dataGridView1, Stores.Values.ToList()),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default));


            Task.WaitAll(TL.ToArray());


            // Total cost of all orders
            double costs = queueOrder.Sum(num => num.cost);

           
			stopwatch.Stop();

            LblLoadingBar.Invoke(new Action(() => LblLoadingBar.Text = "Data loaded successfully. Time taken: " + stopwatch.Elapsed));

            textBox3.Invoke(new Action(() => textBox3.Text = "Total cost: £" + costs.ToString("0.00")));


            /* Enable combo boxes for search queries */
            TaskFactory TFEnableComboBoxes = new TaskFactory();

            List<Task> TLCB = new List<Task>();

            TLCB.Add(TFEnableComboBoxes.StartNew(() => SetComboBoxEnabled(CBStoreSearchStore),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default).ContinueWith((task) => SetComboBoxEnabled(CBStoreSearchWeek))
                                      .ContinueWith((task => SetComboBoxEnabled(CBStoreSearchYear))));

            TLCB.Add(TFEnableComboBoxes.StartNew(() => SetComboBoxEnabled(CBSupplierSearchSupplier),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default).ContinueWith((task) => SetComboBoxEnabled(CBSupplierSearchWeek))
                                      .ContinueWith((task) => SetComboBoxEnabled(CBSupplierSearchStore)));

            TLCB.Add(TFEnableComboBoxes.StartNew(() => SetComboBoxEnabled(CBSupplierTypeSearchSupplierType),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default).ContinueWith((task) => SetComboBoxEnabled(CBSupplierTypeSearchStore))
                                      .ContinueWith((task) => SetComboBoxEnabled(CBSupplierTypeSearchWeek)));

            TLCB.Add(TFEnableComboBoxes.StartNew(() => SetComboBoxEnabled(CBAllStoreSearchWeek),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default).ContinueWith((task) => SetComboBoxEnabled(CBAllStoreSearchSupplier))
                                      .ContinueWith((task) => SetComboBoxEnabled(CBAllStoreSearchSupplierType)));

            TLCB.Add(TFEnableComboBoxes.StartNew(() => SetComboBoxEnabled(CBStoreComparisonStore),
                CancellationToken.None, TaskCreationOptions.PreferFairness,
                TaskScheduler.Default).ContinueWith((task) => SetComboBoxEnabled(CBStoreComparisonStoreToCompare)));


            Task.WaitAll(TLCB.ToArray());

            dataGridView1.Invoke(new Action(() => dataGridView1.Columns[0].HeaderText = "Store Code"));
            dataGridView1.Columns[1].HeaderText = "Store Location";
        }

        #region Invokation functions

        private void SetComboboxData(ComboBox cb, List<string> items)
        {
            string[] data = items.ToArray();

            cb.Invoke(new Action(() => cb.DataSource = data));
        }

        private void SetComboBoxEnabled(ComboBox cb, bool enabled = true)
        {
            cb.Invoke(new Action(() => cb.Enabled = enabled));
        }

        private void SetDataGridViewDatasource(DataGridView dgv, List<Store> data)
        {
            List<Store> list = new List<Store>();
            list.AddRange(data);

            dgv.Invoke(new Action(() => dgv.DataSource = list));
        }

        #endregion

        #region InitFunctions

        private void InitComboBoxes()
		{
			/* Disable use of combo boxes until data is loaded in */
			CBStoreSearchStore.Enabled = false;
			CBStoreSearchWeek.Enabled  = false;
			CBStoreSearchYear.Enabled  = false;

            CBSupplierSearchSupplier.Enabled = false;
            CBSupplierSearchWeek.Enabled     = false;
            CBSupplierSearchStore.Enabled    = false;

            CBSupplierTypeSearchSupplierType.Enabled = false;
            CBSupplierTypeSearchWeek.Enabled         = false;
            CBSupplierTypeSearchStore.Enabled        = false;

            CBAllStoreSearchWeek.Enabled         = false;
            CBAllStoreSearchSupplier.Enabled     = false;
            CBAllStoreSearchSupplierType.Enabled = false;

            CBStoreComparisonStore.Enabled          = false;
            CBStoreComparisonStoreToCompare.Enabled = false;

            /* 
             * Add empty string options for the combo boxes so the user can leave specific
             * search fields blank
             */
            CBAllStoreSearchWeek.Items.Add("");
            CBSupplierSearchWeek.Items.Add("");
            CBSupplierSearchStore.Items.Add("");
            CBSupplierTypeSearchWeek.Items.Add("");

            /* 
			 * Populate the second combo box to have a selection of weeks and populate the 
			 * full store search combo box and supplier search combo box
			 */
            for (int i = 1; i <= 52; i++)
			{
				CBStoreSearchWeek.Items.Add(i);
				CBAllStoreSearchWeek.Items.Add(i);
                CBSupplierSearchWeek.Items.Add(i);
                CBSupplierTypeSearchWeek.Items.Add(i);
			}

			/*
			 * Only two years to choose from in the files so this combo box
			 * only needs two options
			 */
			CBStoreSearchYear.Items.Add(2013);
			CBStoreSearchYear.Items.Add(2014);

            // Hide the supplier search combo boxes and buttons until relevant data is added
            CBSupplierSearchWeek.Hide();
            CBSupplierSearchStore.Hide();
            LblSupplierSearchSupplierWeek.Hide();
            LblSupplierSearchSupplierStore.Hide();
            BtnSupplierSearchSupplier.Hide();

            CBSupplierTypeSearchStore.Hide();
            CBSupplierTypeSearchWeek.Hide();
            LblSupplierSearchSupplierTypeStore.Hide();
            LblSupplierSearchSupplierTypeWeek.Hide();
            BtnSupplierSearchSupplierType.Hide();
        }

		private void InitDataGridViewBoxes()
		{
			dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;


			dataGridView2.ColumnCount = 3;
			dataGridView2.Columns[0].Name = "Supplier";
			dataGridView2.Columns[1].Name = "Supplier type";
			dataGridView2.Columns[2].Name = "Cost";

			dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
		}

        private void InitCharts()
        {
            ChartStoreData.Hide();
            ChartStoreComparisonPieLeft.Hide();
            ChartStoreComparisonPieRight.Hide();
            ChartStoreComparisonLineLeft.Hide();
            ChartStoreComparisonLineRight.Hide();

            ChartStoreComparisonLineLeft.ChartAreas[0].AxisX.Minimum = 1;
            ChartStoreComparisonLineLeft.ChartAreas[0].AxisX.Maximum = 13;
            ChartStoreComparisonLineLeft.ChartAreas[0].AxisX.Interval = 1;

            ChartStoreComparisonLineRight.ChartAreas[0].AxisX.Minimum = 1;
            ChartStoreComparisonLineRight.ChartAreas[0].AxisX.Maximum = 13;
            ChartStoreComparisonLineRight.ChartAreas[0].AxisX.Interval = 1;
        }

        #endregion

        #region ComboBox text changed functions

        private void CBStoreSearchStore_TextChanged(object sender, EventArgs e)
        {
            string store = CBStoreSearchStore.Text;

            if (store != "")
            {
                double test = queueOrder.AsParallel().Where(order => order.store.storeCode == store)
                                                     .Select(order => order.cost).Sum();


                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Cost of all orders for " + store + ": " + test.ToString("C2") + "\n"));
            }

            TabCtrlDataDisplay.SelectedTab = StoreList;
        }

        private void CBSupplierSearchSupplier_TextChanged(object sender, EventArgs e)
        {
            CBSupplierSearchWeek.Show();
            CBSupplierSearchStore.Show();
            LblSupplierSearchSupplierWeek.Show();
            LblSupplierSearchSupplierStore.Show();
            BtnSupplierSearchSupplier.Show();
        }

        private void CBSupplierTypeSearchSupplierType_TextChanged(object sender, EventArgs e)
        {
            CBSupplierTypeSearchStore.Show();
            CBSupplierTypeSearchWeek.Show();
            LblSupplierSearchSupplierTypeStore.Show();
            LblSupplierSearchSupplierTypeWeek.Show();
            BtnSupplierSearchSupplierType.Show();
        }

        private void CBAllStoreSearchWeek_TextChanged(object sender, EventArgs e)
        {
            if (CBAllStoreSearchWeek.Text != "")
            {
                int week = Convert.ToInt32(CBAllStoreSearchWeek.Text);

                double costOfAllStoresInAWeek = queueOrder.Where(order => order.date.week == week)
                                                          .Select(order => order.cost)
                                                          .Sum();

                richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Cost of all orders for week " + week + ": " + costOfAllStoresInAWeek.ToString("C2") + "\n"));

                TabCtrlDataDisplay.SelectedTab = StoreList;
            }
        }

        private void CBAllStoreSearchSupplier_TextChanged(object sender, EventArgs e)
        {
            string supplier = CBAllStoreSearchSupplier.Text;

            double costOfAllOrdersForASupplier = queueOrder.Where(order => order.supplier == supplier)
                                                           .Select(order => order.cost)
                                                           .Sum();

            richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Cost of all orders for " + supplier + ": " + costOfAllOrdersForASupplier.ToString("C2") + "\n"));

            TabCtrlDataDisplay.SelectedTab = StoreList;
        }

        private void CBAllStoreSearchSupplierType_TextChanged(object sender, EventArgs e)
        {
            string supplierType = CBAllStoreSearchSupplierType.Text;

            double costOfAllOrdersToASupplierType = queueOrder.Where(order => order.supplierType == supplierType)
                                                              .Select(order => order.cost)
                                                              .Sum();

            richTextBox3.Invoke(new Action(() => richTextBox3.Text += "Cost of all orders for " + supplierType + ": " + costOfAllOrdersToASupplierType.ToString("C2") + "\n"));

            TabCtrlDataDisplay.SelectedTab = StoreList;
        }

        private void CBStoreComparisonStore_TextChanged(object sender, EventArgs e)
        {
            string store = CBStoreComparisonStore.Text;

            Task task = new Task(() => PopulateComparisonData(ChartStoreComparisonPieLeft, ChartStoreComparisonLineLeft, store));
            task.Start();
        }

        private void CBStoreComparisonStoreToCompare_TextChanged(object sender, EventArgs e)
        {
            string store = CBStoreComparisonStoreToCompare.Text;

            Task task = new Task(() => PopulateComparisonData(ChartStoreComparisonPieRight, ChartStoreComparisonLineRight, store));
            task.Start();
        }

        #endregion

        private void PopulateComparisonData(Chart pieChart, Chart lineChart, string storeToSearch)
        {
            if (pieChart.Series["Quarterly"].Points.Count >= 1)
                pieChart.Invoke(new Action(() => pieChart.Series["Quarterly"].Points.Clear()));

            if (lineChart.Series["2013"].Points.Count >= 1)
            {
                lineChart.Series["2013"].Points.Clear();
                lineChart.Series["2014"].Points.Clear();
            }

            string store = storeToSearch;

            double quarterOne = queueOrder.Where(order => order.store.storeCode == store && order.date.week >= 1 && order.date.week <= 13)
                                           .Select(order => order.cost)
                                           .Sum();

            double quarterTwo = queueOrder.Where(order => order.store.storeCode == store && order.date.week >= 14 && order.date.week <= 26)
                                           .Select(order => order.cost)
                                           .Sum();

            double quarterThree = queueOrder.Where(order => order.store.storeCode == store && order.date.week >= 27 && order.date.week <= 40)
                                           .Select(order => order.cost)
                                           .Sum();

            double quarterFour = queueOrder.Where(order => order.store.storeCode == store && order.date.week >= 41 && order.date.week <= 52)
                                           .Select(order => order.cost)
                                           .Sum();

            pieChart.Invoke(new Action(() => pieChart.Series["Quarterly"].Points.AddXY("First quarter", quarterOne.ToString("C2"))));
            pieChart.Invoke(new Action(() => pieChart.Series["Quarterly"].Points.AddXY("Second quarter", quarterTwo.ToString("C2"))));
            pieChart.Invoke(new Action(() => pieChart.Series["Quarterly"].Points.AddXY("Third quarter", quarterThree.ToString("C2"))));
            pieChart.Invoke(new Action(() => pieChart.Series["Quarterly"].Points.AddXY("Fourth quarter", quarterFour.ToString("C2"))));

            string[] quarters = new string[4] { "First quarter", "Second quarter", "Third quarter", "Fourth quarter" };
            for (int i = 0; i < pieChart.Series["Quarterly"].Points.Count; i++)
            {
                pieChart.Invoke(new Action(() => pieChart.Series["Quarterly"].Points[i].LegendText = quarters[i]));
                pieChart.Invoke(new Action(() => pieChart.Series["Quarterly"].Points[i].Label = pieChart.Series["Quarterly"].Points[i].YValues[0].ToString("C2")));
            }


            pieChart.Invoke(new Action(() => pieChart.Titles[0].Text = store));
            pieChart.Invoke(new Action(() => pieChart.Show()));

            double totalCost = quarterOne + quarterTwo + quarterThree + quarterFour;


            double[] monthOrders2013 = new double[12];
            double[] monthOrders2014 = new double[12];

            Parallel.For(0, 12, (i) =>
            {
                double weekTotal = queueOrder.Where(order => order.date.year == 2013 && Math.Ceiling(order.date.week / 4.0) == i + 1 && order.store.storeCode == store)
                                             .Select(order => order.cost).Sum();

                monthOrders2013[i] = weekTotal;
            });

            Parallel.For(0, 12, (i) =>
            {
                double weekTotal = queueOrder.Where(order => order.date.year == 2014 && Math.Ceiling(order.date.week / 4.0) == i + 1 && order.store.storeCode == store)
                                             .Select(order => order.cost).Sum();

                monthOrders2014[i] = weekTotal;
            });


            string[] months = Enum.GetNames(typeof(Months));


            for (int i = 0; i < 12; i++)
            {
                lineChart.Series["2013"].Points.AddXY(months[i], monthOrders2013[i]);
                lineChart.Series["2014"].Points.AddXY(months[i], monthOrders2014[i]);
            }


            lineChart.Show();
        }
    }
}