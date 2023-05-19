using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Domain.WFERender
{
    public class DformEditconfig
    {
        public int ID { get; set; }
        public int FORMID { get; set; }
        public int RESULTID { get; set; }
        public string COLUMNNAME { get; set; }
        public int USERID { get; set; }
        public int STATUS { get; set; }
        public string Remark { get; set; }
        public int APPROVESTATUS { get; set; }
        public string APPLICANT_NO { get; set; }
        public string FORMRESULT { get; set; }
    }
}
