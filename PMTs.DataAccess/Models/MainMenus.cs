﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace PMTs.DataAccess.Models;

public partial class MainMenus
{
    public int Id { get; set; }

    public string MenuNameEn { get; set; }

    public string MenuNameTh { get; set; }

    public string Controller { get; set; }

    public string Action { get; set; }

    public string IconName { get; set; }

    public int? SortMenu { get; set; }
}