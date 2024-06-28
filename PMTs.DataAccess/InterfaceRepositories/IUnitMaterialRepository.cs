using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IUnitMaterialRepository : IRepository<UnitMaterial>
    {
        UnitMaterial GetUnitMaterialByName(string Name);

    }
}
