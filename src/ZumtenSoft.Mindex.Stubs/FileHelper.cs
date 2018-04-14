using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZumtenSoft.Mindex.Stubs
{
    public static class FileHelper
    {
        public static string FindDirectory(string directoryName)
        {
            for (var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());; currentDirectory = currentDirectory.Parent)
            {
                var fullPath = Path.Combine(currentDirectory.FullName, directoryName);
                if (Directory.Exists(fullPath))
                    return fullPath;
            }
        }
    }
}
