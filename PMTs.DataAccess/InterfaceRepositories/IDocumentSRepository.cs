using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IDocumentSRepository : IRepository<DocumentS>
    {
        IEnumerable<DocumentS> GetDocumentS(string FactoryCode, string SNumber);
        IEnumerable<DocumentSlist> GetDocumentSList(string FactoryCode, string OrderItem);
        DocumentSData GetDataByMo(string FactoryCode, string OrderItem, IConfiguration _configuration);
        void SaveDocumentS(string FactoryCode, CreateDocumentSModel model, IConfiguration _configuration);
        void UpdateDocumentS(string FactoryCode, CreateDocumentSModel model, IConfiguration _configuration);
        DocumentS CreateDocumentS(string FactoryCode, string Username);
        void DeleteDocumentS(string Id, IConfiguration _configuration);
        ReportDocumentS ReportDocumentS(string factorycode, string snumber, string user, IConfiguration _configuration);
        List<DocumentsMOData> GetDocumentsAndMODataByOrderItem(string factoryCode, string orderItem, string sNumber, IConfiguration config);
        void SaveChangeDocuments(string factoryCode, List<DocumentSlist> documents, IConfiguration config);
        List<DocumentSlist> GetDocumentSListForReportDocument(IConfiguration configuration, string factoryCode, string materialNO, string sO, string custName, string pC);
    }
}
