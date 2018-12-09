using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        //use data annotations for validating data
        [Required]
        public string Username {get; set;}

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Must Specify Password between 4 to 8 character")]
        public string Password {get; set;}

        [Required]
        public string KnownAs {get; set;}

        [Required]
        public string City {get; set;}

        [Required]
        public string Country {get; set;}

        [Required]
        public string Gender {get; set;}

        [Required]
        public DateTime DateOfBirth {get; set;}

        public DateTime Created {get; set;}

        public DateTime LastActive {get; set;}

        public UserForRegisterDto()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}