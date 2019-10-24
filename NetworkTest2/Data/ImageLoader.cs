using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Text;
using NetworkTest2.Helper;

namespace NetworkTest2.Data
{
    public static class ImageLoader
    {
        public static IEnumerable<GrayscaleImage> LoadFromFile(string imageFile, string labelFile)
        {
            var result = new List<GrayscaleImage>();

            List<byte> labels = new List<byte>();
            using (var file = File.OpenRead(labelFile))
            {
                using (var binary = new BinaryReader(file))
                {
                    var magicNumber = binary.ReadInt32().InverseEndian();
                    var labelCount = binary.ReadUInt32().InverseEndian();
                    for (var i = 0; i < labelCount; i++)
                    {
                        var label = binary.ReadByte();
                        labels.Add(label);
                    }
                }
            }

            using (var file = File.OpenRead(imageFile))
            {
                using (var binary = new BinaryReader(file))
                {
                    var magicNumber = binary.ReadInt32().InverseEndian();
                    var imageCount = binary.ReadInt32().InverseEndian();
                    var rowCount = binary.ReadInt32().InverseEndian();
                    var columnCount = binary.ReadInt32().InverseEndian();

                    for (var i = 0; i < imageCount; i++)
                    {
                        var image = new GrayscaleImage();
                        image.ImageLabel = labels[i];
                        image.ImageData = new byte[columnCount,rowCount];
                        image.Width = columnCount;
                        image.Height = rowCount;

                        for (var row = 0; row < rowCount; row++)
                        {
                            for (var column = 0; column < columnCount; column++)
                            {
                                image.ImageData[column, row] = binary.ReadByte();
                            }
                        }

                        result.Add(image);
                    }
                }
            }

            return result;
        }

        public static void SaveImage(GrayscaleImage image, string path, Func<byte, byte> color = null)
        {
            using (var file = File.OpenWrite(path))
            {
                using (var binary = new BinaryWriter(file))
                {
                    binary.Write(Encoding.ASCII.GetBytes("BM"));
                    binary.Write((uint) image.ImageData.Length + 54); // File Length
                    binary.Write((uint) 0); // Reserverd
                    binary.Write((uint) (54 + 4 * 256)); // Offset to Data

                    //Header
                    binary.Write((uint) 40); // Header size
                    binary.Write(image.Width);
                    binary.Write(-image.Height); // "-" for topdown bitmap 
                    binary.Write((ushort) 1); // Planes (fix)
                    binary.Write((ushort) 8); // BBP (1, 4, 8, 16, 24, 32). 
                    binary.Write((uint) 0); // Compression (0 = uncompressed)
                    binary.Write((uint) image.ImageData.Length); // Image Data Length
                    binary.Write((int) 0); // XPelsPerMeter
                    binary.Write((int) 0); // YPelsPerMeter
                    binary.Write((uint) 0); // Count of the color palette
                    binary.Write((uint) 0); // Count of colors

                    // color palette
                    for (var i = 0; i < 256; i++)
                    {
                        binary.Write((byte) i); 
                        binary.Write((byte) i); 
                        binary.Write((byte) i); 
                        binary.Write((byte) 0);
                    }

                    //Data

                    if (color == null)
                    {
                        for (int row = 0; row < image.Height; row++)
                            for (int column = 0; column < image.Width; column++)
                                binary.Write(image.ImageData[column, row]);
                    }
                    else
                    {
                        for (int row = 0; row < image.Height; row++)
                            for (int column = 0; column < image.Width; column++)
                                binary.Write(color(image.ImageData[column, row]));
                    }
                }
            }  
        }
    }
}
