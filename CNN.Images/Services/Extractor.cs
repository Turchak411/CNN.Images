using CNN.Images.Core.Layers;
using CNN.Images.Model;
using CNN.Images.Model.Interfaces;
using System.Collections.Generic;

namespace CNN.Images.Services
{
    public class Extractor
    {
        private List<IExtractLayer> m_layers;

        private Extractor() { }

        public Extractor(string layersScheme, List<List<FilterName>> convLayersFilters)
        {
            m_layers = new List<IExtractLayer>();

            for (int i = 0, currentFilterUse = 0; i < layersScheme.Length; i++)
            {
                switch (layersScheme[i])
                {
                    case 'c':
                        m_layers.Add(new ConvolutionLayer(convLayersFilters[currentFilterUse]));
                        currentFilterUse++;
                        break;
                    case 'p':
                    default:
                        m_layers.Add(new MaxPoolingLayer(5, 5)); // default: matrix 3x3 //
                        break;
                }
            }
        }

        public double[] Extract(List<double[,]> rgbMatrix)
        {
            // Обработка через все слои:
            List<double[,]> tempMatrixList = rgbMatrix;

            for (int i = 0; i < m_layers.Count; i++)
            {
                tempMatrixList = m_layers[i].Handle(tempMatrixList);
            }

            // Преобразование полученного списка мультиразмерной матрицы в одноразмерный вектор:
            // Подсчет размера вектора и его создание:
            // P.S. Поскольку размерности всех полученных матриц одинаковы, то размерность принимается равной размерности первой(нулевой) матрицы
            int dimention = tempMatrixList[0].GetLength(0) * tempMatrixList[0].GetLength(1) * tempMatrixList.Count;
            double[] vector = new double[dimention];

            // Загрузка данных в вектор:
            for (int i = 0, vectorIndex = 0; i < tempMatrixList.Count; i++)
            {
                for (int k = 0; k < tempMatrixList[i].GetLength(0); k++)
                {
                    for (int j = 0; j < tempMatrixList[i].GetLength(1); j++, vectorIndex++)
                    {
                        vector[vectorIndex] = tempMatrixList[i][k, j];
                    }
                }
            }

            return vector;
        }

        public double[] Extract(double[,] matrix)
        {
            // Обработка через все слои:
            List<double[,]> tempMatrixList = new List<double[,]>();
            tempMatrixList.Add(matrix);

            for (int i = 0; i < m_layers.Count; i++)
            {
                tempMatrixList = m_layers[i].Handle(tempMatrixList);
            }

            // Преобразование полученного списка мультиразмерной матрицы в одноразмерный вектор:
            // Подсчет размера вектора и его создание:
            // P.S. Поскольку размерности всех полученных матриц одинаковы, то размерность принимается равной размерности первой(нулевой) матрицы
            int dimention = tempMatrixList[0].GetLength(0) * tempMatrixList[0].GetLength(1) * tempMatrixList.Count;
            double[] vector = new double[dimention];

            // Загрузка данных в вектор:
            for (int i = 0, vectorIndex = 0; i < tempMatrixList.Count; i++)
            {
                for (int k = 0; k < tempMatrixList[i].GetLength(0); k++)
                {
                    for (int j = 0; j < tempMatrixList[i].GetLength(1); j++, vectorIndex++)
                    {
                        vector[vectorIndex] = tempMatrixList[i][k, j];
                    }
                }
            }

            return vector;
        }
    }
}
