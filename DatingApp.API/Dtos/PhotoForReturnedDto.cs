using System;

namespace DatingApp.API.Dtos
{
    // this dto is only for getting individual photos from get method(PhotoController) 
    public class PhotoForReturnedDto
    {
         public int Id { get; set; }

        public string Url { get; set; } 
        
        public string Description { get; set; }

        public DateTime DateAdded { get; set; }

        public bool IsMain { get; set; }

        public string PublicId { get; set; }
    }
}