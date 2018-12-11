namespace DatingApp.API.Models
{
    public class Like
    {
        // both id's are actually user id's
        // users are divided into two abstract tables (liker and likee)
        public int LikerId { get; set; }

        public int LikeeId { get; set; }

        // for one--->many relationship of liker with likees
        // for one--->many relationship of likee with likers
        public User Liker { get; set; }

        public User  Likee { get; set; }

    }
}