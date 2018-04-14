using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ProtoBuf;

namespace ZumtenSoft.Mindex.Stubs.IndianCustoms
{
    public static class CustomsImportHelper
    {
        public static CustomsImport[] LoadCustomImports(string fileName)
        {
            var protoFileName = Path.ChangeExtension(fileName, "bin");
            if (File.Exists(protoFileName))
                return LoadCustomsImportFromProtobuf(protoFileName);

            if (!File.Exists(fileName))
                throw new Exception("Failed to find the file " + fileName +". Full dataset can be found at https://public.enigma.com/datasets/indian-customs-imports/a1696eb8-532b-48e0-b9b9-9d06031f8d0d");

            var result = LoadCustomsImportsFromCsv(fileName).ToArray();
            SaveCustomsImportToProtobuf(protoFileName, result);

            return result;
        }

        public static CustomsImport[] LoadCustomsImportFromProtobuf(string fileName)
        {
            using (var file = File.OpenRead(fileName))
                return Serializer.Deserialize<CustomsImport[]>(file);
        }

        public static void SaveCustomsImportToProtobuf(string fileName, CustomsImport[] items)
        {
            using (var file = File.OpenWrite(fileName))
                Serializer.Serialize(file, items);
        }

        public static IEnumerable<CustomsImport> LoadCustomsImportsFromCsv(string fileName)
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
                        yield return new CustomsImport
                        {
                            QuantityUnitCode = Intern(parts[0]),
                            ImportState = Intern(parts[1]),
                            TariffHeading = Int32.Parse(parts[2]),
                            GoodsValue = Decimal.Parse(parts[3], CultureInfo.InvariantCulture),
                            QuantityDescription = Intern(parts[4]),
                            GoodDescription = Intern(parts[5]),
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
