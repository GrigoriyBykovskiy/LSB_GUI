using System;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace LSB_GUI
{
    public class SteganoTransformation
    {
        public void Encrypt(BMPImage Container, MessageFile Message)
        {
            if (Message.Size > Container.InfoHeader.biHeight * Container.InfoHeader.biWidth)
            {
                throw new Exception("Размер контейнера слишком мал, сэмпай!");
            }

            int count = 0;
            for (int row = 0; row < Container.InfoHeader.biHeight; row++)
            {
                for (int column = 0; column < Container.InfoHeader.biWidth; column++)
                {
                    if (count < Message.Size)
                    {
                        RGBTRIPLE buf = Container.Data[count];
                        Byte mask = 0b00000001;
                        if (Message.Data[count] == '0')
                        {
                            buf.rgbtBlue |= mask;
                        }

                        if (Message.Data[count] == '1')
                        {
                            buf.rgbtBlue &= (byte)~mask;
                        }
                        Container.Data[count] = buf;
                        count++;
                    }
                }
            }

            BITMAPFILEHEADER buffer = Container.FileHeader;
            buffer.bfSize = Message.Size;
            Container.FileHeader = buffer;
            Container.PutDataToFile("E:/RiderProjects/LSB_GUI/output.bmp");
        }

        public void Decrypt(BMPImage Container)
        {

            StringBuilder buf = new StringBuilder();

            int count = 0;
            for (int row = 0; row < Container.InfoHeader.biHeight; row++)
            {
                for (int column = 0; column < Container.InfoHeader.biWidth; column++)
                {
                    if (count < Container.FileHeader.bfSize)
                    {
                        RGBTRIPLE pixel = Container.Data[count];
                        if ((pixel.rgbtBlue & 0x01) == 0)
                        {
                            buf.Append('1');
                        }
                        if ((pixel.rgbtBlue & 0x01) == 1)
                        {
                            buf.Append('0');
                        }
                        count++;
                    }
                }
            }
            
            var messageBinary = buf.ToString();
            var arr = new byte[buf.Length / 8];
            var count_tmp = 0;
            
            for (var i = 0; i < messageBinary.Length; i += 8)
            {
                var ss = messageBinary.Substring(count_tmp * 8, 8);
                var bv = Convert.ToByte(ss, 2);
                arr[i / 8] = bv;
                count_tmp++;
            }
            using (BinaryWriter writer = new BinaryWriter(File.Open("E:/RiderProjects/LSB_GUI/output.txt", FileMode.Create)))
            {
                writer.Write(arr, 0, arr.Length);
            }
        }
    }
}