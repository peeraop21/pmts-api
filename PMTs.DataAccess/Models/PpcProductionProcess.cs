﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.Models;

public partial class PpcProductionProcess
{
    public int Id { get; set; }

    public string Plant { get; set; }

    public int? WorkType { get; set; }

    public string PlanCode { get; set; }

    public int? Color { get; set; }

    public int? QuantityStart { get; set; }

    public int? QuantityTo { get; set; }

    public int? PaperWaste { get; set; }

    public decimal? PercentWaste { get; set; }
}