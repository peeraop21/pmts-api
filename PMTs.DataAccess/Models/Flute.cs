﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.Models;

public partial class Flute
{
    public int Id { get; set; }

    public string FactoryCode { get; set; }

    public string Flute1 { get; set; }

    public string Code { get; set; }

    public string Description { get; set; }

    public int? Height { get; set; }

    public bool? Status { get; set; }

    public int? A { get; set; }

    public int? B { get; set; }

    public int? C { get; set; }

    public int? D1 { get; set; }

    public int? D2 { get; set; }

    public double? Tr1 { get; set; }

    public double? Tr2 { get; set; }

    public double? Tr3 { get; set; }

    public int? JoinSize { get; set; }

    public double? GlueArea { get; set; }

    public int? BundlePiece { get; set; }

    public int? Layer { get; set; }

    public int? Trim { get; set; }

    public int? Stack { get; set; }

    public int? WasteStack { get; set; }

    public int? SpeedFactor { get; set; }

    public bool? GlueLiner { get; set; }

    public bool? Bmedium { get; set; }

    public bool? Bliners { get; set; }

    public bool? Cmedium { get; set; }

    public bool? Cliners { get; set; }

    public bool? Dmedium { get; set; }

    public bool? Dliners { get; set; }

    public string Glname { get; set; }

    public string Bmname { get; set; }

    public string Blname { get; set; }

    public string Cmname { get; set; }

    public string Clname { get; set; }

    public string Dmname { get; set; }

    public string Dlname { get; set; }

    public int? Speed { get; set; }

    public int? SetupTime { get; set; }

    public int? NoOfChange { get; set; }

    public int? LayerPallet { get; set; }

    public int? BoxPerBundleNoJoint { get; set; }

    public int? LayerPerPalletNoJoint { get; set; }

    public DateTime? CreatedDate { get; set; }

    public string CreatedBy { get; set; }

    public DateTime? UpdatedDate { get; set; }

    public string UpdatedBy { get; set; }

    public double Thickness { get; set; }

    public string BshFlute { get; set; }

    public double? TruckStack { get; set; }
}