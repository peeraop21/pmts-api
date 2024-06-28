using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IKindOfProductGroupRepository : IRepository<KindOfProductGroup>
    {
        IEnumerable<KindOfProductGroup> GetKindOfProductGroupsByIds(List<string> idKindOfProductGroups);
    }
}
