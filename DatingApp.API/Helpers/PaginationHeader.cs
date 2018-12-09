namespace DatingApp.API.Helpers
{
    public class PaginationHeader
    {
        // paging information will return to clients through a response header
        // implemented in Extensions.cs
        public int CurrentPage { get; set; } // represents number of clients requests
        public int ItemsPerPage { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; } // total number of db entities

        public PaginationHeader(int currentPage, int itemsPerPage, int totalItems, int totalPages)
        {
            this.CurrentPage = currentPage;
            this.ItemsPerPage = itemsPerPage;
            this.TotalItems = totalItems;
            this.TotalPages = totalPages;
        }
    }
}