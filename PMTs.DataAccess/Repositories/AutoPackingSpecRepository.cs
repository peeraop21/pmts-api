using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class AutoPackingSpecRepository : Repository<AutoPackingSpec>, IAutoPackingSpecRepository
    {
        public AutoPackingSpecRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public IEnumerable<AutoPackingSpec> CreateAutoPackingSpecs(List<AutoPackingSpec> autoPackingSpecs)
        {
            var result = new List<AutoPackingSpec>();
            foreach (var autoPackingSpec in autoPackingSpecs)
            {
                try
                {
                    var masterData = PMTsDbContext.MasterData.FirstOrDefault(t => t.MaterialNo.Equals(autoPackingSpec.MaterialNo) && t.FactoryCode == autoPackingSpec.FactoryCode);
                    if (masterData != null)
                    {
                        var entityEntry = PMTsDbContext.AutoPackingSpec.FirstOrDefault(t => t.MaterialNo.Equals(autoPackingSpec.MaterialNo) && t.FactoryCode == autoPackingSpec.FactoryCode);
                        if (entityEntry != null)
                        {
                            entityEntry.NPalletType = autoPackingSpec.NPalletType;
                            entityEntry.CPalletArrange = autoPackingSpec.CPalletArrange;
                            entityEntry.CPalletStackPos = autoPackingSpec.CPalletStackPos;
                            entityEntry.NStrapCompression = autoPackingSpec.NStrapCompression;
                            entityEntry.CStrapType = autoPackingSpec.CStrapType;
                            entityEntry.NWrapType = autoPackingSpec.NWrapType;
                            entityEntry.NTopBoardType = autoPackingSpec.NTopBoardType;
                            entityEntry.NBottomBoardType = autoPackingSpec.NBottomBoardType;
                            entityEntry.CStrapperBottomProtection = autoPackingSpec.CStrapperBottomProtection;
                            entityEntry.CStrapperTopProtection = autoPackingSpec.CStrapperTopProtection;
                            entityEntry.CornerGuard = autoPackingSpec.CornerGuard;
                            entityEntry.UpdatedBy = autoPackingSpec.UpdatedBy;
                            entityEntry.UpdatedDate = autoPackingSpec.UpdatedDate;

                            PMTsDbContext.SaveChanges();

                            entityEntry = PMTsDbContext.AutoPackingSpec.FirstOrDefault(t => t.MaterialNo.Equals(autoPackingSpec.MaterialNo) && t.FactoryCode == autoPackingSpec.FactoryCode);
                            if (entityEntry != null)
                            {
                                result.Add(entityEntry);
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                        else
                        {
                            PMTsDbContext.AutoPackingSpec.Add(autoPackingSpec);
                            PMTsDbContext.SaveChanges();

                            entityEntry = PMTsDbContext.AutoPackingSpec.FirstOrDefault(t => t.MaterialNo.Equals(autoPackingSpec.MaterialNo) && t.FactoryCode == autoPackingSpec.FactoryCode);
                            if (entityEntry != null)
                            {
                                result.Add(entityEntry);
                            }
                            else
                            {
                                throw new Exception();
                            }
                        }
                    }
                    else
                    {
                        result.Add(new AutoPackingSpec
                        {
                            Id = 0,
                            MaterialNo = autoPackingSpec.MaterialNo,
                            FactoryCode = autoPackingSpec.FactoryCode,
                        });
                    }
                }
                catch
                {
                    result.Add(new AutoPackingSpec
                    {
                        MaterialNo = autoPackingSpec.MaterialNo,
                        FactoryCode = autoPackingSpec.FactoryCode,
                        Id = 0,
                    });

                    continue;
                }

            }

            result.GroupBy(x => x.MaterialNo).Select(y => y).ToList();

            return result;
        }

        public IEnumerable<AutoPackingSpec> GetAutoPackingSpecByMaterialNo(string factoryCode, string materialNo)
        {
            return PMTsDbContext.AutoPackingSpec.Where(t => t.MaterialNo.Equals(materialNo) && t.FactoryCode == factoryCode);
        }

        public void SaveAndUpdateAutoPackingSpecFromCusId(string factoryCode, string cusId, string username, string materialNo)
        {
            var originalAutoPackingCustomer = PMTsDbContext.AutoPackingCustomer.AsNoTracking().FirstOrDefault(a => a.CusId == cusId);
            var originalAutoPackingSpec = PMTsDbContext.AutoPackingSpec.AsNoTracking().FirstOrDefault(a => a.MaterialNo == materialNo && a.FactoryCode == factoryCode);
            var originalmasterData = PMTsDbContext.MasterData.AsNoTracking().FirstOrDefault(m => m.MaterialNo == materialNo);

            if (originalAutoPackingCustomer != null && originalmasterData != null)
            {
                if (originalAutoPackingSpec != null)
                {
                    var autoPackingSpec = new AutoPackingSpec
                    {
                        Id = originalAutoPackingSpec.Id,
                        FactoryCode = originalAutoPackingSpec.FactoryCode,
                        MaterialNo = originalAutoPackingSpec.MaterialNo,
                        CornerGuard = originalAutoPackingCustomer.CornerGuard,
                        CPalletArrange = originalAutoPackingCustomer.CPalletArrange,
                        CPalletStackPos = originalAutoPackingCustomer.CPalletStackPos,
                        CStrapperBottomProtection = originalAutoPackingCustomer.CStrapperBottomProtection,
                        CStrapperTopProtection = originalAutoPackingCustomer.CStrapperTopProtection,
                        CStrapType = originalAutoPackingCustomer.CStrapType,
                        NBottomBoardType = originalAutoPackingCustomer.NBottomBoardType,
                        NPalletType = originalAutoPackingCustomer.NPalletType,
                        NStrapCompression = originalAutoPackingCustomer.NStrapCompression,
                        NTopBoardType = originalAutoPackingCustomer.NTopBoardType,
                        NWrapType = originalAutoPackingCustomer.NWrapType,
                        CreatedBy = originalAutoPackingSpec.CreatedBy,
                        CreatedDate = originalAutoPackingSpec.CreatedDate,
                        UpdatedBy = username,
                        UpdatedDate = DateTime.Now
                    };

                    PMTsDbContext.AutoPackingSpec.Update(autoPackingSpec);
                }
                else
                {
                    var autoPackingSpec = new AutoPackingSpec
                    {
                        FactoryCode = factoryCode,
                        MaterialNo = materialNo,
                        CornerGuard = originalAutoPackingCustomer.CornerGuard,
                        CPalletArrange = originalAutoPackingCustomer.CPalletArrange,
                        CPalletStackPos = originalAutoPackingCustomer.CPalletStackPos,
                        CStrapperBottomProtection = originalAutoPackingCustomer.CStrapperBottomProtection,
                        CStrapperTopProtection = originalAutoPackingCustomer.CStrapperTopProtection,
                        CStrapType = originalAutoPackingCustomer.CStrapType,
                        NBottomBoardType = originalAutoPackingCustomer.NBottomBoardType,
                        NPalletType = originalAutoPackingCustomer.NPalletType,
                        NStrapCompression = originalAutoPackingCustomer.NStrapCompression,
                        NTopBoardType = originalAutoPackingCustomer.NTopBoardType,
                        NWrapType = originalAutoPackingCustomer.NWrapType,
                        CreatedBy = username,
                        CreatedDate = DateTime.Now,
                    };

                    PMTsDbContext.AutoPackingSpec.Add(autoPackingSpec);
                }

                PMTsDbContext.SaveChanges();
            }
            //else
            //{
            //    throw new Exception($"Can't Create AutoPackingSpec from CusId : {cusId}");
            //}
        }
    }
}
