namespace DownNotifier.Models
{
    public class TargetApp
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public TimeSpan Interval { get; set; }
        public DateTime LastChecked { get; set; }
        public bool IsUp { get; set; }
    }
}
