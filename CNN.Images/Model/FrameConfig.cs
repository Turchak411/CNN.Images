using System.Drawing;

namespace CNN.Images.Model
{
    public class FrameConfig
    {
        /// <summary>
        /// Координаты левого верхнего угла рамки
        /// </summary>
        public Point StartPoint { get; set; }

        /// <summary>
        /// Ширина рамки в процентах
        /// </summary>
        public int WidthPercent { get; set; }

        /// <summary>
        /// Высота рамки в процентах
        /// </summary>
        public int HeightPercent { get; set; }
    }
}
