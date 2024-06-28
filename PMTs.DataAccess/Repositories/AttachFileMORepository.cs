using Microsoft.EntityFrameworkCore;
using PMTs.DataAccess.InterfaceRepositories;
using PMTs.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PMTs.DataAccess.Repositories
{
    public class AttachFileMORepository : Repository<AttachFileMo>, IAttachFileMORepository
    {
        private readonly PMTsDbContext _context;

        public AttachFileMORepository(PMTsDbContext context) : base(context)
        {
            _context = context;
        }

        public PMTsDbContext PMTsDbContext
        {
            get { return Context as PMTsDbContext; }
        }

        public void AttachFileMoByMat(string factoryCode, string orderItem, string attachFileMoPath)
        {
            var filename = Path.GetFileName(attachFileMoPath);
            var attachFileMO = PMTsDbContext.AttachFileMo.FirstOrDefault(a => a.FactoryCode == factoryCode && a.OrderItem == orderItem && a.PathInit == filename);
            var attachFileMOs = PMTsDbContext.AttachFileMo.Where(a => a.FactoryCode == factoryCode && a.OrderItem == orderItem).ToList();
            var seqNo = attachFileMOs != null && attachFileMOs.Count() > 0 ? attachFileMOs.OrderByDescending(a => a.SeqNo).FirstOrDefault().SeqNo + 1 : 1;
            var modata = PMTsDbContext.MoData.FirstOrDefault(m => m.FactoryCode == factoryCode && m.OrderItem == orderItem);

            if (attachFileMO == null
                && !string.IsNullOrEmpty(attachFileMoPath)
                && !string.IsNullOrEmpty(factoryCode)
                && !string.IsNullOrEmpty(orderItem)
                && modata != null)
            {
                try
                {
                    var attachFileMasterdata = PMTsDbContext.MasterData.FirstOrDefault(a => a.FactoryCode == factoryCode && a.MaterialNo == modata.MaterialNo);
                    var attachFileMasterDataPath = attachFileMasterdata != null && !string.IsNullOrEmpty(attachFileMasterdata.AttachFileMoPath) ?
                        attachFileMasterdata.AttachFileMoPath
                        : throw new Exception($"MaterialNo {modata.MaterialNo} has't attachment");
                    var attachFileMOModel = new AttachFileMo
                    {
                        Id = 0,
                        FactoryCode = factoryCode,
                        OrderItem = orderItem,
                        PathInit = filename,
                        PathNew = attachFileMoPath,
                        SeqNo = seqNo,
                        Status = true,
                        UpdatedBy = "AttachFileMoByMat",
                        UpdatedDate = DateTime.Now,
                        CreatedBy = "AttachFileMoByMat",
                        CreatedDate = DateTime.Now,
                    };

                    //var fNameSrc = Path.GetFileName(attachFileMasterDataPath);
                    //var source = Path.GetDirectoryName(attachFileMasterDataPath);

                    //var fNameDest = Path.GetFileName(attachFileMoPath).Replace("_File", "_" + orderItem);
                    //var destination = Path.GetDirectoryName(attachFileMoPath);

                    //File.Copy(Path.Combine(source, fNameSrc), Path.Combine(destination, fNameDest));

                    PMTsDbContext.AttachFileMo.Add(attachFileMOModel);
                    PMTsDbContext.SaveChanges();

                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                if (modata == null)
                {
                    throw new Exception($"Can't find MoData : {orderItem}.");
                }
                else if (string.IsNullOrEmpty(attachFileMoPath))
                {
                    throw new Exception("AttachFileMO cannot be null.");
                }
                else if (string.IsNullOrEmpty(orderItem))
                {
                    throw new Exception("OrderItem cannot be null.");
                }
                else if (string.IsNullOrEmpty(factoryCode))
                {
                    throw new Exception("FactoryCode cannot be null.");
                }
                else
                {
                    throw new Exception("AttachFileMO already exists.");
                }
            }
        }

        public IEnumerable<AttachFileMo> GetAttachFileMOsBySaleOrders(List<string> saleOrders)
        {
            var attachFileMOs = new List<AttachFileMo>();

            if (saleOrders != null && saleOrders.Count > 0)
            {
                attachFileMOs = PMTsDbContext.AttachFileMo.Where(m => saleOrders.Contains(m.OrderItem)).AsNoTracking().ToList();
            }

            return attachFileMOs;
        }
        public IEnumerable<AttachFileMo> GetAttachFileMOsBySaleOrdersAndFactoryCode(string factoryCode, List<string> saleOrders)
        {
            var attachFileMOs = new List<AttachFileMo>();

            if (saleOrders != null && saleOrders.Count > 0)
            {
                attachFileMOs = PMTsDbContext.AttachFileMo.Where(m => m.FactoryCode == factoryCode && saleOrders.Contains(m.OrderItem)).AsNoTracking().ToList();
            }
            //foreach (var saleOrder in saleOrders)
            //{
            //    attachFileMOs.AddRange(PMTsDbContext.AttachFileMo.Where(m => m.FactoryCode == factoryCode && m.OrderItem.Equals(saleOrder)).AsNoTracking().ToList());
            //}

            return attachFileMOs;
        }
    }
}
