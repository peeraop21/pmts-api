using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModel;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IMasterDataRepository : IRepository<MasterData>
    {
        IEnumerable<MasterData> GetMasterDataByBomChild(string factoryCode, string MaterialNo, string Custcode, string ProductCode);

        // IEnumerable<MasterDataRoutingModel> GetMasterDataList(string factoryCode);
        IEnumerable<MasterData> GetMasterDataTop100Update(string factoryCode);

        MasterData GetMasterDataByMaterialNumber(string factoryCode, string materialNo);

        MasterData GetMasterDataByMaterialNoAndFactory(string factoryCode, string materialNo);

        MasterData SearchBomStructsByMaterialNo(string factoryCode, string materialNo);

        MasterData SearchBomStructsBytxtSearch(string factoryCode, string txtSearch);

        MasterData GetMasterDataByDescription(string factoryCode, string description);

        MasterData GetMasterDataByProdCode(string factoryCode, string prodCode);

        //IEnumerable<MasterDataRoutingModel> GetMasterDataByKeySearch(string factoryCode, string typeSearch, string keySearch);
        IEnumerable<MasterData> GetMasterDataAllByKeySearch(string keySearch);

        void UpdatePDISStatus(string FactoryCode, string MaterialNo, string Status, string Username);

        IEnumerable<MasterData> GetMasterDataHandshake(string factoryCode, string saleOrg);

        IEnumerable<MasterData> GetMasterDataHandshakeOCG(string factoryCode, string saleOrg);

        bool UpdateWeigthBox(string Mat, double? Weigth, string Username, string factoryCode);

        void UpdateTranStatus(string FactoryCode, string MaterialNo, bool Status, string Username);

        void UpdateTranStatusFromHandshake(IConfiguration configuration, string FactoryCode, string MaterialNo, bool Status, string Username);

        IEnumerable<MasterData> GetMasterDatasByMaterialNumber(string materialNo);

        MasterData GetMasterDataByMaterialNumberNonX(string factoryCode, string materialNo);

        MasterData GetMasterDataByMaterialNumberNonNotX(string factoryCode, string materialNo);

        List<MasterData> SearchBomStructsByMaterialNoAll(string factoryCode, string materialNo);

        List<MasterData> SearchBomStructsBytxtSearchAll(string factoryCode, string txtSearch);

        void UpdateTranStatusByBomStruct(string FactoryCode, string MaterialNo, string Status, string Username);

        void UpdateCapImgTransactionDetails(string FactoryCode, string MaterialNo, string Status);

        MasterData GetMasterDataByMaterialNumberX(string factoryCode, string materialNo);

        List<MasterDataQuery> GetProductCatalog(IConfiguration config, string factory, ProductCatalogsSearch ProductCatalogModel);

        IEnumerable<MasterDataRoutingModel> GetMasterDataList(IConfiguration configuration, string factoryCode);

        IEnumerable<MasterDataRoutingModel> GetMasterDataByKeySearch(IConfiguration configuration, string factoryCode, string typeSearch, string keySearch, string flag);

        List<MasterDataQuery> GetProductCatalogNotop(IConfiguration config, string factory, ProductCatalogsSearch ProductCatalogModel);

        string GetCountProductCatalogNotop(IConfiguration config, string factory, ProductCatalogsSearch ProductCatalogModel);

        IEnumerable<MasterData> GetMasterDatasByMatSaleOrgNonX(string factoryCode, string MaterialNo);

        string GetMasterDataDiecutPath(string FactoryCode, string MaterialNo);

        string GetMasterDataPalletPath(string FactoryCode, string MaterialNo);

        IEnumerable<MasterData> GetMasterDatasByMaterialNos(string factoryCode, List<string> materialNos);

        IEnumerable<MasterDataRoutingModel> GetMasterDataRoutingsByMaterialNos(IConfiguration configuration, string factoryCode, List<string> materialNos);

        IEnumerable<MasterData> GetReuseMasterDatasByMaterialNos(IConfiguration configuration, string factoryCode, List<string> materialNos);

        void UpdateReuseMaterialNos(List<MasterData> masterDatas, List<Routing> routings, List<PlantView> plantViews, List<SalesView> salesViews, List<TransactionsDetail> transactionsDetails, string Username);

        IEnumerable<MasterDataRoutingModel> GetReuseMasterDataRoutingsByMaterialNos(IConfiguration config, string factoryCode, List<string> materialNos);

        void UpdateProductCodeAndDescriptionFromPresaleNewMat(IConfiguration configuration, string factoryCode, string productCode, string description, string materialOriginal, string Username);

        void UpdateIsTranfer(string FactoryCode, string MaterialNo, string Username);

        IEnumerable<ChangeBoardNewMaterial> CreateChangeBoardNewMaterial(SqlConnection conn, string factoryCode, string username, bool IsCheckImport, List<ChangeBoardNewMaterial> changeBoardNewMaterials);

        MasterData GetOutsourceFromMaterialNoAndSaleOrg(string factoryCodeOutsource, string materialNo, string saleOrg);

        IEnumerable<ChangeBoardNewMaterial> CreateChangeFactoryNewMaterial(SqlConnection conn, string factoryCode, string username, bool v, List<ChangeBoardNewMaterial> changeBoardNewMaterials);

        IEnumerable<MasterData> UpdateMasterDatasFromExcelFile(List<MasterData> masterDatas);

        IEnumerable<Routing> UpdateRoutingsFromExcelFile(List<Routing> routings);

        IEnumerable<MasterData> GetMasterDataByMaterialAddtag(string factoryCode, string ddlSearch, string inputSerach);

        IEnumerable<MasterData> GetMasterDataByUser(string factory, string user);

        void UpdateMasterDataByChangePalletSize(string factoryCode, string userLogin, MasterData masterData);

        IEnumerable<MasterData> SearchMasterDataByMaterialNo(string factoryCode, string MaterialNo);

        IEnumerable<MasterData> GetMasterDataListByMaterialNoAndPC(IConfiguration configuration, string FactoryCode, string MaterialNo, string PC);

        void UpdateSizeDimension(string FactoryCode, string MaterialNo, string Username, string SizeDimension);

        IEnumerable<MasterData> GetMasterDataListByDateTime(string factoryCode, string DateFrom, string DateTo);

        IEnumerable<string> GetBoardDistinctFromMasterData(string factoryCode);

        List<ChangeBoardNewMaterial> GetForTemplateChangeBoardNewMaterials(string factoryCode, SearchMaterialTemplateParam searchMaterialTemplateParam);

        object GetCustomerDistinctFromMasterData(string factoryCode);

        List<CheckStatusColor> GetReportCheckStatusColor(IConfiguration config, string factoryCode, int colorId);

        void UpdateLeadTime(string FactoryCode, string MaterialNo, List<Routing> model, string Username);
    }
}