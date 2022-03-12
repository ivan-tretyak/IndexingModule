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
using System.Linq;
using System.Text;
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
        public int Flash; //ExifDirectoryBase.TagFlash from ExifSubIFDDirectory
        public float Latitude;
        public float Longitude;
    }
    interface IValue
    {
    }

    class StringValue: IValue
    {
        public string value;
        public StringValue(string v)
        {
            value = v;
        }
    }

    class DoubleValue: IValue
    {
        public double value;
        public DoubleValue(double v)
        {
            value = v;
        }
    }

    class IntValue: IValue
    {
        public int value;
        public IntValue(int v)
        {
            value = v;
        }
    }

    class FloatValue: IValue
    {
        public float value;
        public FloatValue(float v)
        {
            value = v;
        }
    }

    class DateTimeValue: IValue
    {
        public DateTime value;
        public DateTimeValue(DateTime v)
        {
            value = v;
        }
    }

    class FabricReadMetadata
    {
        public FabricReadMetadata()
        {
        }

        public IValue ReadMetadata(ExifDirectoryBase directory, int tag)
        {
            switch (tag)
            {
                case 37_385:
                case 274:
                    if (directory != null && directory.ContainsTag(tag))
                    {
                        return new IntValue(directory.GetInt32(tag));
                    }
                    else
                    {
                        return new IntValue(-1);
                    }

                case 37_386:
                    if (directory != null && directory.ContainsTag(tag))
                    {
                        return new DoubleValue(directory.GetRational(tag).ToDouble());
                    }
                    else
                    {
                        return new DoubleValue(-1.0);
                    }
                case 272:
                case 271:
                    if (directory != null && directory.ContainsTag(tag))
                    {
                        return new StringValue(directory.GetString(tag));
                    }
                    else
                    {
                        return new StringValue("");
                    }
                case 36_867:
                    if (directory != null && directory.ContainsTag(tag))
                    {
                        if (directory.TryGetDateTime(ExifDirectoryBase.TagDateTimeOriginal, out var dateTime))
                            return new DateTimeValue(dateTime);
                        return new DateTimeValue(DateTime.MinValue);
                    }
                    else
                    {
                        return new DateTimeValue(DateTime.MinValue);
                    }
                default:
                    return new DoubleValue(-1.0);
            }
        }

        public IValue FabricMethod(GpsDirectory directory, string coordinate)
        {
            try
            {
                var gps = directory.GetGeoLocation();
                if (coordinate == "lat")
                    return new DoubleValue(gps.Latitude);
                else if (coordinate == "long")
                    return new DoubleValue(gps.Longitude);
            }
            catch (Exception)
            {
                return new DoubleValue(-1.0);
            }
            return new DoubleValue(-1.0);
        }
    }
    class Image
    {
        public string path;
        IReadOnlyList<Directory> metadata;
        public MetaData metaData;
        public Image(string path)
        {
            this.path = path;
            metaData = new();
            FabricReadMetadata fabric = new();
            metadata = ImageMetadataReader.ReadMetadata(path);
            IntValue flash = (IntValue)fabric.ReadMetadata(metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault(),
                                         ExifDirectoryBase.TagFlash);
            IntValue Orientation = (IntValue)fabric.ReadMetadata(metadata.OfType<ExifIfd0Directory>().FirstOrDefault(),
                                               ExifDirectoryBase.TagOrientation);
            DoubleValue focalLenght = (DoubleValue)fabric.ReadMetadata(metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault(),
                                                    ExifDirectoryBase.TagFocalLength);
            DoubleValue longitude = (DoubleValue)fabric.FabricMethod(metadata.OfType<GpsDirectory>().FirstOrDefault(), "long");
            DoubleValue latitude = (DoubleValue)fabric.FabricMethod(metadata.OfType<GpsDirectory>().FirstOrDefault(), "lat");
            DateTimeValue dateTime = (DateTimeValue)fabric.ReadMetadata(metadata.OfType<ExifSubIfdDirectory>().FirstOrDefault(),
                                       ExifDirectoryBase.TagDateTimeOriginal);
            StringValue manufacturer = (StringValue)fabric.ReadMetadata(metadata.OfType<ExifDirectoryBase>().FirstOrDefault(),
                                        ExifDirectoryBase.TagMake);
            StringValue model = (StringValue)fabric.ReadMetadata(metadata.OfType<ExifDirectoryBase>().FirstOrDefault(),
                                        ExifDirectoryBase.TagModel);

            metaData.Flash = flash.value;
            metaData.Orientation = Orientation.value;
            metaData.FocalLength = focalLenght.value;
            metaData.Longitude = (float)longitude.value;
            metaData.Latitude = (float)latitude.value;
            metaData.DateCreation = dateTime.value;
            metaData.Manufacturer = manufacturer.value;
            metaData.Model = model.value;
        }
      }
    }

