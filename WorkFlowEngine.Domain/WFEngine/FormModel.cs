using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Domain.WFEngine
{
    public class FormModel
    {
        public int ID { get; set; }
        public string JSONSTRING { get; set; }
        public int SCHEMEID { get; set; }
        public string FORMID { get; set; }
        public string REMARK { get; set; }
        public string SCHEME_NAME { get; set; }
        public string GOAL_IMAGE { get; set; }
        public string DEPTNAME { get; set; }
        public int DEPTID { get; set; }
        public string APPLICANT_NO { get; set; }
        public int UserId { get; set; }
        public int ALEVEL { get; set; }
        public int DesgId { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string MonthName { get; set; }
        public string QUATER { get; set; }
        public int ApprovedCount { get; set; }
        public int PendingCount { get; set; }
        public int RejectCount { get; set; }
        public int Division { get; set; }
        public int District { get; set; }
        public string DivisionName { get; set; }
        public string DistrictName { get; set; }
        public int Status { get; set; }
        public int Cnt { get; set; }
        public string FORMTYPE { get; set; }
    }
}
