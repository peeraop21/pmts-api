﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.Models;

public partial class FluteTr
{
    public int Id { get; set; }

    public string FactoryCode { get; set; }

    public string FluteCode { get; set; }

    public string Station { get; set; }

    public double? Tr { get; set; }

    public int? Item { get; set; }

    public bool? HasCoating { get; set; }

    public bool? Status { get; set; }
}