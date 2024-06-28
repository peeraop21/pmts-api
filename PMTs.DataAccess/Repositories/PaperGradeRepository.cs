using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class PaperGradeRepository : Repository<PaperGrade>, IPaperGradeRepository
    {
        public PaperGradeRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public List<PaperGrade> GetPaperGradeAll()
        {
            return PMTsDbContext.PaperGrade.ToList();
        }

        public PaperGrade GetPaperGradeByGradeAndActive(string grade)
        {
            return PMTsDbContext.PaperGrade.Where(w => w.Grade == grade && w.Active == true).FirstOrDefault();
        }

        public IEnumerable<PaperGradeViewModel> GetPaperGradesWithGradeCodeMachine(IConfiguration configuration, string factoryCode)
        {
            using IDbConnection db = new SqlConnection(configuration.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();

            string sql = @"
                    SELECT PaperGrade.Id, 
                        PaperGrade.Paper, 
                        PaperGrade.BasicWeight, 
                        PaperGrade.Liners, 
                        PaperGrade.Medium, 
                        PaperGrade.MaxPaperWidth, 
                        PaperGrade.Cost, 
                        PaperGrade.[Group], 
                        PaperGrade.KIWI, 
                        PaperGrade.BSH, 
                        PaperGrade.Grade, 
                        PaperGrade.Paper_Id as PaperId, 
                        PaperGrade.Paper_Des as PaperDes, 
                        PaperGrade.Layer, 
                        PaperGrade.Stang, 
                        PaperGrade.Length, 
                        PaperGrade.Active, 
                        PaperGrade.CreatedDate, 
                        PaperGrade.CreatedBy, 
                        PaperGrade.UpdatedDate, 
                        PaperGrade.UpdatedBy, 
                        PaperGradeCodeMachine.GradeCodeMachine
                        FROM     PaperGrade 
                        LEFT OUTER JOIN PaperGradeCodeMachine 
                        ON PaperGrade.Grade = PaperGradeCodeMachine.Grade AND PaperGradeCodeMachine.FactoryCode = '{0}'";

            string message = string.Format(sql, factoryCode);

            return db.Query<PaperGradeViewModel>(message).ToList();

            //return PMTsDbContext.PaperGrade.Where(w => w.Grade == grade && w.Active == true).FirstOrDefault();
        }

        public void SavePaperGradeWithGradeCodeMachine(string factoryCode, PaperGradeViewModel paperGradeViewModel)
        {
            var paperGradeCodeMachine = PMTsDbContext.PaperGradeCodeMachine.Where(p => p.Grade == paperGradeViewModel.Grade && p.FactoryCode == factoryCode).AsNoTracking().FirstOrDefault();
            var paperGrede = new PaperGrade
            {
                Id = 0,
                Active = paperGradeViewModel.Active,
                BasicWeight = paperGradeViewModel.BasicWeight,
                Bsh = paperGradeViewModel.Bsh,
                Cost = paperGradeViewModel.Cost,
                CreatedBy = paperGradeViewModel.CreatedBy,
                CreatedDate = paperGradeViewModel.CreatedDate,
                Grade = paperGradeViewModel.Grade,
                Group = paperGradeViewModel.Group,
                Kiwi = paperGradeViewModel.Kiwi,
                Layer = paperGradeViewModel.Layer,
                Length = paperGradeViewModel.Length,
                Liners = paperGradeViewModel.Liners,
                MaxPaperWidth = paperGradeViewModel.MaxPaperWidth,
                Medium = paperGradeViewModel.Medium,
                Paper = paperGradeViewModel.Paper,
                PaperDes = paperGradeViewModel.PaperDes,
                PaperId = paperGradeViewModel.PaperId,
                Stang = paperGradeViewModel.Stang,
                UpdatedBy = paperGradeViewModel.UpdatedBy,
                UpdatedDate = paperGradeViewModel.UpdatedDate,
            };

            if (paperGradeCodeMachine != null)
            {
                paperGradeCodeMachine.GradeCodeMachine = paperGradeViewModel.GradeCodeMachine;
                PMTsDbContext.PaperGradeCodeMachine.Update(paperGradeCodeMachine);
            }
            else
            {
                PMTsDbContext.PaperGradeCodeMachine.Add(new PaperGradeCodeMachine
                {
                    FactoryCode = factoryCode,
                    Grade = paperGradeViewModel.Grade,
                    GradeCodeMachine = paperGradeViewModel.GradeCodeMachine
                });
            }
            PMTsDbContext.PaperGrade.Add(paperGrede);
            PMTsDbContext.SaveChanges();
        }

        public void UpdatePaperGradeWithGradeCodeMachine(string factoryCode, PaperGradeViewModel paperGradeViewModel)
        {
            var paperGradeCodeMachine = PMTsDbContext.PaperGradeCodeMachine.FirstOrDefault(p => p.Grade == paperGradeViewModel.Grade && p.FactoryCode == factoryCode);
            var paperGrede = new PaperGrade
            {
                Id = paperGradeViewModel.Id,
                Active = paperGradeViewModel.Active,
                BasicWeight = paperGradeViewModel.BasicWeight,
                Bsh = paperGradeViewModel.Bsh,
                Cost = paperGradeViewModel.Cost,
                CreatedBy = paperGradeViewModel.CreatedBy,
                CreatedDate = paperGradeViewModel.CreatedDate,
                Grade = paperGradeViewModel.Grade,
                Group = paperGradeViewModel.Group,
                Kiwi = paperGradeViewModel.Kiwi,
                Layer = paperGradeViewModel.Layer,
                Length = paperGradeViewModel.Length,
                Liners = paperGradeViewModel.Liners,
                MaxPaperWidth = paperGradeViewModel.MaxPaperWidth,
                Medium = paperGradeViewModel.Medium,
                Paper = paperGradeViewModel.Paper,
                PaperDes = paperGradeViewModel.PaperDes,
                PaperId = paperGradeViewModel.PaperId,
                Stang = paperGradeViewModel.Stang,
                UpdatedBy = paperGradeViewModel.UpdatedBy,
                UpdatedDate = paperGradeViewModel.UpdatedDate,
            };
            if (paperGradeCodeMachine != null)
            {
                paperGradeCodeMachine.GradeCodeMachine = paperGradeViewModel.GradeCodeMachine;
                PMTsDbContext.PaperGradeCodeMachine.Update(paperGradeCodeMachine);
            }
            else
            {
                PMTsDbContext.PaperGradeCodeMachine.Add(new PaperGradeCodeMachine
                {
                    FactoryCode = factoryCode,
                    Grade = paperGradeViewModel.Grade,
                    GradeCodeMachine = paperGradeViewModel.GradeCodeMachine
                });
            }
            PMTsDbContext.PaperGrade.Update(paperGrede);
            PMTsDbContext.SaveChanges();

        }

        public IEnumerable<PaperGrade> GetAllPaperGrades(IConfiguration config)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();

            string sql = @"
                    SELECT PaperGrade.Id, 
                        PaperGrade.Paper, 
                        PaperGrade.BasicWeight, 
                        PaperGrade.Liners, 
                        PaperGrade.Medium, 
                        PaperGrade.MaxPaperWidth, 
                        PaperGrade.Cost, 
                        PaperGrade.[Group], 
                        PaperGrade.KIWI, 
                        PaperGrade.BSH, 
                        PaperGrade.Grade, 
                        PaperGrade.Paper_Id, 
                        PaperGrade.Paper_Des, 
                        PaperGrade.Layer, 
                        PaperGrade.Stang, 
                        PaperGrade.Length, 
                        PaperGrade.Active, 
                        PaperGrade.CreatedDate, 
                        PaperGrade.CreatedBy, 
                        PaperGrade.UpdatedDate, 
                        PaperGrade.UpdatedBy, 
                        PaperGradeCodeMachine.GradeCodeMachine
                        FROM     PaperGrade 
                        LEFT OUTER JOIN PaperGradeCodeMachine 
                        ON PaperGrade.Grade = PaperGradeCodeMachine.Grade";

            string message = string.Format(sql);

            return db.Query<PaperGrade>(message).ToList();
        }

        public List<string> GetGrades()
        {
            return PMTsDbContext.PaperGrade.Where(w => w.Active == true).Select(s => s.Grade).Distinct().ToList();
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
