namespace BloodDonationApp.Components.Pages
{
    public class DonationRequestModel
    {
        public int RequestID { get; set; }
        public int DonorID { get; set; }
        public int BloodBankID { get; set; }
        public string Message { get; set; }
        public string Status { get; set; } 
        public DateTime RequestDate { get; set; }
    }
}
