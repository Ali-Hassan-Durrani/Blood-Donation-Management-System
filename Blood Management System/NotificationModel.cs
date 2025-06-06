namespace BloodDonationApp.Components.Pages
{
    public class NotificationModel
    {
        public int NotificationID { get; set; }
        public int BloodBankID { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
