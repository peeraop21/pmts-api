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
    public class FluteRepository : Repository<Flute>, IFluteRepository
    {
        public FluteRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public List<string> GetFlutesList(string factoryCode)
        {
            return PMTsDbContext.Flute.Where(w => w.FactoryCode == factoryCode).Select(s => s.Flute1).ToList();
        }

        public Flute GetFluteByFlute(string factoryCode, string flute)
        {
            return PMTsDbContext.Flute.Where(f => f.Flute1 == flute && f.FactoryCode == factoryCode).FirstOrDefault();
        }

        #region [MaintainFlute]
        public FluteMaintainModel GetMaintainFlute(string factoryCode)
        {
            FluteMaintainModel model = new FluteMaintainModel();
            model.Flutes = PMTsDbContext.Flute.Where(x => x.FactoryCode == factoryCode).ToList();
            model.FluteTrs = PMTsDbContext.FluteTr.Where(x => x.FactoryCode == factoryCode).OrderBy(x => x.Item).ToList();
            return model;
        }

        public bool AddMaintainFlute(FluteMaintainModel model)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    Flute Flute = new Flute();
                    Flute.FactoryCode = model.Flute.FactoryCode;
                    Flute.Flute1 = model.Flute.Flute1;
                    Flute.Code = string.IsNullOrEmpty(model.Flute.Code) ? null : model.Flute.Code;
                    Flute.Description = string.IsNullOrEmpty(model.Flute.Description) ? null : model.Flute.Description;
                    Flute.Height = model.Flute.Height;
                    Flute.Status = model.Flute.Status;
                    Flute.A = model.Flute.A;
                    Flute.B = model.Flute.B;
                    Flute.C = model.Flute.C;
                    Flute.D1 = model.Flute.D1;
                    Flute.D2 = model.Flute.D2;
                    Flute.Tr1 = model.FluteTrs == null ? 0 : model.FluteTrs.Count <= 1 ? 0 : model.FluteTrs[1].Item == 2 ? model.FluteTrs[1].Tr
                                                           : model.FluteTrs.Count <= 3 ? 0 : model.FluteTrs[3].Item == 2 ? model.FluteTrs[3].Tr
                                                           : model.FluteTrs.Count <= 5 ? 0 : model.FluteTrs[5].Item == 2 ? model.FluteTrs[5].Tr : 0;
                    Flute.Tr2 = model.FluteTrs == null ? 0 : model.FluteTrs.Count <= 1 ? 0 : model.FluteTrs[1].Item == 4 ? model.FluteTrs[1].Tr
                                                           : model.FluteTrs.Count <= 3 ? 0 : model.FluteTrs[3].Item == 4 ? model.FluteTrs[3].Tr
                                                           : model.FluteTrs.Count <= 5 ? 0 : model.FluteTrs[5].Item == 4 ? model.FluteTrs[5].Tr : 0;
                    Flute.Tr3 = model.FluteTrs == null ? 0 : model.FluteTrs.Count <= 1 ? 0 : model.FluteTrs[1].Item == 6 ? model.FluteTrs[1].Tr
                                                           : model.FluteTrs.Count <= 3 ? 0 : model.FluteTrs[3].Item == 6 ? model.FluteTrs[3].Tr
                                                           : model.FluteTrs.Count <= 5 ? 0 : model.FluteTrs[5].Item == 6 ? model.FluteTrs[5].Tr : 0;
                    Flute.JoinSize = model.Flute.JoinSize;
                    Flute.GlueArea = model.Flute.GlueArea;
                    Flute.BundlePiece = model.Flute.BundlePiece;
                    int layertmp = 0;
                    try { layertmp = model.FluteTrs.Count(); } catch { }
                    Flute.Layer = layertmp;// model.Flute.Layer;
                    Flute.Trim = model.Flute.Trim;
                    Flute.Stack = model.Flute.Stack;
                    Flute.WasteStack = model.Flute.WasteStack;
                    Flute.SpeedFactor = model.Flute.SpeedFactor;
                    Flute.Speed = model.Flute.Speed;
                    Flute.SetupTime = model.Flute.SetupTime;
                    Flute.NoOfChange = model.Flute.NoOfChange;
                    Flute.LayerPallet = model.Flute.LayerPallet;
                    Flute.BoxPerBundleNoJoint = model.Flute.BoxPerBundleNoJoint;
                    Flute.LayerPerPalletNoJoint = model.Flute.LayerPerPalletNoJoint;
                    Flute.Thickness = model.Flute.Thickness == 0 ? 0 : model.Flute.Thickness;
                    Flute.CreatedBy = model.Flute.CreatedBy;
                    Flute.CreatedDate = System.DateTime.Now;
                    PMTsDbContext.Flute.Add(Flute);
                    PMTsDbContext.SaveChanges();


                    if (PMTsDbContext.FluteTr.Select(x => x.FactoryCode == model.Flute.FactoryCode && x.FluteCode == model.Flute.Flute1).Count() > 0)
                    {
                        PMTsDbContext.FluteTr.RemoveRange(PMTsDbContext.FluteTr.Where((x => x.FactoryCode == model.Flute.FactoryCode && x.FluteCode == model.Flute.Flute1)));
                        PMTsDbContext.SaveChanges();
                    }

                    foreach (var item in model.FluteTrs)
                    {
                        FluteTr FluteTr = new FluteTr
                        {
                            FactoryCode = model.Flute.FactoryCode,
                            FluteCode = model.Flute.Flute1,
                            Station = item.Station,
                            Tr = item.Tr,
                            Item = item.Item,
                            HasCoating = item.HasCoating,
                            Status = item.Status

                        };
                        PMTsDbContext.FluteTr.Add(FluteTr);
                    }
                    PMTsDbContext.SaveChanges();
                    dbContextTransaction.Commit();
                    return true;
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public bool UpdateMaintainFlute(FluteMaintainModel model)
        {
            using (var dbContextTransaction = PMTsDbContext.Database.BeginTransaction())
            {
                try
                {
                    var Flute = PMTsDbContext.Flute.Where(z => z.FactoryCode == model.Flute.FactoryCode && z.Flute1 == model.Flute.Flute1).FirstOrDefault();
                    //Flute.FactoryCode = model.Flute.FactoryCode;
                    //Flute.Flute1 = model.Flute.Flute1;
                    Flute.Code = string.IsNullOrEmpty(model.Flute.Code) ? null : model.Flute.Code;
                    Flute.Description = string.IsNullOrEmpty(model.Flute.Description) ? null : model.Flute.Description;
                    Flute.Height = model.Flute.Height;
                    Flute.Status = model.Flute.Status;
                    Flute.A = model.Flute.A;
                    Flute.B = model.Flute.B;
                    Flute.C = model.Flute.C;
                    Flute.D1 = model.Flute.D1;
                    Flute.D2 = model.Flute.D2;
                    Flute.Tr1 = model.FluteTrs == null ? 0 : model.FluteTrs.Count <= 1 ? 0 : model.FluteTrs[1].Item == 2 ? model.FluteTrs[1].Tr
                                                           : model.FluteTrs.Count <= 3 ? 0 : model.FluteTrs[3].Item == 2 ? model.FluteTrs[3].Tr
                                                           : model.FluteTrs.Count <= 5 ? 0 : model.FluteTrs[5].Item == 2 ? model.FluteTrs[5].Tr : 0;
                    Flute.Tr2 = model.FluteTrs == null ? 0 : model.FluteTrs.Count <= 1 ? 0 : model.FluteTrs[1].Item == 4 ? model.FluteTrs[1].Tr
                                                           : model.FluteTrs.Count <= 3 ? 0 : model.FluteTrs[3].Item == 4 ? model.FluteTrs[3].Tr
                                                           : model.FluteTrs.Count <= 5 ? 0 : model.FluteTrs[5].Item == 4 ? model.FluteTrs[5].Tr : 0;
                    Flute.Tr3 = model.FluteTrs == null ? 0 : model.FluteTrs.Count <= 1 ? 0 : model.FluteTrs[1].Item == 6 ? model.FluteTrs[1].Tr
                                                           : model.FluteTrs.Count <= 3 ? 0 : model.FluteTrs[3].Item == 6 ? model.FluteTrs[3].Tr
                                                           : model.FluteTrs.Count <= 5 ? 0 : model.FluteTrs[5].Item == 6 ? model.FluteTrs[5].Tr : 0;
                    Flute.JoinSize = model.Flute.JoinSize;
                    Flute.GlueArea = model.Flute.GlueArea;
                    Flute.BundlePiece = model.Flute.BundlePiece;
                    int layertmp = 0;
                    try { layertmp = model.FluteTrs.Count(); } catch { }
                    Flute.Layer = layertmp;// model.Flute.Layer;
                    Flute.Trim = model.Flute.Trim;
                    Flute.Stack = model.Flute.Stack;
                    Flute.WasteStack = model.Flute.WasteStack;
                    Flute.SpeedFactor = model.Flute.SpeedFactor;
                    Flute.Speed = model.Flute.Speed;
                    Flute.SetupTime = model.Flute.SetupTime;
                    Flute.NoOfChange = model.Flute.NoOfChange;
                    Flute.LayerPallet = model.Flute.LayerPallet;
                    Flute.BoxPerBundleNoJoint = model.Flute.BoxPerBundleNoJoint;
                    Flute.LayerPerPalletNoJoint = model.Flute.LayerPerPalletNoJoint;
                    Flute.Thickness = model.Flute.Thickness == 0 ? 0 : model.Flute.Thickness;
                    Flute.UpdatedBy = model.Flute.UpdatedBy;
                    Flute.UpdatedDate = System.DateTime.Now;

                    if (PMTsDbContext.FluteTr.Select(x => x.FactoryCode == model.Flute.FactoryCode && x.FluteCode == model.Flute.Flute1).Count() > 0)
                    {
                        PMTsDbContext.FluteTr.RemoveRange(PMTsDbContext.FluteTr.Where((x => x.FactoryCode == model.Flute.FactoryCode && x.FluteCode == model.Flute.Flute1)));
                        PMTsDbContext.SaveChanges();
                    }

                    foreach (var item in model.FluteTrs)
                    {
                        FluteTr FluteTr = new FluteTr
                        {
                            FactoryCode = model.Flute.FactoryCode,
                            FluteCode = model.Flute.Flute1,
                            Station = item.Station,
                            Tr = item.Tr,
                            Item = item.Item,
                            HasCoating = item.HasCoating,
                            Status = item.Status

                        };
                        PMTsDbContext.FluteTr.Add(FluteTr);
                    }
                    PMTsDbContext.SaveChanges();
                    dbContextTransaction.Commit();
                    return true;
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
                {
                    dbContextTransaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public IEnumerable<FluteAndMachineModel> GetFlutesAndMachinesByFactoryCode(string factoryCode, IConfiguration config)
        {
            using IDbConnection db = new SqlConnection(config.GetConnectionString("PMTsConnect"));
            if (db.State == ConnectionState.Closed)
                db.Open();

            string sql = "";
            string message = "";

            if (factoryCode == "259B")
            {
                sql = @"
                        select m.FactoryCode,
                        m.Flute,
                        r.Machine as Machine, 
						case when t.Trim is not null then t.Trim else f.Trim end Trim,
						m.BoxType,
						m.PrintMethod,
                        m.ProType,
                        count(*), 
                        m.Flute + ' - ' + r.Machine + ' - ' + m.BoxType + ' - ' + m.PrintMethod + ' - ' + m.ProType + ' (' + cast(count(*) as nvarchar(5)) + ')' as DescriptionDisplay,
                        m.Flute + ',' + r.Machine + ',' + m.BoxType + ',' + m.PrintMethod + ',' + m.ProType  as FluteAndMachine
                        from
                        (select FactoryCode, Material_No, Flute,
                        case when substring(Hierarchy, 3, 2) = 'SO' then 'SO' else 'Other' end BoxType,
						case when Print_Method in ('Solid 1 สี','Solid 2 สี') then Print_Method else 'Other' end PrintMethod,
                        case when Pro_Type like 'Cor%' then 'Cor' else 'Other' end ProType
                        from MasterData 
                        where PDIS_Status not in ('N', 'X') and FactoryCode = '{0}') m 
                        left outer join Routing r on r.Material_No = m.Material_No and r.FactoryCode = m.FactoryCode and r.Mat_Code like 'COR%'
                        left outer join Cor_Config c on c.Name = r.Machine and c.FactoryCode = r.FactoryCode
                        left outer join Flute f on f.Flute = m.Flute and f.FactoryCode = m.FactoryCode
                        left outer join Board_Use b on b.Material_No = m.Material_No and b.FactoryCode = m.FactoryCode
                        left outer join MachineFluteTrim t on t.FactoryCode = m.FactoryCode and t.Machine = r.Machine and t.Flute = m.Flute
                        where r.Material_No is not null and c.Name is not null and b.Material_No is not null and case when t.Trim is not null then t.Trim else f.Trim end > 1
                        group by m.FactoryCode, m.Flute, r.Machine, case when t.Trim is not null then t.Trim else f.Trim end, m.BoxType, m.PrintMethod, m.ProType
                        order by m.FactoryCode, m.Flute, r.Machine
                        ";
                message = string.Format(sql, factoryCode);
            }
            else
            {

                sql = @"
                        select m.FactoryCode,
                        m.Flute,
                        case when t.Trim is not null then t.Trim else f.Trim end Trim,
                        r.Machine as Machine, 
                        count(*), 
                        m.Flute + ' - ' + r.Machine + ' (' + cast(count(*) as nvarchar(5)) + ')' as DescriptionDisplay,
                        m.Flute + ',' + r.Machine  as FluteAndMachine
                        from
                        (select FactoryCode, Material_No, Flute
                        from MasterData 
                        where PDIS_Status not in ('N', 'X') and FactoryCode = '{0}') m 
                        left outer join Routing r on r.Material_No = m.Material_No and r.FactoryCode = m.FactoryCode and r.Mat_Code like 'COR%'
                        left outer join Cor_Config c on c.Name = r.Machine and c.FactoryCode = r.FactoryCode
                        left outer join Flute f on f.Flute = m.Flute and f.FactoryCode = m.FactoryCode
                        left outer join Board_Use b on b.Material_No = m.Material_No and b.FactoryCode = m.FactoryCode
                        left outer join MachineFluteTrim t on t.FactoryCode = m.FactoryCode and t.Machine = r.Machine and t.Flute = m.Flute
                        where r.Material_No is not null and c.Name is not null and b.Material_No is not null and case when t.Trim is not null then t.Trim else f.Trim end > 1
                        group by m.FactoryCode, m.Flute, r.Machine, case when t.Trim is not null then t.Trim else f.Trim end
                        order by m.FactoryCode, m.Flute, r.Machine
                        ";
                message = string.Format(sql, factoryCode);
            }
            return db.Query<FluteAndMachineModel>(message).ToList();
        }

        #endregion

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }
    }
}
