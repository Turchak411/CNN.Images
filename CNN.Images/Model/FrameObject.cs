using System.Collections.Generic;
using System.Drawing;

namespace CNN.Images.Model
{
    public class FrameObject
    {
        public Point Location { get; set; }

        public Bitmap BitmapImage { get; set; }
        //public string SubimagePath { get; set; }

        public List<double[,]> MatrixListRGB { get; set; }

        public double[] VectorData { get; set; }
    }
}
