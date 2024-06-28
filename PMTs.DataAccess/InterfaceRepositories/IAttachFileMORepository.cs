using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IAttachFileMORepository : IRepository<AttachFileMo>
    {
        IEnumerable<AttachFileMo> GetAttachFileMOsBySaleOrdersAndFactoryCode(string factoryCode, List<string> saleOrders);
        IEnumerable<AttachFileMo> GetAttachFileMOsBySaleOrders(List<string> saleOrders);
        void AttachFileMoByMat(string factoryCode, string orderItem, string attachFileMoPath);
    }
}
