using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;



namespace PMTs.DataAccess.Repositories
{
    public class SendEmailRepository : Repository<SendEmail>, ISendEmailRepository
    {
        private readonly PMTsDbContext _context;
        public SendEmailRepository(PMTsDbContext context) : base(context)
        {
            _context = context;
        }

        public List<string> GetEmailByFactoryCode(string factoryCode)
        {
            return _context.SendEmail.Where(w => w.FactoryCode == factoryCode).Select(s => s.Email).ToList();
        }
    }
}
