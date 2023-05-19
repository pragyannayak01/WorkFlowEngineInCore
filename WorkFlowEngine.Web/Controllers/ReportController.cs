using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkFlowEngine.Web.Controllers
{
    public class ReportController : Controller
    {
        public IActionResult ReportBuilder()
        {
            return View();
        }
        public IActionResult ReportBuilderInner()
        {
            return View();
        }
    }
}
