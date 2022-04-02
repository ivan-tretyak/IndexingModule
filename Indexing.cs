//Scanning library. Library to found image file in selected directory.
//Copyright (C) 2021 Ivan Tretyak Nickolaevich
//This program is free software; you can redistribute it and/or modify
//it under the terms of the GNU General Public License as published by
//the Free Software Foundation; either version 2 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//GNU General Public License for more details.

//You should have received a copy of the GNU General Public License along
//with this program; if not, write to the Free Software Foundation, Inc.,
//51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA

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
        List<Image> images = new();
        public Indexing()
        {
            scanning = new();
            scanningResults = new();
            List<Image> images = new(0);
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
            //List<Image> images = new();

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

        private string Prefix(int i)
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                return new string(Enumerable.Repeat(chars, i)
                    .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public List<Image> CopyImage(List<Image> images, string destPath)
        {
            foreach(Image image in images)
            {
                String name = Path.GetFileName(image.path);
                File.Copy(image.path, $"{destPath}{Path.DirectorySeparatorChar}{Prefix(5)}{name}");
                image.path = $"{destPath}{Path.DirectorySeparatorChar}{Prefix(5)}{name}";
            }
            return images;
        }

        private void empty()
        {
            scanningResults = new();
        }
    }
}
