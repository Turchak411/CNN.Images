using CNN.Images.Model.Interfaces;
using System;
using System.Collections.Generic;

namespace CNN.Images.Core.Layers
{
    public class MaxPoolingLayer : IExtractLayer
    {
        private int m_handleMatrixDimY;
        private int m_handleMatrixDimX;

        private MaxPoolingLayer() { }

        public MaxPoolingLayer(int handleMatrixDimY, int handleMatrixDimX)
        {
            m_handleMatrixDimY = handleMatrixDimY;
            m_handleMatrixDimX = handleMatrixDimX;
        }

        public List<double[,]> Handle(List<double[,]> inputMatrix)
        {
            List<double[,]> matrixList = new List<double[,]>();

            for (int i = 0; i < inputMatrix.Count; i++)
            {
                matrixList.Add(MaxPool(inputMatrix[i]));
            }

            return matrixList;
        }

        private double[,] MaxPool(double[,] matrix)
        {
            // Проверка необходимости падинга:
            int matrixDimY = matrix.GetLength(0);
            int matrixDimX = matrix.GetLength(1);

            if ((matrixDimY % m_handleMatrixDimY != 0) || (matrixDimX % m_handleMatrixDimX != 0))
            {
                // Padding:
                double[,] newMatrix = matrix;
                // Калибровка по вертикали:
                while (newMatrix.GetLength(0) % m_handleMatrixDimY != 0)
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
                while (newMatrix.GetLength(1) % m_handleMatrixDimX != 0)
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

            // Max-Pooling:
            int pooledMatrixDimY = matrixDimY / m_handleMatrixDimY;
            int pooledMatrixDimX = matrixDimX / m_handleMatrixDimX;

            double[,] pooledMatrix = new double[pooledMatrixDimY, pooledMatrixDimX];

            for (int i = 0; i < pooledMatrixDimY; i++)
            {
                for (int k = 0; k < pooledMatrixDimX; k++)
                {
                    pooledMatrix[i, k] = GetMaxValue(matrix, i * m_handleMatrixDimY, k * m_handleMatrixDimX);
                }
            }

            return pooledMatrix;
        }

        private double GetMaxValue(double[,] matrix, int sectionIndexY, int sectionIndexX)
        {
            double maxValue = matrix[0, 0];

            for (int i = sectionIndexY; i < sectionIndexY + m_handleMatrixDimY; i++)
            {
                for (int k = sectionIndexX; k < sectionIndexX + m_handleMatrixDimX; k++)
                {
                    if (matrix[i, k] > maxValue)
                    {
                        maxValue = matrix[i, k];
                    }
                }
            }

            return maxValue;
        }
    }
}
