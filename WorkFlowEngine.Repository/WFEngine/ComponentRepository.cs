using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.WFEngine;
using WorkFlowEngine.IRepository.DapperConfiguration;
using WorkFlowEngine.IRepository.WFEngine;
using WorkFlowEngine.Repository.DapperConfiguration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WorkFlowEngine.Repository.WFEngine
{
    public class ComponentRepository : RepositoryBase, IComponentRepository
    {
        string strReturnValue = string.Empty;
        public ComponentRepository(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
        public async Task<IEnumerable<Department>> GetDEPARTMENTDetails()
        {
            //if (Connection.State != ConnectionState.Closed)
            //    Connection.Open();
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("P_DEPTID", 0);
                //dyParam.Add("P_Search",  search.DEPTNAME);
                dyParam.Add("P_Search", "");
                dyParam.Add("P_Action", "V");
                //dyParam.Add("P_FROMDATE",  FROMDATE);
                //dyParam.Add("P_TODATE",  TODATE);

                var query = "USP_DEPARTMENT_VIEW";
                var result = await Connection.QueryAsync<Department>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Forms>> GetSchemes(int DeptId)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            try
            {
                var dyParam = new DynamicParameters();
                // var query = "Select * From LGD_TBL_SCHEME where deletedflag=0 and deptid=" + DeptId + "";
                //string query = "SELECT lgd.*,df.* FROM lgd_tbl_scheme  lgd left  join DF_FORM  df on df.SCHEMEID = lgd.id where lgd.deletedflag=0 and lgd.deptid=" + DeptId + "";
                string query = "SELECT lgd.* FROM LGD_TBL_SCHEME  lgd where lgd.deletedflag=0 and CASE WHEN " + DeptId + "=0 THEN " + DeptId + " ELSE lgd.deptid END =" + DeptId + "  ";
                var result = await Connection.QueryAsync<Forms>(query, dyParam, commandType: CommandType.Text);
                return result;
                //var result = Connection.Query<Scheme, DFormDomain, Forms>(
                //                    query,
                //                    (scheme, form) =>
                //                    {
                //                        scheme.Form = form;
                //                        return scheme;
                //                    },
                //                    splitOn: "id");

                //return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<IEnumerable<Forms>> GetSchemesWithForm(int DeptId)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("P_DEPTID", DeptId);
                dyParam.Add("P_Action", "V");

                var query = "USP_FORM_TABLE";
                var result = await Connection.QueryAsync<Forms>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int UpdateSchemeName(int id, string scehmeName, string insertName, string updateName, string deleteName, string selectName, string goalImage, int templateid, string ScriptFileName, string TABLE_NAME, string FORMTYPE, string PRINTCONFIG)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            DbTransactionResult result = new DbTransactionResult();
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("@P_Action", "FU");
                dyParam.Add("@P_ID", id);
                dyParam.Add("@P_ScehmeName", scehmeName);
                dyParam.Add("@P_GoalImage", goalImage);
                dyParam.Add("@P_Templateid", templateid);
                dyParam.Add("@P_ScriptFileName", ScriptFileName);
                dyParam.Add("@P_PRINTCONFIG", PRINTCONFIG);
                dyParam.Add("@P_TABLE_NAME", TABLE_NAME);
                dyParam.Add("@P_FORMTYPE", FORMTYPE);
                dyParam.Add("@P_INSERT_NAME", insertName);
                dyParam.Add("@P_UPDATE_NAME", updateName);
                dyParam.Add("@P_DELETE_NAME", deleteName);
                dyParam.Add("@P_SELECT_NAME", selectName);
                dyParam.Add("@P_Msg", dbType: DbType.String, direction: ParameterDirection.Output, size: 5215585);
                int results = Connection.Execute("USP_FORM_TABLE", dyParam, commandType: CommandType.StoredProcedure);
                int newID = Convert.ToInt32(dyParam.Get<String>("@P_Msg"));
                return newID;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public int InsertScheme(int deptId, string scehmeName, string insertName, string updateName, string deleteName, string selectName, string goalImage, int templateid, string ScriptFileName, string TABLE_NAME, string FORMTYPE, string PRINTCONFIG)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            DbTransactionResult result = new DbTransactionResult();
            try
            {
                var p = new DynamicParameters();
                p.Add("@P_Action", "FI");
                p.Add("@P_ScehmeName", scehmeName);
                p.Add("@P_DEPTID", deptId);
                p.Add("@P_GoalImage", goalImage);
                p.Add("@P_Templateid", templateid);
                p.Add("@P_ScriptFileName", ScriptFileName);
                p.Add("@P_PRINTCONFIG", PRINTCONFIG);
                p.Add("@P_TABLE_NAME", TABLE_NAME);
                p.Add("@P_FORMTYPE", FORMTYPE);
                p.Add("@P_INSERT_NAME", insertName);
                p.Add("@P_UPDATE_NAME", updateName);
                p.Add("@P_DELETE_NAME", deleteName);
                p.Add("@P_SELECT_NAME", selectName);
                p.Add("@P_Msg", dbType: DbType.String, direction: ParameterDirection.Output, size: 5215585);
                //p.Add("@P_Msg_Out", SqlDbType.VarChar, direction: ParameterDirection.Output, size: 5215585);
                int results = Connection.Execute("USP_FORM_TABLE", p, commandType: CommandType.StoredProcedure);
                int newID = Convert.ToInt32(p.Get<String>("@P_Msg"));
                return newID;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<DbTransactionResult> DeleteScehem(int id)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            DbTransactionResult result = new DbTransactionResult();
            try
            {
                try
                {
                    string Tbl_Name = "";
                    var dyParams = new DynamicParameters();
                    Forms scheme = (Forms)GetSchemeAndFormDetailsBySchemeId(id).Result;
                    DataTable tbldt = new DataTable();
                    string Tbl_Data = "select a.table_name  from lgd_tbl_scheme a inner join df_form b on a.id=b.schemeid and b.astatus=1 where b.id = " + scheme.ID + " and a.deletedflag = 0";
                    IDataReader drtbl = Connection.ExecuteReader(Tbl_Data);
                    tbldt.Load(drtbl);
                    if (tbldt.Rows.Count > 0)
                    {
                        Tbl_Name = tbldt.Rows[0][0].ToString();
                    }
                    string sql1 = " drop table " + Tbl_Name;
                    result.count = await Connection.ExecuteAsync(sql1, dyParams, commandType: CommandType.Text);
                }
                catch
                {

                }
                //var query = "UPDATE LGD_TBL_SCHEME set deletedflag=1 where ID=" + id + "";
                var dyParam = new DynamicParameters();
                dyParam.Add("id", id);
                string sql = " delete from LGD_TBL_SCHEME where ID=" + id;
                result.count = await Connection.ExecuteAsync(sql, dyParam, commandType: CommandType.Text);
                return result;
            }
            //catch (SqlException ex)
            //{
            //    if (ex.Message.Contains("child"))
            //    {
            //        result.successid = 0; result.successmessage = "Scheme Has Active Forms";
            //        return result;
            //    }
            //    else throw ex;
            //}
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<Forms> GetScheme(int id)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            try
            {
                var dyParam = new DynamicParameters();
                var query = "Select * From LGD_TBL_SCHEME where ID=" + id + "";
                var result = await Connection.QueryFirstAsync<Forms>(query, dyParam, commandType: CommandType.Text);
                return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<Forms> GetSchemeAndFormDetailsByFormId(int FormId)
        {
            try
            {
                var dyParam = new DynamicParameters();
                var query = "SELECT lgd.*,df.* FROM LGD_TBL_SCHEME  lgd left  join DF_FORM  df on df.SCHEMEID = lgd.id where df.ID=" + FormId + "";
                var result = await Connection.QueryFirstAsync<Forms>(query, dyParam, commandType: CommandType.Text);
                return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<Forms> GetSchemeAndFormDetailsBySchemeId(int SchemeId)
        {
            try
            {
                var dyParam = new DynamicParameters();
                var query = "SELECT df.* FROM LGD_TBL_SCHEME  lgd left  join DF_FORM  df on df.SCHEMEID = lgd.id where lgd.ID=" + SchemeId + "";
                var result = await Connection.QueryFirstAsync<Forms>(query, dyParam, commandType: CommandType.Text);
                return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<IEnumerable<Template>> GetTemplates()
        {
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("P_ACTION", "GET");
                dyParam.Add("P_TEMPLATEID", 0);
                var query = "USP_TEMPLATE_VIEW_DELETE";
                var result = await Connection.QueryAsync<Template>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Template>> ViewTemplates()
        {
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("P_ACTION", "VIEW");
                dyParam.Add("P_TEMPLATEID", 0);
                //var query = "Select TEMPLATEID,TEMPLATE_NAME,CASE COLUMNID WHEN 1 THEN '1 COLUMN' WHEN 2 THEN '2 COLUMN' WHEN 3 THEN '3 COLUMN' WHEN 4 THEN  '4 COLUMN' END AS COLUMNNAME,CASE CSS_FILE WHEN 'Choose file' THEN 'NA' ELSE CSS_FILE END AS CSS_FILE From M_JK_TBL_TEMPLATE";
                var query = "USP_JK_TEMPLATE_VIEW_DELETE";
                var result = await Connection.QueryAsync<Template>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string InsertTemplate(int columnid, string templateName, string cssfile)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("P_ACTION", "ADD");
                dyParam.Add("P_TEMPLATEID", 0);
                dyParam.Add("P_TEMPLATENAME", templateName);
                dyParam.Add("P_COLUMNID", columnid);
                dyParam.Add("P_CSSFILE", cssfile);
                var query = "USP_JK_TEMPLATE_ADD_UPDATE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string UpdateTemplate(int templateid, int columnid, string templateName, string cssfile)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            DbTransactionResult result = new DbTransactionResult();
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("P_ACTION", "UPDATE");
                dyParam.Add("P_TEMPLATEID", templateid);
                dyParam.Add("P_TEMPLATENAME", templateName);
                dyParam.Add("P_COLUMNID", columnid);
                dyParam.Add("P_CSSFILE", cssfile);
                var query = "USP_JK_TEMPLATE_ADD_UPDATE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public string DeleteTemplate(int id)
        {
            IEnumerable<SuccessMessage> SuccessMessages;
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("P_ACTION", "DELETE");
                dyParam.Add("P_TEMPLATEID", id);
                var query = "USP_JK_TEMPLATE_VIEW_DELETE";
                SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                return SuccessMessages.AsList()[0].successid.ToString();
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public async Task<IEnumerable<Districts>> GetDistrictDetails()
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("P_Search", "");
                dyParam.Add("P_Action", "D");
                dyParam.Add("P_DIST_ID", null);
                var query = "USP_DISTRICT_BLOCK_VIEW";
                var result = await Connection.QueryAsync<Districts>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Block>> GetBlockDetails(int DistId)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("P_Search", "");
                dyParam.Add("P_Action", "B");
                dyParam.Add("P_DIST_ID", DistId);

                var query = "USP_DISTRICT_BLOCK_VIEW";
                var result = await Connection.QueryAsync<Block>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

}
