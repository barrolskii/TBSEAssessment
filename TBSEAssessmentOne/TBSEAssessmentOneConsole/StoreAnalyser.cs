using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

        private string storeCodesFilePath;
        private string storesFolderPath;

        bool hasFinished;
        int fileCount;

        public StoreAnalyser()
        {
            Stores = new Dictionary<string, Store>();
            queueDate = new ConcurrentQueue<Date>();
            queueOrder = new ConcurrentQueue<Order>();

            hasFinished = false;
            fileCount = 0;
        }

        public void Start()
        {
            string folderPath = "StoreData";
            string storeCodesFile = "StoreCodes.csv";

            do
            {
                Console.WriteLine("Please enter the path to the store data");
                storeCodesFilePath = Console.ReadLine();

                storesFolderPath = storeCodesFilePath + "\\" + folderPath;
                storeCodesFilePath += "\\" + storeCodesFile;


                if (!File.Exists(storeCodesFilePath))
                {
                    Console.WriteLine("File not found in current directory. Please enter the correct directory path");
                }
                else if (!Directory.Exists(storesFolderPath))
                {
                    Console.WriteLine("Folder for store data not found in current directory. Please enter the correct directory path");
                }
                else
                    break;
            }
            while (true);


            Task t1 = new Task(() => ReadAllFiles());
            Task t2 = new Task(() => PrintWaitList());

            t1.Start();
            t2.Start();

            Task.WaitAll(t1, t2);
        }

        private void PrintWaitList()
        {
            Console.Clear();
            Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine("Loading data please wait");
            Console.WriteLine(" ________________________________________________________________________________\n");

            Console.WriteLine(" ‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾");


            int itr = 0;

            char[] progress = Enumerable.Repeat<char>(' ', 80).ToArray();

            Console.CursorVisible = false;
            Console.SetCursorPosition(0, 2);

            // Wait for the task to finish
            while (!hasFinished || fileCount / 130 > itr)
            {
                if (fileCount / 130 > itr)
                {
                    Console.Write("\r|");
                    progress[itr] = '#';

                    foreach (char c in progress)
                    {
                        Console.Write(c);
                    }
                    Console.Write("|");

                    itr++;
                }
            }

            Console.CursorVisible = true;
            Console.SetCursorPosition(0, 4);
            Console.WriteLine("\nFinished loading");
            Console.WriteLine("Press enter to continue");
        }

        private void ReadAllFiles()
        {
            // Exact folder path for copy paste
            // C:\Users\b012361h\Documents\GitHub\TBSEAssessment\TBSEAssessmentOne\TBSEAssessmentOne\bin\Debug

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

                Interlocked.Increment(ref fileCount);
            });

            stopwatch.Stop();

            hasFinished = true;

            /* Console.WriteLine("\nTime to load: {0} \n", stopwatch.Elapsed); */
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

                if (input == "-q" || input == "-quit") break;

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
