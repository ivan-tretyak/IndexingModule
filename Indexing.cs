using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IndexingModule;

namespace IndexingModule
{
    
    class Indexing
    {
        Scanning scanning;
        List<ScanningResults> scanningResults;
        public Indexing()
        {
            scanning = new();
            scanningResults = new();
        }

        private bool CheckImage(string path)
        {
            string ext = Path.GetExtension(path);
            if (ext.ToLower() == ".jpg" || ext.ToLower() == ".png")
            {
                return true;
            }
            return false;
        }

        public List<Image> IndexingDirectory(string path)
        {
            List<Image> images = new();

            var scannedResultsRoot = scanning.ScanningDirectory(path);
            scanningResults.Add(scannedResultsRoot);

            foreach(string sbs in scannedResultsRoot.directories)
            {
                IndexingDirectory(sbs);
            }

            foreach(ScanningResults scannedResult in scanningResults)
            {
                foreach(string file in scannedResult.files)
                {
                    if (CheckImage(file))
                    {
                        images.Add(new Image(file));
                    }
                }
            }
            empty();
            return images;
        }

        private void empty()
        {
            scanningResults = new();
        }
    }
}
