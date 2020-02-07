using CNN.Images.Model;
using CNN.Images.Model.Interfaces;
using CNN.Images.Services;
using System;
using System.Collections.Generic;

namespace CNN.Images.Core.Layers
{
    public class ConvolutionLayer : IExtractLayer
    {
        private List<FilterConfig> _filtersList;

        private ConvolutionLayer() { }

        public ConvolutionLayer(List<FilterName> filtersToImport)
        {
            FilterLoader filterLoader = new FilterLoader();
            _filtersList = filterLoader.ImportFilters(filtersToImport);
        }

        public List<double[,]> Handle(List<double[,]> inputMatrix)
        {
            List<double[,]> convMatrix = new List<double[,]>();

            for (int i = 0; i < inputMatrix.Count; i++)
            {
                for (int k = 0; k < _filtersList.Count; k++)
                {
                    convMatrix.Add(ImposeFilter(inputMatrix[i], _filtersList[k].Matrix));
                }
            }

            return convMatrix;
        }

        private double[,] ImposeFilter(double[,] matrix, double[,] filter)
        {
            double[,] convoluteMatrix = null;

            // Проверка необходимости падинга 
            // (разрядность обрабатыеваемой матрицы должна делиться на разрядность фильтра без остатка)
            int matrixDimY = matrix.GetLength(0);
            int matrixDimX = matrix.GetLength(1);

            if ((matrixDimY - filter.GetLength(0) < 0) || (matrixDimX - filter.GetLength(1) < 0))
            {
                double[,] newMatrix = matrix;
                // Калибровка по вертикали:
                while (newMatrix.GetLength(0) % filter.GetLength(0) != 0)
                {
                    newMatrix = new double[newMatrix.GetLength(0) + 1, matrix.GetLength(1)];
                    Array.ConstrainedCopy(matrix, 0, newMatrix, 0, matrixDimY * matrixDimX);

                    // Присвоить новым элементам 0:
                    for (int i = 0; i < newMatrix.GetLength(1); i++)
                    {
                        newMatrix[newMatrix.GetLength(0) - 1, i] = 0;
                    }
                }

                matrix = newMatrix;

                // Калибровка по вертикали:
                while (newMatrix.GetLength(1) % filter.GetLength(1) != 0)
                {
                    newMatrix = new double[matrix.GetLength(0), newMatrix.GetLength(1) + 1];
                    Array.ConstrainedCopy(matrix, 0, newMatrix, 0, matrixDimY * matrixDimX);

                    // Присвоить новым элементам 0:
                    for (int k = 0; k < newMatrix.GetLength(0); k++)
                    {
                        newMatrix[k, newMatrix.GetLength(1) - 1] = 0;
                    }
                }

                matrix = newMatrix;
            }

            matrixDimY = matrix.GetLength(0);
            matrixDimX = matrix.GetLength(1);

            // Свертка:
            convoluteMatrix = new double[matrixDimY - filter.GetLength(0) + 1, matrixDimX - filter.GetLength(1) + 1];

            // TODO: [WARP] Тут настраивается stride (шаг обработки) convolution слоя
            // TODO: Выделить stride в поле внутри этого класса
            for (int i = 0; i < matrixDimY - filter.GetLength(0); i+=2)
            {
                for (int k = 0; k < matrixDimX - filter.GetLength(1); k+=2)
                {
                    convoluteMatrix[i, k] = ImposeFilterFrame(matrix, filter, i, k);
                }
            }

            return convoluteMatrix;
        }

        private double ImposeFilterFrame(double[,] matrix, double[,] filter, int relIndexY, int relIndexX)
        {
            double sumValue = 0;

            for (int i = 0; i < filter.GetLength(0); i++)
            {
                for (int k = 0; k < filter.GetLength(1); k++)
                {
                    sumValue += matrix[relIndexY + i, relIndexX + k] * filter[i, k];
                }
            }

            return sumValue;
        }
    }
}
