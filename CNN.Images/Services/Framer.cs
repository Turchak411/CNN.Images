using System.Collections.Generic;
using System.Drawing;
using CNN.Images.Model;

namespace CNN.Images.Services
{
    public class Framer
    {
        public List<FrameObject> GetFrameBitmapsAsObject(Bitmap sourceImg)
        {
            List<FrameObject> frameObjectList = new List<FrameObject>();

            List<FrameConfig> frameConfigs = GetAllPossibleFrameConfigs(sourceImg.Width, sourceImg.Height);

            for (int i = 0; i < frameConfigs.Count; i++)
            {
                frameObjectList.Add(
                    new FrameObject
                    {
                        Location = frameConfigs[i].StartPoint,
                        BitmapImage = sourceImg.Clone(
                            new Rectangle(frameConfigs[i].StartPoint,
                                          new Size(frameConfigs[i].WidthPercent * sourceImg.Width / 100,
                                              frameConfigs[i].HeightPercent * sourceImg.Height / 100)
                                          ),
                            sourceImg.PixelFormat
                            )
                    });
            }

            return frameObjectList;
            // TODO: Нехватка памяти, глянуть
        }

        private List<FrameConfig> GetAllPossibleFrameConfigs(int sourceWidth, int sourceHeight)
        {
            List<FrameConfig> frameConfigs = new List<FrameConfig>();

            int[] framesWidthStart = new[] { 10 };//{ 10, 10, 15 };
            int[] framesHeightStart = new[] { 10 };//{ 15, 10, 10 };

            for (int i = 0; i < framesWidthStart.Length; i++)
            {
                int tempWidth = framesWidthStart[i];
                int tempHeight = framesHeightStart[i];

                while(tempHeight < 100 && tempWidth < 100)
                {
                    int maxPossibleY = sourceHeight - tempHeight;
                    int maxPossibleX = sourceWidth - tempWidth;

                    int tempLocX = 0;
                    int tempLocY = 0;

                    while (tempLocX < maxPossibleX || tempLocY < maxPossibleY)
                    {
                        frameConfigs.Add(new FrameConfig
                        {
                            StartPoint = new Point(tempLocX, tempLocY),
                            HeightPercent = tempHeight,
                            WidthPercent = tempWidth
                        });

                        tempLocX++;
                        tempLocY++;
                    }

                    tempWidth+=2;
                    tempHeight+=2;
                }
            }

            return frameConfigs;
        }
    }
}
