using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class TagPrintSORepository : Repository<TagPrintSo>, ITagPrintSORepository
    {
        public TagPrintSORepository(PMTsDbContext context) : base(context)
        {

        }
    }
}
