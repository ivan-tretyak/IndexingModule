using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;


namespace IndexingModule
{
    public struct MetaData
    {
        public DateTime DateCreation; //ExifDirectory.TagDateTimeOriginal from ExifSubIFDDirectory
        public string Manufacturer; //ExifDirectoryBase.TagMake from ExifIfd0Directory
        public string Model; //ExifDirectoryBase.TagModel from ExifIfd0Directory
        public int Orientation; //ExifDirectoryBase.TagOrientation from ExifIfd0Directory
        public double FocalLenght; //ExifDirectoryBase.TagFocalLength from ExifSubIfdDirectory
        //public int WhiteBalance; Mode or Value?
        public int ISO; //ExifDirectoryBase.TagIsoEquivalent from ExifSubIfdDirectory
        public int TimeExposureDenominator; //ExifDirectoryBase.TagExposureTime from ExifSubIfdDirectory
        public int TimeExposureNumerator; //ExifDirectoryBase.TagExposureTime from ExifSubIfdDirectory
        public int FNumber; //ExifSubIfdDirectory.TagFNumber from ExifSubIfdDirectory
        public int Flash; //ExifDirectoryBase.TagFlash from ExifSubIFDDirectory
        public float Latitude;
        public float longitude;
    }
    class Image
    {
        string path;
        public Image(string path)
        {
            
            
        }
    }
}
