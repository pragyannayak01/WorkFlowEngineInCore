using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WorkFlowEngine.Web.Areas.WFERender.Models;
using System.Net;

namespace WorkFlowEngine.Web.Areas.WFERender.Controllers
{
    [Area("WFERender")]
    public class AdminController : Controller
    {
        private readonly IConfiguration _config;
        HttpClientHandler _clientHandler = new HttpClientHandler();
        string UploadPath = string.Empty;
        string RenderAPIURL = string.Empty;
        string AppPath = string.Empty;
        public AdminController(IConfiguration config)
        {
            _config = config;
            //getting values from wfe appsetting
            var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"DynamicFormJson.json");
            var JSON = System.IO.File.ReadAllText(folderDetails);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(JSON);
            UploadPath = jsonObj["Client"]["UploadPath"].ToString();
            RenderAPIURL = jsonObj["Client"]["RenderAPIURL"].ToString();
            AppPath = jsonObj["Client"]["AppPath"].ToString();

        }
        public async Task<IActionResult> ViewForm(int FormId, string ApplicantNo, string Vtype)
        {
            try
            {
                ViewBag.UploadPath = UploadPath;
                ViewBag.AppPath = AppPath;
                ViewBag.Type = Vtype;
                using (var httpClient = new HttpClient(_clientHandler))
                {
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    //ViewBag.Form = await httpClient.GetStringAsync(jsonObj["Client"]["RenderAPIURL"].ToString()+ "GetCreatedForm?FormId=" + FormId + "&ApplicantNo=" + ApplicantNo);
                    var FormData = await httpClient.GetStringAsync(RenderAPIURL + "GetCreatedForm?FormId=" + FormId + "&ApplicantNo=" + ApplicantNo);
                    //http://localhost:44323/Api/Admin/GetCreatedForm?FormId=57&ApplicantNo=
                    ViewBag.Form = FormData;
                    ViewBag.FormType = FormData.Split("|")[8];
                }
            }
            catch (Exception ex)
            {
                throw;
            }
            return View();
        }
        //[HttpPost]
        //public async Task<IActionResult> InsertFormResult(PostFormResult MD)
        //{
        //    try
        //    {
        //        using (var httpClient = new HttpClient(_clientHandler))
        //        {
        //            var response = await httpClient.PostAsJsonAsync(RenderAPIURL + "InsertResult", MD);
        //            if (response.StatusCode.ToString() != "ServiceUnavailable")
        //            {
        //                string apiResponse = await response.Content.ReadAsStringAsync();
        //                string result = JsonConvert.DeserializeObject<string>(apiResponse);
        //                return Ok(result);
        //            }
        //            else
        //            {
        //                return NotFound();
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        //[HttpPost]
        //public async Task<IActionResult> UpdateVerifyFormResult(PostFormResult MD)
        //{
        //    try
        //    {
        //        using (var httpClient = new HttpClient(_clientHandler))
        //        {
        //            var response = await httpClient.PostAsJsonAsync(RenderAPIURL + "UpdateVerifyFormResult", MD);
        //            if (response.StatusCode.ToString() != "ServiceUnavailable")
        //            {
        //                string apiResponse = await response.Content.ReadAsStringAsync();
        //                return Ok(apiResponse);
        //            }
        //            else
        //            {
        //                return NotFound();
        //            }
        //        }
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //}
        public async Task<IActionResult> FormResultsView(int FormId, string name)
        {
            try
            {
                FormResult fr = new FormResult();
                fr.FormId = FormId;
                fr.FormName = name;
                using (var httpClient = new HttpClient(_clientHandler))
                {
                    var response = await httpClient.GetAsync(RenderAPIURL + "GetFormResults?Formid=" + FormId + "&FormName=" + name);
                    if (response.StatusCode.ToString() != "ServiceUnavailable")
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        FormResult result = JsonConvert.DeserializeObject<FormResult>(apiResponse);
                        if (result != null)
                            ViewBag.Result = result.ResultSet;
                        ViewBag.FormName = result.FormName;
                        return View();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<IActionResult> FormDataForEdit(int ResultId, string FormId)
        {
            try
            {
                ViewBag.FormId = FormId;
                FormResult fr = new FormResult();
                using (var httpClient = new HttpClient(_clientHandler))
                {
                    var response = await httpClient.GetAsync(RenderAPIURL + "GetFormDataForEdit?ResultId=" + ResultId + "&FormId=" + FormId);
                    if (response.StatusCode.ToString() != "ServiceUnavailable")
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        fr.ResultSet = JsonConvert.DeserializeObject<DataSet>(apiResponse);
                        ViewBag.Result = fr.ResultSet;
                        return View();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
        public async Task<IActionResult> DeleteFormData(int ResultId, string FormId)
        {
            try
            {
                ViewBag.FormId = FormId;
                FormResult fr = new FormResult();
                using (var httpClient = new HttpClient(_clientHandler))
                {
                    var response = await httpClient.GetAsync(RenderAPIURL + "DeleteFormData?ResultId=" + ResultId + "&FormId=" + FormId);
                    if (response.StatusCode.ToString() != "ServiceUnavailable")
                    {
                        string apiResponse = await response.Content.ReadAsStringAsync();
                        ViewBag.Result = apiResponse;
                        return Ok('1');
                    }
                    else
                    {
                        return NotFound();
                    }
                }
            }
            catch
            {
                throw;
            }
        }
    }
}
