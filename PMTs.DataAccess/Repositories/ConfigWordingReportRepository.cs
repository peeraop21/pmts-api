using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class ConfigWordingReportRepository : Repository<ConfigWordingReport>, IConfigWordingReportRepository
    {
        public ConfigWordingReportRepository(PMTsDbContext context) : base(context)
        {

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public ConfigWordingReport UpdateConfigWordingReportByFactoryCode(string factoryCode, string username, string configWording)
        {
            var oldConfigWording = PMTsDbContext.ConfigWordingReport.FirstOrDefault(c => c.FactoryCode == factoryCode);
            if (oldConfigWording != null)
            {
                oldConfigWording.Wording = configWording;
                oldConfigWording.CreatedBy = username;
                oldConfigWording.CreatedDate = DateTime.Now;
                PMTsDbContext.ConfigWordingReport.Update(oldConfigWording);
                PMTsDbContext.SaveChanges();
                return oldConfigWording;
            }
            else
            {
                var result = new ConfigWordingReport
                {
                    FactoryCode = factoryCode,
                    Wording = configWording,
                    CreatedBy = username,
                    CreatedDate = DateTime.Now,
                };

                PMTsDbContext.ConfigWordingReport.Add(result);
                PMTsDbContext.SaveChanges();
                return result;
            }
        }
    }
}
