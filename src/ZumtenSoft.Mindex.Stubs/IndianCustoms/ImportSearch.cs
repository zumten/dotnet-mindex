using System;
using ZumtenSoft.Mindex.Criterias;

namespace ZumtenSoft.Mindex.Stubs.IndianCustoms
{
    public class ImportSearch
    {
        public SearchCriteria<DateTime> Date { get; set; }
        public SearchCriteria<string> Type { get; set; }
        public SearchCriteria<string> Origin { get; set; }

        public SearchCriteria<decimal> Quantity { get; set; }
        public SearchCriteria<string> QuantityUnitCode { get; set; }
        public SearchCriteria<string> QuantityType { get; set; }

        public SearchCriteria<string> ImportState { get; set; }
        public SearchCriteria<string> ImportLocationCode { get; set; }
        public SearchCriteria<string> ImportLocationName { get; set; }
        public SearchCriteria<int> TariffHeading { get; set; }
        public SearchCriteria<decimal> GoodsValue { get; set; }
    }
}