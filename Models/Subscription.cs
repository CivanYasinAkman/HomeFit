namespace HomeFit.Models
{
    public class Subscription
    {
        public int SubscriptionId { get; set; }
        public int UserId { get; set; }
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow.AddMonths(1);
        public string PaymentStatus { get; set; } = "Simulated";
        public string Type { get; set; } = "Premium";

        // Navigation
        public User? User { get; set; }
    }
}