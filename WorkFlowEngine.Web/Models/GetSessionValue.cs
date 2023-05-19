using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkFlowEngine.Web.Models
{
    public class GetSessionValue
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        public GetSessionValue(IHttpContextAccessor httpContext)
        {
            httpContextAccessor = httpContext;
        }
        public string GetSessionData(string Svalue)
        {
            return httpContextAccessor.HttpContext.Session.GetString(Svalue);
        }
    }
}
