using System;
using ProtoBuf;

namespace ZumtenSoft.Mindex.Stubs.IndianCustoms
{
    [ProtoContract]
    public class Import
    {
        [ProtoMember(1)]
        public int SerialId { get; set; }
        [ProtoMember(2)]
        public DateTime Date { get; set; }
        [ProtoMember(3)]
        public string Type { get; set; }
        [ProtoMember(4)]
        public string Origin { get; set; }
        [ProtoMember(5)]
        public decimal Quantity { get; set; }
        [ProtoMember(6)]
        public string QuantityUnitCode { get; set; }
        [ProtoMember(7)]
        public string QuantityDescription { get; set; }
        [ProtoMember(8)]
        public string QuantityType { get; set; }

        [ProtoMember(9)]
        public string ImportState { get; set; }
        [ProtoMember(10)]
        public string ImportLocationCode { get; set; }
        [ProtoMember(11)]
        public string ImportLocationName { get; set; }
        [ProtoMember(12)]
        public int TariffHeading { get; set; }
        [ProtoMember(13)]
        public decimal GoodsValue { get; set; }
        //[ProtoMember(14)]
        //public string GoodDescription { get; set; }
    }
}
