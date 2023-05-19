using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace WorkFlowEngine.Web.Areas.WFERender.Models
{
    public class FormResult
    {
        public int FormId { get; set; }
        public string FormName { get; set; }
        public DataSet ResultSet { get; set; }
        public string AppNo { get; set; }
    }
}
