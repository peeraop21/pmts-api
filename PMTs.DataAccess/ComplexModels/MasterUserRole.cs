using System;

namespace PMTs.DataAccess.ComplexModels
{
    public partial class MasterUserRole
    {
        public int Id { get; set; }
        public string SaleOrg { get; set; }
        public string Plant { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int? DefaultRoleId { get; set; }
        public string UserDomain { get; set; }
        public bool? IsLockedOut { get; set; }
        public bool? MustChangePassword { get; set; }
        public string FirstNameTh { get; set; }
        public string LastNameTh { get; set; }
        public string FirstNameEn { get; set; }
        public string LastNameEn { get; set; }
        public string Telephone { get; set; }
        public string MobileNo { get; set; }
        public string Email { get; set; }
        public string PasswordHint { get; set; }
        public string Comment { get; set; }
        public DateTime? LastPasswordChangeDate { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public DateTime? LastLogoutDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedBy { get; set; }
        public bool? IsReceiveMail { get; set; }
        public bool? IsFlagDelete { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }
    }
}
