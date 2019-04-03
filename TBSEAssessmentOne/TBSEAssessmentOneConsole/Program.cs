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

            Console.WriteLine("Please select an option");
            PrintSelectionOptions();

            string input;
            while ((input = Console.ReadLine()) != "q")
            {
                Console.Clear();
                Console.WriteLine("Please select an option");
                PrintSelectionOptions();

                switch (input)
                {
                    case "1":
                        TotalCostOfAllOrders();
                        break;

                    case "2":
                        break;

                    case "3":
                        break;

                    case "4":
                        break;

                    case "5":
                        break;

                    case "6":
                        break;

                    case "7":
                        break;

                    case "8":
                        break;

                    case "9":
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

        private static void TotalCostOfAllOrders()
        {
            double totalCost = queueOrder.Sum(order => order.cost);
            Console.WriteLine("Total cost of all orders is: £{0}", totalCost);
        }

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