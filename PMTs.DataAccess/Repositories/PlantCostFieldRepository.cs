using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class PlantCostFieldRepository : Repository<PlantCostField>, IPlantCostFieldRepository
    {
        public PlantCostFieldRepository(DbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public bool CheckCostFieldInUse(string costField)
        {
            var IsDuplicate = PMTsDbContext.PlantCostField.AsNoTracking().FirstOrDefault(p => p.CostField == costField) != null;
            return IsDuplicate;
        }

        public void CreatePlantCostFields(string factoryCode, List<PlantCostField> plantCostFieldList)
        {
            var plantCostFields = PMTsDbContext.PlantCostField.Where(p => p.FactoryCode.Equals(factoryCode)).ToList();
            PMTsDbContext.PlantCostField.RemoveRange(plantCostFields);
            foreach (var plantCostField in plantCostFieldList)
            {
                PMTsDbContext.PlantCostField.Add(plantCostField);

                PMTsDbContext.SaveChanges();
            }
        }

        public void RemovePlantCostFields(string factoryCode)
        {
            var plantCostFields = PMTsDbContext.PlantCostField.Where(p => p.FactoryCode.Equals(factoryCode)).ToList();
            PMTsDbContext.PlantCostField.RemoveRange(plantCostFields);
        }
    }
}
