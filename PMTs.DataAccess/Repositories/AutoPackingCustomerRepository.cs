using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class AutoPackingCustomerRepository : Repository<AutoPackingCustomer>, IAutoPackingCustomerRepository
    {
        public AutoPackingCustomerRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public AutoPackingCustomer GetAutoPackingCustomerByCusID(string factoryCode, string cusID)
        {
            return PMTsDbContext.AutoPackingCustomer.FirstOrDefault(a => a.CusId == cusID);
        }

        public List<AutoPackingCustomerData> GetAllAutoPackingCustomerAndCustomer(string FactoryCode)
        {
            var companyProfile = PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.Plant == FactoryCode);
            var customers = (from c in PMTsDbContext.Customer.AsEnumerable()
                             join a in PMTsDbContext.AutoPackingCustomer.AsEnumerable()
                             on c.CusId equals a.CusId
                             into s2
                             from f in s2
                             where c.CustomerGroup == companyProfile.CustomerGroup
                             select new AutoPackingCustomerData
                             {
                                 CustCode = c.CustCode,
                                 SoldToCode = c.SoldToCode,
                                 CusName = c.CustName,
                                 CusId = c.CusId,
                                 Zone = c.Zone,
                                 Route = c.Route,
                                 CustClass = c.CustClass,
                                 IndGrp = c.IndGrp,
                                 CustReq = c.CustReq,
                                 CustAlert = c.CustAlert,
                                 CustStatus = c.CustStatus,
                                 PalletOverhang = c.PalletOverhang,
                                 CustShipTo = null,
                                 QASpec = c.Qaspec,
                                 Accgroup = c.Accgroup,
                                 Cust = c.Cust,
                                 PriorityFlag = c.PriorityFlag,
                                 CornerGuard = f.CornerGuard,
                                 CPalletArrange = f.CPalletArrange,
                                 NPalletType = f.NPalletType,
                                 CPalletStackPos = f.CPalletStackPos,
                                 NStrapCompression = f.NStrapCompression,
                                 CStrapType = f.CStrapType,
                                 NWrapType = f.NWrapType,
                                 NTopBoardType = f.NTopBoardType,
                                 NBottomBoardType = f.NBottomBoardType,
                                 CStrapperBottomProtection = f.CStrapperBottomProtection,
                                 CStrapperTopProtection = f.CStrapperTopProtection,
                                 CreatedBy = f.CreatedBy,
                                 CreatedDate = f.CreatedDate,
                                 UpdatedBy = f.UpdatedBy,
                                 UpdatedDate = f.UpdatedDate,
                                 HasAutoPackingCustomer = !string.IsNullOrEmpty(f.CusId)
                             }).OrderBy(c => c.Id).Take(100).ToList();

            return customers;
        }

        public List<AutoPackingCustomerData> GetAutoPackingCustomerAndCustomerByCustName(string FactoryCode, string CustName)
        {
            var companyProfile = PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.Plant == FactoryCode);
            var customers = (from c in PMTsDbContext.Customer.AsEnumerable()
                             where c.CustName.Contains(CustName)
                             join a in PMTsDbContext.AutoPackingCustomer.AsEnumerable()
                             on c.CusId equals a.CusId
                             into s2
                             from f in s2
                             where c.CustomerGroup == companyProfile.CustomerGroup
                             select new AutoPackingCustomerData
                             {
                                 CustCode = c.CustCode,
                                 SoldToCode = c.SoldToCode,
                                 CusName = c.CustName,
                                 CusId = c.CusId,
                                 Zone = c.Zone,
                                 Route = c.Route,
                                 CustClass = c.CustClass,
                                 IndGrp = c.IndGrp,
                                 CustReq = c.CustReq,
                                 CustAlert = c.CustAlert,
                                 CustStatus = c.CustStatus,
                                 PalletOverhang = c.PalletOverhang,
                                 CustShipTo = null,
                                 QASpec = c.Qaspec,
                                 Accgroup = c.Accgroup,
                                 Cust = c.Cust,
                                 PriorityFlag = c.PriorityFlag,
                                 NPalletType = f.NPalletType,
                                 CornerGuard = f.CornerGuard,
                                 CPalletArrange = f.CPalletArrange,
                                 CPalletStackPos = f.CPalletStackPos,
                                 NStrapCompression = f.NStrapCompression,
                                 CStrapType = f.CStrapType,
                                 NWrapType = f.NWrapType,
                                 NTopBoardType = f.NTopBoardType,
                                 NBottomBoardType = f.NBottomBoardType,
                                 CStrapperBottomProtection = f.CStrapperBottomProtection,
                                 CStrapperTopProtection = f.CStrapperTopProtection,
                                 CreatedBy = f.CreatedBy,
                                 CreatedDate = f.CreatedDate,
                                 UpdatedBy = f.UpdatedBy,
                                 UpdatedDate = f.UpdatedDate,
                                 HasAutoPackingCustomer = !string.IsNullOrEmpty(f.CusId)
                             }).OrderBy(c => c.Id).Take(100).ToList();

            return customers;
        }

        public List<AutoPackingCustomerData> GetAutoPackingCustomerAndCustomerByCustCode(string FactoryCode, string CustCode)
        {
            var companyProfile = PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.Plant == FactoryCode);
            var customers = (from c in PMTsDbContext.Customer.AsEnumerable()
                             where c.CustCode == CustCode
                             join a in PMTsDbContext.AutoPackingCustomer.AsEnumerable()
                             on c.CusId equals a.CusId
                             into s2
                             from f in s2.DefaultIfEmpty()
                             where c.CustomerGroup == companyProfile.CustomerGroup
                             select new AutoPackingCustomerData
                             {
                                 CustCode = c.CustCode,
                                 SoldToCode = c.SoldToCode,
                                 CusName = c.CustName,
                                 CusId = c.CusId,
                                 Zone = c.Zone,
                                 Route = c.Route,
                                 CustClass = c.CustClass,
                                 IndGrp = c.IndGrp,
                                 CustReq = c.CustReq,
                                 CustAlert = c.CustAlert,
                                 CustStatus = c.CustStatus,
                                 PalletOverhang = c.PalletOverhang,
                                 CustShipTo = null,
                                 QASpec = c.Qaspec,
                                 Accgroup = c.Accgroup,
                                 Cust = c.Cust,
                                 PriorityFlag = c.PriorityFlag,
                                 CornerGuard = f.CornerGuard,
                                 CPalletArrange = f.CPalletArrange,
                                 NPalletType = f.NPalletType,
                                 CPalletStackPos = f.CPalletStackPos,
                                 NStrapCompression = f.NStrapCompression,
                                 CStrapType = f.CStrapType,
                                 NWrapType = f.NWrapType,
                                 NTopBoardType = f.NTopBoardType,
                                 NBottomBoardType = f.NBottomBoardType,
                                 CStrapperBottomProtection = f.CStrapperBottomProtection,
                                 CStrapperTopProtection = f.CStrapperTopProtection,
                                 CreatedBy = f.CreatedBy,
                                 CreatedDate = f.CreatedDate,
                                 UpdatedBy = f.UpdatedBy,
                                 UpdatedDate = f.UpdatedDate,
                                 HasAutoPackingCustomer = String.IsNullOrEmpty(f.CusId) ? false : true
                             }).OrderBy(c => c.Id).Take(100).ToList();

            return customers;
        }

        public List<AutoPackingCustomerData> GetAutoPackingCustomerAndCustomerByCusId(string FactoryCode, string CusId)
        {
            var companyProfile = PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.Plant == FactoryCode);
            var customers = (from c in PMTsDbContext.Customer.AsEnumerable()
                             where c.CusId.Contains(CusId)
                             join a in PMTsDbContext.AutoPackingCustomer.AsEnumerable()
                             on c.CusId equals a.CusId
                             into s2
                             from f in s2.DefaultIfEmpty()
                             where c.CustomerGroup == companyProfile.CustomerGroup
                             select new AutoPackingCustomerData
                             {
                                 CustCode = c.CustCode,
                                 SoldToCode = c.SoldToCode,
                                 CusName = c.CustName,
                                 CusId = c.CusId,
                                 Zone = c.Zone,
                                 Route = c.Route,
                                 CustClass = c.CustClass,
                                 IndGrp = c.IndGrp,
                                 CustReq = c.CustReq,
                                 CustAlert = c.CustAlert,
                                 CustStatus = c.CustStatus,
                                 PalletOverhang = c.PalletOverhang,
                                 CustShipTo = null,
                                 QASpec = c.Qaspec,
                                 Accgroup = c.Accgroup,
                                 Cust = c.Cust,
                                 PriorityFlag = c.PriorityFlag,
                                 NPalletType = f.NPalletType,
                                 CornerGuard = f.CornerGuard,
                                 CPalletArrange = f.CPalletArrange,
                                 CPalletStackPos = f.CPalletStackPos,
                                 NStrapCompression = f.NStrapCompression,
                                 CStrapType = f.CStrapType,
                                 NWrapType = f.NWrapType,
                                 NTopBoardType = f.NTopBoardType,
                                 NBottomBoardType = f.NBottomBoardType,
                                 CStrapperBottomProtection = f.CStrapperBottomProtection,
                                 CStrapperTopProtection = f.CStrapperTopProtection,
                                 CreatedBy = f.CreatedBy,
                                 CreatedDate = f.CreatedDate,
                                 UpdatedBy = f.UpdatedBy,
                                 UpdatedDate = f.UpdatedDate,
                                 HasAutoPackingCustomer = String.IsNullOrEmpty(f.CusId) ? false : true
                             }).OrderBy(c => c.Id).Take(100).ToList();

            return customers;
        }

        public void UpdateAutoPackingCustomer(AutoPackingCustomer model)
        {
            var originalAutoPackingCustomer = PMTsDbContext.AutoPackingCustomer.AsNoTracking().FirstOrDefault(a => a.CusId == model.CusId);
            if (originalAutoPackingCustomer != null)
            {
                var autoPackingCustomer = new AutoPackingCustomer
                {
                    CusId = model.CusId,
                    CusName = model.CusName,
                    CornerGuard = model.CornerGuard,
                    CPalletArrange = model.CPalletArrange,
                    CPalletStackPos = model.CPalletStackPos,
                    CStrapperBottomProtection = model.CStrapperBottomProtection,
                    CStrapperTopProtection = model.CStrapperTopProtection,
                    CStrapType = model.CStrapType,
                    NBottomBoardType = model.NBottomBoardType,
                    NPalletType = model.NPalletType,
                    NStrapCompression = model.NStrapCompression,
                    NTopBoardType = model.NTopBoardType,
                    NWrapType = model.NWrapType,
                    CreatedBy = originalAutoPackingCustomer.CreatedBy,
                    CreatedDate = originalAutoPackingCustomer.CreatedDate,
                    UpdatedBy = model.UpdatedBy,
                    UpdatedDate = model.UpdatedDate,
                };
                PMTsDbContext.AutoPackingCustomer.Update(autoPackingCustomer);
                PMTsDbContext.SaveChanges();
            }
            else
            {
                throw new Exception($"Can't update AutoPackingCustomer from cusId : {model.CusId}");
            }
        }
    }
}
