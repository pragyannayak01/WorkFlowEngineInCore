using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.WFERender;
using WorkFlowEngine.IRepository.DapperConfiguration;
using WorkFlowEngine.IRepository.WFEngine.ReportDesign;
using WorkFlowEngine.Repository.DapperConfiguration;

namespace WorkFlowEngine.Repository.WFEngine.ReportDesign
{
    public class FormRepository : RepositoryBase, IFormRepository
    {
        string strReturnValue = string.Empty;
        public FormRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
        public async Task<IEnumerable<Forms>> GetAllForms(int DeptId)
        {
            try
            {
                var dyParam = new DynamicParameters();
                string query = "select df.ID ,lgd.SCHEME_NAME  from DF_FORM df join LGD_TBL_SCHEME lgd on df.SCHEMEID=lgd.ID  where lgd.deletedflag = 0 and CASE WHEN " + DeptId + "=0 THEN " + DeptId + " ELSE lgd.deptid END =" + DeptId + "  ";
                var result = await Connection.QueryAsync<Forms>(query, dyParam, commandType: CommandType.Text);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<IEnumerable<Forms>> GetallFormColumnById(int id)
        {
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("@P_Action", "C");
                dyParam.Add("@P_INTFORM_ID", id);
                var query = "USP_GET_COLUMN_NAME";
                var result = await Connection.QueryAsync<Forms>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }

}
