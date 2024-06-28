using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class BoardAlternativeRepository(PMTsDbContext context) : Repository<BoardAlternative>(context), IBoardAlternativeRepository
    {
        public IEnumerable<BoardAlternative> GetByMat(string factoryCode, string mat)
        {
            return PMTsDbContext.BoardAlternative.Where(b => b.MaterialNo == mat && b.FactoryCode == factoryCode).ToList();
        }

        public IEnumerable<BoardAlternative> GetBoardAlternativesByMaterialNos(string factoryCode, List<string> materialNos)
        {
            var boardAlternatives = new List<BoardAlternative>();
            boardAlternatives.AddRange(PMTsDbContext.BoardAlternative.Where(b => b.FactoryCode == factoryCode && materialNos.Contains(b.MaterialNo)).AsNoTracking().ToList());

            return boardAlternatives;
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
