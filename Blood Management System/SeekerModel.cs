using System.ComponentModel.DataAnnotations;

namespace SeekerManager.Models
{
    public class SeekerModel
    {
        public int SeekerID { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string Name { get; set; } = string.Empty;

        [Range(1, 120, ErrorMessage = "Age must be between 1 and 120")]
        public int Age { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Blood Group is required")]
        public string BloodGroup { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required")]
        public string ContactNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "CNIC is required")]
        public string CNIC { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}
