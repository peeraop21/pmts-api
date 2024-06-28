using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public Customer GetCustomerByCusID(string factoryCode, string cusID)
        {
            return PMTsDbContext.Customer.FirstOrDefault(w => w.CusId == cusID);
        }

        public List<Customer> GetCustomerById(string factoryCode, int Id)
        {
            return PMTsDbContext.Customer.Where(w => w.CusId == Id.ToString()).ToList();
        }
        public List<Customer> GetCustomersByCusId(string factoryCode, string cusId)
        {
            return PMTsDbContext.Customer.Where(w => w.CusId == cusId.ToString()).ToList();
        }

        public IEnumerable<CustomerShipToViewModel> GetCustomerByCustname(string factoryCode, string CustName)
        {
            var companyProfile = PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.Plant == factoryCode);
            var CustomerShipTo = (from c in PMTsDbContext.Customer.AsParallel().WithDegreeOfParallelism(10)

                                      //where c.CustName.ToUpper().Contains(CustName.ToUpper()) && !string.IsNullOrEmpty(c.CusId) && c.CustomerGroup == companyProfile.CustomerGroup

                                  where (c.CustName != null && c.CustName.ToUpper().Contains(CustName.ToUpper())) && !string.IsNullOrEmpty(c.CusId) && c.CustomerGroup == companyProfile.CustomerGroup

                                  select new CustomerShipToViewModel
                                  {
                                      Id = c.Id,
                                      CustName = c.CustName,
                                      CustCode = c.CustCode,
                                      SoldToCode = c.SoldToCode,
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
                                      CreatedBy = c.CreatedBy,
                                      CreatedDate = c.CreatedDate,
                                      Cust = c.Cust,
                                      PriorityFlag = c.PriorityFlag,
                                      UpdatedBy = c.UpdatedBy,
                                      UpdatedDate = c.UpdatedDate,
                                      TagBundle = c.TagBundle,
                                      TagPallet = c.TagPallet,
                                      NoTagBundle = c.NoTagBundle,
                                      HeadTagBundle = c.HeadTagBundle,
                                      FootTagBundle = c.FootTagBundle,
                                      HeadTagPallet = c.HeadTagPallet,
                                      FootTagPallet = c.FootTagPallet,
                                      Freetext1TagBundle = c.Freetext1TagBundle,
                                      Freetext2TagBundle = c.Freetext2TagBundle,
                                      Freetext3TagBundle = c.Freetext3TagBundle,

                                      COA = c.Coa,
                                      Film = c.Film,

                                      Freetext1TagPallet = c.Freetext1TagPallet,
                                      Freetext2TagPallet = c.Freetext2TagPallet,
                                      Freetext3TagPallet = c.Freetext3TagPallet,

                                  }).ToList();
            //}).AsSequential().Take(100).ToList();

            return CustomerShipTo;
        }
        public IEnumerable<CustomerShipToViewModel> GetCustomerByCustCode(string factoryCode, string CustCode)
        {
            var companyProfile = PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.Plant == factoryCode);
            var CustomerShipTo = (from c in PMTsDbContext.Customer.AsParallel().WithDegreeOfParallelism(10)

                                  where (c.CustCode != null && c.CustCode.ToUpper().Contains(CustCode.ToUpper())) && !string.IsNullOrEmpty(c.CusId) && c.CustomerGroup == companyProfile.CustomerGroup
                                  select new CustomerShipToViewModel
                                  {
                                      Id = c.Id,
                                      CustName = c.CustName,
                                      CustCode = c.CustCode,
                                      SoldToCode = c.SoldToCode,
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
                                      CreatedBy = c.CreatedBy,
                                      CreatedDate = c.CreatedDate,
                                      Cust = c.Cust,
                                      PriorityFlag = c.PriorityFlag,
                                      UpdatedBy = c.UpdatedBy,
                                      UpdatedDate = c.UpdatedDate,
                                      TagBundle = c.TagBundle,
                                      TagPallet = c.TagPallet,
                                      NoTagBundle = c.NoTagBundle,
                                      HeadTagBundle = c.HeadTagBundle,
                                      FootTagBundle = c.FootTagBundle,
                                      HeadTagPallet = c.HeadTagPallet,
                                      FootTagPallet = c.FootTagPallet,
                                      Freetext1TagBundle = c.Freetext1TagBundle,
                                      Freetext2TagBundle = c.Freetext2TagBundle,
                                      Freetext3TagBundle = c.Freetext3TagBundle,

                                      Freetext1TagPallet = c.Freetext1TagPallet,
                                      Freetext2TagPallet = c.Freetext2TagPallet,
                                      Freetext3TagPallet = c.Freetext3TagPallet,

                                      COA = c.Coa,
                                      Film = c.Film

                                  }).ToList();
            //}).AsSequential().Take(100).ToList();

            return CustomerShipTo;
        }
        public IEnumerable<CustomerShipToViewModel> GetCustomerByCustId(string factoryCode, string CustId)
        {

            var CustomerShipTo = (from c in PMTsDbContext.Customer.AsParallel().WithDegreeOfParallelism(10)

                                  where c.CustStatus == true && (c.CusId != null && c.CusId.ToUpper().Contains(CustId.ToUpper()))
                                  select new CustomerShipToViewModel
                                  {
                                      Id = c.Id,
                                      CustName = c.CustName,
                                      CustCode = c.CustCode,
                                      SoldToCode = c.SoldToCode,
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
                                      CreatedBy = c.CreatedBy,
                                      CreatedDate = c.CreatedDate,
                                      Cust = c.Cust,
                                      PriorityFlag = c.PriorityFlag,
                                      UpdatedBy = c.UpdatedBy,
                                      UpdatedDate = c.UpdatedDate,
                                      TagBundle = c.TagBundle,
                                      TagPallet = c.TagPallet,
                                      NoTagBundle = c.NoTagBundle,
                                      HeadTagBundle = c.HeadTagBundle,
                                      FootTagBundle = c.FootTagBundle,
                                      HeadTagPallet = c.HeadTagPallet,
                                      FootTagPallet = c.FootTagPallet,
                                      Freetext1TagBundle = c.Freetext1TagBundle,
                                      Freetext2TagBundle = c.Freetext2TagBundle,
                                      Freetext3TagBundle = c.Freetext3TagBundle,

                                      Freetext1TagPallet = c.Freetext1TagPallet,
                                      Freetext2TagPallet = c.Freetext2TagPallet,
                                      Freetext3TagPallet = c.Freetext3TagPallet,

                                      COA = c.Coa,
                                      Film = c.Film
                                  }).ToList();
            //}).AsSequential().Take(100).ToList();

            return CustomerShipTo;
        }

        public Customer GetCustomerByCustIdAndCustName(string factoryCode, string CustId, string custName)
        {
            return PMTsDbContext.Customer.FirstOrDefault(w => w.CusId.Equals(CustId) && w.CustName.Contains(custName));
        }

        public IEnumerable<CustomerShipToViewModel> GetCustomerShipTo(string factoryCode)
        {
            var companyProfile = PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.Plant == factoryCode);
            //var CustomerShipTo = (from c in PMTsDbContext.Customer
            //join s in PMTsDbContext.CustShipTo
            //              on c.CustCode equals s.CustCode into s2
            //              from f in s2.DefaultIfEmpty()
            //              where c.CustStatus == true
            //              select new CustomerShipToViewModel
            //              {
            //                  Id = c.Id,
            //                  CustName = c.CustName,
            //                  CustCode = c.CustCode,
            //                  SoldToCode = c.SoldToCode,
            //                  CustId = c.CusId,
            //                  Zone = c.Zone,
            //                  Route = c.Route,
            //                  CustClass = c.CustClass,
            //                  IndGrp = c.IndGrp,
            //                  CustReq = c.CustReq,
            //                  CustAlert = c.CustAlert,
            //                  CustStatus = c.CustStatus,
            //                  CustShipTo = (f.ShipTo == null ? null : f.ShipTo),
            //              }).ToList();

            var CustomerShipTo = (from c in PMTsDbContext.Customer.AsParallel().WithDegreeOfParallelism(10)
                                      //join s in PMTsDbContext.CustShipTo.AsParallel()
                                      //on c.CustCode equals s.CustCode
                                      //into s2
                                      //from f in s2.DefaultIfEmpty()
                                  where c.CustomerGroup == companyProfile.CustomerGroup
                                  select new CustomerShipToViewModel
                                  {
                                      Id = c.Id,
                                      CustName = c.CustName,
                                      CustCode = c.CustCode,
                                      SoldToCode = c.SoldToCode,
                                      CusId = c.CusId,
                                      Zone = c.Zone,
                                      Route = c.Route,
                                      CustClass = c.CustClass,
                                      IndGrp = c.IndGrp,
                                      CustReq = c.CustReq,
                                      CustAlert = c.CustAlert,
                                      CustStatus = c.CustStatus,
                                      PalletOverhang = c.PalletOverhang,
                                      CustShipTo = null, //(s.ShipTo ?? null),
                                      QASpec = c.Qaspec,
                                      Accgroup = c.Accgroup,
                                      CreatedBy = c.CreatedBy,
                                      CreatedDate = c.CreatedDate,
                                      Cust = c.Cust,
                                      PriorityFlag = c.PriorityFlag,
                                      UpdatedBy = c.UpdatedBy,
                                      UpdatedDate = c.UpdatedDate,
                                      TagBundle = c.TagBundle,
                                      TagPallet = c.TagPallet,
                                      NoTagBundle = c.NoTagBundle,
                                      HeadTagBundle = c.HeadTagBundle,
                                      FootTagBundle = c.FootTagBundle,
                                      HeadTagPallet = c.HeadTagPallet,
                                      FootTagPallet = c.FootTagPallet,
                                      Freetext1TagBundle = c.Freetext1TagBundle,
                                      Freetext2TagBundle = c.Freetext2TagBundle,
                                      Freetext3TagBundle = c.Freetext3TagBundle,

                                      Freetext1TagPallet = c.Freetext1TagPallet,
                                      Freetext2TagPallet = c.Freetext2TagPallet,
                                      Freetext3TagPallet = c.Freetext3TagPallet,

                                      COA = c.Coa,
                                      Film = c.Film


                                  }).OrderBy(c => c.Id).Take(100).ToList();
            //}).AsSequential().Take(100).ToList();

            return CustomerShipTo;


            //List<Customer> cus = new List<Customer>();
            //cus = PMTsDbContext.Customer.Where(x => x.CustStatus == true).AsParallel().ToList();

            //#region Parallel
            //var cts = new System.Threading.CancellationTokenSource();
            //var options = new System.Threading.Tasks.ParallelOptions
            //{
            //    MaxDegreeOfParallelism = cus.Count(),
            //    CancellationToken = cts.Token,
            //};
            //var concurrentBag = new System.Collections.Concurrent.ConcurrentBag<CustomerShipToViewModel>();

            //System.Threading.Tasks.Parallel.ForEach(cus, options, property =>
            //{
            //    try
            //    {
            //        CustomerShipToViewModel csv = new CustomerShipToViewModel();
            //        csv.Id = property.Id;
            //        csv.CustName = property.CustName;
            //        csv.CustCode = property.CustCode;
            //        csv.SoldToCode = property.SoldToCode;
            //        csv.CustId = property.CusId;
            //        csv.Zone = property.Zone;
            //        csv.Route = property.Route;
            //        csv.CustClass = property.CustClass;
            //        csv.IndGrp = property.IndGrp;
            //        csv.CustReq = property.CustReq;
            //        csv.CustAlert = property.CustAlert;
            //        csv.CustStatus = property.CustStatus;
            //        csv.CustShipTo = null;
            //        concurrentBag.Add(csv);
            //        options.CancellationToken.ThrowIfCancellationRequested();
            //    }
            //    catch (Exception ex)
            //    {
            //        System.Diagnostics.Debug.WriteLine(ex.Message);
            //        cts.Cancel();
            //    }
            //});
            //#endregion
            //return concurrentBag;
        }

        public IEnumerable<CustomerShipToViewModel> GetCustomerShipToByParalel(string factoryCode)
        {
            List<Customer> cus = new List<Customer>();
            cus = PMTsDbContext.Customer.Where(x => x.CustStatus == true).AsParallel().ToList();

            #region Parallel
            var cts = new System.Threading.CancellationTokenSource();
            var options = new System.Threading.Tasks.ParallelOptions
            {
                MaxDegreeOfParallelism = cus.Count(),
                CancellationToken = cts.Token,
            };
            var concurrentBag = new System.Collections.Concurrent.ConcurrentBag<CustomerShipToViewModel>();

            System.Threading.Tasks.Parallel.ForEach(cus, options, property =>
            {
                try
                {
                    CustomerShipToViewModel csv = new CustomerShipToViewModel();
                    csv.Id = property.Id;
                    csv.CustName = property.CustName;
                    csv.CustCode = property.CustCode;
                    csv.SoldToCode = property.SoldToCode;
                    csv.CusId = property.CusId;
                    csv.Zone = property.Zone;
                    csv.Route = property.Route;
                    csv.CustClass = property.CustClass;
                    csv.IndGrp = property.IndGrp;
                    csv.CustReq = property.CustReq;
                    csv.CustAlert = property.CustAlert;
                    csv.CustStatus = property.CustStatus;
                    csv.CustShipTo = null;
                    concurrentBag.Add(csv);
                    options.CancellationToken.ThrowIfCancellationRequested();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                    cts.Cancel();
                }
            });
            #endregion
            return concurrentBag;
            //var CustomerShipTo = (from c in PMTsDbContext.Customer.AsParallel()

            //                      where c.CustStatus == true
            //                      select new CustomerShipToViewModel
            //                      {
            //                          Id = c.Id,
            //                          CustName = c.CustName,
            //                          CustCode = c.CustCode,
            //                          SoldToCode = c.SoldToCode,
            //                          CustId = c.CusId,
            //                          Zone = c.Zone,
            //                          Route = c.Route,
            //                          CustClass = c.CustClass,
            //                          IndGrp = c.IndGrp,
            //                          CustReq = c.CustReq,
            //                          CustAlert = c.CustAlert,
            //                          CustStatus = c.CustStatus,
            //                          CustShipTo = (s.ShipTo ?? null),
            //                      }).ToList();

            //var CustomerShipTo = (from c in cus.AsEnumerable()
            //                      join s in ship.AsEnumerable()
            //                      on c.CustCode equals s.CustCode 
            //                      into s2 from f in s2.DefaultIfEmpty()
            //                      where c.CustStatus == true
            //                      select new CustomerShipToViewModel
            //                      {
            //                          Id = c.Id,
            //                          CustName = c.CustName,
            //                          CustCode = c.CustCode,
            //                          SoldToCode = c.SoldToCode,
            //                          CustId = c.CusId,
            //                          Zone = c.Zone,
            //                          Route = c.Route,
            //                          CustClass = c.CustClass,
            //                          IndGrp = c.IndGrp,
            //                          CustReq = c.CustReq,
            //                          CustAlert = c.CustAlert,
            //                          CustStatus = c.CustStatus,
            //                          CustShipTo = (f.ShipTo ?? null),
            //                      }).ToList();


        }



        public IEnumerable<CustomerShipToViewModel> GetCustomerShipToByCustCode(string factoryCode, string custCode)
        {
            var companyProfile = PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.Plant == factoryCode);
            var customer = PMTsDbContext.Customer.Where(c => !string.IsNullOrEmpty(c.CusId) && c.CustomerGroup == companyProfile.CustomerGroup).ToList();
            var custShipTo = PMTsDbContext.CustShipTo.ToList();

            List<CustomerShipToViewModel> CustomerShipToList = new List<CustomerShipToViewModel>();

            customer.ForEach(i =>
            {
                CustomerShipToList.Add(new CustomerShipToViewModel
                {
                    Id = i.Id,
                    CustName = i.CustName,
                    CustCode = i.CustCode,
                    SoldToCode = i.SoldToCode,
                    CusId = i.CusId,
                    Zone = i.Zone,
                    Route = i.Route,
                    CustClass = i.CustClass,
                    CustReq = i.CustReq,
                    CustAlert = i.CustAlert,
                    CustStatus = i.CustStatus,
                    PalletOverhang = i.PalletOverhang,
                    ShipTo = custShipTo.Where(s => s.CustCode == i.CustCode).ToList(),
                    QASpec = i.Qaspec,
                    Accgroup = i.Accgroup,
                    CreatedBy = i.CreatedBy,
                    CreatedDate = i.CreatedDate,
                    Cust = i.Cust,
                    PriorityFlag = i.PriorityFlag,
                    UpdatedBy = i.UpdatedBy,
                    UpdatedDate = i.UpdatedDate
                });
            });
            return CustomerShipToList;
        }

        public IEnumerable<Customer> selectAllcus()
        {
            List<Customer> cus = new List<Customer>();
            cus = (from ord in PMTsDbContext.Customer.AsParallel().WithDegreeOfParallelism(5)
                   orderby ord.CusId
                   select ord).
                       AsSequential().ToList();
            return cus;
        }

        public IEnumerable<Customer> selectAllcusNoParalel()
        {
            List<Customer> cus = new List<Customer>();
            cus = (from ord in PMTsDbContext.Customer
                   orderby ord.CusId
                   select ord).
                       ToList();
            return cus;
        }

        public IEnumerable<CustomerShipToViewModel> GetCustomerByCustIdAndCusCode(string factoryCode, string CustomerId, string CustCode)
        {

            var CustomerShipTo = (from c in PMTsDbContext.Customer.AsParallel().WithDegreeOfParallelism(10)
                                  where c.CustStatus == true && (c.CusId != null && c.CusId.ToUpper().Contains(CustomerId.ToUpper()) && c.CustCode.ToUpper().Contains(CustCode.ToUpper()))
                                  select new CustomerShipToViewModel
                                  {
                                      Id = c.Id,
                                      CustName = c.CustName,
                                      CustCode = c.CustCode,
                                      SoldToCode = c.SoldToCode,
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
                                      CreatedBy = c.CreatedBy,
                                      CreatedDate = c.CreatedDate,
                                      Cust = c.Cust,
                                      PriorityFlag = c.PriorityFlag,
                                      UpdatedBy = c.UpdatedBy,
                                      UpdatedDate = c.UpdatedDate
                                  }).ToList();
            //}).AsSequential().Take(100).ToList();

            return CustomerShipTo;
        }


        public void DeleteCustomerByID(int ID)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var it_trans_update = PMTsDbContext.Customer.Where(IT => IT.Id == ID).FirstOrDefault();
                    it_trans_update.CustStatus = false;
                    PMTsDbContext.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch
                {
                    dbContextTransaction.Rollback();
                }
            }
        }

        public IEnumerable<Customer> GetCustomersByCustomerGroup(string factoryCode)
        {
            var customer = new List<Customer>();
            var companyProfile = PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.Plant == factoryCode);

            customer = PMTsDbContext.Customer.Where(c => !string.IsNullOrEmpty(c.CusId) && c.CustomerGroup == companyProfile.CustomerGroup && c.CustStatus.HasValue && c.CustStatus.Value).ToList();
            return customer;
        }
    }
}