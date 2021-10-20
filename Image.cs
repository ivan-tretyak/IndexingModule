using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
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
        public double FNumber; //ExifSubIfdDirectory.TagFNumber from ExifSubIfdDirectory
        public int Flash; //ExifDirectoryBase.TagFlash from ExifSubIFDDirectory
        public float Latitude;
        public float Longitude;
    }
    class Image
    {
        string path;
        IReadOnlyList<Directory> metadata;
        public Image(string path)
        {
            this.path = path;
            metadata = ImageMetadataReader.ReadMetadata(path);
            Console.WriteLine(GetFlash());
        }

        public int GetFlash()
        {
            var directory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (directory != null)
            {
                if (directory.ContainsTag(ExifDirectoryBase.TagFlash))
                    return directory.GetInt32(ExifDirectoryBase.TagFlash);
            }
            return -1;
        }

        public double GetFNumber()
        {
            var directory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (directory != null)
            {
                if (directory.ContainsTag(ExifDirectoryBase.TagFNumber))
                    return directory.GetRational(ExifDirectoryBase.TagFNumber).ToDouble();
            }
            return -1;
        }

        public double GetTimeExposureNumerator()
        {
            var directory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (directory != null)
            {
                if (directory.ContainsTag(ExifDirectoryBase.TagExposureTime))
                    return directory.GetRational(ExifDirectoryBase.TagExposureTime).Numerator;
            }
            return -1;
        }

        public int GetTimeExposureDenominator()
        {
            var directory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (directory != null)
            {
                if (directory.ContainsTag(ExifDirectoryBase.TagExposureTime))
                    return (int)directory.GetRational(ExifDirectoryBase.TagExposureTime).Denominator;
            }
            return -1;
        }

        public int GetISO()
        {
            var directory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (directory != null)
            {
                if (directory.ContainsTag(ExifDirectoryBase.TagIsoEquivalent))
                    return directory.GetInt32(ExifDirectoryBase.TagIsoEquivalent);
            }
            return -1;
        }

        private double GetFocalLength()
        {
            var directory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (directory != null)
            {
                if (directory.ContainsTag(ExifDirectoryBase.TagFocalLength))
                    return directory.GetRational(ExifDirectoryBase.TagFocalLength).ToDouble();
            }
            return -1;
        }

        private int GetOrientation()
        {
            var directory = metadata.OfType<ExifIfd0Directory>().FirstOrDefault();
            if (directory != null)
            {
                if (directory.ContainsTag(ExifDirectoryBase.TagOrientation))
                    return directory.GetInt32(ExifDirectoryBase.TagOrientation);
            }
            return -1;
        }

        private string GetModel()
        {
            var directory = metadata.OfType<ExifDirectoryBase>().FirstOrDefault();
            if (directory != null)
            {
                if (directory.ContainsTag(ExifDirectoryBase.TagModel))
                    return directory.GetString(ExifDirectoryBase.TagModel);
            }
            return "";
        }

        private string GetManufacturer()
        {
            var directory = metadata.OfType<ExifIfd0Directory>().FirstOrDefault();
            if (directory != null)
            {
                if (directory.ContainsTag(ExifDirectoryBase.TagMake))
                    return directory.GetString(ExifDirectoryBase.TagMake);
            }
            return "";
        }

        private DateTime GetDateTime()
        {
            
            var directory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (directory != null)
            {
                if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTime))
                    return dateTime;
            }
            return DateTime.MinValue;
        }

        private double GetLatitude()
        {
            var image = System.Drawing.Image.FromFile(path);
            var latitude = image.GetPropertyItem((int)2);
            return GetGeoTag(latitude);
        }

        private double GetLongitude()
        {
            var image = System.Drawing.Image.FromFile(path);
            var longitude = image.GetPropertyItem((int)4);
            return GetGeoTag(longitude);
        }

        private static double GetGeoTag(PropertyItem propItem)
        {
            uint degreesNumerator = BitConverter.ToUInt32(propItem.Value, 0);
            uint degreesDenominator = BitConverter.ToUInt32(propItem.Value, 4);
            double degrees = degreesNumerator / (double)degreesDenominator;

            uint minutesNumerator = BitConverter.ToUInt32(propItem.Value, 8);
            uint minutesDenominator = BitConverter.ToUInt32(propItem.Value, 12);
            double minutes = minutesNumerator / (double)minutesDenominator;

            uint secondsNumerator = BitConverter.ToUInt32(propItem.Value, 16);
            uint secondsDenominator = BitConverter.ToUInt32(propItem.Value, 20);
            double seconds = secondsNumerator / (double)secondsDenominator;

            double coorditate = degrees + (minutes / 60d) + (seconds / 3600d);
            string gpsRef = System.Text.Encoding.ASCII.GetString(new byte[1] { propItem.Value[0] }); //N, S, E, or W  

            if (gpsRef == "S" || gpsRef == "W")
            {
                coorditate = coorditate * -1;
            }
            return coorditate;
        }
    }
}
