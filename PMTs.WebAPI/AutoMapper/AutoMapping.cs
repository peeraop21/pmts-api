using AutoMapper;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;

namespace PMTs.WebAPI.AutoMapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<MasterData, MoSpec>();
            CreateMap<MoRouting, Routing>().ReverseMap();
            CreateMap<ReCalculateTrimModel, ReCalculateTrimResultModel>();
            //CreateMap<List<Routing>, List<Routing2pc>>();
            CreateMap<Routing, Routing2pc>().ReverseMap();
            CreateMap<MoData, MoDatalog>();
            CreateMap<MoData, MoDataPrintMastercard>();
            //CreateMap<List<MoRouting>, List<MoRoutingPrintMastercard>>();
            CreateMap<MoRouting, MoRoutingPrintMastercard>();
            CreateMap<MoData, MoDataPrintMastercard>();
        }
    }
}
