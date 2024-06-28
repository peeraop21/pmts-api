
using System;

namespace PMTs.DataAccess.ComplexModels
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string FactoryCode { get; set; }
        public string SaleOrg { get; set; }
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
        public int? NumberOfLogins { get; set; }
        public string AppName { get; set; }
        public string Token { get; set; }
        public string PictureUser { get; set; }

    }
}
