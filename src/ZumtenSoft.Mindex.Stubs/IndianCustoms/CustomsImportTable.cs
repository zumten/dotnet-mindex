using System.Collections.Generic;

namespace ZumtenSoft.Mindex.Stubs.IndianCustoms
{
    public class CustomsImportTable : Table<CustomsImport, CustomsImportSearch>
    {
        public CustomsImportTable(IReadOnlyCollection<CustomsImport> rows) : base(rows)
        {
            MapCriteria(s => s.Date, i => i.Date);
            MapCriteria(s => s.Type, i => i.Type);
            MapCriteria(s => s.Origin, i => i.Origin);
            MapCriteria(s => s.Quantity, i => i.Quantity);
            MapCriteria(s => s.QuantityUnitCode, i => i.QuantityUnitCode);
            MapCriteria(s => s.QuantityType, i => i.QuantityType);
            MapCriteria(s => s.ImportState, i => i.ImportState);
            MapCriteria(s => s.ImportLocationCode, i => i.ImportLocationCode);
            MapCriteria(s => s.ImportLocationName, i => i.ImportLocationName);
            MapCriteria(s => s.TariffHeading, i => i.TariffHeading);
            MapCriteria(s => s.GoodsValue, i => i.GoodsValue);

            ConfigureIndex().IncludeColumns(s => s.ImportState, s => s.Origin, s => s.QuantityType, s => s.Date).Build();
        }
    }
}
