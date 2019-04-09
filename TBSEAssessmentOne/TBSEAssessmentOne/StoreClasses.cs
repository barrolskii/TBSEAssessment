using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TBSEAssessmentOne
{
	class Store
	{
		public string storeCode { get; set; }
		public string storeLocation { get; set; }
	}

	class Date
	{
		public int week { get; set; }
		public int year { get; set; }
	}

	class Order
	{
		public Store store { get; set; }

		public Date date { get; set; }
		public string supplier { get; set; }
		public string supplierType { get; set; }
		public double cost { get; set; }
	}

    enum Months
    {
        Jan = 0,
        Feb,
        Mar,
        Apr,
        May,
        Jun,
        Jul,
        Aug,
        Sep,
        Oct,
        Nov,
        Dec
    }
}