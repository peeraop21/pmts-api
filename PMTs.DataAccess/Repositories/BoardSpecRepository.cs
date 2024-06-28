using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class BoardSpecRepository : Repository<BoardSpec>, IBoardSpecRepository
    {
        public BoardSpecRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public IEnumerable<BoardSpec> GetBoardSpecAll()
        {
            return PMTsDbContext.BoardSpec.ToList();
        }

        public List<BoardSpec> GetBoardSpecByBoardId(string boardId)
        {
            return PMTsDbContext.BoardSpec.Where(spec => spec.Code == boardId).ToList();
        }

        public List<BoardSpecStation> GetBoardSpecStationByBoardId(string factoryCode, string boardId)
        {
            // return PMTsDbContext.BoardSpec.Where(spec => spec.BoardId == boardId).ToList();
            var fluTr = PMTsDbContext.FluteTr.Where(f => f.FactoryCode == factoryCode).GroupBy(f => f.FluteCode)
                                             .SelectMany(f => f.Select((j, i) => new { j.FactoryCode, j.FluteCode, rn = i + 1, j.Station, j.Tr }));

            return PMTsDbContext.BoardSpec.Join(PMTsDbContext.BoardCombine.Where(b => b.Code == boardId)
                                                , spec => spec.Code, comb => comb.Code
                                                , (spec, comb) => new { spec, comb })
                                             .Join(fluTr, a => a.comb.Flute, flu => flu.FluteCode
                                                , (a, flu) => new { a.comb, a.spec, flu }).Where(z => z.spec.Item == z.flu.rn)
                                                .Select(x => new BoardSpecStation
                                                {
                                                    BoardId = x.spec.Code,
                                                    Item = x.spec.Item,
                                                    //PaperId = x.spec.PaperId,
                                                    //PaperDes = x.spec.PaperDes,
                                                    //Weight = x.spec.Weight,
                                                    Station = x.flu.Station,
                                                    Tr = x.flu.Tr
                                                }).ToList();

        }

        public IEnumerable<BoardSpec> GetBoardSpecsByCodes(List<string> codes)
        {
            var boardSpecs = new List<BoardSpec>();
            boardSpecs.AddRange(PMTsDbContext.BoardSpec.Where(m => codes.Contains(m.Code)).AsNoTracking().ToList());
            return boardSpecs;
        }

        //public List<BoardSpecWeight> GetBoardSpecWeightByBoardId(string factoryCode, string boardId)
        //{
        //    return PMTsDbContext.BoardSpec.Where(b => b.Code == boardId)
        //                                        .Join(PMTsDbContext.PaperGrade
        //                                        , spec => spec.Grade, pp => pp.PaperId
        //                                        , (spec, pp) => new { spec, pp })
        //                                        .Select(x => new BoardSpecWeight
        //                                        {
        //                                            //BoardId = x.spec.BoardId,
        //                                            //Item = x.spec.Item,
        //                                            //PaperId = x.spec.PaperId,
        //                                            BasicWeight = x.pp.BasicWeight,
        //                                            Grade = x.pp.Grade,
        //                                            PaperDes = x.pp.PaperDes,
        //                                            Layer = x.pp.Layer
        //                                        }).ToList();
        //}

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
