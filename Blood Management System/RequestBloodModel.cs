using System.ComponentModel.DataAnnotations;

namespace RequestBloodManager.Models
{
    public class RequestBloodModel
    {
        public int RequestID { get; set; }

        [Required]
        public string RequesterRole { get; set; } = string.Empty;

        [Required]
        public int RequesterID { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Blood Group is required")]
        [StringLength(10)]
        public string BloodGroup { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Donor Name is required")]
        [StringLength(100)]
        public string DonorName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Required Date is required")]
        [DataType(DataType.Date)]
        public DateTime RequiredDate { get; set; }

        [Required(ErrorMessage = "Contact Number is required")]
        [Phone]
        public string ContactNumber { get; set; } = string.Empty;

        [Required]
        public string Status { get; set; } = "Pending";
    }
}
