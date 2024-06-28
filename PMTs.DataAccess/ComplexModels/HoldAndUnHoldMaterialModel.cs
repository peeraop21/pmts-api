namespace PMTs.DataAccess.ComplexModels
{
    public class HoldAndUnHoldMaterialModel
    {
        public HoldAndUnHoldMaterialRequestModel request { get; set; }
        public HoldAndUnHoldMaterialResponseModel response { get; set; }
    }
}


public class HoldAndUnHoldMaterialRequestModel
{
    public string MaterialNo { get; set; }
    public string HoldRemark { get; set; }
    public string ChangeProductNo { get; set; }
    public string User { get; set; }
}

public class HoldAndUnHoldMaterialResponseModel
{
    public string MaterialNo { get; set; }
    public string Pc { get; set; }
    public string Description { get; set; }
}