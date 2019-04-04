using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace TBSEAssessmentOneConsole
{
    public partial class StoreAnalyser
    {
        private Dictionary<string, Store> Stores;
        private ConcurrentQueue<Date> queueDate;
        private ConcurrentQueue<Order> queueOrder;

        public StoreAnalyser()
        {
            Stores = new Dictionary<string, Store>();
            queueDate = new ConcurrentQueue<Date>();
            queueOrder = new ConcurrentQueue<Order>();
        }

        public void ReadAllFiles()
        {
            // Exact folder path for copy paste
            // C:\Users\b012361h\Documents\GitHub\TBSEAssessment\TBSEAssessmentOne\TBSEAssessmentOne\bin\Debug

            string folderPath = "StoreData";
            string storeCodesFile = "StoreCodes.csv";

            Console.WriteLine("Please enter the path to the store data");
            string storeCodesFilePath = Console.ReadLine();

            string storesFolderPath = storeCodesFilePath + "\\" + folderPath;
            storeCodesFilePath += "\\" + storeCodesFile;


            string[] storeList = File.ReadAllLines(storeCodesFilePath);

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Populate the stores dictionary
            foreach (var s in storeList)
            {
                string[] splitStoreData = s.Split(',');

                Store newStore = new Store { storeCode = splitStoreData[0], storeLocation = splitStoreData[1] };
                if (!Stores.ContainsKey(newStore.storeCode))
                    Stores.Add(newStore.storeCode, newStore);
            }


            string[] fileNames = Directory.GetFiles(storesFolderPath);

            // Start populating the queues
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

            stopwatch.Stop();

            Console.WriteLine("Time to load: {0} \n", stopwatch.Elapsed);
            Console.WriteLine("Dictionary key count: {0} \n", Stores.Keys.Count);
            Console.WriteLine("Queue count: {0} \n", queueDate.Count);
            Console.WriteLine("Queue order count: {0} \n", queueOrder.Count);

            Console.ReadLine();
        }

        public void MainLoop()
        {
            string input;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Please select an option or type -help for help\n");

                PrintSelectionOptions();
                input = Console.ReadLine();

                if (input == "q") break;

                CheckCommand(ref input);
            }

            Console.WriteLine("Goodbye");
        }

        private void CheckCommand(ref string input)
        {
            switch (input)
            {
                case "-h":
                case "-help":
                    PrintCommands();
                    break;

                case "-ps":
                case "-printstores":
                    PrintStores();
                    break;

                case "-pst":
                case "-printsuppliertypes":
                    PrintSupplierTypes();
                    break;

                case "-psup":
                case "-printsuppliers":
                    PrintSuppliers();
                    break;

                default:
                    HandleInput(ref input);
                    break;
            }
        }

        private void HandleInput(ref string input)
        {
            switch (input)
            {
                case "1":
                    TotalCostOfAllOrders();
                    break;

                case "2":
                    CostOfAllOrdersForASingleStore();
                    break;

                case "3":
                    CostOfAllOrdersInAWeekForAllStores();
                    break;

                case "4":
                    CostOfAllOrdersInASingleWeekForAStore();
                    break;

                case "5":
                    CostOfAllOrdersToASupplier();
                    break;

                case "6":
                    CostOfAllOrdersToASupplierType();
                    break;

                case "7":
                    CostOfAllOrdersInAWeekToASupplierType();
                    break;

                case "8":
                    CostOfAllOrdersToASupplierTypeForAStore();
                    break;

                case "9":
                    CostOfAllOrdersInAWeekToASupplierTypeForAStore();
                    break;

                default:
                    Console.WriteLine("Please select a valid option or enter q to quit");
                    break;
            }
        }
    }
}
