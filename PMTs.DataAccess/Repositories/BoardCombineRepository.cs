using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using PMTs.DataAccess.Tracing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace PMTs.DataAccess.Repositories
{
    public class BoardCombineRepository : Repository<BoardCombine>, IBoardCombineRepository
    {
        public BoardCombineRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public IEnumerable<BoardCombine> GetBoardCombineAll()
        {
            return PMTsDbContext.BoardCombine.ToList();
        }

        //ฟังก์ชั่นนี้ไม่ได้ใช้แล้ว
        //public List<BoardViewModel> GetBoard(string FactoryCode, string costField, string lv2, string lv3)
        //{
        //    return PMTsDbContext.BoardCombine.Join(PMTsDbContext.Flute.Where(f => f.FactoryCode == FactoryCode)
        //                                        , board => board.Flute, flute => flute.Flute1
        //                                        , (board, flute) => new { Board = board, Flutex = flute })
        //                                     .GroupJoin(PMTsDbContext.BoardCost.Where(cost => cost.CostField == costField && cost.FactoryCode == FactoryCode)
        //                                        , a => a.Board.Code, cost => cost.BoardCode
        //                                        , (a, cost) => new { a.Board, a.Flutex, cost })
        //                                        .SelectMany(x => x.cost.DefaultIfEmpty(), (b, y) => new BoardViewModel
        //                                        {
        //                                            Code = b.Board.Code,
        //                                            Flute = b.Board.Flute,
        //                                            Board = b.Board.Board,
        //                                            SearchBoard = b.Board.Code + " " + b.Board.Flute + " " + b.Board.Board,
        //                                            BoardKiwi = b.Board.Kiwi,
        //                                            Weight = b.Board.Weight,
        //                                            A = b.Flutex.A,
        //                                            B = b.Flutex.B,
        //                                            C = b.Flutex.C,
        //                                            D1 = b.Flutex.D1,
        //                                            D2 = b.Flutex.D2,
        //                                            JoinSize = b.Flutex.JoinSize,
        //                                            CostPerTon = y.CostPerTon,
        //                                            Hierarchy = "03" + lv2 + lv3 + "999" + b.Board.Code + "0000"
        //                                        }).OrderBy(a => a.Code).AsNoTracking().ToList();
        //}

        public List<SearchBoardAlt> GetBoardSearch()
        {
            var rootActivity = ActivitySourceProvider.Source!.StartActivity($"{nameof(GetBoardSearch)} Start");

            var flu = PMTsDbContext.Flute.GroupBy(f => f.Flute1).Select(f => new { Flute = f.Key, Height = f.Max(a => a.Height), JoinSize = f.Max(a => a.JoinSize) });

            var result = PMTsDbContext.BoardCombine.Where(b => b.Status == true).Join(flu, b => b.Flute, f => f.Flute, (b, f) => new { b, f }).Select(x => new SearchBoardAlt
            {
                Code = x.b.Code,
                Flute = x.b.Flute,
                Board = x.b.Board,
                BoardCombine1 = x.b.BoardCombine1,
                Kiwi = x.b.Kiwi,
                Thickness = x.b.Thickness,
                SearchBoard = x.b.Code + " " + x.b.Flute + " " + x.b.Board,
                Height = x.f.Height,
                JoinSize = x.f.JoinSize
            }).ToList();
            rootActivity.Stop();
            return result;
            //return PMTsDbContext.BoardCombine.Select(b => new SearchBoardAlt
            //{
            //    Code = b.Code,
            //    Flute = b.Flute,
            //    Board = b.Board,
            //    BoardCombine1 = b.BoardCombine1,
            //    Kiwi = b.Kiwi,
            //    Thickness = b.Thickness,
            //    SearchBoard = b.Code + " " + b.Flute + " " + b.Board
            //}).ToList();
        }

        public BoardCombine GetBoardByCode(string code)
        {
            return PMTsDbContext.BoardCombine.Where(b => b.Code == code).FirstOrDefault();
        }

        public IEnumerable<BoardCombine> GetBoardByFlute(string flute)
        {
            return PMTsDbContext.BoardCombine.Where(b => b.Flute == flute && b.Status == true);
        }

        public BoardCombine GetBoardByBoard(string board, string flute)
        {
            return PMTsDbContext.BoardCombine
                .Where(b => b.Board == board && b.Flute == flute && b.Status == true)
                .OrderByDescending(p => p.UpdatedDate)
                .ThenByDescending(o => o.CreatedDate)
                .FirstOrDefault();
        }
        public List<BoardCombine> GetBoardsByBoard(string board, string flute)
        {
            return PMTsDbContext.BoardCombine
                .Where(b => b.Board == board && b.Flute == flute && b.Status == true)
                .OrderByDescending(p => p.UpdatedDate)
                .ThenByDescending(o => o.CreatedDate)
                .ToList();
        }
        public List<BoardSpecWeight> GetBoardSpecWeightByCode(string FactoryCode, string code)
        {
            var board = GetBoardByCode(code);
            var boardSpecs = new List<BoardSpecWeight>();

            //var fluTr = PMTsDbContext.FluteTr.Where(f => f.FactoryCode == FactoryCode && f.FluteCode == board.Flute)
            //                                 .Select((j, i) => new { j.FactoryCode, j.FluteCode, rn = i + 1, j.Station, j.Tr });

            if (board != null)
            {
                string[] ArrBoard = board.Board.Split("/");

                var join = ArrBoard.GroupJoin(PMTsDbContext.PaperGrade
                                                     , spec => spec.Trim().ToString(), pp => pp.Grade
                                                     , (spec, pp) => new { spec, pp })
                                                     .SelectMany(x => x.pp.DefaultIfEmpty(), (b, p) => new { b, p });

                foreach (var item in join)
                {
                    if (item.b.spec != "")
                    {
                        var boardSpec = new BoardSpecWeight();

                        if (item.p == null)
                        {
                            if (item.b.spec.Trim().ToString().Length == 5)
                            {
                                boardSpec.BasicWeight = Convert.ToInt32(item.b.spec.ToString().Substring(2, 3));
                            }
                            else if (item.b.spec.Trim().ToString().Length == 6)
                            {
                                boardSpec.BasicWeight = Convert.ToInt32(item.b.spec.ToString().Substring(3, 3));
                            }
                            else if (item.b.spec.Trim().ToString().Length == 4)
                            {
                                boardSpec.BasicWeight = Convert.ToInt32(item.b.spec.ToString().Substring(2, 2));
                            }
                        }
                        else
                        {
                            boardSpec.BasicWeight = item.p.BasicWeight;
                        }
                        boardSpec.Grade = item.b.spec.ToString();
                        boardSpec.PaperDes = item.p == null ? boardSpec.Grade : item.p.PaperDes;
                        boardSpec.Layer = item.p == null ? 1 : item.p.Layer;
                        boardSpecs.Add(boardSpec);
                    }
                }

            }

            return boardSpecs;
        }

        public List<BoardViewModel> GetBoard(string factoryCode, string costField, string lv2, string lv3)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<BoardCombine> GetBoardsByCodes(string factoryCode, List<string> codes)
        {
            var boardCombines = new List<BoardCombine>();
            boardCombines.AddRange(PMTsDbContext.BoardCombine.Where(m => codes.Contains(m.Code)).AsNoTracking().ToList());
            return boardCombines;
        }
        public string GenerateCode()
        {
            string pattern = @"^[A-Z]\d{3}$";
            RegexOptions options = RegexOptions.Multiline;
            Regex regex = new Regex(pattern, options);
            var lastBoardCombineCode = PMTsDbContext.BoardCombine.AsEnumerable().Where(w => regex.IsMatch(w.Code)).Select(s => s.Code).OrderByDescending(o => o).FirstOrDefault();
            if (!string.IsNullOrEmpty(lastBoardCombineCode))
            {
                try
                {
                    char letter = lastBoardCombineCode[0];
                    int number = int.Parse(lastBoardCombineCode.Substring(1));
                    number = number + 1;
                    if (number > 999)
                    {
                        number = 1;
                        letter = (char)(letter + 1);
                    }
                    string result = $"{letter}{number:D3}";
                    return result;
                }
                catch
                {
                    return string.Empty;
                }

            }
            else
            {
                return string.Empty;
            }
        }

        public ExportDataForSAPResponse GenerateDataForSAP(ExportDataForSAPRequest request)
        {
            ExportDataForSAPResponse result = new ExportDataForSAPResponse();
            result.Items = new List<ExportDataForSAPItem>();
            var data = PMTsDbContext.MapCost.Where(w => request.HirerarchyLv2Codes.Contains(w.Hierarchy2) && w.Active == true).ToList();
            for (int i = 0; i < data.Count(); i++)
            {
                ExportDataForSAPItem item = new ExportDataForSAPItem();
                item.Code = "503" + data[i].Hierarchy2.Trim() + data[i].Hierarchy3.Trim() + data[i].Hierarchy4.Trim() + request.BoardCode.Trim();
                item.Board = request.Board.Trim();
                result.Items.Add(item);
            }

            return result;

        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
