using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.ComplexModels
{
    public class MODataManualModel
    {
        public MoData MoData { get; set; }
        public bool CreatedStatus { get; set; }
        public string ErrorMessage { get; set; }
    }
}
