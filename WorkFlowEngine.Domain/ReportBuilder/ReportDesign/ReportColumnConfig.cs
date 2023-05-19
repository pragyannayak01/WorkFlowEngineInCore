using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Domain.ReportBuilder.ReportDesign
{
    public class ReportColumnConfig
    {
        public int FormId { get; set; }
        public int ColumnId { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
        public DateTime UpdatedOn { get; set; }
        public int UpdatedBy { get; set; }
        public bool DeletedFlag { get; set; }
        public int ColumnConfigId { get; set; }
    }
}
