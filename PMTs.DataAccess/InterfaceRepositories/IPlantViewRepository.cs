using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IPlantViewRepository : IRepository<PlantView>
    {
        PlantView GetPlantViewByMaterialNo(string factoryCode, string materialNo);
        PlantView GetPlantViewByMaterialNoAndPlant(string factoryCode, string materialNo);
        PlantView GetPlantViewByPlant(string factoryCode, string materialNo);
        IEnumerable<PlantView> GetPlantViewsByMaterialNo(string factoryCode, string materialNo);
        void UpdatePlantViewShipBlk(string FactoryCode, string MaterialNo, string Status);
        IEnumerable<PlantView> GetPlanViewByhandshake(SqlConnection conn, string FactoryCode, string materialNo);
        IEnumerable<PlantView> GetReusePlantViewsByMaterialNos(string factoryCode, List<string> materialNos);
    }


}
