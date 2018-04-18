using System.Collections.Generic;

namespace ZumtenSoft.Mindex.Stubs.IndianCustoms
{
    public class ImportTable : Table<Import, ImportSearch>
    {
        public ImportTable(IReadOnlyCollection<Import> rows) : base(rows)
        {
            MapCriteria(s => s.Date).ToProperty(i => i.Date);
            MapCriteria(s => s.Type).ToProperty(i => i.Type);
            MapCriteria(s => s.Origin).ToProperty(i => i.Origin);
            MapCriteria(s => s.Quantity).ToProperty(i => i.Quantity);
            MapCriteria(s => s.QuantityUnitCode).ToProperty(i => i.QuantityUnitCode);
            MapCriteria(s => s.QuantityType).ToProperty(i => i.QuantityType);
            MapCriteria(s => s.ImportState).ToProperty(i => i.ImportState);
            MapCriteria(s => s.ImportLocationCode).ToProperty(i => i.ImportLocationCode);
            MapCriteria(s => s.ImportLocationName).ToProperty(i => i.ImportLocationName);
            MapCriteria(s => s.TariffHeading).ToProperty(i => i.TariffHeading);
            MapCriteria(s => s.GoodsValue).ToProperty(i => i.GoodsValue);

            ConfigureIndex().IncludeColumns(s => s.Origin, s => s.ImportState, s => s.QuantityType, s => s.Date).Build();
        }
    }
}
