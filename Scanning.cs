using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexingModule
{
    public struct ScanningResults
    {
        public List<string> files;
        public List<string> directories;
    }
    class Scanning
    {
        public Scanning()
        {

        }

        public ScanningResults ScanningDirectory(string path)
        {
            ScanningResults results = new();
            results.files = new();
            results.directories = new();

            string[] fileEntries = Directory.GetFiles(path);
            foreach (string fileName in fileEntries)
                results.files.Add(fileName);

            string[] directoryEntries = Directory.GetDirectories(path);
            foreach (string directoryName in directoryEntries)
                results.directories.Add(directoryName);

            return results;
        }
    }
}
