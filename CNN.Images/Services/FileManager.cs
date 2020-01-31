using CNN.Images.Model;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using WeightsGenerator;

namespace CNN.Images.Services
{
    public class FileManager
    {
        private readonly string _dataPath;

        private Logger _logger;

        private FileManager() { }

        public FileManager(NetworkStructure netStructure = null, string dataPath = "memory.txt")
        {
            _logger = new Logger();

            _dataPath = dataPath;

            // Запуск процесса генерации памяти в случае ее отсутствия:
            if (!File.Exists(dataPath))
            {
                _logger.LogError(ErrorType.MemoryMissing);

                Console.WriteLine("Start generating process...");
                ServiceWeightGenerator weightsGenerator = new ServiceWeightGenerator();

                if (netStructure != null)
                {
                    weightsGenerator.GenerateMemory(_dataPath, netStructure.InputVectorLength, netStructure.NeuronsByLayers);
                }
                else
                {
                    weightsGenerator.GenerateMemory(_dataPath);
                }
            }
        }

        public double[] LoadMemory(int layerNumber, int neuronNumber)
        {
            double[] memory = new double[0];

            using (StreamReader fileReader = new StreamReader(_dataPath))
            {
                while (!fileReader.EndOfStream)
                {
                    string[] readedLine = fileReader.ReadLine().Split(' ');

                    if ((readedLine[0] == "layer_" + layerNumber) && (readedLine[1] == "neuron_" + neuronNumber))
                    {
                        memory = GetWeights(readedLine);
                    }
                }
            }

            return memory;
        }

        public double[] LoadMemory(int layerNumber, int neuronNumber, string memoryPath)
        {
            double[] memory = new double[0];

            if (!File.Exists(memoryPath))
            {
                // Создание памяти для отдельного класса в случае отсутствия таковой
                File.Copy(_dataPath, memoryPath);
            }

            using (StreamReader fileReader = new StreamReader(memoryPath))
            {
                while (!fileReader.EndOfStream)
                {
                    string[] readedLine = fileReader.ReadLine().Split(' ');

                    if ((readedLine[0] == "layer_" + layerNumber) && (readedLine[1] == "neuron_" + neuronNumber))
                    {
                        memory = GetWeights(readedLine);
                    }
                }
            }

            return memory;
        }

        private double[] GetWeights(string[] readedLine)
        {
            double[] weights = new double[readedLine.Length - 2];

            for (int i = 0; i < weights.Length; i++)
            {
                weights[i] = double.Parse(readedLine[i + 2]);
            }

            return weights;
        }

        public void PrepareToSaveMemory()
        {
            File.Delete(_dataPath);
        }

        public void PrepareToSaveMemory(string path)
        {
            File.Delete(path);
        }

        public void SaveMemory(int layerNumber, int neuronNumber, double[] weights)
        {
            using (StreamWriter fileWriter = new StreamWriter(_dataPath, true))
            {
                fileWriter.Write("layer_{0} neuron_{1}", layerNumber, neuronNumber);

                for (int i = 0; i < weights.Length; i++)
                {
                    fileWriter.Write(" " + weights[i]);
                }

                fileWriter.WriteLine("");
            }
        }

        public void SaveMemory(int layerNumber, int neuronNumber, double[] weights, string path)
        {
            using (StreamWriter fileWriter = new StreamWriter(path, true))
            {
                fileWriter.Write("layer_{0} neuron_{1}", layerNumber, neuronNumber);

                for (int i = 0; i < weights.Length; i++)
                {
                    fileWriter.Write(" " + weights[i]);
                }

                fileWriter.WriteLine("");
            }
        }

        public List<double[]> LoadSingleDataset(string path)
        {
            List<double[]> sets = new List<double[]>();

            using (StreamReader fileReader = new StreamReader(path))
            {
                while (!fileReader.EndOfStream)
                {
                    string[] readedLine = fileReader.ReadLine().Split(' ');
                    double[] set = new double[readedLine.Length - 1];

                    for (int i = 0; i < readedLine.Length - 1; i++)
                    {
                        set[i] = double.Parse(readedLine[i]);
                    }

                    sets.Add(set);
                }
            }

            return sets;
        }

        public void SaveSets(List<double[]> inputSets, List<double[]> outputSets,
                               string inputSetsFilename, string outputSetsFilename)
        {
            // Saving input sets:
            using (StreamWriter fileWriter = new StreamWriter(inputSetsFilename))
            {
                for (int i = 0; i < inputSets.Count; i++)
                {
                    for (int k = 0; k < inputSets[i].Length; k++)
                    {
                        fileWriter.Write(inputSets[i][k] + " ");
                    }

                    fileWriter.WriteLine();
                }
            }

            // Saving output sets:
            using (StreamWriter fileWriter = new StreamWriter(outputSetsFilename))
            {
                for (int i = 0; i < outputSets.Count; i++)
                {
                    for (int k = 0; k < outputSets[i].Length; k++)
                    {
                        fileWriter.Write(outputSets[i][k] + " ");
                    }

                    fileWriter.WriteLine();
                }
            }
        }
    }
}
