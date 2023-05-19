using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.WFEngine;

namespace WorkFlowEngine.IRepository.WFEngine
{
    public interface IComponentRepository
    {
        Task<IEnumerable<Department>> GetDEPARTMENTDetails();
        Task<IEnumerable<Forms>> GetSchemes(int DeptId);
        int UpdateSchemeName(int id, string scehmeName, string insertName, string updateName, string deleteName, string selectName, string goalImage, int templateid, string ScriptFileName, string TABLE_NAME, string FORMTYPE, string PRINTCONFIG);
        int InsertScheme(int deptId, string scehmeName, string insertName, string updateName, string deleteName, string selectName, string goalImage, int templateid, string ScriptFileName, string TABLE_NAME, string FORMTYPE, string PRINTCONFIG);
        Task<DbTransactionResult> DeleteScehem(int id);
        Task<Forms> GetScheme(int id);
        Task<Forms> GetSchemeAndFormDetailsByFormId(int FormId);
        Task<Forms> GetSchemeAndFormDetailsBySchemeId(int SchemeId);
        Task<IEnumerable<Forms>> GetSchemesWithForm(int DeptId);
        Task<IEnumerable<Template>> GetTemplates();
        Task<IEnumerable<Template>> ViewTemplates();
        string UpdateTemplate(int templateid, int columnid, string templateName, string cssfile);
        string InsertTemplate(int columnid, string templateName, string cssfile);
        string DeleteTemplate(int id);
        Task<IEnumerable<Districts>> GetDistrictDetails();
        Task<IEnumerable<Block>> GetBlockDetails(int DistId);
    }

}
