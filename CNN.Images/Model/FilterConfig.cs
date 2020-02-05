
namespace CNN.Images.Model
{
    public class FilterConfig
    {
        /// <summary>
        /// Название фильтра
        /// </summary>
        public FilterName Name { get; set; }

        /// <summary>
        /// Матрица коэфициентов фильтра
        /// </summary>
        public double[,] Matrix { get; set; }
    }
}
