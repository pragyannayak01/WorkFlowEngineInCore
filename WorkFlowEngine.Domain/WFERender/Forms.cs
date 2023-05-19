using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WorkFlowEngine.Domain.WFERender
{
    public class Forms
    {
        public int ID { get; set; }
        public string SCHEME_NAME { get; set; }

        public int DEPTID { get; set; }

        public string DEPTNAME { get; set; }
        public string TYPE { get; set; }

        public string GOAL_IMAGE { get; set; }
        public string SCRIPT_FILE { get; set; }

        public int TEMPLATEID { get; set; }
        public virtual DFormDomain Form { get; set; }
    }

}
