using System.ComponentModel.DataAnnotations;

namespace ClaimsSystem.Models
{

    //get,set method for user
    public class User
    {
        [Key]
        public string UserID { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        public string UserRole { get; set; }
    }
    public class Login
    {
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
