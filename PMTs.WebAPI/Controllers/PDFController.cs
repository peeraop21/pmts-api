using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PMTs.WebAPI.Controllers
{
    public class PDFController : ControllerBase
    {
        public byte[] SavePDFWithOutAttachFile(string FactoryCode, PrintMasterCardMOModel printMasterCardMOModel, string pmtsConfigPath, MoDataController moData)
        {
            var fileNames = new List<string>();
            var fileTemps = new List<string>();
            var MOsWithoutAttachfiles = new List<MasterCardMO>();
            var MOsSerialize = JsonConvert.SerializeObject(printMasterCardMOModel);
            var originalMOs = JsonConvert.DeserializeObject<PrintMasterCardMOModel>(MOsSerialize);
            var fileNo = 0;
            var MOsNo = 0;
            var MOsWithAttachfiles = printMasterCardMOModel.MasterCardMOs.Where(m => !string.IsNullOrEmpty(m.AttchFilesBase64)).ToList();
            var filePath = !string.IsNullOrEmpty(pmtsConfigPath) ? pmtsConfigPath : throw new Exception("Can't get file path.");
            var datetimeNow = DateTime.Now.ToString("dd_MM_yyyy_HH_mm_ss");
            var pageSize = Size.A4;
            var viewMasterCardMOName = "MasterCardMO";

            //create folder if doesn't exist
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            #region Combine all print mastercard file

            foreach (var masterCardMO in originalMOs.MasterCardMOs)
            {
                MOsNo++;
                if (string.IsNullOrEmpty(masterCardMO.AttchFilesBase64))
                {
                    MOsWithoutAttachfiles.Add(masterCardMO);
                    if (originalMOs.MasterCardMOs.Count == MOsNo)
                    {
                        if (MOsWithoutAttachfiles != null && MOsWithoutAttachfiles.Count > 0)
                        {
                            fileNo++;
                            var GenarateFileName = $"{datetimeNow}_FileMain{fileNo}.pdf";
                            var fullPath = Path.Combine(filePath, GenarateFileName);
                            printMasterCardMOModel.MasterCardMOs = new List<MasterCardMO>();
                            printMasterCardMOModel.MasterCardMOs.AddRange(MOsWithoutAttachfiles);

                            var actionResultWithoutAttachfiles = new ViewAsPdf(viewMasterCardMOName, printMasterCardMOModel)
                            {
                                FileName = Path.Combine(filePath, $"{datetimeNow}_FileMain{fileNo}.pdf"),
                                PageMargins = new Margins(0, 2, 0, 2),
                                PageSize = pageSize,
                                ContentType = "application/pdf",
                                PageOrientation = Orientation.Portrait,
                            };

                            var byteArrayWithoutAttachfiles = actionResultWithoutAttachfiles.BuildFile(moData.ControllerContext);

                            var fileStreamWithoutAttachfiles = new FileStream(Path.Combine(filePath, $"{datetimeNow}_FileMain{fileNo}.pdf"), FileMode.Create, FileAccess.Write);
                            fileStreamWithoutAttachfiles.Write(byteArrayWithoutAttachfiles.Result, 0, byteArrayWithoutAttachfiles.Result.Length);
                            fileStreamWithoutAttachfiles.Close();

                            fileTemps.Add(fullPath);
                            fileNames.Add(fullPath);

                            MOsWithoutAttachfiles.Clear();
                        }
                    }
                }
                else if (!string.IsNullOrEmpty(masterCardMO.AttchFilesBase64) || originalMOs.MasterCardMOs.Count == MOsNo)
                {
                    if (MOsWithoutAttachfiles != null && MOsWithoutAttachfiles.Count > 0)
                    {
                        fileNo++;
                        var GenarateFileName = $"{datetimeNow}_FileMain{fileNo}.pdf";
                        var fullPath = Path.Combine(filePath, GenarateFileName);
                        printMasterCardMOModel.MasterCardMOs = new List<MasterCardMO>();
                        printMasterCardMOModel.MasterCardMOs.AddRange(MOsWithoutAttachfiles);

                        var actionResultWithoutAttachfiles = new ViewAsPdf(viewMasterCardMOName, printMasterCardMOModel)
                        {
                            FileName = Path.Combine(filePath, $"{datetimeNow}_FileMain{fileNo}.pdf"),
                            PageMargins = new Margins(0, 2, 0, 2),
                            PageSize = pageSize,
                            ContentType = "application/pdf",
                            PageOrientation = Orientation.Portrait,
                        };

                        var byteArrayWithoutAttachfiles = actionResultWithoutAttachfiles.BuildFile(moData.ControllerContext);

                        var fileStreamWithoutAttachfiles = new FileStream(Path.Combine(filePath, $"{datetimeNow}_FileMain{fileNo}.pdf"), FileMode.Create, FileAccess.Write);
                        fileStreamWithoutAttachfiles.Write(byteArrayWithoutAttachfiles.Result, 0, byteArrayWithoutAttachfiles.Result.Length);
                        fileStreamWithoutAttachfiles.Close();

                        fileTemps.Add(fullPath);
                        fileNames.Add(fullPath);

                        MOsWithoutAttachfiles.Clear();
                    }

                    var attachfiles = JsonConvert.DeserializeObject<List<AttachFileMo>>(masterCardMO.AttchFilesBase64);
                    if (attachfiles != null && attachfiles.Count > 0)
                    {
                        var GenarateFileName = $"{datetimeNow}_{masterCardMO.OrderItem}_FileMain.pdf";
                        var fullPath = Path.Combine(filePath, GenarateFileName);
                        printMasterCardMOModel.MasterCardMOs = new List<MasterCardMO>();
                        printMasterCardMOModel.MasterCardMOs.Add(masterCardMO);
                        var actionResult = new ViewAsPdf(viewMasterCardMOName, printMasterCardMOModel)
                        {
                            FileName = fullPath,
                            PageMargins = new Margins(0, 2, 0, 2),
                            PageSize = pageSize,
                            ContentType = "application/pdf",
                            PageOrientation = Orientation.Portrait,
                        };

                        var byteArray = actionResult.BuildFile(moData.ControllerContext);

                        var fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write);
                        fileStream.Write(byteArray.Result, 0, byteArray.Result.Length);
                        fileStream.Close();

                        fileTemps.Add(fullPath);
                        fileNames.Add(fullPath);

                        foreach (var attachfile in attachfiles)
                        {
                            fileNames.Add(attachfile.PathNew);
                        }
                    }
                }
            }

            var bytesOfAllFile = CombineMultiplePdFs(fileNames, FactoryCode, pmtsConfigPath);

            if (!string.IsNullOrEmpty(bytesOfAllFile))
            {
                byte[] pdfbytearray = JsonConvert.DeserializeObject<byte[]>(bytesOfAllFile);
                if (pdfbytearray.Length > 0)
                {
                    //delete temp pdf file
                    foreach (var fileTemp in fileTemps)
                    {
                        System.IO.File.Delete(fileTemp);
                    }
                }
                return pdfbytearray;
            }
            else
            {
                throw new Exception("Print mastercard Failed");
            }

            #endregion Combine all print mastercard file
        }

        private string CombineMultiplePdFs(List<string> fileNames, string factoryCode, string pmtsConfigPath)
        {
            var isMergeSuccess = false;
            var pdfFileLocation = !string.IsNullOrEmpty(pmtsConfigPath) ? pmtsConfigPath : throw new Exception("Can't get file path.");
            var result = string.Empty;

            try
            {
                var document = new Document();
                var baos = new MemoryStream();
                PdfCopy writer = new PdfCopy(document, baos);
                document.Open();
                foreach (string fileName in fileNames)
                {
                    try
                    {
                        PdfReader reader = new PdfReader(fileName);
                        reader.ConsolidateNamedDestinations();
                        for (int i = 1; i <= reader.NumberOfPages; i++)
                        {
                            PdfImportedPage page = writer.GetImportedPage(reader, i);
                            writer.AddPage(page);
                        }
                        PrAcroForm form = reader.AcroForm;
                        if (form != null)
                        {
                            writer.CopyAcroForm(reader);
                        }

                        reader.Close();
                    }
                    catch
                    {
                        continue;
                    }
                }
                writer.Close();
                document.Close();
                byte[] documentBytes = baos.ToArray();
                result = JsonConvert.SerializeObject(documentBytes);
                isMergeSuccess = true;
            }
            catch
            {
                result = string.Empty;
                isMergeSuccess = false;
            }

            if (isMergeSuccess)
            {
                return result;
            }
            else
            {
                return result;
            }
        }
    }
}