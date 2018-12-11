namespace DatingApp.API.Helpers
{
    public class UserParams
    {
        // Defining default PageNumber and PageSize
        private const int MaxPageSize = 50;
        public int PageNumber{ get; set; } = 1;
        private int pageSize = 10;
        public int PageSize {
            get { return pageSize;}
            set { pageSize = (value > MaxPageSize) ? MaxPageSize: value;}
        }

        // params for filtering
        public int UserId { get; set; } // to remove loggedIn User from list
        public string Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 99;
        // params for sorting
        public string OrderBy { get; set; }

        // params for like filter
        public bool Likees { get; set; } = false; // if we like someone it will get true
        public bool Likers { get; set; } = false; // if someone like us it will get true
        
    }
}