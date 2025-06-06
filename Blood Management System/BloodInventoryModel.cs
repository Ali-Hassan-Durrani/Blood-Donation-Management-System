using System.ComponentModel.DataAnnotations;

namespace BloodInventoryManager.Models
{
    public class BloodInventoryModel
    {
        public int InventoryID { get; set; }

        [Required(ErrorMessage = "Blood group is required")]
        public string BloodGroup { get; set; }

        [Required(ErrorMessage = "Units available is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Units must be a positive number")]
        public int UnitsAvailable { get; set; }
    }
}
