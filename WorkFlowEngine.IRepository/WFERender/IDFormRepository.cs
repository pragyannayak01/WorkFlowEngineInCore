using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.WFERender;

namespace WorkFlowEngine.IRepository.WFERender
{
    public interface IDFormRepository
    {
        Task<string> FillDataTable_New(int Formid, string ApplicantNo);
        Task<string> PostResults(DformResultDomain res, int Userid, string Status, int DeptId);
        Task<string> UpdateResult(string Data, int Formid, int APPROVESTATUS, string Appno, string Remark, int UserId, int ActionNo, string Result);
        DataSet GetLastUpdatedData(int Id, int DeptId, int UserId, int DesgId, string District, string Year, string Month);
        DataSet GetEditResultData(int Id, int Formid);
        Task<int> DeleteResult(int resultId, int Id);
        Task<IEnumerable<FormModel>> GetFormName();
        bool IsRecordAlreadyExist(int FormId, string Result, string ApplicantNo);
    }

}
