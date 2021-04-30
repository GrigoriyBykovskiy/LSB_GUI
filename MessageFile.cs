using System;
using System.IO;
using System.Text;

namespace LSB_GUI
{
    public class MessageFile
    {
        public UInt32 Size = 0;
        public byte[] MessageData;
        public StringBuilder Data = new StringBuilder();
        
        public void GetDataFromFile(string pathFile)
        {
            MessageData = File.ReadAllBytes(pathFile);
            InitData();
            InitSize();
        }

        public void InitData()
        {
            foreach (byte b in MessageData)
                Data.Append(Convert.ToString(b, 2).PadLeft(8,'0'));
        }

        public void InitSize()
        {
            Size = (UInt32)MessageData.Length * 8;
        }
    }
}