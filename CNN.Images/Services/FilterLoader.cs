using CNN.Images.Model;
using System;
using System.IO;
using System.Collections.Generic;

namespace CNN.Images.Services
{
    public class FilterLoader
    {
        // README: In this DB uses 5x5 filters
        // TODO: Переработать данный класс

        private List<double[,]> _filtersList = new List<double[,]>();
        private string _dbFilePath;

        private FilterLoader() { }

        public FilterLoader(List<Filter> filtersToImport, string filtersFileName)
        {
            _dbFilePath = filtersFileName;
            ImportFilters(filtersToImport);
        }

        private void ImportFilters(List<Filter> filtersToImport)
        {
            using (StreamReader fileReader = new StreamReader(_dbFilePath))
            {
                for (int i = 0; !fileReader.EndOfStream; i++)
                {
                    string[] filterText = fileReader.ReadLine().Split(' ');

                    Filter filterEnum = (Filter)Enum.Parse(typeof(Filter), filterText[0]);

                    if (filtersToImport.Contains(filterEnum))
                    {
                        double[,] filterMatrix = ConvertToMatrix(filterText);

                        _filtersList.Add(filterMatrix);
                    }
                }
            }
        }

        private double[,] ConvertToMatrix(string[] filter)
        {
            int matrixDim = Convert.ToInt32(Math.Sqrt(filter.Length - 1));
            double[,] filterMatrix = new double[matrixDim, matrixDim];

            for (int i = 0, j = 0; i < matrixDim; i++)
            {
                for (int k = 0; k < matrixDim; k++, j++)
                {
                    filterMatrix[i, k] = Convert.ToInt32(filter[j + 1]);
                }
            }

            return filterMatrix;
        }

        public double[,] GetFilter(int index)
        {
            return _filtersList[index];
        }

        public int GetFiltersCount()
        {
            return _filtersList.Count;
        }
    }
}
