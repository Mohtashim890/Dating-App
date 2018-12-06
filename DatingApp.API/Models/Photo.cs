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

        // to store public id returned by cloudinary
        // NOTE: add migrations after changing models!!!
        public string PublicId { get; set; }

        // To Avoid EF making restricted deletion and enable it to make 
        // cascaded deletion between Photos and Users tables we have to include
        // parent table's name and its primary key as navigation properties in child model
        public User User { get; set; }

        public int UserId { get; set; }
        
    }
}