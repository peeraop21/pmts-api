using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class BoardUseRepository : Repository<BoardUse>, IBoardUseRepository
    {
        public BoardUseRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public BoardUse GetBoardUseByMaterialNo(string factoryCode, string materialNo)
        {
            return PMTsDbContext.BoardUse.Where(b => b.MaterialNo == materialNo && b.FactoryCode == factoryCode).FirstOrDefault();
        }

        public IEnumerable<string> GetPaperItemByMaterialNo(string factoryCode, string materialNo)
        {
            var BoardUse = PMTsDbContext.BoardUse.Where(b => b.MaterialNo == materialNo && b.FactoryCode == factoryCode).FirstOrDefault();
            if (BoardUse == null)
            {
                return null;
            }

            List<string> result = new List<string>();
            if (BoardUse.Gl != null)
            {
                result.Add(BoardUse.Gl);
            }
            if (BoardUse.Bm != null)
            {
                result.Add(BoardUse.Bm);
            }
            if (BoardUse.Bl != null)
            {
                result.Add(BoardUse.Bl);
            }
            if (BoardUse.Cm != null)
            {
                result.Add(BoardUse.Cm);
            }
            if (BoardUse.Cl != null)
            {
                result.Add(BoardUse.Cl);
            }
            if (BoardUse.Dm != null)
            {
                result.Add(BoardUse.Dm);
            }
            if (BoardUse.Dl != null)
            {
                result.Add(BoardUse.Dl);
            }

            return result;
        }

        public IEnumerable<BoardUse> GetBoardUsesByMaterialNos(string factoryCode, List<string> materialNos)
        {
            var boardUses = new List<BoardUse>();
            if (materialNos != null && materialNos.Count > 0)
            {
                boardUses.AddRange(PMTsDbContext.BoardUse.Where(b => materialNos.Contains(b.MaterialNo) && b.FactoryCode == factoryCode).AsNoTracking().ToList());
            }

            return boardUses;
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
