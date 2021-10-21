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
        public double FocalLength; //ExifDirectoryBase.TagFocalLength from ExifSubIfdDirectory
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
        MetaData metaData;
        public Image(string path)
        {
            this.path = path;
            metadata = ImageMetadataReader.ReadMetadata(path);
            metaData = new();
            metaData.DateCreation = GetDateTime();
            metaData.Manufacturer = GetManufacturer();
            metaData.Model = GetModel();
            metaData.Orientation = GetOrientation();
            metaData.FocalLength = GetFocalLength();
            metaData.ISO = GetISO();
            metaData.TimeExposureDenominator = GetTimeExposureDenominator();
            metaData.TimeExposureNumerator = GetTimeExposureNumerator();
            metaData.FNumber = GetFNumber();
            metaData.Flash = GetFlash();
            metaData.Latitude = GetLatitude();
            metaData.Longitude = GetLongitude();
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

        public int GetTimeExposureNumerator()
        {
            var directory = metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault();
            if (directory != null)
            {
                if (directory.ContainsTag(ExifDirectoryBase.TagExposureTime))
                    return (int)directory.GetRational(ExifDirectoryBase.TagExposureTime).Numerator;
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

        private float GetLatitude()
        {
            var image = System.Drawing.Image.FromFile(path);
            try
            {
                PropertyItem latitude = image.GetPropertyItem((int)2);
                return GetGeoTag(latitude);
            }
            catch
            {
                return 0;
            }
        }

        private float GetLongitude()
        {
            var image = System.Drawing.Image.FromFile(path);
            try
            {
                PropertyItem longitude = image.GetPropertyItem((int)4);
                return GetGeoTag(longitude);
            }
            catch
            {
                return 0;
            }
        }

        private static float GetGeoTag(PropertyItem propItem)
        {
            uint degreesNumerator = BitConverter.ToUInt32(propItem.Value, 0);
            uint degreesDenominator = BitConverter.ToUInt32(propItem.Value, 4);
            float degrees = degreesNumerator / (float)degreesDenominator;

            uint minutesNumerator = BitConverter.ToUInt32(propItem.Value, 8);
            uint minutesDenominator = BitConverter.ToUInt32(propItem.Value, 12);
            float minutes = minutesNumerator / (float)minutesDenominator;

            uint secondsNumerator = BitConverter.ToUInt32(propItem.Value, 16);
            uint secondsDenominator = BitConverter.ToUInt32(propItem.Value, 20);
            float seconds = secondsNumerator / (float)secondsDenominator;

            float coorditate = degrees + (minutes / 60) + (seconds / 3600);
            string gpsRef = System.Text.Encoding.ASCII.GetString(new byte[1] { propItem.Value[0] }); //N, S, E, or W  

            if (gpsRef == "S" || gpsRef == "W")
            {
                coorditate = coorditate * -1;
            }
            return coorditate;
        }

        public override string ToString()
        {
            string s = $"Путь - {path}.\n========================\n";
            s = s + $"Производитель - {metaData.Manufacturer}\n";
            s = s + $"Модель - {metaData.Model}\n";
            s = s + $"Ориентация - {metaData.Orientation}\n";
            s = s + $"Фокусное расстояние - {metaData.FocalLength}\n";
            s = s + $"ISO - {metaData.ISO}\n";
            s = s + $"Числитель времени экспозиции - {metaData.TimeExposureNumerator}\n";
            s = s + $"Делитель времени экспозиции - {metaData.TimeExposureDenominator}\n";
            s = s + $"Относительное отверстие - {metaData.FNumber}\n";
            s = s + $"Вспышка - {metaData.Flash}\n";
            s = s + $"Широта - {metaData.Latitude}\n";
            s = s + $"Долгота - {metaData.Longitude}\n";
            s = s + $"===========================================\n";
            return s;
        }
    }
}
