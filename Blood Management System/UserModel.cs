using System.ComponentModel.DataAnnotations;

namespace UserManager.Models
{
    public class UserModel
    {
        public int UserID { get; set; }

        [Required]
        public string? Name { get; set; }

        [Required]
        public string? Username { get; set; }

        [Required]
        public string? Password { get; set; }

        public string? Role { get; set; }

        public int LinkedID { get; set; }
    }

}