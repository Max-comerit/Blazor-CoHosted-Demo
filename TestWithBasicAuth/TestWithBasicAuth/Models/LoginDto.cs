using System.ComponentModel.DataAnnotations;

namespace TestWithBasicAuth.Models
{
    public class LoginDto
    {
        [Required(ErrorMessage ="Du måste ange ett användarnamn")]
        public string Username { get; set; }
        [Required(ErrorMessage ="Du måste ange ett lösenord")]
        public string Password { get; set; }
    }
}
