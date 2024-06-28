using PMTs.DataAccess.InterfaceRepositories;
using System;

namespace PMTs.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        ICoatingRepository Coating { get; }
        ICustomerRepository Customers { get; }
        ICustShipToRepository CustShipTos { get; }
        //Tassanai Update 27/3/62
        IMasterUserRepository MasterUsers { get; }
        IPlantViewRepository PlantViews { get; }
        ISaleViewRepository SalesViews { get; }
        //Nut Update 4/1/2019
        IBoardAlternativeRepository BoardAlternatives { get; }
        IBoardCombineRepository BoardCombines { get; }
        IBoardCombineAccRepository BoardCombineAccs { get; }
        IBoardSpecRepository BoardSpecs { get; }
        IBoardUseRepository BoardUses { get; }
        //IBoardAltSpecRepository BoardAltSpecs { get; }
        IFluteRepository Flutes { get; }
        IFluteTRRepository FluteTRs { get; }
        IHireOrderRepository HireOrders { get; }
        IHireMappingRepository HireMappings { get; }
        IMapCostRepository MapCosts { get; }
        IHierarchyLV3Repository HierarchyLV3s { get; }
        //Nut Update 4/3/2019
        IAllowanceHardRepository AllowanceHards { get; }
        IAllowanceProcessRepository AllowanceProcesss { get; }
        IBomStructRepository BomStructs { get; }
        IBuildRemarkRepository BuildRemarks { get; }
        IColorRepository Colors { get; }
        ICompanyProfileRepository CompanyProfiles { get; }
        ICorConfigRepository CorConfigs { get; }
        IHierarchyLV2MatrixRepository HierarchyLV2Matrixs { get; }
        IHoneyPaperRepository HoneyPapers { get; }
        IJointRepository Joints { get; }
        IMachineRepository Machines { get; }
        IMasterDataRepository MasterDatas { get; }
        IMoBoardAlternativeRepository MoBoardAlternatives { get; }
        IMoBoardUseRepository MoBoardUses { get; }
        IMoDataRepository MoDatas { get; }
        IMoDataLogRepository MoDataLogs { get; }
        IMoRoutingRepository MoRoutings { get; }
        IMoSpecRepository MoSpecs { get; }
        IMoTempRepository MoTemps { get; }
        IPalletRepository Pallets { get; }
        IPaperGradeRepository PaperGrades { get; }
        IPaperWidthRepository PaperWidths { get; }
        IPMTsConfigRepository PMTsConfigs { get; }
        IPrintMethodRepository PrintMethods { get; }
        IProductionTypeRepository ProductionTypes { get; }
        IProductGroupRepository ProductGroup { get; }
        IRoutingRepository Routings { get; }
        IRouting2pcRepository Routings2pc { get; }
        IRunningNoRepository RunningNos { get; }
        ITransactionsDetailRepository TransactionsDetails { get; }
        //Bent Update 3/4/2019
        IKindOfProductGroupRepository KindOfProductGroup { get; }
        IKindOfProductRepository KindOfProduct { get; }
        IProcessCostRepository ProcessCost { get; }
        IProductTypeRepository ProductType { get; }
        IMaterialTypeRepository MaterialType { get; }
        IUnitMaterialRepository UnitMaterial { get; }
        IMainmenusRepository MainMenuss { get; }
        IChangeHistoryRepository ChangeHistorys { get; }
        ISubMenusRepository SubMenuss { get; }
        IPlantCostFieldRepository PlantCostField { get; }

        IScoreGapRepository ScoreGaps { get; }
        IScoreTypeRepository ScoreTypes { get; }
        IAdditiveRepository Additive { get; }
        IMachineGroupRepository MachineGroup { get; }

        IQualitySpecRepository QualitySpec { get; }
        IQaItemsRepository QaItems { get; }

        IFormulaRepository Formulas { get; }

        IBoardCombindMaintainRepository BoardCombindMaintains { get; }
        IHvaMasterRepository HvaMaster { get; }

        IPresaleChangeProductRepository PresaleChangeProduct { get; }
        IPresaleChangeRoutingRepository PresaleChangeRouting { get; }

        //tassanai update 15012020
        IStandardPatternNameRepository StandardPatternNames { get; }
        IProductCatalogConfigRepository ProductCatalogConfigRepository { get; }
        int Complete();

        IAttachFileMORepository AttachFileMO { get; }
        ISetCategoriesOldMatRepository SetCategoriesOldMat { get; }

        IPricingMasterRepository PricingMaster { get; }

        IOrderTrackingServiceRepository OrderTrackingService { get; }
        IVMIServiceRepository VMIService { get; }

        //Tassanai update 
        IMenuRoleRepository MenuRoles { get; }

        ITruckOptimizeRepository TruckOptimize { get; }
        IDocumentSRepository DocumentS { get; }

        IConfigWordingReportRepository ConfigWordingReport { get; }

        IAutoPackingSpecRepository AutoPackingSpec { get; }
        IAutoPackingCustomerRepository AutoPackingCustomer { get; }
        IAutoPackingConfigRepository AutoPackingConfig { get; }

        IJoinCharacterRepository JoinCharacter { get; }

        ITagPrintSORepository TagPrintSO { get; }
        IPpcProductionPrintingProcessRepository PpcProductionPrintingProcess { get; }
        IPpcProductionProcessRepository PpcProductionProcess { get; }
        IMoBomRawMatRepository MoBomRawMat { get; }
        IPPCMasterRpacRepository PPCMasterRpac { get; }
        IInterfaceSystemAPIRepository InterfaceSystemAPI { get; }
        IMachineFluteTrimRepository MachineFluteTrim { get; }
        IRemarkRepository Remark { get; }
        ILogPrintMoRepository LogPrintMo { get; }
        ICustomerSKICRepository CustomerSKIC { get; }
        IEOrderingLogRepository EOrderingLog { get; }
        ISendEmailRepository SendEmail { get; }
    }
}