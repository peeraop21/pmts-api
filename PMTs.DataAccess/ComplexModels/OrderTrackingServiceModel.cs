using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.ComplexModels
{
    public class OrderTrackingServiceModel
    {
        public OrderTrackingServiceModel()
        {
            moDatas = new List<MoData>();
            moSpecs = new List<MoSpec>();
            moRoutings = new List<MoRouting>();
            allOrders = new List<AllOrderTracking>();
        }
        public List<MoData> moDatas { get; set; }
        public List<MoSpec> moSpecs { get; set; }
        public List<MoRouting> moRoutings { get; set; }
        public List<AllOrderTracking> allOrders { get; set; }
    }

    public class AllOrderTracking
    {
        public string FactoryCode { get; set; }
        public string OrderItem { get; set; }

    }
}
