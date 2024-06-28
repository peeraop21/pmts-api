using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Repositories;

namespace PMTs.DataAccess
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PMTsDbContext _context;

        public UnitOfWork(PMTsDbContext context)
        {
            _context = context;
            Additive = new AdditiveRepository(_context);
            Coating = new CoatingRepository(_context);
            Customers = new CustomerRepository(_context);
            CustShipTos = new CustShipToRepository(_context);
            //Tassanai Update 27/3/62
            MasterUsers = new MasterUserRepository(_context);
            PlantViews = new PlantViewRepository(_context);
            SalesViews = new SalesViewRepository(_context);
            //Nut Update 4/1/2019
            BoardCombines = new BoardCombineRepository(_context);
            BoardCombineAccs = new BoardCombineAccRepository(_context);
            BoardAlternatives = new BoardAlternativeRepository(_context);
            BoardSpecs = new BoardSpecRepository(_context);
            BoardUses = new BoardUseRepository(_context);
            //BoardAltSpecs = new BoardAltSpecRepository(_context);
            Flutes = new FluteRepository(_context);
            FluteTRs = new FluteTRRepository(_context);
            MapCosts = new MapCostRepository(_context);
            HireMappings = new HireMappingRepository(_context);
            HireOrders = new HireOrderRepository(_context);
            //Nut Update 4/3/2019
            AllowanceHards = new AllowanceHardRepository(_context);
            AllowanceProcesss = new AllowanceProcessRepository(_context);
            BomStructs = new BomStructRepository(_context);
            BuildRemarks = new BuildRemarkRepository(_context);
            Colors = new ColorRepository(_context);
            CompanyProfiles = new CompanyProfileRepository(_context);
            CorConfigs = new CorConfigRepository(_context);
            HierarchyLV2Matrixs = new HierarchyLv2MatrixRepository(_context);
            HierarchyLV3s = new HierarchyLV3Repository(_context);
            HierarchyLV4s = new HierarchyLV4Repository(_context);
            HoneyPapers = new HoneyPaperRepository(_context);
            Joints = new JointRepository(_context);
            Machines = new MachineRepository(_context);
            MachineGroup = new MachineGroupRepository(_context);
            MasterDatas = new MasterDataRepository(_context);
            MoBoardAlternatives = new MoBoardAlternativeRepository(_context);
            MoBoardUses = new MoBoardUseRepository(_context);
            MoDatas = new MoDataRepository(_context);
            MoDataLogs = new MoDataLogRepository(_context);
            MoRoutings = new MoRoutingRepository(_context);
            MoSpecs = new MoSpecRepository(_context);
            MoTemps = new MoTempRepository(_context);
            Pallets = new PalletRepository(_context);
            PaperGrades = new PaperGradeRepository(_context);
            PaperWidths = new PaperWidthRepository(_context);
            PMTsConfigs = new PMTsConfigRepository(_context);
            PrintMethods = new PrintMethodRepository(_context);
            ProductionTypes = new ProductionTypeRepository(_context);
            ProductGroup = new ProductGroupRepository(_context);
            Routings = new RoutingRepository(_context);
            Routings2pc = new Routing2pcRepository(_context);
            RunningNos = new RunningNoRepository(_context);
            TransactionsDetails = new TransactionsDetailRepository(_context);
            //Bent Update 3/4/62
            KindOfProductGroup = new KindOfProductGroupRepository(_context);
            ProcessCost = new ProcessCostRepository(_context);
            KindOfProduct = new KindOfProductRepository(_context);
            ProductType = new ProductTypeRepository(_context);
            MaterialType = new MaterialTypeRepository(_context);
            UnitMaterial = new UnitMaterialRepository(_context);
            PlantCostField = new PlantCostFieldRepository(_context);

            //Tassanai Update 10/4/62
            MainMenuss = new MainmenusRepository(_context);
            SubMenuss = new SubMenusRepository(_context);
            MasterRoles = new MasterRoleRepository(_context);
            ChangeHistorys = new ChangeHistoryRepository(_context);

            ScoreGaps = new ScoreGapRepository(_context);
            ScoreTypes = new ScoreTypeRepository(_context);

            Formulas = new FormulaRepository(_context);

            SbRoutings = new SbRoutingRepository(_context);

            BoardCombindMaintains = new BoardCombindMaintainRepository(_context);

            QualitySpec = new QualitySpecRepository(_context);
            QaItems = new QaItemsRepository(_context);

            HvaMaster = new HvaMasterRepository(_context);

            PresaleChangeProduct = new PresaleChangeProductRepository(_context);
            PresaleChangeRouting = new PresaleChangeRoutingRepository(_context);

            //Tassanai update 15012020
            StandardPatternNames = new StandardPatternNameRepository(_context);
            ProductCatalogConfigRepository = new ProductCatalogConfigRepository(_context);

            AttachFileMO = new AttachFileMORepository(_context);
            SetCategoriesOldMat = new SetCategoriesOldMatRepository(_context);

            PricingMaster = new PricingMasterRepository(_context);

            OrderTrackingService = new OrderTrackingServiceRepository(_context);
            VMIService = new VMIServiceRepository(_context);

            //Tassanai update 
            MenuRoles = new MenuRoleRepository(_context);
            SubMenuRoles = new SubMenuRoleRepository(_context);

            TruckOptimize = new TruckOptimizeRepository(_context);

            DocumentS = new DocumentSRepository(_context);

            ConfigWordingReport = new ConfigWordingReportRepository(_context);

            AutoPackingSpec = new AutoPackingSpecRepository(_context);
            AutoPackingCustomer = new AutoPackingCustomerRepository(_context);
            AutoPackingConfig = new AutoPackingConfigRepository(_context);

            //tassanai Update 11/01/2021
            JoinCharacter = new JoinCharacterRepository(_context);

            //tassanai Update 25/05/2021
            ProductUpPalletMon = new ProductUpPalletMonRepository(_context);

            // tassanai Update 11062021
            TagPrintSO = new TagPrintSORepository(_context);

            FSCCode = new FSCCodeRepository(_context);
            FSCFGCode = new FSCFGCodeRepository(_context);
            // tassanai Update 12052022
            PpcBoiStatus = new PpcBoiStatusRepository(_context);
            PpcWorkType = new PpcWorkTypeRepository(_context);
            //pawaratn update
            PPCRawMaterialMaster = new PPCRawMaterialMasterRepository(_context);
            PPCRawMaterialProductionBom = new PPCRawMaterialProductionBomRepository(_context);
            //Nut 23Jun2022
            PpcProductionBomLine = new PpcProductionBomLineRepository(_context);
            PpcProductionPrintingProcess = new PpcProductionPrintingProcessRepository(_context);
            PpcProductionProcess = new PpcProductionProcessRepository(_context);
            MoBomRawMat = new MoBomRawMatRepository(_context);
            PPCMasterRpac = new PPCMasterRpacRepository(_context);
            InterfaceSystemAPI = new InterfaceSystemAPIRepository(_context);
            //Nut 14Sep2023
            MachineFluteTrim = new MachineFluteTrimRepository(_context);
            Remark = new RemarkRepository(_context);
            LogPrintMo = new LogPrintMoRepository(_context);
            CustomerSKIC = new CustomerSKICRepository(_context);
            EOrderingLog = new EOrderingLogRepository(_context);
            SendEmail = new SendEmailRepository(_context);
        }

        public IAdditiveRepository Additive { get; private set; }
        public ICoatingRepository Coating { get; private set; }
        public ICustomerRepository Customers { get; private set; }
        public ICustShipToRepository CustShipTos { get; private set; }

        //Tassanai Update 27/3/62
        public IMasterUserRepository MasterUsers { get; private set; }
        public IPlantViewRepository PlantViews { get; private set; }
        public ISaleViewRepository SalesViews { get; private set; }
        public IMainmenusRepository MainMenuss { get; private set; }
        public ISubMenusRepository SubMenuss { get; private set; }

        public IChangeHistoryRepository ChangeHistorys { get; private set; }
        public IMasterRoleRepository MasterRoles { get; set; }

        //Nut Update 4/2/2019
        public IBoardAlternativeRepository BoardAlternatives { get; private set; }
        public IBoardCombineRepository BoardCombines { get; private set; }
        public IBoardCombineAccRepository BoardCombineAccs { get; private set; }
        public IBoardSpecRepository BoardSpecs { get; private set; }
        public IBoardUseRepository BoardUses { get; private set; }
        //public IBoardAltSpecRepository BoardAltSpecs { get; private set; }
        public IFluteRepository Flutes { get; private set; }
        public IFluteTRRepository FluteTRs { get; private set; }
        public IMapCostRepository MapCosts { get; private set; }
        public IHireMappingRepository HireMappings { get; private set; }
        public IHireOrderRepository HireOrders { get; private set; }
        //Nut Update 4/3/2019
        public IAllowanceHardRepository AllowanceHards { get; private set; }
        public IAllowanceProcessRepository AllowanceProcesss { get; private set; }
        public IBomStructRepository BomStructs { get; private set; }
        public IBuildRemarkRepository BuildRemarks { get; private set; }
        public IColorRepository Colors { get; private set; }
        public ICompanyProfileRepository CompanyProfiles { get; private set; }
        public ICorConfigRepository CorConfigs { get; private set; }
        public IHierarchyLV2MatrixRepository HierarchyLV2Matrixs { get; private set; }
        public IHierarchyLV3Repository HierarchyLV3s { get; private set; }
        public IHierarchyLV4Repository HierarchyLV4s { get; private set; }
        public IHoneyPaperRepository HoneyPapers { get; private set; }
        public IJointRepository Joints { get; private set; }
        public IMachineRepository Machines { get; private set; }
        public IMachineGroupRepository MachineGroup { get; private set; }
        public IMasterDataRepository MasterDatas { get; private set; }
        public IMoBoardAlternativeRepository MoBoardAlternatives { get; private set; }
        public IMoBoardUseRepository MoBoardUses { get; private set; }
        public IMoDataRepository MoDatas { get; private set; }
        public IMoDataLogRepository MoDataLogs { get; private set; }
        public IMoRoutingRepository MoRoutings { get; private set; }
        public IMoSpecRepository MoSpecs { get; private set; }
        public IMoTempRepository MoTemps { get; private set; }
        public IPalletRepository Pallets { get; private set; }
        public IPaperGradeRepository PaperGrades { get; private set; }
        public IPaperWidthRepository PaperWidths { get; private set; }
        public IPMTsConfigRepository PMTsConfigs { get; private set; }
        public IPrintMethodRepository PrintMethods { get; private set; }
        public IProductionTypeRepository ProductionTypes { get; private set; }

        public IRoutingRepository Routings { get; private set; }

        public IRouting2pcRepository Routings2pc { get; private set; }
        public IRunningNoRepository RunningNos { get; private set; }
        public ITransactionsDetailRepository TransactionsDetails { get; private set; }
        //Bent Update 3/4/2019
        public IKindOfProductGroupRepository KindOfProductGroup { get; private set; }
        public IKindOfProductRepository KindOfProduct { get; private set; }
        public IProcessCostRepository ProcessCost { get; private set; }
        public IProductTypeRepository ProductType { get; private set; }
        public IProductGroupRepository ProductGroup { get; private set; }
        public IMaterialTypeRepository MaterialType { get; private set; }
        public IUnitMaterialRepository UnitMaterial { get; private set; }
        public IPlantCostFieldRepository PlantCostField { get; private set; }

        public IScoreGapRepository ScoreGaps { get; private set; }
        public IScoreTypeRepository ScoreTypes { get; private set; }

        public IFormulaRepository Formulas { get; private set; }

        public ISbRoutingRepository SbRoutings { get; private set; }

        public IQualitySpecRepository QualitySpec { get; private set; }
        public IQaItemsRepository QaItems { get; private set; }

        public IBoardCombindMaintainRepository BoardCombindMaintains { get; private set; }

        public IHvaMasterRepository HvaMaster { get; private set; }

        public IPresaleChangeProductRepository PresaleChangeProduct { get; private set; }
        public IPresaleChangeRoutingRepository PresaleChangeRouting { get; private set; }


        //tassanai update 15/01/2020
        public IStandardPatternNameRepository StandardPatternNames { get; private set; }

        public IProductCatalogConfigRepository ProductCatalogConfigRepository { get; private set; }

        public IAttachFileMORepository AttachFileMO { get; private set; }
        public ISetCategoriesOldMatRepository SetCategoriesOldMat { get; private set; }

        public IPricingMasterRepository PricingMaster { get; private set; }

        public IOrderTrackingServiceRepository OrderTrackingService { get; private set; }
        public IVMIServiceRepository VMIService { get; private set; }

        //Tassanai update 
        public IMenuRoleRepository MenuRoles { get; private set; }

        public ISubMenuRoleRepository SubMenuRoles { get; private set; }

        public ITruckOptimizeRepository TruckOptimize { get; private set; }
        public IDocumentSRepository DocumentS { get; private set; }

        public IConfigWordingReportRepository ConfigWordingReport { get; private set; }

        public IAutoPackingSpecRepository AutoPackingSpec { get; set; }
        public IAutoPackingCustomerRepository AutoPackingCustomer { get; set; }
        public IAutoPackingConfigRepository AutoPackingConfig { get; set; }

        //Tassanai update 11/01/2021
        public IJoinCharacterRepository JoinCharacter { get; set; }

        //tassanai update 25/05/2021
        public IProductUpPalletMonRepository ProductUpPalletMon { get; set; }

        //tassanai update 
        public ITagPrintSORepository TagPrintSO { get; set; }
        public IFSCCodeRepository FSCCode { get; set; }
        public IFSCFGCodeRepository FSCFGCode { get; set; }


        // tassanai Update 12052022
        public IPpcBoiStatusRepository PpcBoiStatus { get; set; }
        public IPpcWorkTypeRepository PpcWorkType { get; set; }

        //pawaratn update
        public IPPCRawMaterialMasterRepository PPCRawMaterialMaster { get; set; }
        public IPPCRawMaterialProductionBomRepository PPCRawMaterialProductionBom { get; set; }

        //Nut 23jun2022
        public IPpcProductionBomLineRepository PpcProductionBomLine { get; set; }
        public IPpcProductionPrintingProcessRepository PpcProductionPrintingProcess { get; set; }
        public IPpcProductionProcessRepository PpcProductionProcess { get; set; }
        public IMoBomRawMatRepository MoBomRawMat { get; set; }
        public IPPCMasterRpacRepository PPCMasterRpac { get; set; }
        public IInterfaceSystemAPIRepository InterfaceSystemAPI { get; set; }

        //Nut 14Sep2023
        public IMachineFluteTrimRepository MachineFluteTrim { get; private set; }
        public IRemarkRepository Remark { get; private set; }
        public ILogPrintMoRepository LogPrintMo { get; private set; }
        public ICustomerSKICRepository CustomerSKIC { get; private set; }
        public IEOrderingLogRepository EOrderingLog { get; private set; }
        public ISendEmailRepository SendEmail { get; private set; }
        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}