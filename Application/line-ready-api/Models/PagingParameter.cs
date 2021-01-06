namespace LineReadyApi.Models
{
    public class PagingParameter
    {
        private const int maxPageSize = 20;

        public int pageNumber { get; set; } = 1;

        public int pageSize
        {
            get => _pageSize;
            set => _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }

        private int _pageSize { get; set; } = 10;
    }
}