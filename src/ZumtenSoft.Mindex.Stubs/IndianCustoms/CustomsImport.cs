using System;

namespace ZumtenSoft.Mindex.Stubs.IndianCustoms
{
    public class CustomsImport
    {
        public int SerialId { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
        public string Origin { get; set; }

        public decimal Quantity { get; set; }
        public string QuantityUnitCode { get; set; }
        public string QuantityDescription { get; set; }
        public string QuantityType { get; set; }

        public string ImportState { get; set; }
        public string ImportLocationCode { get; set; }
        public string ImportLocationName { get; set; }
        public int TariffHeading { get; set; }
        public decimal GoodsValue { get; set; }
        public string GoodDescription { get; set; }
    }
}
