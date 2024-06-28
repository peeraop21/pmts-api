using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class CompanyProfileRepository : Repository<CompanyProfile>, ICompanyProfileRepository
    {
        public CompanyProfileRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public IEnumerable<CompanyProfile> GetCompanyProfileAll()
        {
            return PMTsDbContext.CompanyProfile.ToList();
        }

        public CompanyProfile GetCompanyProfileByPlant(string plantCode)
        {
            return PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.Plant == plantCode);
        }

        public IEnumerable<CompanyProfile> GetCompanyProfilesBySaleOrg(string saleOrg)
        {
            return PMTsDbContext.CompanyProfile.Where(c => c.SaleOrg == saleOrg);
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public CompanyProfile GetFirstCompanyProfileBySaleOrg(string saleOrg)
        {
            return PMTsDbContext.CompanyProfile.FirstOrDefault(c => c.SaleOrg == saleOrg);
        }



    }
}
