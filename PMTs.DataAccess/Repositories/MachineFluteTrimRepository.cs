using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.Repositories
{
    public class MachineFluteTrimRepository : Repository<MachineFluteTrim>, IMachineFluteTrimRepository
    {
        public MachineFluteTrimRepository(PMTsDbContext context) : base(context)
        {

        }











        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
