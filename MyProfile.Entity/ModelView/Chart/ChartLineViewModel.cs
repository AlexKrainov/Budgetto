namespace MyProfile.Entity.ModelView.Chart
{
    public class ChartLineViewModel : IChartData
    {
        public string Label { get; set; }
        public string BorderColor { get; set; }
        public decimal[] Data { get; set; }
        public bool Fill { get; set; }
        public decimal MinValue { get; set; }
        public decimal MaxValue { get; set; }
        public decimal AvgValue { get; set; }
        public decimal TotalValue { get; set; }
        public bool Hidden { get; set; }
    }
}
