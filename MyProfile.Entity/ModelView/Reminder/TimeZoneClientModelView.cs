namespace MyProfile.Entity.ModelView.Reminder
{
    public class TimeZoneClientModelView
    {
        public int OlzonTZID { get; set; }
        public string OlzonTZName { get; set; }
        public string WindowsDisplayName { get; set; }
        public string WindowsTimezoneID { get; set; }
        public decimal UtcOffset { get; set; }
        public string Abreviature { get; set; }
        public bool IsDST { get; set; }
        public int TimeZoneID { get; set; }

        public string Title { get { return OlzonTZName + " " + WindowsDisplayName; } }

    }
}
