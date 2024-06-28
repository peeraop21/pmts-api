using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class MachineRepository : Repository<Machine>, IMachineRepository
    {
        public MachineRepository(PMTsDbContext context)
            : base(context)
        {
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public List<string> GetMachinesList(string factoryCode)
        {
            return PMTsDbContext.Machine.Where(w => w.FactoryCode == factoryCode).Select(s => s.Machine1).ToList();
        }
        public Machine GetMachineGroupByMachine(string factoryCode, string machine)
        {
            return PMTsDbContext.Machine.FirstOrDefault(w => w.Machine1.Trim() == machine.Trim() && w.FactoryCode == factoryCode && w.MachineStatus == true);//(w => w.Machine1.Equals(machine));
        }

        public IEnumerable<Machine> GetMachinesByPlanCodes(string factoryCode, List<string> planCodes)
        {
            var machines = new List<Machine>();
            machines.AddRange(PMTsDbContext.Machine.Where(m => m.FactoryCode == factoryCode && planCodes.Contains(m.PlanCode)).AsNoTracking().ToList());

            return machines;
        }
        //public IEnumerable<Machine> GetMachineHierarchy(string factoryCode, string hieLv2)
        //{


        //    //var data = (from m in PMTsDbContext.Machine
        //    //              join hm in PMTsDbContext.HierarchyLv2machineMapping on
        //    //              m.MachineGroup equals hm.MachineGroup
        //    //              where hm.HierarchyLv2 == hieLv2 && m.FactoryCode == factoryCode && 
        //    //              m.Mina >= masterdata.ScoreL2  && m.Maxa >= masterdata.ScoreL2 &&
        //    //              m.Minb >= masterdata.ScoreL3 && m.Maxb >= masterdata.ScoreL3 &&
        //    //              m.Minc >= masterdata.Scorew2 && m.Maxc >= masterdata.Scorew2 &&
        //    //              m.Mind >= masterdata.Scorew3 && m.Maxd >= masterdata.Scorew3 &&
        //    //              m.Mine >= masterdata.JointLap && m.Maxe >= masterdata.JointLap 
        //    //            select new Machine {
        //    //                  Machine1 = m.Machine1
        //    //              }).ToList();

        //    //return data;

        //    var data = (from m in PMTsDbContext.Machine
        //                join hm in PMTsDbContext.HierarchyLv2machineMapping on
        //                m.MachineGroup equals hm.MachineGroup
        //                where hm.HierarchyLv2 == hieLv2 && m.FactoryCode == factoryCode
        //                select new Machine
        //                {
        //                    Machine1 = m.Machine1
        //                }).ToList();

        //    return data;
        //}

        // public IEnumerable<Machine> GetMachineHierarchy(string factoryCode, string hieLv2,MasterData masterData)
        public List<Machine> GetMachineHierarchy(IConfiguration config, string factoryCode, string hieLv2, MasterData masterData, string floxotype, string JoinType)
        {

            var machines = new List<Machine>();
            if (floxotype == "DC")
            {
                machines = (from m in PMTsDbContext.Machine.Where(m =>
                    m.FactoryCode == factoryCode &&
                    m.CodeMachineType == "D/C" &&
                    m.Minl <= masterData.CutSheetLeng &&
                    m.Maxl >= masterData.CutSheetLeng &&
                    m.Minw <= masterData.CutSheetWid &&
                    m.Maxw >= masterData.CutSheetWid)
                            orderby m.Priority ascending
                            select m).ToList();

            }
            // หา flexo
            else if (hieLv2 == "DF")
            {
                machines = (from m in PMTsDbContext.Machine.Where(m =>
                    m.FactoryCode == factoryCode &&
                    m.CodeMachineType == floxotype &&
                    m.Minl <= masterData.CutSheetLeng &&
                    m.Maxl >= masterData.CutSheetLeng &&
                    m.Minw <= masterData.CutSheetWid &&
                    m.Maxw >= masterData.CutSheetWid)
                            orderby m.Priority ascending
                            select m).ToList();
            }
            else
            {

                if (JoinType == "กาวเครื่อง")
                {
                    machines = (from m in PMTsDbContext.Machine.Where(m =>
                        m.FactoryCode == factoryCode &&
                        m.CodeMachineType == floxotype &&
                        m.Minl <= masterData.CutSheetLeng &&
                        m.Maxl >= masterData.CutSheetLeng &&
                        m.Minw <= masterData.CutSheetWid &&
                        m.Maxw >= masterData.CutSheetWid &&
                        m.Mina <= masterData.ScoreL2 &&
                        m.Maxa >= masterData.ScoreL2 &&
                        m.Minb <= masterData.ScoreL3 &&
                        m.Maxb >= masterData.ScoreL3 &&
                        m.Minc <= masterData.Scorew2 &&
                        m.Maxc >= masterData.Scorew2 &&
                        m.Mind <= masterData.Scorew3 &&
                        m.Maxd >= masterData.Scorew3 &&
                        m.Mine <= masterData.JointLap &&
                        m.Maxe >= masterData.JointLap &&
                        m.GlueType == JoinType)
                                orderby m.Priority ascending
                                select m).ToList();
                }
                else
                {
                    machines = (from m in PMTsDbContext.Machine.Where(m =>
                        m.FactoryCode == factoryCode &&
                        m.CodeMachineType == floxotype &&
                        m.Minl <= masterData.CutSheetLeng &&
                        m.Maxl >= masterData.CutSheetLeng &&
                        m.Minw <= masterData.CutSheetWid &&
                        m.Maxw >= masterData.CutSheetWid &&
                        m.Mina <= masterData.ScoreL2 &&
                        m.Maxa >= masterData.ScoreL2 &&
                        m.Minb <= masterData.ScoreL3 &&
                        m.Maxb >= masterData.ScoreL3 &&
                        m.Minc <= masterData.Scorew2 &&
                        m.Maxc >= masterData.Scorew2 &&
                        m.Mind <= masterData.Scorew3 &&
                        m.Maxd >= masterData.Scorew3 &&
                        m.Mine <= masterData.JointLap &&
                        m.Maxe >= masterData.JointLap)
                                orderby m.Priority ascending
                                select m).ToList();
                }
            }

            return machines;
        }


        public List<Machine> GetMachineByMachineGroup(IConfiguration config, string factoryCode, string machineGroup)
        {

            var machines = new List<Machine>();
            if (machineGroup.Contains("ฟิล์ม"))
            {
                machines = (from m in PMTsDbContext.Machine.Where(x =>
                    x.FactoryCode == factoryCode &&
                    x.Machine1.Contains("ฟิล์ม"))
                            orderby m.Priority ascending, m.Machine1 ascending
                            select m).ToList();
            }
            else if (machineGroup.Contains("ตอก"))
            {
                machines = (from m in PMTsDbContext.Machine.Where(x =>
                    x.FactoryCode == factoryCode &&
                    x.GlueType == "ตอก")
                            orderby m.Priority ascending, m.Machine1 ascending
                            select m).ToList();

            }
            else if (machineGroup.Contains("กาว"))
            {
                machines = (from m in PMTsDbContext.Machine.Where(x =>
                    x.FactoryCode == factoryCode &&
                    x.GlueType == machineGroup)
                            orderby m.Priority ascending, m.Machine1 ascending
                            select m).ToList();

            }
            else
            {
                machines = (from m in PMTsDbContext.Machine.Where(x =>
                    x.FactoryCode == factoryCode &&
                    x.MachineGroup == machineGroup)
                            orderby m.Priority ascending, m.Machine1 ascending
                            select m).ToList();
            }

            return machines;

        }

        public void UpdateMachine(Machine machine)
        {
            try
            {
                if (machine == null)
                {
                    throw new Exception("Can't save machine without data");
                }
                //if(machine.MachineStatus !=false)
                //{


                //var existMachine = PMTsDbContext.Machine.FirstOrDefault(m => m.PlanCode == machine.PlanCode);
                //if (existMachine != null)
                //{
                //    throw new Exception("Can't save duplicate PlanCode data");
                //}
                //}
                PMTsDbContext.Machine.Update(machine);
                PMTsDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void CreateMachine(Machine machine)
        {
            try
            {
                if (machine == null)
                {
                    throw new Exception("Can't save machine without data");
                }

                var existMachine = PMTsDbContext.Machine.FirstOrDefault(m => m.PlanCode == machine.PlanCode && m.FactoryCode == machine.FactoryCode);
                if (existMachine != null)
                {
                    throw new Exception("Can't save duplicate PlanCode data");
                }

                PMTsDbContext.Machine.Add(machine);
                PMTsDbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
