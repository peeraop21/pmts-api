using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IBomStructRepository : IRepository<BomStruct>
    {
        BomStruct GetBomStructById(string factoryCode, int Id);
        IEnumerable<BomStruct> SearchBomStructsByMaterialNo(string factoryCode, string materialNo);
        IEnumerable<BomStruct> GetBomStructByhandshake(SqlConnection conn, string factoryCode, string materialNo);
        BomStruct SearchBomStructsByFollower(string factoryCode, string follower);
        BomStruct GetBomStructsByFollowerMasterDataNonX(string factoryCode, string follower);
        void UpdateBomstructPreviousFields(BomStruct model);

        void UpdateSaptatus(SqlConnection conn, string FactoryCode, string MaterialNo, bool Status);

        void UpdateMasterdataSapstatusForBomstruct(BomStruct model);
        void UpdateBomstructSapstatus(BomStruct model);
        void CopyBomstructToNewPlant(string parentmat, string newfactory, string oldfactory, string username);

        IEnumerable<BomStruct> GetBomstructsByMaterialNos(string factoryCode, List<string> materialNos);

    }

}
