using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModels
{
    public class FluteMaintainModel
    {
        public FluteMaintainModel()
        {
            Flute = new Flute();
            Flutes = new List<Flute>();
            FluteTrs = new List<FluteTr>();

        }

        public Flute Flute { get; set; }
        public List<Flute> Flutes { get; set; }
        public List<FluteTr> FluteTrs { get; set; }

    }
}
