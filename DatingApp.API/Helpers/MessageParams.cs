namespace DatingApp.API.Helpers
{
    public class MessageParams
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
        public int UserId { get; set; } // filtering messages for each container (privatize messages)
        public string  MessageContainer { get; set; } = "Unread"; // three containers(inbox, outbox, unread)


    }
}