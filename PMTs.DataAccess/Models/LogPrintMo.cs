﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.Models;

public partial class LogPrintMo
{
    public int Id { get; set; }

    public string FactoryCode { get; set; }

    public string OrderItem { get; set; }

    public int? Printed { get; set; }

    public string PrintedBy { get; set; }

    public DateTime? PrintedDate { get; set; }
}