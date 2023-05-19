using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using WorkFlowEngine.Domain.WFEngine;

namespace WorkFlowEngine.Domain.WFERender
{
    public class DFormDomain
    {
        public int ID { get; set; }
        public int SchemeId { get; set; }
        public string FORMID { get; set; }
        public string JSONSTRING { get; set; }
        public Forms scheme { get; set; }
    }
}
