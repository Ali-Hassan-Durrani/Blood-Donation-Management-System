using System;
using System.ComponentModel.DataAnnotations;

namespace DonorManager.Models
{
    public class DonorModel
    {
        public int DonorID { get; set; }

        [Required(ErrorMessage = "Full name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Blood group is required")]
        public string BloodGroup { get; set; }

        [Required(ErrorMessage = "First donation date is required")]
        public DateTime FirstDonationDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Last donation date is required")]
        public DateTime LastDonationDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Contact number is required")]
        public string ContactNo { get; set; }

        [Required(ErrorMessage = "CNIC is required")]
        public string CNIC { get; set; }

        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }
        public bool IsAvailable { get; set; }

    }
}
