using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Dtos
{
    public class UserForRegisterDto
    {
        //use dataannotations for validating data
        [Required]
        public string Username {get; set;}

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Must Specify Password between 4 to 8 character")]
        public string Password {get; set;}
    }
}