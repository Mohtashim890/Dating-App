using System;

namespace DatingApp.API.Models
{
    public class Photo
    {
        public int Id { get; set; }

        public string Url { get; set; } 
        
        public string Description { get; set; }

        public DateTime DateAdded { get; set; }

        public bool IsMain { get; set; }

        // To Avoid EF making restricted deletion and enable it to make 
        // cascaded deletion between Photos and Users tables
        public User User { get; set; }

        public int UserId { get; set; }
        
    }
}