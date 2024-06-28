using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class BomStructRepository(PMTsDbContext context) : Repository<BomStruct>(context), IBomStructRepository
    {
        private readonly PMTsDbContext _context = context;

        public BomStruct GetBomStructById(string factoryCode, int Id)
        {

            return _context.BomStruct.Where(x => x.Id == Id && x.FactoryCode == factoryCode && x.PdisStatus != "X").FirstOrDefault();
        }

        public IEnumerable<BomStruct> SearchBomStructsByMaterialNo(string factoryCode, string materialNo)
        {

            return _context.BomStruct.Where(x => x.MaterialNo == materialNo && x.FactoryCode == factoryCode && x.PdisStatus != "X").OrderBy(x => x.PreviousBom).ToList();
        }

        public IEnumerable<BomStruct> GetBomStructByhandshake(SqlConnection conn, string factoryCode, string materialNo)
        {
            DataTable dt = new DataTable();
            string sqlQuery = @$"	 
                                    select  
	                                    b.PDIS_Status,
	                                    b.Material_No,
	                                    b.Plant,
	                                    b.Follower, 
	                                    b.FactoryCode ,
	                                    isnull(b.Bom_Usage, '') as Bom_Usage,
	                                    isnull(b.Weigh_Bom, '') as Weigh_Bom, 
	                                    isnull(Previous_Bom, '') as Previous_Bom ,
	                                    isnull(b.Amount, '') as Amount,
	                                    isnull(b.Unit, '') as Unit ,
	                                    m.PDIS_Status as MasterPDIS_Status
                                    from Bom_Struct b
                                    inner join MasterData m on b.Material_No = m.Material_No  AND b.FactoryCode = m.FactoryCode 
                                    where 1=1
                                    --and b.FactoryCode = 'FactoryCode' 
                                    and b.Material_No in ({materialNo})
                                    and b.Sap_status <> '1' 
                                    order by m.Material_No,b.id asc
                                ";

            using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            //  List<SalesView> sales = dt.Rows.();
            var bomstruct = (from DataRow row in dt.Rows
                             select new BomStruct
                             {
                                 FactoryCode = row["FactoryCode"].ToString(),
                                 MaterialNo = row["Material_No"].ToString(),
                                 Plant = row["plant"].ToString(),
                                 BomUsage = row["Bom_Usage"].ToString(),
                                 WeighBom = Convert.ToInt64(Convert.ToDouble(row["Weigh_Bom"])),
                                 PreviousBom = row["Previous_Bom"] == null ? 0 : Convert.ToInt32(row["Previous_Bom"]),
                                 Follower = row["Follower"].ToString(),
                                 Amount = row["Amount"] == null ? 0 : Convert.ToInt32(row["Amount"]),
                                 Unit = row["Unit"].ToString(),
                                 PdisStatus = row["PDIS_Status"].ToString()
                                 //PdisStatus = row["PDIS_Status"].ToString() != "X" ? row["MasterPDIS_Status"].ToString() : row["PDIS_Status"].ToString()
                             }).ToList();

            return bomstruct;
        }

        public BomStruct SearchBomStructsByFollower(string factoryCode, string follower)
        {
            return _context.BomStruct.Where(x => x.Follower == follower && x.FactoryCode == factoryCode && x.PdisStatus != "X").FirstOrDefault();
        }
        public BomStruct GetBomStructsByFollowerMasterDataNonX(string factoryCode, string follower)
        {
            //var res = new BomStruct();
            var bom = _context.BomStruct.Where(x => x.Follower == follower && x.FactoryCode == factoryCode && x.PdisStatus != "X").ToList();
            if (bom.Count > 0)
            {
                var listMat = bom.Select(s => s.MaterialNo).ToList();
                var master = _context.MasterData.FirstOrDefault(p => p.FactoryCode == factoryCode && listMat.Contains(p.MaterialNo) && p.PdisStatus != "X");
                if (master != null)
                {
                    return bom.FirstOrDefault(p => p.MaterialNo == master.MaterialNo);
                }
                else
                {
                    return bom.FirstOrDefault();
                }
            }
            else
            {
                return bom.FirstOrDefault();
            }
        }

        public void UpdateBomstructPreviousFields(BomStruct model)
        {
            var bom = _context.BomStruct.Where(x => x.MaterialNo == model.MaterialNo && x.FactoryCode == model.FactoryCode).OrderBy(x => x.Id).ToList();
            if (bom != null)
            {
                int? cc = 0;
                foreach (var item in bom)
                {
                    var someSaleView = _context.BomStruct.Where(s => s.Id == item.Id && s.FactoryCode == item.FactoryCode).ToList();
                    someSaleView.ForEach(a => a.PreviousBom = cc);
                    var response = _context.SaveChanges();
                    // dbContextTransaction.Commit();
                    cc++;
                }
            }

        }


        public void UpdateSaptatus(SqlConnection conn, string FactoryCode, string MaterialNo, bool Status)
        {

            var sqlupdate = @"UPDATE Bom_Struct SET SAP_Status = 1 WHERE FactoryCode = '" + FactoryCode + "' and Material_No in (" + MaterialNo + ") ";
            using (SqlCommand cmd = new SqlCommand(sqlupdate, conn))
            {
                cmd.CommandText = sqlupdate;
                conn.Open();
                cmd.ExecuteNonQuery();
                conn.Close();
            }


            //using (var dbContextTransaction = _context.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        //var some = PMTsDbContext.MasterData.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo && s.PdisStatus != "X").ToList();

            //        //var some = PMTsDbContext.MasterData.Where(s => s.FactoryCode == FactoryCode && s.MaterialNo == MaterialNo).ToList();

            //        var some = _context.BomStruct.Where(s => s.FactoryCode == FactoryCode && MaterialNo.Contains(s.MaterialNo)).ToList();
            //        //some.ForEach(a => a.TranStatus = Status);
            //        some.ForEach(a => a.SapStatus = Status);
            //        var response = _context.SaveChanges();
            //        dbContextTransaction.Commit();
            //    }
            //    catch (Exception ex)
            //    {
            //        dbContextTransaction.Rollback();
            //        throw ex;
            //    }
            //}

        }


        public void UpdateBomstructSapstatus(BomStruct model)
        {


            //tassanai update 21/07/2020 


            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {


                    var bom = _context.BomStruct.Where(s => s.FactoryCode == model.FactoryCode && model.MaterialNo.Contains(s.MaterialNo)).ToList();
                    //some.ForEach(a => a.TranStatus = Status);
                    bom.ForEach(a => a.SapStatus = false);
                    var response = _context.SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }

            //var bom = _context.BomStruct.Where(x => x.MaterialNo == model.Follower && x.FactoryCode == model.FactoryCode).FirstOrDefault();
            //if (bom != null)
            //{

            //    bom.SapStatus = false;
            //    var response = _context.SaveChanges();
            //    // dbContextTransaction.Commit();
            //}

        }

        public void UpdateMasterdataSapstatusForBomstruct(BomStruct model) // เปลี่ยน ความหมายคือการ update PDIS ของ masterdata แทน  // tassanai update 21/07/2020
        {
            var masterdata = _context.MasterData.Where(x => x.MaterialNo == model.MaterialNo && x.FactoryCode == model.FactoryCode).FirstOrDefault();
            if (masterdata != null)
            {


                if (masterdata.PdisStatus == "C" && masterdata.SapStatus == false)
                {
                    masterdata.PdisStatus = "C";
                }
                else if (masterdata.PdisStatus == "C" && masterdata.SapStatus == true)
                {
                    masterdata.PdisStatus = "M";
                }


                //masterdata.SapStatus = false;
                var response = _context.SaveChanges();
                // dbContextTransaction.Commit();
            }

        }

        public void CopyBomstructToNewPlant(string parentmat, string newfactory, string oldfactory, string username)
        {
            var tempOldData = _context.BomStruct.Where(x => x.MaterialNo == parentmat && x.FactoryCode == oldfactory).ToList();
            using (var dbContextTransaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (var item in tempOldData)
                    {
                        var checkdata = _context.BomStruct.Where(x => x.MaterialNo == parentmat && x.FactoryCode == newfactory && x.Follower == item.Follower).FirstOrDefault();
                        if (checkdata == null)
                        {
                            BomStruct model = new BomStruct();
                            model.Amount = item.Amount;
                            model.BomUsage = item.BomUsage;
                            model.FactoryCode = newfactory;
                            model.Follower = item.Follower;
                            model.MaterialNo = item.MaterialNo;
                            model.PdisStatus = item.PdisStatus;
                            model.Plant = newfactory;
                            model.PreviousBom = item.PreviousBom;
                            model.SapStatus = item.SapStatus;
                            model.TranStatus = item.TranStatus;
                            model.Unit = item.Unit;
                            model.WeighBom = item.WeighBom;
                            model.CreatedBy = username;
                            model.CreatedDate = DateTime.Now;

                            //model = item;
                            //model.Id = 0;
                            //model.FactoryCode = newfactory;
                            _context.BomStruct.Add(model);
                            _context.SaveChanges();
                        }

                    }

                    dbContextTransaction.Commit();
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }

        }

        public IEnumerable<BomStruct> GetBomstructsByMaterialNos(string factoryCode, List<string> materialNos)
        {
            var bomStructs = new List<BomStruct>();

            if (materialNos != null && materialNos.Count > 0)
            {
                bomStructs.AddRange(_context.BomStruct.Where(b => materialNos.Contains(b.MaterialNo) && b.FactoryCode == factoryCode).AsNoTracking().ToList());
            }

            return bomStructs;
        }
    }
}
