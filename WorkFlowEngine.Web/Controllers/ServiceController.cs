using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkFlowEngine.IRepository.WFEngine;

namespace WorkFlowEngine.Web.Controllers
{
    public class ServiceController : Controller
    {
        private IDFormRepository _iDFormRepository { get; }
        public ServiceController(IDFormRepository iDFormRepository)
        {
            _iDFormRepository = iDFormRepository;
        }
        [HttpGet("getSurvey")]
        public async Task<IActionResult> GetSurvey(string surveyId)
        {
            var UserId = HttpContext.Session.GetInt32("_UserId");
            if (!string.IsNullOrEmpty(UserId.ToString()))
            {
                //var db = new SessionStorage(HttpContext.Session);
                //return db.GetSurvey(surveyId);
                HttpContext.Session.SetString("_SurveyMode", "display");
                string firstJson = "";
                var survey = await _iDFormRepository.GetSurvey(surveyId);
                firstJson = survey == null ? "" : survey.JSONSTRING;
                var model = JsonConvert.DeserializeObject(firstJson);
                return Json(model);
            }
            else
            {
                return RedirectToAction("Logout", "Account");
            }
        }
    }

}
