using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace LSB_GUI
{
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct BITMAPFILEHEADER
    {
        public UInt16 bfType;
        public UInt32 bfSize;
        public UInt16 bfReserved1;
        public UInt16 bfReserved2;
        public UInt32 bfOffBits;
    }
        
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct BITMAPINFOHEADER
    {
        public UInt32 biSize;
        public Int32 biWidth;
        public Int32 biHeight;
        public UInt16 biPlanes;
        public UInt16 biBitCount;
        public UInt32 biCompression;
        public UInt32 biSizeImage;
        public Int32 biXPelsPerMeter;
        public Int32 biYPelsPerMeter;
        public UInt32 biClrUsed;
        public UInt32 biClrImportant;
    }
        
    [StructLayout(LayoutKind.Sequential, Pack=1)]
    public struct RGBTRIPLE
    {
        public Byte rgbtBlue;
        public Byte rgbtGreen;
        public Byte rgbtRed;
    }
    public class BMPImage
    {
        public BITMAPFILEHEADER FileHeader;
        public BITMAPINFOHEADER InfoHeader;
        public int Padding = 0;
        public List<RGBTRIPLE> Data = new List<RGBTRIPLE>();

        public void InitPadding()
        {
            Padding = InfoHeader.biWidth % 4;
        }

        public void GetDataFromFile(string pathFile)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(pathFile, FileMode.Open)))
            {
                // get BITMAPFILEHEADER
                FileHeader.bfType = reader.ReadUInt16();
                FileHeader.bfSize = reader.ReadUInt32();
                FileHeader.bfReserved1 = reader.ReadUInt16();
                FileHeader.bfReserved2 = reader.ReadUInt16();
                FileHeader.bfOffBits = reader.ReadUInt32();
                // get BITMAPINFOHEADER
                InfoHeader.biSize = reader.ReadUInt32();
                InfoHeader.biWidth = reader.ReadInt32();
                InfoHeader.biHeight = reader.ReadInt32();
                InfoHeader.biPlanes = reader.ReadUInt16();
                InfoHeader.biBitCount = reader.ReadUInt16();
                InfoHeader.biCompression = reader.ReadUInt32();
                InfoHeader.biSizeImage = reader.ReadUInt32();
                InfoHeader.biXPelsPerMeter = reader.ReadInt32();
                InfoHeader.biYPelsPerMeter = reader.ReadInt32();
                InfoHeader.biClrUsed = reader.ReadUInt32();
                InfoHeader.biClrImportant = reader.ReadUInt32();
                // get BITMAPDATA
                for (int row = 0; row < InfoHeader.biHeight; row++)
                {
                    for (int column = 0; column < InfoHeader.biWidth; column++)
                    {
                        RGBTRIPLE buf;
                        buf.rgbtBlue = reader.ReadByte();
                        buf.rgbtGreen = reader.ReadByte();
                        buf.rgbtRed = reader.ReadByte();
                        Data.Add(buf);
                    }
                }
            }
            InitPadding();
        }

        public void PutDataToFile(string pathFile)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(pathFile, FileMode.OpenOrCreate)))
            {
                // put BITMAPFILEHEADER
                writer.Write(FileHeader.bfType);
                writer.Write(FileHeader.bfSize);
                writer.Write(FileHeader.bfReserved1);
                writer.Write(FileHeader.bfReserved2);
                writer.Write(FileHeader.bfOffBits);
                // put BITMAPINFOHEADER                
                writer.Write(InfoHeader.biSize);
                writer.Write(InfoHeader.biWidth);
                writer.Write(InfoHeader.biHeight);
                writer.Write(InfoHeader.biPlanes);
                writer.Write(InfoHeader.biBitCount);
                writer.Write(InfoHeader.biCompression);
                writer.Write(InfoHeader.biSizeImage);
                writer.Write(InfoHeader.biXPelsPerMeter);
                writer.Write(InfoHeader.biYPelsPerMeter);
                writer.Write(InfoHeader.biClrUsed);
                writer.Write(InfoHeader.biClrImportant);
                // put BITMAPDATA
                var count = 0;
                for (int y = 0; y < InfoHeader.biHeight; y++)
                {
                    for (int x = 0; x < InfoHeader.biWidth; x++)
                    {
                        writer.Write(Data[count].rgbtBlue);
                        writer.Write(Data[count].rgbtGreen);
                        writer.Write(Data[count].rgbtRed);
                        count++;
                    }
                    for (int i = 0; i < Padding; i++)
                    {
                        Byte buf = 0;
                        writer.Write(buf);
                        writer.Write(buf);
                        writer.Write(buf);
                    }
                }
            }
        }
    }
}