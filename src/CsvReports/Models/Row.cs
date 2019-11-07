namespace CsvReports {
    public class Row {
        public string Band { get; set; }
        public int Pcl { get; set; }
        public float TxPower { get; set; }
        public float TargetPower { get; set; }
        public float MinPower { get; set; }
        public float MaxPower { get; set; }
        public bool IsPassed { get; set; }
    }
}