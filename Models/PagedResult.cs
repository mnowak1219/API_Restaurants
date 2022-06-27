namespace API_Restaurants.Models
{
    public class PagedResult<T>
    {
        public List<T> Items{ get; set; }
        public int TotalItemsCount { get; set; }
        public int TotalPages { get; set; }
        public int ItemsFrom { get; set; }
        public int ItemsTo { get; set; }

        public PagedResult(List<T> items, int totalItemsCount, int pageSize, int pageNumber)
        {
            Items = items;
            TotalItemsCount = totalItemsCount;
            TotalPages = (int)Math.Ceiling((double)totalItemsCount / pageSize);
            ItemsFrom = pageSize * (pageNumber - 1) + 1;
            ItemsTo = ItemsFrom + pageSize - 1;
            if (ItemsTo > TotalItemsCount)
            { 
                ItemsTo = TotalItemsCount;
            }
        } 
    }
}
