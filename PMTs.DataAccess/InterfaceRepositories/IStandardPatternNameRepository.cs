using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IStandardPatternNameRepository : IRepository<StandardPatternName>
    {
        StandardPatternName GetStandardPatternName(string palletname);

        PalletCalulate GetCalculatePallet(string FactoryCode, PalletCalculateParam model);
    }
}
