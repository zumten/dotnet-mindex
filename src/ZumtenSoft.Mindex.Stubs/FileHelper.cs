using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using ProtoBuf;
using ZumtenSoft.Mindex.Stubs.IndianCustoms;

namespace ZumtenSoft.Mindex.Stubs
{
    public static class FileHelper
    {
        public static string FindDirectory(string directoryName)
        {
            for (var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory()); ; currentDirectory = currentDirectory.Parent)
            {
                var fullPath = Path.Combine(currentDirectory.FullName, directoryName);
                if (Directory.Exists(fullPath))
                    return fullPath;
            }


        }

        public static T LoadOrConvertToProtobuf<T>(string fileName, Func<string, T> initialLoad)
        {
            var dataFolder = FindDirectory("data");

            var protoPath = Path.Combine(dataFolder, Path.ChangeExtension(fileName, "bin"));
            var csvPath = Path.Combine(dataFolder, fileName);
            if (File.Exists(protoPath + ".gz") && !File.Exists(protoPath))
                ExtractGzip(protoPath + ".gz");

            if (File.Exists(protoPath))
                return LoadFromProtobuf<T>(protoPath);

            if (File.Exists(csvPath + ".gz") && !File.Exists(csvPath))
                ExtractGzip(csvPath + ".gz");

            if (File.Exists(csvPath))
            {
                var fileContent = initialLoad(csvPath);
                SaveToProtobuf(protoPath, fileContent);
                return fileContent;
            }

            throw new FileNotFoundException($"File '{fileName}' could not be found or built.");
        }

        private static void ExtractGzip(string filePath)
        {
            using (FileStream file = File.OpenRead(filePath))
            using (GZipStream gzip = new GZipStream(file, CompressionMode.Decompress))
            using (FileStream output = File.OpenWrite(filePath.Replace(".gz", "")))
               gzip.CopyTo(output);
        }

        private static T LoadFromProtobuf<T>(string fileName)
        {
            using (var file = File.OpenRead(fileName))
                return Serializer.Deserialize<T>(file);
        }

        private static void SaveToProtobuf<T>(string fileName, T items)
        {
            using (var file = File.OpenWrite(fileName))
                Serializer.Serialize(file, items);
        }
    }
}
