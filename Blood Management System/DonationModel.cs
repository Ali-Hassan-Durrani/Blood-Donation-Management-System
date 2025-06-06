namespace BloodDonationApp.Components.Pages
{
    public class DonationModel
    {
        public int DonationID { get; set; }
        public int DonorID { get; set; }
        public int BloodBankID { get; set; }
        public string BloodBankName { get; set; } = string.Empty;
        public DateTime DonationDate { get; set; }
        public string Status { get; set; } // Accepted, Rejected
    }
}
