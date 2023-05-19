using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.WFEngine;

namespace WorkFlowEngine.IRepository.WFEngine
{
    public interface IFormTableServices
    {
        Task<IEnumerable<FormModel>> GetDepartment(int Deptid, int HID);
        Task<IEnumerable<FormModel>> GetNoApproveData(int status, FormModel Survey);
        Task<int> CreateSurveyGetId(string postId, string resultJson, int schemeId = 0);
        Task<int> GetLastInsertFormId();
        Task<string> SaveApproveData(string postId);
        Task<string> StoreSurvey(string postId, string resultJson);
        Task<FormModel> GetFormData(int ID);
        Task<string> CreateTable(List<Element> objs, int Id, string name, string remark, string status, string Formtype);
        Task<IEnumerable<FormModel>> GetFormName();
        Task<IEnumerable<Domain.WFERender.Dformconfig>> GetColumnData(int FormId);
        string InsertColumnconfigData(Domain.WFERender.Dformconfig component);
        Task<IEnumerable<Domain.WFERender.Dformconfig>> GetConstraintData(int FormId);
        string InsertConstraintConfigData(Domain.WFERender.Dformconfig component);
        DataSet GetTakeActionData(int Formid, string Applicant_No);
        DataTable GetDropdownData(string Value_Field, string Display_Field, string Table_Name, string listkey);
    }

}
