using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Domain.Account
{
    public class Login
    {
        public int INTUSERID { get; set; }
        public int LOGID { get; set; }
        public int INTLEVELDETAILID { get; set; }
        public int ROLEID { get; set; }
        public string VCHUSERNAME { get; set; }
        public string vchfullname { get; set; }
        public string nvchdesigname { get; set; }
        public string nvchlevelname { get; set; }
        public int intdesigid { get; set; }
        public string vchpassword { get; set; }
        public string Message { get; set; }
        public string MOBILENO { get; set; }
        public string strCaptcha { get; set; }
        public int LoginType { get; set; }
        public int UrlId { get; set; }
        public string Url { get; set; }
        public string DashboardName { get; set; }
        public int INTUSERTYPE { get; set; }
    }

}
