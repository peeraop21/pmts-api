using System;

namespace PMTs.DataAccess.ComplexModels
{
    public class CheckRepeatOrder
    {
        public string FactoryCode { get; set; }
        public string OrderItem { get; set; }
        public string MaterialNo { get; set; }
        public string PC { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public int OrderQuant { get; set; }
        public DateTime OriginalDueDate { get; set; }
        public DateTime DueDate { get; set; }
        public int? TargetQuant { get; set; }
        public string PoNo { get; set; }
        public int repeatCount { get; set; }
        public string ItemNote { get; set; }
        public string Batch { get; set; }
        public string MOStatus { get; set; }
        public string DateTimeStamp { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }

    public class CheckDiffDueDate
    {
        public string FactoryCode { get; set; }
        public string OrderItem { get; set; }
        public string MaterialNo { get; set; }
        public string PC { get; set; }
        public string Name { get; set; }
        public string BoxType { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime MaxDue { get; set; }
        public DateTime CreatedDate { get; set; }
        public int diff { get; set; }
    }

    public class CheckDueDateToolong
    {
        public string FactoryCode { get; set; }
        public string OrderItem { get; set; }
        public string MaterialNo { get; set; }
        public string PC { get; set; }
        public string PoNo { get; set; }
        public string Name { get; set; }
        public int OrderQuant { get; set; }
        public int? TargetQuant { get; set; }
        public DateTime DueDate { get; set; }
        public string ItemNote { get; set; }
        public string Batch { get; set; }
        public string DateTimeStamp { get; set; }
    }

    public class CheckOrderQtyTooMuch
    {
        public string FactoryCode { get; set; }
        public string OrderItem { get; set; }
        public string MaterialNo { get; set; }
        public string PC { get; set; }
        public string Description { get; set; }
        public string Name { get; set; }
        public DateTime DueDate { get; set; }
        public int OrderQuant { get; set; }
        public int SumQty { get; set; }
        public int CountTime { get; set; }
        public int AvgQty { get; set; }
    }
}
