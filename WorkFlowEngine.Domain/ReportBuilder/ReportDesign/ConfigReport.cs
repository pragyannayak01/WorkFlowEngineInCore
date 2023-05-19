using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Domain.ReportBuilder.ReportDesign
{
    public class ConfigReport
    {
        public int ReportId { get; set; }
        public string ReportType { get; set; }
        public int FormId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public bool DeletedFlag { get; set; }
        public List<ReportColumnConfig> ReportColumns { get; set; }
    }

}
