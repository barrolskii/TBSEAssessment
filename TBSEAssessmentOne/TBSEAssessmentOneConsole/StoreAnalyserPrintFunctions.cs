using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TBSEAssessmentOneConsole
{
    public partial class StoreAnalyser
    {

        private void PrintSelectionOptions()
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

        private void PrintCommands()
        {
            Console.WriteLine("\n===================================================================================");

            Console.WriteLine("-h or -help for a list of all commands");
            Console.WriteLine("-ps or -printstores to print all stores");
            Console.WriteLine("-pst or -printsuppliertypes to print all supplier types");
            Console.WriteLine("-psup or -printsuppliers to print all suppliers");
            Console.WriteLine("-q or -quit to exit the application");

            Console.WriteLine("===================================================================================");

            Console.ReadLine();
        }

        private void PrintStores()
        {
            foreach (var store in Stores)
            {
                Console.WriteLine("{0} : {1}", store.Key, store.Value.storeLocation);
            }

            Console.ReadLine();
        }

        private void PrintSuppliers()
        {
            string[] suppliers = queueOrder.AsParallel().Select(order => order.supplier).Distinct().OrderBy(order => order).ToArray();

            foreach (string s in suppliers)
            {
                Console.WriteLine("{0}", s);
            }

            Console.ReadLine();
        }

        private void PrintSupplierTypes()
        {
            string[] supplierTypes = queueOrder.AsParallel().Select(order => order.supplierType).Distinct().OrderBy(order => order).ToArray();

            foreach (string s in supplierTypes)
            {
                Console.WriteLine("{0}", s);
            }

            Console.ReadLine();
        }

        #region Assignment required functions

        private void TotalCostOfAllOrders()
        {
            double totalCost = queueOrder.Sum(order => order.cost);
            Console.WriteLine("Total cost of all orders is: {0}", totalCost.ToString("C2"));

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private void CostOfAllOrdersForASingleStore()
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
            Console.WriteLine("{0} total order cost: {1}", store, totalCost.ToString("C2"));

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private void CostOfAllOrdersInAWeekForAllStores()
        {
            Console.WriteLine("Please enter the week you want to search");
            int week = Convert.ToInt32(Console.ReadLine());

            double totalCost = queueOrder.Where(order => order.date.week == week).Select(order => order.cost).Sum();
            Console.WriteLine("Total cost of all orders for week {0}: {1}", week, totalCost.ToString("C2"));

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private void CostOfAllOrdersInASingleWeekForAStore()
        {
            Console.WriteLine("Please enter the week you want to search");
            int week = Convert.ToInt32(Console.ReadLine());

            do
            {
                if (week < 1 || week > 52)
                {
                    Console.WriteLine("Please enter a week from 1 - 52");
                    week = Convert.ToInt32(Console.ReadLine());
                }
                else
                    break;
            }
            while (true);

            Console.WriteLine("Please enter the store you want to search");
            string store = Console.ReadLine();

            do
            {
                if (Stores.Keys.Contains(store)) break;

                Console.WriteLine("Please enter a correct store code");
                store = Console.ReadLine();
            }
            while (true);

            Console.WriteLine("Enter the year you would like to search");
            int year = Convert.ToInt32(Console.ReadLine());

            do
            {
                if (year < 2013 || year > 2014)
                {
                    Console.WriteLine("Please enter a valid year");
                    year = Convert.ToInt32(Console.ReadLine());
                }
                else
                    break;
            }
            while (true);

            double totalCost = queueOrder.Where(order => order.date.week == week && order.store.storeCode == store && order.date.year == year).Select(order => order.cost).Sum();
            Console.WriteLine("Total cost of all orders for {0} in week {1}: {2}", store, week, totalCost.ToString("C2"));

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private void CostOfAllOrdersToASupplier()
        {
            Console.WriteLine("Please enter the supplier you want to search");
            string supplier = Console.ReadLine();

            double totalCost = queueOrder.Where(order => order.supplier == supplier).Select(order => order.cost).Sum();
            Console.WriteLine("Total cost for {0} orders: {1}", supplier, totalCost.ToString("C2"));

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private void CostOfAllOrdersToASupplierType()
        {
            Console.WriteLine("Please enter the supplier type you want to search");
            string supplierType = Console.ReadLine();

            double totalCost = queueOrder.Where(order => order.supplierType == supplierType).Select(order => order.cost).Sum();
            Console.WriteLine("Total cost for {0}: {1}", supplierType, totalCost.ToString("C2"));

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private void CostOfAllOrdersInAWeekToASupplierType()
        {
            Console.WriteLine("Please enter the supplier type you want to search");
            string supplierType = Console.ReadLine();

            Console.WriteLine("Please enter the week you want to search");
            int week = Convert.ToInt32(Console.ReadLine());

            do
            {
                if (week < 1 || week > 52)
                {
                    Console.WriteLine("Please enter a week from 1 - 52");
                    week = Convert.ToInt32(Console.ReadLine());
                }
                else
                    break;
            }
            while (true);

            Console.WriteLine("Enter the year you would like to search");
            int year = Convert.ToInt32(Console.ReadLine());

            do
            {
                if (year < 2013 || year > 2014)
                {
                    Console.WriteLine("Please enter a valid year");
                    year = Convert.ToInt32(Console.ReadLine());
                }
                else
                    break;
            }
            while (true);

            double totalCost = queueOrder.Where(order => order.supplierType == supplierType && order.date.week == week && order.date.year == year)
                                         .Select(order => order.cost).Sum();

            Console.WriteLine("Total cost for {0} in week {1}: {2}", supplierType, week, totalCost.ToString("C2"));

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private void CostOfAllOrdersToASupplierTypeForAStore()
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
            Console.WriteLine("Total cost for {0} in store {1}: {2}", supplierType, store, totalCost.ToString("C2"));

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        private void CostOfAllOrdersInAWeekToASupplierTypeForAStore()
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

            do
            {
                if (week < 1 || week > 52)
                {
                    Console.WriteLine("Please enter a week from 1 - 52");
                    week = Convert.ToInt32(Console.ReadLine());
                }
                else
                    break;
            }
            while (true);

            Console.WriteLine("Enter the year you would like to search");
            int year = Convert.ToInt32(Console.ReadLine());

            do
            {
                if (year < 2013 || year > 2014)
                {
                    Console.WriteLine("Please enter a valid year");
                    year = Convert.ToInt32(Console.ReadLine());
                }
                else
                    break;
            }
            while (true);

            double totalCost = queueOrder.Where(order => order.supplierType == supplierType && order.store.storeCode == store && order.date.week == week && order.date.year == year)
                                         .Select(order => order.cost).Sum();
            Console.WriteLine("Total cost for {0} in store {1} for week {2}: {3}", supplierType, store, week, totalCost.ToString("C2"));

            Console.WriteLine("\nPress any key to continue");
            Console.ReadLine();
        }

        #endregion
    }
}
