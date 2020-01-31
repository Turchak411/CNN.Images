using CNN.Images.Model;
using CNN.Images.Services;
using System.Collections.Generic;

namespace CNN.Images.Core
{
    public class ConvolutionalNeuralNetwork : NeuralNetwork
    {
        private Extractor _extractor;

        private ConvolutionalNeuralNetwork() { }

        public ConvolutionalNeuralNetwork(Extractor extractor, int receptorsNumber, int[] neuronsNumberByLayers, FileManager fileManager, string memoryPath)
        {
            _extractor = extractor;

            _fileManager = fileManager;

            Layer firstLayer = new Layer(neuronsNumberByLayers[0], receptorsNumber, 0, fileManager, memoryPath);
            _layerList.Add(firstLayer);

            for (int i = 1; i < neuronsNumberByLayers.Length; i++)
            {
                Layer layer = new Layer(neuronsNumberByLayers[i], neuronsNumberByLayers[i - 1], i, fileManager, memoryPath);
                _layerList.Add(layer);
            }
        }

        public double[] Handle(double[,] data)
        {
            double[] dataSet = _extractor.Extract(data);

            return base.Handle(dataSet);
        }

        public void Teach(double[,] data, double[] rightAnwser, double learningSpeed)
        {
            double[] dataSet = _extractor.Extract(data);
            base.Teach(dataSet, rightAnwser, learningSpeed);
        }
    }
}
