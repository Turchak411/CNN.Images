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
                //Bitmap imgPart = sourceImg.Clone(
                //    new Rectangle(frameConfigs[i].StartPoint,
                //                  new Size(frameConfigs[i].WidthPercent * sourceImg.Width / 100,
                //                      frameConfigs[i].HeightPercent * sourceImg.Height / 100)
                //                  ),
                //    sourceImg.PixelFormat
                //    );

                //imgPart.Save("img " + i + ".png");

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
                    int maxPossibleY = sourceHeight * (100 - tempHeight) / 100;
                    int maxPossibleX = sourceWidth * (100 - tempWidth) / 100;

                    int tempLocX = 0;
                    int tempLocY = 0;

                    while (tempLocY < maxPossibleY)
                    {
                        while (tempLocX < maxPossibleX)
                        {
                            frameConfigs.Add(new FrameConfig
                            {
                                StartPoint = new Point(tempLocX, tempLocY),
                                HeightPercent = tempHeight,
                                WidthPercent = tempWidth
                            });

                            tempLocX += 10;
                        }

                        tempLocY += 10;
                        tempLocX = 0;
                    }

                    tempWidth+=10;
                    tempHeight+=10;
                }
            }

            return frameConfigs;
        }
    }
}
