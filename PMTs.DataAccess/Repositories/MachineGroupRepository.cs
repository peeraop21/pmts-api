
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class MachineGroupRepository : Repository<MachineGroup>, IMachineGroupRepository
    {

        private readonly PMTsDbContext _context;
        public MachineGroupRepository(PMTsDbContext context) : base(context)
        {
            _context = context;
        }

        public IEnumerable<GroupMachineModels> GetMachineGroup(string FactoryCode)
        {
            List<GroupMachineModels> machines = new List<GroupMachineModels>();
            //machines = (from m in _context.Machine
            //             join g in _context.MachineGroup
            //             on m.MachineGroup equals g.GroupCode
            //             where m.FactoryCode  == FactoryCode
            //             select new GroupMachineModels { Id =  m.MachineGroup , GroupMachine = g.GroupName }
            //              ).Distinct().ToList();

            var tmp = (from m in _context.Machine
                       join g in _context.MachineGroup
                       on m.MachineGroup equals g.GroupCode
                       where m.FactoryCode == FactoryCode
                       select new { Id = m.MachineGroup, GroupMachine = g.GroupName, sort = g.SortIndex }
                       ).Distinct().ToList();
            foreach (var item in tmp.OrderBy(x => x.sort).ToList())
            {
                GroupMachineModels tmpmachine = new GroupMachineModels();
                tmpmachine.Id = item.Id;
                tmpmachine.GroupMachine = item.GroupMachine;
                machines.Add(tmpmachine);
            }

            return machines;
        }
    }
}
