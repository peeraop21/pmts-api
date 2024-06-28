using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface ICompanyProfileRepository : IRepository<CompanyProfile>
    {
        CompanyProfile GetCompanyProfileByPlant(string plantCode);

        IEnumerable<CompanyProfile> GetCompanyProfilesBySaleOrg(string saleOrg);
        CompanyProfile GetFirstCompanyProfileBySaleOrg(string saleOrg);

    }
}
