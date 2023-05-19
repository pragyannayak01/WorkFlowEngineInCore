using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkFlowEngine.Domain.WFERender;
using WorkFlowEngine.IRepository.WFERender;

namespace WorkFlowEngine.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {       
        private IDFormRepository _iDFormRepository { get; }
        public AdminController(IDFormRepository iDFormRepository, ILogger<AdminController> logger)
        {
            
            _iDFormRepository = iDFormRepository;
        }
        [HttpGet("GetCreatedForm")]
        public IActionResult GetCreatedForm(int FormId, string ApplicantNo)
        {
            return Ok(_iDFormRepository.FillDataTable_New(FormId, ApplicantNo).Result);
        }
        
        [HttpPost("InsertResult")]
        public async Task<IActionResult> InsertResult(PostFormResult model)
        {
            try { 
            DformResultDomain res = new DformResultDomain();
            res.FORMID = Convert.ToInt32(model.postId);
            res.CREATEDDATE = DateTime.Now;
            res.RESULTJSON = model.formResult;
            res.DELETEDFLAG = 0;
            var result = model.formResult.Replace(", ", "&");
                var resultdata = result.Split("|");
            string[] resultObjects = result.Split(",");
            res.RESULTJSON = result;
            string Appno = string.Empty;
            //if (_iDFormRepository.IsRecordAlreadyExist(Convert.ToInt32(model.postId), model.formResult,null))
            //{
            //    Appno = "0";
            //}
            //else
            //{
                Appno = await _iDFormRepository.PostResults(res, 0, model.Status.ToString(), 0);
                    
            //}
                return Ok(Appno);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return BadRequest();
            }
            

        }
        [HttpPost("UpdateVerifyFormResult")]
        public async Task<IActionResult> UpdateVerifyFormResult(PostFormResult model)
        {
            try { 
            var Query = "";
            string[] columnlist = model.COLUMNNAME.Split(new char[] { '^' }, StringSplitOptions.RemoveEmptyEntries);
            if (model.COLUMNNAME != "")
            {
                foreach (string item in columnlist)
                {
                    Query = Query + item.Split("&")[0].Trim().Replace(" ","") + "='" + item.Split("&")[1].Trim() + "',";
                }
                Query = Query.Remove(Query.Length - 1);
            }
            string Appno = string.Empty;
            //if (_iDFormRepository.IsRecordAlreadyExist(Convert.ToInt32(model.FORMID), model.formResult, model.APPLICANT_NO)) 
            //{
            //    Appno = "0";
            //}
            //else
            //{
                Appno = await _iDFormRepository.UpdateResult(Query, model.FORMID, 0, model.APPLICANT_NO, "", 0, 0,model.formResult);
            //}
            return Ok(Appno);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return BadRequest();
            }
        }
        [HttpGet("GetFormResults")]
        public async Task<IActionResult> GetFormResults(int FormId, string FormName)
        {
            try { 
            FormResult result = new FormResult();
            result.FormId = FormId;
            result.FormName = FormName;
            result.ResultSet = _iDFormRepository.GetLastUpdatedData(FormId, 0, 0, 0, "", "", "");
            result.FormName = FormName;
            if(result.ResultSet.Tables[1].Rows.Count>0)
            {
                result.AppNo = result.ResultSet.Tables[1].Rows[0]["APPLICANT_NO"].ToString();
            }
            else
            {
                result.AppNo = null;
            }
            return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return BadRequest();
            }
        }
        [HttpGet("GetFormDataForEdit")]
        public async Task<IActionResult> GetFormDataForEdit(int ResultId, int FormId)
        {
            try { 
            var result = _iDFormRepository.GetEditResultData(ResultId, FormId);
            return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return BadRequest();
            }
        }
        [HttpGet("DeleteFormData")]
        public async Task<IActionResult> DeleteFormData(int ResultId, int FormId)
        {
            try { 
            var result = await _iDFormRepository.DeleteResult(ResultId, FormId);
            return Ok(result);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return BadRequest();
            }
        }
        [HttpPost("PostTest")]
        public ActionResult PostTest()
        {
            try { 
            List<Forms> forms = new List<Forms>();
            Forms forms1 = new Forms();
            forms1.SCHEME_NAME = "Abhijit";
            forms.Add(forms1);
            return Ok(forms);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return BadRequest();
            }
        }
        [HttpGet("PostTesta")]
        public ActionResult<IEnumerable<string>> PostTesta()
        {

            return new string[] { "value1", "value2" };
        }
        [Route("GetFormDetails")]
        public async Task<ActionResult<DFormDomain>> GetFormDetails()
        {
            try { 
            var products = await _iDFormRepository.GetFormName();
            return Ok(products);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex.StackTrace);
                return BadRequest();
            }
        }
    }
}
