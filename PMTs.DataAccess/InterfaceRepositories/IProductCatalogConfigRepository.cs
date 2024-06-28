using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IProductCatalogConfigRepository : IRepository<ProductCatalogConfig>
    {
        List<ProductCatalogConfig> GetProductCatalogConfigs(string Factory, string Username);
        void RemoveProductCatalogConfigs(string Factory, string Username);
        void UpdateRemark(IConfiguration config, ProductCatalogRemark productCatalogRemark, string FactoryCode);
        ProductCatalogRemark GetRemark(IConfiguration config, string fac, string pc);

        HoldMaterial GetHoldMaterialByMaterial(string material);
        void SaveHoldMaterialByMaterial(HoldMaterial model);
        void UpdateHoldMaterialByMaterial(HoldMaterial model);
        void UpdateHoldMaterialPresale(HoldMaterial model);
        List<HoldMaterialHistory> GetHoldMaterialHistory(string material);
        void SaveHoldMaterialHistory(HoldMaterialHistory model);
        string GetOrderItemInMoData(string FactoryCode, string Material);
        List<HoldAndUnHoldMaterialResponseModel> GetResponseHoldMaterial(string MaterialNo);
        List<ScalePriceMatProductModel> GetScalePriceMatProduct(IConfiguration config, string factoryCode, string custId, string custName, string custCode, string pc1, string pc2, string pc3, string materialType, string salePlants, string plantPdts, string MaterialNo);
        List<BOMMaterialProductModel> GetBOMMaterialProduct(IConfiguration config, string factoryCode, string custId, string custName, string custCode, string pc1, string pc2, string pc3);
    }
}
