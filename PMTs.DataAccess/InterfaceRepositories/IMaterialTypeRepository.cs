using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IMaterialTypeRepository : IRepository<MaterialType>
    {
        MaterialType GetMaterialTypeByMatCode(string matCode);
    }
}
