namespace PhishingSiteDetector_API.Models.DTOs
{
    public class ListPageDTO<T>
    {
        public List<T> Items { get; set; }
        public int Count { get; set; }
        public int PageNumber { get; set; }

        public ListPageDTO(List<T> items, int count, int pageNumber)
        {
            Items = items;
            Count = count;
            PageNumber = pageNumber;
        }
    }
}