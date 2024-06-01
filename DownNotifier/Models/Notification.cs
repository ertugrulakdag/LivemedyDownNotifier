namespace DownNotifier.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public int TargetAppId { get; set; }
        public TargetApp TargetApp { get; set; }
        public DateTime SentAt { get; set; }
        public string Message { get; set; }
    }
}
