using CNN.Images.Model;
using System.Collections.Generic;

namespace CNN.Images.Services
{
    public class FilterLoader
    {
        public List<FilterConfig> ImportFilters(List<FilterName> filtersToImport)
        {
            FilterConfigurator filterConfigurator = new FilterConfigurator();

            // Получение всех возможных фильтров:
            List<FilterConfig> allFilters = filterConfigurator.GetAllFilters();

            // Загрузка только тех, которые есть в списке:
            List<FilterConfig> filterList = new List<FilterConfig>();

            for (int i = 0; i < filtersToImport.Count; i++)
            {
                for (int k = 0; k < allFilters.Count; k++)
                {
                    if (filtersToImport.Contains(allFilters[k].Name))
                    {
                        filterList.Add(allFilters[k]);
                    }
                }
            }

            return filterList;
        }
    }
}
