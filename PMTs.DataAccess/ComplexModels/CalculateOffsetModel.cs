﻿using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModels
{
    public class CalculateOffsetModel
    {
        public List<MoBomRawMat> moBomRawmats { get; set; }
        public MoData MoData { get; set; }
    }
}
