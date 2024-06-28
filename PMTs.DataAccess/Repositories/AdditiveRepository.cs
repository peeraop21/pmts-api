using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class AdditiveRepository : Repository<Additive>, IAdditiveRepository
    {

        private readonly PMTsDbContext _context;
        public AdditiveRepository(PMTsDbContext context) : base(context)
        {
            _context = context;
        }

        public bool AddAdditive(Additive model)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    Additive additive = new Additive();
                    additive.Type = model.Type;
                    additive.Code = model.Code;
                    additive.Description = model.Description;
                    additive.ConsumptionRate = model.ConsumptionRate;
                    additive.GlueSolidContent = model.GlueSolidContent;
                    additive.Remark = model.Remark;
                    additive.Status = model.Status;
                    additive.CreatedBy = model.CreatedBy;
                    additive.CreatedDate = DateTime.Now;

                    _context.Additive.Add(additive);
                    _context.SaveChanges();

                    dbContextTransaction.Commit();
                    return true;
                }
                catch
                {
                    dbContextTransaction.Rollback();
                    return false;
                }
            }

        }


        public bool UpdateAdditive(Additive model)
        {
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var it_trans_update = _context.Additive.Where(IT => IT.Id == model.Id).FirstOrDefault();
                    it_trans_update.Type = model.Type;
                    it_trans_update.Code = model.Code;
                    it_trans_update.Description = model.Description;
                    it_trans_update.ConsumptionRate = model.ConsumptionRate;
                    it_trans_update.GlueSolidContent = model.GlueSolidContent;
                    it_trans_update.Remark = model.Remark;
                    it_trans_update.Status = model.Status;
                    it_trans_update.UpdatedBy = model.UpdatedBy;
                    it_trans_update.UpdatedDate = DateTime.Now;
                    _context.SaveChanges();
                    dbContextTransaction.Commit();
                    return true;
                }
                catch
                {
                    dbContextTransaction.Rollback();
                    return false;
                }
            }

        }
    }
}
