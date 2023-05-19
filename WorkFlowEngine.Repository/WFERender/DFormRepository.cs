using Dapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.WFERender;
using WorkFlowEngine.IRepository.DapperConfiguration;
using WorkFlowEngine.IRepository.WFERender;
using WorkFlowEngine.Repository.DapperConfiguration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace WorkFlowEngine.Repository.WFERender
{
    public class DFormRepository : RepositoryBase, IDFormRepository
    {
        //private readonly IHttpContextAccessor httpContextAccessor;
        public DFormRepository(IConnectionFactory connectionFactory/*, IHttpContextAccessor httpContext*/) : base(connectionFactory)
        {
            // httpContextAccessor = httpContext;
        }
        public async Task<string> FillDataTable_New(int Formid, string ApplicantNo)
        {
            string strHtmlFinal = "";
            DataTable PnlDt = new DataTable();
            string Check = "select distinct int_id,pvch_control_name,replace(replace(replace(pvch_label_name,'\"',''),'[',''),']','') as pvch_label_name,PVCH_CONTROL_TYPE,PVCH_CHILD_CONTROL_NAME,PVCH_CHILD_LABEL_NAME,PVCH_CHILD_CONTROL_TYPE,PVCH_PRNT_CHILD_LABEL_NAME,(select S.FORMTYPE from [dbo].[LGD_TBL_SCHEME] S inner join DF_FORM D on S.ID=D.SCHEMEID where D.ID=" + Formid + ") FORMTYPE,APIParam,MapControl,EndPoint,(select S.TABLE_NAME from [dbo].[LGD_TBL_SCHEME] S inner join DF_FORM D on S.ID=D.SCHEMEID where D.ID=" + Formid + ") TABLE_NAME from DF_FORM_CONFIG DF  WHERE INT_FORM_ID=" + Formid + " and int_deleted_flag=0 and PVCH_CONTROL_TYPE!='Child' and pvch_label_name is not null order by int_id";
            IDataReader dr = await Connection.ExecuteReaderAsync(Check);
            PnlDt.Load(dr);
            string strHtmls = GetHTML_New(Formid, ApplicantNo, PnlDt);
            strHtmlFinal = strHtmlFinal + "<div class='sectionPanel'><div class='row'></div><div class='row'>" + strHtmls.Split('|')[0] + "</div></div>";
            string finalrawdata = strHtmlFinal + "|" + strHtmls.Split('|')[1] + "|" + strHtmls.Split('|')[2] + "|" + strHtmls.Split('|')[3] + "|" + strHtmls.Split('|')[4] + "|" + strHtmls.Split('|')[5] + "|" + strHtmls.Split('|')[6] + "|" + strHtmls.Split('|')[7] + "|" + PnlDt.Rows[0]["FORMTYPE"] + "|" + PnlDt.Rows[0]["APIParam"] + "|" + PnlDt.Rows[0]["MapControl"] + "|" + PnlDt.Rows[0]["EndPoint"] + "|" + strHtmls.Split('|')[8];
            return finalrawdata;
        }
        public string GetHTML_New(int FormId, string ApplicantNo, DataTable dt)
        {
            string intAllignment = "";
            string css = "";
            string csslink = "#";
            string scriptfile = "#";
            string strHtml = "";
            string strReq = "1";
            string lebeltext = "";
            string controlText = "";
            string strRow = "";
            string strGroupDiv = "";//";
            string expressionscript = "";
            string strColumn = "";
            string strColumnId = "";
            string strColumntype = "";
            string CstrRow = "";
            string Script = "";
            string RequiredCheckValue = "";
            string RequiredKeyWord = "";
            bool IsFirstIf = true;
            string fname = "";
            string BindScript = "";
            string ValidationScript = "";
            string FileCntrlScript = "";
            string MatrixDynamicScript = "";
            #region to get templateid and css            
            var Allignment = GetColumnid(FormId).Result.ToList();
            intAllignment = Allignment[0].COLUMNID.ToString();
            if (Allignment[0].CSS_FILE != "Choose file")
            {
                css = Allignment[0].CSS_FILE;
                //csslink= "<link href='~/TemplateDocuments/"+ css +"' rel='stylesheet'/>";                
                csslink = css;
            }

            #endregion
            Forms scheme = GetSchemeAndFormDetailsByFormId(FormId).Result;
            if (scheme != null && (scheme.SCRIPT_FILE != null || scheme.SCRIPT_FILE != ""))
            {
                scriptfile = scheme.SCRIPT_FILE;
            }
            #region To get all the properties of the form
            string JsonString = GetDFFormResult(FormId).JSONSTRING;
            JObject o = JObject.Parse(JsonString);
            List<Element> objs = new List<Element>();
            var elementsP1 = o.SelectToken("pages[0].elements", false); //pages[0].elements[0].type

            foreach (var item in elementsP1.Children())
            {
                var element = new Element();
                element.name = (string)item.SelectToken("name", false);
                element.type = (string)item.SelectToken("type", false);
                element.title = (string)item.SelectToken("title", false);
                element.isRequired = (string)item.SelectToken("isRequired", false);
                element.readOnly = (string)item.SelectToken("readOnly", false);
                element.expression = (string)item.SelectToken("expression", false);
                element.visibleIf = (string)item.SelectToken("visibleIf", false);
                element.inputType = (string)item.SelectToken("inputType", false);
                element.acceptedTypes = (string)item.SelectToken("acceptedTypes", false);
                element.maxSize = (string)item.SelectToken("maxSize", false);
                element.CascadeControlName = (string)item.SelectToken("CascadeControlName", false);
                element.DepandantControlName = (string)item.SelectToken("DepandantControlName", false);
                element.ApiUrl = (string)item.SelectToken("ApiUrl", false);
                element.ValueFiled = (string)item.SelectToken("ValueFiled", false);
                element.DisplayFiled = (string)item.SelectToken("DisplayFiled", false);
                element.DbTableName = (string)item.SelectToken("DbTableName", false);
                element.Condition = (string)item.SelectToken("Condition", false);
                element.ApiType = (string)item.SelectToken("InternalApi", false);
                element.ApiParameter = (string)item.SelectToken("ApiParameter", false);
                element.ValidationFiled = (string)item.SelectToken("ValidationFiled", false);
                var choicesByUrl = new choicesByUrl();
                choicesByUrl.url = (string)item.SelectToken("choicesByUrl.url", false);
                choicesByUrl.titleName = (string)item.SelectToken("choicesByUrl.titleName", false);
                choicesByUrl.valueName = (string)item.SelectToken("choicesByUrl.valueName", false);
                choicesByUrl.path = (string)item.SelectToken("choicesByUrl.path", false);
                choicesByUrl.type = "POST";
                element.choicesByUrl = choicesByUrl;
                var elementsP12 = item.SelectToken("choices[0]");
                if (element.type != "matrixdynamic" && elementsP12 != null)
                {
                    if (element.choicesByUrl.url == null)
                    {
                        element.choices = JsonConvert.DeserializeObject<List<choices>>(elementsP12.Parent.ToString());
                    }
                }
                var mdynamic = item.SelectToken("columns");
                if (mdynamic != null)
                {
                    element.matrixdynamicData = JsonConvert.DeserializeObject<List<matrixdynamicData>>(mdynamic.ToString());
                }
                element.calltype = (string)item.SelectToken("calltype", false);
                element.QueryString = (string)item.SelectToken("QueryString", false);
                element.Session = (string)item.SelectToken("Session", false);
                element.html = (string)item.SelectToken("html", false);
                if (element.type == "matrixdynamic")
                {
                    var MatrixdynamicData = new List<matrixdynamicData>();
                    var json = item.SelectToken("columns", false);
                    foreach (var c in json.Children())
                    {
                        matrixdynamicData mt = new matrixdynamicData();
                        mt.name = (string)c.SelectToken("name", false);
                        mt.title = (string)c.SelectToken("title", false);
                        mt.cellType = (string)c.SelectToken("cellType", false);
                        if (mt.cellType == "dropdown" || mt.cellType == "checkbox" || mt.cellType == "radiogroup")
                        {
                            //mt.matrixdropdownData = (string)c.SelectToken("choices", false); ;
                            var MatrixdropdownData = new List<choices>();
                            var Rjson = c.SelectToken("choices", false);
                            foreach (var p in Rjson.Children())
                            {
                                choices cmt = new choices();
                                cmt.text = (string)p.SelectToken("text", false);
                                cmt.value = (string)p.SelectToken("value", false);
                                MatrixdropdownData.Add(cmt);
                            }
                            mt.matrixdropdownData = MatrixdropdownData;
                        }
                        MatrixdynamicData.Add(mt);
                    }
                    element.ColmatrixdropdownData = MatrixdynamicData;
                }
                objs.Add(element);
            }


            #endregion
            int counter = 0;
            for (int k = 0; k < dt.Rows.Count; k++)
            {
                var element = new Element();

                element = objs.Where(e => e.name.Replace(" ", "") == dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "")).FirstOrDefault();

                if (strColumn == "")
                {
                    strColumn = dt.Rows[k]["pvch_control_name"].ToString();
                }
                else
                {
                    strColumn = strColumn + "," + dt.Rows[k]["pvch_control_name"].ToString();
                }
                if (strColumnId == "")
                {
                    strColumnId = "" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "");
                }
                else
                {
                    strColumnId = strColumnId + "," + "" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "");
                }
                if (strColumntype == "")
                {
                    strColumntype = dt.Rows[k]["pvch_control_type"].ToString();
                }
                else
                {
                    strColumntype = strColumntype + "," + dt.Rows[k]["pvch_control_type"].ToString();
                }

                if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() != "Declaration")
                {
                    if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() != "SameAs")
                    {
                        strRow = "";
                        string colname = dt.Rows[k]["pvch_control_name"].ToString();
                        //DataTable dtt = FindColumnData(FormId, ApplicantNo);
                        DataTable dtt = FindColumnData(FormId);
                        if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() != "panel" && dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() != "matrixdynamic" && dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() != "Child")
                        {

                            if (dt.Rows[k]["pvch_label_name"].ToString() != "" && dt.Rows[k]["pvch_label_name"].ToString() != null)
                            {
                                if (element.isRequired == "True")
                                {
                                    lebeltext = dt.Rows[k]["pvch_label_name"].ToString() + "<span class='text-danger'>*</span>";
                                }
                                else
                                {
                                    lebeltext = dt.Rows[k]["pvch_label_name"].ToString();
                                }

                            }
                            else
                            {
                                if (element.isRequired == "True")
                                {
                                    lebeltext = dt.Rows[k]["pvch_control_name"].ToString() + "<span class='text-danger'>*</span>";
                                }
                                else
                                {
                                    lebeltext = dt.Rows[k]["pvch_label_name"].ToString();
                                }
                            }
                        }

                        #region Building the html for all the inputs

                        if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "text")
                        {
                            RequiredCheckValue = "";
                            RequiredKeyWord = "Enter";
                            string QSvalue = "";
                            if (element.QueryString != null)
                            {
                                //QSvalue = GetSessionData(element.QueryString);
                            }
                            if (element.Session != null)
                            {
                            }
                            if (ApplicantNo != null)
                            {
                                string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), element.Condition);
                                if (QSvalue != "")
                                { Value = QSvalue; }
                                if (element.inputType == "date")
                                {
                                    controlText = "<input type='text' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='" + Value + "' class='form-control datepicker' name=" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "aria-label='Default' aria-describedby='inputGroup-sizing-default' autocomplete='off'><div class='input-group-append'><span class='input-group-text' id='inputGroup-sizing-default'><i class='icon-calendar1'></i></span><span id='Clear' value='Clear dates' class='input-group-text'><i class='fa fa-paint-brush Clear_" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "'></i></span></div>";
                                }
                                else
                                {
                                    controlText = "<input type='text' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='" + Value + "' class='form-control' name=" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + ">";
                                }
                            }
                            else if (element.readOnly == "True")
                            {
                                controlText = "<input type='text' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='" + QSvalue + "' class='form-control' name=" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + " readonly>";
                            }
                            else if (element.inputType == "date")
                            {
                                controlText = "<input type='text' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='' class='form-control datepicker' name=" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "aria-label='Default' aria-describedby='inputGroup-sizing-default' autocomplete='off'><div class='input-group-append'><span class='input-group-text' id='inputGroup-sizing-default'><i class='icon-calendar1'></i></span><span id='Clear' value='Clear dates' class='input-group-text'><i class='fa fa-paint-brush Clear_" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "'></i></span></div>";
                            }
                            else
                            {
                                controlText = "<input type='text' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='" + QSvalue + "' class='form-control' name=" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + ">";
                            }
                        }
                        else if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "number")
                        {
                            RequiredCheckValue = "";
                            RequiredKeyWord = "Enter";
                            if (ApplicantNo != null)
                            {
                                string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), element.Condition);
                                controlText = "<input type='number' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='" + Value + "' class='form-control' name=" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + ">";
                            }
                            else
                            {
                                controlText = "<input type='number' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='' class='form-control' name=" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + ">";
                            }
                        }
                        else if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "expression")
                        {
                            RequiredCheckValue = "";
                            RequiredKeyWord = "Enter";
                            if (ApplicantNo != null)
                            {
                                string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), element.Condition);
                                controlText = "<input type='text' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='" + Value + "' class='form-control' name=" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + " readonly>";
                            }
                            else
                            {
                                if (element.expression != null)
                                {
                                    string strExpp = element.expression.Replace("{", "parseFloat($('#").Replace("}", "').val())");
                                    string[] strexp = element.expression.Replace("{", "").Replace(" ", "").Replace("}", "").Split('+', '-', '/', '*');
                                    string resultfield = element.name;
                                    if (expressionscript == "#") { expressionscript = ""; };
                                    foreach (var item in strexp)
                                    {
                                        expressionscript = expressionscript + "$('#" + item + "').change(function(){$('#" + resultfield + "').val((" + strExpp + ").toFixed(2))});";
                                    }
                                }
                                controlText = "<input type='text' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='' class='form-control' name=" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + " readonly>";
                            }
                        }
                        else if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "dropdown")
                        {
                            RequiredCheckValue = "0";
                            RequiredKeyWord = "Select";
                            if (element.DepandantControlName != null && element.DepandantControlName != "")
                            {
                                if (ApplicantNo != null)
                                {
                                    string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), "");
                                    var Trandata = "";
                                    if (element.Condition.Contains("&"))
                                    {
                                        for (int i = 0; i < element.Condition.Split('&').Length; i++)
                                        {
                                            if (element.Condition.Split('&')[i].Split('=')[1].Contains("@"))
                                            {
                                                if (Trandata == "")
                                                {
                                                    Trandata = element.Condition.Split('&')[i].Split('=')[0] + "=" + "'+$('#" + element.DepandantControlName + "').val()+'";
                                                }
                                                else
                                                {
                                                    Trandata = Trandata + " and" + element.Condition.Split('&')[i].Split('=')[0] + "=" + "'+$('#" + element.DepandantControlName + "').val()+'";
                                                }
                                            }
                                            else
                                            {
                                                if (Trandata == "")
                                                {
                                                    Trandata = element.Condition.Split('&')[i];
                                                }
                                                else
                                                {
                                                    Trandata = Trandata + " and" + element.Condition.Split('&')[i];
                                                }
                                            }

                                        }
                                    }
                                    else
                                    {
                                        var ddldata = "";
                                        if (element.Condition.Split('=')[1].Contains("@"))
                                        {
                                            ddldata = "'+$('#" + element.DepandantControlName + "').val()+'";
                                            Trandata = element.Condition.Split('=')[0] + "=" + ddldata;
                                        }
                                        else
                                        {
                                            Trandata = element.Condition;
                                        }
                                    }
                                    if (element.Condition == "")
                                    {
                                        BindScript = BindScript +
                                                        "$.ajax({ type: 'GET', url: '/Admin/DisplayUserNameByDept?Value_Field=" + element.ValueFiled + "&Display_Field=" + element.DisplayFiled + "&Table_Name=" + element.DbTableName + "&listkey=" + Trandata + "', contentType: 'json'," +
                                                                        " success: function (response) {var result = keysToUppercase(response); var items = '';" +
                                                                        "for (var i = 0; i < result.length; i++) {" +
                                                                        "if(" + Value + "==result[i]." + element.ValueFiled + "){items += '<option selected value=' + result[i]." + element.ValueFiled + " + '>' + result[i]." + element.DisplayFiled + " + '</option>';}" +
                                               " else {items += '<option value=' + result[i]." + element.ValueFiled + " + '>' + result[i]." + element.DisplayFiled + " + '</option>';}}" +
                                               "$('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').html(items);} });" +
                                               " $('#" + element.DepandantControlName + "').change(function () {" +
                                                        "$.ajax({ type: 'GET', url: '/Admin/DisplayUserNameByDept?Value_Field=" + element.ValueFiled + "&Display_Field=" + element.DisplayFiled + "&Table_Name=" + element.DbTableName + "&listkey=" + Trandata + "', contentType: 'json'," +
                                                                        " success: function (response) {var result = keysToUppercase(response); var items = '';" +
                                                                        "for (var i = 0; i < result.length; i++) {" +
                                               "items += '<option value=' + result[i]." + element.ValueFiled + " + '>' + result[i]." + element.DisplayFiled + " + '</option>';}" +
                                               "$('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').html(items);} });});";
                                    }
                                    else
                                    {
                                        BindScript = BindScript +
                                                   "$.ajax({ type: 'GET', url: '/Admin/DisplayUserNameByDept?Value_Field=" + element.ValueFiled + "&Display_Field=" + element.DisplayFiled + "&Table_Name=" + element.DbTableName + "&listkey=" + Trandata + "', contentType: 'json'," +
                                                                   " success: function (response) {var result = keysToUppercase(response); var items = '';" +
                                                                   "for (var i = 0; i < result.length; i++) {" +
                                                                   "if(" + Value + "==result[i]." + element.ValueFiled + "){items += '<option selected value=' + result[i]." + element.ValueFiled + " + '>' + result[i]." + element.DisplayFiled + " + '</option>';}" +
                                          " else {items += '<option value=' + result[i]." + element.ValueFiled + " + '>' + result[i]." + element.DisplayFiled + " + '</option>';}}" +
                                          "$('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').html(items);} });" +
                                          " $('#" + element.DepandantControlName + "').change(function () {" +
                                                        "$.ajax({ type: 'GET', url: '/Admin/DisplayUserNameByDept?Value_Field=" + element.ValueFiled + "&Display_Field=" + element.DisplayFiled + "&Table_Name=" + element.DbTableName + "&listkey=" + Trandata + "', contentType: 'json'," +
                                                                        " success: function (response) {var result = keysToUppercase(response); var items = '';" +
                                                                        "for (var i = 0; i < result.length; i++) {" +
                                               "items += '<option value=' + result[i]." + element.ValueFiled + " + '>' + result[i]." + element.DisplayFiled + " + '</option>';}" +
                                               "$('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').html(items);} });});";
                                    }
                                }
                                else
                                {
                                    if (element.Condition == "")
                                    {
                                        BindScript = BindScript + " $('#" + element.DepandantControlName + "').change(function () {" +
                                                        "$.ajax({ type: 'GET', url: '/Admin/DisplayUserNameByDept?Value_Field=" + element.ValueFiled + "&Display_Field=" + element.DisplayFiled + "&Table_Name=" + element.DbTableName + "', contentType: 'json'," +
                                                                        " success: function (response) {var result = keysToUppercase(response); var items = '';" +
                                                                        "for (var i = 0; i < result.length; i++) {" +
                                               "items += '<option value=' + result[i]." + element.ValueFiled + " + '>' + result[i]." + element.DisplayFiled + " + '</option>';}" +
                                               "$('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').html(items);} });});";
                                    }
                                    else
                                    {
                                        var Trandata = "";
                                        if (element.Condition.Contains("&"))
                                        {
                                            for (int i = 0; i < element.Condition.Split('&').Length; i++)
                                            {
                                                if (element.Condition.Split('&')[i].Split('=')[1].Contains("@"))
                                                {
                                                    if (Trandata == "")
                                                    {
                                                        Trandata = element.Condition.Split('&')[i].Split('=')[0] + "=" + "'+$('#" + element.DepandantControlName + "').val()+'";
                                                    }
                                                    else
                                                    {
                                                        Trandata = Trandata + " and" + element.Condition.Split('&')[i].Split('=')[0] + "=" + "'+$('#" + element.DepandantControlName + "').val()+'";
                                                    }
                                                }
                                                else
                                                {
                                                    if (Trandata == "")
                                                    {
                                                        Trandata = element.Condition.Split('&')[i];
                                                    }
                                                    else
                                                    {
                                                        Trandata = Trandata + " and" + element.Condition.Split('&')[i];
                                                    }
                                                }

                                            }
                                        }
                                        else
                                        {
                                            var ddldata = "";
                                            if (element.Condition.Split('=')[1].Contains("@"))
                                            {
                                                ddldata = "'+$('#" + element.DepandantControlName + "').val()+'";
                                                Trandata = element.Condition.Split('=')[0] + "=" + ddldata;
                                            }
                                            else
                                            {
                                                Trandata = element.Condition;
                                            }

                                        }

                                        BindScript = BindScript + "" +
                                            " $('#" + element.DepandantControlName + "').change(function () {" +
                                                   "$.ajax({ type: 'GET', url: '/Admin/DisplayUserNameByDept?Value_Field=" + element.ValueFiled + "&Display_Field=" + element.DisplayFiled + "&Table_Name=" + element.DbTableName + "&listkey=" + Trandata + "', contentType: 'json'," +
                                                                   " success: function (response) {var result = keysToUppercase(response); var items = '';" +
                                                                   "for (var i = 0; i < result.length; i++) {" +
                                          "items += '<option value=' + result[i]." + element.ValueFiled + " + '>' + result[i]." + element.DisplayFiled + " + '</option>';}" +
                                          "$('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').html(items);} });});";
                                    }
                                }
                            }
                            if (element != null && element.choicesByUrl.url != null)
                            {
                                var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"DynamicFormJson.json");
                                var JSON = System.IO.File.ReadAllText(folderDetails);
                                dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(JSON);
                                if (element.ApiType != null)
                                {
                                    if (element.ApiType.ToUpper() == "TRUE")
                                    {
                                        element.choicesByUrl.url = jsonObj["Client"]["APIURL"].ToString() + element.choicesByUrl.url;
                                    }
                                }

                                if (ApplicantNo != null)
                                {
                                    string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), element.Condition);
                                    string apiResponse = string.Empty;
                                    if (element.calltype == "POST")
                                    {
                                        apiResponse = PostResultAsync(element.choicesByUrl.url).Result;
                                    }
                                    else
                                    {
                                        apiResponse = GetResultAsync(element.choicesByUrl.url).Result;
                                    }
                                    if (apiResponse != null)
                                    {
                                        dynamic dynJson = JsonConvert.DeserializeObject(apiResponse);
                                        if (element.choicesByUrl.path != null)
                                        {
                                            StringBuilder stringBuilder = new StringBuilder();
                                            foreach (var item in dynJson[element.choicesByUrl.path])
                                            {
                                                if (Value == item[element.choicesByUrl.valueName].ToString())
                                                {
                                                    stringBuilder.Append("<option selected value='" + item[element.choicesByUrl.valueName] + "'>" + item[element.choicesByUrl.titleName] + "</option>");
                                                }
                                                else
                                                {
                                                    stringBuilder.Append("<option value='" + item[element.choicesByUrl.valueName] + "'>" + item[element.choicesByUrl.titleName] + "</option>");
                                                }
                                            }

                                            controlText = "<select id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' class='form-control'><option value='0'>Select</option> " + stringBuilder.ToString() + "</select > ";
                                        }
                                        else
                                        {
                                            StringBuilder stringBuilder = new StringBuilder();
                                            foreach (var item in dynJson)
                                            {
                                                if (Value == item[element.choicesByUrl.valueName].ToString())
                                                {
                                                    stringBuilder.Append("<option selected value='" + item[element.choicesByUrl.valueName] + "'>" + item[element.choicesByUrl.titleName] + "</option>");
                                                }
                                                else
                                                {
                                                    stringBuilder.Append("<option value='" + item[element.choicesByUrl.valueName] + "'>" + item[element.choicesByUrl.titleName] + "</option>");
                                                }
                                            }

                                            controlText = "<select id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' class='form-control'><option value='0'>Select</option> " + stringBuilder.ToString() + "</select > ";
                                        }
                                    }
                                    else
                                    {
                                        controlText = "<select id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' class='form-control'><option value='0'>Select</option></select >";
                                    }

                                }
                                else
                                {
                                    string apiResponse = string.Empty;
                                    if (element.calltype == "POST")
                                    {
                                        apiResponse = PostResultAsync(element.choicesByUrl.url).Result;
                                    }
                                    else
                                    {
                                        apiResponse = GetResultAsync(element.choicesByUrl.url).Result;
                                    }
                                    if (apiResponse != null)
                                    {
                                        dynamic dynJson = JsonConvert.DeserializeObject(apiResponse);
                                        if (element.choicesByUrl.path != null)
                                        {
                                            StringBuilder stringBuilder = new StringBuilder();
                                            foreach (var item in dynJson[element.choicesByUrl.path])
                                            {
                                                stringBuilder.Append("<option value='" + item[element.choicesByUrl.valueName] + "'>" + item[element.choicesByUrl.titleName] + "</option>");
                                            }
                                            controlText = "<select id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' class='form-control'><option value='0'>Select</option> " + stringBuilder.ToString() + "</select > ";

                                        }
                                        else
                                        {
                                            StringBuilder stringBuilder = new StringBuilder();
                                            foreach (var item in dynJson)
                                            {
                                                stringBuilder.Append("<option value='" + item[element.choicesByUrl.valueName] + "'>" + item[element.choicesByUrl.titleName] + "</option>");
                                            }
                                            controlText = "<select id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' class='form-control'><option value='0'>Select</option> " + stringBuilder.ToString() + "</select > ";

                                        }
                                    }
                                    else
                                    {
                                        controlText = "<select id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' class='form-control'><option value='0'>Select</option></select >";
                                    }
                                }

                            }
                            else
                            {
                                if (element.DbTableName == "" || element.DbTableName == null)
                                {
                                    if (ApplicantNo != null)
                                    {
                                        string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), element.Condition);
                                        StringBuilder stringBuilder = new StringBuilder();
                                        if (element.choices != null)
                                        {
                                            foreach (choices item in element.choices)
                                            {
                                                if (Value == item.value)
                                                {
                                                    stringBuilder.Append("<option selected value='" + item.value + "'>" + item.text + "</option>");
                                                }
                                                else
                                                {
                                                    stringBuilder.Append("<option value='" + item.value + "'>" + item.text + "</option>");
                                                }
                                            }
                                        }
                                        controlText = "<select id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' class='form-control'><option value='0'>Select</option> " + stringBuilder.ToString() + "</select > ";
                                    }
                                    else
                                    {
                                        StringBuilder stringBuilder = new StringBuilder();
                                        if (element != null && element.choices != null)
                                        {
                                            foreach (choices item in element.choices)
                                            {
                                                stringBuilder.Append("<option value='" + item.value + "'>" + item.text + "</option>");
                                            }
                                        }

                                        controlText = "<select id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' class='form-control'><option value='0'>Select</option> " + stringBuilder.ToString() + "</select > ";
                                    }
                                }
                                else
                                {
                                    if (element.DepandantControlName == "" || element.DepandantControlName == null)
                                    {
                                        string Col_Name = element.ValueFiled + "," + element.DisplayFiled;
                                        if (ApplicantNo != null)
                                        {
                                            DataTable drpdt = GateDynamicDropdownValues(element.DbTableName, Col_Name, element.Condition);
                                            string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), element.Condition);
                                            StringBuilder stringBuilder = new StringBuilder();
                                            if (drpdt.Rows.Count > 0)
                                            {
                                                for (int p = 0; p < drpdt.Rows.Count; p++)
                                                {
                                                    if (Value == drpdt.Rows[p][element.ValueFiled].ToString())
                                                    {
                                                        stringBuilder.Append("<option selected value='" + drpdt.Rows[p][element.ValueFiled] + "'>" + drpdt.Rows[p][element.DisplayFiled] + "</option>");
                                                    }
                                                    else
                                                    {
                                                        stringBuilder.Append("<option value='" + drpdt.Rows[p][element.ValueFiled] + "'>" + drpdt.Rows[p][element.DisplayFiled] + "</option>");
                                                    }
                                                }
                                            }
                                            controlText = "<select id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' class='form-control'><option value='0'>Select</option> " + stringBuilder.ToString() + "</select > ";
                                        }
                                        else
                                        {
                                            DataTable drpdt = GateDynamicDropdownValues(element.DbTableName, Col_Name, element.Condition);
                                            StringBuilder stringBuilder = new StringBuilder();
                                            if (drpdt.Rows.Count > 0)
                                            {
                                                for (int p = 0; p < drpdt.Rows.Count; p++)
                                                {
                                                    stringBuilder.Append("<option value='" + drpdt.Rows[p][element.ValueFiled] + "'>" + drpdt.Rows[p][element.DisplayFiled] + "</option>");
                                                }
                                            }

                                            controlText = "<select id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' class='form-control'><option value='0'>Select</option> " + stringBuilder.ToString() + "</select > ";
                                        }
                                    }
                                    else
                                    {
                                        controlText = "<select id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' class='form-control'><option value='0'>Select</option></select > ";
                                    }
                                }
                            }

                        }
                        else if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "file")
                        {
                            RequiredCheckValue = "Choose File";
                            RequiredKeyWord = "Select";
                            if (ApplicantNo != null)
                            {
                                string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), element.Condition);
                                //<label class='custom-file-label' for='customFile' id='fuUploadPocPrev'>" + Value + "</label>
                                //controlText = "<input type='file' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' text='" + Value + "' value='" + Value + "' class='col-sm-6 form-control' name='postedFiles'><small class='text-danger'>Max file size is " + element.maxSize + "MB Only " + element.acceptedTypes + " allowed</small><input type='hidden' id='hdn_" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='" + element.maxSize + "&" + element.acceptedTypes + "'/>";
                                controlText = "<div class='custom-file'><input type='file' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='" + Value + "' class='custom-file-input'><label class='custom-file-label' for='customFile' id='fuUploadPocPrev_" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "'>" + Value + "</label><small class='text-danger'>Max file size is " + element.maxSize + "MB Only " + element.acceptedTypes + " allowed</small><input type='hidden' id='hdn_" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='" + element.maxSize + "&" + element.acceptedTypes + "'/></div>";
                            }
                            else
                            {
                                controlText = "<div class='custom-file'><input type='file' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='Choose File' class='custom-file-input'><label class='custom-file-label' for='customFile' id='fuUploadPocPrev_" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "'>Choose File</label><small class='text-danger'>Max file size is " + element.maxSize + "MB Only " + element.acceptedTypes + " allowed</small><input type='hidden' id='hdn_" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='" + element.maxSize + "&" + element.acceptedTypes + "'/></div>";
                            }
                            if (FileCntrlScript == "")
                            {
                                FileCntrlScript = " $('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').on('change', function (e) { $('#fuUploadPocPrev_" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').text($('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "')[0].files[0].name);});";
                            }
                            else
                            {
                                FileCntrlScript = FileCntrlScript + "$('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').on('change', function (e) {  $('#fuUploadPocPrev_" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').text($('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "')[0].files[0].name);});";
                            }
                        }
                        else if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "radio")
                        {
                            controlText = " <input type='radio' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name=" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + " value='HTML' class='col-sm-9 form-control'>< label for= 'html' > HTML </ label > ";
                        }
                        else if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "radiogroup")
                        {
                            RequiredCheckValue = "undefined";
                            RequiredKeyWord = "Select";
                            if (element.DbTableName == "" || element.DbTableName == null)
                            {
                                if (ApplicantNo != null)
                                {
                                    string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), element.Condition);
                                    string radiotext = "";
                                    foreach (choices item in element.choices)
                                    {
                                        if (Value == item.value)
                                        {
                                            radiotext = radiotext + " <input type='radio' style='margin-left: -15px;' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value=" + item.value + " class='form-control' checked='true'>" + item.text + "</input></br>";
                                        }
                                        else
                                        {
                                            radiotext = radiotext + " <input type='radio' style='margin-left: -15px;' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value=" + item.value + " class='form-control'>" + item.text + "</input></br>";
                                        }
                                    }

                                    controlText = "<td><div data='checkdiv' style='margin-left: 15px;' id='" + dt.Rows[k]["pvch_control_name"].ToString() + "'>" + radiotext + "</div></td>";

                                }
                                else
                                {
                                    string Mradiotext = "";
                                    if (element != null && element.choices != null)
                                    {
                                        foreach (choices item in element.choices)
                                        {
                                            Mradiotext = Mradiotext + " <input type='radio' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value=" + item.value + " class='form-control'><span style='margin-left: 20px;'>" + item.text + "</span></input></br>";

                                        }

                                    }
                                    controlText = "<td><div data='checkdiv' style='margin-left: 15px;' id='" + dt.Rows[k]["pvch_control_name"].ToString() + "'>" + Mradiotext + "</div></td>";

                                }
                            }
                            else
                            {
                                if (ApplicantNo != null)
                                {
                                    string Col_Name = element.ValueFiled + "," + element.DisplayFiled;
                                    DataTable drpdt = GateDynamicDropdownValues(element.DbTableName, Col_Name, element.Condition);
                                    string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), "");
                                    string Mradiotext = "";
                                    if (drpdt.Rows.Count > 0)
                                    {
                                        for (int p = 0; p < drpdt.Rows.Count; p++)
                                        {
                                            if (Value == drpdt.Rows[p][element.ValueFiled].ToString())
                                            {
                                                //stringBuilder.Append("<option selected value='" + drpdt.Rows[p][element.ValueFiled] + "'>" + drpdt.Rows[p][element.DisplayFiled] + "</option>");
                                                Mradiotext = Mradiotext + "<input type='radio' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value=" + drpdt.Rows[p][element.ValueFiled] + " class='form-control' checked><span style='margin-left: 20px;'>" + drpdt.Rows[p][element.DisplayFiled] + "</span></input></br>";
                                            }
                                            else
                                            {
                                                //stringBuilder.Append("<option value='" + drpdt.Rows[p][element.ValueFiled] + "'>" + drpdt.Rows[p][element.DisplayFiled] + "</option>");
                                                Mradiotext = Mradiotext + "<input type='radio' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value=" + drpdt.Rows[p][element.ValueFiled] + " class='form-control'><span style='margin-left: 20px;'>" + drpdt.Rows[p][element.DisplayFiled] + "</span></input></br>";
                                            }
                                        }
                                    }
                                    controlText = "<td><div data='checkdiv' style='margin-left: 15px;' id='" + dt.Rows[k]["pvch_control_name"].ToString() + "'>" + Mradiotext + "</div></td>";

                                }
                                else
                                {
                                    string Col_Name = element.ValueFiled + "," + element.DisplayFiled;
                                    DataTable drpdt = GateDynamicDropdownValues(element.DbTableName, Col_Name, element.Condition);
                                    string Mradiotext = "";
                                    if (drpdt.Rows.Count > 0)
                                    {
                                        for (int p = 0; p < drpdt.Rows.Count; p++)
                                        {
                                            //< option value = '" + drpdt.Rows[p][element.ValueFiled] + "' > " + drpdt.Rows[p][element.DisplayFiled] + " </ option >
                                            Mradiotext = Mradiotext + "<input type='radio' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value=" + drpdt.Rows[p][element.ValueFiled] + " class='form-control'><span style='margin-left: 20px;'>" + drpdt.Rows[p][element.DisplayFiled] + "</span></input></br>";
                                        }
                                    }

                                    controlText = "<td><div data='checkdiv' style='margin-left: 15px;' id='" + dt.Rows[k]["pvch_control_name"].ToString() + "'>" + Mradiotext + "</div></td>";
                                }
                            }
                        }
                        else if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "checkbox")
                        {
                            StringBuilder stringBuilder = new StringBuilder();
                            if (element.DbTableName == "" || element.DbTableName == null)
                            {

                                if (element.choices != null)
                                {
                                    if (ApplicantNo != null)
                                    {
                                        string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), element.Condition);

                                        if (Value.ToString().Contains(","))
                                        {
                                            foreach (choices item in element.choices)
                                            {
                                                if (Value.ToString().Contains(item.text))
                                                {
                                                    stringBuilder.Append("<input type='checkbox' checked style='height: 20px;width: 30px;position: relative;' value='" + item.text + "'>" + item.text + " </input></br>");
                                                }
                                                else
                                                {
                                                    stringBuilder.Append("<input type='checkbox' style='height: 20px;width: 30px;position: relative;' value='" + item.text + "'>" + item.text + " </input></br>");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            foreach (choices item in element.choices)
                                            {
                                                if (item.text == Value)
                                                {
                                                    stringBuilder.Append("<input type='checkbox' checked style='height: 20px;width: 30px;position: relative;' value='" + item.text + "'>" + item.text + " </input></br>");
                                                }
                                                else
                                                {
                                                    stringBuilder.Append("<input type='checkbox' style='height: 20px;width: 30px;position: relative;' value='" + item.text + "'>" + item.text + " </input></br>");
                                                }
                                            }
                                        }

                                    }
                                    else
                                    {
                                        foreach (choices item in element.choices)
                                        {
                                            stringBuilder.Append("<input type='checkbox' style='height: 20px;width: 30px;position: relative;' value='" + item.text + "'>" + item.text + " </input></br>");

                                        }
                                    }
                                }
                                controlText = "<td><div  id='" + dt.Rows[k]["pvch_control_name"].ToString() + "'>" + stringBuilder + "</div></td>";
                            }
                            else
                            {
                                if (ApplicantNo != null)
                                {
                                    string Col_Name = element.ValueFiled + "," + element.DisplayFiled;
                                    DataTable drpdt = GateDynamicDropdownValues(element.DbTableName, Col_Name, element.Condition);
                                    string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), "");
                                    string Mradiotext = "";
                                    if (drpdt.Rows.Count > 0)
                                    {
                                        if (Value.Contains(","))
                                        {
                                            //for(int m=0;m<Value.Split(',').Length;m++)
                                            //{
                                            for (int p = 0; p < drpdt.Rows.Count; p++)
                                            {
                                                if (Value.ToString().Contains(drpdt.Rows[p][element.ValueFiled].ToString()))
                                                {
                                                    //stringBuilder.Append("<option selected value='" + drpdt.Rows[p][element.ValueFiled] + "'>" + drpdt.Rows[p][element.DisplayFiled] + "</option>");
                                                    stringBuilder.Append("<input type='checkbox' style='height: 20px;width: 30px;position: relative;' value='" + drpdt.Rows[p][element.ValueFiled] + "' checked>" + drpdt.Rows[p][element.DisplayFiled] + " </input></br>");
                                                }
                                                else
                                                {
                                                    //stringBuilder.Append("<option value='" + drpdt.Rows[p][element.ValueFiled] + "'>" + drpdt.Rows[p][element.DisplayFiled] + "</option>");
                                                    stringBuilder.Append("<input type='checkbox' style='height: 20px;width: 30px;position: relative;' value='" + drpdt.Rows[p][element.ValueFiled] + "'>" + drpdt.Rows[p][element.DisplayFiled] + " </input></br>");
                                                }
                                            }
                                            //}
                                        }
                                        else
                                        {
                                            for (int p = 0; p < drpdt.Rows.Count; p++)
                                            {
                                                if (Value == drpdt.Rows[p][element.ValueFiled].ToString())
                                                {
                                                    //stringBuilder.Append("<option selected value='" + drpdt.Rows[p][element.ValueFiled] + "'>" + drpdt.Rows[p][element.DisplayFiled] + "</option>");
                                                    stringBuilder.Append("<input type='checkbox' style='height: 20px;width: 30px;position: relative;' value='" + drpdt.Rows[p][element.ValueFiled] + "' checked>" + drpdt.Rows[p][element.DisplayFiled] + " </input></br>");
                                                }
                                                else
                                                {
                                                    //stringBuilder.Append("<option value='" + drpdt.Rows[p][element.ValueFiled] + "'>" + drpdt.Rows[p][element.DisplayFiled] + "</option>");
                                                    stringBuilder.Append("<input type='checkbox' style='height: 20px;width: 30px;position: relative;' value='" + drpdt.Rows[p][element.ValueFiled] + "'>" + drpdt.Rows[p][element.DisplayFiled] + " </input></br>");
                                                }
                                            }
                                        }
                                    }
                                    controlText = "<td><div  id='" + dt.Rows[k]["pvch_control_name"].ToString() + "'>" + stringBuilder + "</div></td>";
                                }
                                else
                                {
                                    string Col_Name = element.ValueFiled + "," + element.DisplayFiled;
                                    DataTable drpdt = GateDynamicDropdownValues(element.DbTableName, Col_Name, element.Condition);
                                    if (drpdt.Rows.Count > 0)
                                    {
                                        for (int p = 0; p < drpdt.Rows.Count; p++)
                                        {
                                            //< option value = '" + drpdt.Rows[p][element.ValueFiled] + "' > " + drpdt.Rows[p][element.DisplayFiled] + " </ option >
                                            //Mradiotext = Mradiotext + "<input type='radio' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value=" + drpdt.Rows[p][element.ValueFiled] + " class='form-control'><span style='margin-left: 20px;'>" + drpdt.Rows[p][element.DisplayFiled] + "</span></input></br>";
                                            stringBuilder.Append("<input type='checkbox' style='height: 20px;width: 30px;position: relative;' value='" + drpdt.Rows[p][element.ValueFiled] + "'>" + drpdt.Rows[p][element.DisplayFiled] + " </input></br>");

                                        }
                                    }

                                    controlText = "<td><div  id='" + dt.Rows[k]["pvch_control_name"].ToString() + "'>" + stringBuilder + "</div></td>";
                                }
                            }
                        }
                        else if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "comment")
                        {
                            if (ApplicantNo != null)
                            {
                                string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), element.Condition);
                                controlText = "<textarea class='form-control' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "'>" + Value + "</textarea>";
                            }
                            else
                            {
                                controlText = "<textarea class='form-control' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "'></textarea>";
                            }
                        }

                        else if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "date")
                        {

                            if (ApplicantNo != null)
                            {
                                string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), element.Condition);
                                controlText = "<input type='date' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='" + Value + "' class='form-control'>";
                            }
                            else
                            {
                                controlText = "<input type='date' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='' class='form-control'>";
                            }
                        }
                        else if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "datetime-local")
                        {

                            if (ApplicantNo != null)
                            {
                                string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), element.Condition);
                                controlText = "<input type='datetime-local' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='" + Value.Replace(" ", "T") + "' class='form-control'>";
                            }
                            else
                            {
                                controlText = "<input type='datetime-local' id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='' class='form-control'>";
                            }
                        }
                        else if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "label")
                        {

                            if (ApplicantNo != null)
                            {
                                string Value = GateDynamicTableValues(FormId, ApplicantNo, dt.Rows[k]["pvch_control_name"].ToString(), element.Condition);
                                controlText = "<label id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='' >" + Value + "</label>";
                            }
                            else
                            {
                                controlText = "<label id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' value='' ></label>";
                            }
                        }
                        else
                        {

                        }
                        #endregion
                        if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "panel")
                        {
                            strHtml = strHtml + "<div class='col-sm-12'><h5>" + dt.Rows[k]["PVCH_LABEL_NAME"].ToString() + "</h5></div> <div class='clearfix'></div>";// +"</div></div>";
                            lebeltext = "";
                            controlText = "";
                        }

                        strRow = strRow + LayOut(Convert.ToInt32(intAllignment), lebeltext, controlText, strReq, "1");
                        if (dt.Rows.Count - 1 == k)
                        {
                            CstrRow = "<div><input type='hidden' id='hdncolumn' value='" + strColumn + "'/><input type='hidden' id='hdncolumnid' value='" + strColumnId + "'/><input type='hidden' id='hdncolumntype' value='" + strColumntype + "'/></div>";
                        }
                        if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "matrixdynamic")
                        {
                            var Coldata = "";/*var Rowdata = "";*/
                            var Coltypedata = "";
                            if (ApplicantNo != null)
                            {
                                DataTable chldt = FindChildTableData(FormId, dt.Rows[k]["TABLE_NAME"].ToString(), dt.Rows[k]["pvch_control_name"].ToString(), ApplicantNo);
                                for (int j = 0; j < chldt.Rows.Count; j++)
                                {
                                    Coldata = "";
                                    var Actiondata = "";
                                    for (int i = 0; i < element.ColmatrixdropdownData.Count; i++)
                                    {
                                        if (Coldata == "")
                                        {
                                            Coldata = "<th> " + element.ColmatrixdropdownData[i].title + "<input type='hidden' id='hdncol_" + i + "' value='" + element.ColmatrixdropdownData[i].name + "' /><input type='hidden' id='hdntype_" + i + "' value='" + element.ColmatrixdropdownData[i].cellType + "' /> </th>";
                                        }
                                        else
                                        {
                                            Coldata = Coldata + "<th> " + element.ColmatrixdropdownData[i].title + "<input type='hidden' id='hdncol_" + i + "' value='" + element.ColmatrixdropdownData[i].name + "' /><input type='hidden' id='hdntype_" + i + "' value='" + element.ColmatrixdropdownData[i].cellType + "' /> </th>";
                                        }
                                        if (element.ColmatrixdropdownData[i].cellType == "text")
                                        {
                                            Coltypedata = Coltypedata + "<td><input type='text' id='" + dt.Rows[k]["pvch_control_name"].ToString() + "_" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "_" + j + "' value='" + chldt.Rows[j][element.ColmatrixdropdownData[i].title] + "' class='form-control' name=" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "></td>";
                                        }
                                        else if (element.ColmatrixdropdownData[i].cellType == "dropdown")
                                        {
                                            StringBuilder stringBuilder = new StringBuilder();
                                            if (element.ColmatrixdropdownData[i].matrixdropdownData != null)
                                            {
                                                foreach (choices item in element.ColmatrixdropdownData[i].matrixdropdownData)
                                                {
                                                    if (item.value == chldt.Rows[j][element.ColmatrixdropdownData[i].title].ToString())
                                                    {
                                                        stringBuilder.Append("<option value='" + item.value + "' selected='true'>" + item.text + "</option>");
                                                    }
                                                    else
                                                    {
                                                        stringBuilder.Append("<option value='" + item.value + "'>" + item.text + "</option>");
                                                    }
                                                }
                                            }

                                            //controlText = "<select id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' class='form-control'><option value='0'>Select</option> " + stringBuilder.ToString() + "</select > ";
                                            Coltypedata = Coltypedata + "<td><select id = '" + dt.Rows[k]["pvch_control_name"].ToString() + "_" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "_" + j + "' name = '" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "' class='form-control'><option value = '0' > Select </option >" + stringBuilder.ToString() + "</select></td>";
                                        }
                                        else if (element.ColmatrixdropdownData[i].cellType == "file")
                                        {
                                            Coltypedata = Coltypedata + "<td><input type='file' id='" + dt.Rows[k]["pvch_control_name"].ToString() + "_" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "_" + j + "' value='" + chldt.Rows[j][element.ColmatrixdropdownData[i].title] + "' class='form-control' name=" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + ">" + chldt.Rows[j][element.ColmatrixdropdownData[i].title] + "</input><span><a href='/Image_" + dt.Rows[k]["pvch_control_name"].ToString() + "_ " + element.ColmatrixdropdownData[i].title + "_ " + FormId + " / " + chldt.Rows[j][element.ColmatrixdropdownData[i].title] + "' download><i class='fa fa-download fa-lg' aria-hidden='true'></i></a></span></td>";
                                        }
                                        else if (element.ColmatrixdropdownData[i].cellType == "checkbox")
                                        {
                                            StringBuilder stringBuilder = new StringBuilder();
                                            if (element.ColmatrixdropdownData[i].matrixdropdownData != null)
                                            {
                                                if (chldt.Rows[j][element.ColmatrixdropdownData[i].title].ToString().Contains(","))
                                                {
                                                    foreach (choices item in element.ColmatrixdropdownData[i].matrixdropdownData)
                                                    {
                                                        for (int p = 0; p < chldt.Rows[j][element.ColmatrixdropdownData[i].title].ToString().Split(',').Length; p++)
                                                        {

                                                            if (item.value == chldt.Rows[j][element.ColmatrixdropdownData[i].title].ToString().Split(',')[p].ToString())
                                                            {
                                                                stringBuilder.Append("<input type='checkbox' style='height: 20px;width: 30px;position: relative;' checked='true' value='" + item.text + "'>" + item.text + " </input></br>");
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    foreach (choices item in element.ColmatrixdropdownData[i].matrixdropdownData)
                                                    {
                                                        if (item.value == chldt.Rows[j][element.ColmatrixdropdownData[i].title].ToString())
                                                        {
                                                            stringBuilder.Append("<input type='checkbox' style='height: 20px;width: 30px;position: relative;' checked='true' value='" + item.text + "'>" + item.text + " </input></br>");
                                                        }
                                                        else
                                                        {
                                                            stringBuilder.Append("<input type='checkbox' style='height: 20px;width: 30px;position: relative;' value='" + item.text + "'>" + item.text + " </input></br>");
                                                        }
                                                    }
                                                }
                                            }
                                            Coltypedata = Coltypedata + "<td><div  id='" + dt.Rows[k]["pvch_control_name"].ToString() + "_" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "_" + j + "'>" + stringBuilder + "</div></td>";
                                            //Coltypedata = Coltypedata + "<td>"+stringBuilder+"</td>";
                                        }
                                        else if (element.ColmatrixdropdownData[i].cellType == "radiogroup")
                                        {
                                            string Mradiotext = "";
                                            if (element.ColmatrixdropdownData[i].matrixdropdownData != null)
                                            {
                                                foreach (choices item in element.ColmatrixdropdownData[i].matrixdropdownData)
                                                {
                                                    if (item.value == chldt.Rows[j][element.ColmatrixdropdownData[i].title].ToString())
                                                    {
                                                        Mradiotext = Mradiotext + "<input type='radio' style='margin-left: -15px;' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "_" + j + "' checked='true' value='" + item.value + "' class='form-control'>" + item.text + " </input></br>";
                                                    }
                                                    else
                                                    {
                                                        Mradiotext = Mradiotext + " <input type='radio' style='margin-left: -15px;'  name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "_" + j + "' value=" + item.value + " class='form-control'>" + item.text + "</input></br>";
                                                    }
                                                }

                                            }
                                            Coltypedata = Coltypedata + "<td><div data='checkdiv' id='" + dt.Rows[k]["pvch_control_name"].ToString() + "_" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "_" + j + "'>" + Mradiotext + "</div></td>";

                                        }
                                        else
                                        {
                                            Coltypedata = Coltypedata + "<td><input type='text' id='" + dt.Rows[k]["pvch_control_name"].ToString() + "_" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "_" + j + "' value='" + chldt.Rows[j][element.ColmatrixdropdownData[i].title] + "' class='form-control' name=" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "></td>";
                                        }
                                    }
                                    Coldata = Coldata + "<th> Action </th>";
                                    if (j == 0)
                                    {
                                        Actiondata = "<button class='btn btn-primary btn-sm add-btn' id='btn_" + dt.Rows[k]["pvch_control_name"].ToString() + "' data-toggle='tooltip' data-placement='top' title='Add'><i class='fa fa-plus' aria-hidden='true'></i></button>";
                                    }
                                    else
                                    {
                                        Actiondata = "<button class='btn btn-danger btn-sm remove' id='btn_" + dt.Rows[k]["pvch_control_name"].ToString() + "' data-toggle='tooltip' data-placement='top' title='Add'><i class='fa fa-minus' aria-hidden='true'></i></button>";
                                    }
                                    Coltypedata = "<tr>" + Coltypedata + "<td>" + Actiondata + "</td></tr>";
                                }

                                var data = "<table id='" + dt.Rows[k]["pvch_control_name"].ToString() + "' cellspacing='0' class='table'><thead><tr>" + Coldata + "</tr></thead><tbody>" + Coltypedata + "</tbody></table> ";
                                strHtml = strHtml + "<div class='col-sm-12' id='div_" + dt.Rows[k]["INT_ID"].ToString() + "'><h5>" + dt.Rows[k]["pvch_label_name"].ToString() + "</h5>" + data + "</div>" + CstrRow;
                            }
                            else
                            {
                                for (int i = 0; i < element.ColmatrixdropdownData.Count; i++)
                                {
                                    if (Coldata == "")
                                    {
                                        Coldata = "<th> " + element.ColmatrixdropdownData[i].title + "<input type='hidden' id='hdncol_" + i + "' value='" + element.ColmatrixdropdownData[i].name + "' /><input type='hidden' id='hdntype_" + i + "' value='" + element.ColmatrixdropdownData[i].cellType + "' /> </th>";
                                    }
                                    else
                                    {
                                        Coldata = Coldata + "<th> " + element.ColmatrixdropdownData[i].title + "<input type='hidden' id='hdncol_" + i + "' value='" + element.ColmatrixdropdownData[i].name + "' /><input type='hidden' id='hdntype_" + i + "' value='" + element.ColmatrixdropdownData[i].cellType + "' /> </th>";
                                    }
                                    if (element.ColmatrixdropdownData[i].cellType == "text")
                                    {
                                        Coltypedata = Coltypedata + "<td><input type='text' id='" + dt.Rows[k]["pvch_control_name"].ToString() + "_" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "_0' value='' class='form-control' name=" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "></td>";
                                    }
                                    else if (element.ColmatrixdropdownData[i].cellType == "dropdown")
                                    {
                                        StringBuilder stringBuilder = new StringBuilder();
                                        if (element.ColmatrixdropdownData[i].matrixdropdownData != null)
                                        {
                                            foreach (choices item in element.ColmatrixdropdownData[i].matrixdropdownData)
                                            {
                                                stringBuilder.Append("<option value='" + item.value + "'>" + item.text + "</option>");
                                            }
                                        }

                                        //controlText = "<select id='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "' class='form-control'><option value='0'>Select</option> " + stringBuilder.ToString() + "</select > ";
                                        Coltypedata = Coltypedata + "<td><select id = '" + dt.Rows[k]["pvch_control_name"].ToString() + "_" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "_0' name = '" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "' class='form-control'><option value = '0' > Select </option >" + stringBuilder.ToString() + "</select></td>";
                                    }
                                    else if (element.ColmatrixdropdownData[i].cellType == "file")
                                    {
                                        Coltypedata = Coltypedata + "<td><input type='file' id='" + dt.Rows[k]["pvch_control_name"].ToString() + "_" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "_0' value='' class='form-control' name=" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "></td>";
                                    }
                                    else if (element.ColmatrixdropdownData[i].cellType == "checkbox")
                                    {
                                        StringBuilder stringBuilder = new StringBuilder();
                                        if (element.ColmatrixdropdownData[i].matrixdropdownData != null)
                                        {
                                            foreach (choices item in element.ColmatrixdropdownData[i].matrixdropdownData)
                                            {
                                                stringBuilder.Append("<input type='checkbox' style='height: 20px;width: 30px;position: relative;' value='" + item.text + "'>" + item.text + " </input></br>");

                                            }
                                        }
                                        Coltypedata = Coltypedata + "<td><div  id='" + dt.Rows[k]["pvch_control_name"].ToString() + "_" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "_0'>" + stringBuilder + "</div></td>";
                                        //Coltypedata = Coltypedata + "<td>"+stringBuilder+"</td>";
                                    }
                                    else if (element.ColmatrixdropdownData[i].cellType == "radiogroup")
                                    {
                                        string Mradiotext = "";
                                        if (element.ColmatrixdropdownData[i].matrixdropdownData != null)
                                        {
                                            foreach (choices item in element.ColmatrixdropdownData[i].matrixdropdownData)
                                            {
                                                Mradiotext = Mradiotext + " <input type='radio' style='margin-left: -15px;'  name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "_0' value=" + item.value + " class='form-control'>" + item.text + "</input></br>";

                                            }

                                        }
                                        Coltypedata = Coltypedata + "<td><div data='checkdiv' id='" + dt.Rows[k]["pvch_control_name"].ToString() + "_" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "_0'>" + Mradiotext + "</div></td>";

                                    }
                                    else
                                    {
                                        Coltypedata = Coltypedata + "<td><input type='text' id='" + dt.Rows[k]["pvch_control_name"].ToString() + "_" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "_0' value='' class='form-control' name=" + element.ColmatrixdropdownData[i].name.Replace(" ", "") + "></td>";
                                    }
                                }
                                Coldata = Coldata + "<th> Action </th>";
                                Coltypedata = "<tr>" + Coltypedata + "<td><button class='btn btn-primary btn-sm add-btn' id='btn_" + dt.Rows[k]["pvch_control_name"].ToString() + "' data-toggle='tooltip' data-placement='top' title='Add'><i class='fa fa-plus' aria-hidden='true'></i></button></td></tr>";
                                var data = "<table id='" + dt.Rows[k]["pvch_control_name"].ToString() + "' cellspacing='0' class='table'><thead><tr>" + Coldata + "</tr></thead><tbody>" + Coltypedata + "</tbody></table> ";
                                strHtml = strHtml + "<div class='col-sm-12' id='div_" + dt.Rows[k]["INT_ID"].ToString() + "'><h5>" + dt.Rows[k]["pvch_label_name"].ToString() + "</h5>" + data + "</div>" + CstrRow;
                            }
                        }
                        else if (dt.Rows[k]["PVCH_CONTROL_TYPE"].ToString() == "html")
                        {
                            strHtml = strHtml + "<div class='col-sm-12' id='div_" + dt.Rows[k]["INT_ID"].ToString() + "'>" + element.html + "</div>";
                        }
                        else
                        {
                            counter = counter + 1;
                            if (counter > 1)
                            {
                                if ((Convert.ToInt32(intAllignment) % counter) == 0)
                                {
                                    strHtml = strHtml + strRow + "";
                                    counter = 0;
                                }
                                else
                                {
                                    strHtml = strHtml + strRow;
                                }
                            }
                            else
                            {
                                strHtml = strHtml + strGroupDiv + strRow;
                                // strHtml = strHtml + strRow;
                            }
                            strHtml = strHtml + CstrRow;
                        }
                        if (element.ApiUrl != null && element.CascadeControlName != null)
                        {
                            string Aparameter = "";
                            if (element.ApiParameter != "" && element.ApiParameter != null)
                            {
                                if (element.ApiParameter.Contains(",") == true)
                                {
                                    int dtcnt = element.ApiParameter.Split(',').Length;
                                    for (int i = 0; i < dtcnt; i++)
                                    {
                                        if (Aparameter == "")
                                        {
                                            Aparameter = "$('#" + element.ApiParameter.Split(',')[i].ToString().Replace(" ", "") + "').val()";
                                        }
                                        else
                                        {
                                            Aparameter = Aparameter + "+'/'" + "+$('#" + element.ApiParameter.Split(',')[i].ToString().Replace(" ", "") + "').val()";
                                        }
                                    }
                                }
                                else
                                {
                                    Aparameter = "$('#" + element.ApiParameter.ToString().Replace(" ", "") + "').val()";
                                }
                            }
                            if (element.ApiType == "True")
                            {
                                var uri = new Uri(element.ApiUrl);
                                var Aurl = uri.PathAndQuery;

                                if (element.CascadeControlName.Contains(",") == true)
                                {
                                    var textvalue = "";
                                    int dtcnt = element.CascadeControlName.Split(',').Length;
                                    for (int i = 0; i < dtcnt; i++)
                                    {
                                        DataRow[] drArr = dt.Select("pvch_control_name='" + element.CascadeControlName.Split(',')[i] + "'");
                                        if (drArr[0].ItemArray[3].ToString() == "text")
                                        {
                                            if (textvalue == "")
                                            {
                                                textvalue = "$('#" + element.CascadeControlName.Split(',')[i] + "').val(sdata." + element.DisplayFiled.Split(',')[i] + "== undefined ? 0 : sdata." + element.DisplayFiled.Split(',')[i] + ");";
                                            }
                                            else
                                            {
                                                textvalue = textvalue + "$('#" + element.CascadeControlName.Split(',')[i] + "').val(sdata." + element.DisplayFiled.Split(',')[i] + "== undefined ? '0' : sdata." + element.DisplayFiled.Split(',')[i] + ");";
                                            }
                                        }
                                    }
                                    BindScript = " $('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').change(function () {" +
                       "$.getJSON('" + Aurl + "/' + " + Aparameter + ", function(data) {" +
                           "$.each(data, function(i, sdata) {" +
                                           "" + textvalue + "}); });});";
                                }
                                else
                                {
                                    DataRow[] drArr = dt.Select("pvch_control_name='" + element.CascadeControlName + "'");
                                    if (drArr[0].ItemArray[3].ToString() == "dropdown")
                                    {

                                        BindScript = " $('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').change(function () {" +
                            "$.getJSON('" + Aurl + "/' + " + Aparameter + ", function(data) {" +
                                            "var items = '<option>Select</option>';" +
                                "$.each(data, function(i, sdata) {" +
                                                "items += '<option value=' + sdata." + element.ValueFiled + " + '>' + sdata." + element.DisplayFiled + " + '</option>';" +
                                            "});$('#" + element.CascadeControlName + "').html(items);}); });";
                                    }
                                    else if (drArr[0].ItemArray[3].ToString() == "text")
                                    {
                                        BindScript = " $('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').change(function () {" +
                        "$.getJSON('" + Aurl + "/' + " + Aparameter + ", function(data) {" +
                            "if(data.length>0){$.each(data, function(i, sdata) {" +
                                            "$('#" + element.CascadeControlName + "').val(sdata." + element.DisplayFiled + " == undefined ? '0' : sdata." + element.DisplayFiled + ");});}else{$('#" + element.CascadeControlName + "').val(data." + element.DisplayFiled + " == undefined ? '0' : data." + element.DisplayFiled + "); }});});";
                                    }
                                }
                            }
                            else
                            {
                                if (element.CascadeControlName.Contains(",") == true)
                                {
                                    var textvalue = "";
                                    int dtcnt = element.CascadeControlName.Split(',').Length;
                                    for (int i = 0; i < dtcnt; i++)
                                    {
                                        DataRow[] drArr = dt.Select("pvch_control_name='" + element.CascadeControlName.Split(',')[i] + "'");
                                        if (drArr[0].ItemArray[3].ToString() == "text")
                                        {
                                            if (textvalue == "")
                                            {
                                                textvalue = "$('#" + element.CascadeControlName.Split(',')[i] + "').val(ditem[i]." + element.DisplayFiled.Split(',')[i] + ");";
                                            }
                                            else
                                            {
                                                textvalue = textvalue + "$('#" + element.CascadeControlName.Split(',')[i] + "').val(ditem[i]." + element.DisplayFiled.Split(',')[i] + ");";
                                            }
                                        }
                                    }
                                    BindScript = " $('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').change(function () {" +
                       "$.getJSON('" + element.ApiUrl + "', function(data) {" +
                        " var arrayToString = JSON.stringify(Object.assign({}, data)); var ditem = JSON.parse(arrayToString);" +
                                                                "for (var i = 0; i < Object.keys(ditem).length; i++){" +
                                       "" + textvalue + "}}); });";
                                }
                                else
                                {
                                    DataRow[] drArr = dt.Select("pvch_control_name='" + element.CascadeControlName + "'");
                                    if (drArr[0].ItemArray[3].ToString() == "dropdown")
                                    {
                                        BindScript = BindScript + " $('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').change(function () {" +
                                                 "$.getJSON('" + element.ApiUrl + "' , function(data) {" +
                                                                 " var arrayToString = JSON.stringify(Object.assign({}, data)); var ditem = JSON.parse(arrayToString); var items = '<option>Select</option>';" +
                                                                 "for (var i = 0; i < Object.keys(ditem).length; i++){" +
                                        "items += '<option value=' + ditem[i]." + element.ValueFiled + " + '>' + ditem[i]." + element.DisplayFiled + " + '</option>';}" +
                                        "$('#" + element.CascadeControlName + "').html(items);}); });";
                                    }
                                    else if (drArr[0].ItemArray[3].ToString() == "text")
                                    {
                                        BindScript = BindScript + " $('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').change(function () {" +
                        "$.getJSON('" + element.ApiUrl + "', function(data) {" +
                         " var arrayToString = JSON.stringify(Object.assign({}, data)); var ditem = JSON.parse(arrayToString); var items = '<option>Select State</option>';" +
                                                                 "for (var i = 0; i < Object.keys(ditem).length; i++){" +
                                        "$('#" + element.CascadeControlName + "').val(ditem[i]." + element.DisplayFiled + ");}}); });";
                                    }
                                }
                            }

                        }
                        if (element.isRequired == "True")
                        {
                            //creating script for required validation
                            if (element.type == "radiogroup")
                            {

                                string ScriptCondition = "if ($(\"input[name='" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "']:checked\").val() == " + RequiredCheckValue + "){"
                                + "bootbox.alert('Please " + RequiredKeyWord + " " + dt.Rows[k]["pvch_label_name"].ToString() + "!');"
                                + "$('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').focus();return false;}";
                                if (IsFirstIf == true)
                                {

                                    IsFirstIf = false;
                                    Script = Script + ScriptCondition;
                                }
                                else
                                {
                                    Script = Script + "else " + ScriptCondition;
                                }
                            }
                            else if (element.type == "file")
                            {
                                string ScriptCondition = "if($('#fuUploadPocPrev_" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "')[0].innerText=='" + RequiredCheckValue + "'){"
                                + "bootbox.alert('Please " + RequiredKeyWord + " " + dt.Rows[k]["pvch_label_name"].ToString() + "!');"
                                + "$('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').focus();return false;}";
                                if (IsFirstIf == true)
                                {
                                    IsFirstIf = false;
                                    Script = Script + ScriptCondition;
                                }
                                else
                                {
                                    Script = Script + "else " + ScriptCondition;
                                }
                            }
                            else
                            {
                                string ScriptCondition = "if($('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').val()=='" + RequiredCheckValue + "'){"
                                + "bootbox.alert('Please " + RequiredKeyWord + " " + dt.Rows[k]["pvch_label_name"].ToString() + "!');"
                                + "$('#" + dt.Rows[k]["pvch_control_name"].ToString().Replace(" ", "") + "').focus();return false;}";
                                if (IsFirstIf == true)
                                {
                                    IsFirstIf = false;
                                    Script = Script + ScriptCondition;
                                }
                                else
                                {
                                    Script = Script + "else " + ScriptCondition;
                                }
                            }
                        }
                        if (element.ValidationFiled != null)
                        {
                            var ControlDetails = ""; var Cmessage = "";

                            if (element.ValidationFiled.Contains(">") == true)
                            {
                                var data = element.ValidationFiled.ToString().Split('>');
                                for (int i = 0; i < data.Length; i++)
                                {
                                    if (ControlDetails == "")
                                    {
                                        ControlDetails = "parseFloat($('#" + data[i] + "').val())";
                                    }
                                    else
                                    {
                                        ControlDetails = ControlDetails + ">" + "parseFloat($('#" + data[i] + "').val())";
                                    }
                                    DataView dv = new DataView(dt);
                                    dv.RowFilter = "pvch_control_name='" + data[i] + "'";
                                    DataTable dt_1 = dv.ToTable();
                                    if (Cmessage == "")
                                    {
                                        Cmessage = dt_1.Rows[0]["pvch_label_name"].ToString();
                                    }
                                    else
                                    {
                                        Cmessage = Cmessage + " can not be greater than " + dt_1.Rows[0]["pvch_label_name"].ToString();
                                    }
                                }

                            }
                            else if (element.ValidationFiled.Contains("<") == true)
                            {
                                var data = element.ValidationFiled.ToString().Split('<');
                                for (int i = 0; i < data.Length; i++)
                                {
                                    if (ControlDetails == "")
                                    {
                                        ControlDetails = "parseFloat($('#" + data[i] + "').val())";
                                    }
                                    else
                                    {
                                        ControlDetails = ControlDetails + "<" + "parseFloat($('#" + data[i] + "').val())";
                                    }
                                    DataView dv = new DataView(dt);
                                    dv.RowFilter = "pvch_control_name='" + data[i] + "'";
                                    DataTable dt_1 = dv.ToTable();
                                    if (Cmessage == "")
                                    {
                                        Cmessage = dt_1.Rows[0]["pvch_label_name"].ToString();
                                    }
                                    else
                                    {
                                        Cmessage = Cmessage + " must be less than " + dt_1.Rows[0]["pvch_label_name"].ToString();
                                    }
                                }

                            }
                            else if (element.ValidationFiled.Contains("<=") == true)
                            {
                                var data = element.ValidationFiled.ToString().Split('<');
                                for (int i = 0; i < data.Length; i++)
                                {
                                    if (ControlDetails == "")
                                    {
                                        ControlDetails = "parseFloat($('#" + data[i] + "').val())";
                                    }
                                    else
                                    {
                                        ControlDetails = ControlDetails + "<=" + "parseFloat($('#" + data[i] + "').val())";
                                    }
                                    DataView dv = new DataView(dt);
                                    dv.RowFilter = "pvch_control_name='" + data[i] + "'";
                                    DataTable dt_1 = dv.ToTable();
                                    if (Cmessage == "")
                                    {
                                        Cmessage = dt_1.Rows[0]["pvch_label_name"].ToString();
                                    }
                                    else
                                    {
                                        Cmessage = Cmessage + " must be greater than and equal to " + dt_1.Rows[0]["pvch_label_name"].ToString();
                                    }
                                }

                            }
                            else if (element.ValidationFiled.Contains(">=") == true)
                            {
                                var data = element.ValidationFiled.ToString().Split('<');
                                for (int i = 0; i < data.Length; i++)
                                {
                                    if (ControlDetails == "")
                                    {
                                        ControlDetails = "parseFloat($('#" + data[i] + "').val())";
                                    }
                                    else
                                    {
                                        ControlDetails = ControlDetails + ">=" + "parseFloat($('#" + data[i] + "').val())";
                                    }
                                    DataView dv = new DataView(dt);
                                    dv.RowFilter = "pvch_control_name='" + data[i] + "'";
                                    DataTable dt_1 = dv.ToTable();
                                    if (Cmessage == "")
                                    {
                                        Cmessage = dt_1.Rows[0]["pvch_label_name"].ToString();
                                    }
                                    else
                                    {
                                        Cmessage = Cmessage + " must be less than and equal to " + dt_1.Rows[0]["pvch_label_name"].ToString();
                                    }
                                }

                            }
                            else if (element.ValidationFiled.Contains("=") == true)
                            {
                                var data = element.ValidationFiled.ToString().Split('<');
                                for (int i = 0; i < data.Length; i++)
                                {
                                    if (ControlDetails == "")
                                    {
                                        ControlDetails = "parseFloat($('#" + data[i] + "').val())";
                                    }
                                    else
                                    {
                                        ControlDetails = ControlDetails + "==" + "parseFloat($('#" + data[i] + "').val())";
                                    }
                                    DataView dv = new DataView(dt);
                                    dv.RowFilter = "pvch_control_name='" + data[i] + "'";
                                    DataTable dt_1 = dv.ToTable();
                                    if (Cmessage == "")
                                    {
                                        Cmessage = dt_1.Rows[0]["pvch_label_name"].ToString();
                                    }
                                    else
                                    {
                                        Cmessage = Cmessage + " must be equal to " + dt_1.Rows[0]["pvch_label_name"].ToString();
                                    }
                                }

                            }
                            ValidationScript = "if (" + ControlDetails + "){return true;}else{"
                                + "bootbox.alert('" + Cmessage + "!');return false;}";
                        }
                    }
                }
            }
            if (ValidationScript != null)
            {
                ValidationScript = "return true;";
            }
            if (Script != "")
            {
                return strHtml + "|" + "function validate() {" + Script + "else { return true; }}" + "|" + csslink + "|" + expressionscript + "|" + scriptfile + "|" + "$(function () {" + BindScript + "});" + "|" + "function validateControl() {" + ValidationScript + "}" + "|" + FileCntrlScript + "|" + MatrixDynamicScript;
            }
            else
            {
                return strHtml + "|" + "function validate() { return true; }" + "|" + csslink + "|" + expressionscript + "|" + scriptfile + "|" + "$(function () {" + BindScript + "});" + "|" + "function validateControl() {" + ValidationScript + "}" + "|" + FileCntrlScript + "|" + MatrixDynamicScript;
            }
        }
        public async Task<string> PostResults(DformResultDomain res, int Userid, string Status, int DeptId)
        {
            try
            {
                //    var invoices = await Connection.QueryAsync(query);
                // string sql = "INSERT INTO DF_FORM_RESULT (FORMID,RESULTJSON) Values ('" + formId + "','" + resultJson + "') ";
                // var affectedRows = await Connection.ExecuteAsync(sql, new { formId = formId, resultJson = resultJson });
                var appno = InsertData(res.RESULTJSON, res.FORMID, Userid, Status, DeptId);

                var p = new DynamicParameters();
                p.Add("FORMID", res.FORMID);
                p.Add("RESULTJSON", res.RESULTJSON);
                p.Add("CREATEDDATE", res.CREATEDDATE);
                p.Add("DELETEDFLAG", res.DELETEDFLAG);
                p.Add("APPLICATIONNO", appno.Result);
                var results = await Connection.QueryAsync<dynamic>("USP_DF_INSERTRESULT", p, commandType: CommandType.StoredProcedure);


                return appno.Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<string> UpdateResult(string Data, int Formid, int APPROVESTATUS, string Appno, string Remark, int UserId, int ActionNo, string Result)
        {
            try
            {
                var affectedRows = 0; string Tbl_Name = "";
                List<string> elements = new List<string>();
                DataTable dtt = new DataTable();
                DataSet ds = new DataSet();
                if (Data != "")
                {
                    string PUserid = "0";
                    string PDesgid = "0";
                    string Check = "SELECT dr.tablename,dr.columnname,df.schemeid,REPLACE(dr.TABLENAME, (select tablename from DF_FORM_REFERENCE WHERE FORMID=" + Formid + " and columnname='Main')+'_', '') CntrlName FROM DF_FORM_REFERENCE dr inner join DF_FORM df on dr.formid = df.id WHERE dr.FORMID=" + Formid + "";
                    IDataReader dr = await Connection.ExecuteReaderAsync(Check);
                    DataTable dt = new DataTable();
                    dt.Load(dr);

                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < Result.Split('|').Length; i++)
                        {
                            if (Result.Split('|')[i].Contains(":{"))
                            {
                                string Dtname = ("dt_" + Result.Split('|')[i].Split(new[] { ":{" }, StringSplitOptions.None)[0].Replace(@"\", "//")).Replace("\"", "");
                                ds.Tables.Add(convertToDataTable("{" + Result.Split('|')[i].Split(new[] { ":{" }, StringSplitOptions.None)[1].Replace(@"\", "//"), Dtname));
                            }
                        }
                        string[] columnNames = dtt.Columns.Cast<DataColumn>().Select(x => x.ColumnName.Replace(" ", "")).ToArray();
                        for (int i = 0; i < dtt.Columns.Count; i++)
                        {
                            elements.Add(dtt.Rows[0][i].ToString());
                        }

                        for (int j = 0; j < dt.Rows.Count; j++)
                        {
                            if (dt.Rows[j]["columnname"].ToString() == "Main")
                            {
                                Tbl_Name = dt.Rows[j]["tablename"].ToString();

                            }
                            else
                            {
                                string Textvalue = "";
                                string Text = "";
                                if (ds.Tables.Count > 0)
                                {
                                    for (int a = 0; a < ds.Tables.Count; a++)
                                    {
                                        if ("dt_" + dt.Rows[j]["CntrlName"].ToString() == ds.Tables[a].TableName)
                                        {

                                            if (ds.Tables[a].Rows.Count > 0)
                                            {
                                                string dltsql = " delete from " + dt.Rows[j]["tablename"] + " where APPLICANT_NO='" + Appno + "'";
                                                var daffectedRows = await Connection.ExecuteAsync(dltsql);
                                                Text = string.Join(",", ds.Tables[a].Columns.Cast<DataColumn>().Select(x => x.ColumnName)).Replace(" ", "");
                                                for (int i = 0; i < ds.Tables[a].Rows.Count; i++)
                                                {
                                                    for (int k = 0; k < ds.Tables[a].Columns.Count; k++)
                                                    {
                                                        if (Textvalue == "")
                                                        { Textvalue = "'" + ds.Tables[a].Rows[i][k] + "'"; }
                                                        else
                                                        { Textvalue = Textvalue + "," + "'" + ds.Tables[a].Rows[i][k] + "'"; }
                                                    }

                                                    string childsql = " INSERT INTO " + dt.Rows[j]["tablename"] + " (APPLICANT_NO," + Text + ") VALUES('" + Appno + "'," + Textvalue + ")";
                                                    var maffectedRows = await Connection.ExecuteAsync(childsql);
                                                    Textvalue = "";
                                                }
                                            }
                                        }
                                    }
                                }

                            }

                        }
                        string sqll = "update " + Tbl_Name + " set " + Data + ",approvestatus=" + APPROVESTATUS + ",cstatus='Open', Status=1,PENDINGWITHUSER=" + PUserid + ",PENDINGWITHDESG=" + PDesgid + ",remark='" + Remark + "' where APPLICANT_NO=" + Appno + "";
                        affectedRows = await Connection.ExecuteAsync(sqll);
                        string sql3 = "UPDATE DF_FORM_RESULT_MAIN SET STATUS=" + APPROVESTATUS + " WHERE FORMID=" + Formid + " AND APPLICANT_NO='" + Appno + "' and id>0";
                        affectedRows = await Connection.ExecuteAsync(sql3);
                    }

                }
                //string sqll1 = "insert into DF_REMARK_LOG (APPLICANT_NO,`KEY`,`VALUE`,USERID,STATUS,ACTION_NO) VALUES ('" + Appno + "','Remark','" + Remark + "'," + UserId + ",1," + ActionNo + ")";
                //affectedRows = await Connection.ExecuteAsync(sqll1);
                return affectedRows.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            };
        }
        public DataTable GateDynamicDropdownValues(string TableName, string ColumnName, string DbCondition)
        {
            try
            {
                DataTable dt = new DataTable();
                string Check = "";
                if (DbCondition == null)
                {
                    Check = "SELECT " + ColumnName.Replace(" ", "") + " FROM " + TableName + " ";
                }
                else
                {
                    Check = "SELECT " + ColumnName.Replace(" ", "") + " FROM " + TableName + " WHERE " + DbCondition + "";
                }
                IDataReader dr = Connection.ExecuteReader(Check);
                dt.Load(dr);
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet GetLastUpdatedData(int Id, int DeptId, int UserId, int DesgId, string District, string Year, string Month)
        {
            try
            {
                string Tbl_Name = "";
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                DataTable dt_data = new DataTable();
                var dyParam = new DynamicParameters();
                //string Data = "SELECT pvch_control_name,trim(substring(substring(pvch_label_name, '[(][A-Z]+[)]', ''), '[^a-zA-Z ]', '')) as pvch_label_name,int_id FROM DF_FORM_CONFIG where int_form_id=" + Id + " and pvch_label_name is not null order by int_id";
                string Data = "SELECT pvch_control_name,replace(replace(replace(pvch_label_name,'\"',''),'[',''),']','') as pvch_label_name,int_id,pvch_control_type,(select count(1) from DF_FORM_CONFIG where INT_FORM_ID=df.INT_FORM_ID and pvch_control_type='matrixdynamic') Matrixcnt,(select PRINTCONFIG from LGD_TBL_SCHEME sch inner join df_form df on sch.ID=df.SCHEMEID where df.ID=" + Id + ") PRINTCONFIG,df.Db_Table_Name,df.ValueFiled,df.DisplayFiled,(select a.table_name  from lgd_tbl_scheme a inner join df_form b on a.id=b.schemeid and b.astatus=1 where b.id = " + Id + " and a.deletedflag = 0) table_name FROM DF_FORM_CONFIG df INNER JOIN DF_FORM_CONTROL_CONFIG DFC ON DF.INT_FORM_ID=DFC.FormId AND DF.INT_ID=DFC.ConfigControlId AND ConfigControlId>0 where int_form_id=" + Id + " and pvch_label_name is not null  and DF.INT_DELETED_FLAG=0 and pvch_control_type not in('matrixdynamic','Child') order by int_id";
                IDataReader dr = Connection.ExecuteReader(Data);
                dt.Load(dr);
                //DataTable tbldt = new DataTable();
                //string Tbl_Data = "select a.table_name  from lgd_tbl_scheme a inner join df_form b on a.id=b.schemeid and b.astatus=1 where b.id = " + Id + " and a.deletedflag = 0";
                //IDataReader drtbl = Connection.ExecuteReader(Tbl_Data);
                //tbldt.Load(drtbl);
                if (dt.Rows.Count > 0)
                {
                    Tbl_Name = dt.Rows[0]["table_name"].ToString();
                    if (Tbl_Name == "")
                    {
                        Tbl_Name = "df_" + Id;
                    }
                }
                var columndata = ""; string Join_Column = ""; string Join_Table = "";
                if (dt.Rows.Count > 0)
                {
                    for (int p = 0; p < dt.Rows.Count; p++)
                    {
                        if (columndata == "")
                        {
                            if (dt.Rows[p]["Db_Table_Name"].ToString() == "")
                            {
                                columndata = dt.Rows[p]["PVCH_CONTROL_NAME"].ToString().Replace(" ", "") + " as [" + dt.Rows[p]["PVCH_LABEL_NAME"] + "|" + "]";
                            }
                            else
                            {
                                columndata = dt.Rows[p]["PVCH_CONTROL_NAME"].ToString().Replace(" ", "") + " as [" + dt.Rows[p]["PVCH_LABEL_NAME"] + "|" + dt.Rows[p]["DisplayFiled"] + "]";
                            }
                        }
                        else
                        {
                            if (dt.Rows[p]["Db_Table_Name"].ToString() == "")
                            {
                                columndata = columndata + "," + dt.Rows[p]["PVCH_CONTROL_NAME"].ToString().Replace(" ", "") + " as [" + dt.Rows[p]["PVCH_LABEL_NAME"] + "|" + "]";
                            }
                            else
                            {
                                columndata = columndata + "," + dt.Rows[p]["PVCH_CONTROL_NAME"].ToString().Replace(" ", "") + " as [" + dt.Rows[p]["PVCH_LABEL_NAME"] + "|" + dt.Rows[p]["DisplayFiled"] + "]";
                            }
                        }
                        if (dt.Rows[p]["Db_Table_Name"].ToString() != "")
                        {
                            if (dt.Rows[p]["pvch_control_type"].ToString() == "checkbox")
                            {
                                Join_Column = Join_Column + "," + "(select distinct  stuff((select ',' + u." + dt.Rows[p]["DisplayFiled"] + " from " + dt.Rows[p]["Db_Table_Name"] + " u where u." + dt.Rows[p]["DisplayFiled"] + " = " + dt.Rows[p]["DisplayFiled"] + " and " + dt.Rows[p]["ValueFiled"] + " in(select [value] from fn_split_string(df.Control2,',')) order by u.TEMPLATE_NAME for xml path('') ),1,1,'') as TEMPLATE_NAME from TBL_TEMPLATE)" + " as [" + dt.Rows[p]["DisplayFiled"] + "]";
                            }
                            else
                            {
                                Join_Column = Join_Column + "," + "" + dt.Rows[p]["DisplayFiled"] + " as [" + dt.Rows[p]["DisplayFiled"] + "]";
                                Join_Table = Join_Table + " inner join " + dt.Rows[p]["Db_Table_Name"] + "  on df." + dt.Rows[p]["pvch_control_name"] + "=" + dt.Rows[p]["Db_Table_Name"] + "." + dt.Rows[p]["ValueFiled"] + "";
                            }
                        }
                    }
                }

                string Exist_Data = "";

                StringBuilder Sb = new StringBuilder();

                Exist_Data = "SELECT " + Id + " as formid,df.INTID,df.APPLICANT_NO," + columndata + "" + Join_Column + "  FROM " + Tbl_Name + " df " + Join_Table + " where df.DELETEDFLAG=0 ";
                Sb.Append(Exist_Data);
                if (IsColumnExist(Tbl_Name, "district"))
                {
                    Sb.Append(" AND CASE WHEN '" + District + "' IN ('') THEN '" + District + "' ELSE df.district END ='" + District + "'");
                }
                if (IsColumnExist(Tbl_Name, "year"))
                {
                    Sb.Append(" AND CASE WHEN '" + Year + "' IN ('') THEN '" + Year + "' ELSE df.year END ='" + Year + "'");
                }
                if (IsColumnExist(Tbl_Name, "month"))
                {
                    Sb.Append(" AND CASE WHEN '" + Month + "' IN ('') THEN '" + Month + "' ELSE df.month END ='" + Month + "'");
                }
                Sb.Append(" order by df.intid desc");
                IDataReader dr1 = Connection.ExecuteReader(Sb.ToString());
                dt_data.Load(dr1);
                ds.Tables.Add(dt);
                ds.Tables.Add(dt_data);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataSet GetEditResultData(int Id, int Formid)
        {
            try
            {
                string DYear = ""; string DMonth = "";
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                DataTable dt_data = new DataTable();
                var dyParam = new DynamicParameters();
                string Data = "SELECT pvch_control_name,replace(replace(replace(pvch_label_name,'\"',''),'[',''),']','') as pvch_label_name FROM DF_FORM_CONFIG where int_form_id=" + Formid + " and pvch_label_name is not null and DF_FORM_CONFIG.INT_DELETED_FLAG=0 order by int_id";
                IDataReader dr = Connection.ExecuteReader(Data);
                dt.Load(dr);
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (dt.Rows[i]["pvch_label_name"].ToString().ToUpper() == "YEAR")
                    {
                        DYear = dt.Rows[i]["pvch_control_name"].ToString();
                    }
                    if (dt.Rows[i]["pvch_label_name"].ToString().ToUpper() == "MONTH")
                    {
                        DMonth = dt.Rows[i]["pvch_control_name"].ToString();
                    }
                }
                string Exist_Data = "SELECT df.*  FROM DF_" + Formid + " df where intid=" + Id + "";
                IDataReader dr1 = Connection.ExecuteReader(Exist_Data);
                dt_data.Load(dr1);
                ds.Tables.Add(dt);
                ds.Tables.Add(dt_data);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<int> DeleteResult(int resultId, int Id)
        {
            try
            {
                string Tbl_Name = "";
                DataTable tbldt = new DataTable();
                string Tbl_Data = "select a.table_name  from lgd_tbl_scheme a inner join df_form b on a.id=b.schemeid and b.astatus=1 where b.id = " + Id + " and a.deletedflag = 0";
                IDataReader drtbl = Connection.ExecuteReader(Tbl_Data);
                tbldt.Load(drtbl);
                if (tbldt.Rows.Count > 0)
                {
                    Tbl_Name = tbldt.Rows[0][0].ToString();
                }
                string sql = "Update " + Tbl_Name + " set DELETEDFLAG=1  where INTID=" + resultId + "";
                var affectedRows = await Connection.ExecuteAsync(sql);
                //await Connection.ExecuteAsync(sql, new { resultId = resultId });

                return affectedRows;
            }
            catch (Exception ex)
            {
                throw ex;
            };
        }
        public DataTable FindColumnData(int FormId/*, string ApplicantNo*/)
        {
            DataTable Dt = new DataTable();
            //string Check = "select * from DF_" + FormId + " where applicant_no='"+ ApplicantNo + "'";
            string Check = "select a.table_name  from lgd_tbl_scheme a inner join df_form b on a.id=b.schemeid and b.astatus=1 where b.id=" + FormId + " and a.deletedflag=0";
            IDataReader dr = Connection.ExecuteReader(Check);
            Dt.Load(dr);
            return Dt;
        }
        public string GateDynamicTableValues(int FormId, string ApplicantNo, string ColumnName, string Condition)
        {
            try
            {
                string Tbl_Name = "";
                DataTable tbldt = new DataTable();
                string Tbl_Data = "select a.table_name  from lgd_tbl_scheme a inner join df_form b on a.id=b.schemeid and b.astatus=1 where b.id = " + FormId + " and a.deletedflag = 0";
                IDataReader drtbl = Connection.ExecuteReader(Tbl_Data);
                tbldt.Load(drtbl);
                if (tbldt.Rows.Count > 0)
                {
                    Tbl_Name = tbldt.Rows[0][0].ToString();
                }
                DataTable dt = new DataTable();
                string Check = "";
                if (Condition != null && Condition != "")
                {
                    Check = "SELECT " + ColumnName.Replace(" ", "") + " FROM " + Tbl_Name + " WHERE APPLICANT_NO=" + ApplicantNo + " and " + Condition + "";
                }
                else
                {
                    Check = "SELECT " + ColumnName.Replace(" ", "") + " FROM " + Tbl_Name + " WHERE APPLICANT_NO=" + ApplicantNo + "";
                }
                IDataReader dr = Connection.ExecuteReader(Check);
                dt.Load(dr);
                return dt.Rows[0][ColumnName.Replace(" ", "")].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Dformconfig>> GetChildTabledetails(int FormId, string Cname)
        {

            try
            {
                var p = new DynamicParameters();
                p.Add("@P_Action", "C");
                p.Add("@P_FormId", FormId);
                p.Add("@P_ControlName", Cname);
                var result = await Connection.QueryAsync<Dformconfig>("USP_DF_FORM", p, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IEnumerable<Template>> GetColumnid(int FormId)
        {
            IEnumerable<Template> result;
            try
            {
                var dyParam = new DynamicParameters();
                dyParam.Add("P_ACTION", "GETCOL");
                dyParam.Add("P_TEMPLATEID", FormId);
                var query = "USP_TEMPLATE_VIEW_DELETE";
                result = await Connection.QueryAsync<Template>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        static async Task<string> GetResultAsync(string path)
        {
            string apiResponse = null;
            try
            {
                //path = "http://dashboard.jk.gov.in/api/GetAllDistrict";
                //HttpResponseMessage response = await client.GetAsync(path);
                //if (response.IsSuccessStatusCode)
                //{
                //    apiResponse = await response.Content.ReadAsStringAsync();
                //}
                //return apiResponse;
                //using (var httpClientHandler = new HttpClientHandler())
                //{
                //    httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
                //    using (var client = new HttpClient(httpClientHandler))
                //    {
                //        HttpResponseMessage response = await client.GetAsync(path);
                //        if (response.IsSuccessStatusCode)
                //        {
                //            apiResponse = await response.Content.ReadAsStringAsync();
                //        }
                //    }
                //}
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpClient client = new HttpClient(clientHandler);
                var unTokenedClientResponse = client.GetAsync(path).Result;
                if (unTokenedClientResponse.IsSuccessStatusCode)
                {
                    apiResponse = await unTokenedClientResponse.Content.ReadAsStringAsync();
                }
                return apiResponse;
            }
            catch (Exception Ex)
            {
                //throw Ex;
                return apiResponse;
            }
        }
        static async Task<string> PostResultAsync(string path)
        {
            string apiResponse = null;
            try
            {
                object mydata = new
                {

                };
                var myContent = JsonConvert.SerializeObject(mydata);
                var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
                var byteContent = new ByteArrayContent(buffer);

                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

                HttpClient client = new HttpClient(clientHandler);
                var unTokenedClientResponse = client.PostAsync(path, byteContent).Result;
                if (unTokenedClientResponse.IsSuccessStatusCode)
                {
                    apiResponse = await unTokenedClientResponse.Content.ReadAsStringAsync();
                }
                return apiResponse;
            }
            catch (Exception Ex)
            {
                //throw Ex;
                return apiResponse;
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
        public FormModel GetDFFormResult(int FormId)
        {
            try
            {

                var dyParam = new DynamicParameters();
                dyParam.Add("P_Action", "A");
                dyParam.Add("P_FormId", FormId);
                var query = "USP_DF_FORM";
                FormModel result = Connection.QueryFirstOrDefault<FormModel>(query, dyParam, commandType: CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public bool IsColumnExist(string TableName, string ColumnName)
        {
            try
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                var dyParam = new DynamicParameters();
                string Data = "SELECT * FROM INFORMATION_SCHEMA.COLUMNS  WHERE TABLE_NAME = '" + TableName + "' AND COLUMN_NAME = '" + ColumnName + "'";
                IDataReader dr = Connection.ExecuteReader(Data);
                dt.Load(dr);
                if (dt.Rows.Count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string LayOut(int AllignmentType, string lblText, string controls, string isRequired, string id)
        {
            string strText = "";
            if (lblText != "")
            {
                if (AllignmentType == 3)
                {
                    //strText = "<div class='col-sm-4'><label for='sss'>" + lblText + "</label>" + controls + "</div>";
                    strText = "<div class='form-group col-md-4'><label>" + lblText + "</label>" + controls + "</div>";
                }
                else if (AllignmentType == 2)
                {
                    //strText = "<label for='sss' class='col-sm-2' id='lbl_" + id + "'>" + lblText + " </label><div class='col-sm-4'><span class='colon'>:</span>" + controls + "</div>";
                    strText = "<div class='form-group col-md-6'><label>" + lblText + "</label>" + controls + "</div>";
                }
                else if (AllignmentType == 1)
                {
                    //strText = "<div class='' id='div_" + id + "'><div class='col-sm-6 margin-bottom8'><label for='sss' class='col-sm-5' id='lbl_" + id + "'>" + lblText + "</label><div class='col-sm-7'><span class='colon'>:</span>" + controls + "</div></div></div>";
                    strText = "<div class='form-group col-md-12'><label>" + lblText + "</label>" + controls + "</div>";
                }
                else if (AllignmentType == 4)
                {
                    //strText = "<div class='col-sm-12'><label for='sss'>" + lblText + "</label>" + controls + "</div>";
                    strText = "<div class='form-group col-md-3'><label>" + lblText + "</label>" + controls + "</div>";
                }
                else
                {
                    strText = "<div class='form-group col-md-4'><label>" + lblText + "</label>" + controls + "</div>";
                    //strText = "<div class='form-group col-md-4'><label for='col-12 col-md-6 col-sm-2 control-label'>" + lblText + "</label>" + controls + "</div>";
                    //<div class='col-sm-3'></div>
                }
            }
            else
            {
                strText = "<div class='col-sm-12'>" + controls + "</div>";
            }
            return strText;
        }
        public async Task<string> InsertData(string Result, int FORMID, int Userid, string Status, int DeptId)
        {
            try
            {
                List<string> elements = new List<string>();
                DataTable dtt = new DataTable();
                DataSet ds = new DataSet();
                string TableName = "";

                DateTime d = DateTime.Now;
                string dateString = d.ToString("yyyyMMddHHmmss");
                string Refid = "R" + d.ToString("yyMMddHHmmss");


                string Check = "SELECT dr.tablename,dr.columnname,df.schemeid,REPLACE(dr.TABLENAME, (select tablename from DF_FORM_REFERENCE WHERE FORMID=" + FORMID + " and columnname='Main')+'_', '') CntrlName FROM DF_FORM_REFERENCE dr inner join DF_FORM df on dr.formid = df.id WHERE dr.FORMID=" + FORMID + "";
                IDataReader dr = await Connection.ExecuteReaderAsync(Check);
                DataTable dt = new DataTable();
                dt.Load(dr);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < Result.Split('|').Length; i++)
                    {
                        if (Result.Split('|')[i] != "")
                        {
                            if (Result.Split('|')[i].Contains(":{"))
                            {
                                string Dtname = ("dt_" + Result.Split('|')[i].Split(new[] { ":{" }, StringSplitOptions.None)[0].Replace(@"\", "//")).Replace("\"", "");
                                ds.Tables.Add(convertToDataTable("{" + Result.Split('|')[i].Split(new[] { ":{" }, StringSplitOptions.None)[1].Replace(@"\", "//"), Dtname));
                            }
                            else
                            {
                                dtt = convertToDataTable(Result.Split('|')[i].Replace(@"\", "//"), "tblMain");
                            }
                        }
                    }
                    string[] columnNames = dtt.Columns.Cast<DataColumn>().Select(x => x.ColumnName.Replace(" ", "")).ToArray();
                    for (int i = 0; i < dtt.Columns.Count; i++)
                    {
                        elements.Add(dtt.Rows[0][i].ToString());
                    }

                    for (int j = 0; j < dt.Rows.Count; j++)
                    {
                        if (dt.Rows[j]["columnname"].ToString() == "Main")
                        {
                            TableName = dt.Rows[j]["tablename"].ToString();

                        }
                        else
                        {
                            string Textvalue = "";
                            string Text = "";
                            if (ds.Tables.Count > 0)
                            {
                                for (int a = 0; a < ds.Tables.Count; a++)
                                {
                                    if ("dt_" + dt.Rows[j]["CntrlName"].ToString() == ds.Tables[a].TableName)
                                    {
                                        if (ds.Tables[a].Rows.Count > 0)
                                        {
                                            Text = string.Join(",", ds.Tables[a].Columns.Cast<DataColumn>().Select(x => x.ColumnName)).Replace(" ", "");
                                            for (int i = 0; i < ds.Tables[a].Rows.Count; i++)
                                            {
                                                for (int k = 0; k < ds.Tables[a].Columns.Count; k++)
                                                {
                                                    if (Textvalue == "")
                                                    { Textvalue = "'" + ds.Tables[a].Rows[i][k] + "'"; }
                                                    else
                                                    { Textvalue = Textvalue + "," + "'" + ds.Tables[a].Rows[i][k] + "'"; }
                                                }
                                                string childsql = " INSERT INTO " + dt.Rows[j]["tablename"] + " (APPLICANT_NO," + Text + ") VALUES('" + dateString + "'," + Textvalue + ")";
                                                var maffectedRows = await Connection.ExecuteAsync(childsql);
                                                Textvalue = "";
                                            }
                                        }
                                    }
                                }
                            }

                        }

                    }
                    var colvalue = "'" + String.Join("','", elements) + "'";
                    string PUserid = "0";
                    string PDesgid = "0";

                    int caffectedRows = 0;
                    if (Status == "0")
                    {
                        string csql = " INSERT INTO " + TableName + " (APPLICANT_NO,REFID,INTCREATEDBY,status,PENDINGWITHUSER,PENDINGWITHDESG,APPROVESTATUS," + string.Join(",", columnNames) + ") VALUES('" + dateString + "','" + Refid + "'," + Userid + ",1," + PUserid + "," + PDesgid + ",0," + colvalue + ")";
                        caffectedRows = Convert.ToInt32(Connection.ExecuteScalar(csql));
                    }
                    else
                    {
                        string csql = " INSERT INTO " + TableName + " (APPLICANT_NO,REFID,INTCREATEDBY,APPROVESTATUS," + string.Join(",", columnNames) + ") VALUES('" + dateString + "','" + Refid + "'," + Userid + ",0," + colvalue + ")";
                        caffectedRows = Convert.ToInt32(Connection.ExecuteScalar(csql));
                    }
                    string Csql = "SELECT max(INTID) from " + TableName + "";
                    IDataReader dr11 = await Connection.ExecuteReaderAsync(Csql);
                    DataTable dt11 = new DataTable();
                    dt11.Load(dr11);
                    if (dt11.Rows.Count > 0)
                    {
                        string sql = "insert into DF_ACTION_LOG (formid,resultid,userid,status) VALUES (" + FORMID + ",  " + dt11.Rows[0][0] + "," + Userid + ",' 0 ')";
                        var affectedRows = await Connection.ExecuteAsync(sql);
                    }

                    var p = new DynamicParameters();
                    string year = "";
                    string month = "";
                    p.Add("P_Action", "I");
                    p.Add("P_INTID", FORMID);
                    p.Add("P_CREATEDBY", Userid);
                    p.Add("P_APPLICANT_NO", dateString);
                    p.Add("P_DistId", DeptId);
                    if (columnNames.Contains("year"))
                    {
                        year = dtt.Rows[0]["year"].ToString();
                    }
                    if (columnNames.Contains("month"))
                    {
                        month = dtt.Rows[0]["month"].ToString();
                    }
                    p.Add("P_YEAR", year);
                    p.Add("P_MONTH", month);
                    p.Add("@P_Msg", SqlDbType.VarChar, direction: ParameterDirection.Output, size: 5215585);
                    p.Add("@P_Msg_Out", SqlDbType.VarChar, direction: ParameterDirection.Output, size: 5215585);
                    var results = await Connection.QueryAsync<dynamic>("USP_DYNAMIC_TABLE", p, commandType: CommandType.StoredProcedure);
                }
                return dateString;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable convertToDataTable(string j, string StrTableName)
        {
            DataTable dt = new DataTable();
            dt.TableName = StrTableName;
            if (j.Contains(",{"))
            {
                for (int i = 0; i < j.Split(new[] { ",{" }, StringSplitOptions.None).Length; i++)
                {
                    var data = "";
                    if (i == 0)
                    {
                        data = j.Split(new[] { ",{" }, StringSplitOptions.None)[i];
                    }
                    else
                    {
                        data = "{" + j.Split(new[] { ",{" }, StringSplitOptions.None)[i];
                    }
                    JToken JO = JToken.Parse(data);
                    switch (JO.Type)
                    {
                        case JTokenType.Array:
                            dt = convertJArrayToTable(data, dt);
                            break;
                        case JTokenType.Object:
                            dt = convertJObjectToTable(data, dt);
                            break;
                    }
                }
            }
            else
            {
                JToken JO = JToken.Parse(j);
                switch (JO.Type)
                {
                    case JTokenType.Array:
                        dt = convertJArrayToTable(j, dt);
                        break;
                    case JTokenType.Object:
                        dt = convertJObjectToTable(j, dt);
                        break;
                }
            }
            return dt;

        }
        public DataTable convertJArrayToTable(string j, DataTable dt)
        {
            var response = JsonConvert.DeserializeObject<List<Dictionary<string, Object>>>(j);

            //DataTable dt = new DataTable();
            if (response != null)
            {
                dt.Columns.AddRange(response.FirstOrDefault().Select(x =>
                new DataColumn
                {
                    ColumnName = x.Key
                }).ToArray());

                foreach (Dictionary<string, Object> dict in response)
                {
                    DataRow dr = dt.NewRow();
                    foreach (var kv in dict)
                    {
                        dr[kv.Key] = kv.Value.ToString();

                    }
                    dt.Rows.Add(dr);
                }
            }
            return dt;

        }
        public DataTable convertJObjectToTable(string j, DataTable dt)
        {
            var response = JsonConvert.DeserializeObject<Dictionary<string, Object>>(j);

            //DataTable dt = new DataTable();
            if (response != null)
            {
                if (dt.Rows.Count == 0)
                {
                    dt.Columns.AddRange(response.Select(x =>
                    new DataColumn
                    {
                        ColumnName = x.Key
                    }).ToArray());
                }
                DataRow dr = dt.NewRow();
                foreach (var kv in response)
                {
                    dr[kv.Key] = kv.Value.ToString();

                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public async Task<IEnumerable<FormModel>> GetFormName()
        {
            try
            {
                var p = new DynamicParameters();
                p.Add("@Action", "A");
                p.Add("PMSGOUT", dbType: DbType.String, direction: ParameterDirection.Output, size: 5215585);
                var results = await Connection.QueryAsync<FormModel>("USP_DF_CONTROL_CONFIG", p, commandType: CommandType.StoredProcedure);
                return results;
            }
            catch
            {
                throw;
            };
        }
        public bool IsRecordAlreadyExist(int FormId, string Result, string ApplicantNo)
        {
            try
            {
                dynamic dynJson = JsonConvert.DeserializeObject(Result);
                bool IsExist = false;
                string ConstraintsSelect = "select PVCH_CONTROL_NAME from DF_FORM_CONFIG where int_id in (select ConfigControlId from DF_FORM_CONSTRAINT_CONFIG where FormId=" + FormId + " and ConfigControlId not in (0))";
                IDataReader drct = Connection.ExecuteReader(ConstraintsSelect);
                DataTable dtct = new DataTable();
                dtct.Load(drct);
                string TableSelect = "select TABLE_NAME from LGD_TBL_SCHEME sc join DF_FORM df on sc.ID = df.SCHEMEID where df.ID = " + FormId + "";
                IDataReader tct = Connection.ExecuteReader(TableSelect);
                DataTable dtt = new DataTable();
                dtt.Load(tct);
                string TableName = string.Empty;
                if (dtt.Rows[0]["TABLE_NAME"].ToString() != null)
                {
                    TableName = dtt.Rows[0]["TABLE_NAME"].ToString();
                }
                else
                {
                    TableName = "DF_" + FormId;
                }
                List<bool> ListIsExist = new List<bool>();
                if (dtct.Rows.Count > 0)
                {
                    for (int i = 0; i < dtct.Rows.Count; i++)
                    {
                        DataTable dt = new DataTable();
                        string Check = string.Empty;
                        if (ApplicantNo != null)
                        {
                            Check = "SELECT count(1) as count FROM " + TableName + " where DELETEDFLAG=0 and " + dtct.Rows[i]["PVCH_CONTROL_NAME"].ToString() + "=upper('" + dynJson[dtct.Rows[i]["PVCH_CONTROL_NAME"].ToString()] + "') and APPLICANT_NO!='" + ApplicantNo + "'";
                        }
                        else
                        {
                            Check = "SELECT count(1) as count FROM " + TableName + " where DELETEDFLAG=0 and " + dtct.Rows[i]["PVCH_CONTROL_NAME"].ToString() + "=upper('" + dynJson[dtct.Rows[i]["PVCH_CONTROL_NAME"].ToString()] + "')";
                        }
                        IDataReader dr = Connection.ExecuteReader(Check);
                        dt.Load(dr);
                        if (dt.Rows.Count > 0)
                        {
                            if (Convert.ToInt32(dt.Rows[0]["count"].ToString()) > 0)
                            {
                                ListIsExist.Add(true);
                            }
                        }
                    }
                }
                if (dtct.Rows.Count != 0 && dtct.Rows.Count == ListIsExist.Count)
                {
                    IsExist = true;
                }
                else
                {
                    IsExist = false;
                }
                return IsExist;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable FindChildTableData(int Formid, string TableName, string ControlName, string Applicant_No)
        {
            var columndata = "";
            DataTable chlddt = new DataTable();
            string chl_Data = "select PVCH_CHILD_CONTROL_NAME,PVCH_CHILD_LABEL_NAME from DF_FORM_CONFIG where INT_FORM_ID='" + Formid + "' and PVCH_PRNT_CHILD_LABEL_NAME='" + ControlName + "'";
            IDataReader chdrtbl = Connection.ExecuteReader(chl_Data);
            chlddt.Load(chdrtbl);
            if (chlddt.Rows.Count > 0)
            {
                for (int p = 0; p < chlddt.Rows.Count; p++)
                {
                    if (columndata == "")
                    {
                        columndata = chlddt.Rows[p]["PVCH_CHILD_CONTROL_NAME"].ToString().Replace(" ", "") + " as [" + chlddt.Rows[p]["PVCH_CHILD_LABEL_NAME"] + "]";
                    }
                    else
                    {
                        columndata = columndata + "," + chlddt.Rows[p]["PVCH_CHILD_CONTROL_NAME"].ToString().Replace(" ", "") + " as [" + chlddt.Rows[p]["PVCH_CHILD_LABEL_NAME"] + "]";
                    }
                }
            }

            string Tbl_Name = TableName + "_" + ControlName;
            DataTable tbldt = new DataTable();
            string Tbl_Data = "select (select PVCH_LABEL_NAME from DF_FORM_CONFIG where INT_FORM_ID='" + Formid + "' and PVCH_CONTROL_TYPE='matrixdynamic' and PVCH_CONTROL_NAME='" + ControlName + "') PVCH_LABEL_NAME," + columndata + " from " + Tbl_Name + " where Applicant_No='" + Applicant_No + "'";
            IDataReader drtbl = Connection.ExecuteReader(Tbl_Data);
            tbldt.Load(drtbl);
            return tbldt;
        }
    }

}
