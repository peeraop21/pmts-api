using Dapper;
using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class BoardCombindMaintainRepository : IBoardCombindMaintainRepository
    {
        //   public IDbConnection _db { get; set; }


        PMTsDbContext PMTsDbContext;
        public BoardCombindMaintainRepository(PMTsDbContext context)
        {
            PMTsDbContext = context;
        }

        public BoardCombindMaintainModel GetAllMaxcode(IConfiguration config)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            return db.Query<BoardCombindMaintainModel>("select Dummy_Code as MaxID FROM Board_Combind_DummyCode").FirstOrDefault();
        }

        public List<BoardCombind> GetAllBoardcombind(IConfiguration config)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            return db.Query<BoardCombind>("select * FROM Board_Combine").ToList();
        }

        public List<FluteTR> GetAllFluteByFactoryCode(IConfiguration config, string factory)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            return db.Query<FluteTR>("select *  FROM FluteTR where FactoryCode = '" + factory + "' ").ToList();
        }

        public List<PaperGrades> GetAllPaperGrade(IConfiguration config, string factory)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            return db.Query<PaperGrades>("select *  FROM PaperGrade").ToList();
        }

        public List<Option> GetDistinctFluteByFactoryCode(IConfiguration config, string factory)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            return db.Query<Option>("select DISTINCT FluteCode as value, FluteCode as text  FROM FluteTR where FactoryCode = '" + factory + "' ").ToList();
        }

        public List<BoardSpect> GetAllBoardSpect(IConfiguration config)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            return db.Query<BoardSpect>("select * FROM BoardSpec").ToList();
        }

        public List<BoardSpect> GetAllBoardSpectByCode(IConfiguration config, string code)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            return db.Query<BoardSpect>("select * FROM BoardSpec where Code = '" + code + "' order by Item ").ToList();
        }


        public int UpdateDummyCode(IConfiguration config, string Maxcode)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();
            //Execute sql query
            var query = db.Query<BoardCombindMaintainModel>("select Dummy_Code as MaxID FROM Board_Combind_DummyCode").FirstOrDefault();
            if (query == null)
            {
                return db.Execute("insert into Board_Combind_DummyCode (Id,Dummy_Code) values (1,'" + Maxcode + "')");
            }
            else
            {
                return db.Execute("update Board_Combind_DummyCode set  Dummy_Code = '" + Maxcode + "'  ");
            }
        }

        public bool checkUserWriteOrGeneratecode(IConfiguration config, string code)
        {
            var oldcode = GetAllMaxcode(config);
            string codenext = NextID(oldcode != null ? oldcode.MaxID : null);
            if (codenext == code)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static readonly char _minChar = 'A';
        private static readonly char _maxChar = 'Z';
        private static readonly int _minDigit = 1;
        private static readonly int _maxDigit = 999;
        private static int _fixedLength = 4;//zero means variable length
        private static int _currentDigit = 1;
        private static string _currentBase = "A";

        #region [001A - 999Z]
        public static string NextID()
        {
            if (_currentBase[_currentBase.Length - 1] <= _maxChar)
            {
                _currentDigit++;
                if (_currentDigit <= _maxDigit)
                {
                    var result = string.Empty;
                    if (_fixedLength > 0)
                    {
                        var prefixZeroCount = _fixedLength - _currentBase.Length;
                        if (prefixZeroCount < _currentDigit.ToString().Length)
                            throw new InvalidOperationException("The maximum length possible has been exeeded.");
                        result = result = _currentDigit.ToString("D" + prefixZeroCount.ToString()) + _currentBase;
                    }
                    else
                    {
                        result = _currentDigit.ToString() + _currentBase;
                    }

                    return result;
                }
                else
                {
                    _currentDigit = 0;
                    if (_currentBase[_currentBase.Length - 1] == _maxChar)
                    {
                        _currentBase = _currentBase.Remove(_currentBase.Length - 1) + _minChar;
                        _currentBase += _minChar.ToString();
                    }
                    else
                    {
                        var newChar = _currentBase[_currentBase.Length - 1];
                        newChar++;
                        _currentBase = _currentBase.Remove(_currentBase.Length - 1) + newChar.ToString();
                    }

                    return NextID();
                }
            }
            else
            {
                _currentDigit = _minDigit;
                _currentBase += _minChar.ToString();
                return NextID();

            }
        }

        public static string NextID(string currentId)
        {
            if (string.IsNullOrWhiteSpace(currentId))
            {
                _currentDigit = 0;
                return NextID();
            }
            else
            {

                var charCount = currentId.Length;
                var indexFound = -1;
                for (int i = 0; i < charCount; i++)
                {
                    if (char.IsNumber(currentId[i]))
                        continue;

                    indexFound = i;
                    break;
                }
                if (indexFound > -1)
                {
                    _currentBase = currentId.Substring(indexFound, 4 - indexFound);
                    _currentDigit = int.Parse(currentId.Substring(0, indexFound));
                }
                return NextID();
            }
        }
        #endregion


        public bool AddBoard(IConfiguration config, BoardCombindMaintainModel model)
        {
            model._BoardCombind.Code = model._BoardCombind.Code.ToUpper();
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    BoardCombine combind = new BoardCombine();
                    combind.Code = model._BoardCombind.Code;
                    combind.Flute = model._BoardCombind.Flute;
                    //if (model._BoardCombind.StandardBoard.ToString() == "False")
                    //{
                    //    string[] arrayboard = model._BoardCombind.Board.Split("/");
                    //    string boardtemp = "";
                    //    for (int i = 1; i < arrayboard.Length - 1; i++)
                    //    {
                    //        boardtemp = boardtemp + "/" + arrayboard[i];
                    //    }
                    //    combind.Board = boardtemp.Substring(1, boardtemp.Length - 1);
                    //    combind.BoardCombine1 = model._BoardCombind.Flute + "/" + combind.Board;
                    //}
                    //else
                    //{
                    //    string[] arrayboard = model._BoardCombind.Board.Split("/");
                    //    string boardtemp = "";
                    //    for(int i = 1; i<arrayboard.Length - 1;i++)
                    //    {
                    //        boardtemp = boardtemp + "/" + arrayboard[i];
                    //    }
                    //    combind.Board = boardtemp.Substring(1,boardtemp.Length -1);
                    //    combind.BoardCombine1 = model._BoardCombind.Flute + "/" + combind.Board;
                    //}
                    if (model._BoardCombind.Board.Length > 0)
                    {
                        combind.Board = model._BoardCombind.Board.Substring(model._BoardCombind.Board.IndexOf('/') + 1, (model._BoardCombind.Board.Length - model._BoardCombind.Board.IndexOf('/')) - 1);
                        combind.BoardCombine1 = model._BoardCombind.Board;
                    }


                    combind.Thickness = model._BoardCombind.Thickness == null ? 0 : Convert.ToDouble(model._BoardCombind.Thickness);
                    combind.StandardCost = model._BoardCombind.StandardCost;
                    combind.CorrControl = model._BoardCombind.CorrControl;
                    combind.Strength = model._BoardCombind.Strength;
                    combind.Ectstrength = model._BoardCombind.ECTStrength;
                    combind.Fctstrength = model._BoardCombind.FCTStrength;
                    combind.Burst = model._BoardCombind.Burst;
                    combind.StandardBoard = model._BoardCombind.StandardBoard;
                    combind.Status = model._BoardCombind.Status;
                    combind.Kiwi = model._BoardCombind.Kiwi;
                    combind.CreatedBy = model._BoardCombind.CreatedBy;
                    combind.Weight = model._BoardCombind.Weight;
                    combind.CreatedDate = DateTime.Now;
                    PMTsDbContext.BoardCombine.Add(combind);
                    PMTsDbContext.SaveChanges();


                    //if (PMTsDbContext.BoardSpec.Select(x => x.Code == model._BoardCombind.Code).Count() > 0)
                    //{
                    //    PMTsDbContext.BoardSpec.RemoveRange(PMTsDbContext.BoardSpec.Where(x => x.Code == model._BoardCombind.Code));
                    //    PMTsDbContext.SaveChanges();
                    //}
                    if (model._BoardCombind.StandardBoard.ToString() == "False")
                    {
                        if (PMTsDbContext.BoardSpec.Select(x => x.Code == model._BoardCombind.Code).Count() > 0)
                        {
                            PMTsDbContext.BoardSpec.RemoveRange(PMTsDbContext.BoardSpec.Where(x => x.Code == model._BoardCombind.Code));
                            PMTsDbContext.SaveChanges();
                        }

                        foreach (var item in model.BoardSpect)
                        {
                            BoardSpec BoardSpect = new BoardSpec
                            {
                                Code = model._BoardCombind.Code,
                                Grade = item.Grade == "" ? null : item.Grade,
                                Station = item.Station == "" ? null : item.Station,
                                Item = item.Item
                            };
                            PMTsDbContext.BoardSpec.Add(BoardSpect);
                        }
                        PMTsDbContext.SaveChanges();
                    }


                    dbContextTransaction.Commit();
                    if (checkUserWriteOrGeneratecode(config, model._BoardCombind.Code))
                    {
                        UpdateDummyCode(config, model._BoardCombind.Code);
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return false;
                }
            }

        }


        public bool UpdateBoard(IConfiguration config, BoardCombindMaintainModel model)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var it_trans_update = PMTsDbContext.BoardCombine.Where(IT => IT.Code == model._BoardCombind.Code).FirstOrDefault();
                    //  it_trans_update.Code = model._BoardCombind.Code;
                    // it_trans_update.Flute = model._BoardCombind.Flute;
                    if (model._BoardCombind.Board.Length > 0)
                    {
                        it_trans_update.Board = model._BoardCombind.Board.Substring(model._BoardCombind.Board.IndexOf('/') + 1, (model._BoardCombind.Board.Length - model._BoardCombind.Board.IndexOf('/')) - 1);
                        it_trans_update.BoardCombine1 = model._BoardCombind.Board;
                    }

                    //it_trans_update.Board = it_trans_update.Board.Substring(0, 1);

                    it_trans_update.Thickness = model._BoardCombind.Thickness == null ? 0 : Convert.ToDouble(model._BoardCombind.Thickness);
                    it_trans_update.StandardCost = model._BoardCombind.StandardCost;
                    it_trans_update.CorrControl = model._BoardCombind.CorrControl;
                    it_trans_update.Strength = model._BoardCombind.Strength;
                    it_trans_update.Ectstrength = model._BoardCombind.ECTStrength;
                    it_trans_update.Fctstrength = model._BoardCombind.FCTStrength;
                    it_trans_update.Burst = model._BoardCombind.Burst;
                    //it_trans_update.StandardBoard = model._BoardCombind.StandardBoard;
                    it_trans_update.Status = model._BoardCombind.Status;
                    it_trans_update.Kiwi = model._BoardCombind.Kiwi;
                    it_trans_update.Weight = model._BoardCombind.Weight;
                    it_trans_update.UpdatedBy = model._BoardCombind.UpdatedBy;
                    it_trans_update.UpdatedDate = DateTime.Now;

                    if (model._BoardCombind.StandardBoard.ToString() == "False")
                    {
                        if (PMTsDbContext.BoardSpec.Select(x => x.Code == model._BoardCombind.Code).Count() > 0)
                        {
                            PMTsDbContext.BoardSpec.RemoveRange(PMTsDbContext.BoardSpec.Where(x => x.Code == model._BoardCombind.Code));
                            PMTsDbContext.SaveChanges();
                        }

                        foreach (var item in model.BoardSpect)
                        {
                            BoardSpec BoardSpect = new BoardSpec
                            {
                                Code = model._BoardCombind.Code,
                                Grade = item.Grade == "" ? null : item.Grade,
                                Station = item.Station == "" ? null : item.Station,
                                Item = item.Item
                            };
                            PMTsDbContext.BoardSpec.Add(BoardSpect);
                        }

                        // PMTsDbContext.SaveChanges();
                    }

                    PMTsDbContext.SaveChanges();
                    dbContextTransaction.Commit();
                    // UpdateDummyCode(config , model._BoardCombind.Code);
                    return true;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    return false;
                }
            }

        }


    }
}
