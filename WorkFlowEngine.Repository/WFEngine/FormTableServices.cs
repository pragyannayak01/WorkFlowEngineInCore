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

namespace WorkFlowEngine.Repository.WFEngine
{
    public class FormTableServices : RepositoryBase, IFormTableServices
    {
        string strReturnValue = string.Empty;
        public FormTableServices(IConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }
        public async Task<IEnumerable<FormModel>> GetDepartment(int Deptid, int HID)
        {
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("P_Action", "D");
                dyParam.Add("P_DEPTID", Deptid);
                dyParam.Add("p_inthierarchyid", HID);
                dyParam.Add("@P_Msg", size: 5215585);
                dyParam.Add("@P_Msg_Out", size: 5215585);
                var query = "USP_DYNAMIC_TABLE";
                var result = await Connection.QueryAsync<FormModel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch
            {
                throw;
            }
        }
        public async Task<IEnumerable<FormModel>> GetNoApproveData(int status, FormModel Survey)
        {
            try
            {
                var dyParam = new DynamicParameters();

                dyParam.Add("P_Action", "B");
                dyParam.Add("P_STATUS", status);
                dyParam.Add("P_DEPTID", Convert.ToInt32(Survey.DEPTID));
                dyParam.Add("@P_Msg", size: 5215585);
                dyParam.Add("@P_Msg_Out", size: 5215585);
                var query = "USP_DYNAMIC_TABLE";
                var result = await Connection.QueryAsync<FormModel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch
            {
                throw;
            }
        }
        public async Task<int> CreateSurveyGetId(string postId, string resultJson, int schemeId = 0)
        {
            if (Connection.State != ConnectionState.Open)
                Connection.Open();

            try
            {
                var p = new DynamicParameters();
                p.Add("FORMID", postId);
                p.Add("JSONSTRING", resultJson);
                p.Add("SCHEMEID", schemeId);
                var results = await Connection.QueryAsync<dynamic>("USP_DF_INSERTFORM", p, commandType: CommandType.StoredProcedure);

                return results.Count();
            }
            catch
            {
                throw;
            }
        }
        public async Task<int> GetLastInsertFormId()
        {
            try
            {
                //var query = " select * from DF_FORM  WHERE ROWNUM <= 1 ORDER BY id DESC ";
                var query = "select top 1 * from DF_FORM ORDER BY id desc";
                var dyParam = new DynamicParameters();
                var result = await Connection.QueryAsync<DFormDomain>(query, dyParam, commandType: CommandType.Text);
                var form = result.FirstOrDefault();
                return form.ID;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public async Task<string> SaveApproveData(string postId)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("P_Action", "C");
                p.Add("P_INTID", postId);
                p.Add("@P_Msg", size: 5215585);
                p.Add("@P_Msg_Out", size: 5215585);
                await Connection.QueryAsync<dynamic>("USP_DYNAMIC_TABLE", p, commandType: CommandType.StoredProcedure);
                string results = "2";
                return results;
            }
            catch
            {
                throw;
            };
        }
        public async Task<string> StoreSurvey(string postId, string resultJson)
        {
            try
            {
                string sql = "UPDATE  DF_FORM SET JSONSTRING='" + resultJson + "' where ID='" + postId + "'";
                var affectedRows = await Connection.ExecuteAsync(sql, new { postid = postId, resultJson = resultJson });

                //var p = new SqlDynamicParameters();
                //p.Add("fid", SqlDbType.Int, ParameterDirection.Input, postId);
                //p.Add("fjsonstring", SqlDbType.LongText, ParameterDirection.Input, JObject.Parse(resultJson) );
                //var results = await Connection.QueryAsync<dynamic>("USP_DF_UPDATEFORM", p, commandType: CommandType.StoredProcedure);

                return "Ok";
            }
            catch
            {
                throw;
            };
        }
        public async Task<FormModel> GetFormData(int ID)
        {
            try
            {
                var dyParam = new DynamicParameters();

                dyParam.Add("P_Action", "V");
                dyParam.Add("P_INTID", ID);
                dyParam.Add("@P_Msg", SqlDbType.VarChar, direction: ParameterDirection.Output, size: 5215585);
                dyParam.Add("@P_Msg_Out", SqlDbType.VarChar, direction: ParameterDirection.Output, size: 5215585);
                var query = "USP_DYNAMIC_TABLE";
                var result = await Connection.QueryAsync<FormModel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result.FirstOrDefault(); ;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> CreateTable(List<Element> objs, int Id, string fname, string remark, string status, string FormType)
        {
            try
            {

                string sqlbody = "";
                string sql_data = "";
                string sql_SEQ = "";
                string sql_TRG = "";
                char quote = '"';
                string sqlchild = "";
                string sql = "";
                string sqlvalue = "";
                string sqlquery = "";
                int count = 0;
                string Tbl_Name = "";
                String[] spearator = { "INTO" };
                StringSplitOptions options = 0;
                //prepare table params string..
                sqlbody = sqlbody + String.Join(",", objs.Select(x => x.name.Replace(" ", "") + ' ' + "VarChar(256)").ToArray());
                sqlvalue = sqlvalue + String.Join(",", objs.Select(x => "'" + x.defaultValue + "'").ToArray());

                DataTable tbldt = new DataTable();
                string Tbl_Data = "select a.table_name  from lgd_tbl_scheme a inner join df_form b on a.id=b.schemeid and b.astatus=1 where b.id = " + Id + " and a.deletedflag = 0";
                IDataReader drtbl = await Connection.ExecuteReaderAsync(Tbl_Data);
                tbldt.Load(drtbl);
                if (tbldt.Rows.Count > 0)
                {
                    Tbl_Name = tbldt.Rows[0][0].ToString();
                }
                #region Alter Query
                //sqlbody = sqlbody + "," + String.Join(",", objs.Select(x => quote + x.name + "Type" + quote + ' ' + x.title).ToArray());

                //For Compare Extra column


                string[] name = String.Join(",", objs.Select(x => x.name).ToArray()).Split(',');
                DataTable tbl = new DataTable();
                tbl.Columns.Add("name", typeof(string));
                for (int i = 0; i < name.Length; i++)
                {
                    tbl.Rows.Add(name[i].ToUpper());
                }

                DataTable dt_data = new DataTable();
                //string Check = "SELECT COUNT(*) FROM USER_TABLES WHERE table_name =upper('DF_" + Id + "' )";
                string Check = "SELECT COUNT(*) FROM information_schema.tables WHERE table_name =upper('" + Tbl_Name + "' )";
                IDataReader dr = await Connection.ExecuteReaderAsync(Check);
                DataTable dt = new DataTable();
                dt.Load(dr);
                if (Convert.ToInt32(dt.Rows[0][0]) > 0)
                {
                    string Exist_Data = "SELECT column_name as name  FROM information_schema.columns  WHERE table_name =upper('" + Tbl_Name + "') and column_name not in('INTID','APPLICANT_NO','INTCREATEDBY','DTMCREATEDON','REFID','DELETEDFLAG','INTUPDATEDBY','STATUS')";
                    IDataReader dr1 = await Connection.ExecuteReaderAsync(Exist_Data);
                    dt_data.Load(dr1);
                }
                string data = "";
                for (int i = 0; i < tbl.Rows.Count; i++)
                {
                    bool add = true;


                    for (int j = 0; j < dt_data.Rows.Count; j++)
                    {
                        if (dt_data.Rows[j]["name"].ToString().Replace(" ", "").ToUpper() == tbl.Rows[i]["name"].ToString().Replace(" ", "").ToUpper())
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        //sqlquery = sqlquery + "INTO DF_FORM_CONFIG(int_form_id,pvch_control_name,pvch_control_type,pvch_label_name) VALUES(" + Id + ",'','','')";
                        if (data == "")
                        { data = tbl.Rows[i]["name"].ToString().Replace(" ", "") + " VarChar(256)"; }
                        else
                        {
                            data = data + "," + tbl.Rows[i]["name"].ToString().Replace(" ", "") + " VarChar(256)";
                        }
                        foreach (var ob in objs)
                        {
                            if (ob.inputType != null)
                            {
                                ob.type = ob.inputType;
                            }
                            if (ob.name.ToUpper() == tbl.Rows[i]["name"].ToString().ToUpper())
                            {
                                if (sqlquery == "")
                                {
                                    sqlquery = "(" + Id + ",'" + ob.name + "','" + ob.type + "','" + ob.title + "',null,null,'',0,'','" + ob.Page_ApiParameter + "','" + ob.Page_Mapcontrol + "','" + ob.Page_Endpoint + "','" + ob.DbTable_Name + "','" + ob.ValueFiled + "','" + ob.DisplayFiled + "')";
                                }
                                else
                                {
                                    //sqlquery = sqlquery + "INTO DF_FORM_CONFIG(int_form_id,pvch_control_name,pvch_control_type,pvch_label_name) VALUES(" + Id + ",'" + ob.name + "','" + ob.type + "','" + ob.title + "')";
                                    sqlquery = sqlquery + ",(" + Id + ",'" + ob.name + "','" + ob.type + "','" + ob.title + "',null,null,'',0,'','" + ob.Page_ApiParameter + "','" + ob.Page_Mapcontrol + "','" + ob.Page_Endpoint + "','" + ob.DbTable_Name + "','" + ob.ValueFiled + "','" + ob.DisplayFiled + "')";
                                }
                            }
                        }
                    }
                    else
                    {
                        foreach (var ob in objs)
                        {
                            if (ob.inputType != null)
                            {
                                ob.type = ob.inputType;
                            }
                            if (ob.name.ToUpper() == tbl.Rows[i]["name"].ToString().ToUpper())
                            {
                                if (sqlquery == "")
                                {

                                    sqlquery = "(" + Id + ",'" + ob.name + "','" + ob.type + "','" + ob.title + "',null,null,'',0,'','" + ob.Page_ApiParameter + "','" + ob.Page_Mapcontrol + "','" + ob.Page_Endpoint + "','" + ob.DbTable_Name + "','" + ob.ValueFiled + "','" + ob.DisplayFiled + "')";
                                }
                                else
                                {
                                    sqlquery = sqlquery + ",(" + Id + ", '" + ob.name + "', '" + ob.type + "', '" + ob.title + "', null,null,'', 0,'','" + ob.Page_ApiParameter + "','" + ob.Page_Mapcontrol + "','" + ob.Page_Endpoint + "','" + ob.DbTable_Name + "','" + ob.ValueFiled + "','" + ob.DisplayFiled + "')";
                                }
                            }
                        }

                    }

                }
                // var dyParamc = new DynamicParameters();
                // dyParamc.Add("P_Action", "TBN");
                // dyParamc.Add("P_INTID", Id);
                // dyParamc.Add("@P_Msg", SqlDbType.VarChar, direction: ParameterDirection.Output, size: 5215585);
                // dyParamc.Add("@P_Msg_Out", SqlDbType.VarChar, direction: ParameterDirection.Output, size: 5215585);
                // var queryc = "USP_DYNAMIC_TABLE";
                // Connection.Execute(queryc, dyParamc, commandType: CommandType.StoredProcedure);
                //var Tbl_Name = dyParamc.Get<string>("@P_Msg_Out");


                if (Convert.ToInt32(dt.Rows[0][0]) > 0)
                {
                    if (data != "")
                    {
                        try
                        {
                            sql = " ALTER TABLE " + Tbl_Name + " ADD  " + data + "";
                            var affectedRows = await Connection.ExecuteAsync(sql);
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        };
                    }
                    //updating the deleted flag to avoid duplicacy
                    string dfsql = "update DF_FORM_CONFIG set int_deleted_flag=1 where int_form_id=" + Id + "";
                    var dfaffectedRows = Connection.Execute(dfsql);
                    //Thread.Sleep(10000);
                    string cfsql = "INSERT INTO DF_FORM_CONFIG(int_form_id,pvch_control_name,pvch_control_type,pvch_label_name,pvch_child_control_name,pvch_child_label_name,PVCH_CHILD_CONTROL_TYPE,INT_DELETED_FLAG,PVCH_PRNT_CHILD_LABEL_NAME,APIParam,MapControl,EndPoint,Db_Table_Name,ValueFiled,DisplayFiled) VALUES" + sqlquery + "";
                    var cfaffectedRows = await Connection.ExecuteAsync(cfsql);
                    sqlquery = "";
                }
                else
                {
                    #endregion
                    string sqlcol = String.Join(",", objs.Select(x => x.name).ToArray());
                    //string Textvalue = "";
                    //string Text = "";
                    if (status == "1")
                    {
                        DateTime d = DateTime.Now;
                        string dateString = d.ToString("yyyyMMddHHmmss");
                        string Refid = "R" + d.ToString("yyMMddHHmmss");
                        foreach (var ob in objs)
                        {
                            if (ob.MultipleText != null)
                            {
                                if (ob.title.Contains("'"))
                                {
                                    ob.title = ob.title.Replace("'", "");
                                }
                                List<MultiText> obm = ob.MultipleText;

                                string strname = String.Join(",", obm.Select(x => x.Mtext).ToArray());
                                string strtitle = String.Join(",", obm.Select(x => x.title).ToArray());

                                string tablename = "";
                                if (ob.type == "panel")
                                { tablename = "DF_" + Id + "_" + ob.Panel[0].name; }
                                else
                                {
                                    tablename = Tbl_Name;
                                    //"DF_" + Id + "_" + ob.name; 
                                }
                                sqlchild = sqlchild + String.Join(",", obm.Select(x => x.Mtext + ' ' + "VarChar(256)").ToArray());
                                string msql = " CREATE TABLE " + tablename + " (INTID INT,REFID VarChar(50), " + sqlchild + ")";
                                var MaffectedRows = await Connection.ExecuteAsync(msql);
                                //string msql_data = "ALTER TABLE " + tablename + " ADD ( CONSTRAINT " + tablename + "_pk PRIMARY KEY(INTID)) ";
                                string msql_data = "ALTER TABLE " + tablename + "CHANGE COLUMN `INTID` `INTID` INT(10) NOT NULL AUTO_INCREMENT ,ADD PRIMARY KEY(`INTID`)";
                                //var maffectedRows_Alt = await Connection.ExecuteAsync(msql_data);
                                //string msql_SEQ = " CREATE SEQUENCE " + tablename + "_seq START WITH 1";
                                //var maffectedRows_Seq = await Connection.ExecuteAsync(msql_SEQ);
                                //string msql_TRG = " create or replace TRIGGER " + tablename + "_TRG BEFORE INSERT ON " + tablename + " FOR EACH ROW BEGIN BEGIN  IF INSERTING AND :NEW.INTID IS NULL THEN  SELECT " + tablename + "_seq.NEXTVAL INTO :NEW.INTID FROM SYS.DUAL;   END IF; END COLUMN_SEQUENCES;  END; ";
                                //var maffectedRows_Trg = await Connection.ExecuteAsync(msql_TRG);
                                //string childsql = " INSERT INTO DF_" + fname + "_" + ob.name + " (REFID," + Text + ") VALUES('" + Refid + "'," + Textvalue + ")";
                                //var maffectedRows = await Connection.ExecuteAsync(childsql);
                                if (ob.type == "panel")
                                {
                                    string refsql = " (" + Id + ",'" + tablename + "','" + ob.Panel[0].name + "')";
                                    var refaffectedRows = await Connection.ExecuteAsync(refsql);
                                    if (sqlquery == "")
                                    {
                                        sqlquery = "(" + Id + ", '" + ob.name + "', '" + ob.type + "', '" + ob.title + "', null,null,'', 0,'','','','')";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + ",(" + Id + ", '" + ob.name + "', '" + ob.type + "', '" + ob.title + "', null,null, '',0,'','','','')";
                                    }
                                    //sqlquery = sqlquery + String.Join(" ", "INTO DF_FORM_CONFIG(int_form_id,pvch_control_name,pvch_control_type,pvch_label_name) VALUES(" + Id + ",'" + ob.name + "','" + ob.type + "','" + ob.title + "','','',0)");
                                    refsql = "";
                                }
                                else
                                {
                                    string refsql = " INSERT INTO DF_FORM_REFERENCE (FORMID,TABLENAME,COLUMNNAME) VALUES(" + Id + ",'" + tablename + "','" + ob.name + "')";
                                    var refaffectedRows = await Connection.ExecuteAsync(refsql);
                                    if (sqlquery == "")
                                    {
                                        sqlquery = "(" + Id + ",'" + ob.name + "','Child','" + ob.title + "','" + strname + "','" + strtitle + "','',0,'','" + ob.DbTable_Name + "','" + ob.ValueFiled + "','" + ob.DisplayFiled + "')";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + ",(" + Id + ",'" + ob.name + "','Child','" + ob.title + "','" + strname + "','" + strtitle + "','',0,'','" + ob.DbTable_Name + "','" + ob.ValueFiled + "','" + ob.DisplayFiled + "')";
                                    }
                                    //sqlquery = sqlquery + String.Join(" ", "INTO DF_FORM_CONFIG(int_form_id,pvch_control_name,pvch_control_type,pvch_label_name,pvch_child_control_name,pvch_child_label_name) VALUES(" + Id + ",'" + ob.name + "','Child','" + ob.title + "','" + strname + "','" + strtitle + "',0)");
                                    refsql = "";
                                }
                                sqlchild = "";
                                //Textvalue = "";
                                //Text = "";
                            }
                            else
                            {
                                if (ob.type == "multipletext")
                                {
                                    string strname = "";
                                    string strtitle = "";
                                    if (ob.MultipleText != null)
                                    {
                                        List<MultiText> obm = ob.MultipleText;
                                        strname = String.Join(",", obm.Select(x => x.Mtext).ToArray());
                                        strtitle = String.Join(",", obm.Select(x => x.title).ToArray());
                                    }
                                    if (sqlquery == "")
                                    {
                                        sqlquery = "(" + Id + ",'" + ob.name + "','Child','" + ob.title + "','" + strname + "','" + strtitle + "','',0,'','" + ob.DbTable_Name + "','" + ob.ValueFiled + "','" + ob.DisplayFiled + "')";
                                    }
                                    else
                                    {
                                        sqlquery = sqlquery + ",(" + Id + ",'" + ob.name + "','Child','" + ob.title + "','" + strname + "','" + strtitle + "','',0,'','" + ob.DbTable_Name + "','" + ob.ValueFiled + "','" + ob.DisplayFiled + "')";
                                    }
                                    //sqlquery = sqlquery + "INTO DF_FORM_CONFIG(int_form_id,pvch_control_name,pvch_control_type,pvch_label_name,pvch_child_control_name,pvch_child_label_name) VALUES(" + Id + ",'" + ob.name + "','Child','" + ob.title + "','" + strname + "','" + strtitle + "',0)";
                                }

                                //else if (ob.type == "matrixdropdown")
                                //{
                                //    sqlquery = sqlquery + "INTO DF_FORM_CONFIG(int_form_id,pvch_control_name,pvch_control_type,pvch_label_name) VALUES(" + Id + ",'" + ob.name + "','" + ob.type + "','" + ob.Prows + "')";
                                //}
                                //else
                                //{
                                //    sqlquery = sqlquery + "INTO DF_FORM_CONFIG(int_form_id,pvch_control_name,pvch_control_type,pvch_label_name) VALUES(" + Id + ",'" + ob.name + "','" + ob.type + "','" + ob.title + "')";
                                //}
                            }
                            if (ob.Panel != null)
                            {
                                List<PanelData> obm = ob.Panel;
                                string strname = String.Join(",", obm.Select(x => x.name).ToArray());
                                sqlbody = sqlbody + "," + String.Join(",", obm.Select(x => x.name.Replace(" ", "") + ' ' + "VarChar(256)").ToArray());
                                string strtitle = String.Join(",", obm.Select(x => x.title).ToArray());
                                //sqlquery = sqlquery + "INTO DF_FORM_CONFIG(int_form_id,pvch_control_name,pvch_control_type,pvch_label_name) VALUES(" + Id + ",'" + ob.name + "','"+ob.type+"','" + ob.title + "')";

                                for (int i = 0; i < obm.Count; i++)
                                {
                                    if (obm[i].title.Contains("'"))
                                    {
                                        obm[i].title = ob.title.Replace("'", "");
                                    }
                                    if (obm[i].type == "multipletext" && obm[i].type == "matrixdropdown")
                                    {
                                        string multipletextname = "";
                                        string multipletexttitle = "";
                                        if (ob.MultipleText != null)
                                        {
                                            List<MultiText> obt = ob.MultipleText;
                                            multipletextname = String.Join(",", obt.Select(x => x.Mtext).ToArray());
                                            multipletexttitle = String.Join(",", obt.Select(x => x.title).ToArray());
                                        }
                                        if (sqlquery == "")
                                        {
                                            sqlquery = "(" + Id + ",'" + obm[i].name + "','Child','" + obm[i].title + "','" + multipletextname + "','" + multipletexttitle + "','',0,'','','','')";
                                        }
                                        else
                                        {
                                            sqlquery = sqlquery + ",(" + Id + ",'" + obm[i].name + "','Child','" + obm[i].title + "','" + multipletextname + "','" + multipletexttitle + "','',0,'','','','')";
                                        }
                                        //sqlquery = sqlquery + "INTO DF_FORM_CONFIG(int_form_id,pvch_control_name,pvch_control_type,pvch_label_name,pvch_child_control_name,pvch_child_label_name) VALUES(" + Id + ",'" + obm[i].name + "','Child','" + obm[i].title + "','" + multipletextname + "','" + multipletexttitle + "',0)";
                                    }
                                    else
                                    {
                                        if (sqlquery == "")
                                        {
                                            sqlquery = "(" + Id + ",'" + obm[i].name + "','" + obm[i].type + "','" + obm[i].title + "',null,null,'',0,'','','','')";
                                        }
                                        else
                                        {
                                            sqlquery = sqlquery + ",(" + Id + ",'" + obm[i].name + "','" + obm[i].type + "','" + obm[i].title + "',null,null,'',0,'','','','')";
                                        }
                                        //sqlquery = sqlquery + "INTO DF_FORM_CONFIG(int_form_id,pvch_control_name,pvch_control_type,pvch_label_name) VALUES(" + Id + ",'" + obm[i].name + "','" + obm[i].type + "','" + obm[i].title + "','','',0)";
                                    }
                                }
                            }
                            else if (ob.matrixdynamicData != null)
                            {
                                List<matrixdynamicData> obm = ob.matrixdynamicData;
                                var tablename = Tbl_Name + "_" + ob.name;
                                sqlchild = String.Join(",", obm.Select(x => x.name.Replace(" ", "") + ' ' + "VarChar(256)").ToArray());
                                string msql = " CREATE TABLE " + tablename + " (INTID INT IDENTITY(1,1) NOT NULL,APPLICANT_NO VarChar(50), " + sqlchild + ")";
                                var MaffectedRows = await Connection.ExecuteAsync(msql);
                                string strname = String.Join(",", obm.Select(x => x.name).ToArray());
                                //sqlbody = sqlbody + "," + String.Join(",", obm.Select(x => x.name.Replace(" ", "") + ' ' + "Varchar(256)").ToArray());

                                for (int i = 0; i < obm.Count; i++)
                                {

                                    sqlquery = sqlquery + "," + "(" + Id + ",'" + obm[i].name + "','Child','" + obm[i].title + "','" + obm[i].name + "','" + obm[i].title + "','" + obm[i].cellType + "',0,'" + ob.name + "','','','','','','')";

                                }
                                string rfsql = " INSERT INTO DF_FORM_REFERENCE (FORMID,TABLENAME,COLUMNNAME) VALUES(" + Id + ",'" + tablename + "','Child')";
                                var rfaffectedRows = Connection.ExecuteAsync(rfsql);
                            }
                            else if (ob.type == "matrixdropdown")
                            {
                                List<matrixdynamicData> obm = ob.matrixdropdownData;
                                var tablename = Tbl_Name + "_" + ob.name;
                                sqlchild = sqlchild + String.Join(",", obm.Select(x => x.name.Replace(" ", "").Replace("(", "_").Replace(")", "").Replace(".", "_") + ' ' + "VarChar(256)").ToArray());
                                string msql = " CREATE TABLE " + tablename + " (INTID INT IDENTITY(1,1) NOT NULL,REFID VarChar(50),ROWTEXT VarChar(500), " + sqlchild + ")";
                                var MaffectedRows = await Connection.ExecuteAsync(msql);
                                sqlchild = "";
                                string strname = String.Join(",", obm.Select(x => x.name).ToArray());
                                //sqlbody = sqlbody + "," + String.Join(",", obm.Select(x => x.name.Replace(" ", "") + ' ' + "Varchar(256)").ToArray());

                                for (int i = 0; i < obm.Count; i++)
                                {

                                    sqlquery = sqlquery + "," + "(" + Id + ",'" + obm[i].name + "','Child','" + obm[i].title + "','" + obm[i].name + "','" + obm[i].title + "','" + obm[i].cellType + "',0,'" + ob.name + "','','','','','','')";

                                }
                            }
                        }
                        if (FormType != "API Type")
                        {
                            sql = " CREATE TABLE " + Tbl_Name + " (INTID INT IDENTITY(1,1) NOT NULL,APPLICANT_NO VarChar(50), " + sqlbody + ",REFID VarChar(50),INTCREATEDBY INT,INTUPDATEDBY INT,DTMCREATEDON date DEFAULT getdate(),DELETEDFLAG INT DEFAULT 0,STATUS INT DEFAULT 0,PENDINGWITHUSER INT,PENDINGWITHDESG INT,CSTATUS  VarChar(20),REMARK  VarChar(500),APPROVESTATUS INT DEFAULT 0,APPROVEDDATE date NULL)";
                            var affectedRows = await Connection.ExecuteAsync(sql);

                            sql_data = "ALTER TABLE " + Tbl_Name + " ADD PRIMARY KEY (INTID)";
                            var affectedRows_Alt = await Connection.ExecuteAsync(sql_data);

                            string rfsql = " INSERT INTO DF_FORM_REFERENCE (FORMID,TABLENAME,COLUMNNAME) VALUES(" + Id + ",'" + Tbl_Name + "','Main')";
                            var rfaffectedRows = await Connection.ExecuteAsync(rfsql);
                        }
                        string cfsql = "INSERT INTO DF_FORM_CONFIG(int_form_id,pvch_control_name,pvch_control_type,pvch_label_name,pvch_child_control_name,pvch_child_label_name,PVCH_CHILD_CONTROL_TYPE,INT_DELETED_FLAG,PVCH_PRNT_CHILD_LABEL_NAME,APIParam,MapControl,EndPoint) VALUES" + sqlquery + "";
                        var cfaffectedRows = await Connection.ExecuteAsync(cfsql);
                    }
                }
                //IEnumerable<SuccessMessage> SuccessMessages;

                var dyParam = new DynamicParameters();
                dyParam.Add("P_Action", "A");
                dyParam.Add("P_INTID", Id);
                dyParam.Add("P_REMARK", remark);
                dyParam.Add("P_STATUS", status);
                dyParam.Add("@P_Msg", SqlDbType.VarChar, direction: ParameterDirection.Output, size: 5215585);
                dyParam.Add("@P_Msg_Out", SqlDbType.VarChar, direction: ParameterDirection.Output, size: 5215585);
                var query = "USP_DYNAMIC_TABLE";
                var msg = await Connection.QueryAsync<int>(query, dyParam, commandType: CommandType.StoredProcedure);

                //SuccessMessages = Connection.Query<SuccessMessage>(query, dyParam, commandType: CommandType.StoredProcedure);
                string result = "1";
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            };
        }
        public async Task<IEnumerable<FormModel>> GetFormName()
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@Action", "XX");
                p.Add("PMSGOUT", dbType: DbType.String, direction: ParameterDirection.Output, size: 5215585);
                var results = await Connection.QueryAsync<FormModel>("USP_DF_CONTROL_CONFIG", p, commandType: CommandType.StoredProcedure);
                return results;
            }
            catch
            {
                throw;
            };
        }
        public async Task<IEnumerable<Domain.WFERender.Dformconfig>> GetColumnData(int FormId)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@Action", "D");
                p.Add("@P_FORMID", FormId);
                p.Add("PMSGOUT", dbType: DbType.String, direction: ParameterDirection.Output, size: 5215585);
                var results = await Connection.QueryAsync<Domain.WFERender.Dformconfig>("USP_DF_CONTROL_CONFIG", p, commandType: CommandType.StoredProcedure);
                return results;
            }
            catch
            {
                throw;
            };
        }
        public string InsertColumnconfigData(Domain.WFERender.Dformconfig component)
        {
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("@Action", "FC");
                dyParam.Add("P_FORMID", component.FormId);
                dyParam.Add("P_XML_DATA", component.ColumnXml);
                dyParam.Add("PMSGOUT", dbType: DbType.String, direction: ParameterDirection.Output, size: 5215585);
                var query = "USP_DF_CONTROL_CONFIG";
                Connection.Execute(query, dyParam, commandType: CommandType.StoredProcedure);
                var result = dyParam.Get<string>("PMSGOUT");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return strOutput;
        }
        public async Task<IEnumerable<Domain.WFERender.Dformconfig>> GetConstraintData(int FormId)
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@Action", "C");
                p.Add("@P_FORMID", FormId);
                p.Add("PMSGOUT", dbType: DbType.String, direction: ParameterDirection.Output, size: 5215585);
                var results = await Connection.QueryAsync<Domain.WFERender.Dformconfig>("USP_DF_CONTROL_CONFIG", p, commandType: CommandType.StoredProcedure);
                return results;
            }
            catch
            {
                throw;
            };
        }
        public string InsertConstraintConfigData(Domain.WFERender.Dformconfig component)
        {
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("@Action", "CC");
                dyParam.Add("P_FORMID", component.FormId);
                dyParam.Add("P_XML_DATA", component.ColumnXml);
                dyParam.Add("PMSGOUT", dbType: DbType.String, direction: ParameterDirection.Output, size: 5215585);
                var query = "USP_DF_CONTROL_CONFIG";
                Connection.Execute(query, dyParam, commandType: CommandType.StoredProcedure);
                var result = dyParam.Get<string>("PMSGOUT");
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return strOutput;
        }
        public DataSet GetTakeActionData(int Formid, string Applicant_No)
        {
            try
            {

                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                var dyParam = new DynamicParameters();
                string Data = "select sch.TABLE_NAME,dfc.PVCH_CONTROL_NAME,dfc.PVCH_CONTROL_TYPE,dfc.PVCH_LABEL_NAME from DF_FORM df inner join LGD_TBL_SCHEME sch on df.SCHEMEID=sch.ID inner join DF_FORM_CONFIG dfc on df.ID=dfc.INT_FORM_ID where df.ID=" + Formid + " and PVCH_CONTROL_TYPE!='Child'";
                IDataReader dr = Connection.ExecuteReader(Data);
                dt.Load(dr);
                if (dt.Rows.Count > 0)
                {
                    var column = "";
                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        if (column == "")
                        {
                            column = dt.Rows[j]["PVCH_CONTROL_NAME"].ToString().Replace(" ", "") + " as [" + dt.Rows[j]["PVCH_LABEL_NAME"] + "|" + dt.Rows[j]["PVCH_CONTROL_TYPE"] + "|" + dt.Rows[j]["PVCH_CONTROL_NAME"].ToString().Replace(" ", "") + "]";
                        }
                        else
                        {
                            column = column + "," + dt.Rows[j]["PVCH_CONTROL_NAME"].ToString().Replace(" ", "") + " as [" + dt.Rows[j]["PVCH_LABEL_NAME"] + "|" + dt.Rows[j]["PVCH_CONTROL_TYPE"] + "|" + dt.Rows[j]["PVCH_CONTROL_NAME"].ToString().Replace(" ", "") + "]";
                        }
                    }
                    DataTable Mdt = new DataTable();
                    string M_Data = "select " + column + " from " + dt.Rows[0]["TABLE_NAME"] + " where Applicant_No='" + Applicant_No + "'";
                    IDataReader Mdrtbl = Connection.ExecuteReader(M_Data);
                    Mdt.Load(Mdrtbl);
                    ds.Tables.Add(Mdt);
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        if (dt.Rows[i]["PVCH_CONTROL_TYPE"].ToString() == "matrixdynamic")
                        {
                            var columndata = "";
                            DataTable chlddt = new DataTable();
                            string chl_Data = "select PVCH_CHILD_CONTROL_NAME,PVCH_CHILD_LABEL_NAME,PVCH_CHILD_CONTROL_TYPE from DF_FORM_CONFIG where INT_FORM_ID='" + Formid + "' and PVCH_PRNT_CHILD_LABEL_NAME='" + dt.Rows[i]["PVCH_CONTROL_NAME"] + "'";
                            IDataReader chdrtbl = Connection.ExecuteReader(chl_Data);
                            chlddt.Load(chdrtbl);
                            if (chlddt.Rows.Count > 0)
                            {
                                for (int p = 0; p < chlddt.Rows.Count; p++)
                                {
                                    if (columndata == "")
                                    {
                                        columndata = chlddt.Rows[p]["PVCH_CHILD_CONTROL_NAME"].ToString().Replace(" ", "") + " as [" + chlddt.Rows[p]["PVCH_CHILD_LABEL_NAME"] + "|" + chlddt.Rows[p]["PVCH_CHILD_CONTROL_TYPE"] + "]";
                                    }
                                    else
                                    {
                                        columndata = columndata + "," + chlddt.Rows[p]["PVCH_CHILD_CONTROL_NAME"].ToString().Replace(" ", "") + " as [" + chlddt.Rows[p]["PVCH_CHILD_LABEL_NAME"] + "|" + chlddt.Rows[p]["PVCH_CHILD_CONTROL_TYPE"] + "]";
                                    }
                                }
                            }

                            string Tbl_Name = dt.Rows[i]["TABLE_NAME"] + "_" + dt.Rows[i]["PVCH_CONTROL_NAME"];
                            DataTable tbldt = new DataTable();
                            string Tbl_Data = "select (select PVCH_LABEL_NAME+'|" + dt.Rows[i]["PVCH_CONTROL_NAME"] + "' from DF_FORM_CONFIG where INT_FORM_ID='" + Formid + "' and PVCH_CONTROL_TYPE='matrixdynamic' and PVCH_CONTROL_NAME='" + dt.Rows[i]["PVCH_CONTROL_NAME"] + "') PVCH_LABEL_NAME," + columndata + " from " + Tbl_Name + " where Applicant_No='" + Applicant_No + "'";
                            IDataReader drtbl = Connection.ExecuteReader(Tbl_Data);
                            tbldt.Load(drtbl);
                            tbldt.TableName = "Tbl_" + dt.Rows[i]["PVCH_CONTROL_NAME"];
                            ds.Tables.Add(tbldt);
                        }
                    }
                }
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetDropdownData(string Value_Field, string Display_Field, string Table_Name, string listkey)
        {
            try
            {
                DataTable dt = new DataTable();
                var dyParam = new DynamicParameters();
                string Data = "";
                if (listkey == "")
                {
                    Data = "select " + Value_Field + "," + Display_Field + " from " + Table_Name + "";
                }
                else
                {
                    Data = "select " + Value_Field + "," + Display_Field + " from " + Table_Name + " where " + listkey + "";
                }
                IDataReader dr = Connection.ExecuteReader(Data);
                dt.Load(dr);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }

}
