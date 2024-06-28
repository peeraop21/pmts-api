namespace PMTs.DataAccess.ComplexModels
{
    public partial class MainMenuAllByRoles
    {
        public int Id { get; set; }
        public string MenuNameEn { get; set; }
        public string MenuNameTh { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string IconName { get; set; }
        public int? SortMenu { get; set; }
        public int? RoleId { get; set; }
        public int? IdmenuRole { get; set; }
    }
}
