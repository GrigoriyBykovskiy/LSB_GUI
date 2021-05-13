using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LSB_GUI
{
    public class SteganoTransformation
    {
        public void Encrypt(BMPImage Container, MessageFile Message)
        {
            // message length and key size
            if (Message.Size + 32 > Container.InfoHeader.biHeight * Container.InfoHeader.biWidth)
            {
                throw new Exception("Размер контейнера слишком мал, сэмпай!");
            }
            // delete in the future release
            if (Container.InfoHeader.biWidth < 33)
            {
                throw new Exception("Ширина файла слишком мала, сэмпай!");
            }

            if (Container.InfoHeader.biBitCount != 24)
            {
                throw new Exception("Тип файла не поддерживается, сэмпай!");
            }
            
            var messageDataLength = new StringBuilder();
            foreach (byte b in BitConverter.GetBytes(Message.Size))
                messageDataLength.Insert(0, Convert.ToString(b, 2).PadLeft(8, '0'));
            
            while (messageDataLength.Length % 32 != 0)
            {
                messageDataLength.Insert(0, '0');
            }
            
            for (var counter  = 0; counter < 32; counter++)
            {
                RGBTRIPLE buf = Container.Data[counter];
                Byte mask = 0b00000001;
                if (messageDataLength[counter] == '0')
                {
                    buf.rgbtBlue |= mask;
                }

                if (messageDataLength[counter] == '1')
                {
                    buf.rgbtBlue &= (byte)~mask;
                }
                Container.Data[counter] = buf;
            }
            
            int count = 0;
            
            for (int row = 0; row < Container.InfoHeader.biHeight; row++)
            {
                for (int column = 0; column < Container.InfoHeader.biWidth; column++)
                {
                    if (row == 0 && column < 32)
                    {
                        continue;
                    }
                    if (count < Message.Size)
                    {
                        RGBTRIPLE buf = Container.Data[count + 32];
                        Byte mask = 0b00000001;
                        if (Message.Data[count] == '0')
                        {
                            buf.rgbtBlue |= mask;
                        }

                        if (Message.Data[count] == '1')
                        {
                            buf.rgbtBlue &= (byte)~mask;
                        }
                        Container.Data[count + 32] = buf;
                        count++;
                    }
                }
            }

            // BITMAPFILEHEADER buffer = Container.FileHeader;
            // buffer.bfSize = Message.Size;
            // Container.FileHeader = buffer;
            Container.PutDataToFile(Directory.GetCurrentDirectory() + "/" + Path.GetExtension(Message.PathFile).Replace(".","") + ".bmp");
        }

        public void Decrypt(BMPImage Container)
        {
            var messageDataLength = new StringBuilder();
            
            for (var counter  = 0; counter < 32; counter++)
            {
                RGBTRIPLE pixel = Container.Data[counter];
                if ((pixel.rgbtBlue & 0x01) == 0)
                {
                    messageDataLength.Append('1');
                }
                if ((pixel.rgbtBlue & 0x01) == 1)
                {
                    messageDataLength.Append('0');
                }
            }
            
            var messageDataLengthBinary = messageDataLength.ToString();
            var arrDataLength = new byte[messageDataLength.Length / 8];
            var countTmp = 0;
            
            for (var i = 0; i < messageDataLength.Length; i += 8)
            {
                var ss = messageDataLengthBinary .Substring(countTmp * 8, 8);
                var bv = Convert.ToByte(ss, 2);
                arrDataLength[i / 8] = bv;
                countTmp++;
            }

            Array.Reverse(arrDataLength, 0, 4);
            
            UInt32 outputDataLength = BitConverter.ToUInt32(arrDataLength, 0);

            StringBuilder buf = new StringBuilder();

            int count = 0;
            for (int row = 0; row < Container.InfoHeader.biHeight; row++)
            {
                for (int column = 0; column < Container.InfoHeader.biWidth; column++)
                {
                    if (row == 0 && column < 32)
                    {
                        continue;
                    }
                    if (count < outputDataLength)
                    {
                        RGBTRIPLE pixel = Container.Data[count + 32];
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
            using (BinaryWriter writer = new BinaryWriter(File.Open(Directory.GetCurrentDirectory() + "/"+  "output." + Path.GetFileNameWithoutExtension(Container.PathFile), FileMode.Create)))
            {
                writer.Write(arr, 0, arr.Length);
            }
        }
    }
}