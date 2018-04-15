using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ProtoBuf;

namespace ZumtenSoft.Mindex.Stubs.IndianCustoms
{
    public static class IndianCustomsHelper
    {
        private static Import[] _cache;
        private static readonly object Lock = new object();

        public static Import[] LoadImports()
        {
            if (_cache == null)
                lock (Lock)
                    if (_cache == null)
                        _cache = FileHelper.LoadOrConvertToProtobuf("IndianCustoms-Imports.csv", f => LoadCustomsImportsFromCsv(f).ToArray());

            return _cache;
        }

        private static IEnumerable<Import> LoadCustomsImportsFromCsv(string fileName)
        {
            var regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)", RegexOptions.Compiled);
            using (FileStream file = File.OpenRead(fileName))
            using (StreamReader reader = new StreamReader(file))
            {
                if (reader.ReadLine() != null)
                {
                    for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
                    {
                        string[] parts = regex.Split(line);
                        yield return new Import
                        {
                            QuantityUnitCode = Intern(parts[0]),
                            ImportState = Intern(parts[1]),
                            TariffHeading = Int32.Parse(parts[2]),
                            GoodsValue = Decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                            QuantityDescription = Intern(parts[4]),
                            //GoodDescription = Intern(parts[5]),
                            ImportLocationCode = Intern(parts[6]),
                            QuantityType = Intern(parts[7]),
                            Date = DateTime.Parse(parts[8], CultureInfo.InvariantCulture),
                            ImportLocationName = Intern(parts[9]),
                            Type = Intern(parts[10]),
                            Origin = Intern(parts[11]),
                            Quantity = Decimal.Parse(parts[12]),
                            SerialId = Int32.Parse(parts[13])
                        };
                    }
                }
            }
        }

        private static string Intern(string str)
        {
            if (str.StartsWith("\\\""))
                str = str.Substring(2, str.Length - 4);
            return String.Intern(str);
        }
    }
}
