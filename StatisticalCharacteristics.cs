using System;

namespace LSB_GUI
{
    public class StatisticalCharacteristics
    {
        public RGBTRIPLE getMaxDifference(BMPImage original ,BMPImage container)
        {
            if (container.InfoHeader.biHeight != original.InfoHeader.biHeight ||
                container.InfoHeader.biWidth != original.InfoHeader.biWidth)
            {
                throw new Exception("Размеры изображений не совпадают, сэмпай!");
            }
            
            RGBTRIPLE maxDifference = new RGBTRIPLE();
            var count = 0;
            
            for (var row = 0; row < container.InfoHeader.biHeight; row++)
            {
                for (var column = 0; column < container.InfoHeader.biWidth; column++)
                {
                    RGBTRIPLE pixelBuf = new RGBTRIPLE();
                    pixelBuf.rgbtRed = (byte)(original.Data[count].rgbtRed ^ container.Data[count].rgbtRed);
                    pixelBuf.rgbtGreen = (byte)(original.Data[count].rgbtGreen ^ container.Data[count].rgbtGreen);
                    pixelBuf.rgbtBlue = (byte)(original.Data[count].rgbtBlue ^ container.Data[count].rgbtBlue);
                    if (maxDifference.rgbtRed < pixelBuf.rgbtRed)
                        maxDifference.rgbtRed = pixelBuf.rgbtRed;
                    if (maxDifference.rgbtGreen < pixelBuf.rgbtGreen)
                        maxDifference.rgbtGreen = pixelBuf.rgbtGreen;
                    if (maxDifference.rgbtBlue < pixelBuf.rgbtBlue)
                        maxDifference.rgbtBlue = pixelBuf.rgbtBlue;
                    count++;
                }
            }

            return maxDifference;
        }
    }
}