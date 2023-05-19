using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using WorkFlowEngine.IRepository.WFEngine;
using WorkFlowEngine.Web.Models;
using WorkFlowEngine.Domain.WFEngine;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Linq;
using WorkFlowEngine.Web.Areas.WFERender.Models;
using Serilog.Core;
using Serilog;
using System.Reflection;
using System.Collections;
using System.Text;
using WorkFlowEngine.Web.Models.Extensions;

namespace WorkFlowEngine.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly IConfiguration _config;
        private IHostingEnvironment _hostingEnvironment;
        private IComponentRepository _iComponentRepository { get; }
        private IFormTableServices _iFormTableServices { get; }

        public AdminController(IConfiguration config, IHostingEnvironment hostingEnvironment, IComponentRepository iComponentRepository, IFormTableServices iFormTableServices)
        {
            _config = config;
            _hostingEnvironment = hostingEnvironment;
            _iComponentRepository = iComponentRepository;
            _iFormTableServices = iFormTableServices;
        }
        [HttpGet]
        public async Task<IActionResult> Scheme()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    ViewBag.Department = _iComponentRepository.GetDEPARTMENTDetails().Result;
                    ViewBag.Template = _iComponentRepository.GetTemplates().Result;
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return BadRequest();
            }
        }
        public async Task<JsonResult> GetSchemesWithForm(int DeptId)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    var deps = await _iComponentRepository.GetSchemesWithForm(DeptId);
                    return Json(deps);
                }
                else
                {
                    RedirectToAction("Logout", "Account");
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost]
        public async Task<JsonResult> UpdateSchemeName(Forms scheme)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    scheme.SCHEME_NAME = scheme.SCHEME_NAME.Trim();
                    if (string.IsNullOrEmpty(scheme.SCHEME_NAME))
                        return Json(new { status = 3, msg = "Scheme name cannot' be empty" });
                    //if (scheme.SCHEME_NAME.HasSpecialChar())
                    //    return Json(new { status = 3, msg = "Only alphanumeric allowed " });
                    //if (_iComponentRepository.CheckDuplicate("lgd_tbl_scheme", "scheme_name", scehmeName, deptId).Result > 0)
                    //    return Json(new { status = 2, msg = $"{scehmeName} already exists in schemes" });

                    #region code for image upload
                    string flodername = "SchemeDocuments";
                    string scriptflodername = "ScriptDocuments";
                    string webrootpath = _hostingEnvironment.WebRootPath;
                    string ProcDocPath = Path.Combine(webrootpath, flodername);
                    if (!Directory.Exists(ProcDocPath))
                        Directory.CreateDirectory(ProcDocPath);
                    string ScriptProcDocPath = Path.Combine(webrootpath, scriptflodername);
                    if (!Directory.Exists(ScriptProcDocPath))
                        Directory.CreateDirectory(ScriptProcDocPath);
                    var file = Request.Form.Files;
                    // file.CopyTo(file);


                    var fullname = "";
                    var ScriptFileName = "";
                    if (file.Count > 0)
                    {
                        for (int i = 0; i < file.Count; i++)
                        {
                            var filename = Path.GetExtension(ContentDispositionHeaderValue.Parse(file[i].ContentDisposition).FileName.Trim('"'));
                            if (filename == ".js")
                            {
                                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                ScriptFileName = file[i].Name.Remove(file[i].Name.Length - 3) + timestamp + "" + filename;
                                using (var stream = new FileStream(Path.Combine(ScriptProcDocPath, ScriptFileName), FileMode.Create))
                                {
                                    file[i].CopyTo(stream);
                                }
                            }
                            else
                            {
                                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                fullname = "scheme" + timestamp + "" + filename;
                                using (var stream = new FileStream(Path.Combine(ProcDocPath, fullname), FileMode.Create))
                                {
                                    file[i].CopyTo(stream);
                                }
                            }

                        }
                    }
                    else
                    {
                        fullname = scheme.GOAL_IMAGE;
                    }
                    #endregion
                    if (scheme.ID != 0)
                    {

                        int x = _iComponentRepository.UpdateSchemeName(scheme.ID, scheme.SCHEME_NAME, scheme.INSERT_NAME, scheme.UPDATE_NAME, scheme.DELETE_NAME, scheme.SELECT_NAME, fullname, scheme.TEMPLATEID, ScriptFileName, scheme.TABLE_NAME, scheme.FORMTYPE, scheme.PRINTCONFIG);
                        return Json(new { status = x });
                    }
                    else
                    {
                        int x = _iComponentRepository.InsertScheme(scheme.DEPTID, scheme.SCHEME_NAME, scheme.INSERT_NAME, scheme.UPDATE_NAME, scheme.DELETE_NAME, scheme.SELECT_NAME, fullname, scheme.TEMPLATEID, ScriptFileName, scheme.TABLE_NAME, scheme.FORMTYPE, scheme.PRINTCONFIG);
                        return Json(new { status = x });
                    }
                }
                else
                {
                    RedirectToAction("Logout", "Account");
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }

        public async Task<JsonResult> DeleteScehem(int id)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    var result = await _iComponentRepository.DeleteScehem(id);
                    if (result.successid != 1)
                    {
                        return Json(new { status = 0, msg = result.successmessage });
                    }
                    return Json(result);
                }
                else
                {
                    RedirectToAction("Logout", "Account");
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        public async Task<IActionResult> AddNewForm(string suggestedname, int schemeId = 0, int BLOCKID = 0)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                    int secondsSinceEpoch = (int)t.TotalSeconds;
                    string name = string.IsNullOrEmpty(suggestedname) ? "NewForm-" + secondsSinceEpoch : suggestedname;
                    int ID = await _iFormTableServices.CreateSurveyGetId(name, "{}", schemeId);
                    ID = await _iFormTableServices.GetLastInsertFormId();
                    // var lastId = await _iComponentRepository.LastInsertId("df_form", "id");
                    return Redirect("/Admin/NewFormCreator?id=" + ID + "&name=" + name + "");
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        public IActionResult NewFormCreator()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }

        }
        [HttpPost]
        public async Task<string> SaveForApprove(string Id)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    try
                    {
                        var count = "";
                        if (Convert.ToInt32(Id) < 1)
                            return "0";  // Invalid

                        count = await _iFormTableServices.SaveApproveData(Id);
                        return count;
                    }
                    catch
                    {
                        throw;
                    }
                }
                else
                {
                    RedirectToAction("Logout", "Account");
                    return "0";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost("changeJson")]
        public async Task<IActionResult> ChangeJson([FromBody] ChangeFormModel model)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    try
                    {
                        //var db = new SessionStorage(HttpContext.Session);
                        //db.StoreSurvey(model.Id, model.Json);
                        await _iFormTableServices.StoreSurvey(model.Id, model.Json);

                        JObject o = JObject.Parse(model.Json);
                        List<Element> objs = new List<Element>();
                        var elementsP1 = o.SelectToken("pages[0].elements", false); //pages[0].elements[0].type

                        foreach (var item in elementsP1.Children())
                        {
                            var element = new Element();
                            element.name = (string)item.SelectToken("name", false);
                            element.type = (string)item.SelectToken("type", false);
                            element.title = (string)item.SelectToken("title", false);
                            element.isRequired = (string)item.SelectToken("isRequired", false);
                            objs.Add(element);
                        }
                        
                        return Json(model.Json);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }

        [HttpGet]
        public async Task<IActionResult> PendingForm()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    FormModel Survey = new FormModel();
                    var HId = _config.GetValue<int>("MySettings:HierarchyId");
                    ViewBag.Department = _iFormTableServices.GetDepartment(Survey.DEPTID, HId).Result;
                    ViewBag.FormList = _iFormTableServices.GetNoApproveData(0, Survey);
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost]
        public async Task<IActionResult> PendingForm(FormModel Survey)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    var HId = _config.GetValue<int>("MySettings:HierarchyId");
                    ViewBag.Department = _iFormTableServices.GetDepartment(Survey.DEPTID, HId).Result;
                    ViewBag.FormList = _iFormTableServices.GetNoApproveData(0, Survey);
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        public async Task<IActionResult> RejectForm()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    FormModel Survey = new FormModel();
                    var HId = _config.GetValue<int>("MySettings:HierarchyId");
                    ViewBag.Department = _iFormTableServices.GetDepartment(Survey.DEPTID, HId).Result;
                    ViewBag.FormList = _iFormTableServices.GetNoApproveData(2, Survey);
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost]
        public async Task<IActionResult> RejectForm(FormModel Survey)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    var HId = _config.GetValue<int>("MySettings:HierarchyId");
                    ViewBag.Department = _iFormTableServices.GetDepartment(Survey.DEPTID, HId).Result;
                    ViewBag.FormList = _iFormTableServices.GetNoApproveData(2, Survey);
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpGet]
        public async Task<IActionResult> FormApprove()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    FormModel Survey = new FormModel();
                    var HId = _config.GetValue<int>("MySettings:HierarchyId");
                    ViewBag.Department = _iFormTableServices.GetDepartment(Survey.DEPTID, HId).Result;
                    ViewBag.FormList = _iFormTableServices.GetNoApproveData(1, Survey);
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost]
        public async Task<IActionResult> FormApprove(FormModel Survey)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    var HId = _config.GetValue<int>("MySettings:HierarchyId");
                    ViewBag.Department = _iFormTableServices.GetDepartment(Survey.DEPTID, HId).Result;
                    ViewBag.FormList = _iFormTableServices.GetNoApproveData(1, Survey);
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost]
        public async Task<string> ConvertJsonToTable(string Id, string remark, string status)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    try
                    {
                        var count = "";
                        if (Convert.ToInt32(Id) < 1)
                            return "0";  // Invalid
                        var modObj = await _iFormTableServices.GetFormData(Convert.ToInt32(Id));
                        var elements = modObj.GetObjectFromElements();
                        count = await _iFormTableServices.CreateTable(elements, Convert.ToInt32(Id), modObj.FORMID, remark, status, modObj.FORMTYPE);
                        return count;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    RedirectToAction("Logout", "Account");
                    return "0";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpGet]
        public async Task<IActionResult> ConfigView()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    ViewBag.FormName = _iFormTableServices.GetFormName().Result;
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        public async Task<JsonResult> GetFormColumnData(int FormId)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    var deps = await _iFormTableServices.GetColumnData(FormId);
                    return Json(deps);
                }
                else
                {
                    RedirectToAction("Logout", "Account");
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost]
        public async Task<JsonResult> SaveFormColumnData(Domain.WFERender.Dformconfig col)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    try
                    {
                        var value = HttpContext.Session.GetInt32("_UserId");
                        if (!string.IsNullOrEmpty(value.ToString()))
                        {
                            string retMsg = "";
                            var data = HttpContext.Request.Form["Elements"];
                            var ResultDtls = JsonConvert.DeserializeObject<List<Domain.WFERender.Dformconfig>>(data);
                            var xEle = new XElement("person",
                                        from emp in ResultDtls
                                        select new XElement("row",
                                                       new XElement("ConfigControlId", emp.ConfigControlId)
                                                   ));
                            col.ColumnXml = xEle;
                            retMsg = _iFormTableServices.InsertColumnconfigData(col);

                            return Json(retMsg);
                        }
                        else
                        {
                            RedirectToAction("Logout", "Home");
                            return Json(null);
                        }

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    RedirectToAction("Logout", "Account");
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        public async Task<JsonResult> UploadFile(PostFormResult model)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    #region code for image upload
                    string flodername = "Image_" + model.Foldername + "_" + model.postId;
                    string webrootpath = _hostingEnvironment.WebRootPath;
                    string ProcDocPath = Path.Combine(webrootpath, flodername);
                    if (!Directory.Exists(ProcDocPath))
                        Directory.CreateDirectory(ProcDocPath);
                    var file = Request.Form.Files;
                    // file.CopyTo(file);


                    var fullname = "";
                    if (file.Count > 0)
                    {
                        //var filename = file[0].Name;
                        //Path.GetExtension(ContentDispositionHeaderValue.Parse(file[0].ContentDisposition).FileName.Trim('"'));
                        //var timestamp = DateTime.Now.ToString("yyyyMMddhhmmss");
                        fullname = model.Filename;
                        using (var stream = new FileStream(Path.Combine(ProcDocPath, fullname), FileMode.Create))
                        {
                            file[0].CopyTo(stream);
                        }
                    }

                    #endregion

                    return Json(fullname);
                }
                else
                {
                    RedirectToAction("Logout", "Home");
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpGet]
        public async Task<IActionResult> FormList()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    ViewBag.FormList = _iFormTableServices.GetFormName();
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpGet]
        public IActionResult TestForm()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpGet]
        public async Task<IActionResult> ConfigConstraint()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    ViewBag.FormName = _iFormTableServices.GetFormName().Result;
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        public async Task<JsonResult> GetFormConstraintData(int FormId)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    var deps = await _iFormTableServices.GetConstraintData(FormId);
                    return Json(deps);
                }
                else
                {
                    RedirectToAction("Logout", "Account");
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost]
        public async Task<JsonResult> SaveFormConstraintData(Domain.WFERender.Dformconfig col)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    try
                    {
                        var value = HttpContext.Session.GetInt32("_UserId");
                        if (!string.IsNullOrEmpty(value.ToString()))
                        {
                            string retMsg = "";
                            var data = HttpContext.Request.Form["Elements"];
                            var ResultDtls = JsonConvert.DeserializeObject<List<Domain.WFERender.Dformconfig>>(data);
                            var xEle = new XElement("person",
                                        from emp in ResultDtls
                                        select new XElement("row",
                                                       new XElement("ConfigControlId", emp.ConfigControlId)
                                                   ));
                            col.ColumnXml = xEle;
                            retMsg = _iFormTableServices.InsertConstraintConfigData(col);

                            return Json(retMsg);
                        }
                        else
                        {
                            RedirectToAction("Logout", "Home");
                            return Json(null);
                        }

                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                else
                {
                    RedirectToAction("Logout", "Account");
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpGet]
        public async Task<IActionResult> ReadHtml()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    ViewBag.Department = _iComponentRepository.GetDEPARTMENTDetails().Result;
                    ViewBag.Template = _iComponentRepository.GetTemplates().Result;
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return View();
            }
        }
        [HttpPost]
        public async Task<JsonResult> ReadHtmlControl(Forms scheme)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    #region code for image upload
                    string flodername = "SchemeDocuments";
                    string scriptflodername = "ScriptDocuments";
                    string webrootpath = _hostingEnvironment.WebRootPath;
                    string ProcDocPath = Path.Combine(webrootpath, flodername);
                    if (!Directory.Exists(ProcDocPath))
                        Directory.CreateDirectory(ProcDocPath);
                    string ScriptProcDocPath = Path.Combine(webrootpath, scriptflodername);
                    if (!Directory.Exists(ScriptProcDocPath))
                        Directory.CreateDirectory(ScriptProcDocPath);
                    var file = Request.Form.Files;
                    // file.CopyTo(file);


                    var fullname = "";
                    var ScriptFileName = "";
                    if (file.Count > 0)
                    {
                        for (int i = 0; i < file.Count; i++)
                        {
                            var filename = Path.GetExtension(ContentDispositionHeaderValue.Parse(file[i].ContentDisposition).FileName.Trim('"'));
                            if (filename == ".js")
                            {
                                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                ScriptFileName = file[i].Name.Remove(file[i].Name.Length - 3) + timestamp + "" + filename;
                                using (var stream = new FileStream(Path.Combine(ScriptProcDocPath, ScriptFileName), FileMode.Create))
                                {
                                    file[i].CopyTo(stream);
                                }
                            }
                            else
                            {
                                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                fullname = "scheme" + timestamp + "" + filename;
                                using (var stream = new FileStream(Path.Combine(ProcDocPath, fullname), FileMode.Create))
                                {
                                    file[i].CopyTo(stream);
                                }
                            }

                        }
                    }
                    else
                    {
                        fullname = scheme.GOAL_IMAGE;
                    }

                    #endregion
                    string path = Path.Combine(this._hostingEnvironment.WebRootPath, flodername + "/") + fullname;
                    //var text = System.IO.File.ReadAllText(path);
                    //ViewBag.Htmltext = text;
                    //string entireText = "<input|<select";
                    //var data = FindSurroundingLines(text, entireText, 1);
                    return Json(new { fullname });
                }
                else
                {
                    RedirectToAction("Logout", "Account");
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpPost]
        public async Task<JsonResult> InsertSchemethroughHtml(Forms scheme)
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    scheme.SCHEME_NAME = scheme.SCHEME_NAME.Trim();
                    if (string.IsNullOrEmpty(scheme.SCHEME_NAME))
                        return Json(new { status = 3, msg = "Scheme name cannot' be empty" });

                    #region code for image upload
                    string flodername = "SchemeDocuments";
                    string scriptflodername = "ScriptDocuments";
                    string webrootpath = _hostingEnvironment.WebRootPath;
                    string ProcDocPath = Path.Combine(webrootpath, flodername);
                    if (!Directory.Exists(ProcDocPath))
                        Directory.CreateDirectory(ProcDocPath);
                    string ScriptProcDocPath = Path.Combine(webrootpath, scriptflodername);
                    if (!Directory.Exists(ScriptProcDocPath))
                        Directory.CreateDirectory(ScriptProcDocPath);
                    var file = Request.Form.Files;
                    // file.CopyTo(file);


                    var fullname = "";
                    var ScriptFileName = "";
                    if (file.Count > 0)
                    {
                        for (int i = 0; i < file.Count; i++)
                        {
                            var filename = Path.GetExtension(ContentDispositionHeaderValue.Parse(file[i].ContentDisposition).FileName.Trim('"'));
                            if (filename == ".js")
                            {
                                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                ScriptFileName = file[i].Name.Remove(file[i].Name.Length - 3) + timestamp + "" + filename;
                                using (var stream = new FileStream(Path.Combine(ScriptProcDocPath, ScriptFileName), FileMode.Create))
                                {
                                    file[i].CopyTo(stream);
                                }
                            }
                            else
                            {
                                var timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                fullname = "scheme" + timestamp + "" + filename;
                                using (var stream = new FileStream(Path.Combine(ProcDocPath, fullname), FileMode.Create))
                                {
                                    file[i].CopyTo(stream);
                                }
                            }

                        }
                    }
                    else
                    {
                        fullname = scheme.GOAL_IMAGE;
                    }
                    #endregion
                    if (scheme.ID != 0)
                    {
                        _iComponentRepository.UpdateSchemeName(scheme.ID, scheme.SCHEME_NAME, scheme.INSERT_NAME, scheme.UPDATE_NAME, scheme.DELETE_NAME, scheme.SELECT_NAME, fullname, scheme.TEMPLATEID, ScriptFileName, scheme.TABLE_NAME, scheme.FORMTYPE, scheme.PRINTCONFIG);
                        return Json(new { status = 2 });
                    }
                    else
                    {
                        var data = _iComponentRepository.InsertScheme(scheme.DEPTID, scheme.SCHEME_NAME, scheme.INSERT_NAME, scheme.UPDATE_NAME, scheme.DELETE_NAME, scheme.SELECT_NAME, fullname, scheme.TEMPLATEID, ScriptFileName, scheme.TABLE_NAME, "", scheme.PRINTCONFIG);
                        int ID = await _iFormTableServices.CreateSurveyGetId(scheme.SCHEME_NAME, scheme.JSONSTRING, Convert.ToInt32(data));
                        return Json(new { status = 1 });
                    }
                }
                else
                {
                    RedirectToAction("Logout", "Account");
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return null;
            }
        }
        [HttpGet]
        public async Task<IActionResult> TestPage()
        {
            try
            {
                var UserId = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(UserId.ToString()))
                {
                    return View();
                }
                else
                {
                    return RedirectToAction("Logout", "Account");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return View();
            }
        }
        [HttpGet]
        public IActionResult TakeActionInView(int Formid, string Applicant_No)
        {
            var value = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                ViewBag.Result = _iFormTableServices.GetTakeActionData(Formid, Applicant_No);
                ViewBag.Formid = Formid;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        [HttpGet]
        public IActionResult ViewDetailsView(int Formid, string Applicant_No)
        {
            var value = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                ViewBag.Result = _iFormTableServices.GetTakeActionData(Formid, Applicant_No);
                ViewBag.Formid = Formid;
                return View();
            }
            else
            {
                return RedirectToAction("Logout", "Home");
            }
        }
        public JsonResult DisplayUserNameByDept(string Value_Field, string Display_Field, string Table_Name, string listkey)
        {
            try
            {
                var value = HttpContext.Session.GetInt32("_UserId");
                if (!string.IsNullOrEmpty(value.ToString()))
                {
                    var result = _iFormTableServices.GetDropdownData(Value_Field, Display_Field, Table_Name, listkey);
                    return Json(result);
                }
                else
                {
                    RedirectToAction("Logout", "Home");
                    return Json(null);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
