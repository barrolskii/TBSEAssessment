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
        }

        private static void Initalise()
        {
            Stores = new Dictionary<string, Store>();
            queueDate = new ConcurrentQueue<Date>();
            queueOrder = new ConcurrentQueue<Order>();
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
