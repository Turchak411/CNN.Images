using System.Drawing;
using System;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CNN.Images.Services
{
    public class ImageLoader
    {
        // TODO: Здесь приводить каждый пиксель в промежуток от 0.000 до 1.000

        // Image data:
        private int m_imgDimentionX;
        private int m_imgDimentionY;

        public ImageLoader()
        {
            m_imgDimentionX = 32;
            m_imgDimentionY = 32;
        }

        public ImageLoader(int dimentionX, int dimentionY)
        {
            m_imgDimentionX = dimentionX;
            m_imgDimentionY = dimentionY;
        }

        public double[,] LoadImageData(string imgPath)
        {
            // Загрузка и стандартизация изображения:
            Bitmap img = LoadImage(imgPath);

            // Приведение изображения к заданынм размерам:
            img = Compress(img);

            // Обесцвечивание изображения:
            img = Discolor(img);

            img.Save("test.png");

            // Отцифровка изображения:
            return ConvertToMatrix(img);
        }

        private Bitmap LoadImage(string path)
        {
            return new Bitmap(path);
        }

        private Bitmap Compress(Bitmap img)
        {
            return new Bitmap(img, new Size(m_imgDimentionY, m_imgDimentionX));
        }

        private Bitmap Discolor(Bitmap img)
        {
            // Задаём формат Пикселя.
            PixelFormat pxf = PixelFormat.Format24bppRgb;

            // Получаем данные картинки.
            Rectangle rect = new Rectangle(0, 0, img.Width, img.Height);
            //Блокируем набор данных изображения в памяти
            BitmapData bmpData = img.LockBits(rect, ImageLockMode.ReadWrite, pxf);

            // Получаем адрес первой линии.
            IntPtr ptr = bmpData.Scan0;

            // Задаём массив из Byte и помещаем в него надор данных.
            // int numBytes = bmp.Width * bmp.Height * 3; 
            // На 3 умножаем - поскольку RGB цвет кодируется 3-мя байтами
            // Либо используем вместо Width - Stride
            int numBytes = bmpData.Stride * img.Height;
            int widthBytes = bmpData.Stride;
            byte[] rgbValues = new byte[numBytes];

            // Копируем значения в массив.
            Marshal.Copy(ptr, rgbValues, 0, numBytes);

            // Перебираем пикселы по 3 байта на каждый и меняем значения
            for (int counter = 0; counter < rgbValues.Length; counter += 3)
            {
                int value = rgbValues[counter] + rgbValues[counter + 1] + rgbValues[counter + 2];
                byte color_b = 0;

                color_b = Convert.ToByte(value / 3);

                rgbValues[counter] = color_b;
                rgbValues[counter + 1] = color_b;
                rgbValues[counter + 2] = color_b;
            }
            // Копируем набор данных обратно в изображение
            Marshal.Copy(rgbValues, 0, ptr, numBytes);

            // Разблокируем набор данных изображения в памяти.
            img.UnlockBits(bmpData);

            return img;
        }

        private double[,] ConvertToMatrix(Bitmap img)
        {
            double[,] imgData = new double[m_imgDimentionY, m_imgDimentionX];

            for (int i = 0; i < m_imgDimentionY; i++)
            {
                for (int k = 0; k < m_imgDimentionX; k++)
                {
                    int absColorValue = img.GetPixel(k, i).ToArgb();
                    imgData[i, k] += DiscreteRationing(absColorValue);
                }
            }

            return imgData;
        }

        private double DiscreteRationing(int absValue)
        {
            double maxValue = 16777216;

            return (absValue * -1) / maxValue;
        }
    }
}
