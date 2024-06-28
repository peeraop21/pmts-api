using PMTs.DataAccess.ComplexModels;
using System.Threading.Tasks;

namespace PMTs.DataAccess.Email.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailRequest email);
    }
}
