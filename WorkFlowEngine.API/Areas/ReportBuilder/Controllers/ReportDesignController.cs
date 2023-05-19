using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkFlowEngine.IRepository.WFEngine.ReportDesign;

namespace WorkFlowEngine.API.Areas.ReportBuilder.Controllers
{
    [Area("ReportBuilder")]
    [Route("api/[controller]")]
    [ApiController]
    public class ReportDesignController : ControllerBase
    {
        private IFormRepository _iFormRepository { get; }
        public ReportDesignController(IFormRepository iFormRepository)
        {
            _iFormRepository = iFormRepository;
        }
        [HttpGet("GetallForms")]
        public ActionResult GetallForms()
        {
            string result = string.Empty;
            try
            {
                var res = _iFormRepository.GetAllForms(0);//getting all the sectors
                return Ok(res.Result.ToList());
            }
            catch (Exception ex)
            {
                //Logger.LogError(ex, ex.Message);
                result = ex.Message;
                return BadRequest(new { message = "Server Side Error, Contact to Admin", errordetail = result });
            }

            //return View();
        }
        [HttpGet("GetallFormColumnById")]

        public ActionResult GetallFormColumnById(int id)
        {
            string result = string.Empty;
            try
            {
                var res = _iFormRepository.GetallFormColumnById(id);
                if(res!=null)
                {
                    return Ok(res.Result.ToList());
                }
                else
                {
                    return NotFound();
                }
               
            }
            catch (Exception ex)
            {
                //Logger.LogError(ex, ex.Message);
                result = ex.Message;
                return BadRequest(new { message = "Server Side Error, Contact to Admin", errordetail = result });
            }

            //return View();
        }

    }
}
