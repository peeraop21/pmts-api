using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class TruckOptimizeRepository : Repository<TruckOptimize>, ITruckOptimizeRepository
    {
        public TruckOptimizeRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<TruckOptimize> GetTruckOptimizeByMaterialNo(string factoryCode, string materialNo)
        {
            return PMTsDbContext.TruckOptimize.Where(t => t.MaterialNo.Equals(materialNo) && t.FactoryCode == factoryCode);
        }

        public IEnumerable<TruckOptimize> CreateTruckOptimizes(List<TruckOptimize> truckOptimizes)
        {
            var result = new List<TruckOptimize>();
            foreach (var truckOptimize in truckOptimizes)
            {
                try
                {
                    var masterData = PMTsDbContext.MasterData.FirstOrDefault(t => t.MaterialNo.Equals(truckOptimize.MaterialNo) && t.FactoryCode == truckOptimize.FactoryCode);
                    if (masterData != null)
                    {
                        var entityEntry = PMTsDbContext.TruckOptimize.FirstOrDefault(t => t.MaterialNo.Equals(truckOptimize.MaterialNo) && t.FactoryCode == truckOptimize.FactoryCode);
                        if (entityEntry != null)
                        {
                            entityEntry.FgpalletW = truckOptimize.FgpalletW;
                            entityEntry.FgpalletL = truckOptimize.FgpalletL;
                            entityEntry.FgpalletH = truckOptimize.FgpalletH;
                            entityEntry.FgbundleW = truckOptimize.FgbundleW;
                            entityEntry.FgbundleL = truckOptimize.FgbundleL;
                            entityEntry.FgbundleH = truckOptimize.FgbundleH;
                            entityEntry.PalletSizeW = truckOptimize.PalletSizeW;
                            entityEntry.PalletSizeL = truckOptimize.PalletSizeL;
                            entityEntry.PalletSizeH = truckOptimize.PalletSizeH;
                            entityEntry.UpdatedBy = truckOptimize.UpdatedBy;
                            entityEntry.UpdatedDate = truckOptimize.UpdatedDate;

                            PMTsDbContext.SaveChanges();

                            entityEntry = PMTsDbContext.TruckOptimize.FirstOrDefault(t => t.MaterialNo.Equals(truckOptimize.MaterialNo) && t.FactoryCode == truckOptimize.FactoryCode);
                            if (entityEntry != null)
                            {
                                result.Add(entityEntry);
                            }
                            else
                            {
                                throw new Exception("data is already exists");
                            }
                        }
                        else
                        {
                            PMTsDbContext.TruckOptimize.Add(truckOptimize);
                            PMTsDbContext.SaveChanges();

                            entityEntry = PMTsDbContext.TruckOptimize.FirstOrDefault(t => t.MaterialNo.Equals(truckOptimize.MaterialNo) && t.FactoryCode == truckOptimize.FactoryCode);
                            if (entityEntry != null)
                            {
                                result.Add(entityEntry);
                            }
                            else
                            {
                                throw new Exception("data is already exists");
                            }
                        }
                    }
                    else
                    {
                        result.Add(new TruckOptimize
                        {
                            MaterialNo = truckOptimize.MaterialNo,
                            FactoryCode = truckOptimize.FactoryCode,
                            Id = 0,
                        });
                    }
                }
                catch
                {
                    result.Add(new TruckOptimize
                    {
                        MaterialNo = truckOptimize.MaterialNo,
                        FactoryCode = truckOptimize.FactoryCode,
                        Id = 0,
                    });

                    continue;
                }

            }

            result = result.GroupBy(x => x.MaterialNo).Select(y => y.First()).ToList();
            List<TruckOptimize> asd = new List<TruckOptimize>();

            return result;
        }
    }
}
