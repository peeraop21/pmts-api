using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IHoneyPaperRepository : IRepository<HoneyPaper>
    {
        HoneyPaper GetPaperByGrade(string grade);
    }
}
