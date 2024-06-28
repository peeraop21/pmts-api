﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.Models;

public partial class TransactionsDetail
{
    public int Id { get; set; }

    public string FactoryCode { get; set; }

    public string MaterialNo { get; set; }

    public int? IdKindOfProductGroup { get; set; }

    public int? IdProcessCost { get; set; }

    public int? IdKindOfProduct { get; set; }

    public int? IdProductType { get; set; }

    public int? IdMaterialType { get; set; }

    public int? IdProductUnit { get; set; }

    public int? IdSaleUnit { get; set; }

    public string HierarchyLv4 { get; set; }

    public bool? Glwid { get; set; }

    public bool? Gltail { get; set; }

    public bool? CapImg { get; set; }

    public string IsWrap { get; set; }

    public bool? IsNotch { get; set; }

    public int? NotchDegree { get; set; }

    public int? NotchArea { get; set; }

    public string NotchSide { get; set; }

    public int? SideA { get; set; }

    public int? SideB { get; set; }

    public int? SideC { get; set; }

    public double? SideD { get; set; }

    public string Cgtype { get; set; }

    public int? AmountColor { get; set; }

    public string HvaGroup1 { get; set; }

    public string HvaGroup2 { get; set; }

    public string HvaGroup3 { get; set; }

    public string HvaGroup4 { get; set; }

    public string HvaGroup5 { get; set; }

    public string HvaGroup6 { get; set; }

    public string HvaGroup7 { get; set; }

    public int? PalletOverhang { get; set; }

    public bool Outsource { get; set; }

    public int? HireOrderType { get; set; }

    public string MatSaleOrg { get; set; }

    public string PdisStatus { get; set; }

    public int? MaxStep { get; set; }

    public string HoneyCoreSize { get; set; }

    public string NewPrintPlate { get; set; }

    public string OldPrintPlate { get; set; }

    public string NewBlockDieCut { get; set; }

    public string OldBlockDieCut { get; set; }

    public string ExampleColor { get; set; }

    public string CoatingType { get; set; }

    public string CoatingTypeDesc { get; set; }

    public bool? PaperHorizontal { get; set; }

    public bool? PaperVertical { get; set; }

    public bool? FluteHorizontal { get; set; }

    public bool? FluteVertical { get; set; }
}