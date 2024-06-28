namespace PMTs.DataAccess.ComplexModels
{
    public class BoardViewModel
    {
        public string Code { get; set; }
        public string Flute { get; set; }
        public string Board { get; set; }
        public string SearchBoard { get; set; }
        public string BoardKiwi { get; set; }
        public double? Weight { get; set; }

        public int? A { get; set; }
        public int? B { get; set; }
        public int? C { get; set; }
        public int? D1 { get; set; }
        public int? D2 { get; set; }
        public int? JoinSize { get; set; }
        public double? CostPerTon { get; set; }
        public string Hierarchy { get; set; }
    }
}
