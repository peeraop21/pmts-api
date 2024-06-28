using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IPlantCostFieldRepository : IRepository<PlantCostField>
    {
        bool CheckCostFieldInUse(string costField);

        void RemovePlantCostFields(string factoryCode);

        void CreatePlantCostFields(string factoryCode, List<PlantCostField> plantCostFieldList);
    }
}
