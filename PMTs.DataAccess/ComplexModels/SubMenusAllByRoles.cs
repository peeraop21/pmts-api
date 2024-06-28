namespace PMTs.DataAccess.ComplexModels
{
    public partial class SubMenusAllByRoles
    {
        public int Id { get; set; }
        public string SubMenuName { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public int MainMenuId { get; set; }
        public int? SubMenuroleID { get; set; } // SubMenurole.Id
        public int? IdSubMenuRole { get; set; }
        public int? Idrole { get; set; }
        public int? Idmenu { get; set; }


    }
}
