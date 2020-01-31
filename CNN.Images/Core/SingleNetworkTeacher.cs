﻿using CNN.Images.Model;
using CNN.Images.Services;
using System;
using System.Collections.Generic;

namespace CNN.Images.Core
{
    public class SingleNetworkTeacher
    {
        public int Id { get; set; }

        public NeuralNetwork Network { get; set; }

        public TrainConfiguration TrainConfiguration { get; set; }

        public List<double[]> InputDatasets { get; set; }

        public List<double[]> OutputDatasets { get; set; }

        public Logger Logger { get; set; }

        private int Iteration = 0;

        public void Train()
        {
            if (Network == null) return;
            if (TrainConfiguration == null) return;

            Iteration = TrainConfiguration.EndIteration;
            if (Iteration == 0 || TrainConfiguration.EndIteration - TrainConfiguration.StartIteration <= 0) return;

            if (InputDatasets == null) return;
            if (OutputDatasets == null) return;

            for (int iteration = TrainConfiguration.StartIteration; iteration < Iteration; iteration++)
            {
                // Calculating learn-speed rate:
                var learningSpeed = 0.01 * Math.Pow(0.1, iteration / 150000);
                for (int k = 0; k < InputDatasets.Count; k++)
                {
                    Network.Handle(InputDatasets[k]);

                    // Передает для обучения только 1 элемент выходного вектора
                    // (Класс на который конкретной сети нужно активироваться)
                    double[] outputDataSetArray = { OutputDatasets[k][Id] };

                    try
                    {
                        Network.Teach(InputDatasets[k], outputDataSetArray, learningSpeed);
                    }
                    catch (Exception ex)
                    {
                        Logger.LogError(ErrorType.TrainError, ex);
                        return;
                    }
                }
            }

            // Saving memory:
            if (TrainConfiguration.MemoryFolder != "")
            {
                Network.SaveMemory(TrainConfiguration.MemoryFolder + "//" + "memory_" + Id + ".txt");
            }
            else
            {
                Network.SaveMemory("memory_" + Id + ".txt");
            }
        }
    }
}
