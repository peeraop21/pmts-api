namespace PMTs.DataAccess.ComplexModels
{
    public class ColumnPalletModel
    {
        public string Type { get; set; }
        public string LxW { get; set; }
        public int BundlePerLayyer { get; set; }
        public int L { get; set; }
        public int W { get; set; }
        public int L1 { get; set; }
        public int W1 { get; set; }
        public int L2 { get; set; }
        public int W2 { get; set; }
        public int L1_L1 { get; set; }
        public int W1_W1 { get; set; }
        public int L1_W1 { get; set; }
        public int W1_L1 { get; set; }
        public int L2_L2 { get; set; }
        public int W2_W2 { get; set; }
        public int L2_W2 { get; set; }
        public int W2_L2 { get; set; }
        public int CartonPerLayer { get; set; }
    }
}
