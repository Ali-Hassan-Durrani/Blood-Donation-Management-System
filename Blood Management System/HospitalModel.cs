using System.ComponentModel.DataAnnotations;

namespace HospitalManager.Models
{
    public class HospitalModel
    {
        public int HospitalID { get; set; }

        [Required(ErrorMessage = "Hospital name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Address is required")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        public string PhoneNo { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        public string? Website { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
    }
}
