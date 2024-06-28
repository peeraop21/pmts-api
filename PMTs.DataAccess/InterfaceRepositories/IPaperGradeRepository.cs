using Microsoft.Extensions.Configuration;
using PMTs.DataAccess.ComplexModels;
using PMTs.DataAccess.Models;
using System.Collections.Generic;

namespace PMTs.DataAccess.InterfaceRepositories
{
    public interface IPaperGradeRepository : IRepository<PaperGrade>
    {
        List<PaperGrade> GetPaperGradeAll();
        PaperGrade GetPaperGradeByGradeAndActive(string grade);
        IEnumerable<PaperGradeViewModel> GetPaperGradesWithGradeCodeMachine(IConfiguration config, string factoryCode);
        void SavePaperGradeWithGradeCodeMachine(string factoryCode, PaperGradeViewModel paperGradeViewModel);
        void UpdatePaperGradeWithGradeCodeMachine(string factoryCode, PaperGradeViewModel paperGradeViewModel);
        IEnumerable<PaperGrade> GetAllPaperGrades(IConfiguration config);
        List<string> GetGrades();
    }
}
