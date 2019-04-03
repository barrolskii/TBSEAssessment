using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Diagnostics;
using System.IO;

namespace TBSEAssessmentOneConsole
{
    class Program
    {
        static Dictionary<string, Store> Stores;
        static ConcurrentQueue<Date> queueDate;
        static ConcurrentQueue<Order> queueOrder;

        static void Main(string[] args)
        {
            Initalise();
            ReadAllFiles();
            Console.ReadLine();

            string input;
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Please select an option or type -help for help");
                PrintSelectionOptions();
                input = Console.ReadLine();

                if (input == "q") break;

                CheckCommand(ref input);

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

            Console.WriteLine("Goodbye");
        }

        private static void Initalise()
        {
            Stores = new Dictionary<string, Store>();
            queueDate = new ConcurrentQueue<Date>();
            queueOrder = new ConcurrentQueue<Order>();
        }

        private static void CheckCommand(ref string input)
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
                    break;
            }
        }

        private static void PrintSelectionOptions()
        {
            Console.WriteLine("===================================================================================");

            Console.WriteLine("1: Total cost of all orders");
            Console.WriteLine("2: Cost of all orders for a single store");
            Console.WriteLine("3: Cost of all orders in a week for all stores");
            Console.WriteLine("4: Cost of all orders in a week for a single store");
            Console.WriteLine("5: Cost of all orders to a supplier");
            Console.WriteLine("6: Cost of all orders to a supplier type");
            Console.WriteLine("7: Cost of orders in a week for a supplier type");
            Console.WriteLine("8: Cost of orders to a supplier type for a store");
            Console.WriteLine("9: Cost of orders in a week for a supplier type for a store");

            Console.WriteLine("===================================================================================");
        }

        private static void PrintCommands()
        {
            Console.WriteLine("-h or -help for a list of all commands");
            Console.WriteLine("-ps or -printstores to print all stores");
            Console.WriteLine("-pst or -printsuppliertypes to print all supplier types");
            Console.WriteLine("-psup or -printsuppliers to print all suppliers");

            Console.ReadLine();
        }

        private static void PrintStores()
        {
            foreach (var store in Stores)
            {
                Console.WriteLine("{0} : {1}", store.Key, store.Value.storeLocation);
            }

            Console.ReadLine();
        }

        private static void PrintSuppliers()
        {
            string[] suppliers = queueOrder.Select(order => order.supplier).Distinct().ToArray();

            foreach (string s in suppliers)
            {
                Console.WriteLine("{0}", s);
            }

            Console.ReadLine();
        }

        private static void PrintSupplierTypes()
        {
            string[] supplierTypes = queueOrder.Select(order => order.supplierType).Distinct().ToArray();

            foreach (string s in supplierTypes)
            {
                Console.WriteLine("{0}", s);
            }

            Console.ReadLine();
        }

        #region Assignment required functions

        private static void TotalCostOfAllOrders()
        {
            double totalCost = queueOrder.Sum(order => order.cost);
            Console.WriteLine("Total cost of all orders is: £{0}", totalCost);

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private static void CostOfAllOrdersForASingleStore()
        {
            Console.WriteLine("Please enter the store you would like to search");
            string store = Console.ReadLine();

            do
            {
                if (Stores.Keys.Contains(store)) break;

                Console.WriteLine("Please enter a correct store code");
                store = Console.ReadLine();
            }
            while (true);

            double totalCost = queueOrder.Where(order => order.store.storeCode == store).Select(order => order.cost).Sum();
            Console.WriteLine("{0} total order cost: £{1}", store, totalCost);

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private static void CostOfAllOrdersInAWeekForAllStores()
        {
            Console.WriteLine("Please enter the week you want to search");
            int week = Convert.ToInt32(Console.ReadLine());

            double totalCost = queueOrder.Where(order => order.date.week == week).Select(order => order.cost).Sum();
            Console.WriteLine("Total cost of all orders for week {0}: £{1}", week, totalCost);

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        // TODO: Add a year option for a single store search
        private static void CostOfAllOrdersInASingleWeekForAStore()
        {
            Console.WriteLine("Please enter the week you want to search");
            int week = Convert.ToInt32(Console.ReadLine());

            Console.WriteLine("Please enter the store you want to search");
            string store = Console.ReadLine();

            do
            {
                if (Stores.Keys.Contains(store)) break;

                Console.WriteLine("Please enter a correct store code");
                store = Console.ReadLine();
            }
            while (true);

            double totalCost = queueOrder.Where(order => order.date.week == week && order.store.storeCode == store).Select(order => order.cost).Sum();
            Console.WriteLine("Total cost of all orders for {0} in week {1}: £{2}", store, week, totalCost);

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private static void CostOfAllOrdersToASupplier()
        {
            Console.WriteLine("Please enter the supplier you want to search");
            string supplier = Console.ReadLine();

            double totalCost = queueOrder.Where(order => order.supplier == supplier).Select(order => order.cost).Sum();
            Console.WriteLine("Total cost for {0} orders: £{1}", supplier, totalCost);

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private static void CostOfAllOrdersToASupplierType()
        {
            Console.WriteLine("Please enter the supplier type you want to search");
            string supplierType = Console.ReadLine();

            double totalCost = queueOrder.Where(order => order.supplierType == supplierType).Select(order => order.cost).Sum();
            Console.WriteLine("Total cost for {0}: £{1}", supplierType, totalCost);

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private static void CostOfAllOrdersInAWeekToASupplierType()
        {
            Console.WriteLine("Please enter the supplier type you want to search");
            string supplierType = Console.ReadLine();

            Console.WriteLine("Please enter the week you want to search");
            int week = Convert.ToInt32(Console.ReadLine());

            double totalCost = queueOrder.Where(order => order.supplierType == supplierType && order.date.week == week).Select(order => order.cost).Sum();
            Console.WriteLine("Total cost for {0} in week {1}: £{2}", supplierType, week, totalCost);

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private static void CostOfAllOrdersToASupplierTypeForAStore()
        {
            Console.WriteLine("Please enter the supplier type you want to search");
            string supplierType = Console.ReadLine();

            Console.WriteLine("Please enter the store you would like to search");
            string store = Console.ReadLine();

            do
            {
                if (Stores.Keys.Contains(store)) break;

                Console.WriteLine("Please enter a correct store code");
                store = Console.ReadLine();
            }
            while (true);

            double totalCost = queueOrder.Where(order => order.supplierType == supplierType && order.store.storeCode == store).Select(order => order.cost).Sum();
            Console.WriteLine("Total cost for {0} in store {1}: £{2}", supplierType, store, totalCost.ToString("0.00"));

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private static void CostOfAllOrdersInAWeekToASupplierTypeForAStore()
        {
            Console.WriteLine("Please enter the supplier type you want to search");
            string supplierType = Console.ReadLine();

            Console.WriteLine("Please enter the store you would like to search");
            string store = Console.ReadLine();

            do
            {
                if (Stores.Keys.Contains(store)) break;

                Console.WriteLine("Please enter a correct store code");
                store = Console.ReadLine();
            }
            while (true);

            Console.WriteLine("Please enter the week you want to search");
            int week = Convert.ToInt32(Console.ReadLine());

            double totalCost = queueOrder.Where(order => order.supplierType == supplierType && order.store.storeCode == store && order.date.week == week)
                                         .Select(order => order.cost).Sum();
            Console.WriteLine("Total cost for {0} in store {1} for week {2}: £{3}", supplierType, store, week, totalCost);

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        #endregion

        private static void ReadAllFiles()
        {
            string folderPath = "StoreData";
            string storeCodesFile = "StoreCodes.csv";

            //string storeCodesFilePath = Directory.GetCurrentDirectory() + @"\" + storeCodesFile;
            string storeCodesFilePath = @"C:\Users\b012361h\Documents\GitHub\TBSEAssessment\TBSEAssessmentOne\TBSEAssessmentOne\bin\Debug\" + storeCodesFile;
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


            //string[] fileNames = Directory.GetFiles(folderPath);
            string[] fileNames = Directory.GetFiles(@"C:\Users\b012361h\Documents\GitHub\TBSEAssessment\TBSEAssessmentOne\TBSEAssessmentOne\bin\Debug\" + folderPath);

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
        }
    }
}