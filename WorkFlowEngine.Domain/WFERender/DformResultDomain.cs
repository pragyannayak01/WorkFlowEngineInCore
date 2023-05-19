using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Domain.WFERender
{
    public class DformResultDomain
    {
        [Key]
        public int ID { get; set; }
        public int FORMID { get; set; }

        public string RESULTJSON { get; set; }

        public int ISAPPROVED { get; set; }

        public int DELETEDFLAG { get; set; }
        public DateTime CREATEDDATE { get; set; }
        //public string MONTH { get; set; }
        //public string YEAR { get; set; }
        public DFormDomain Form { get; set; }

        public DformAdminApproval approval { get; set; }
    }

}
